"use strict";
/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
            // hide all module menus
            if (YetaWF_ComponentsHTML.MenuULComponent)
                YetaWF_ComponentsHTML.MenuULComponent.closeMenus();
            // hide all edit icons
            var editIcons = $YetaWF.getElementsBySelector(".yModuleMenuEditIcon");
            if (editIcons.length) {
                // hide all edit icons
                for (var _i = 0, editIcons_1 = editIcons; _i < editIcons_1.length; _i++) {
                    var editIcon = editIcons_1[_i];
                    editIcon.style.display = "none";
                }
            }
        };
        MenuHandler.registerModuleHandlers = function (modDiv) {
            var editIcon = $YetaWF.getElement1BySelectorCond(".yModuleMenuEditIcon", [modDiv]);
            if (editIcon) {
                $YetaWF.registerEventHandler(modDiv, "mouseenter", null, function (ev) {
                    if (MenuHandler.ClearInterval)
                        clearInterval(MenuHandler.ClearInterval);
                    MenuHandler.ClearInterval = 0;
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
                });
                $YetaWF.registerEventHandler(modDiv, "mouseleave", null, function (ev) {
                    if (MenuHandler.ClearInterval)
                        clearInterval(MenuHandler.ClearInterval);
                    MenuHandler.ClearInterval = setInterval(function () { MenuHandler.clearMenus(false); }, MenuHandler.ClearTime);
                    $YetaWF.elementRemoveClass(modDiv, "yModule-current");
                    return true;
                });
                $YetaWF.registerEventHandler(editIcon, "mouseenter", null, function (ev) {
                    // find the module's menu
                    var menuDiv = $YetaWF.getElement1BySelector(".yModuleMenu", [modDiv]);
                    var menu = YetaWF_ComponentsHTML.MenuULComponent.getControlFromTagCond(menuDiv, YetaWF_ComponentsHTML.MenuULComponent.SELECTOR);
                    if (!menu)
                        menu = new YetaWF_ComponentsHTML.MenuULComponent(menuDiv.id, { "Owner": editIcon, "AutoOpen": false, "AutoRemove": false, "AttachTo": null });
                    menu.open();
                    return true;
                });
            }
        };
        MenuHandler.registerModule = function (modDiv) {
            if (!$YetaWF.getAttributeCond(modDiv, "data-modreg")) {
                MenuHandler.registerModuleHandlers(modDiv);
                $YetaWF.setAttribute(modDiv, "data-modreg", "1"); // registered
            }
        };
        MenuHandler.ClearInterval = 0;
        MenuHandler.ClearTime = 1000;
        return MenuHandler;
    }());
    YetaWF_ComponentsHTML.MenuHandler = MenuHandler;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        if (YVolatile.Basics.EditModeActive) {
            var modDivs = YetaWF.ModuleBase.getModuleDivs(".yModule", ev.detail.containers);
            for (var _i = 0, modDivs_1 = modDivs; _i < modDivs_1.length; _i++) {
                var modDiv = modDivs_1[_i];
                MenuHandler.registerModule(modDiv);
            }
        }
        return true;
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Modules.js.map
