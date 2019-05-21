"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
        function TextEditComponent(controlId /*, setup: TextEditSetup*/) {
            var _this = _super.call(this, controlId, TextEditComponent.TEMPLATE, TextEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Input,
                ChangeEvent: null,
                GetValue: function (control) {
                    return control.value;
                },
                Enable: function (control, enable) {
                    control.removeAttribute("disabled");
                    $YetaWF.elementRemoveClass(control, "k-state-disabled");
                    if (!enable) {
                        control.setAttribute("disabled", "disabled");
                        $YetaWF.elementAddClass(control, "k-state-disabled");
                    }
                },
            }) || this;
            // synonyms
            var t = YetaWF.ComponentBaseDataImpl.getTemplateDefinition(TextEditComponent.TEMPLATE);
            _this.registerTemplate("yt_text", t.Selector, t.UserData, t.Display, t.DestroyControl);
            _this.registerTemplate("yt_text10", t.Selector, t.UserData, t.Display, t.DestroyControl);
            _this.registerTemplate("yt_text20", t.Selector, t.UserData, t.Display, t.DestroyControl);
            _this.registerTemplate("yt_text40", t.Selector, t.UserData, t.Display, t.DestroyControl);
            _this.registerTemplate("yt_text80", t.Selector, t.UserData, t.Display, t.DestroyControl);
            return _this;
        }
        TextEditComponent.TEMPLATE = "yt_text_base";
        TextEditComponent.SELECTOR = ".yt_text_base.t_edit";
        return TextEditComponent;
    }(YetaWF.ComponentBaseNoDataImpl));
    YetaWF_ComponentsHTML.TextEditComponent = TextEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=TextEdit.js.map
