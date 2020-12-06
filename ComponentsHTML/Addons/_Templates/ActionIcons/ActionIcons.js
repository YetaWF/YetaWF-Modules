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
    var ActionIconsComponent = /** @class */ (function (_super) {
        __extends(ActionIconsComponent, _super);
        function ActionIconsComponent(controlId, setup) {
            var _this = _super.call(this, controlId, ActionIconsComponent.TEMPLATE, ActionIconsComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            _this.MenuControl = $YetaWF.getElementById(setup.MenuId);
            _this.ButtonControl = YetaWF_ComponentsHTML.DropDownButtonComponent.getControlFromSelector("button", YetaWF_ComponentsHTML.DropDownButtonComponent.SELECTOR, [_this.Control]);
            $YetaWF.registerCustomEventHandler(_this.ButtonControl.Control, YetaWF_ComponentsHTML.DropDownButtonComponent.CLICKEDEVENT, null, function (ev) {
                if (!YetaWF_ComponentsHTML.MenuULComponent.closeMenus())
                    _this.openMenu();
                return false;
            });
            return _this;
        }
        ActionIconsComponent.prototype.openMenu = function () {
            var menuDiv = this.MenuControl.cloneNode(true);
            menuDiv.id = this.MenuControl.id + "_live";
            document.body.appendChild(menuDiv);
            new YetaWF_ComponentsHTML.MenuULComponent(menuDiv.id, { "Owner": this.MenuControl, "AutoOpen": true, "AutoRemove": true, "AttachTo": this.ButtonControl.Control, Dynamic: true });
        };
        ActionIconsComponent.TEMPLATE = "yt_actionicons";
        ActionIconsComponent.SELECTOR = ".yt_actionicons";
        return ActionIconsComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.ActionIconsComponent = ActionIconsComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=ActionIcons.js.map
