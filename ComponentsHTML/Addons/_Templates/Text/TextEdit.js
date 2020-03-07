"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
    var TextEditComponent = /** @class */ (function (_super) {
        __extends(TextEditComponent, _super);
        function TextEditComponent() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        TextEditComponent.TEMPLATE = "yt_text_base";
        TextEditComponent.SELECTOR = ".yt_text_base.t_edit";
        return TextEditComponent;
    }(YetaWF.ComponentBase));
    YetaWF_ComponentsHTML.TextEditComponent = TextEditComponent;
    TextEditComponent.register(TextEditComponent.TEMPLATE, TextEditComponent.SELECTOR, false, {
        ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Input,
        ChangeEvent: null,
        GetValue: function (control) {
            return control.value;
        },
        Enable: function (control, enable, clearOnDisable) {
            YetaWF_BasicsImpl.elementEnableToggle(control, enable);
            if (clearOnDisable)
                control.value = "";
        },
    });
    var t = TextEditComponent.getTemplateDefinition(TextEditComponent.TEMPLATE);
    TextEditComponent.register("yt_text", ".yt_text.t_edit", t.HasData, t.UserData, t.Display, t.DestroyControl);
    TextEditComponent.register("yt_text10", ".yt_text10.t_edit", t.HasData, t.UserData, t.Display, t.DestroyControl);
    TextEditComponent.register("yt_text20", ".yt_text20.t_edit", t.HasData, t.UserData, t.Display, t.DestroyControl);
    TextEditComponent.register("yt_text40", ".yt_text40.t_edit", t.HasData, t.UserData, t.Display, t.DestroyControl);
    TextEditComponent.register("yt_text80", ".yt_text80.t_edit", t.HasData, t.UserData, t.Display, t.DestroyControl);
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
