/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

namespace YetaWF_Panels {

    export interface IPackageLocs {
        RemoveStepConfirm: string;
        RemoveStepTitle: string;
    }

    enum StepAction {
        Apply = 0,
        MoveLeft = 1,
        MoveRight = 2,
        Add = 3,
        Insert = 4,
        Remove = 5,
    }

    export class StepInfoEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_panels_stepinfo";
        public static readonly SELECTOR: string = ".yt_panels_stepinfo.t_edit";
        public static TEMPLATENAME: string = "YetaWF_Panels_StepInfo";

        private Up: HTMLInputElement;
        private Down: HTMLInputElement;
        private Delete: HTMLInputElement;
        private Apply: HTMLInputElement;
        private Insert: HTMLInputElement;
        private Add: HTMLInputElement;

        constructor(controlId: string) {
            super(controlId, StepInfoEditComponent.TEMPLATE, StepInfoEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });

            this.Up = $YetaWF.getElement1BySelector("input.t_up", [this.Control]) as HTMLInputElement;
            this.Down = $YetaWF.getElement1BySelector("input.t_down", [this.Control]) as HTMLInputElement;
            this.Delete = $YetaWF.getElement1BySelector("input.t_delete", [this.Control]) as HTMLInputElement;
            this.Apply = $YetaWF.getElement1BySelector("input.t_apply", [this.Control]) as HTMLInputElement;
            this.Insert = $YetaWF.getElement1BySelector("input.t_ins", [this.Control]) as HTMLInputElement;
            this.Add = $YetaWF.getElement1BySelector("input.t_add", [this.Control]) as HTMLInputElement;

            this.updateButtons();

            // Apply button click
            $YetaWF.registerEventHandler(this.Apply, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, StepInfoEditComponent.TEMPLATENAME, StepAction.Apply, this.getPanelIndex().toString());
                return false;
            });
            // << button click
            $YetaWF.registerEventHandler(this.Up, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, StepInfoEditComponent.TEMPLATENAME, StepAction.MoveLeft, this.getPanelIndex().toString());
                return false;
            });
            // >> button click
            $YetaWF.registerEventHandler(this.Down, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, StepInfoEditComponent.TEMPLATENAME, StepAction.MoveRight, this.getPanelIndex().toString());
                return false;
            });
            // delete button click
            $YetaWF.registerEventHandler(this.Delete, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.alertYesNo(YLocs.YetaWF_Panels.RemoveStepConfirm, YLocs.YetaWF_Panels.RemoveStepTitle, (): void => {
                    $YetaWF.Forms.submitTemplate(this.Control, false, StepInfoEditComponent.TEMPLATENAME, StepAction.Remove, this.getPanelIndex().toString());
                });
                return false;
            });
            // Insert button click
            $YetaWF.registerEventHandler(this.Insert, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, StepInfoEditComponent.TEMPLATENAME, StepAction.Insert, this.getPanelIndex().toString());
                return false;
            });
            // Add button click
            $YetaWF.registerEventHandler(this.Add, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, StepInfoEditComponent.TEMPLATENAME, StepAction.Add, this.getPanelIndex().toString());
                return false;
            });
        }
        private getPanelIndex(): number {
            var tabActive = $YetaWF.getElement1BySelector("input[name$='_ActiveTab']", [this.Control]) as HTMLInputElement;
            return Number(tabActive.value);
        }
        private getPanelCount(): number {
            var tabs = $YetaWF.getElementsBySelector(".t_tabstrip li", [this.Control]);
            return tabs.length;
        }
        public updateButtons(): void {
            var panelIndex = this.getPanelIndex();
            $YetaWF.elementEnableToggle(this.Up, panelIndex !== 0);
            $YetaWF.elementEnableToggle(this.Down, panelIndex < this.getPanelCount() - 1);
            $YetaWF.elementEnableToggle(this.Delete, this.getPanelCount() > 1);
        }
    }

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTPANELSWITCHED, null, (ev: CustomEvent<YetaWF.DetailsPanelSwitched>): boolean => {
        let panel = ev.detail.panel;
        var panelInfo = YetaWF.ComponentBaseDataImpl.getControlFromTagCond<StepInfoEditComponent>(panel, StepInfoEditComponent.SELECTOR);
        if (panelInfo) {
            var tabActive = $YetaWF.getElement1BySelector("input[name$='_ActiveTab']", [panelInfo.Control]) as HTMLInputElement;
            tabActive.value = $YetaWF.getAttribute(panel, "data-tab");
            panelInfo.updateButtons();
        }
        return true;
    });
}
