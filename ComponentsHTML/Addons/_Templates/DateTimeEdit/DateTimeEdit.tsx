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
        Today: Date;
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

    export class DateTimeEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_datetime";
        public static readonly SELECTOR: string = ".yt_datetime.t_edit";
        public static readonly EVENTCHANGE: string = "datetime_change";

        public static readonly TIMEPOPUPID: string = "yt_datetime_popup";
        public static readonly CALENDARPOPUPID: string = "yt_datetime_calendarpopup";
        public static readonly YEARPOPUPID: string = "yt_datetime_popup";
        public static readonly MONTHPOPUPID: string = "yt_datetime_popup";

        private Setup: DateTimeEditSetup;
        private InputHidden: HTMLInputElement;
        private InputControl: HTMLInputElement;
        private Selected: Date;
        private TimePopup: HTMLElement | null = null;
        private CalendarPopup: HTMLElement | null = null;
        private YearPopup: HTMLElement | null = null;
        private MonthPopup: HTMLElement | null = null;
        private tempTimeSelectedValue: number|null = null;
        private tempCalSelectedValue: string|null = null;
        private tempCalCurrentDate: Date;
        private tempArrow: HTMLAnchorElement|null = null;
        private tempYearSelectedValue: number|null = null;
        private tempMonthSelectedValue: number|null = null;

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

            if (this.InputHidden.value)
                this.Selected = new Date(this.InputHidden.value);
            else
                this.Selected = new Date();
            this.tempCalCurrentDate = new Date(this.Selected);

            $YetaWF.registerEventHandler(this.Control, "mousedown", ".t_time", (ev: Event): boolean => {
                if (this.enabled) {
                    if (this.TimePopup) {
                        this.closeTimeList();
                    } else {
                        this.InputControl.focus();
                        // delay opening calendar. On mobile devices the view may be resized on focus (keyboard) so we want to wait
                        // until after the resize event so we don't just close the calendar.
                        setTimeout((): void => {
                            this.openTimeList();
                        },100);
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
                        // delay opening calendar. On mobile devices the view may be resized on focus (keyboard) so we want to wait
                        // until after the resize event so we don't just close the calendar.
                        setTimeout((): void => {
                            this.openCalendar();
                        },100);
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
                return true;
            });
            $YetaWF.registerEventHandler(this.InputControl, "keydown", null, (ev: KeyboardEvent): boolean => {
                let key = ev.key;
                if (this.TimePopup) {
                    if (key === "ArrowDown" || key === "ArrowRight") {
                        this.setSelectedIndex(this.TimePopup, this.getSelectedIndex(this.TimePopup) + 1);
                        return false;
                    } else if (key === "ArrowUp" || key === "Up" || key === "ArrowLeft" || key === "Left") {
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
                            this.timeSelectedValue = Number(time);
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
                            let date = this.tempCalCurrentDate;
                            date.setMonth(Number(month));
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
                            let date = this.tempCalCurrentDate;
                            date.setFullYear(Number(year));
                            let tbody = $YetaWF.getElement1BySelector(".t_calendarbody tbody", [this.CalendarPopup!]);
                            this.buildCalendarMonthPage(tbody, date);
                            this.updateCalendarTitle();
                            this.closeYearList();
                            return false;
                        }
                    } else {
                        let date = this.tempCalCurrentDate;
                        if (ev.altKey) {
                            if (key === "ArrowUp") {
                                this.close();
                                return false;
                            }
                        } else {
                            if (key === "Enter") {
                                this.tempCalCurrentDate = new Date(date);
                                this.dateSelectedValue = this.tempCalCurrentDate;
                                this.close();
                                this.sendChangeEvent();
                                return false;
                            } else if (key === "ArrowUp") {
                                date.setDate(date.getDate() - 7 );
                            } else if (key === "ArrowDown") {
                                date.setDate(date.getDate() + 7 );
                            } else if (key === "ArrowLeft") {
                                date.setDate(date.getDate() - 1 );
                            } else if (key === "ArrowRight") {
                                date.setDate(date.getDate() + 1 );
                            } else if (key === "PageUp") {
                                let dom = date.getDate();// day of month current month
                                date.setDate(0);// last day of last month
                                let daysInMonth = date.getDate();
                                if (dom >= daysInMonth)
                                    dom = daysInMonth;
                                date.setDate(dom);
                            } else if (key === "PageDown") {
                                let dom = date.getDate();// day of month current month
                                date.setDate(1);// first day of this month
                                date.setMonth(date.getMonth() + 2);// first day of next-next month
                                date.setDate(0);// last day of next month
                                let daysInMonth = date.getDate();
                                if (dom >= daysInMonth)
                                    dom = daysInMonth;
                                date.setDate(dom);
                            } else if (key === "Home") {
                                date.setDate(1);
                            } else if (key === "End") {
                                date.setMonth(date.getMonth() + 1 );
                                date.setDate(date.getDate() - 1 );
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
            let selValue = this.timeSelectedValue;
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
                    this.timeSelectedValue = this.tempTimeSelectedValue;
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

            let month = this.Setup.Months[this.tempCalCurrentDate.getMonth()];
            let year = this.tempCalCurrentDate.getFullYear().toFixed(0);

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
            this.buildCalendarMonthPage(tbody, this.tempCalCurrentDate || new Date());

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
                this.tempCalSelectedValue = value;
                return false;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "mouseup", "table td .t_link", (ev: MouseEvent): boolean => {
                let anchor = ev.__YetaWFElem as HTMLAnchorElement;
                let value = $YetaWF.getAttribute(anchor, "data-value");
                if (this.tempCalSelectedValue === value) {
                    this.dateSelectedValue = new Date(this.tempCalSelectedValue);
                    this.tempCalSelectedValue = null;
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
                    let date = this.tempCalCurrentDate;
                    if ($YetaWF.elementHasClass(anchor, "t_prev")) {
                        date.setMonth(date.getMonth()-1);
                    } else {
                        date.setMonth(date.getMonth()+1);
                    }
                    let tbody = $YetaWF.getElement1BySelector(".t_calendarbody tbody", [this.CalendarPopup!]);
                    this.buildCalendarMonthPage(tbody, date);
                    this.updateCalendarTitle();
                    this.tempArrow = null;
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "click", ".t_header .t_prev svg, .t_header .t_next svg", (ev: MouseEvent): boolean => {
                return false;
            });

            $YetaWF.registerEventHandler(this.CalendarPopup, "click", ".t_footer .t_today", (ev: MouseEvent): boolean => {
                this.dateSelectedValue = new Date(this.Setup.Today);
                this.tempCalSelectedValue = null;
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

            this.tempCalCurrentDate = new Date(startDate);

            let date = new Date(startDate);
            let startMonth = startDate.getMonth();
            date.setDate(1);// set the first day
            date.setDate(- date.getDay() + 1); // get to the start of the week (Sunday)

            let today = new Date();

            tbody.innerHTML = "";

            let selDate = this.tempCalCurrentDate;
            for (let last = false ; !last ; ) {

                let row = <tr role="row"></tr> as HTMLTableRowElement;
                for (let day = 0 ; day < 7 ; ++day ) {
                    let css = "";
                    if (day === 0 || day === 6) // Saturday, Sunday
                        css += " t_weekend";
                    if (date < startDate && date.getMonth() !== startMonth)
                        css += " t_othermonth";
                    else if (date > startDate && date.getMonth() !== startMonth) {
                        css += " t_othermonth";
                        last = true;
                    }
                    if (date.getFullYear() === selDate.getFullYear() && date.getMonth() === selDate.getMonth() && date.getDate() === selDate.getDate())
                        css += " t_selected";
                    if (date.getDate() === today.getDate() && date.getMonth() === today.getMonth() && date.getFullYear() === today.getFullYear())
                        css += " t_today";

                    css = css.trim();
                    let tt = this.getLongFormattedDate(date);

                    let cell =
                        <td class={css} role="gridcell">
                            <a tabindex="-1" class="t_link" href="#" data-value={date.toISOString()} title={tt}>{date.getDate()}</a>
                        </td> as HTMLTableCellElement;

                    row.appendChild(cell);

                    date.setDate(date.getDate()+1);
                }
                tbody.appendChild(row);

                if (!last && date.getMonth() !== startMonth)
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
            let startYear = minDate.getFullYear();
            let endYear = maxDate.getFullYear();
            let selValue = this.tempCalCurrentDate.getFullYear();
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
                    let date = this.tempCalCurrentDate;
                    date.setFullYear(value);
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
            let selValue = this.tempCalCurrentDate.getMonth();
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
                    let date = this.tempCalCurrentDate;
                    date.setMonth(value);
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
            let month = this.Setup.Months[this.tempCalCurrentDate.getMonth()];
            let year = this.tempCalCurrentDate.getFullYear();
            let monthSpan = $YetaWF.getElement1BySelector(".t_header .t_month", [this.CalendarPopup]);
            monthSpan.innerHTML = month;
            let yearSpan = $YetaWF.getElement1BySelector(".t_header .t_year", [this.CalendarPopup]);
            yearSpan.innerHTML = year.toFixed(0);
        }

        private getFormattedDateTime(date: Date): string {
            let time = date.getHours()* 60 + date.getMinutes();
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
            let d = date.getDate();
            let m = date.getMonth() + 1;
            let y = date.getFullYear();
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
            let day = date.getDay();
            let dom = date.getDate();
            let m = date.getMonth();
            let y = date.getFullYear();
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
                s = dateVal.toISOString();
            this.InputHidden.setAttribute("value", s);
        }

        // API

        get value(): Date {
            return this.dateSelectedValue;
        }
        set value(val: Date) {
            this.dateSelectedValue = val;
            this.tempCalCurrentDate = val;
        }
        get valueText(): string {
            return this.InputHidden.value;
        }

        get timeSelectedValue(): number {
            return this.Selected.getHours() * 60 + this.Selected.getMinutes();
        }
        set timeSelectedValue(value: number) {
            this.Selected.setHours(value/60);
            this.Selected.setMinutes(value % 60);
            this.Selected.setSeconds(0);
            this.Selected.setMilliseconds(0);
            this.InputControl.value = this.getFormattedDateTime(this.Selected);
            this.setHidden(this.Selected);
        }

        get dateSelectedValue(): Date {
            return this.Selected;
        }
        set dateSelectedValue(date: Date) {
            let time = this.timeSelectedValue;
            this.Selected = date;
            this.timeSelectedValue = time;
        }

        public clear(): void {
            this.Selected = new Date();
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
                c.close();
            }
        }
    }
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: CustomEvent<YetaWF.DetailsEventContainerResize>): boolean => {
        DateTimeEditComponent.closeAll();
        return true;
    });
}

