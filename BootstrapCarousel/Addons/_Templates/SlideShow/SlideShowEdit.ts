/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

namespace YetaWF_BootstrapCarousel {

    export interface IPackageConfigs {
        Action_Apply: string;
        Action_MoveLeft: string;
        Action_MoveRight: string;
        Action_Add: string;
        Action_Insert: string;
        Action_Remove: string;
    }
    export interface IPackageLocs {
        RemoveConfirm: string;
        RemoveTitle: string;
    }

    export class SlideShowEdit extends YetaWF.ComponentBase<HTMLElement> {

        public static readonly SELECTOR: string = ".yt_bootstrapcarousel_slideshow";
        private static readonly TEMPLATENAME: string = "YetaWF_BootstrapCarousel_SlideShow";

        private buttonUp: HTMLElement;
        private buttonDown: HTMLElement;
        private buttonDelete: HTMLElement;

        constructor(controlId:string) {
            super(controlId);
            $YetaWF.addObjectDataById(controlId, this);

            this.buttonUp = $YetaWF.getElement1BySelector("input.t_up", [this.Control]);
            this.buttonDown = $YetaWF.getElement1BySelector("input.t_down", [this.Control]);
            this.buttonDelete = $YetaWF.getElement1BySelector("input.t_delete", [this.Control]);

            // Apply button click
            $YetaWF.registerEventHandler(this.Control, "click", "input.t_apply", (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, SlideShowEdit.TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_Apply, this.getPanelIndex().toString());
                return false;
            });
            // << button click
            $YetaWF.registerEventHandler(this.buttonUp, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, SlideShowEdit.TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_MoveLeft, this.getPanelIndex().toString());
                return false;
            });
            // >> button click
            $YetaWF.registerEventHandler(this.buttonDown, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, SlideShowEdit.TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_MoveRight, this.getPanelIndex().toString());
                return false;
            });
            // delete button click
            $YetaWF.registerEventHandler(this.buttonDelete, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.alertYesNo(YLocs.YetaWF_BootstrapCarousel.RemoveConfirm, YLocs.YetaWF_BootstrapCarousel.RemoveTitle, (): void => {
                    $YetaWF.Forms.submitTemplate(this.Control, false, SlideShowEdit.TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_Remove, this.getPanelIndex().toString());
                });
                return false;
            });
            // Insert button click
            $YetaWF.registerEventHandler(this.Control, "click", "input.t_ins", (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, SlideShowEdit.TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_Insert, this.getPanelIndex().toString());
                return false;
            });
            // Add button click
            $YetaWF.registerEventHandler(this.Control, "click", "input.t_add", (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, SlideShowEdit.TEMPLATENAME, YConfigs.YetaWF_BootstrapCarousel.Action_Add, this.getPanelIndex().toString());
                return false;
            });

            this.updateButtons();
        }

        private getActiveTab(): HTMLInputElement {
            return $YetaWF.getElement1BySelector("input[name$='_ActiveTab']", [this.Control]) as HTMLInputElement;
        }
        private getPanelIndex(): number {
            var activeTab = this.getActiveTab();
            return Number(activeTab.value);
        }
        private getPanelCount(): number {
            var lis = $YetaWF.getElementsBySelector(".t_tabstrip li", [this.Control]);
            return lis.length;
        }

        private updateButtons(): void {

            var panelIndex = this.getPanelIndex();
            var panelCount = this.getPanelCount();

            // disable the << button if the active tab is the first one
            $YetaWF.elementEnableToggle(this.buttonUp, panelIndex !== 0);
            // disable the >> button if the last panel is active
            $YetaWF.elementEnableToggle(this.buttonDown, panelIndex < panelCount - 1);
            // disable if there is only one panel
            $YetaWF.elementEnableToggle(this.buttonDelete, panelCount > 1);
        }

        public updateActiveTab(panel: HTMLElement): void {

            // TODO: This needs to be moved into the tab control
            var activeTab = $YetaWF.getElement1BySelector("input[name$='_ActiveTab']", [this.Control]) as HTMLInputElement;
            activeTab.value = $YetaWF.getAttribute(panel, "data-tab");

            this.updateButtons();
        }

        public destroy(): void { $YetaWF.removeObjectDataById(this.Control.id); }
        public static getControlFromTagCond(elem: HTMLElement): SlideShowEdit | null { return super.getControlBaseFromTag<SlideShowEdit>(elem, SlideShowEdit.SELECTOR); }
    }

    $YetaWF.registerPanelSwitched((panel: HTMLElement): void => {
        var ctrl = SlideShowEdit.getControlFromTagCond(panel);
        if (ctrl != null)
            ctrl.updateActiveTab(panel);
    });

    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        YetaWF.ComponentBase.clearDiv<SlideShowEdit>(tag, SlideShowEdit.SELECTOR, (control: SlideShowEdit): void => {
            control.destroy();
        });
    });
}