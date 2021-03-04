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
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var MaskedEditComponent = /** @class */ (function (_super) {
        __extends(MaskedEditComponent, _super);
        function MaskedEditComponent(controlId, setup, template, selector, eventChange) {
            var _this = _super.call(this, controlId, template, selector, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: eventChange,
                GetValue: function (control) {
                    var v = control.Hidden.value;
                    return v;
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }) || this;
            _this.EVENTCHANGE = eventChange;
            _this.Setup = setup;
            _this.Hidden = $YetaWF.getElement1BySelector("input[type='hidden']", [_this.Control]);
            _this.Input = $YetaWF.getElement1BySelector("input[type='text']", [_this.Control]);
            _this.value = _this.Hidden.value;
            var warn = $YetaWF.createElement("div", { class: "t_warn", style: "display:none" });
            warn.innerHTML = YConfigs.YetaWF_ComponentsHTML.SVG_fas_exclamation_triangle;
            _this.Input.insertAdjacentElement("afterend", warn);
            $YetaWF.registerEventHandler(_this.Input, "keypress", null, function (ev) {
                if (ev.key.length !== 1)
                    return true; // special key, like Enter
                if (!_this.isValidKeyPress(ev)) {
                    _this.flashError();
                    return false;
                }
                setTimeout(function () {
                    var caret = _this.Input.selectionStart;
                    _this.setHidden(_this.mergeForHidden(_this.Input.value, _this.Setup.Mask));
                    _this.Input.value = _this.mergeForOutput(_this.Hidden.value, _this.Setup.Mask);
                    _this.setCaretForward(caret);
                }, 1);
                return true;
            });
            $YetaWF.registerEventHandler(_this.Input, "keydown", null, function (ev) {
                var key = ev.key;
                if (!ev.altKey) {
                    switch (key) {
                        case "Backspace":
                            setTimeout(function () {
                                var caret = _this.Input.selectionStart;
                                _this.setHidden(_this.mergeForHidden(_this.Input.value, _this.Setup.Mask));
                                _this.Input.value = _this.mergeForOutput(_this.Hidden.value, _this.Setup.Mask);
                                _this.setCaretBackward(caret);
                            }, 1);
                            break;
                        case "Delete":
                            setTimeout(function () {
                                var caret = _this.Input.selectionStart;
                                _this.setHidden(_this.mergeForHidden(_this.Input.value, _this.Setup.Mask));
                                _this.Input.value = _this.mergeForOutput(_this.Hidden.value, _this.Setup.Mask);
                                _this.setCaretForward(caret);
                            }, 1);
                            break;
                    }
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.Input, "paste", null, function (ev) {
                setTimeout(function () {
                    var caret = _this.Input.selectionStart;
                    _this.setHidden(_this.mergeForHidden(_this.Input.value, _this.Setup.Mask));
                    _this.Input.value = _this.mergeForOutput(_this.Hidden.value, _this.Setup.Mask);
                    _this.setCaretBackward(caret);
                }, 1);
                return true;
            });
            $YetaWF.registerEventHandler(_this.Input, "focusin", null, function (ev) {
                _this.Input.value = _this.mergeForOutput(_this.Hidden.value, _this.Setup.Mask);
                _this.Input.select();
                return true;
            });
            $YetaWF.registerEventHandler(_this.Input, "focusout", null, function (ev) {
                _this.setHidden(_this.mergeForHidden(_this.Input.value, _this.Setup.Mask));
                if (!_this.Hidden.value)
                    _this.Input.value = "";
                return true;
            });
            return _this;
        }
        MaskedEditComponent.prototype.isValidKeyPress = function (ev) {
            var caret = this.Input.selectionStart;
            var mask = this.Setup.Mask;
            if (caret == null || caret < 0)
                return false;
            var v = ev.key;
            for (;;) {
                if (caret >= mask.length)
                    return false;
                var m = mask[caret];
                if (!m)
                    return false;
                if (m) {
                    switch (m) {
                        case "N": // 0-9
                            if (v >= "0" && v <= "9")
                                return true;
                            return false;
                        case "-":
                            break;
                        //case ...
                        // TODO: add other options
                        default:
                            throw "Invalid mask character " + m;
                    }
                }
                caret++;
            }
        };
        MaskedEditComponent.prototype.setCaretForward = function (pos) {
            if (pos == null) {
                this.Input.select();
            }
            else {
                var found = false;
                --pos;
                if (pos >= 0) {
                    var len = this.Input.value.length;
                    var maskLen = this.Setup.Mask.length;
                    // find the non-separator we just entered
                    while (pos < len && pos < maskLen) {
                        var m = this.Setup.Mask[pos];
                        switch (m) {
                            case "N": // 0-9
                                found = true;
                                break;
                            case "-":
                                break;
                            default:
                                throw "Invalid mask character " + m;
                        }
                        if (found)
                            break;
                        ++pos;
                    }
                    ++pos;
                    // skip if we're now on a separator
                    found = false;
                    while (pos < len && pos < maskLen) {
                        var m = this.Setup.Mask[pos];
                        switch (m) {
                            case "N": // 0-9
                                found = true;
                                break;
                            case "-":
                                break;
                            default:
                                throw "Invalid mask character " + m;
                        }
                        if (found)
                            break;
                        ++pos;
                    }
                    this.Input.selectionStart = pos;
                    this.Input.selectionEnd = pos;
                }
                else {
                    this.Input.selectionStart = 0;
                    this.Input.selectionEnd = 0;
                }
            }
        };
        MaskedEditComponent.prototype.setCaretBackward = function (pos) {
            if (pos == null) {
                this.Input.select();
            }
            else {
                this.Input.selectionStart = pos;
                this.Input.selectionEnd = pos;
            }
        };
        MaskedEditComponent.prototype.mergeForOutput = function (value, mask) {
            var out = "";
            for (;;) {
                var m = mask.substring(0, 1);
                mask = mask.substring(1);
                var v = value.substring(0, 1);
                value = value.substring(1);
                if (!m)
                    break;
                if (m) {
                    switch (m) {
                        case "N": // 0-9
                            if (v >= "0" && v <= "9")
                                out += v;
                            else if (v === "_")
                                out += v;
                            else {
                                out += "_";
                                value = "";
                            }
                            break;
                        case "-":
                            out += m;
                            value = v + value; // push back
                            break;
                        //case ...
                        // TODO: add other options
                        default:
                            throw "Invalid mask character " + m;
                    }
                }
                if (!mask)
                    break;
            }
            return out;
        };
        MaskedEditComponent.prototype.mergeForHidden = function (value, mask) {
            var out = "";
            for (;;) {
                var m = mask.substring(0, 1);
                mask = mask.substring(1);
                var v = value.substring(0, 1);
                value = value.substring(1);
                if (!m)
                    break;
                if (m) {
                    if (v === "_") {
                        v = "";
                        mask = m + mask; // push back
                        continue;
                    }
                    else if (v === "-") {
                        v = "";
                        mask = m + mask; // push back
                        continue;
                    }
                    switch (m) {
                        case "N": // 0-9
                            if (v >= "0" && v <= "9")
                                out += v;
                            else
                                value = v + value; // push back
                            break;
                        case "-":
                            value = v + value; // push back
                            break;
                        //case ...
                        // TODO: add other options
                        default: // assume some other separator
                            throw "Invalid mask character " + m;
                    }
                }
                if (!mask)
                    break;
            }
            return out;
        };
        MaskedEditComponent.prototype.setHidden = function (value) {
            if (this.Hidden.value !== value) {
                this.Hidden.value = value;
                this.sendChangeEvent();
            }
        };
        MaskedEditComponent.prototype.sendChangeEvent = function () {
            $YetaWF.sendCustomEvent(this.Control, this.EVENTCHANGE);
        };
        MaskedEditComponent.prototype.flashError = function () {
            var warn = $YetaWF.getElement1BySelector(".t_warn", [this.Control]);
            warn.style.display = "";
            setTimeout(function () {
                warn.style.display = "none";
            }, MaskedEditComponent.WARNDELAY);
        };
        Object.defineProperty(MaskedEditComponent.prototype, "value", {
            // API
            get: function () {
                return this.Hidden.value;
            },
            set: function (value) {
                this.Hidden.value = value;
                if (this.Hidden.value)
                    this.Input.value = this.mergeForOutput(this.Hidden.value, this.Setup.Mask);
                else
                    this.Input.value = "";
            },
            enumerable: false,
            configurable: true
        });
        MaskedEditComponent.prototype.clear = function () {
            this.value = "";
        };
        MaskedEditComponent.prototype.enable = function (enabled) {
            $YetaWF.elementEnableToggle(this.Input, enabled);
        };
        MaskedEditComponent.WARNDELAY = 300;
        return MaskedEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.MaskedEditComponent = MaskedEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=MaskedEdit.js.map
