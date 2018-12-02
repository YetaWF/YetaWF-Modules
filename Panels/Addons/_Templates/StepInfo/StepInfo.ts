/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

namespace YetaWF_Panels {

    export interface StepOptions {
        Name: string;
        Url: string | null; // only used for enable calls to set Url to link to
    }

    export class StepInfoComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly SELECTOR: string = ".yt_panels_stepinfo.t_display";

        constructor(controlId: string) {
            super(controlId);
        }

        public static setActive(option: StepOptions) {
            var stepCtrls = $YetaWF.getElementsBySelector(StepInfoComponent.SELECTOR);
            for (let stepCtrl of stepCtrls) {
                var control = YetaWF.ComponentBaseDataImpl.getControlFromTag<StepInfoComponent>(stepCtrl, StepInfoComponent.SELECTOR);
                control.clear();
                control.activate(option);
            }
        }
        public static setAllActive(names: string[]) {
            var stepCtrls = $YetaWF.getElementsBySelector(StepInfoComponent.SELECTOR);
            for (let stepCtrl of stepCtrls) {
                var control = YetaWF.ComponentBaseDataImpl.getControlFromTag<StepInfoComponent>(stepCtrl, StepInfoComponent.SELECTOR);
                control.clear();
                control.activateAll(names);
            }
        }
        public static setLastActive(names: string[]) {
            var stepCtrls = $YetaWF.getElementsBySelector(StepInfoComponent.SELECTOR);
            for (let stepCtrl of stepCtrls) {
                var control = YetaWF.ComponentBaseDataImpl.getControlFromTag<StepInfoComponent>(stepCtrl, StepInfoComponent.SELECTOR);
                control.clear();
                control.activateLast(names);
            }
        }
        public static setEnabled(options: StepOptions[]) {
            var stepCtrls = $YetaWF.getElementsBySelector(StepInfoComponent.SELECTOR);
            for (let stepCtrl of stepCtrls) {
                var control = YetaWF.ComponentBaseDataImpl.getControlFromTag<StepInfoComponent>(stepCtrl, StepInfoComponent.SELECTOR);
                control.enable(options);
            }
        }

        // API

        public clear(): void {
            var steps = $YetaWF.getElementsBySelector(`a.t_step.t_active`) as HTMLAnchorElement[];
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
                for (let step of steps) {
                    $YetaWF.elementAddClass(step, "t_active");
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

    // A <div> is being emptied. Destroy all steps the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        StepInfoComponent.clearDiv(tag, StepInfoComponent.SELECTOR);
    });
}