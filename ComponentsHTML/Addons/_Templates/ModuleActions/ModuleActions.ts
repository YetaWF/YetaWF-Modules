/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface ModuleActionsSetup {
        MenuId: string;
        OwningModuleGuid: string|null;
    }

    export class ModuleActionsComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_moduleactions";
        public static readonly SELECTOR: string = ".yt_moduleactions";

        private Setup: ModuleActionsSetup;
        private MenuControl: HTMLDivElement;
        private ButtonControl: DropDownButtonComponent;

        constructor(controlId: string, setup: ModuleActionsSetup) {
            super(controlId, ModuleActionsComponent.TEMPLATE, ModuleActionsComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });

            this.Setup = setup;
            this.MenuControl = $YetaWF.getElementById(setup.MenuId) as HTMLDivElement;
            this.ButtonControl = DropDownButtonComponent.getControlFromSelector("button", DropDownButtonComponent.SELECTOR, [this.Control]);

            $YetaWF.registerCustomEventHandler(this.ButtonControl.Control, DropDownButtonComponent.CLICKEDEVENT, null, (ev: CustomEvent):boolean => {
                if (!YetaWF_ComponentsHTML.MenuULComponent.closeMenus())
                    this.openMenu();
                return false;
            });
        }

        private openMenu(): void {
            let menuDiv = this.MenuControl.cloneNode(true) as HTMLDivElement;
            menuDiv.id = `${this.MenuControl.id}_live`;
            if (this.Setup.OwningModuleGuid)
                menuDiv.setAttribute(YConfigs.Basics.CssModuleGuid, this.Setup.OwningModuleGuid);
            document.body.appendChild(menuDiv);
            new YetaWF_ComponentsHTML.MenuULComponent(menuDiv.id, 
                { 
                    Owner: this.MenuControl, 
                    AutoOpen: true, 
                    AutoRemove: true, 
                    AttachTo: this.ButtonControl.Control, 
                    Dynamic: true 
                }
            );
        }
    }
}

