"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
    var CurrencyEditComponent = /** @class */ (function (_super) {
        __extends(CurrencyEditComponent, _super);
        function CurrencyEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, CurrencyEditComponent.TEMPLATE, CurrencyEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: CurrencyEditComponent.EVENT,
                GetValue: function (control) {
                    return control.value ? control.value.toString() : null;
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                }
            }, false, function (tag, control) {
                control.kendoNumericTextBox.destroy();
            }) || this;
            _this.Currency = $YetaWF.getElement1BySelector("input", [_this.Control]);
            $(_this.Currency).kendoNumericTextBox({
                format: YVolatile.YetaWF_ComponentsHTML.CurrencyFormat,
                min: setup.Min, max: setup.Max,
                culture: YVolatile.Basics.Language,
                change: function (e) {
                    $(_this.Control).trigger("change");
                    var event = document.createEvent("Event");
                    event.initEvent(CurrencyEditComponent.EVENT, true, true);
                    _this.Control.dispatchEvent(event);
                    FormsSupport.validateElement(_this.Currency);
                }
            });
            _this.kendoNumericTextBox = $(_this.Currency).data("kendoNumericTextBox");
            return _this;
        }
        Object.defineProperty(CurrencyEditComponent.prototype, "value", {
            get: function () {
                return this.kendoNumericTextBox.value();
            },
            enumerable: true,
            configurable: true
        });
        CurrencyEditComponent.prototype.enable = function (enable) {
            this.kendoNumericTextBox.enable(enable);
        };
        CurrencyEditComponent.prototype.clear = function () {
            this.kendoNumericTextBox.value("");
        };
        CurrencyEditComponent.TEMPLATE = "yt_currency";
        CurrencyEditComponent.SELECTOR = ".yt_currency.t_edit";
        CurrencyEditComponent.EVENT = "currency_change";
        return CurrencyEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.CurrencyEditComponent = CurrencyEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Currency.js.map
