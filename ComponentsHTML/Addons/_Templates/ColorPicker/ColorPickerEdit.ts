/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class ColorPickerEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_colorpicker";
        public static readonly SELECTOR: string = ".yt_colorpicker.t_edit";

        constructor(controlId: string, setup: kendo.ui.ColorPickerOptions) {
            super(controlId, ColorPickerEditComponent.TEMPLATE, ColorPickerEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: "colorpicker_change",
                GetValue: (control: ColorPickerEditComponent): string | null => {
                    let colorPicker = $(control.Control).data("kendoColorPicker");
                    return colorPicker.value();
                },
                Enable: (control: ColorPickerEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    let colorPicker = $(control.Control).data("kendoColorPicker");
                    colorPicker.enable(enable);
                    //if (clearOnDisable) { // resetting color doesn't work (8/7/2019 with 2019.2.619)
                    //    colorPicker.value(null as any); // "as any" because ts.d defines as string|undefined, only null resets color
                    //    colorPicker.color(null as any); // "as any" because ts.d defines as string|undefined, only null resets color
                    //}
                },
            }, false, (tag: HTMLElement, control: ColorPickerEditComponent): void => {
                let colorPicker = $(control.Control).data("kendoColorPicker");
                colorPicker.destroy();
            });

            setup.change = (ev: kendo.ui.ColorPickerChangeEvent): void => {
                FormsSupport.validateElement(this.Control);
                var event = document.createEvent("Event");
                event.initEvent("colorpicker_change", true, true);
                this.Control.dispatchEvent(event);
            }

            $(`#${controlId}`).kendoColorPicker(setup);
        }
    }
}
