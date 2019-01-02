/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

namespace YetaWF_DevTests {

    declare var YetaWF_Panels: any; // don't want to reference package for this

    export class TestStepsModule {

        private Module: HTMLDivElement;

        constructor(id: string) {
            this.Module = $YetaWF.getElementById(id) as HTMLDivElement;

            $YetaWF.registerEventHandler(this.Module, "click", "input[name='Clear']", (ev: MouseEvent): boolean => {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.clearAll();// deactivate all steps
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='ActivateAll']", (ev: MouseEvent): boolean => {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setAllActive(["YetaWF_DevTests_Step"]);// activates all named steps that start with the specified name(s)
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='ActivateLast']", (ev: MouseEvent): boolean => {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setLastActive(["YetaWF_DevTests_Step"]);// activates the last named step that start with the specified name(s)
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='Step1']", (ev: MouseEvent): boolean => {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setActive({ Name: "YetaWF_DevTests_Step1" });
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='Step2']", (ev: MouseEvent): boolean => {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setActive({ Name: "YetaWF_DevTests_Step2" });
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='Step3']", (ev: MouseEvent): boolean => {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setActive({ Name: "YetaWF_DevTests_Step3" });
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='Step4']", (ev: MouseEvent): boolean => {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setActive({ Name: "YetaWF_DevTests_Step4" });
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='Step12']", (ev: MouseEvent): boolean => {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setActive({ Name: "YetaWF_DevTests_Step2" });
                    YetaWF_Panels.StepInfoComponent.setEnabled([{ Name: "YetaWF_DevTests_Step1", Url: "/" }]);
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='Step13']", (ev: MouseEvent): boolean => {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setActive({ Name: "YetaWF_DevTests_Step3" });
                    YetaWF_Panels.StepInfoComponent.setEnabled([{ Name: "YetaWF_DevTests_Step1", Url: "/" }, { Name: "YetaWF_DevTests_Step2", Url: "/" }]);
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Module, "click", "input[name='Step14']", (ev: MouseEvent): boolean => {
                if (typeof YetaWF_Panels !== "undefined" && YetaWF_Panels.StepInfoComponent) {
                    YetaWF_Panels.StepInfoComponent.setActive({ Name: "YetaWF_DevTests_Step4" });
                    YetaWF_Panels.StepInfoComponent.setEnabled([{ Name: "YetaWF_DevTests_Step1", Url: "/" }, { Name: "YetaWF_DevTests_Step2", Url: "/" }, { Name: "YetaWF_DevTests_Step3", Url: "/" }]);
                }
                return true;
            });
        }
    }
}

