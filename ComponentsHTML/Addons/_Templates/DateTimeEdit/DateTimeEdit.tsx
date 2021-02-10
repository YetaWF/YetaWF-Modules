/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF {
    export interface IConfigs {
        YetaWF_ComponentsHTML: YetaWF_ComponentsHTML.IPackageConfigs;
    }
}
namespace YetaWF_ComponentsHTML {

    export interface IPackageConfigs {
        SVG_fas_caret_left: string;
        SVG_fas_caret_right: string;
    }

    interface DateTimeEditSetup {
        Style: DateTimeStyleEnum;
        InitialCalendarDate: string;
        MinDate: string;
        MaxDate: string;
        MinTime: number;
        MaxTime: number;
        DateFormat: DateFormatEnum;
        TimeFormat: TimeFormatEnum;
        WeekDays2: string[];
        WeekDays: string[];
        Months: string[];
        TodayString: string;
        Today: string;
        BaseUtcOffset: number;
    }
    enum DateTimeStyleEnum {
        DateTime = 0,
        Date = 1,
        Time = 2,
    }
    enum DateFormatEnum {
        MMDDYYYY = 0,
        MMDDYYYYdash = 1,
        MMDDYYYYdot = 2,
        DDMMYYYY = 10,
        DDMMYYYYdash = 11,
        DDMMYYYYdot = 12,
        YYYYMMDD = 20,
        YYYYMMDDdash = 21,
        YYYYMMDDdot = 22,
    }
    enum TimeFormatEnum {
        HHMMAM = 0,
        HHMMAMdot = 1,
        HHMM = 10,
        HHMMdot = 11,
        HHMMSSAM = 20,
        HHMMSSAMdot = 21,
        HHMMSS = 30,
        HHMMSSdot = 31,
    }
    interface NumberToken {
        text: string;
        token: number;
        success: boolean;
    }
    interface TextToken {
        text: string;
        token: string;
        success: boolean;
    }
    interface DateToken {
        text: string;
        token: Date;
        success: boolean;
    }
    interface TimeToken {
        text: string;
        token: number;
        success: boolean;
    }

    export class DateTimeEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_datetime";
        public static readonly SELECTOR: string = ".yt_datetime.t_edit";
        public static readonly EVENTCHANGE: string = "datetime_change";

        public static readonly TIMEPOPUPID: string = "yt_datetime_popup";
        public static readonly CALENDARPOPUPID: string = "yt_datetime_calendarpopup";
        public static readonly YEARPOPUPID: string = "yt_datetime_popup";
        public static readonly MONTHPOPUPID: string = "yt_datetime_popup";
        private static readonly WARNDELAY:number = 300;

        private Setup: DateTimeEditSetup;
        private InputHidden: HTMLInputElement;
        private InputControl: HTMLInputElement;
        private TimePopup: HTMLElement | null = null;
        private CalendarPopup: HTMLElement | null = null;
        private YearPopup: HTMLElement | null = null;
        private MonthPopup: HTMLElement | null = null;
        private tempTimeSelectedValue: number|null = null;
        private tempCalSelectedValueUTC: string|null = null;
        private tempArrow: HTMLAnchorElement|null = null;
        private tempYearSelectedValue: number|null = null;
        private tempMonthSelectedValue: number|null = null;
        private IgnoreResizeUntil: number = 0;

