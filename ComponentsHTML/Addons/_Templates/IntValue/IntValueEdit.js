"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
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
    var IntValueEditComponent = /** @class */ (function (_super) {
        __extends(IntValueEditComponent, _super);
        function IntValueEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, IntValueEditComponent.TEMPLATE, IntValueEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: "intvalue_change",
                GetValue: function (control) {
                    return control.value.toString();
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                },
            }, false, function (tag, control) {
                if (control.kendoNumericTextBox)
                    control.kendoNumericTextBox.destroy();
            }) || this;
            _this.kendoNumericTextBox = null;
            _this.InputControl = _this.Control;
            $(_this.InputControl).kendoNumericTextBox({
                decimals: 0, format: "n0",
                min: setup.Min, max: setup.Max,
                placeholder: setup.NoEntryText,
                step: setup.Step,
                downArrowText: "",
                upArrowText: "",
                change: function (e) {
                    $(_this.Control).trigger("change");
                    var event = document.createEvent("Event");
                    event.initEvent("intvalue_change", true, true);
                    _this.Control.dispatchEvent(event);
                    FormsSupport.validateElement(_this.Control);
                }
            });
            _this.kendoNumericTextBox = $(_this.InputControl).data("kendoNumericTextBox");
            return _this;
        }
        Object.defineProperty(IntValueEditComponent.prototype, "value", {
            get: function () {
                return parseInt(this.InputControl.value, 10);
            },
            set: function (val) {
                if (this.kendoNumericTextBox == null) {
                    this.InputControl.value = val.toString();
                }
                else {
                    this.kendoNumericTextBox.value(val);
                }
            },
            enumerable: true,
            configurable: true
        });
        IntValueEditComponent.prototype.clear = function () {
            if (this.kendoNumericTextBox == null) {
                this.InputControl.value = "";
            }
            else {
                this.kendoNumericTextBox.value("");
            }
        };
        IntValueEditComponent.prototype.enable = function (enabled) {
            if (this.kendoNumericTextBox == null) {
                $YetaWF.elementEnableToggle(this.InputControl, enabled);
            }
            else {
                this.kendoNumericTextBox.enable(enabled);
            }
        };
        IntValueEditComponent.TEMPLATE = "yt_intvalue_base";
        IntValueEditComponent.SELECTOR = "input.yt_intvalue_base.t_edit.k-input[name]";
        return IntValueEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.IntValueEditComponent = IntValueEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=IntValueEdit.js.map
