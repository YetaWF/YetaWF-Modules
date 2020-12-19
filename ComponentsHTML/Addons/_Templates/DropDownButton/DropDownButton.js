"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
/// <reference types="kendo-ui" />
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
            }, false, function (tag, control) {
                var btn = $(control.Control).data("kendoButton");
                btn.destroy();
            }) || this;
            $(_this.Control).kendoButton({
                click: function (ev) {
                    ev.preventDefault();
                    $YetaWF.sendCustomEvent(_this.Control, DropDownButtonComponent.CLICKEDEVENT);
                }
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
