/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface ActionIconsSetup {
        MenuId: string;
    }

    export class ActionIconsComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_actionicons";
        public static readonly SELECTOR: string = ".yt_actionicons";

        private MenuControl: HTMLDivElement;
        private ButtonControl: DropDownButtonComponent;

        constructor(controlId: string, setup: ActionIconsSetup) {
            super(controlId, ActionIconsComponent.TEMPLATE, ActionIconsComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });

            this.MenuControl = $YetaWF.getElementById(setup.MenuId) as HTMLDivElement;
            this.ButtonControl = DropDownButtonComponent.getControlFromSelector("button", DropDownButtonComponent.SELECTOR, [this.Control]);

            $YetaWF.registerCustomEventHandler(this.ButtonControl, DropDownButtonComponent.CLICKEDEVENT, null, (ev: CustomEvent):boolean => {
                if (!YetaWF_ComponentsHTML.MenuULComponent.closeMenus())
                    this.openMenu();
                return false;
            });
        }

        private openMenu(): void {
            let menuDiv = this.MenuControl.cloneNode(true) as HTMLDivElement;
            menuDiv.id = `${this.MenuControl.id}_live`;
            document.body.appendChild(menuDiv);
            new YetaWF_ComponentsHTML.MenuULComponent(menuDiv.id, { "Owner": this.MenuControl, "AutoOpen": true, "AutoRemove": true, "AttachTo": this.ButtonControl.Control, Dynamic: true });
        }
    }
}

