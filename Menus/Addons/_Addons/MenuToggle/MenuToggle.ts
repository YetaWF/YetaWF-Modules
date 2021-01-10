/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

namespace YetaWF_Menus {

    interface Setup {
        ButtonId: string;
        Target: string;
    }

    export class MenuToggleModule extends YetaWF.ModuleBaseDataImpl {

        public static readonly SELECTOR: string = ".YetaWF_Menus_MenuToggle";

        private Setup: Setup;
        private Button: HTMLButtonElement;

        constructor(id: string, setup: Setup) {
            super(id, MenuToggleModule.SELECTOR, null);
            this.Setup = setup;

            this.Button = $YetaWF.getElementByIdCond(this.Setup.ButtonId) as HTMLButtonElement;
            this.updateButton();

            $YetaWF.registerEventHandler(this.Button, "click", null, (ev: MouseEvent): boolean =>{
                let menus = YetaWF.ComponentBaseDataImpl.getControls<YetaWF_ComponentsHTML.MenuComponent>(YetaWF_ComponentsHTML.MenuComponent.SELECTOR, $YetaWF.getElementsBySelector(this.Setup.Target));
                for (let menu of menus) {
                    if (menu.isShown)
                        menu.hide();
                    else
                        menu.show();
                }
                return false;
            });
        }

        public updateButton(): void {
            let menus = YetaWF.ComponentBaseDataImpl.getControls<YetaWF_ComponentsHTML.MenuComponent>(YetaWF_ComponentsHTML.MenuComponent.SELECTOR, $YetaWF.getElementsBySelector(this.Setup.Target));
            for (let menu of menus) {
                this.Button.style.display = menu.isSmall ? "" : "none";
            }
        }
    }

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: CustomEvent<YetaWF.DetailsEventContainerResize>): boolean => {
        let toggleMods = YetaWF.ModuleBaseDataImpl.getModules<MenuToggleModule>(MenuToggleModule.SELECTOR, [ev.detail.container]);
        for (let toggleMod of toggleMods) {
            toggleMod.updateButton();
        }
        return true;
    });
}

