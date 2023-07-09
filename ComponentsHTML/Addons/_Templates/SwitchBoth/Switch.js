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
    var SwitchComponent = /** @class */ (function (_super) {
        __extends(SwitchComponent, _super);
        function SwitchComponent(controlId) {
            var _this = _super.call(this, controlId, SwitchComponent.TEMPLATE, SwitchComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: SwitchComponent.EVENTCHANGE,
                GetValue: function (control) {
                    return control.Input.checked ? "true" : "false";
                },
                Enable: function (control, enable, clearOnDisable) {
                    YetaWF_BasicsImpl.elementEnableToggle(control.Input, enable);
                    if (clearOnDisable)
                        control.Input.checked = false;
                },
            }) || this;
            _this.Input = $YetaWF.getElement1BySelector("input", [_this.Control]);
            _this.Label = $YetaWF.getElement1BySelector("label", [_this.Control]);
            $YetaWF.registerEventHandler(_this.Label, "keypress", null, function (ev) {
                if (_this.Input.disabled)
                    return true;
                if (ev.key.length !== 1)
                    return true; // special key, like Enter
                if (ev.key === " ") {
                    _this.Input.checked = !_this.Input.checked;
                    _this.sendChangeEvent();
                    return false;
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.Input, "change", null, function (ev) {
                _this.sendChangeEvent();
                return true;
            });
            return _this;
        }
        SwitchComponent.prototype.sendChangeEvent = function () {
            var event = document.createEvent("Event");
            event.initEvent(SwitchComponent.EVENTCHANGE, false, true);
            this.Control.dispatchEvent(event);
        };
        SwitchComponent.TEMPLATE = "yt_switch";
        SwitchComponent.SELECTOR = ".yt_switch";
        SwitchComponent.EVENTCHANGE = "switch_change";
        return SwitchComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.SwitchComponent = SwitchComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Switch.js.map