        constructor(controlId: string, setup: DateTimeEditSetup) {
            super(controlId, DateTimeEditComponent.TEMPLATE, DateTimeEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: DateTimeEditComponent.EVENTCHANGE,
                GetValue: (control: DateTimeEditComponent): string | null => {
                    return control.valueText;
                },
                Enable: (control: DateTimeEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    YetaWF_BasicsImpl.elementEnableToggle(control.InputHidden, enable);
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            });
            this.Setup = setup;
            this.InputHidden = $YetaWF.getElement1BySelector("input[type='hidden']", [this.Control]) as HTMLInputElement;
            this.InputControl = $YetaWF.getElement1BySelector("input[type='text']", [this.Control]) as HTMLInputElement;

            let warn = <div class="t_warn" style="display:none"></div>;
            warn.innerHTML = YConfigs.YetaWF_ComponentsHTML.SVG_fas_exclamation_triangle;
            this.InputControl.insertAdjacentElement("afterend", warn);

            this.SelectedUTC = new Date(this.Setup.InitialCalendarDate || this.Setup.Today);
            this.TempCalCurrentDateUTC = new Date(this.SelectedUTC);

            $YetaWF.registerEventHandler(this.Control, "mousedown", ".t_time", (ev: Event): boolean => {
                if (this.enabled) {
                    if (this.TimePopup) {
                        this.closeTimeList();
                    } else {
                        this.InputControl.focus();
                        // ignore resize events as we're receiving the focus. On mobile this may result in resize events due to keyboard appearing.
                        this.IgnoreResizeUntil = Date.now() + 1000;
                        this.openTimeList();
                    }
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.Control, "mousedown", ".t_date", (ev: Event): boolean => {
                if (this.enabled) {
                    if (this.CalendarPopup) {
                        this.closeCalendar();
                    } else {
                        this.InputControl.focus();
                        // ignore resize events as we're receiving the focus. On mobile this may result in resize events due to keyboard appearing.
                        this.IgnoreResizeUntil = Date.now() + 1000;
                        this.openCalendar();
                    }
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.InputControl, "focusin", null, (ev: FocusEvent): boolean => {
                if (this.enabled) {
                    $YetaWF.elementRemoveClass(this.Control, "t_focused");
                    $YetaWF.elementAddClass(this.Control, "t_focused");
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.InputControl, "focusout", null, (ev: FocusEvent): boolean => {
                $YetaWF.elementRemoveClass(this.Control, "t_focused");
                this.close();
                if (this.validateInput())
                    this.sendChangeEvent();
                else {
                    this.flashError();
                    this.setHiddenInvalid(this.InputControl.value);
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.InputControl, "keydown", null, (ev: KeyboardEvent): boolean => {
                let key = ev.key;
                if (this.TimePopup) {
                    if (key === "ArrowDown" || key === "ArrowRight") {
                        this.setSelectedIndex(this.TimePopup, this.getSelectedIndex(this.TimePopup) + 1);
                        return false;
                    } else if (key === "ArrowUp" || key === "ArrowLeft") {
                        this.setSelectedIndex(this.TimePopup, this.getSelectedIndex(this.TimePopup) - 1);
                        return false;
                    } else if (key === "Home") {
                        this.setSelectedIndex(this.TimePopup, 0);
                        return false;
                    } else if (key === "End") {
                        this.setSelectedIndex(this.TimePopup, 9999999);
                        return false;
                    } else if (key === "Escape") {
                        this.closeTimeList();
                        return false;
                    } else if (key === "Enter") {
                        let time = this.getSelectedValue(this.TimePopup);
                        if (time)
                            this.TimeSelectedUser = Number(time);
                        this.closeTimeList();
                        this.sendChangeEvent();
                        return false;
                    }
                } else if (this.CalendarPopup) {
                    if (this.MonthPopup) {
                        if (key === "ArrowDown" || key === "ArrowRight") {
                            this.setSelectedIndex(this.MonthPopup, this.getSelectedIndex(this.MonthPopup) + 1);
                            return false;
                        } else if (key === "ArrowUp" || key === "Up" || key === "ArrowLeft" || key === "Left") {
                            this.setSelectedIndex(this.MonthPopup, this.getSelectedIndex(this.MonthPopup) - 1);
                            return false;
                        } else if (key === "Home") {
                            this.setSelectedIndex(this.MonthPopup, 0);
                            return false;
                        } else if (key === "End") {
                            this.setSelectedIndex(this.MonthPopup, 9999999);
                            return false;
                        } else if (key === "Escape") {
                            this.closeTimeList();
                            return false;
                        } else if (key === "Enter") {
                            let month = this.getSelectedValue(this.MonthPopup);
                            let date = this.TempCalCurrentDateUser;
                            date.setUTCMonth(Number(month));
                            let tbody = $YetaWF.getElement1BySelector(".t_calendarbody tbody", [this.CalendarPopup]);
                            this.buildCalendarMonthPage(tbody, date);
                            this.updateCalendarTitle();
                            this.closeMonthList();
                            return false;
                        }
                    } else if (this.YearPopup) {
                        if (key === "ArrowDown" || key === "ArrowRight") {
                            this.setSelectedIndex(this.YearPopup, this.getSelectedIndex(this.YearPopup) + 1);
                            return false;
                        } else if (key === "ArrowUp" || key === "Up" || key === "ArrowLeft" || key === "Left") {
                            this.setSelectedIndex(this.YearPopup, this.getSelectedIndex(this.YearPopup) - 1);
                            return false;
                        } else if (key === "Home") {
                            this.setSelectedIndex(this.YearPopup, 0);
                            return false;
                        } else if (key === "End") {
                            this.setSelectedIndex(this.YearPopup, 9999999);
                            return false;
                        } else if (key === "Escape") {
                            this.closeTimeList();
                            return false;
                        } else if (key === "Enter") {
                            let year = this.getSelectedValue(this.YearPopup);
                            let date = this.TempCalCurrentDateUser;
                            date.setUTCFullYear(Number(year));
                            let tbody = $YetaWF.getElement1BySelector(".t_calendarbody tbody", [this.CalendarPopup!]);
                            this.buildCalendarMonthPage(tbody, date);
                            this.updateCalendarTitle();
                            this.closeYearList();
                            return false;
                        }
                    } else {
                        let date = this.TempCalCurrentDateUser;
                        if (ev.altKey) {
                            if (key === "ArrowUp") {
                                this.close();
                                return false;
                            }
                        } else {
                            if (key === "Enter") {
                                this.TempCalCurrentDateUser = new Date(date);
                                this.dateSelectedValueUTC = this.TempCalCurrentDateUTC;
                                this.close();
                                this.sendChangeEvent();
                                return false;
                            } else if (key === "ArrowUp") {
                                date.setUTCDate(date.getUTCDate() - 7 );
                            } else if (key === "ArrowDown") {
                                date.setUTCDate(date.getUTCDate() + 7 );
                            } else if (key === "ArrowLeft") {
                                date.setUTCDate(date.getUTCDate() - 1 );
                            } else if (key === "ArrowRight") {
                                date.setUTCDate(date.getUTCDate() + 1 );
                            } else if (key === "PageUp") {
                                let dom = date.getUTCDate();// day of month current month
                                date.setUTCDate(0);// last day of last month
                                let daysInMonth = date.getUTCDate();
                                if (dom >= daysInMonth)
                                    dom = daysInMonth;
                                date.setUTCDate(dom);
                            } else if (key === "PageDown") {
                                let dom = date.getUTCDate();// day of month current month
                                date.setUTCDate(1);// first day of this month
                                date.setUTCMonth(date.getUTCMonth() + 2);// first day of next-next month
                                date.setUTCDate(0);// last day of next month
                                let daysInMonth = date.getUTCDate();
                                if (dom >= daysInMonth)
                                    dom = daysInMonth;
                                date.setUTCDate(dom);
                            } else if (key === "Home") {
                                date.setUTCDate(1);
                            } else if (key === "End") {
                                date.setUTCMonth(date.getUTCMonth() + 1 );
                                date.setUTCDate(date.getUTCDate() - 1 );
                            } else
                                return true;
                            let tbody = $YetaWF.getElement1BySelector(".t_calendarbody tbody", [this.CalendarPopup]);
                            this.buildCalendarMonthPage(tbody, date);
                            this.updateCalendarTitle();
                            return false;
                        }
                    }
                } else {
                    if (ev.altKey) {
                        if (key === "ArrowDown") {
                            this.openCalendar();
                            return false;
                        }
                    }
                }
                return true;
            });
        }

        public sendChangeEvent(): void {
            $YetaWF.sendCustomEvent(this.Control, DateTimeEditComponent.EVENTCHANGE);
            FormsSupport.validateElement(this.InputHidden);
        }

        private validateInput(): boolean {
            // validate input control and update hidden field
            let text = this.InputControl.value.trim();
            if (text.length === 0) return true;
            switch (this.Setup.Style) {
                default:
                case DateTimeStyleEnum.DateTime: {
                    let dt = this.extractInputDate(text);
                    if (!dt.success) return false;
                    text = dt.text;
                    let wt = this.getWhitespace(text);
                    if (!wt.success) return false;
                    text = wt.text;
                    let tt = this.extractInputTime(text);
                    if (!tt.success) return false;
                    text = tt.text;
                    if (text.length > 0) return false;
                    this.SelectedUser = dt.token;
                    this.TimeSelectedUser = tt.token;
                    break;
                }
                case DateTimeStyleEnum.Date:
                    let dt = this.extractInputDate(text);
                    if (!dt.success) return false;
                    text = dt.text;
                    if (text.length > 0) return false;
                    this.SelectedUser = dt.token;
                    break;
                case DateTimeStyleEnum.Time:
                    let tt = this.extractInputTime(text);
                    if (!tt.success) return false;
                    text = tt.text;
                    if (text.length > 0) return false;
                    this.TimeSelectedUser = tt.token;
                    break;
            }
            return true;
        }

        private extractInputDate(text: string): DateToken {
            let dt: DateToken = { text: text, token: new Date(), success: false };
            let m: number, d: number, y: number;
            let nt: NumberToken;
            let rt: TextToken;
            let sep = "/";
            switch (this.Setup.DateFormat) {
                default:
                case DateFormatEnum.MMDDYYYYdash:
                case DateFormatEnum.MMDDYYYYdot:
                case DateFormatEnum.MMDDYYYY:
                    if (this.Setup.DateFormat === DateFormatEnum.MMDDYYYYdash) sep = "-";
                    else if (this.Setup.DateFormat === DateFormatEnum.MMDDYYYYdot) sep = ".";
                    nt = this.getNumberToken(text);
                    if (!nt.success) return dt;
                    text = nt.text;
                    m = nt.token;
                    rt = this.getRequired(text, sep);
                    if (!rt.success) return dt;
                    text = rt.text;
                    nt = this.getNumberToken(text);
                    if (!nt.success) return dt;
                    text = nt.text;
                    d = nt.token;
                    rt = this.getRequired(text, sep);
                    if (!rt.success) return dt;
                    text = rt.text;
                    nt = this.getNumberToken(text);
                    if (!nt.success) return dt;
                    text = nt.text;
                    y = nt.token;
                    break;

                case DateFormatEnum.DDMMYYYY:
                case DateFormatEnum.DDMMYYYYdash:
                case DateFormatEnum.DDMMYYYYdot:
                    if (this.Setup.DateFormat === DateFormatEnum.DDMMYYYYdash) sep = "-";
                    else if (this.Setup.DateFormat === DateFormatEnum.DDMMYYYYdot) sep = ".";
                    nt = this.getNumberToken(text);
                    if (!nt.success) return dt;
                    text = nt.text;
                    d = nt.token;
                    rt = this.getRequired(text, sep);
                    if (!rt.success) return dt;
                    text = rt.text;
                    nt = this.getNumberToken(text);
                    if (!nt.success) return dt;
                    text = nt.text;
                    m = nt.token;
                    rt = this.getRequired(text, sep);
                    if (!rt.success) return dt;
                    text = rt.text;
                    nt = this.getNumberToken(text);
                    if (!nt.success) return dt;
                    text = nt.text;
                    y = nt.token;
                    break;

                case DateFormatEnum.YYYYMMDD:
                case DateFormatEnum.YYYYMMDDdash:
                case DateFormatEnum.YYYYMMDDdot:
                    if (this.Setup.DateFormat === DateFormatEnum.YYYYMMDDdash) sep = "-";
                    else if (this.Setup.DateFormat === DateFormatEnum.YYYYMMDDdot) sep = ".";
                    nt = this.getNumberToken(text);
                    if (!nt.success) return dt;
                    text = nt.text;
                    y = nt.token;
                    rt = this.getRequired(text, sep);
                    if (!rt.success) return dt;
                    text = rt.text;
                    nt = this.getNumberToken(text);
                    if (!nt.success) return dt;
                    text = nt.text;
                    m = nt.token;
                    rt = this.getRequired(text, sep);
                    if (!rt.success) return dt;
                    text = rt.text;
                    nt = this.getNumberToken(text);
                    if (!nt.success) return dt;
                    text = nt.text;
                    d = nt.token;
                    break;
            }
            if (d < 1 || d > 31) return dt;
            if (m < 1 || m > 12) return dt;
            let minDate = new Date(this.Setup.MinDate);
            let maxDate = new Date(this.Setup.MaxDate);
            if (y < minDate.getUTCFullYear() || y > maxDate.getUTCFullYear()) return dt;
            dt.token = new Date(y, m-1, d);
            dt.token.setUTCSeconds(dt.token.getUTCSeconds() - this.Setup.BaseUtcOffset);
            dt.text = text;
            dt.success = true;
            return dt;
        }
        private extractInputTime(text: string): TimeToken {
            let tt: TimeToken = { text: text, token: 0, success: false };
            let h: number, m: number, ampm: string;
            let nt: NumberToken;
            let rt: TextToken;
            let wt: TextToken;
            let sep = ":";
            switch (this.Setup.TimeFormat) {
                default:
                case TimeFormatEnum.HHMMAM:
                case TimeFormatEnum.HHMMSSAM:
                case TimeFormatEnum.HHMMAMdot:
                case TimeFormatEnum.HHMMSSAMdot: {
                    if (this.Setup.TimeFormat === TimeFormatEnum.HHMMAMdot) sep = ".";
                    nt = this.getNumberToken(text);
                    if (!nt.success) return tt;
                    text = nt.text;
                    h = nt.token;
                    if (h < 0 || h > 12) return tt;
                    rt = this.getRequired(text, sep);
                    if (!rt.success) return tt;
                    text = rt.text;
                    nt = this.getNumberToken(text);
                    if (!nt.success) return tt;
                    text = nt.text;
                    m = nt.token;
                    if (m < 0 || m > 59) return tt;
                    wt = this.getWhitespace(text);
                    if (!wt.success) return tt;
                    text = wt.text;
                    ampm = "A";
                    if (text.length > 0) {
                        rt = this.getRequired(text, "A");
                        if (!rt.success) {
                            rt = this.getRequired(text, "P");
                            if (!rt.success)
                                return tt;
                        }
                        text = rt.text;
                        ampm = rt.token;
                        rt = this.getRequired(text, "M");
                        if (!rt.success) return tt;
                        text = rt.text;
                    }
                    if (ampm === "P" && h < 12) h += 12;
                    let val = (h * 60 + m) % (24*60);
                    if (val < this.Setup.MinTime || val > this.Setup.MaxTime) return tt;
                    tt.text = text;
                    tt.token = val;
                    tt.success = true;
                    return tt;
                }
                case TimeFormatEnum.HHMM:
                case TimeFormatEnum.HHMMSS:
                case TimeFormatEnum.HHMMdot:
                case TimeFormatEnum.HHMMSSdot: {
                    if (this.Setup.TimeFormat === TimeFormatEnum.HHMMdot) sep = ".";
                    nt = this.getNumberToken(text);
                    if (!nt.success) return tt;
                    text = nt.text;
                    h = nt.token;
                    if (h < 0 || h >= 24) return tt;
                    rt = this.getRequired(text, sep);
                    if (!rt.success) return tt;
                    text = rt.text;
                    nt = this.getNumberToken(text);
                    if (!nt.success) return tt;
                    text = nt.text;
                    m = nt.token;
                    if (m < 0 || m > 59) return tt;
                    wt = this.getWhitespace(text);
                    if (!wt.success) return tt;
                    text = wt.text;
                    let val = (h * 60 + m) % (24*60);
                    if (val < this.Setup.MinTime || val > this.Setup.MaxTime) return tt;
                    tt.text = text;
                    tt.token = val;
                    tt.success = true;
                    return tt;
                }
            }
        }
        private getNumberToken(text: string): NumberToken {
            let t: NumberToken = { text: text, token: 0, success: false };
            let num = 0;
            let success = false;
            while (text.length > 0) {
                let n = text[0];
                if (n >= "0" && n <= "9") {
                    success = true;
                    num = num * 10 + Number(n);
                    text = text.substr(1);
                } else
                    break;
            }
            t.text = text;
            t.token = num;
            t.success = success;
            return t;
        }
        private getRequired(text: string, required: string): TextToken {
            let t: TextToken = { text: text, token: "", success: false };
            if (required.length !== 1) throw "required must be exact 1 character";
            if (text.length === 0 || (text[0].toLowerCase() !== required.toLowerCase() && text[0] !== required)) return t;
            t.text = text = text.length > 0 ? text.substr(1): "";
            t.token = required.toUpperCase();
            t.success = true;
            return t;
        }
        private getWhitespace(text: string): TextToken {
            let t: TextToken = { text: text, token: "", success: false };
            let success = false;
            while (text.length > 0) {
                if (text[0] !== " ") {
                    if (success)
                        break;
                    return t;
                }
                success = true;
                text = text.substr(1);
            }
            t.text = text;
            t.token = " ";
            t.success = true;
            return t;
        }

        private flashError(): void {
            let warn = $YetaWF.getElement1BySelector(".t_warn", [this.Control]);
            warn.style.display = "";
            setTimeout(():void => {
                warn.style.display = "none";
            }, DateTimeEditComponent.WARNDELAY);
        }

        private openTimeList(): void {

            this.closeCalendar();
            this.closeTimeList();

            let times: number[] = [];
            for (let i = 0 ; i < 24*60 ; i += 30) {
                if (this.Setup.MinTime <= i && i <= this.Setup.MaxTime)
                    times.push(i);
            }
            this.TimePopup =
                <div id={DateTimeEditComponent.TIMEPOPUPID} data-owner={this.ControlId} aria-hidden="false">
                    <div class="t_container" data-role="popup" aria-hidden="false">
                        <div class="t_scroller" unselectable="on">
                            <ul unselectable="on" class="t_list" tabindex="-1" aria-hidden="true" aria-live="off" data-role="staticlist" role="listbox">
                            </ul>
                        </div>
                    </div>
                </div> as HTMLElement;

            let ul = $YetaWF.getElement1BySelector("ul", [this.TimePopup]);
            const len = times.length;
            let html = "";
            let selValue = this.TimeSelectedUser;
            for (let i = 0; i < len; ++i) {
                let val = times[i];
                let o = this.getFormattedTime(val);
                let extra = "";
                if (val === selValue)
                    extra = " t_selected";
                html += `<li tabindex="-1" role="option" unselectable="on" class="t_item${extra}" data-value="${val}">${o}</li>`; //Format
            }
            ul.innerHTML = html;

            let style = window.getComputedStyle(this.Control);
            this.TimePopup.style.font = style.font;
            this.TimePopup.style.fontStyle = style.fontStyle;
            this.TimePopup.style.fontWeight = style.fontWeight;
            this.TimePopup.style.fontSize = style.fontSize;
            let ctrlRect = this.Control.getBoundingClientRect();
            this.TimePopup.style.width = `${ctrlRect.width}px`;

            $YetaWF.positionLeftAlignedBelow(this.Control, this.TimePopup);

            document.body.appendChild(this.TimePopup);
            this.Control.setAttribute("aria-expanded", "true");

            let sel =  $YetaWF.getElement1BySelectorCond(".t_list .t_selected", [this.TimePopup]);
            if (sel) {
                let selRect = sel.getBoundingClientRect();
                let list =  $YetaWF.getElement1BySelector(".t_scroller", [this.TimePopup]);
                let listRect = list.getBoundingClientRect();
                list.scrollTop = selRect.top - listRect.top;
                //sel.scrollIntoView({ behavior: "auto", block: "nearest", inline: "start" }); // causes outer page jump
            }

            $YetaWF.registerEventHandler(this.TimePopup, "mousedown", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem as HTMLLIElement;
                let value = Number($YetaWF.getAttribute(li, "data-value"));
                this.tempTimeSelectedValue = value;
                return false;
            });
            $YetaWF.registerEventHandler(this.TimePopup, "mouseup", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem as HTMLLIElement;
                let value = Number($YetaWF.getAttribute(li, "data-value"));
                if (this.tempTimeSelectedValue === value) {
                    this.TimeSelectedUser = this.tempTimeSelectedValue;
                    this.tempTimeSelectedValue = null;
                    this.closeTimeList();
                    this.sendChangeEvent();
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.TimePopup, "mouseover", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "t_hover");
                $YetaWF.elementAddClass(li, "t_hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.TimePopup, "mouseout", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "t_hover");
                return true;
            });
        }
        private closeTimeList(): void {
            if (this.TimePopup) {
                this.TimePopup.remove();
                this.TimePopup = null;
                this.Control.setAttribute("aria-expanded", "false");
            }
        }

        private openCalendar(): void {

            this.closeTimeList();
            this.closeCalendar();

            let month = this.Setup.Months[this.TempCalCurrentDateUser.getUTCMonth()];
            let year = this.TempCalCurrentDateUser.getUTCFullYear().toFixed(0);

            this.CalendarPopup =
                <div id={DateTimeEditComponent.CALENDARPOPUPID} data-owner={this.ControlId} aria-hidden="false">
                    <div class="t_container" data-role="popup" aria-hidden="false">
                        <div data-role="calendar" class="t_calendar">
                            <div class="t_header">
                                <a href="#" data-action="prev" role="button" class="t_prev" aria-label="Previous" aria-disabled="false"></a>
                                <div class="t_title" aria-disabled="false">
                                    <span class="t_month">{month}</span> <span class="t_year">{year}</span>
                                </div>
                                <a href="#" data-action="next" role="button" class="t_next" aria-label="Next" aria-disabled="false"></a>
                            </div>
                            <div class="t_calendarbody">
                                <table tabindex="0" role="grid">
                                    <thead>
                                        <tr role="row">
                                            <th scope="col" data-tooltip={this.Setup.WeekDays[0]}>{this.Setup.WeekDays2[0]}</th>
                                            <th scope="col" data-tooltip={this.Setup.WeekDays[1]}>{this.Setup.WeekDays2[1]}</th>
                                            <th scope="col" data-tooltip={this.Setup.WeekDays[2]}>{this.Setup.WeekDays2[2]}</th>
                                            <th scope="col" data-tooltip={this.Setup.WeekDays[3]}>{this.Setup.WeekDays2[3]}</th>
                                            <th scope="col" data-tooltip={this.Setup.WeekDays[4]}>{this.Setup.WeekDays2[4]}</th>
                                            <th scope="col" data-tooltip={this.Setup.WeekDays[5]}>{this.Setup.WeekDays2[5]}</th>
                                            <th scope="col" data-tooltip={this.Setup.WeekDays[6]}>{this.Setup.WeekDays2[6]}</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                    </tbody>
                                </table>
                            </div>
                            <div class="t_footer">
                                <a href="#" class="t_today">{this.Setup.TodayString}</a>
                            </div>
                        </div>
                    </div>
                </div> as HTMLElement;

            let prev = $YetaWF.getElement1BySelector(".t_prev", [this.CalendarPopup]);
            prev.innerHTML = YConfigs.YetaWF_ComponentsHTML.SVG_fas_caret_left;
            let next = $YetaWF.getElement1BySelector(".t_next", [this.CalendarPopup]);
            next.innerHTML = YConfigs.YetaWF_ComponentsHTML.SVG_fas_caret_right;

            let tbody = $YetaWF.getElement1BySelector(".t_calendarbody tbody", [this.CalendarPopup]);
            this.buildCalendarMonthPage(tbody, this.TempCalCurrentDateUser || new Date());

            let style = window.getComputedStyle(this.Control);
            this.CalendarPopup.style.font = style.font;
            this.CalendarPopup.style.fontStyle = style.fontStyle;
            this.CalendarPopup.style.fontWeight = style.fontWeight;
            this.CalendarPopup.style.fontSize = style.fontSize;

            $YetaWF.positionLeftAlignedBelow(this.Control, this.CalendarPopup);

            document.body.appendChild(this.CalendarPopup);
            this.Control.setAttribute("aria-expanded", "true");

            $YetaWF.registerEventHandler(this.CalendarPopup, "mouseover", "table td", (ev: MouseEvent): boolean => {
                let td = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(td, "t_hover");
                $YetaWF.elementAddClass(td, "t_hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "mouseout", "table td", (ev: MouseEvent): boolean => {
                let td = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(td, "t_hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "mousedown", "table td .t_link", (ev: MouseEvent): boolean => {
                let anchor = ev.__YetaWFElem as HTMLAnchorElement;
                let value = $YetaWF.getAttribute(anchor, "data-value");
                this.tempCalSelectedValueUTC = value;
                return false;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "mouseup", "table td .t_link", (ev: MouseEvent): boolean => {
                let anchor = ev.__YetaWFElem as HTMLAnchorElement;
                let value = $YetaWF.getAttribute(anchor, "data-value");
                if (this.tempCalSelectedValueUTC === value) {
                    this.dateSelectedValueUTC = this.TempCalCurrentDateUTC = new Date(this.tempCalSelectedValueUTC);
                    this.tempCalSelectedValueUTC = null;
                    this.close();
                    this.sendChangeEvent();
                }
                return false;
            });

            $YetaWF.registerEventHandler(this.CalendarPopup, "mousedown", ".t_header .t_prev svg, .t_header .t_next svg", (ev: MouseEvent): boolean => {
                let anchor = $YetaWF.elementClosest(ev.__YetaWFElem, "a") as HTMLAnchorElement;
                this.tempArrow = anchor;
                this.closeMonthList();
                this.closeYearList();
                return false;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "mouseup", ".t_header .t_prev svg, .t_header .t_next svg", (ev: MouseEvent): boolean => {
                let anchor = $YetaWF.elementClosest(ev.__YetaWFElem, "a") as HTMLAnchorElement;
                if (this.tempArrow === anchor) {
                    let date = this.TempCalCurrentDateUser;
                    if ($YetaWF.elementHasClass(anchor, "t_prev")) {
                        date.setUTCMonth(date.getUTCMonth()-1);
                    } else {
                        date.setUTCMonth(date.getUTCMonth()+1);
                    }
                    let tbody = $YetaWF.getElement1BySelector(".t_calendarbody tbody", [this.CalendarPopup!]);
                    this.buildCalendarMonthPage(tbody, date);
                    this.updateCalendarTitle();
                    this.tempArrow = null;
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "click", "table td .t_link, .t_header .t_prev svg, .t_header .t_next svg", (ev: MouseEvent): boolean => {
                return false;
            });

            $YetaWF.registerEventHandler(this.CalendarPopup, "click", ".t_footer .t_today", (ev: MouseEvent): boolean => {
                this.dateSelectedValueUTC = new Date(this.Setup.Today);
                this.tempCalSelectedValueUTC = null;
                this.close();
                this.sendChangeEvent();
                return false;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "mousedown", ".t_header .t_month", (ev: Event): boolean => {
                if (this.MonthPopup) {
                    this.closeMonthList();
                } else {
                    this.openMonthList();
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "mousedown", ".t_header .t_year", (ev: Event): boolean => {
                if (this.YearPopup) {
                    this.closeYearList();
                } else {
                    this.openYearList();
                }
                return false;
            });
        }
        private closeCalendar(): void {
            if (this.CalendarPopup) {
                this.CalendarPopup.remove();
                this.CalendarPopup = null;
                this.Control.setAttribute("aria-expanded", "false");
            }
        }
        private buildCalendarMonthPage(tbody: HTMLElement, startDate: Date): void {

            this.TempCalCurrentDateUser = new Date(startDate);

            let date = new Date(startDate);
            let startMonth = startDate.getUTCMonth();
            date.setUTCDate(1);// set the first day
            date.setUTCDate(- date.getUTCDay() + 1); // get to the start of the week (Sunday)

            let today = new Date(this.Setup.Today);
            today.setUTCSeconds(today.getUTCSeconds() + this.Setup.BaseUtcOffset);

            tbody.innerHTML = "";

            let selDate = this.TempCalCurrentDateUser;
            for (let last = false ; !last ; ) {

                let row = <tr role="row"></tr> as HTMLTableRowElement;
                for (let day = 0 ; day < 7 ; ++day ) {
                    let css = "";
                    if (day === 0 || day === 6) // Saturday, Sunday
                        css += " t_weekend";
                    if (date < startDate && date.getUTCMonth() !== startMonth)
                        css += " t_othermonth";
                    else if (date > startDate && date.getUTCMonth() !== startMonth) {
                        css += " t_othermonth";
                        last = true;
                    }
                    if (date.getUTCFullYear() === selDate.getUTCFullYear() && date.getUTCMonth() === selDate.getUTCMonth() && date.getUTCDate() === selDate.getUTCDate())
                        css += " t_selected";
                    if (date.getUTCDate() === today.getUTCDate() && date.getUTCMonth() === today.getUTCMonth() && date.getUTCFullYear() === today.getUTCFullYear())
                        css += " t_today";

                    css = css.trim();
                    let tt = this.getLongFormattedDate(date);

                    let cell =
                        <td class={css} role="gridcell">
                            <a tabindex="-1" class="t_link" href="#" data-value={this.UserToUTC(date).toISOString()} title={tt}>{date.getUTCDate()}</a>
                        </td> as HTMLTableCellElement;

                    row.appendChild(cell);

                    date.setUTCDate(date.getUTCDate()+1);
                }
                tbody.appendChild(row);

                if (!last && date.getUTCMonth() !== startMonth)
                    last = true;
            }
        }

        private openYearList(): void {

            this.closeMonthList();
            this.closeYearList();
            if (!this.CalendarPopup) return;

            this.YearPopup =
                <div id={DateTimeEditComponent.YEARPOPUPID} data-owner={this.ControlId} aria-hidden="false">
                    <div class="t_container" data-role="popup" aria-hidden="false">
                        <div class="t_scroller" unselectable="on">
                            <ul unselectable="on" class="t_list" tabindex="-1" aria-hidden="true" aria-live="off" data-role="staticlist" role="listbox">
                            </ul>
                        </div>
                    </div>
                </div> as HTMLElement;

            let minDate = new Date(this.Setup.MinDate);
            let maxDate = new Date(this.Setup.MaxDate);

            let ul = $YetaWF.getElement1BySelector("ul", [this.YearPopup]);
            let startYear = minDate.getUTCFullYear();
            let endYear = maxDate.getUTCFullYear();
            let selValue = this.TempCalCurrentDateUser.getUTCFullYear();
            let html = "";
            for (let i = startYear; i <= endYear; ++i) {
                let extra = "";
                if (i === selValue)
                    extra = " t_selected";
                html += `<li tabindex="-1" role="option" unselectable="on" class="t_item${extra}" data-value="${i}">${i}</li>`; //Format
            }
            ul.innerHTML = html;

            let yearSel = $YetaWF.getElement1BySelector(".t_header .t_year", [this.CalendarPopup]);
            let style = window.getComputedStyle(yearSel);
            this.YearPopup.style.font = style.font;
            this.YearPopup.style.fontStyle = style.fontStyle;
            this.YearPopup.style.fontWeight = style.fontWeight;
            this.YearPopup.style.fontSize = style.fontSize;

            $YetaWF.positionLeftAlignedBelow(yearSel, this.YearPopup);

            document.body.appendChild(this.YearPopup);
            let sel =  $YetaWF.getElement1BySelectorCond(".t_list .t_selected", [this.YearPopup]);
            if (sel) {
                let selRect = sel.getBoundingClientRect();
                let list =  $YetaWF.getElement1BySelector(".t_scroller", [this.YearPopup]);
                let listRect = list.getBoundingClientRect();
                list.scrollTop = selRect.top - listRect.top;
                //sel.scrollIntoView({ behavior: "auto", block: "nearest", inline: "start" }); // causes outer page jump
            }

            $YetaWF.registerEventHandler(this.YearPopup, "mousedown", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem as HTMLLIElement;
                let value = Number($YetaWF.getAttribute(li, "data-value"));
                this.tempYearSelectedValue = value;
                return false;
            });
            $YetaWF.registerEventHandler(this.YearPopup, "mouseup", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem as HTMLLIElement;
                let value = Number($YetaWF.getAttribute(li, "data-value"));
                if (this.tempYearSelectedValue === value) {
                    let date = this.TempCalCurrentDateUser;
                    date.setUTCFullYear(value);
                    let tbody = $YetaWF.getElement1BySelector(".t_calendarbody tbody", [this.CalendarPopup!]);
                    this.buildCalendarMonthPage(tbody, date);
                    this.updateCalendarTitle();
                    this.tempYearSelectedValue = null;
                    this.closeYearList();
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.YearPopup, "click", "ul li", (ev: MouseEvent): boolean => {
                return false;
            });
            $YetaWF.registerEventHandler(this.YearPopup, "mouseover", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "t_hover");
                $YetaWF.elementAddClass(li, "t_hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.YearPopup, "mouseout", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "t_hover");
                return true;
            });
        }
        private closeYearList(): void {
            if (this.YearPopup) {
                this.YearPopup.remove();
                this.YearPopup = null;
            }
        }


        private openMonthList(): void {

            this.closeMonthList();
            this.closeYearList();
            if (!this.CalendarPopup) return;

            this.MonthPopup =
                <div id={DateTimeEditComponent.MONTHPOPUPID} data-owner={this.ControlId} aria-hidden="false">
                    <div class="t_container" data-role="popup" aria-hidden="false">
                        <div class="t_scroller" unselectable="on">
                            <ul unselectable="on" class="t_list" tabindex="-1" aria-hidden="true" aria-live="off" data-role="staticlist" role="listbox">
                            </ul>
                        </div>
                    </div>
                </div> as HTMLElement;

            let ul = $YetaWF.getElement1BySelector("ul", [this.MonthPopup]);
            let startMonth = 0;
            let endMonth = 11;
            let selValue = this.TempCalCurrentDateUser.getUTCMonth();
            let html = "";
            for (let i = startMonth; i <= endMonth; ++i) {
                let extra = "";
                if (i === selValue)
                    extra = " t_selected";
                html += `<li tabindex="-1" role="option" unselectable="on" class="t_item${extra}" data-value="${i}">${this.Setup.Months[i]}</li>`; //Format
            }
            ul.innerHTML = html;

            let monthSel = $YetaWF.getElement1BySelector(".t_header .t_month", [this.CalendarPopup]);
            let style = window.getComputedStyle(monthSel);
            this.MonthPopup.style.font = style.font;
            this.MonthPopup.style.fontStyle = style.fontStyle;
            this.MonthPopup.style.fontWeight = style.fontWeight;
            this.MonthPopup.style.fontSize = style.fontSize;

            $YetaWF.positionLeftAlignedBelow(monthSel, this.MonthPopup);

            document.body.appendChild(this.MonthPopup);
            let sel =  $YetaWF.getElement1BySelectorCond(".t_list .t_selected", [this.MonthPopup]);
            if (sel) {
                let selRect = sel.getBoundingClientRect();
                let list =  $YetaWF.getElement1BySelector(".t_scroller", [this.MonthPopup]);
                let listRect = list.getBoundingClientRect();
                list.scrollTop = selRect.top - listRect.top;
                //sel.scrollIntoView({ behavior: "auto", block: "nearest", inline: "start" }); // causes outer page jump
            }

            $YetaWF.registerEventHandler(this.MonthPopup, "mousedown", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem as HTMLLIElement;
                let value = Number($YetaWF.getAttribute(li, "data-value"));
                this.tempMonthSelectedValue = value;
                return false;
            });
            $YetaWF.registerEventHandler(this.MonthPopup, "mouseup", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem as HTMLLIElement;
                let value = Number($YetaWF.getAttribute(li, "data-value"));
                if (this.tempMonthSelectedValue === value) {
                    let date = this.TempCalCurrentDateUser;
                    date.setUTCMonth(value);
                    let tbody = $YetaWF.getElement1BySelector(".t_calendarbody tbody", [this.CalendarPopup!]);
                    this.buildCalendarMonthPage(tbody, date);
                    this.updateCalendarTitle();
                    this.tempMonthSelectedValue = null;
                    this.closeMonthList();
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.MonthPopup, "click", "ul li", (ev: MouseEvent): boolean => {
                return false;
            });
            $YetaWF.registerEventHandler(this.MonthPopup, "mouseover", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "t_hover");
                $YetaWF.elementAddClass(li, "t_hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.MonthPopup, "mouseout", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "t_hover");
                return true;
            });
        }
        private closeMonthList(): void {
            if (this.MonthPopup) {
                this.MonthPopup.remove();
                this.MonthPopup = null;
            }
        }


        private updateCalendarTitle(): void {
            if (!this.CalendarPopup) return;
            let month = this.Setup.Months[this.TempCalCurrentDateUser.getUTCMonth()];
            let year = this.TempCalCurrentDateUser.getUTCFullYear();
            let monthSpan = $YetaWF.getElement1BySelector(".t_header .t_month", [this.CalendarPopup]);
            monthSpan.innerHTML = month;
            let yearSpan = $YetaWF.getElement1BySelector(".t_header .t_year", [this.CalendarPopup]);
            yearSpan.innerHTML = year.toFixed(0);
        }

        private getFormattedDateTime(date: Date): string {
            let time = date.getUTCHours()* 60 + date.getUTCMinutes();
            switch (this.Setup.Style) {
                default:
                case DateTimeStyleEnum.DateTime:
                    return this.getFormattedDate(date) + " " + this.getFormattedTime(time);
                case DateTimeStyleEnum.Date:
                    return this.getFormattedDate(date);
                case DateTimeStyleEnum.Time:
                    return this.getFormattedTime(time);
            }
        }
        private getFormattedDate(date: Date): string {
            let d = date.getUTCDate();
            let m = date.getUTCMonth() + 1;
            let y = date.getUTCFullYear();
            switch (this.Setup.DateFormat) {
                default:
                case DateFormatEnum.MMDDYYYY:
                    return `${this.zeroPad(m, 2)}/${this.zeroPad(d, 2)}/${y}`;
                case DateFormatEnum.MMDDYYYYdash:
                    return `${this.zeroPad(m, 2)}-${this.zeroPad(d, 2)}-${y}`;
                case DateFormatEnum.MMDDYYYYdot:
                    return `${this.zeroPad(m, 2)}.${this.zeroPad(d, 2)}.${y}`;
                case DateFormatEnum.DDMMYYYY:
                    return `${this.zeroPad(d, 2)}/${this.zeroPad(m, 2)}/${y}`;
                case DateFormatEnum.DDMMYYYYdash:
                    return `${this.zeroPad(d, 2)}-${this.zeroPad(m, 2)}-${y}`;
                case DateFormatEnum.DDMMYYYYdot:
                    return `${this.zeroPad(d, 2)}.${this.zeroPad(m, 2)}.${y}`;
                case DateFormatEnum.YYYYMMDD:
                    return `${y}/${this.zeroPad(m, 2)}/${this.zeroPad(d, 2)}`;
                case DateFormatEnum.YYYYMMDDdash:
                    return `${y}-${this.zeroPad(m, 2)}-${this.zeroPad(d, 2)}`;
                case DateFormatEnum.YYYYMMDDdot:
                    return `${y}.${this.zeroPad(m, 2)}.${this.zeroPad(d, 2)}`;
            }
        }
        private getFormattedTime(time: number):string {
            let h = Math.floor(time/60);
            let m =  time % 60;
            switch (this.Setup.TimeFormat) {
                default:
                case TimeFormatEnum.HHMMAM:
                case TimeFormatEnum.HHMMSSAM:
                case TimeFormatEnum.HHMMAMdot:
                case TimeFormatEnum.HHMMSSAMdot:
                    let hour = h % 12;
                    if (hour === 0) hour += 12;
                    switch (this.Setup.TimeFormat) {
                        default:
                        case TimeFormatEnum.HHMMAM:
                        case TimeFormatEnum.HHMMSSAM:
                            return `${this.zeroPad(hour, 2)}:${this.zeroPad(m, 2)} ${h < 12 ? "AM" : "PM"}`;
                        case TimeFormatEnum.HHMMAMdot:
                        case TimeFormatEnum.HHMMSSAMdot:
                            return `${this.zeroPad(hour, 2)}.${this.zeroPad(m, 2)} ${h < 12 ? "AM" : "PM"}`;
                    }
                case TimeFormatEnum.HHMM:
                case TimeFormatEnum.HHMMSS:
                    return `${this.zeroPad(h, 2)}:${this.zeroPad(m, 2)}`;
                case TimeFormatEnum.HHMMdot:
                case TimeFormatEnum.HHMMSSdot:
                    return `${this.zeroPad(h, 2)}.${this.zeroPad(m, 2)}`;
            }
        }
        private zeroPad(val: number, pos: number): string {
            if (val < 0) return val.toFixed();
            let s = val.toFixed(0);
            while (s.length < pos)
                s = "0" + s;
            return s;
        }
        private getLongFormattedDate(date: Date): string {
            let day = date.getUTCDay();
            let dom = date.getUTCDate();
            let m = date.getUTCMonth();
            let y = date.getUTCFullYear();
            switch (this.Setup.DateFormat) {
                default:
                case DateFormatEnum.MMDDYYYY:
                case DateFormatEnum.MMDDYYYYdash:
                case DateFormatEnum.MMDDYYYYdot:
                    return `${this.Setup.WeekDays[day]}, ${this.Setup.Months[m]} ${dom}, ${y}`;
                case DateFormatEnum.DDMMYYYY:
                case DateFormatEnum.DDMMYYYYdash:
                case DateFormatEnum.DDMMYYYYdot:
                    return `${this.Setup.WeekDays[day]}, ${dom} ${this.Setup.Months[m]}, ${y}`;
                case DateFormatEnum.YYYYMMDD:
                case DateFormatEnum.YYYYMMDDdash:
                case DateFormatEnum.YYYYMMDDdot:
                    return `${y}, ${this.Setup.Months[m]} ${dom}, ${this.Setup.WeekDays[day]}`;
            }
        }

        private setSelectedIndex(popup: HTMLElement, index: number): void {
            let list = $YetaWF.getElement1BySelector(".t_list", [popup]);
            let entries = $YetaWF.getElementsBySelector("li", [list]);
            let active = $YetaWF.getElement1BySelectorCond("li.t_selected", [list]);
            if (active)
                $YetaWF.elementRemoveClass(active, "t_selected");
            if (index < 0) index = 0;
            if (index >= entries.length) index = entries.length-1;
            if (index >= 0)
                $YetaWF.elementAddClass(entries[index], "t_selected");
            entries[index].scrollIntoView({ behavior: "auto", block: "nearest", inline: "start" });
        }

        private getSelectedIndex(popup: HTMLElement): number {
            let list = $YetaWF.getElement1BySelector(".t_list", [popup]);
            let entries = $YetaWF.getElementsBySelector("li", [list]);
            let active = $YetaWF.getElement1BySelectorCond("li.t_selected", [list]);
            if (!active)
                return -1;
            let index = entries.indexOf(active);
            return index;
        }
        private getSelectedValue(popup: HTMLElement): string|null {
            let active = $YetaWF.getElement1BySelectorCond("li.t_selected", [popup]);
            if (!active)
                return null;
            let value = $YetaWF.getAttribute(active, "data-value");
            return value;
        }

        private setHidden(dateVal: Date | null): void {
            var s: string = "";
            if (dateVal != null)
                s = `${dateVal.getUTCFullYear()}-${this.zeroPad(dateVal.getUTCMonth()+1, 2)}-${this.zeroPad(dateVal.getUTCDate(), 2)}T${this.zeroPad(dateVal.getUTCHours(), 2)}:${this.zeroPad(dateVal.getUTCMinutes(), 2)}:00.000Z`;
            this.InputHidden.setAttribute("value", s);
        }
        private setHiddenInvalid(dateVal: string): void {
            this.InputHidden.setAttribute("value", dateVal);
        }

        private get SelectedUTC(): Date {
            return this._SelectedUTC;
        }
        private set SelectedUTC(date: Date) {
            this._SelectedUTC = new Date(date);
            this._SelectedUser = this.UTCToUser(date);
        }
        private _SelectedUTC: Date = new Date();

        private get SelectedUser(): Date {
            return this._SelectedUser;
        }
        private set SelectedUser(date: Date) {
            this._SelectedUser = new Date(date);
            this._SelectedUTC = this.UserToUTC(date);
        }
        private _SelectedUser: Date = new Date();

        private get TempCalCurrentDateUTC(): Date {
            return this._TempCalCurrentDateUTC;
        }
        private set TempCalCurrentDateUTC(date: Date) {
            this._TempCalCurrentDateUTC = new Date(date);
            this._TempCalCurrentDateUser = this.UTCToUser(date);
        }
        private _TempCalCurrentDateUTC: Date = new Date();

        private get TempCalCurrentDateUser(): Date {
            return this._TempCalCurrentDateUser;
        }
        private set TempCalCurrentDateUser(date: Date) {
            this._TempCalCurrentDateUser = new Date(date);
            this._TempCalCurrentDateUTC = this.UserToUTC(date);
        }
        private _TempCalCurrentDateUser: Date = new Date();

        private UTCToUser(date: Date): Date {
            let d = new Date(date);
            d.setUTCSeconds(d.getUTCSeconds() + this.Setup.BaseUtcOffset);
            return d;
        }
        private UserToUTC(date: Date): Date {
            let d = new Date(date);
            d.setUTCSeconds(d.getUTCSeconds() - this.Setup.BaseUtcOffset);
            return d;
        }

        private get TimeSelectedUser(): number {
            return this.SelectedUser.getUTCHours() * 60 + this.SelectedUser.getUTCMinutes();
        }
        private set TimeSelectedUser(value: number) {
            let d = new Date(this.SelectedUser);
            d.setUTCHours(value/60);
            d.setUTCMinutes(value % 60);
            d.setUTCSeconds(0);
            d.setUTCMilliseconds(0);
            this.SelectedUser = d;
            this.InputControl.value = this.getFormattedDateTime(this.SelectedUser);
            this.setHidden(this.SelectedUTC);
        }

        get dateSelectedValueUTC(): Date {
            return this.SelectedUser;
        }
        set dateSelectedValueUTC(date: Date) {
            let time = this.TimeSelectedUser;
            this.SelectedUTC = date;
            this.TimeSelectedUser = time;
        }

        // API

        get value(): Date {
            return new Date(this.dateSelectedValueUTC);
        }
        get valueText(): string {
            return this.InputHidden.value;
        }

        public clear(): void {
            this.SelectedUTC = new Date();
            this.InputControl.value = "";
            this.setHidden(null);
        }
        public enable(enabled: boolean): void {
            this.closeTimeList();
            $YetaWF.elementEnableToggle(this.InputControl, enabled);
            $YetaWF.elementEnableToggle(this.InputHidden, enabled);
            $YetaWF.elementRemoveClass(this.Control, "t_disabled");
            if (!enabled)
                $YetaWF.elementAddClass(this.Control, "t_disabled");
        }
        public get enabled(): boolean {
            return !$YetaWF.elementHasClass(this.Control, "t_disabled");
        }
        public get focused(): boolean {
            return $YetaWF.elementHasClass(this.Control, "t_focused");
        }

        public close(): void {
            ToolTipsHTMLHelper.removeTooltips();
            this.closeMonthList();
            this.closeYearList();
            this.closeTimeList();
            this.closeCalendar();
        }
        public static closeAll(): void {
            let ctrls = $YetaWF.getElementsBySelector(DateTimeEditComponent.SELECTOR);
            for (let ctrl of ctrls) {
                let c = DateTimeEditComponent.getControlFromTag<DateTimeEditComponent>(ctrl, DateTimeEditComponent.SELECTOR);
                if (c.IgnoreResizeUntil < Date.now())
                    c.close();
            }
        }
    }
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: CustomEvent<YetaWF.DetailsEventContainerResize>): boolean => {
        DateTimeEditComponent.closeAll();
        return true;
    });
}

