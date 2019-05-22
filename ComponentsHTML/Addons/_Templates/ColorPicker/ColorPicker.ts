/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class ColorPickerComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_colorpicker";
        public static readonly SELECTOR: string = ".yt_colorpicker.t_edit";

        constructor(controlId: string, setup: kendo.ui.ColorPickerOptions) {
            super(controlId, ColorPickerComponent.TEMPLATE, ColorPickerComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: "colorpicker_change",
                GetValue: (control: ColorPickerComponent): string | null => {
                    let colorPicker = $(control.Control).data("kendoColorPicker");
                    return colorPicker.value();
                },
                Enable: (control: ColorPickerComponent, enable: boolean): void => {
                    let colorPicker = $(control.Control).data("kendoColorPicker");
                    colorPicker.enable(enable);
                },
            }, false, (tag: HTMLElement, control: ColorPickerComponent): void => {
                let colorPicker = $(control.Control).data("kendoColorPicker");
                colorPicker.destroy();
            });
            $(`#${controlId}`).kendoColorPicker(setup);
        }
    }
}
