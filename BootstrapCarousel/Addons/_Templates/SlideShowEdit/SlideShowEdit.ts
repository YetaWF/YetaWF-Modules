/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

namespace YetaWF {
    export interface IConfigs {
        YetaWF_BootstrapCarousel: YetaWF_BootstrapCarousel.IPackageConfigs;
    }
    export interface ILocs {
        YetaWF_BootstrapCarousel: YetaWF_BootstrapCarousel.IPackageLocs;
    }
}

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

    export class SlideShowEdit extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_bootstrapcarousel_slideshow";
        public static readonly SELECTOR: string = ".yt_bootstrapcarousel_slideshow.t_edit";
        private static readonly TEMPLATENAME: string = "YetaWF_BootstrapCarousel_SlideShow";

        private buttonUp: HTMLElement;
        private buttonDown: HTMLElement;
        private buttonDelete: HTMLElement;

        constructor(controlId: string) {
            super(controlId, SlideShowEdit.TEMPLATE, SlideShowEdit.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });

            this.buttonUp = $YetaWF.getElement1BySelector("input.t_up", [this.Control]);
            this.buttonDown = $YetaWF.getElement1BySelector("input.t_down", [this.Control]);
            this.buttonDelete = $YetaWF.getElement1BySelector("input.t_delete", [this.Control]);

            // Apply button click
            $YetaWF.registerEventHandler(this.Control, "click", "input.t_apply", (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, SlideShowEdit.TEMPLATENAME, YetaWF.PanelAction.Apply, this.getPanelIndex().toString());
                return false;
            });
            // << button click
            $YetaWF.registerEventHandler(this.buttonUp, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, SlideShowEdit.TEMPLATENAME, YetaWF.PanelAction.MoveLeft, this.getPanelIndex().toString());
                return false;
            });
            // >> button click
            $YetaWF.registerEventHandler(this.buttonDown, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, SlideShowEdit.TEMPLATENAME, YetaWF.PanelAction.MoveRight, this.getPanelIndex().toString());
                return false;
            });
            // delete button click
            $YetaWF.registerEventHandler(this.buttonDelete, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.alertYesNo(YLocs.YetaWF_BootstrapCarousel.RemoveConfirm, YLocs.YetaWF_BootstrapCarousel.RemoveTitle, (): void => {
                    $YetaWF.Forms.submitTemplate(this.Control, false, SlideShowEdit.TEMPLATENAME, YetaWF.PanelAction.Remove, this.getPanelIndex().toString());
                });
                return false;
            });
            // Insert button click
            $YetaWF.registerEventHandler(this.Control, "click", "input.t_ins", (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, SlideShowEdit.TEMPLATENAME, YetaWF.PanelAction.Insert, this.getPanelIndex().toString());
                return false;
            });
            // Add button click
            $YetaWF.registerEventHandler(this.Control, "click", "input.t_add", (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, SlideShowEdit.TEMPLATENAME, YetaWF.PanelAction.Add, this.getPanelIndex().toString());
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
    }
}