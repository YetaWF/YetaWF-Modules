"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */
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
var YetaWF_Panels;
(function (YetaWF_Panels) {
    var StepInfoComponent = /** @class */ (function (_super) {
        __extends(StepInfoComponent, _super);
        function StepInfoComponent(controlId) {
            return _super.call(this, controlId, StepInfoComponent.TEMPLATE, StepInfoComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }, true) || this;
        }
        StepInfoComponent.setActive = function (option) {
            var stepCtrls = $YetaWF.getElementsBySelector(StepInfoComponent.SELECTOR);
            for (var _i = 0, stepCtrls_1 = stepCtrls; _i < stepCtrls_1.length; _i++) {
                var stepCtrl = stepCtrls_1[_i];
                var control = YetaWF.ComponentBaseDataImpl.getControlFromTag(stepCtrl, StepInfoComponent.SELECTOR);
                control.clear();
                control.activate(option);
            }
        };
        StepInfoComponent.setAllActive = function (names) {
            var stepCtrls = $YetaWF.getElementsBySelector(StepInfoComponent.SELECTOR);
            for (var _i = 0, stepCtrls_2 = stepCtrls; _i < stepCtrls_2.length; _i++) {
                var stepCtrl = stepCtrls_2[_i];
                var control = YetaWF.ComponentBaseDataImpl.getControlFromTag(stepCtrl, StepInfoComponent.SELECTOR);
                control.clear();
                control.activateAll(names);
            }
        };
        StepInfoComponent.setLastActive = function (names) {
            var stepCtrls = $YetaWF.getElementsBySelector(StepInfoComponent.SELECTOR);
            for (var _i = 0, stepCtrls_3 = stepCtrls; _i < stepCtrls_3.length; _i++) {
                var stepCtrl = stepCtrls_3[_i];
                var control = YetaWF.ComponentBaseDataImpl.getControlFromTag(stepCtrl, StepInfoComponent.SELECTOR);
                control.clear();
                control.activateLast(names);
            }
        };
        StepInfoComponent.setEnabled = function (options) {
            var stepCtrls = $YetaWF.getElementsBySelector(StepInfoComponent.SELECTOR);
            for (var _i = 0, stepCtrls_4 = stepCtrls; _i < stepCtrls_4.length; _i++) {
                var stepCtrl = stepCtrls_4[_i];
                var control = YetaWF.ComponentBaseDataImpl.getControlFromTag(stepCtrl, StepInfoComponent.SELECTOR);
                control.enable(options);
            }
        };
        StepInfoComponent.clearAll = function () {
            var stepCtrls = $YetaWF.getElementsBySelector(StepInfoComponent.SELECTOR);
            for (var _i = 0, stepCtrls_5 = stepCtrls; _i < stepCtrls_5.length; _i++) {
                var stepCtrl = stepCtrls_5[_i];
                var control = YetaWF.ComponentBaseDataImpl.getControlFromTag(stepCtrl, StepInfoComponent.SELECTOR);
                control.clear();
            }
        };
        // API
        StepInfoComponent.prototype.clear = function () {
            var steps = $YetaWF.getElementsBySelector("a.t_step.t_active,a.t_step.t_enabled");
            for (var _i = 0, steps_1 = steps; _i < steps_1.length; _i++) {
                var step = steps_1[_i];
                $YetaWF.elementRemoveClass(step, "t_active");
                $YetaWF.elementRemoveClass(step, "t_enabled");
                step.removeAttribute("href");
            }
        };
        StepInfoComponent.prototype.activate = function (option) {
            var steps = $YetaWF.getElementsBySelector("a[data-name=" + option.Name + "].t_step", [this.Control]);
            for (var _i = 0, steps_2 = steps; _i < steps_2.length; _i++) {
                var step = steps_2[_i];
                $YetaWF.elementAddClass(step, "t_active");
            }
        };
        StepInfoComponent.prototype.activateAll = function (names) {
            for (var _i = 0, names_1 = names; _i < names_1.length; _i++) {
                var name_1 = names_1[_i];
                var steps = $YetaWF.getElementsBySelector("a[data-name^=" + name_1 + "].t_step", [this.Control]);
                var count = 0;
                for (var _a = 0, steps_3 = steps; _a < steps_3.length; _a++) {
                    var step = steps_3[_a];
                    if (count >= steps.length - 1)
                        $YetaWF.elementAddClass(step, "t_active");
                    else
                        $YetaWF.elementAddClass(step, "t_enabled");
                    ++count;
                }
            }
        };
        StepInfoComponent.prototype.activateLast = function (names) {
            for (var _i = 0, names_2 = names; _i < names_2.length; _i++) {
                var name_2 = names_2[_i];
                var steps = $YetaWF.getElementsBySelector("a[data-name^=" + name_2 + "].t_step", [this.Control]);
                if (steps.length > 0) {
                    $YetaWF.elementAddClass(steps[steps.length - 1], "t_active");
                }
            }
        };
        StepInfoComponent.prototype.enable = function (options) {
            for (var _i = 0, options_1 = options; _i < options_1.length; _i++) {
                var option = options_1[_i];
                var steps = $YetaWF.getElementsBySelector("a[data-name=" + option.Name + "].t_step", [this.Control]);
                for (var _a = 0, steps_4 = steps; _a < steps_4.length; _a++) {
                    var step = steps_4[_a];
                    $YetaWF.elementAddClass(step, "t_enabled");
                    if (option.Url)
                        step.href = option.Url;
                }
            }
        };
        StepInfoComponent.TEMPLATE = "yt_panels_stepinfo";
        StepInfoComponent.SELECTOR = ".yt_panels_stepinfo.t_display";
        return StepInfoComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_Panels.StepInfoComponent = StepInfoComponent;
})(YetaWF_Panels || (YetaWF_Panels = {}));

//# sourceMappingURL=StepInfo.js.map
