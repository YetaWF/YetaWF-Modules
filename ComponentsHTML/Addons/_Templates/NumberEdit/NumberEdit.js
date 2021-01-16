"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var NumberEditComponentBase = /** @class */ (function (_super) {
        __extends(NumberEditComponentBase, _super);
        function NumberEditComponentBase(controlId, setup, template, selector) {
            var _this = _super.call(this, controlId, template, selector, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: NumberEditComponentBase.EVENT,
                GetValue: function (control) {
                    var v = control.Value;
                    if (!v)
                        return null;
                    return v.toString();
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }) || this;
            _this.Value = null;
            _this.Interval = 0;
            _this.Setup = setup;
            _this.InputControl = _this.Control;
            _this.Container = $YetaWF.elementClosest(_this.Control, ".yt_number_container");
            _this.setInternalValue(_this.InputControl.value);
            // icons used: fas-exclamation-triangle
            var warn = $YetaWF.createElement("div", { class: "t_warn", style: "display:none" });
            warn.innerHTML = "<svg aria-hidden='true' focusable='false' role='img' viewBox='0 0 576 512' xmlns='http://www.w3.org/2000/svg'><path fill='currentColor' d='M569.517 440.013C587.975 472.007 564.806 512 527.94 512H48.054c-36.937 0-59.999-40.055-41.577-71.987L246.423 23.985c18.467-32.009 64.72-31.951 83.154 0l239.94 416.028zM288 354c-25.405 0-46 20.595-46 46s20.595 46 46 46 46-20.595 46-46-20.595-46-46-46zm-43.673-165.346l7.418 136c.347 6.364 5.609 11.346 11.982 11.346h48.546c6.373 0 11.635-4.982 11.982-11.346l7.418-136c.375-6.874-5.098-12.654-11.982-12.654h-63.383c-6.884 0-12.356 5.78-11.981 12.654z'></path></svg>";
            // icons used: fas-caret-up, fas-caret-down
            var updown = $YetaWF.createElement("div", { class: "t_updown" },
                $YetaWF.createElement("div", { class: "t_up" }),
                $YetaWF.createElement("div", { class: "t_down" }));
            $YetaWF.getElement1BySelector(".t_up", [updown]).innerHTML = "<svg aria-hidden='true' focusable='false' role='img' viewBox='0 0 320 512' xmlns='http://www.w3.org/2000/svg'><path fill='currentColor' d='M288.662 352H31.338c-17.818 0-26.741-21.543-14.142-34.142l128.662-128.662c7.81-7.81 20.474-7.81 28.284 0l128.662 128.662c12.6 12.599 3.676 34.142-14.142 34.142z'></path></svg>";
            $YetaWF.getElement1BySelector(".t_down", [updown]).innerHTML = "<svg aria-hidden='true' focusable='false' role='img' viewBox='0 0 320 512'><path fill='currentColor' d='M31.3 192h257.3c17.8 0 26.7 21.5 14.1 34.1L174.1 354.8c-7.8 7.8-20.5 7.8-28.3 0L17.2 226.1C4.6 213.5 13.5 192 31.3 192z'></path></svg>";
            _this.Control.insertAdjacentElement("afterend", updown);
            _this.Control.insertAdjacentElement("afterend", warn);
            $YetaWF.registerMultipleEventHandlers([_this.InputControl], ["change", "input"], null, function (ev) {
                if (!_this.isValid(_this.InputControl.value)) {
                    _this.setInternalValue(_this.Value);
                    _this.flashError();
                    return false;
                }
                _this.setInternalValue(_this.InputControl.value, false);
                _this.sendChangeEvent();
                return true;
            });
            $YetaWF.registerEventHandler(_this.InputControl, "keydown", null, function (ev) {
                var key = ev.key;
                if (key === "ArrowDown" || key === "Down") {
                    _this.setNewSpinValue(false);
                    return false;
                }
                else if (key === "ArrowUp" || key === "Up") {
                    _this.setNewSpinValue(true);
                    return false;
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.Container, "mousedown", ".t_up", function (ev) {
                _this.InputControl.focus();
                _this.setNewSpinValue(true);
                _this.startSpin(true);
                return false;
            });
            $YetaWF.registerEventHandler(_this.Container, "mousedown", ".t_down", function (ev) {
                _this.InputControl.focus();
                _this.setNewSpinValue(false);
                _this.startSpin(false);
                return false;
            });
            $YetaWF.registerMultipleEventHandlers([_this.Container], ["mouseup", "mouseout"], ".t_down,.t_up", function (ev) {
                _this.clearSpin();
                return false;
            });
            $YetaWF.registerEventHandler(_this.InputControl, "focusin", null, function (ev) {
                if (_this.enabled) {
                    $YetaWF.elementRemoveClass(_this.Container, "t_focused");
                    $YetaWF.elementAddClass(_this.Container, "t_focused");
                    _this.setInternalValue(_this.Value);
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.InputControl, "focusout", null, function (ev) {
                $YetaWF.elementRemoveClass(_this.Container, "t_focused");
                _this.clearSpin();
                _this.setInternalValue(_this.Value);
                return true;
            });
            return _this;
        }
        // Spin
        NumberEditComponentBase.prototype.clearSpin = function () {
            if (this.Interval)
                clearInterval(this.Interval);
            this.Interval = 0;
        };
        NumberEditComponentBase.prototype.startSpin = function (up, timeout) {
            var _this = this;
            this.clearSpin();
            this.Interval = setInterval(function () {
                _this.setNewSpinValue(up);
                _this.clearSpin();
                _this.startSpin(up, NumberEditComponentBase.STEPDELAY);
            }, timeout === undefined ? NumberEditComponentBase.INITIALDELAY : timeout);
        };
        NumberEditComponentBase.prototype.setNewSpinValue = function (up) {
            var value;
            if (this.Value !== null) {
                value = this.value;
                value += up ? this.Setup.Step : -this.Setup.Step;
                value = Math.min(this.Setup.Max, value);
                value = Math.max(this.Setup.Min, value);
            }
            else {
                value = this.Setup.Min;
            }
            if (value !== this.Value) {
                this.setControlValue(value);
                this.sendSpinEvent();
            }
            else {
                this.clearSpin();
            }
        };
        // events
        NumberEditComponentBase.prototype.sendChangeEvent = function () {
            // $(this.Control).trigger("change");
            $YetaWF.sendCustomEvent(this.Control, NumberEditComponentBase.EVENT);
            $YetaWF.sendCustomEvent(this.Control, NumberEditComponentBase.EVENTCHANGE);
            FormsSupport.validateElement(this.Control);
        };
        NumberEditComponentBase.prototype.sendSpinEvent = function () {
            $YetaWF.sendCustomEvent(this.Control, NumberEditComponentBase.EVENT);
            $YetaWF.sendCustomEvent(this.Control, NumberEditComponentBase.EVENTSPIN);
        };
        Object.defineProperty(NumberEditComponentBase.prototype, "value", {
            // API
            get: function () {
                var _a;
                return (_a = this.Value) !== null && _a !== void 0 ? _a : 0;
            },
            set: function (val) {
                this.Value = val;
                if (this.focused)
                    this.InputControl.value = val.toString();
                else {
                    if (this.Setup.Currency && this.Setup.Locale) {
                        var v = val.toLocaleString(this.Setup.Locale, { style: "currency", currency: this.Setup.Currency });
                        // special case for $US
                        if (v.startsWith("$"))
                            v = "$ " + v.substring(1);
                        this.InputControl.value = v;
                    }
                    else {
                        var l = this.Setup.Lead ? this.Setup.Lead + " " : "";
                        var t = this.Setup.Trail ? " " + this.Setup.Trail : "";
                        this.InputControl.value = "" + l + val.toLocaleString(this.Setup.Locale, { style: "decimal", minimumFractionDigits: this.Setup.Digits, maximumFractionDigits: this.Setup.Digits }) + t;
                    }
                }
            },
            enumerable: false,
            configurable: true
        });
        NumberEditComponentBase.prototype.setInternalValue = function (val, updateIfValid) {
            if (typeof val === "string") {
                if (val) {
                    val = Number(val);
                    if (isNaN(val))
                        val = null;
                }
                else {
                    val = null;
                }
            }
            if (val != null) {
                if (updateIfValid === undefined || updateIfValid === true)
                    this.value = val;
                else
                    this.Value = val;
            }
            else {
                this.InputControl.value = "";
                this.Value = val;
            }
        };
        NumberEditComponentBase.prototype.setControlValue = function (val) {
            this.Value = val;
            if (this.Setup.Currency && this.Setup.Locale) {
                var v = val.toLocaleString(this.Setup.Locale, { style: "currency", currency: this.Setup.Currency });
                // special case for $US
                if (v.startsWith("$"))
                    v = "$ " + v.substring(1);
                this.InputControl.value = v;
            }
            else {
                var l = this.Setup.Lead ? this.Setup.Lead + " " : "";
                var t = this.Setup.Trail ? " " + this.Setup.Trail : "";
                this.InputControl.value = "" + l + val.toLocaleString(this.Setup.Locale, { style: "decimal", minimumFractionDigits: this.Setup.Digits, maximumFractionDigits: this.Setup.Digits }) + t;
            }
        };
        NumberEditComponentBase.prototype.isValid = function (text) {
            var val = Number(text);
            if (isNaN(val))
                return false;
            if (val > this.Setup.Max || val < this.Setup.Min)
                return false;
            return true;
        };
        Object.defineProperty(NumberEditComponentBase.prototype, "valueText", {
            get: function () {
                return this.Value != null ? this.value.toString() : "";
            },
            enumerable: false,
            configurable: true
        });
        NumberEditComponentBase.prototype.clear = function () {
            this.setInternalValue(null);
        };
        NumberEditComponentBase.prototype.enable = function (enabled) {
            $YetaWF.elementEnableToggle(this.InputControl, enabled);
            $YetaWF.elementRemoveClass(this.Container, "t_disabled");
            if (!enabled)
                $YetaWF.elementAddClass(this.Container, "t_disabled");
        };
        Object.defineProperty(NumberEditComponentBase.prototype, "enabled", {
            get: function () {
                return !$YetaWF.elementHasClass(this.Container, "t_disabled");
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(NumberEditComponentBase.prototype, "focused", {
            get: function () {
                return $YetaWF.elementHasClass(this.Container, "t_focused");
            },
            enumerable: false,
            configurable: true
        });
        NumberEditComponentBase.prototype.flashError = function () {
            var warn = $YetaWF.getElement1BySelector(".t_warn", [this.Container]);
            warn.style.display = "";
            setTimeout(function () {
                warn.style.display = "none";
            }, NumberEditComponentBase.WARNDELAY);
        };
        NumberEditComponentBase.EVENT = "number_changespin"; // combines change and spin
        NumberEditComponentBase.EVENTCHANGE = "number_change";
        NumberEditComponentBase.EVENTSPIN = "number_spin";
        NumberEditComponentBase.INITIALDELAY = 300;
        NumberEditComponentBase.STEPDELAY = 100;
        NumberEditComponentBase.WARNDELAY = 100;
        return NumberEditComponentBase;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.NumberEditComponentBase = NumberEditComponentBase;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=NumberEdit.js.map
