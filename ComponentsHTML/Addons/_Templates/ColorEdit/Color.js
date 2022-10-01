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
    var ColorEditComponent = /** @class */ (function (_super) {
        __extends(ColorEditComponent, _super);
        //private Setup:ColorEditSetup;
        function ColorEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, ColorEditComponent.TEMPLATE, ColorEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: ColorEditComponent.EVENTCHANGE,
                GetValue: function (control) {
                    return control.value;
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                }
            }) || this;
            //this.Setup = setup;
            _this.Input = $YetaWF.getElement1BySelector("input[type='text']", [_this.Control]);
            _this.Color = $YetaWF.getElement1BySelector("input[type='color']", [_this.Control]);
            $YetaWF.registerMultipleEventHandlers([_this.Color], ["input", "change"], null, function (ev) {
                _this.Input.value = _this.Color.value;
                $YetaWF.sendCustomEvent(_this.Control, ColorEditComponent.EVENTCHANGE);
                return false;
            });
            $YetaWF.registerMultipleEventHandlers([_this.Input], ["input", "change"], null, function (ev) {
                if (_this.Input.value.trim().startsWith("#"))
                    _this.Color.value = _this.Input.value;
                else
                    _this.Color.value = "#FFFFFF";
                $YetaWF.sendCustomEvent(_this.Control, ColorEditComponent.EVENTCHANGE);
                return false;
            });
            return _this;
        }
        Object.defineProperty(ColorEditComponent.prototype, "value", {
            get: function () {
                return this.Input.value;
            },
            set: function (val) {
                this.Input.value = val;
                if (val.trim().startsWith("#"))
                    this.Color.value = val;
                else
                    this.Color.value = "#FFFFFF";
            },
            enumerable: false,
            configurable: true
        });
        ColorEditComponent.prototype.clear = function () {
            this.Input.value = "";
            this.Color.value = "";
        };
        ColorEditComponent.prototype.enable = function (enabled) {
            YetaWF_BasicsImpl.elementEnableToggle(this.Input, enabled);
            YetaWF_BasicsImpl.elementEnableToggle(this.Control, enabled);
        };
        ColorEditComponent.TEMPLATE = "yt_color";
        ColorEditComponent.SELECTOR = ".yt_color.t_edit";
        ColorEditComponent.EVENTCHANGE = "color_change";
        return ColorEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.ColorEditComponent = ColorEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Color.js.map
