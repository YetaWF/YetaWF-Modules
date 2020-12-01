"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
/// <reference types="kendo-ui" />
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var ColorPickerEditComponent = /** @class */ (function (_super) {
        __extends(ColorPickerEditComponent, _super);
        function ColorPickerEditComponent(controlId, setup) {
            var _this = _super.call(this, controlId, ColorPickerEditComponent.TEMPLATE, ColorPickerEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: "colorpicker_change",
                GetValue: function (control) {
                    var colorPicker = $(control.Control).data("kendoColorPicker");
                    return colorPicker.value();
                },
                Enable: function (control, enable, clearOnDisable) {
                    var colorPicker = $(control.Control).data("kendoColorPicker");
                    colorPicker.enable(enable);
                    //if (clearOnDisable) { // resetting color doesn't work (8/7/2019 with 2019.2.619)
                    //    colorPicker.value(null as any); // "as any" because ts.d defines as string|undefined, only null resets color
                    //    colorPicker.color(null as any); // "as any" because ts.d defines as string|undefined, only null resets color
                    //}
                },
            }, false, function (tag, control) {
                var colorPicker = $(control.Control).data("kendoColorPicker");
                colorPicker.destroy();
            }) || this;
            setup.change = function (ev) {
                FormsSupport.validateElement(_this.Control);
                var event = document.createEvent("Event");
                event.initEvent("colorpicker_change", true, true);
                _this.Control.dispatchEvent(event);
            };
            $("#" + controlId).kendoColorPicker(setup);
            return _this;
        }
        ColorPickerEditComponent.TEMPLATE = "yt_colorpicker";
        ColorPickerEditComponent.SELECTOR = ".yt_colorpicker.t_edit";
        return ColorPickerEditComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.ColorPickerEditComponent = ColorPickerEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=ColorPickerEdit.js.map
