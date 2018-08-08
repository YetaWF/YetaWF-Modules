"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var ColorPickerComponent = /** @class */ (function () {
        function ColorPickerComponent(id, setup) {
            $("#" + id).kendoColorPicker(setup);
        }
        return ColorPickerComponent;
    }());
    YetaWF_ComponentsHTML.ColorPickerComponent = ColorPickerComponent;
    // A <div> is being emptied. Destroy all color pickers the <div> may contain.
    $YetaWF.addClearDiv(function (tag) {
        var list = $YetaWF.getElementsBySelector(".yt_colorpicker.t_edit", [tag]);
        for (var _i = 0, list_1 = list; _i < list_1.length; _i++) {
            var el = list_1[_i];
            var colorpicker = $(el).data("kendoColorPicker");
            if (!colorpicker)
                throw "No kendo colorpicker object found"; /*DEBUG*/
            colorpicker.destroy();
        }
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
