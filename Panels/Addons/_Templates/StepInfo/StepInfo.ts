/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

namespace YetaWF_Panels {

    export interface StepOptions {
        Name: string;
        Url?: string; // only used for enable calls to set Url to link to
    }

    export class StepInfoComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_panels_stepinfo";
        public static readonly SELECTOR: string = ".yt_panels_stepinfo.t_display";

        constructor(controlId: string) {
            super(controlId, StepInfoComponent.TEMPLATE, StepInfoComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: "",//$$$$
                GetValue: (control: StepInfoComponent): string | null => { return null; },
                Enable: (control: StepInfoComponent, enable: boolean): void => { }
            }, true);
        }

        public static setActive(option: StepOptions): void {
            var stepCtrls = $YetaWF.getElementsBySelector(StepInfoComponent.SELECTOR);
            for (let stepCtrl of stepCtrls) {
                var control = YetaWF.ComponentBaseDataImpl.getControlFromTag<StepInfoComponent>(stepCtrl, StepInfoComponent.SELECTOR);
                control.clear();
                control.activate(option);
            }
        }
        public static setAllActive(names: string[]): void {
            var stepCtrls = $YetaWF.getElementsBySelector(StepInfoComponent.SELECTOR);
            for (let stepCtrl of stepCtrls) {
                var control = YetaWF.ComponentBaseDataImpl.getControlFromTag<StepInfoComponent>(stepCtrl, StepInfoComponent.SELECTOR);
                control.clear();
                control.activateAll(names);
            }
        }
        public static setLastActive(names: string[]): void {
            var stepCtrls = $YetaWF.getElementsBySelector(StepInfoComponent.SELECTOR);
            for (let stepCtrl of stepCtrls) {
                var control = YetaWF.ComponentBaseDataImpl.getControlFromTag<StepInfoComponent>(stepCtrl, StepInfoComponent.SELECTOR);
                control.clear();
                control.activateLast(names);
            }
        }
        public static setEnabled(options: StepOptions[]): void {
            var stepCtrls = $YetaWF.getElementsBySelector(StepInfoComponent.SELECTOR);
            for (let stepCtrl of stepCtrls) {
                var control = YetaWF.ComponentBaseDataImpl.getControlFromTag<StepInfoComponent>(stepCtrl, StepInfoComponent.SELECTOR);
                control.enable(options);
            }
        }
        public static clearAll(): void {
            var stepCtrls = $YetaWF.getElementsBySelector(StepInfoComponent.SELECTOR);
            for (let stepCtrl of stepCtrls) {
                var control = YetaWF.ComponentBaseDataImpl.getControlFromTag<StepInfoComponent>(stepCtrl, StepInfoComponent.SELECTOR);
                control.clear();
            }
        }

        // API

        public clear(): void {
            var steps = $YetaWF.getElementsBySelector(`a.t_step.t_active,a.t_step.t_enabled`) as HTMLAnchorElement[];
            for (let step of steps) {
                $YetaWF.elementRemoveClass(step, "t_active");
                $YetaWF.elementRemoveClass(step, "t_enabled");
                step.removeAttribute("href");
            }
        }
        public activate(option: StepOptions): void {
            var steps = $YetaWF.getElementsBySelector(`a[data-name=${option.Name}].t_step`, [this.Control]) as HTMLAnchorElement[];
            for (let step of steps) {
                $YetaWF.elementAddClass(step, "t_active");
            }
        }
        public activateAll(names: string[]): void {
            for (let name of names) {
                var steps = $YetaWF.getElementsBySelector(`a[data-name^=${name}].t_step`, [this.Control]) as HTMLAnchorElement[];
                var count = 0;
                for (let step of steps) {
                    if (count >= steps.length - 1)
                        $YetaWF.elementAddClass(step, "t_active");
                    else
                        $YetaWF.elementAddClass(step, "t_enabled");
                    ++count;
                }
            }
        }
        public activateLast(names: string[]): void {
            for (let name of names) {
                var steps = $YetaWF.getElementsBySelector(`a[data-name^=${name}].t_step`, [this.Control]) as HTMLAnchorElement[];
                if (steps.length > 0) {
                    $YetaWF.elementAddClass(steps[steps.length-1], "t_active");
                }
            }
        }
        public enable(options: StepOptions[]): void {
            for (let option of options) {
                var steps = $YetaWF.getElementsBySelector(`a[data-name=${option.Name}].t_step`, [this.Control]) as HTMLAnchorElement[];
                for (let step of steps) {
                    $YetaWF.elementAddClass(step, "t_enabled");
                    if (option.Url)
                        step.href = option.Url;
                }
            }
        }
    }
}