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
    var ModuleActionsComponent = /** @class */ (function (_super) {
        __extends(ModuleActionsComponent, _super);
        function ModuleActionsComponent(controlId, setup) {
            var _this = _super.call(this, controlId, ModuleActionsComponent.TEMPLATE, ModuleActionsComponent.SELECTOR, {
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
        ModuleActionsComponent.prototype.openMenu = function () {
            var menuDiv = this.MenuControl.cloneNode(true);
            menuDiv.id = "".concat(this.MenuControl.id, "_live");
            document.body.appendChild(menuDiv);
            new YetaWF_ComponentsHTML.MenuULComponent(menuDiv.id, { "Owner": this.MenuControl, "AutoOpen": true, "AutoRemove": true, "AttachTo": this.ButtonControl.Control, Dynamic: true });
        };
        ModuleActionsComponent.TEMPLATE = "yt_moduleactions";
        ModuleActionsComponent.SELECTOR = ".yt_moduleactions";
        return ModuleActionsComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.ModuleActionsComponent = ModuleActionsComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=ModuleActions.js.map
