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
    var CurrencyEditComponent = /** @class */ (function (_super) {
        __extends(CurrencyEditComponent, _super);
        function CurrencyEditComponent(controlId, setup) {
            return _super.call(this, controlId, setup, CurrencyEditComponent.TEMPLATE, CurrencyEditComponent.SELECTOR) || this;
        }
        CurrencyEditComponent.TEMPLATE = "yt_currency";
        CurrencyEditComponent.SELECTOR = ".yt_currency.t_edit";
        // events duplicated from NumberEditComponentBase to avoid changes in component users
        CurrencyEditComponent.EVENT = "number_changespin"; // combines change and spin
        CurrencyEditComponent.EVENTCHANGE = "number_change";
        CurrencyEditComponent.EVENTSPIN = "number_spin";
        return CurrencyEditComponent;
    }(YetaWF_ComponentsHTML.NumberEditComponentBase));
    YetaWF_ComponentsHTML.CurrencyEditComponent = CurrencyEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Currency.js.map
