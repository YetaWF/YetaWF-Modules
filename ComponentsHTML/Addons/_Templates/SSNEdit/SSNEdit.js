"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
    var SSNEditComponent = /** @class */ (function (_super) {
        __extends(SSNEditComponent, _super);
        function SSNEditComponent(controlId) {
            var _this = _super.call(this, controlId, SSNEditComponent.TEMPLATE, SSNEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: SSNEditComponent.EVENTCHANGE,
                GetValue: function (control) {
                    return control.valueRaw;
                },
                Enable: function (control, enable, clearOnDisable) {
                    YetaWF_BasicsImpl.elementEnableToggle(control.Hidden, enable);
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                }
            }, false, function (tag, control) {
                control.kendoMaskedTextBox.destroy();
            }) || this;
            _this.Hidden = $YetaWF.getElement1BySelector("input[type=\"hidden\"]", [_this.Control]);
            _this.SSN = $YetaWF.getElement1BySelector("input[name=\"ssninput\"]", [_this.Control]);
            $(_this.SSN).kendoMaskedTextBox({
                mask: "000-00-0000",
                change: function (ev) {
                    var kdMask = ev.sender;
                    var val = kdMask.raw();
                    _this.setHidden(val);
                    FormsSupport.validateElement(_this.Hidden);
                    var event = document.createEvent("Event");
                    event.initEvent(SSNEditComponent.EVENTCHANGE, true, true);
                    _this.Control.dispatchEvent(event);
                }
            });
            _this.kendoMaskedTextBox = $(_this.SSN).data("kendoMaskedTextBox");
            _this.setHidden(_this.kendoMaskedTextBox.raw());
            return _this;
        }
        SSNEditComponent.prototype.setHidden = function (val) {
            this.Hidden.setAttribute("value", val !== null && val !== void 0 ? val : "");
        };
        Object.defineProperty(SSNEditComponent.prototype, "valueRaw", {
            get: function () {
                return this.kendoMaskedTextBox.raw();
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(SSNEditComponent.prototype, "valueText", {
            get: function () {
                return this.kendoMaskedTextBox.value();
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(SSNEditComponent.prototype, "value", {
            set: function (val) {
                this.setHidden(val);
                this.kendoMaskedTextBox.value(val);
            },
            enumerable: false,
            configurable: true
        });
        SSNEditComponent.prototype.clear = function () {
            this.setHidden(null);
            this.kendoMaskedTextBox.value("");
        };
        SSNEditComponent.prototype.enable = function (enabled) {
            this.kendoMaskedTextBox.enable(enabled);
        };
        SSNEditComponent.TEMPLATE = "yt_ssn";
        SSNEditComponent.SELECTOR = ".yt_ssn.t_edit";
        SSNEditComponent.EVENTCHANGE = "ssn_change";
        return SSNEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.SSNEditComponent = SSNEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=SSNEdit.js.map
