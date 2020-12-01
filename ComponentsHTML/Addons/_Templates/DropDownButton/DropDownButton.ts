/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/// <reference types="kendo-ui" />

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
            }, false, (tag: HTMLElement, control: DropDownButtonComponent): void => {
                let btn = $(control.Control).data("kendoButton");
                btn.destroy();
            });

            $(this.Control).kendoButton({ // kendo use
                click: (ev: kendo.ui.ButtonClickEvent): void => {
                    ev.preventDefault();
                    $YetaWF.sendCustomEvent(this.Control, DropDownButtonComponent.CLICKEDEVENT);
                }
            });
            $YetaWF.registerMultipleEventHandlers([this.Control], ["click", "mousedown"], null, (ev: Event):boolean => {
                return false;
            });
        }
    }
}

