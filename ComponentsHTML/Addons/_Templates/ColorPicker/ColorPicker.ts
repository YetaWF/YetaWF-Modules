/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class ColorPickerComponent {

        constructor(id: string, setup: kendo.ui.ColorPickerOptions) {
            $(`#${id}`).kendoColorPicker(setup);
        }
    }

    // A <div> is being emptied. Destroy all color pickers the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        var list = $YetaWF.getElementsBySelector(".yt_colorpicker.t_edit", [tag]);
        for (let el of list) {
            var colorpicker: kendo.ui.ColorPicker = $(el).data("kendoColorPicker");
            if (!colorpicker) throw "No kendo colorpicker object found";/*DEBUG*/
            colorpicker.destroy();
        }
    });
}
