"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */
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
var YetaWF_DevTests;
(function (YetaWF_DevTests) {
    var TemplateProgressBarModule = /** @class */ (function (_super) {
        __extends(TemplateProgressBarModule, _super);
        function TemplateProgressBarModule(id) {
            var _this = _super.call(this, id, TemplateProgressBarModule.SELECTOR, null) || this;
            _this.ProgressBar = YetaWF.ComponentBaseDataImpl.getControlFromSelector(".t_row.t_bar " + YetaWF_ComponentsHTML.ProgressBarComponent.SELECTOR, YetaWF_ComponentsHTML.ProgressBarComponent.SELECTOR, [_this.Module]);
            _this.Value = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='Value']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [_this.Module]);
            $YetaWF.registerCustomEventHandler(_this.Value.Control, YetaWF_ComponentsHTML.IntValueEditComponent.EVENT, null, function (ev) {
                _this.ProgressBar.value = _this.Value.value;
                return true;
            });
            return _this;
        }
        TemplateProgressBarModule.SELECTOR = ".YetaWF_DevTests_TemplateProgressBar";
        return TemplateProgressBarModule;
    }(YetaWF.ModuleBaseNoDataImpl));
    YetaWF_DevTests.TemplateProgressBarModule = TemplateProgressBarModule;
})(YetaWF_DevTests || (YetaWF_DevTests = {}));

//# sourceMappingURL=TemplateProgressBar.js.map
