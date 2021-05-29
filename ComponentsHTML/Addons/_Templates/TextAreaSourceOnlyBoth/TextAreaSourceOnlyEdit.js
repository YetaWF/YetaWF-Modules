"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var TextAreaSourceOnlyEditComponent = /** @class */ (function (_super) {
        __extends(TextAreaSourceOnlyEditComponent, _super);
        function TextAreaSourceOnlyEditComponent(controlId /*, setup: TextAreaSourceOnlyEditSetup*/) {
            return _super.call(this, controlId, TextAreaSourceOnlyEditComponent.TEMPLATE, TextAreaSourceOnlyEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.TextArea,
                ChangeEvent: null,
                GetValue: function (control) {
                    return control.value;
                },
                Enable: function (control, enable, clearOnDisable) {
                    $YetaWF.elementEnableToggle(control, enable);
                    if (clearOnDisable)
                        control.value = "";
                },
            }) || this;
            //this.Setup = setup;
        }
        TextAreaSourceOnlyEditComponent.TEMPLATE = "yt_textareasourceonly";
        TextAreaSourceOnlyEditComponent.SELECTOR = ".yt_textareasourceonly.t_edit";
        TextAreaSourceOnlyEditComponent.clip = null;
        return TextAreaSourceOnlyEditComponent;
    }(YetaWF.ComponentBaseNoDataImpl));
    YetaWF_ComponentsHTML.TextAreaSourceOnlyEditComponent = TextAreaSourceOnlyEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=TextAreaSourceOnlyEdit.js.map
