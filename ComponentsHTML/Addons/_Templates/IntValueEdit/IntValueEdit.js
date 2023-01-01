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
    var IntValueEditComponent = /** @class */ (function (_super) {
        __extends(IntValueEditComponent, _super);
        function IntValueEditComponent(controlId, setup) {
            return _super.call(this, controlId, setup, IntValueEditComponent.TEMPLATE, IntValueEditComponent.SELECTOR) || this;
        }
        IntValueEditComponent.TEMPLATE = "yt_intvalue_base";
        IntValueEditComponent.SELECTOR = ".yt_intvalue_base.t_edit";
        // events duplicated from NumberEditComponentBase to avoid changes in component users
        IntValueEditComponent.EVENT = "number_changespin"; // combines change and spin
        IntValueEditComponent.EVENTCHANGE = "number_change";
        IntValueEditComponent.EVENTSPIN = "number_spin";
        return IntValueEditComponent;
    }(YetaWF_ComponentsHTML.NumberEditComponentBase));
    YetaWF_ComponentsHTML.IntValueEditComponent = IntValueEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=IntValueEdit.js.map
