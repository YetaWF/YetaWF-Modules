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
    var BooleanTextEditComponent = /** @class */ (function (_super) {
        __extends(BooleanTextEditComponent, _super);
        function BooleanTextEditComponent() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        BooleanTextEditComponent.TEMPLATE = "yt_booleantext";
        BooleanTextEditComponent.SELECTOR = ".yt_booleantext.t_edit";
        return BooleanTextEditComponent;
    }(YetaWF.ComponentBase));
    YetaWF_ComponentsHTML.BooleanTextEditComponent = BooleanTextEditComponent;
    BooleanTextEditComponent.register(BooleanTextEditComponent.TEMPLATE, BooleanTextEditComponent.SELECTOR, false, {
        ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Input,
        ChangeEvent: null,
        GetValue: function (control) {
            return control.checked ? "true" : "false";
        },
        Enable: function (control, enable, clearOnDisable) {
            YetaWF_BasicsImpl.elementEnableToggle(control, enable);
            if (clearOnDisable)
                control.checked = false;
        },
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
