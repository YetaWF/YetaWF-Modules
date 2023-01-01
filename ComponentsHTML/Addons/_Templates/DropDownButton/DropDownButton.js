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
    var DropDownButtonComponent = /** @class */ (function (_super) {
        __extends(DropDownButtonComponent, _super);
        function DropDownButtonComponent(controlId) {
            var _this = _super.call(this, controlId, DropDownButtonComponent.TEMPLATE, DropDownButtonComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            $YetaWF.registerEventHandler(_this.Control, "click", null, function (ev) {
                $YetaWF.sendCustomEvent(_this.Control, DropDownButtonComponent.CLICKEDEVENT);
                return false;
            });
            $YetaWF.registerMultipleEventHandlers([_this.Control], ["click", "mousedown"], null, function (ev) {
                return false;
            });
            return _this;
        }
        DropDownButtonComponent.TEMPLATE = "yt_dropdownbutton";
        DropDownButtonComponent.SELECTOR = ".yt_dropdownbutton";
        DropDownButtonComponent.CLICKEDEVENT = "dropdownbutton_clicked";
        DropDownButtonComponent.menusOpen = 0;
        return DropDownButtonComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.DropDownButtonComponent = DropDownButtonComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=DropDownButton.js.map
