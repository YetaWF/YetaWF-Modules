/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class MenuHandler {

        public static ClearInterval: number = 0;

        private static readonly ClearTime = 1000;

        public static clearMenus(quick: boolean): void {

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

            var editIcons = $YetaWF.getElementsBySelector(".yModuleMenuEditIcon");
            if (editIcons.length) {
                var menus = $YetaWF.getElementsBySelector(".yModuleMenu");
                // hide all module menus
                for (let menu of menus) {
                    menu.style.display = "none";
                }
                // hide all edit icons
                for (let editIcon of editIcons) {
                    editIcon.style.display = "none";
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

            console.log("Entering module");

            if (MenuHandler.ClearInterval)
                clearInterval(MenuHandler.ClearInterval);
            MenuHandler.ClearInterval = 0;

            var modDiv = ev.__YetaWFElem as HTMLDivElement;

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
            MenuHandler.ClearInterval = setInterval((): void => { MenuHandler.clearMenus(false); }, MenuHandler.ClearTime);

            return true;
        }
        private static onHandleModuleMouseLeave(ev: MouseEvent): boolean {

            console.log("Exiting module");

            if (MenuHandler.ClearInterval)
                clearInterval(MenuHandler.ClearInterval);
            MenuHandler.ClearInterval = setInterval((): void => { MenuHandler.clearMenus(false); }, MenuHandler.ClearTime);

            var modDiv = ev.__YetaWFElem as HTMLDivElement;
            $YetaWF.elementRemoveClass(modDiv, "yModule-current");

            return true;
        }

        /** Show/hide menu as we're hovering over the edit icon */
        private static onHandleEditIconMouseEnter(ev: MouseEvent): boolean {

            console.log("Entering edit icon");

            var modDiv = ev.__YetaWFElem as HTMLDivElement;
            // find the module's menu
            var menu = $YetaWF.getElement1BySelector(".yModuleMenu", [modDiv]);

            menu.style.display = "";
            return true;
        }
    }

    MenuHandler.registerMouseEnterHandlers();
}