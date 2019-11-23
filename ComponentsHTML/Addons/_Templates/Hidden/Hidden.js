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
    var HiddenComponent = /** @class */ (function (_super) {
        __extends(HiddenComponent, _super);
        function HiddenComponent() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        HiddenComponent.TEMPLATE = "yt_hidden";
        HiddenComponent.SELECTOR = ".yt_hidden";
        return HiddenComponent;
    }(YetaWF.ComponentBase));
    YetaWF_ComponentsHTML.HiddenComponent = HiddenComponent;
    HiddenComponent.register(HiddenComponent.TEMPLATE, HiddenComponent.SELECTOR, false, {
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
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Hidden.js.map
