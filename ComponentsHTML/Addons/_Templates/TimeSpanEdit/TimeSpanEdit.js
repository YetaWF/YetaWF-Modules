"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
    //interface Setup {
    //}
    var TimeSpanEditComponent = /** @class */ (function (_super) {
        __extends(TimeSpanEditComponent, _super);
        function TimeSpanEditComponent(controlId /*, setup: Setup*/) {
            var _this = _super.call(this, controlId, TimeSpanEditComponent.TEMPLATE, TimeSpanEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: TimeSpanEditComponent.EVENTCHANGE,
                GetValue: function (control) {
                    return control.Hidden.value;
                },
                Enable: function (control, enable, clearOnDisable) {
                    YetaWF_BasicsImpl.elementEnableToggle(control.Hidden, enable);
                    if (control.InputDays)
                        control.InputDays.enable(enable);
                    if (control.InputHours)
                        control.InputHours.enable(enable);
                    if (control.InputMins)
                        control.InputMins.enable(enable);
                    if (control.InputSecs)
                        control.InputSecs.enable(enable);
                    if (!enable && clearOnDisable) {
                        if (control.InputDays)
                            control.InputDays.clear();
                        if (control.InputHours)
                            control.InputHours.clear();
                        if (control.InputMins)
                            control.InputMins.clear();
                        if (control.InputSecs)
                            control.InputSecs.clear();
                    }
                },
            }) || this;
            _this.Hidden = $YetaWF.getElement1BySelector("input[type='hidden']", [_this.Control]);
            _this.InputDays = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond("input[name$='Days']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [_this.Control]);
            _this.InputHours = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond("input[name$='Hours']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [_this.Control]);
            _this.InputMins = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond("input[name$='Minutes']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [_this.Control]);
            _this.InputSecs = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond("input[name$='Seconds']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [_this.Control]);
            // capture changes in all edit controls
            if (_this.InputDays) {
                _this.InputDays.Control.addEventListener(YetaWF_ComponentsHTML.IntValueEditComponent.EVENT, function (evt) {
                    _this.updateValue();
                });
            }
            if (_this.InputHours) {
                _this.InputHours.Control.addEventListener(YetaWF_ComponentsHTML.IntValueEditComponent.EVENT, function (evt) {
                    _this.updateValue();
                });
            }
            if (_this.InputMins) {
                _this.InputMins.Control.addEventListener(YetaWF_ComponentsHTML.IntValueEditComponent.EVENT, function (evt) {
                    _this.updateValue();
                });
            }
            if (_this.InputSecs) {
                _this.InputSecs.Control.addEventListener(YetaWF_ComponentsHTML.IntValueEditComponent.EVENT, function (evt) {
                    _this.updateValue();
                });
            }
            return _this;
        }
        TimeSpanEditComponent.prototype.sendUpdateEvent = function () {
            var event = document.createEvent("Event");
            event.initEvent(TimeSpanEditComponent.EVENTCHANGE, false, true);
            this.Control.dispatchEvent(event);
        };
        TimeSpanEditComponent.prototype.updateValue = function () {
            if (this.InputDays && this.InputHours && this.InputMins && this.InputSecs) {
                this.Hidden.value = "".concat(this.InputDays.value, ".").concat(this.zeroPad(this.InputHours.value, 2), ":").concat(this.zeroPad(this.InputMins.value, 2), ":").concat(this.zeroPad(this.InputSecs.value, 2));
            }
            else if (this.InputDays && this.InputHours && this.InputMins) {
                this.Hidden.value = "".concat(this.InputDays.value, ".").concat(this.zeroPad(this.InputHours.value, 2), ":").concat(this.zeroPad(this.InputMins.value, 2));
            }
            else if (this.InputDays && this.InputHours) {
                this.Hidden.value = "".concat(this.InputDays.value, ".").concat(this.InputHours.value, ":0");
            }
            else if (this.InputHours && this.InputMins && this.InputSecs) {
                this.Hidden.value = "".concat(this.zeroPad(this.InputHours.value, 2), ":").concat(this.zeroPad(this.InputMins.value, 2), ":").concat(this.zeroPad(this.InputSecs.value, 2));
            }
            else if (this.InputHours && this.InputMins) {
                this.Hidden.value = "".concat(this.zeroPad(this.InputHours.value, 2), ":").concat(this.zeroPad(this.InputMins.value, 2));
            }
            this.sendUpdateEvent();
        };
        TimeSpanEditComponent.prototype.zeroPad = function (val, pos) {
            if (val < 0)
                return val.toFixed();
            var s = val.toFixed(0);
            while (s.length < pos)
                s = "0" + s;
            return s;
        };
        TimeSpanEditComponent.TEMPLATE = "yt_timespan";
        TimeSpanEditComponent.SELECTOR = ".yt_timespan.t_edit";
        TimeSpanEditComponent.EVENTCHANGE = "timespan_change";
        return TimeSpanEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.TimeSpanEditComponent = TimeSpanEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=TimeSpanEdit.js.map
