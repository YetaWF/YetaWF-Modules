"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */
var YetaWF_DevTests;
(function (YetaWF_DevTests) {
    var TestStepsModule = /** @class */ (function () {
        function TestStepsModule(id) {
            this.Module = $YetaWF.getElementById(id);
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='Clear']", function (ev) {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.clearAll(); // deactivate all steps
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='ActivateAll']", function (ev) {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setAllActive(["YetaWF_DevTests_Step"]); // activates all named steps that start with the specified name(s)
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='ActivateLast']", function (ev) {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setLastActive(["YetaWF_DevTests_Step"]); // activates the last named step that start with the specified name(s)
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='Step1']", function (ev) {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setActive({ Name: "YetaWF_DevTests_Step1" });
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='Step2']", function (ev) {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setActive({ Name: "YetaWF_DevTests_Step2" });
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='Step3']", function (ev) {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setActive({ Name: "YetaWF_DevTests_Step3" });
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='Step4']", function (ev) {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setActive({ Name: "YetaWF_DevTests_Step4" });
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='Step12']", function (ev) {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setActive({ Name: "YetaWF_DevTests_Step2" });
                    YetaWF_Panels.StepInfoComponent.setEnabled([{ Name: "YetaWF_DevTests_Step1", Url: "/" }]);
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='Step13']", function (ev) {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setActive({ Name: "YetaWF_DevTests_Step3" });
                    YetaWF_Panels.StepInfoComponent.setEnabled([{ Name: "YetaWF_DevTests_Step1", Url: "/" }, { Name: "YetaWF_DevTests_Step2", Url: "/" }]);
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='Step14']", function (ev) {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setActive({ Name: "YetaWF_DevTests_Step4" });
                    YetaWF_Panels.StepInfoComponent.setEnabled([{ Name: "YetaWF_DevTests_Step1", Url: "/" }, { Name: "YetaWF_DevTests_Step2", Url: "/" }, { Name: "YetaWF_DevTests_Step3", Url: "/" }]);
                }
                return true;
            });
        }
        return TestStepsModule;
    }());
    YetaWF_DevTests.TestStepsModule = TestStepsModule;
})(YetaWF_DevTests || (YetaWF_DevTests = {}));
