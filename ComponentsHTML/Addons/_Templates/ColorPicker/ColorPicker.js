"use strict";
/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var ColorPickerComponent = /** @class */ (function (_super) {
        __extends(ColorPickerComponent, _super);
        function ColorPickerComponent(controlId, setup) {
            var _this = _super.call(this, controlId, ColorPickerComponent.TEMPLATE, ColorPickerComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: "colorpicker_change",
                GetValue: function (control) {
                    var colorPicker = $(control.Control).data("kendoColorPicker");
                    return colorPicker.value();
                },
                Enable: function (control, enable) {
                    var colorPicker = $(control.Control).data("kendoColorPicker");
                    colorPicker.enable(enable);
                },
            }, false, function (tag, control) {
                var colorPicker = $(control.Control).data("kendoColorPicker");
                colorPicker.destroy();
            }) || this;
            $("#" + controlId).kendoColorPicker(setup);
            return _this;
        }
        ColorPickerComponent.TEMPLATE = "yt_colorpicker";
        ColorPickerComponent.SELECTOR = ".yt_colorpicker.t_edit";
        return ColorPickerComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.ColorPickerComponent = ColorPickerComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=ColorPicker.js.map
