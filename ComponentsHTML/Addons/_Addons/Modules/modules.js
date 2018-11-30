"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var MenuHandler = /** @class */ (function () {
        function MenuHandler() {
        }
        MenuHandler.clearMenus = function (quick) {
            // clear the interval
            if (MenuHandler.ClearInterval)
                clearInterval(MenuHandler.ClearInterval);
            MenuHandler.ClearInterval = 0;
            if (!quick) {
                // if we still have a current module, we can't clear menus
                if ($YetaWF.getElement1BySelectorCond(".yModule.yModule-current")) {
                    MenuHandler.ClearInterval = setInterval(function () { MenuHandler.clearMenus(false); }, MenuHandler.ClearTime);
                    return;
                }
            }
            var editIcons = $YetaWF.getElementsBySelector(".yModuleMenuEditIcon");
            if (editIcons.length) {
                var menus = $YetaWF.getElementsBySelector(".yModuleMenu");
                // hide all module menus
                for (var _i = 0, menus_1 = menus; _i < menus_1.length; _i++) {
                    var menu = menus_1[_i];
                    menu.style.display = "none";
                }
                // hide all edit icons
                for (var _a = 0, editIcons_1 = editIcons; _a < editIcons_1.length; _a++) {
                    var editIcon = editIcons_1[_a];
                    editIcon.style.display = "none";
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
            console.log("Entering module");
            if (MenuHandler.ClearInterval)
                clearInterval(MenuHandler.ClearInterval);
            MenuHandler.ClearInterval = 0;
            var modDiv = ev.__YetaWFElem;
            // add a class to the module to identify it's the current module
            $YetaWF.elementRemoveClass(modDiv, "yModule-current");
            $YetaWF.elementAddClass(modDiv, "yModule-current");
            // find the module's edit icon
            var editIcon = $YetaWF.getElement1BySelectorCond(".yModuleMenuEditIcon", [modDiv]);
            if (editIcon) {
                // entered a new module - clear all module menus that may be open
                MenuHandler.clearMenus(true);
                // fade in edit icon
                ComponentsHTMLHelper.fadeIn(editIcon, 500);
            }
            MenuHandler.ClearInterval = setInterval(function () { MenuHandler.clearMenus(false); }, MenuHandler.ClearTime);
            return true;
        };
        MenuHandler.onHandleModuleMouseLeave = function (ev) {
            console.log("Exiting module");
            if (MenuHandler.ClearInterval)
                clearInterval(MenuHandler.ClearInterval);
            MenuHandler.ClearInterval = setInterval(function () { MenuHandler.clearMenus(false); }, MenuHandler.ClearTime);
            var modDiv = ev.__YetaWFElem;
            $YetaWF.elementRemoveClass(modDiv, "yModule-current");
            return true;
        };
        /** Show/hide menu as we're hovering over the edit icon */
        MenuHandler.onHandleEditIconMouseEnter = function (ev) {
            console.log("Entering edit icon");
            var modDiv = ev.__YetaWFElem;
            // find the module's menu
            var menu = $YetaWF.getElement1BySelector(".yModuleMenu", [modDiv]);
            menu.style.display = "";
            return true;
        };
        MenuHandler.ClearInterval = 0;
        MenuHandler.ClearTime = 1000;
        return MenuHandler;
    }());
    YetaWF_ComponentsHTML.MenuHandler = MenuHandler;
    MenuHandler.registerMouseEnterHandlers();
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
