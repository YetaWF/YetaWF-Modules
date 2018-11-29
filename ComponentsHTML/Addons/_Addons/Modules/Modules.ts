/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class MenuHandler {

        public static ClearInterval: number = 0;

        public static clearMenus(force: boolean): void {
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
            for (let menu of menus) {
                // hide all module menus
                menu.style.display = "none";
                // there is no longer a current module
                $YetaWF.elementRemoveClass(menu, "yModule-current");
            }

            for (let editIcon of editIcons) {
                if (force) {
                    editIcon.style.display = "none";
                } else {
                    ComponentsHTMLHelper.fadeOut(editIcon, 200);
                }
            }
        }

        public static registerMouseEnterHandlers(): void {
            var mods = $YetaWF.getElementsBySelector(".yModule");
            for (let mod of mods) {
                $YetaWF.registerEventHandler(mod, "mouseenter", null, (ev: MouseEvent): boolean => {
                    return MenuHandler.onHandleModuleMouseEnter(ev);
                });
                $YetaWF.registerEventHandler(mod, "mouseleave", null, (ev: MouseEvent): boolean => {
                    return MenuHandler.onHandleModuleMouseLeave(ev);
                });
            }
            var editIcons = $YetaWF.getElementsBySelector(".yModuleMenuEditIcon");
            for (let editIcon of editIcons) {
                $YetaWF.registerEventHandler(editIcon, "mouseenter", null, (ev: MouseEvent): boolean => {
                    return MenuHandler.onHandleEditIconMouseEnter(ev);
                });
            }
        }
        private static onHandleModuleMouseEnter(ev: MouseEvent): boolean {
            //console.log("Entering module");

            var modDiv = ev.__YetaWFElem as HTMLDivElement;
            // check if this module is already current
            if ($YetaWF.elementHasClass(modDiv, "yModule-current"))
                return true;

            // find the module's edit icon
            var editIcon = $YetaWF.getElement1BySelectorCond(".yModuleMenuEditIcon", [modDiv]);
            if (!editIcon)// it just doesn't have a menu
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

            MenuHandler.ClearInterval = setInterval((): void => { MenuHandler.clearMenus(false); }, 1500);

            return true;
        }
        private static onHandleModuleMouseLeave(ev: MouseEvent): boolean {

            //console.log("Exiting module");

            var modDiv = ev.__YetaWFElem as HTMLDivElement;
            $YetaWF.elementRemoveClass(modDiv, "yModule-current");
            return true;
        }

        /** Show/hide menu as we're hovering over the edit icon */
        private static onHandleEditIconMouseEnter(ev: MouseEvent): boolean {

            //console.log("Entering edit icon");

            var modDiv = ev.__YetaWFElem as HTMLDivElement;
            // find the module's menu
            var menu = $YetaWF.getElement1BySelector(".yModuleMenu", [modDiv]);

            menu.style.display = "";
            return true;
        }
    }

    MenuHandler.registerMouseEnterHandlers();
}