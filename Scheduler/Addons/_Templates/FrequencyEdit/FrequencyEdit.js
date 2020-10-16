"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */
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
var YetaWF_Scheduler;
(function (YetaWF_Scheduler) {
    var FrequencyEdit = /** @class */ (function (_super) {
        __extends(FrequencyEdit, _super);
        function FrequencyEdit(controlId /*, setup: FrequencyEditSetup*/) {
            var _this = _super.call(this, controlId, FrequencyEdit.TEMPLATE, FrequencyEdit.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: function (control) {
                    var _a;
                    return (_a = control.Value.value) === null || _a === void 0 ? void 0 : _a.toString(); // not quite, doesn't include units
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                    if (clearOnDisable)
                        control.clear();
                },
            }) || this;
            _this.Value = YetaWF.ComponentBaseDataImpl.getControlFromSelector("[name$='.Value']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [_this.Control]);
            _this.Units = YetaWF.ComponentBaseDataImpl.getControlFromSelector("[name$='.TimeUnits']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [_this.Control]);
            return _this;
        }
        FrequencyEdit.prototype.clear = function () {
            this.Value.clear();
            this.Units.clear();
        };
        FrequencyEdit.prototype.enable = function (enabled) {
            this.Value.enable(enabled);
            this.Units.enable(enabled);
        };
        FrequencyEdit.TEMPLATE = "yt_yetawf_scheduler_frequency";
        FrequencyEdit.SELECTOR = ".yt_yetawf_scheduler_frequency.t_edit";
        return FrequencyEdit;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_Scheduler.FrequencyEdit = FrequencyEdit;
})(YetaWF_Scheduler || (YetaWF_Scheduler = {}));

//# sourceMappingURL=FrequencyEdit.js.map
