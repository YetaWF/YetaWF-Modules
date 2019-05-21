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
    var BooleanTextEditComponent = /** @class */ (function (_super) {
        __extends(BooleanTextEditComponent, _super);
        function BooleanTextEditComponent(controlId /*, setup: BooleanTextEditSetup*/) {
            return _super.call(this, controlId, YetaWF_ComponentsHTML.BooleanEditComponent.TEMPLATE, YetaWF_ComponentsHTML.BooleanEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Input,
                ChangeEvent: null,
                GetValue: function (control) {
                    return control.checked ? "true" : "false";
                },
                Enable: function (control, enable) {
                    if (enable) {
                        control.setAttribute("disabled", "disabled");
                        $YetaWF.elementRemoveClass(control, "k-state-disabled");
                    }
                    else {
                        control.removeAttribute("disabled");
                        $YetaWF.elementAddClass(control, "k-state-disabled");
                    }
                },
            }) || this;
        }
        BooleanTextEditComponent.TEMPLATE = "yt_booleantext";
        BooleanTextEditComponent.SELECTOR = ".yt_booleantext.t_edit";
        return BooleanTextEditComponent;
    }(YetaWF.ComponentBaseNoDataImpl));
    YetaWF_ComponentsHTML.BooleanTextEditComponent = BooleanTextEditComponent;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=BooleanTextEdit.js.map
