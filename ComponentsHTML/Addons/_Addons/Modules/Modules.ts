/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class MenuHandler {

        private static ClearInterval: number = 0;
        private static readonly ClearTime: number = 1000;

        private static clearMenus(quick: boolean): void {

            // clear the interval
            if (MenuHandler.ClearInterval)
                clearInterval(MenuHandler.ClearInterval);
            MenuHandler.ClearInterval = 0;

            if (!quick) {
                // if we still have a current module, we can't clear menus
                if ($YetaWF.getElement1BySelectorCond(".yModule.yModule-current")) {
                    MenuHandler.ClearInterval = setInterval((): void => { MenuHandler.clearMenus(false); }, MenuHandler.ClearTime);
                    return;
                }
            }

            // hide all module menus
            if (MenuULComponent)
                MenuULComponent.closeMenus();
            // hide all edit icons
            let editIcons = $YetaWF.getElementsBySelector(".yModuleMenuEditIcon");
            if (editIcons.length) {
                // hide all edit icons
                for (let editIcon of editIcons) {
                    editIcon.style.display = "none";
                }
            }
        }

        private static registerModuleHandlers(modDiv: HTMLDivElement): void {

            let editIcon = $YetaWF.getElement1BySelectorCond(".yModuleMenuEditIcon", [modDiv]);
            if (editIcon) {

                $YetaWF.registerEventHandler(modDiv, "mouseenter", null, (ev: MouseEvent): boolean => {
                    if (MenuHandler.ClearInterval)
                        clearInterval(MenuHandler.ClearInterval);
                    MenuHandler.ClearInterval = 0;

                    // add a class to the module to identify it's the current module
                    $YetaWF.elementRemoveClass(modDiv, "yModule-current");
                    $YetaWF.elementAddClass(modDiv, "yModule-current");

                    // find the module's edit icon
                    let editIcon = $YetaWF.getElement1BySelectorCond(".yModuleMenuEditIcon", [modDiv]);
                    if (editIcon) {

                        // entered a new module - clear all module menus that may be open
                        MenuHandler.clearMenus(true);

                        // fade in edit icon
                        ComponentsHTMLHelper.fadeIn(editIcon, 500);
                    }
                    MenuHandler.ClearInterval = setInterval((): void => { MenuHandler.clearMenus(false); }, MenuHandler.ClearTime);

                    return true;
                });
                $YetaWF.registerEventHandler(modDiv, "mouseleave", null, (ev: MouseEvent): boolean => {
                    if (MenuHandler.ClearInterval)
                        clearInterval(MenuHandler.ClearInterval);
                    MenuHandler.ClearInterval = setInterval((): void => { MenuHandler.clearMenus(false); }, MenuHandler.ClearTime);

                    $YetaWF.elementRemoveClass(modDiv, "yModule-current");
                    return true;
                });

                $YetaWF.registerEventHandler(editIcon, "mouseenter", null, (ev: MouseEvent): boolean => {
                    // find the module's menu
                    var menuDiv = $YetaWF.getElement1BySelector(".yModuleMenu", [modDiv]);
                    let menu = YetaWF_ComponentsHTML.MenuULComponent.getControlFromTagCond<YetaWF_ComponentsHTML.MenuULComponent>(menuDiv, YetaWF_ComponentsHTML.MenuULComponent.SELECTOR);
                    if (!menu)
                        menu = new YetaWF_ComponentsHTML.MenuULComponent(menuDiv.id, {"Owner": editIcon!, "AutoOpen": false, "AutoRemove": false, "AttachTo": null });
                    menu.open();
                    return true;
                });
            }
        }

        public static registerModule(modDiv: HTMLDivElement): void {
            if (!$YetaWF.getAttributeCond(modDiv, "data-modreg")) {
                MenuHandler.registerModuleHandlers(modDiv);
                $YetaWF.setAttribute(modDiv, "data-modreg", "1");// registered
            }
        }
    }

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: CustomEvent<YetaWF.DetailsEventNavPageLoaded>): boolean => {
        if (YVolatile.Basics.EditModeActive) {
            let modDivs = YetaWF.ModuleBase.getModuleDivs(".yModule", ev.detail.containers);
            for (let modDiv of modDivs) {
                MenuHandler.registerModule(modDiv);
            }
        }
        return true;
    });
}