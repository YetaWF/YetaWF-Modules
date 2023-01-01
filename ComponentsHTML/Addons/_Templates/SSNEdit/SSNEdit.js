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
    var SSNEditComponent = /** @class */ (function (_super) {
        __extends(SSNEditComponent, _super);
        function SSNEditComponent(controlId, setup) {
            return _super.call(this, controlId, setup, SSNEditComponent.TEMPLATE, SSNEditComponent.SELECTOR, SSNEditComponent.EVENTCHANGE) || this;
        }
        SSNEditComponent.TEMPLATE = "yt_ssn";
        SSNEditComponent.SELECTOR = ".yt_ssn.t_edit";
        SSNEditComponent.EVENTCHANGE = "ssn_change";
        return SSNEditComponent;
    }(YetaWF_ComponentsHTML.MaskedEditComponent));
    YetaWF_ComponentsHTML.SSNEditComponent = SSNEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=SSNEdit.js.map
