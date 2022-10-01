"use strict";
/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var DateTimeStyleEnum;
    (function (DateTimeStyleEnum) {
        DateTimeStyleEnum[DateTimeStyleEnum["DateTime"] = 0] = "DateTime";
        DateTimeStyleEnum[DateTimeStyleEnum["Date"] = 1] = "Date";
        DateTimeStyleEnum[DateTimeStyleEnum["Time"] = 2] = "Time";
    })(DateTimeStyleEnum || (DateTimeStyleEnum = {}));
    var DateFormatEnum;
    (function (DateFormatEnum) {
        DateFormatEnum[DateFormatEnum["MMDDYYYY"] = 0] = "MMDDYYYY";
        DateFormatEnum[DateFormatEnum["MMDDYYYYdash"] = 1] = "MMDDYYYYdash";
        DateFormatEnum[DateFormatEnum["MMDDYYYYdot"] = 2] = "MMDDYYYYdot";
        DateFormatEnum[DateFormatEnum["DDMMYYYY"] = 10] = "DDMMYYYY";
        DateFormatEnum[DateFormatEnum["DDMMYYYYdash"] = 11] = "DDMMYYYYdash";
        DateFormatEnum[DateFormatEnum["DDMMYYYYdot"] = 12] = "DDMMYYYYdot";
        DateFormatEnum[DateFormatEnum["YYYYMMDD"] = 20] = "YYYYMMDD";
        DateFormatEnum[DateFormatEnum["YYYYMMDDdash"] = 21] = "YYYYMMDDdash";
        DateFormatEnum[DateFormatEnum["YYYYMMDDdot"] = 22] = "YYYYMMDDdot";
    })(DateFormatEnum = YetaWF_ComponentsHTML.DateFormatEnum || (YetaWF_ComponentsHTML.DateFormatEnum = {}));
    var TimeFormatEnum;
    (function (TimeFormatEnum) {
        TimeFormatEnum[TimeFormatEnum["HHMMAM"] = 0] = "HHMMAM";
        TimeFormatEnum[TimeFormatEnum["HHMMAMdot"] = 1] = "HHMMAMdot";
        TimeFormatEnum[TimeFormatEnum["HHMM"] = 10] = "HHMM";
        TimeFormatEnum[TimeFormatEnum["HHMMdot"] = 11] = "HHMMdot";
        TimeFormatEnum[TimeFormatEnum["HHMMSSAM"] = 20] = "HHMMSSAM";
        TimeFormatEnum[TimeFormatEnum["HHMMSSAMdot"] = 21] = "HHMMSSAMdot";
        TimeFormatEnum[TimeFormatEnum["HHMMSS"] = 30] = "HHMMSS";
        TimeFormatEnum[TimeFormatEnum["HHMMSSdot"] = 31] = "HHMMSSdot";
    })(TimeFormatEnum = YetaWF_ComponentsHTML.TimeFormatEnum || (YetaWF_ComponentsHTML.TimeFormatEnum = {}));
    var DateTimeEditComponent = /** @class */ (function (_super) {
        __extends(DateTimeEditComponent, _super);
        function DateTimeEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, DateTimeEditComponent.TEMPLATE, DateTimeEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: DateTimeEditComponent.EVENTCHANGE,
                GetValue: function (control) {
                    return control.valueText;
                },
                Enable: function (control, enable, clearOnDisable) {
                    YetaWF_BasicsImpl.elementEnableToggle(control.InputHidden, enable);
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }) || this;
            _this.TimePopup = null;
            _this.CalendarPopup = null;
            _this.YearPopup = null;
            _this.MonthPopup = null;
            _this.tempTimeSelectedValue = null;
            _this.tempCalSelectedValue = null;
            _this.tempArrow = null;
            _this.tempYearSelectedValue = null;
            _this.tempMonthSelectedValue = null;
            _this._SelectedUser = new Date();
            _this._TempCalCurrentDateUser = new Date();
            _this.IgnoreResizeUntil = 0;
            _this.Setup = setup;
            _this.WeekDays2 = YLocs.YetaWF_ComponentsHTML.WeekDays2.split(",");
            _this.WeekDays = YLocs.YetaWF_ComponentsHTML.WeekDays.split(",");
            _this.MonthNames = YLocs.YetaWF_ComponentsHTML.MonthNames.split(",");
            _this.InputHidden = $YetaWF.getElement1BySelector("input[type='hidden']", [_this.Control]);
            _this.InputControl = $YetaWF.getElement1BySelector("input[type='text']", [_this.Control]);
            var warn = $YetaWF.createElement("div", { class: "t_warn", style: "display:none" });
            warn.innerHTML = YConfigs.YetaWF_ComponentsHTML.SVG_fas_exclamation_triangle;
            _this.InputControl.insertAdjacentElement("afterend", warn);
            _this.SelectedUser = new Date(_this.Setup.InitialCalendarDate || _this.Setup.Today);
            _this.TempCalCurrentDateUser = new Date(_this.SelectedUser);
            $YetaWF.registerEventHandler(_this.Control, "mousedown", ".t_time", function (ev) {
                if (_this.enabled) {
                    if (_this.TimePopup) {
                        _this.closeTimeList();
                    }
                    else {
                        _this.InputControl.focus();
                        // ignore resize events as we're receiving the focus. On mobile this may result in resize events due to keyboard appearing.
                        _this.IgnoreResizeUntil = Date.now() + 1000;
                        _this.openTimeList();
                    }
                }
                return false;
            });
            $YetaWF.registerEventHandler(_this.Control, "mousedown", ".t_date", function (ev) {
                if (_this.enabled) {
                    if (_this.CalendarPopup) {
                        _this.closeCalendar();
                    }
                    else {
                        _this.InputControl.focus();
                        // ignore resize events as we're receiving the focus. On mobile this may result in resize events due to keyboard appearing.
                        _this.IgnoreResizeUntil = Date.now() + 1000;
                        _this.openCalendar();
                    }
                }
                return false;
            });
            $YetaWF.registerEventHandler(_this.InputControl, "focusin", null, function (ev) {
                if (_this.enabled) {
                    $YetaWF.elementRemoveClass(_this.Control, "t_focused");
                    $YetaWF.elementAddClass(_this.Control, "t_focused");
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.InputControl, "focusout", null, function (ev) {
                $YetaWF.elementRemoveClass(_this.Control, "t_focused");
                _this.close();
                _this.setHiddenInvalid(_this.InputControl.value);
                if (_this.validateInput(true)) {
                    ; //this.sendChangeEvent(); causes change event just leaving the field
                }
                else
                    _this.flashError();
                return true;
            });
            $YetaWF.registerEventHandler(_this.InputControl, "keydown", null, function (ev) {
                var key = ev.key;
                if (_this.TimePopup) {
                    if (key === "ArrowDown" || key === "ArrowRight") {
                        _this.setSelectedIndex(_this.TimePopup, _this.getSelectedIndex(_this.TimePopup) + 1);
                        return false;
                    }
                    else if (key === "ArrowUp" || key === "ArrowLeft") {
                        _this.setSelectedIndex(_this.TimePopup, _this.getSelectedIndex(_this.TimePopup) - 1);
                        return false;
                    }
                    else if (key === "Home") {
                        _this.setSelectedIndex(_this.TimePopup, 0);
                        return false;
                    }
                    else if (key === "End") {
                        _this.setSelectedIndex(_this.TimePopup, 9999999);
                        return false;
                    }
                    else if (key === "Escape") {
                        _this.closeTimeList();
                        return false;
                    }
                    else if (key === "Enter") {
                        var time = _this.getSelectedValue(_this.TimePopup);
                        if (time)
                            _this.TimeSelectedUser = Number(time);
                        _this.close();
                        _this.sendChangeEvent();
                        return false;
                    }
                }
                else if (_this.CalendarPopup) {
                    if (_this.MonthPopup) {
                        if (key === "ArrowDown" || key === "ArrowRight") {
                            _this.setSelectedIndex(_this.MonthPopup, _this.getSelectedIndex(_this.MonthPopup) + 1);
                            return false;
                        }
                        else if (key === "ArrowUp" || key === "ArrowLeft") {
                            _this.setSelectedIndex(_this.MonthPopup, _this.getSelectedIndex(_this.MonthPopup) - 1);
                            return false;
                        }
                        else if (key === "Home") {
                            _this.setSelectedIndex(_this.MonthPopup, 0);
                            return false;
                        }
                        else if (key === "End") {
                            _this.setSelectedIndex(_this.MonthPopup, 9999999);
                            return false;
                        }
                        else if (key === "Escape") {
                            _this.closeMonthList();
                            return false;
                        }
                        else if (key === "Enter") {
                            var month = _this.getSelectedValue(_this.MonthPopup);
                            var date = _this.TempCalCurrentDateUser;
                            date.setMonth(Number(month));
                            var tbody = $YetaWF.getElement1BySelector(".t_calendarbody tbody", [_this.CalendarPopup]);
                            _this.buildCalendarMonthPage(tbody, date);
                            _this.updateCalendarTitle();
                            _this.closeMonthList();
                            return false;
                        }
                    }
                    else if (_this.YearPopup) {
                        if (key === "ArrowDown" || key === "ArrowRight") {
                            _this.setSelectedIndex(_this.YearPopup, _this.getSelectedIndex(_this.YearPopup) + 1);
                            return false;
                        }
                        else if (key === "ArrowUp" || key === "ArrowLeft") {
                            _this.setSelectedIndex(_this.YearPopup, _this.getSelectedIndex(_this.YearPopup) - 1);
                            return false;
                        }
                        else if (key === "Home") {
                            _this.setSelectedIndex(_this.YearPopup, 0);
                            return false;
                        }
                        else if (key === "End") {
                            _this.setSelectedIndex(_this.YearPopup, 9999999);
                            return false;
                        }
                        else if (key === "Escape") {
                            _this.closeYearList();
                            return false;
                        }
                        else if (key === "Enter") {
                            var year = _this.getSelectedValue(_this.YearPopup);
                            var date = _this.TempCalCurrentDateUser;
                            date.setFullYear(Number(year));
                            var tbody = $YetaWF.getElement1BySelector(".t_calendarbody tbody", [_this.CalendarPopup]);
                            _this.buildCalendarMonthPage(tbody, date);
                            _this.updateCalendarTitle();
                            _this.closeYearList();
                            return false;
                        }
                    }
                    else {
                        var date = _this.TempCalCurrentDateUser;
                        if (ev.altKey) {
                            if (key === "ArrowUp") {
                                _this.close();
                                return false;
                            }
                        }
                        else {
                            if (key === "Enter") {
                                _this.TempCalCurrentDateUser = new Date(date);
                                _this.dateSelectedValue = _this.TempCalCurrentDateUser;
                                _this.close();
                                _this.sendChangeEvent();
                                return false;
                            }
                            else if (key === "Escape") {
                                _this.close();
                                return false;
                            }
                            else if (key === "ArrowUp") {
                                date.setDate(date.getDate() - 7);
                            }
                            else if (key === "ArrowDown") {
                                date.setDate(date.getDate() + 7);
                            }
                            else if (key === "ArrowLeft") {
                                date.setDate(date.getDate() - 1);
                            }
                            else if (key === "ArrowRight") {
                                date.setDate(date.getDate() + 1);
                            }
                            else if (key === "PageUp") {
                                var dom = date.getDate(); // day of month current month
                                date.setDate(0); // last day of last month
                                var daysInMonth = date.getDate();
                                if (dom >= daysInMonth)
                                    dom = daysInMonth;
                                date.setDate(dom);
                            }
                            else if (key === "PageDown") {
                                var dom = date.getDate(); // day of month current month
                                date.setDate(1); // first day of this month
                                date.setMonth(date.getMonth() + 2); // first day of next-next month
                                date.setDate(0); // last day of next month
                                var daysInMonth = date.getDate();
                                if (dom >= daysInMonth)
                                    dom = daysInMonth;
                                date.setDate(dom);
                            }
                            else if (key === "Home") {
                                date.setDate(1);
                            }
                            else if (key === "End") {
                                date.setMonth(date.getMonth() + 1);
                                date.setDate(date.getDate() - 1);
                            }
                            else
                                return true;
                            var tbody = $YetaWF.getElement1BySelector(".t_calendarbody tbody", [_this.CalendarPopup]);
                            _this.buildCalendarMonthPage(tbody, date);
                            _this.updateCalendarTitle();
                            return false;
                        }
                    }
                }
                else {
                    if (ev.altKey) {
                        if (key === "ArrowDown") {
                            _this.openCalendar();
                            return false;
                        }
                    }
                }
                return true;
            });
            return _this;
        }
        DateTimeEditComponent.prototype.sendChangeEvent = function () {
            $YetaWF.sendCustomEvent(this.Control, DateTimeEditComponent.EVENTCHANGE);
            FormsSupport.validateElement(this.InputHidden);
        };
        DateTimeEditComponent.prototype.validateInput = function (updateHidden) {
            // validate input control and update hidden field
            var text = this.InputControl.value.trim();
            if (text.length === 0)
                return true;
            switch (this.Setup.Style) {
                default:
                case DateTimeStyleEnum.DateTime: {
                    var dt_1 = this.extractInputDate(text);
                    if (!dt_1.success)
                        return false;
                    text = dt_1.text;
                    var wt = this.getWhitespace(text);
                    if (!wt.success)
                        return false;
                    text = wt.text;
                    var tt_1 = this.extractInputTime(text);
                    if (!tt_1.success)
                        return false;
                    text = tt_1.text;
                    if (text.length > 0)
                        return false;
                    if (updateHidden) {
                        this.SelectedUser = dt_1.token;
                        this.TimeSelectedUser = tt_1.token;
                    }
                    break;
                }
                case DateTimeStyleEnum.Date:
                    var dt = this.extractInputDate(text);
                    if (!dt.success)
                        return false;
                    text = dt.text;
                    if (text.length > 0)
                        return false;
                    if (updateHidden)
                        this.dateSelectedValue = dt.token;
                    break;
                case DateTimeStyleEnum.Time:
                    var tt = this.extractInputTime(text);
                    if (!tt.success)
                        return false;
                    text = tt.text;
                    if (text.length > 0)
                        return false;
                    if (updateHidden)
                        this.TimeSelectedUser = tt.token;
                    break;
            }
            if (updateHidden)
                this.setHidden(this.SelectedUser);
            return true;
        };
        DateTimeEditComponent.prototype.extractInputDate = function (text) {
            var dt = { text: text, token: new Date(), success: false };
            var m, d, y;
            var nt;
            var rt;
            var sep = "/";
            switch (YLocs.YetaWF_ComponentsHTML.DateFormat) {
                default:
                case DateFormatEnum.MMDDYYYYdash:
                case DateFormatEnum.MMDDYYYYdot:
                case DateFormatEnum.MMDDYYYY:
                    if (YLocs.YetaWF_ComponentsHTML.DateFormat === DateFormatEnum.MMDDYYYYdash)
                        sep = "-";
                    else if (YLocs.YetaWF_ComponentsHTML.DateFormat === DateFormatEnum.MMDDYYYYdot)
                        sep = ".";
                    nt = this.getNumberToken(text);
                    if (!nt.success)
                        return dt;
                    text = nt.text;
                    m = nt.token;
                    rt = this.getRequired(text, sep);
                    if (!rt.success)
                        return dt;
                    text = rt.text;
                    nt = this.getNumberToken(text);
                    if (!nt.success)
                        return dt;
                    text = nt.text;
                    d = nt.token;
                    rt = this.getRequired(text, sep);
                    if (!rt.success)
                        return dt;
                    text = rt.text;
                    nt = this.getNumberToken(text);
                    if (!nt.success)
                        return dt;
                    text = nt.text;
                    y = nt.token;
                    break;
                case DateFormatEnum.DDMMYYYY:
                case DateFormatEnum.DDMMYYYYdash:
                case DateFormatEnum.DDMMYYYYdot:
                    if (YLocs.YetaWF_ComponentsHTML.DateFormat === DateFormatEnum.DDMMYYYYdash)
                        sep = "-";
                    else if (YLocs.YetaWF_ComponentsHTML.DateFormat === DateFormatEnum.DDMMYYYYdot)
                        sep = ".";
                    nt = this.getNumberToken(text);
                    if (!nt.success)
                        return dt;
                    text = nt.text;
                    d = nt.token;
                    rt = this.getRequired(text, sep);
                    if (!rt.success)
                        return dt;
                    text = rt.text;
                    nt = this.getNumberToken(text);
                    if (!nt.success)
                        return dt;
                    text = nt.text;
                    m = nt.token;
                    rt = this.getRequired(text, sep);
                    if (!rt.success)
                        return dt;
                    text = rt.text;
                    nt = this.getNumberToken(text);
                    if (!nt.success)
                        return dt;
                    text = nt.text;
                    y = nt.token;
                    break;
                case DateFormatEnum.YYYYMMDD:
                case DateFormatEnum.YYYYMMDDdash:
                case DateFormatEnum.YYYYMMDDdot:
                    if (YLocs.YetaWF_ComponentsHTML.DateFormat === DateFormatEnum.YYYYMMDDdash)
                        sep = "-";
                    else if (YLocs.YetaWF_ComponentsHTML.DateFormat === DateFormatEnum.YYYYMMDDdot)
                        sep = ".";
                    nt = this.getNumberToken(text);
                    if (!nt.success)
                        return dt;
                    text = nt.text;
                    y = nt.token;
                    rt = this.getRequired(text, sep);
                    if (!rt.success)
                        return dt;
                    text = rt.text;
                    nt = this.getNumberToken(text);
                    if (!nt.success)
                        return dt;
                    text = nt.text;
                    m = nt.token;
                    rt = this.getRequired(text, sep);
                    if (!rt.success)
                        return dt;
                    text = rt.text;
                    nt = this.getNumberToken(text);
                    if (!nt.success)
                        return dt;
                    text = nt.text;
                    d = nt.token;
                    break;
            }
            if (d < 1 || d > 31)
                return dt;
            if (m < 1 || m > 12)
                return dt;
            var minDate = new Date(this.Setup.MinDate);
            var maxDate = new Date(this.Setup.MaxDate);
            if (y < minDate.getFullYear() || y > maxDate.getFullYear())
                return dt;
            dt.token = new Date(y, m - 1, d, 0, 0, 0);
            dt.text = text;
            dt.success = true;
            return dt;
        };
        DateTimeEditComponent.prototype.extractInputTime = function (text) {
            var tt = { text: text, token: 0, success: false };
            var h, m, ampm;
            var nt;
            var rt;
            var wt;
            var sep = ":";
            switch (YLocs.YetaWF_ComponentsHTML.TimeFormat) {
                default:
                case TimeFormatEnum.HHMMAM:
                case TimeFormatEnum.HHMMSSAM:
                case TimeFormatEnum.HHMMAMdot:
                case TimeFormatEnum.HHMMSSAMdot: {
                    if (YLocs.YetaWF_ComponentsHTML.TimeFormat === TimeFormatEnum.HHMMAMdot)
                        sep = ".";
                    nt = this.getNumberToken(text);
                    if (!nt.success)
                        return tt;
                    text = nt.text;
                    h = nt.token;
                    if (h < 0 || h > 12)
                        return tt;
                    rt = this.getRequired(text, sep);
                    if (!rt.success)
                        return tt;
                    text = rt.text;
                    nt = this.getNumberToken(text);
                    if (!nt.success)
                        return tt;
                    text = nt.text;
                    m = nt.token;
                    if (m < 0 || m > 59)
                        return tt;
                    wt = this.getWhitespace(text);
                    if (!wt.success)
                        return tt;
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
                        if (!rt.success)
                            return tt;
                        text = rt.text;
                    }
                    if (ampm === "P" && h < 12)
                        h += 12;
                    else if (ampm === "A" && h === 12)
                        h -= 12;
                    var val = (h * 60 + m) % (24 * 60);
                    if (val < this.Setup.MinTime || val > this.Setup.MaxTime)
                        return tt;
                    tt.text = text;
                    tt.token = val;
                    tt.success = true;
                    return tt;
                }
                case TimeFormatEnum.HHMM:
                case TimeFormatEnum.HHMMSS:
                case TimeFormatEnum.HHMMdot:
                case TimeFormatEnum.HHMMSSdot: {
                    if (YLocs.YetaWF_ComponentsHTML.TimeFormat === TimeFormatEnum.HHMMdot)
                        sep = ".";
                    nt = this.getNumberToken(text);
                    if (!nt.success)
                        return tt;
                    text = nt.text;
                    h = nt.token;
                    if (h < 0 || h >= 24)
                        return tt;
                    rt = this.getRequired(text, sep);
                    if (!rt.success)
                        return tt;
                    text = rt.text;
                    nt = this.getNumberToken(text);
                    if (!nt.success)
                        return tt;
                    text = nt.text;
                    m = nt.token;
                    if (m < 0 || m > 59)
                        return tt;
                    wt = this.getWhitespace(text);
                    if (!wt.success)
                        return tt;
                    text = wt.text;
                    var val = (h * 60 + m) % (24 * 60);
                    if (val < this.Setup.MinTime || val > this.Setup.MaxTime)
                        return tt;
                    tt.text = text;
                    tt.token = val;
                    tt.success = true;
                    return tt;
                }
            }
        };
        DateTimeEditComponent.prototype.getNumberToken = function (text) {
            var t = { text: text, token: 0, success: false };
            var num = 0;
            var success = false;
            while (text.length > 0) {
                var n = text[0];
                if (n >= "0" && n <= "9") {
                    success = true;
                    num = num * 10 + Number(n);
                    text = text.substr(1);
                }
                else
                    break;
            }
            t.text = text;
            t.token = num;
            t.success = success;
            return t;
        };
        DateTimeEditComponent.prototype.getRequired = function (text, required) {
            var t = { text: text, token: "", success: false };
            if (required.length !== 1)
                throw "required must be exact 1 character";
            if (text.length === 0 || (text[0].toLowerCase() !== required.toLowerCase() && text[0] !== required))
                return t;
            t.text = text = text.length > 0 ? text.substr(1) : "";
            t.token = required.toUpperCase();
            t.success = true;
            return t;
        };
        DateTimeEditComponent.prototype.getWhitespace = function (text) {
            var t = { text: text, token: "", success: false };
            var success = false;
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
        };
        DateTimeEditComponent.prototype.flashError = function () {
            var warn = $YetaWF.getElement1BySelector(".t_warn", [this.Control]);
            warn.style.display = "";
            setTimeout(function () {
                warn.style.display = "none";
            }, DateTimeEditComponent.WARNDELAY);
        };
        DateTimeEditComponent.prototype.openTimeList = function () {
            var _this = this;
            this.close();
            var times = [];
            for (var i = 0; i < 24 * 60; i += 30) {
                if (this.Setup.MinTime <= i && i <= this.Setup.MaxTime)
                    times.push(i);
            }
            this.TimePopup =
                $YetaWF.createElement("div", { id: DateTimeEditComponent.TIMEPOPUPID, "data-owner": this.ControlId, "aria-hidden": "false" },
                    $YetaWF.createElement("div", { class: "t_container", "data-role": "popup", "aria-hidden": "false" },
                        $YetaWF.createElement("div", { class: "t_scroller", unselectable: "on" },
                            $YetaWF.createElement("ul", { unselectable: "on", class: "t_list", tabindex: "-1", "aria-hidden": "true", "aria-live": "off", "data-role": "staticlist", role: "listbox" }))));
            var ul = $YetaWF.getElement1BySelector("ul", [this.TimePopup]);
            var len = times.length;
            var html = "";
            var selValue = this.TimeSelectedUser;
            for (var i = 0; i < len; ++i) {
                var val = times[i];
                var o = this.getFormattedTime(val);
                var extra = "";
                if (val === selValue)
                    extra = " t_selected";
                html += "<li tabindex=\"-1\" role=\"option\" unselectable=\"on\" class=\"t_item".concat(extra, "\" data-value=\"").concat(val, "\">").concat(o, "</li>"); //Format
            }
            ul.innerHTML = html;
            var style = window.getComputedStyle(this.Control);
            this.TimePopup.style.font = style.font;
            this.TimePopup.style.fontStyle = style.fontStyle;
            this.TimePopup.style.fontWeight = style.fontWeight;
            this.TimePopup.style.fontSize = style.fontSize;
            var ctrlRect = this.Control.getBoundingClientRect();
            this.TimePopup.style.width = "".concat(ctrlRect.width, "px");
            $YetaWF.positionLeftAlignedBelow(this.Control, this.TimePopup);
            document.body.appendChild(this.TimePopup);
            this.Control.setAttribute("aria-expanded", "true");
            var sel = $YetaWF.getElement1BySelectorCond(".t_list .t_selected", [this.TimePopup]);
            if (sel) {
                var selRect = sel.getBoundingClientRect();
                var list = $YetaWF.getElement1BySelector(".t_scroller", [this.TimePopup]);
                var listRect = list.getBoundingClientRect();
                list.scrollTop = selRect.top - listRect.top;
                //sel.scrollIntoView({ behavior: "auto", block: "nearest", inline: "start" }); // causes outer page jump
            }
            $YetaWF.registerEventHandler(this.TimePopup, "mousedown", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                var value = Number($YetaWF.getAttribute(li, "data-value"));
                _this.tempTimeSelectedValue = value;
                return false;
            });
            $YetaWF.registerEventHandler(this.TimePopup, "mouseup", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                var value = Number($YetaWF.getAttribute(li, "data-value"));
                if (_this.tempTimeSelectedValue === value) {
                    _this.TimeSelectedUser = _this.tempTimeSelectedValue;
                    _this.tempTimeSelectedValue = null;
                    _this.closeTimeList();
                    _this.sendChangeEvent();
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.TimePopup, "mouseover", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "t_hover");
                $YetaWF.elementAddClass(li, "t_hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.TimePopup, "mouseout", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "t_hover");
                return true;
            });
        };
        DateTimeEditComponent.prototype.closeTimeList = function () {
            if (this.TimePopup) {
                this.TimePopup.remove();
                this.TimePopup = null;
                this.Control.setAttribute("aria-expanded", "false");
            }
        };
        DateTimeEditComponent.prototype.openCalendar = function () {
            var _this = this;
            this.close();
            var month = this.MonthNames[this.TempCalCurrentDateUser.getMonth()];
            var year = this.TempCalCurrentDateUser.getFullYear().toFixed(0);
            this.CalendarPopup =
                $YetaWF.createElement("div", { id: DateTimeEditComponent.CALENDARPOPUPID, "data-owner": this.ControlId, "aria-hidden": "false" },
                    $YetaWF.createElement("div", { class: "t_container", "data-role": "popup", "aria-hidden": "false" },
                        $YetaWF.createElement("div", { "data-role": "calendar", class: "t_calendar" },
                            $YetaWF.createElement("div", { class: "t_header" },
                                $YetaWF.createElement("a", { href: "#", "data-action": "prev", role: "button", class: "t_prev", "aria-label": "Previous", "aria-disabled": "false" }),
                                $YetaWF.createElement("div", { class: "t_title", "aria-disabled": "false" },
                                    $YetaWF.createElement("span", { class: "t_month" }, month),
                                    " ",
                                    $YetaWF.createElement("span", { class: "t_year" }, year)),
                                $YetaWF.createElement("a", { href: "#", "data-action": "next", role: "button", class: "t_next", "aria-label": "Next", "aria-disabled": "false" })),
                            $YetaWF.createElement("div", { class: "t_calendarbody" },
                                $YetaWF.createElement("table", { tabindex: "0", role: "grid" },
                                    $YetaWF.createElement("thead", null,
                                        $YetaWF.createElement("tr", { role: "row" },
                                            $YetaWF.createElement("th", { scope: "col", "data-tooltip": this.WeekDays[0] }, this.WeekDays2[0]),
                                            $YetaWF.createElement("th", { scope: "col", "data-tooltip": this.WeekDays[1] }, this.WeekDays2[1]),
                                            $YetaWF.createElement("th", { scope: "col", "data-tooltip": this.WeekDays[2] }, this.WeekDays2[2]),
                                            $YetaWF.createElement("th", { scope: "col", "data-tooltip": this.WeekDays[3] }, this.WeekDays2[3]),
                                            $YetaWF.createElement("th", { scope: "col", "data-tooltip": this.WeekDays[4] }, this.WeekDays2[4]),
                                            $YetaWF.createElement("th", { scope: "col", "data-tooltip": this.WeekDays[5] }, this.WeekDays2[5]),
                                            $YetaWF.createElement("th", { scope: "col", "data-tooltip": this.WeekDays[6] }, this.WeekDays2[6]))),
                                    $YetaWF.createElement("tbody", null))),
                            $YetaWF.createElement("div", { class: "t_footer" },
                                $YetaWF.createElement("a", { href: "#", class: "t_todaysel" }, this.Setup.TodayString)))));
            var prev = $YetaWF.getElement1BySelector(".t_prev", [this.CalendarPopup]);
            prev.innerHTML = YConfigs.YetaWF_ComponentsHTML.SVG_fas_caret_left;
            var next = $YetaWF.getElement1BySelector(".t_next", [this.CalendarPopup]);
            next.innerHTML = YConfigs.YetaWF_ComponentsHTML.SVG_fas_caret_right;
            var tbody = $YetaWF.getElement1BySelector(".t_calendarbody tbody", [this.CalendarPopup]);
            this.buildCalendarMonthPage(tbody, this.TempCalCurrentDateUser || new Date());
            var style = window.getComputedStyle(this.Control);
            this.CalendarPopup.style.font = style.font;
            this.CalendarPopup.style.fontStyle = style.fontStyle;
            this.CalendarPopup.style.fontWeight = style.fontWeight;
            this.CalendarPopup.style.fontSize = style.fontSize;
            $YetaWF.positionLeftAlignedBelow(this.Control, this.CalendarPopup);
            document.body.appendChild(this.CalendarPopup);
            this.Control.setAttribute("aria-expanded", "true");
            $YetaWF.registerEventHandler(this.CalendarPopup, "mouseover", "table td", function (ev) {
                var td = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(td, "t_hover");
                $YetaWF.elementAddClass(td, "t_hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "mouseout", "table td", function (ev) {
                var td = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(td, "t_hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "mousedown", "table td .t_link", function (ev) {
                var anchor = ev.__YetaWFElem;
                var value = $YetaWF.getAttribute(anchor, "data-value");
                _this.tempCalSelectedValue = value;
                return false;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "mouseup", "table td .t_link", function (ev) {
                var anchor = ev.__YetaWFElem;
                var value = $YetaWF.getAttribute(anchor, "data-value");
                if (_this.tempCalSelectedValue === value) {
                    _this.dateSelectedValue = _this.TempCalCurrentDateUser = new Date(_this.tempCalSelectedValue);
                    _this.tempCalSelectedValue = null;
                    _this.close();
                    _this.sendChangeEvent();
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "mousedown", ".t_header .t_prev svg, .t_header .t_next svg", function (ev) {
                var anchor = $YetaWF.elementClosest(ev.__YetaWFElem, "a");
                _this.tempArrow = anchor;
                _this.closeMonthList();
                _this.closeYearList();
                return false;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "mouseup", ".t_header .t_prev svg, .t_header .t_next svg", function (ev) {
                var anchor = $YetaWF.elementClosest(ev.__YetaWFElem, "a");
                if (_this.tempArrow === anchor) {
                    var date = _this.TempCalCurrentDateUser;
                    if ($YetaWF.elementHasClass(anchor, "t_prev")) {
                        date.setMonth(date.getMonth() - 1);
                    }
                    else {
                        date.setMonth(date.getMonth() + 1);
                    }
                    var tbody_1 = $YetaWF.getElement1BySelector(".t_calendarbody tbody", [_this.CalendarPopup]);
                    _this.buildCalendarMonthPage(tbody_1, date);
                    _this.updateCalendarTitle();
                    _this.tempArrow = null;
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "click", "table td .t_link, .t_header .t_prev svg, .t_header .t_next svg", function (ev) {
                return false;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "mousedown", ".t_footer .t_todaysel", function (ev) {
                _this.dateSelectedValue = new Date(_this.Setup.Today);
                _this.tempCalSelectedValue = null;
                _this.close();
                _this.sendChangeEvent();
                return false;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "mousedown", ".t_header .t_month", function (ev) {
                if (_this.MonthPopup) {
                    _this.closeMonthList();
                }
                else {
                    _this.openMonthList();
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.CalendarPopup, "mousedown", ".t_header .t_year", function (ev) {
                if (_this.YearPopup) {
                    _this.closeYearList();
                }
                else {
                    _this.openYearList();
                }
                return false;
            });
        };
        DateTimeEditComponent.prototype.closeCalendar = function () {
            if (this.CalendarPopup) {
                this.CalendarPopup.remove();
                this.CalendarPopup = null;
                this.Control.setAttribute("aria-expanded", "false");
            }
        };
        DateTimeEditComponent.prototype.buildCalendarMonthPage = function (tbody, startDate) {
            this.TempCalCurrentDateUser = new Date(startDate);
            var date = new Date(startDate);
            var startMonth = startDate.getMonth();
            date.setDate(1); // set the first day
            date.setDate(-date.getDay() + 1); // get to the start of the week (Sunday)
            var today = new Date(this.Setup.Today);
            tbody.innerHTML = "";
            var selDate = this.TempCalCurrentDateUser;
            for (var last = false; !last;) {
                var row = $YetaWF.createElement("tr", { role: "row" });
                for (var day = 0; day < 7; ++day) {
                    var css = "";
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
                    var tt = this.getLongFormattedDate(date);
                    var cell = $YetaWF.createElement("td", { class: css, role: "gridcell" },
                        $YetaWF.createElement("a", { tabindex: "-1", class: "t_link", href: "#", "data-value": date.toISOString(), title: tt }, date.getDate()));
                    row.appendChild(cell);
                    date.setDate(date.getDate() + 1);
                }
                tbody.appendChild(row);
                if (!last && date.getMonth() !== startMonth)
                    last = true;
            }
        };
        DateTimeEditComponent.prototype.openYearList = function () {
            var _this = this;
            this.closeMonthList();
            this.closeYearList();
            if (!this.CalendarPopup)
                return;
            this.YearPopup =
                $YetaWF.createElement("div", { id: DateTimeEditComponent.YEARPOPUPID, "data-owner": this.ControlId, "aria-hidden": "false" },
                    $YetaWF.createElement("div", { class: "t_container", "data-role": "popup", "aria-hidden": "false" },
                        $YetaWF.createElement("div", { class: "t_scroller", unselectable: "on" },
                            $YetaWF.createElement("ul", { unselectable: "on", class: "t_list", tabindex: "-1", "aria-hidden": "true", "aria-live": "off", "data-role": "staticlist", role: "listbox" }))));
            var minDate = new Date(this.Setup.MinDate);
            var maxDate = new Date(this.Setup.MaxDate);
            var ul = $YetaWF.getElement1BySelector("ul", [this.YearPopup]);
            var startYear = minDate.getFullYear();
            var endYear = maxDate.getFullYear();
            var selValue = this.TempCalCurrentDateUser.getFullYear();
            var html = "";
            for (var i = startYear; i <= endYear; ++i) {
                var extra = "";
                if (i === selValue)
                    extra = " t_selected";
                html += "<li tabindex=\"-1\" role=\"option\" unselectable=\"on\" class=\"t_item".concat(extra, "\" data-value=\"").concat(i, "\">").concat(i, "</li>"); //Format
            }
            ul.innerHTML = html;
            var yearSel = $YetaWF.getElement1BySelector(".t_header .t_year", [this.CalendarPopup]);
            var style = window.getComputedStyle(yearSel);
            this.YearPopup.style.font = style.font;
            this.YearPopup.style.fontStyle = style.fontStyle;
            this.YearPopup.style.fontWeight = style.fontWeight;
            this.YearPopup.style.fontSize = style.fontSize;
            $YetaWF.positionLeftAlignedBelow(yearSel, this.YearPopup);
            document.body.appendChild(this.YearPopup);
            var sel = $YetaWF.getElement1BySelectorCond(".t_list .t_selected", [this.YearPopup]);
            if (sel) {
                var selRect = sel.getBoundingClientRect();
                var list = $YetaWF.getElement1BySelector(".t_scroller", [this.YearPopup]);
                var listRect = list.getBoundingClientRect();
                list.scrollTop = selRect.top - listRect.top;
                //sel.scrollIntoView({ behavior: "auto", block: "nearest", inline: "start" }); // causes outer page jump
            }
            $YetaWF.registerEventHandler(this.YearPopup, "mousedown", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                var value = Number($YetaWF.getAttribute(li, "data-value"));
                _this.tempYearSelectedValue = value;
                return false;
            });
            $YetaWF.registerEventHandler(this.YearPopup, "mouseup", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                var value = Number($YetaWF.getAttribute(li, "data-value"));
                if (_this.tempYearSelectedValue === value) {
                    var date = _this.TempCalCurrentDateUser;
                    date.setFullYear(value);
                    var tbody = $YetaWF.getElement1BySelector(".t_calendarbody tbody", [_this.CalendarPopup]);
                    _this.buildCalendarMonthPage(tbody, date);
                    _this.updateCalendarTitle();
                    _this.tempYearSelectedValue = null;
                    _this.closeYearList();
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.YearPopup, "click", "ul li", function (ev) {
                return false;
            });
            $YetaWF.registerEventHandler(this.YearPopup, "mouseover", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "t_hover");
                $YetaWF.elementAddClass(li, "t_hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.YearPopup, "mouseout", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "t_hover");
                return true;
            });
        };
        DateTimeEditComponent.prototype.closeYearList = function () {
            if (this.YearPopup) {
                this.YearPopup.remove();
                this.YearPopup = null;
            }
        };
        DateTimeEditComponent.prototype.openMonthList = function () {
            var _this = this;
            this.closeYearList();
            this.closeMonthList();
            if (!this.CalendarPopup)
                return;
            this.MonthPopup =
                $YetaWF.createElement("div", { id: DateTimeEditComponent.MONTHPOPUPID, "data-owner": this.ControlId, "aria-hidden": "false" },
                    $YetaWF.createElement("div", { class: "t_container", "data-role": "popup", "aria-hidden": "false" },
                        $YetaWF.createElement("div", { class: "t_scroller", unselectable: "on" },
                            $YetaWF.createElement("ul", { unselectable: "on", class: "t_list", tabindex: "-1", "aria-hidden": "true", "aria-live": "off", "data-role": "staticlist", role: "listbox" }))));
            var ul = $YetaWF.getElement1BySelector("ul", [this.MonthPopup]);
            var startMonth = 0;
            var endMonth = 11;
            var selValue = this.TempCalCurrentDateUser.getMonth();
            var html = "";
            for (var i = startMonth; i <= endMonth; ++i) {
                var extra = "";
                if (i === selValue)
                    extra = " t_selected";
                html += "<li tabindex=\"-1\" role=\"option\" unselectable=\"on\" class=\"t_item".concat(extra, "\" data-value=\"").concat(i, "\">").concat(this.MonthNames[i], "</li>"); //Format
            }
            ul.innerHTML = html;
            var monthSel = $YetaWF.getElement1BySelector(".t_header .t_month", [this.CalendarPopup]);
            var style = window.getComputedStyle(monthSel);
            this.MonthPopup.style.font = style.font;
            this.MonthPopup.style.fontStyle = style.fontStyle;
            this.MonthPopup.style.fontWeight = style.fontWeight;
            this.MonthPopup.style.fontSize = style.fontSize;
            $YetaWF.positionLeftAlignedBelow(monthSel, this.MonthPopup);
            document.body.appendChild(this.MonthPopup);
            var sel = $YetaWF.getElement1BySelectorCond(".t_list .t_selected", [this.MonthPopup]);
            if (sel) {
                var selRect = sel.getBoundingClientRect();
                var list = $YetaWF.getElement1BySelector(".t_scroller", [this.MonthPopup]);
                var listRect = list.getBoundingClientRect();
                list.scrollTop = selRect.top - listRect.top;
                //sel.scrollIntoView({ behavior: "auto", block: "nearest", inline: "start" }); // causes outer page jump
            }
            $YetaWF.registerEventHandler(this.MonthPopup, "mousedown", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                var value = Number($YetaWF.getAttribute(li, "data-value"));
                _this.tempMonthSelectedValue = value;
                return false;
            });
            $YetaWF.registerEventHandler(this.MonthPopup, "mouseup", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                var value = Number($YetaWF.getAttribute(li, "data-value"));
                if (_this.tempMonthSelectedValue === value) {
                    var date = _this.TempCalCurrentDateUser;
                    date.setMonth(value);
                    var tbody = $YetaWF.getElement1BySelector(".t_calendarbody tbody", [_this.CalendarPopup]);
                    _this.buildCalendarMonthPage(tbody, date);
                    _this.updateCalendarTitle();
                    _this.tempMonthSelectedValue = null;
                    _this.closeMonthList();
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.MonthPopup, "click", "ul li", function (ev) {
                return false;
            });
            $YetaWF.registerEventHandler(this.MonthPopup, "mouseover", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "t_hover");
                $YetaWF.elementAddClass(li, "t_hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.MonthPopup, "mouseout", "ul li", function (ev) {
                var li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "t_hover");
                return true;
            });
        };
        DateTimeEditComponent.prototype.closeMonthList = function () {
            if (this.MonthPopup) {
                this.MonthPopup.remove();
                this.MonthPopup = null;
            }
        };
        DateTimeEditComponent.prototype.updateCalendarTitle = function () {
            if (!this.CalendarPopup)
                return;
            var month = this.MonthNames[this.TempCalCurrentDateUser.getMonth()];
            var year = this.TempCalCurrentDateUser.getFullYear();
            var monthSpan = $YetaWF.getElement1BySelector(".t_header .t_month", [this.CalendarPopup]);
            monthSpan.innerHTML = month;
            var yearSpan = $YetaWF.getElement1BySelector(".t_header .t_year", [this.CalendarPopup]);
            yearSpan.innerHTML = year.toFixed(0);
        };
        DateTimeEditComponent.prototype.getFormattedDateTime = function (date) {
            var time = date.getHours() * 60 + date.getMinutes();
            switch (this.Setup.Style) {
                default:
                case DateTimeStyleEnum.DateTime:
                    return this.getFormattedDate(date) + " " + this.getFormattedTime(time);
                case DateTimeStyleEnum.Date:
                    return this.getFormattedDate(date);
                case DateTimeStyleEnum.Time:
                    return this.getFormattedTime(time);
            }
        };
        DateTimeEditComponent.prototype.getFormattedDate = function (date) {
            var d = date.getDate();
            var m = date.getMonth() + 1;
            var y = date.getFullYear();
            switch (YLocs.YetaWF_ComponentsHTML.DateFormat) {
                default:
                case DateFormatEnum.MMDDYYYY:
                    return "".concat(this.zeroPad(m, 2), "/").concat(this.zeroPad(d, 2), "/").concat(y);
                case DateFormatEnum.MMDDYYYYdash:
                    return "".concat(this.zeroPad(m, 2), "-").concat(this.zeroPad(d, 2), "-").concat(y);
                case DateFormatEnum.MMDDYYYYdot:
                    return "".concat(this.zeroPad(m, 2), ".").concat(this.zeroPad(d, 2), ".").concat(y);
                case DateFormatEnum.DDMMYYYY:
                    return "".concat(this.zeroPad(d, 2), "/").concat(this.zeroPad(m, 2), "/").concat(y);
                case DateFormatEnum.DDMMYYYYdash:
                    return "".concat(this.zeroPad(d, 2), "-").concat(this.zeroPad(m, 2), "-").concat(y);
                case DateFormatEnum.DDMMYYYYdot:
                    return "".concat(this.zeroPad(d, 2), ".").concat(this.zeroPad(m, 2), ".").concat(y);
                case DateFormatEnum.YYYYMMDD:
                    return "".concat(y, "/").concat(this.zeroPad(m, 2), "/").concat(this.zeroPad(d, 2));
                case DateFormatEnum.YYYYMMDDdash:
                    return "".concat(y, "-").concat(this.zeroPad(m, 2), "-").concat(this.zeroPad(d, 2));
                case DateFormatEnum.YYYYMMDDdot:
                    return "".concat(y, ".").concat(this.zeroPad(m, 2), ".").concat(this.zeroPad(d, 2));
            }
        };
        DateTimeEditComponent.prototype.getFormattedTime = function (time) {
            var h = Math.floor(time / 60);
            var m = time % 60;
            switch (YLocs.YetaWF_ComponentsHTML.TimeFormat) {
                default:
                case TimeFormatEnum.HHMMAM:
                case TimeFormatEnum.HHMMSSAM:
                case TimeFormatEnum.HHMMAMdot:
                case TimeFormatEnum.HHMMSSAMdot:
                    var hour = h % 12;
                    if (hour === 0)
                        hour += 12;
                    switch (YLocs.YetaWF_ComponentsHTML.TimeFormat) {
                        default:
                        case TimeFormatEnum.HHMMAM:
                        case TimeFormatEnum.HHMMSSAM:
                            return "".concat(this.zeroPad(hour, 2), ":").concat(this.zeroPad(m, 2), " ").concat(h < 12 ? "AM" : "PM");
                        case TimeFormatEnum.HHMMAMdot:
                        case TimeFormatEnum.HHMMSSAMdot:
                            return "".concat(this.zeroPad(hour, 2), ".").concat(this.zeroPad(m, 2), " ").concat(h < 12 ? "AM" : "PM");
                    }
                case TimeFormatEnum.HHMM:
                case TimeFormatEnum.HHMMSS:
                    return "".concat(this.zeroPad(h, 2), ":").concat(this.zeroPad(m, 2));
                case TimeFormatEnum.HHMMdot:
                case TimeFormatEnum.HHMMSSdot:
                    return "".concat(this.zeroPad(h, 2), ".").concat(this.zeroPad(m, 2));
            }
        };
        DateTimeEditComponent.prototype.zeroPad = function (val, pos) {
            if (val < 0)
                return val.toFixed();
            var s = val.toFixed(0);
            while (s.length < pos)
                s = "0" + s;
            return s;
        };
        DateTimeEditComponent.prototype.getLongFormattedDate = function (date) {
            var day = date.getDay();
            var dom = date.getDate();
            var m = date.getMonth();
            var y = date.getFullYear();
            switch (YLocs.YetaWF_ComponentsHTML.DateFormat) {
                default:
                case DateFormatEnum.MMDDYYYY:
                case DateFormatEnum.MMDDYYYYdash:
                case DateFormatEnum.MMDDYYYYdot:
                    return "".concat(this.WeekDays[day], ", ").concat(this.MonthNames[m], " ").concat(dom, ", ").concat(y);
                case DateFormatEnum.DDMMYYYY:
                case DateFormatEnum.DDMMYYYYdash:
                case DateFormatEnum.DDMMYYYYdot:
                    return "".concat(this.WeekDays[day], ", ").concat(dom, " ").concat(this.MonthNames[m], ", ").concat(y);
                case DateFormatEnum.YYYYMMDD:
                case DateFormatEnum.YYYYMMDDdash:
                case DateFormatEnum.YYYYMMDDdot:
                    return "".concat(y, ", ").concat(this.MonthNames[m], " ").concat(dom, ", ").concat(this.WeekDays[day]);
            }
        };
        DateTimeEditComponent.prototype.setSelectedIndex = function (popup, index) {
            var list = $YetaWF.getElement1BySelector(".t_list", [popup]);
            var entries = $YetaWF.getElementsBySelector("li", [list]);
            var active = $YetaWF.getElement1BySelectorCond("li.t_selected", [list]);
            if (active)
                $YetaWF.elementRemoveClass(active, "t_selected");
            if (index < 0)
                index = 0;
            if (index >= entries.length)
                index = entries.length - 1;
            if (index >= 0)
                $YetaWF.elementAddClass(entries[index], "t_selected");
            entries[index].scrollIntoView({ behavior: "auto", block: "nearest", inline: "start" });
        };
        DateTimeEditComponent.prototype.getSelectedIndex = function (popup) {
            var list = $YetaWF.getElement1BySelector(".t_list", [popup]);
            var entries = $YetaWF.getElementsBySelector("li", [list]);
            var active = $YetaWF.getElement1BySelectorCond("li.t_selected", [list]);
            if (!active)
                return -1;
            var index = entries.indexOf(active);
            return index;
        };
        DateTimeEditComponent.prototype.getSelectedValue = function (popup) {
            var active = $YetaWF.getElement1BySelectorCond("li.t_selected", [popup]);
            if (!active)
                return null;
            var value = $YetaWF.getAttribute(active, "data-value");
            return value;
        };
        DateTimeEditComponent.prototype.setHidden = function (dateVal) {
            var s = "";
            if (dateVal != null) {
                if (this.Setup.Style === DateTimeStyleEnum.DateTime || this.Setup.Style === DateTimeStyleEnum.Time || (this.Setup.Style === DateTimeStyleEnum.Date && !this.Setup.UtcMidnight))
                    s = "".concat(dateVal.getUTCFullYear(), "-").concat(this.zeroPad(dateVal.getUTCMonth() + 1, 2), "-").concat(this.zeroPad(dateVal.getUTCDate(), 2), "T").concat(this.zeroPad(dateVal.getUTCHours(), 2), ":").concat(this.zeroPad(dateVal.getUTCMinutes(), 2), ":00.000Z");
                else if (this.Setup.Style === DateTimeStyleEnum.Date && this.Setup.UtcMidnight)
                    s = "".concat(dateVal.getUTCFullYear(), "-").concat(this.zeroPad(dateVal.getUTCMonth() + 1, 2), "-").concat(this.zeroPad(dateVal.getUTCDate(), 2), "T00:00:00.000Z");
            }
            this.InputHidden.setAttribute("value", s);
        };
        DateTimeEditComponent.prototype.setHiddenInvalid = function (dateVal) {
            this.InputHidden.setAttribute("value", dateVal);
        };
        Object.defineProperty(DateTimeEditComponent.prototype, "SelectedUser", {
            get: function () {
                return this._SelectedUser;
            },
            set: function (date) {
                this._SelectedUser = new Date(date);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTimeEditComponent.prototype, "TempCalCurrentDateUser", {
            get: function () {
                return this._TempCalCurrentDateUser;
            },
            set: function (date) {
                this._TempCalCurrentDateUser = new Date(date);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTimeEditComponent.prototype, "TimeSelectedUser", {
            get: function () {
                return this.SelectedUser.getHours() * 60 + this.SelectedUser.getMinutes();
            },
            set: function (value) {
                var d = new Date(this.SelectedUser);
                d.setHours(value / 60);
                d.setMinutes(value % 60);
                d.setSeconds(0);
                d.setMilliseconds(0);
                this.SelectedUser = d;
                this.InputControl.value = this.getFormattedDateTime(this.SelectedUser);
                this.setHidden(this.SelectedUser);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTimeEditComponent.prototype, "dateSelectedValue", {
            get: function () {
                return this.SelectedUser;
            },
            set: function (date) {
                var time = this.TimeSelectedUser;
                this.SelectedUser = date;
                this.TimeSelectedUser = time;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTimeEditComponent.prototype, "value", {
            // API
            get: function () {
                return new Date(this.dateSelectedValue);
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTimeEditComponent.prototype, "valueText", {
            get: function () {
                return this.InputHidden.value;
            },
            enumerable: false,
            configurable: true
        });
        DateTimeEditComponent.prototype.clear = function () {
            this.SelectedUser = new Date();
            this.InputControl.value = "";
            this.setHidden(null);
        };
        DateTimeEditComponent.prototype.enable = function (enabled) {
            this.closeTimeList();
            $YetaWF.elementEnableToggle(this.InputControl, enabled);
            $YetaWF.elementEnableToggle(this.InputHidden, enabled);
            $YetaWF.elementRemoveClass(this.Control, "t_disabled");
            if (!enabled)
                $YetaWF.elementAddClass(this.Control, "t_disabled");
        };
        Object.defineProperty(DateTimeEditComponent.prototype, "enabled", {
            get: function () {
                return !$YetaWF.elementHasClass(this.Control, "t_disabled");
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(DateTimeEditComponent.prototype, "focused", {
            get: function () {
                return $YetaWF.elementHasClass(this.Control, "t_focused");
            },
            enumerable: false,
            configurable: true
        });
        DateTimeEditComponent.prototype.close = function () {
            ToolTipsHTMLHelper.removeTooltips();
            this.closeMonthList();
            this.closeYearList();
            this.closeTimeList();
            this.closeCalendar();
        };
        DateTimeEditComponent.closeAll = function () {
            var ctrls = $YetaWF.getElementsBySelector(DateTimeEditComponent.SELECTOR);
            for (var _i = 0, ctrls_1 = ctrls; _i < ctrls_1.length; _i++) {
                var ctrl = ctrls_1[_i];
                var c = DateTimeEditComponent.getControlFromTag(ctrl, DateTimeEditComponent.SELECTOR);
                if (c.IgnoreResizeUntil < Date.now())
                    c.close();
            }
        };
        DateTimeEditComponent.TEMPLATE = "yt_datetime";
        DateTimeEditComponent.SELECTOR = ".yt_datetime.t_edit";
        DateTimeEditComponent.EVENTCHANGE = "datetime_change";
        DateTimeEditComponent.TIMEPOPUPID = "yt_datetime_popup";
        DateTimeEditComponent.CALENDARPOPUPID = "yt_datetime_calendarpopup";
        DateTimeEditComponent.YEARPOPUPID = "yt_datetime_popup";
        DateTimeEditComponent.MONTHPOPUPID = "yt_datetime_popup";
        DateTimeEditComponent.WARNDELAY = 300;
        return DateTimeEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.DateTimeEditComponent = DateTimeEditComponent;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        DateTimeEditComponent.closeAll();
        return true;
    });
    YetaWF_ComponentsHTML_Validation.registerValidator("componentshtml_datetime", function (form, elem, val) {
        var comp = DateTimeEditComponent.getControlFromTag(elem, DateTimeEditComponent.SELECTOR);
        return comp.validateInput(false);
    });
    YetaWF_ComponentsHTML_Validation.registerValidator("componentshtml_date", function (form, elem, val) {
        var comp = DateTimeEditComponent.getControlFromTag(elem, DateTimeEditComponent.SELECTOR);
        return comp.validateInput(false);
    });
    YetaWF_ComponentsHTML_Validation.registerValidator("componentshtml_timeofday", function (form, elem, val) {
        var comp = DateTimeEditComponent.getControlFromTag(elem, DateTimeEditComponent.SELECTOR);
        return comp.validateInput(false);
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=DateTimeEdit.js.map
