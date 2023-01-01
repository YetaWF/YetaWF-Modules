/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class DropDownButtonComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_dropdownbutton";
        public static readonly SELECTOR: string = ".yt_dropdownbutton";
        public static readonly CLICKEDEVENT: string = "dropdownbutton_clicked";

        public static menusOpen: number = 0;

        constructor(controlId: string) {
            super(controlId, DropDownButtonComponent.TEMPLATE, DropDownButtonComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });

            $YetaWF.registerEventHandler(this.Control, "click", null, (ev: MouseEvent): boolean =>{
                $YetaWF.sendCustomEvent(this.Control, DropDownButtonComponent.CLICKEDEVENT);
                return false;
            });
            $YetaWF.registerMultipleEventHandlers([this.Control], ["click", "mousedown"], null, (ev: Event):boolean => {
                return false;
            });
        }
    }
}

