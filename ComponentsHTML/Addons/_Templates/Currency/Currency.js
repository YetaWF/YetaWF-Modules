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
    var CurrencyEditComponent = /** @class */ (function (_super) {
        __extends(CurrencyEditComponent, _super);
        function CurrencyEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId) || this;
            _this.Currency = $YetaWF.getElement1BySelector("input", [_this.Control]);
            $(_this.Currency).kendoNumericTextBox({
                format: YVolatile.YetaWF_ComponentsHTML.CurrencyFormat,
                min: setup.Min, max: setup.Max,
                culture: YVolatile.Basics.Language
            });
            _this.kendoNumericTextBox = $(_this.Control).data("kendoNumericTextBox");
            return _this;
        }
        CurrencyEditComponent.SELECTOR = ".yt_currency.t_edit";
        return CurrencyEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.CurrencyEditComponent = CurrencyEditComponent;
    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv(function (tag) {
        YetaWF.ComponentBaseDataImpl.clearDiv(tag, CurrencyEditComponent.SELECTOR, function (control) {
            control.kendoNumericTextBox.destroy();
        });
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Currency.js.map
