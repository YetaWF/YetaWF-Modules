"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var MenuHandler = /** @class */ (function () {
        function MenuHandler() {
        }
        MenuHandler.clearMenus = function (force) {
            if (!force) {
                // if we still have a current module, we can't clear menus
                if ($YetaWF.getElement1BySelectorCond(".yModule.yModule-current"))
                    return;
            }
            // clear the interval
            if (MenuHandler.ClearInterval)
                clearInterval(MenuHandler.ClearInterval);
            MenuHandler.ClearInterval = 0;
            var editIcons = $YetaWF.getElementsBySelector(".yModuleMenuEditIcon");
            if (editIcons.length === 0)
                return; // modules don't necessarily have an edit menu
            var menus = $YetaWF.getElementsBySelector(".yModuleMenu");
            for (var _i = 0, menus_1 = menus; _i < menus_1.length; _i++) {
                var menu = menus_1[_i];
                // hide all module menus
                menu.style.display = "none";
                // there is no longer a current module
                $YetaWF.elementRemoveClass(menu, "yModule-current");
            }
            for (var _a = 0, editIcons_1 = editIcons; _a < editIcons_1.length; _a++) {
                var editIcon = editIcons_1[_a];
                if (force) {
                    editIcon.style.display = "none";
                }
                else {
                    ComponentsHTMLHelper.fadeOut(editIcon, 200);
                }
            }
        };
        MenuHandler.registerMouseEnterHandlers = function () {
            var mods = $YetaWF.getElementsBySelector(".yModule");
            for (var _i = 0, mods_1 = mods; _i < mods_1.length; _i++) {
                var mod = mods_1[_i];
                $YetaWF.registerEventHandler(mod, "mouseenter", null, function (ev) {
                    return MenuHandler.onHandleModuleMouseEnter(ev);
                });
                $YetaWF.registerEventHandler(mod, "mouseleave", null, function (ev) {
                    return MenuHandler.onHandleModuleMouseLeave(ev);
                });
            }
            var editIcons = $YetaWF.getElementsBySelector(".yModuleMenuEditIcon");
            for (var _a = 0, editIcons_2 = editIcons; _a < editIcons_2.length; _a++) {
                var editIcon = editIcons_2[_a];
                $YetaWF.registerEventHandler(editIcon, "mouseenter", null, function (ev) {
                    return MenuHandler.onHandleEditIconMouseEnter(ev);
                });
            }
        };
        MenuHandler.onHandleModuleMouseEnter = function (ev) {
            //console.log("Entering module");
            var modDiv = ev.__YetaWFElem;
            // check if this module is already current
            if ($YetaWF.elementHasClass(modDiv, "yModule-current"))
                return true;
            // find the module's edit icon
            var editIcon = $YetaWF.getElement1BySelectorCond(".yModuleMenuEditIcon", [modDiv]);
            if (!editIcon) // it just doesn't have a menu
                return true;
            // find the module's menu
            var menu = $YetaWF.getElement1BySelector(".yModuleMenu", [modDiv]);
            // if the edit icon is already visible, we're done
            if ($YetaWF.isVisible(menu))
                return true;
            // entered a new module - clear all module menus that may be open
            MenuHandler.clearMenus(true);
            // add a class to the module to identify it's the current module
            $YetaWF.elementAddClass(modDiv, "yModule-current");
            //if (YVolatile.Basics.EditModeActive) { }
            // fade in edit icon
            ComponentsHTMLHelper.fadeIn(editIcon, 500);
            MenuHandler.ClearInterval = setInterval(function () { MenuHandler.clearMenus(false); }, 1500);
            return true;
        };
        MenuHandler.onHandleModuleMouseLeave = function (ev) {
            //console.log("Exiting module");
            var modDiv = ev.__YetaWFElem;
            $YetaWF.elementRemoveClass(modDiv, "yModule-current");
            return true;
        };
        /** Show/hide menu as we're hovering over the edit icon */
        MenuHandler.onHandleEditIconMouseEnter = function (ev) {
            //console.log("Entering edit icon");
            var modDiv = ev.__YetaWFElem;
            // find the module's menu
            var menu = $YetaWF.getElement1BySelector(".yModuleMenu", [modDiv]);
            menu.style.display = "";
            return true;
        };
        MenuHandler.ClearInterval = 0;
        return MenuHandler;
    }());
    YetaWF_ComponentsHTML.MenuHandler = MenuHandler;
    MenuHandler.registerMouseEnterHandlers();
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Modules.js.map
