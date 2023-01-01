"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */
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
var YetaWF_Menus;
(function (YetaWF_Menus) {
    var MenuToggleModule = /** @class */ (function (_super) {
        __extends(MenuToggleModule, _super);
        function MenuToggleModule(id, setup) {
            var _this = _super.call(this, id, MenuToggleModule.SELECTOR, null) || this;
            _this.Setup = setup;
            _this.Button = $YetaWF.getElementByIdCond(_this.Setup.ButtonId);
            _this.updateButton();
            $YetaWF.registerEventHandler(_this.Button, "click", null, function (ev) {
                var menus = YetaWF.ComponentBaseDataImpl.getControls(YetaWF_ComponentsHTML.MenuComponent.SELECTOR, $YetaWF.getElementsBySelector(_this.Setup.Target));
                for (var _i = 0, menus_1 = menus; _i < menus_1.length; _i++) {
                    var menu = menus_1[_i];
                    if (menu.isShown)
                        menu.hide();
                    else
                        menu.show();
                }
                return false;
            });
            return _this;
        }
        MenuToggleModule.prototype.updateButton = function () {
            var menus = YetaWF.ComponentBaseDataImpl.getControls(YetaWF_ComponentsHTML.MenuComponent.SELECTOR, $YetaWF.getElementsBySelector(this.Setup.Target));
            if (menus.length > 0)
                this.Button.style.display = menus[0].isSmall ? "" : "none";
        };
        MenuToggleModule.SELECTOR = ".YetaWF_Menus_MenuToggle";
        return MenuToggleModule;
    }(YetaWF.ModuleBaseDataImpl));
    YetaWF_Menus.MenuToggleModule = MenuToggleModule;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        var toggleMods = YetaWF.ModuleBaseDataImpl.getModules(MenuToggleModule.SELECTOR, [ev.detail.container]);
        for (var _i = 0, toggleMods_1 = toggleMods; _i < toggleMods_1.length; _i++) {
            var toggleMod = toggleMods_1[_i];
            toggleMod.updateButton();
        }
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        var toggleMods = YetaWF.ModuleBaseDataImpl.getModules(MenuToggleModule.SELECTOR);
        for (var _i = 0, toggleMods_2 = toggleMods; _i < toggleMods_2.length; _i++) {
            var toggleMod = toggleMods_2[_i];
            toggleMod.updateButton();
        }
        return true;
    });
})(YetaWF_Menus || (YetaWF_Menus = {}));

//# sourceMappingURL=MenuToggle.js.map
