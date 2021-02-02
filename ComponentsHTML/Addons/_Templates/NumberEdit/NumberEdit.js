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
                    var v = control.InputHidden.value;
                    return v;
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
            _this.InputControl = $YetaWF.getElement1BySelector("input[type='text']", [_this.Control]);
            _this.InputHidden = $YetaWF.getElement1BySelector("input[type='hidden']", [_this.Control]);
            _this.setInternalValue(_this.InputControl.value);
            // icons used: fas-exclamation-triangle
            var warn = $YetaWF.createElement("div", { class: "t_warn", style: "display:none" });
            warn.innerHTML = YConfigs.YetaWF_ComponentsHTML.SVG_fas_exclamation_triangle;
            // icons used: fas-caret-up, fas-caret-down
            var updown = $YetaWF.createElement("div", { class: "t_updown" },
                $YetaWF.createElement("div", { class: "t_up" }),
                $YetaWF.createElement("div", { class: "t_down" }));
            $YetaWF.getElement1BySelector(".t_up", [updown]).innerHTML = YConfigs.YetaWF_ComponentsHTML.SVG_fas_caret_up;
            $YetaWF.getElement1BySelector(".t_down", [updown]).innerHTML = YConfigs.YetaWF_ComponentsHTML.SVG_fas_caret_down;
            _this.InputControl.insertAdjacentElement("afterend", updown);
            _this.InputControl.insertAdjacentElement("afterend", warn);
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
                switch (key) {
                    case "ArrowDown":
                    case "Down":
                        _this.setNewSpinValue(false);
                        return false;
                    case "ArrowUp":
                    case "Up":
                        _this.setNewSpinValue(true);
                        return false;
                }
                return true;
            });
            // deprecated, understood. Switch to beforeinput, but oh no IE11.
            $YetaWF.registerEventHandler(_this.InputControl, "keypress", null, function (ev) {
                var key = ev.key;
                switch (key) {
                    case "0":
                    case "1":
                    case "2":
                    case "3":
                    case "4":
                    case "5":
                    case "6":
                    case "7":
                    case "8":
                    case "9":
                        break;
                    case ".":
                        if (!_this.Setup.Digits) {
                            _this.flashError();
                            return false;
                        }
                        break;
                    default:
                        _this.flashError();
                        return false;
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "mousedown", ".t_up", function (ev) {
                _this.InputControl.focus();
                _this.setNewSpinValue(true);
                _this.startSpin(true);
                return false;
            });
            $YetaWF.registerEventHandler(_this.Control, "mousedown", ".t_down", function (ev) {
                _this.InputControl.focus();
                _this.setNewSpinValue(false);
                _this.startSpin(false);
                return false;
            });
            $YetaWF.registerMultipleEventHandlers([_this.Control], ["mouseup", "mouseout"], ".t_down,.t_up", function (ev) {
                _this.clearSpin();
                return false;
            });
            $YetaWF.registerEventHandler(_this.InputControl, "focusin", null, function (ev) {
                if (_this.enabled) {
                    $YetaWF.elementRemoveClass(_this.Control, "t_focused");
                    $YetaWF.elementAddClass(_this.Control, "t_focused");
                    _this.setInternalValue(_this.Value);
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.InputControl, "focusout", null, function (ev) {
                $YetaWF.elementRemoveClass(_this.Control, "t_focused");
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
                value = Number(value.toFixed(this.Setup.Digits));
            }
            else {
                value = this.Setup.Min;
            }
            if (value !== this.Value) {
                this.value = value;
                this.sendSpinEvent();
            }
            else {
                this.clearSpin();
            }
        };
        // events
        NumberEditComponentBase.prototype.sendChangeEvent = function () {
            $YetaWF.sendCustomEvent(this.Control, NumberEditComponentBase.EVENT);
            $YetaWF.sendCustomEvent(this.Control, NumberEditComponentBase.EVENTCHANGE);
            FormsSupport.validateElement(this.InputHidden);
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
                val = Number(val.toFixed(this.Setup.Digits));
                this.Value = val;
                this.InputHidden.value = val.toString();
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
                if (updateIfValid === undefined || updateIfValid === true) {
                    this.value = val;
                }
                else {
                    this.Value = val;
                    this.InputHidden.value = val.toString();
                }
            }
            else {
                this.InputControl.value = "";
                this.InputHidden.value = "";
                this.Value = val;
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
            $YetaWF.elementEnableToggle(this.InputHidden, enabled);
            $YetaWF.elementRemoveClass(this.Control, "t_disabled");
            if (!enabled)
                $YetaWF.elementAddClass(this.Control, "t_disabled");
        };
        Object.defineProperty(NumberEditComponentBase.prototype, "enabled", {
            get: function () {
                return !$YetaWF.elementHasClass(this.Control, "t_disabled");
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(NumberEditComponentBase.prototype, "focused", {
            get: function () {
                return $YetaWF.elementHasClass(this.Control, "t_focused");
            },
            enumerable: false,
            configurable: true
        });
        NumberEditComponentBase.prototype.flashError = function () {
            var warn = $YetaWF.getElement1BySelector(".t_warn", [this.Control]);
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
        NumberEditComponentBase.WARNDELAY = 300;
        return NumberEditComponentBase;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.NumberEditComponentBase = NumberEditComponentBase;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=NumberEdit.js.map
