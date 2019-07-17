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
    var DecimalEditComponent = /** @class */ (function (_super) {
        __extends(DecimalEditComponent, _super);
        function DecimalEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, DecimalEditComponent.TEMPLATE, DecimalEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: "decimal_change",
                GetValue: function (control) {
                    return control.valueText;
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                },
            }, false, function (tag, control) {
                control.kendoNumericTextBox.destroy();
            }) || this;
            $(_this.Control).kendoNumericTextBox({
                format: "0.00",
                min: setup.Min, max: setup.Max,
                culture: YVolatile.Basics.Language,
                downArrowText: "",
                upArrowText: "",
                change: function (e) {
                    $(_this.Control).trigger("change");
                    var event = document.createEvent("Event");
                    event.initEvent("decimal_change", true, true);
                    _this.Control.dispatchEvent(event);
                    FormsSupport.validateElement(_this.Control);
                }
            });
            _this.kendoNumericTextBox = $(_this.Control).data("kendoNumericTextBox");
            return _this;
        }
        Object.defineProperty(DecimalEditComponent.prototype, "value", {
            get: function () {
                return this.kendoNumericTextBox.value();
            },
            set: function (val) {
                if (val == null)
                    this.kendoNumericTextBox.value("");
                else
                    this.kendoNumericTextBox.value(val);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(DecimalEditComponent.prototype, "valueText", {
            get: function () {
                return this.value ? this.value.toString() : "";
            },
            enumerable: true,
            configurable: true
        });
        DecimalEditComponent.prototype.clear = function () {
            this.kendoNumericTextBox.value("");
        };
        DecimalEditComponent.prototype.enable = function (enabled) {
            this.kendoNumericTextBox.enable(enabled);
        };
        DecimalEditComponent.TEMPLATE = "yt_decimal";
        DecimalEditComponent.SELECTOR = "input.yt_decimal.t_edit.k-input[name]";
        return DecimalEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.DecimalEditComponent = DecimalEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=DecimalEdit.js.map
