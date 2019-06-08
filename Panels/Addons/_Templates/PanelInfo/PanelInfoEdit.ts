/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

namespace YetaWF_Panels {

    export interface IPackageLocs {
        RemovePanelConfirm: string;
        RemovePanelTitle: string;
    }

    export class PanelInfoEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_panels_panelinfo";
        public static readonly SELECTOR: string = ".yt_panels_panelinfo.t_edit";
        public static TEMPLATENAME: string = "YetaWF_Panels_PanelInfo";

        private Up: HTMLInputElement;
        private Down: HTMLInputElement;
        private Delete: HTMLInputElement;
        private Apply: HTMLInputElement;
        private Insert: HTMLInputElement;
        private Add: HTMLInputElement;

        constructor(controlId: string) {
            super(controlId, PanelInfoEditComponent.TEMPLATE, PanelInfoEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }, false, (tag: HTMLElement, control: PanelInfoEditComponent): void => {
                // tabs
                var list = $YetaWF.getElementsBySelector(".yt_panels_panelinfo .t_panels.t_acctabs", [control.Control]);
                for (let el of list) {
                    var tabs = $(el);
                    if (tabs) tabs.tabs("destroy"); //jQuery-ui use
                }
                // jquery ui accordion
                var list = $YetaWF.getElementsBySelector(".yt_panels_panelinfo .t_panels.t_accjquery", [control.Control]);
                for (let el of list) {
                    var accordion = $(el);
                    if (accordion) accordion.accordion("destroy"); //jQuery-ui use
                }
                // kendo accordion
                var list = $YetaWF.getElementsBySelector(".yt_panels_panelinfo .t_panels.t_acckendo", [control.Control]);
                for (let el of list) {
                    var panelBar = $(el).data("kendoPanelBar");
                    if (panelBar) panelBar.destroy();
                }
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
                $YetaWF.Forms.submitTemplate(this.Control, true, PanelInfoEditComponent.TEMPLATENAME, YConfigs.YetaWF_Panels.Action_Apply, this.getPanelIndex().toString());
                return false;
            });
            // << button click
            $YetaWF.registerEventHandler(this.Up, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, PanelInfoEditComponent.TEMPLATENAME, YConfigs.YetaWF_Panels.Action_MoveLeft, this.getPanelIndex().toString());
                return false;
            });
            // >> button click
            $YetaWF.registerEventHandler(this.Down, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, PanelInfoEditComponent.TEMPLATENAME, YConfigs.YetaWF_Panels.Action_MoveRight, this.getPanelIndex().toString());
                return false;
            });
            // delete button click
            $YetaWF.registerEventHandler(this.Delete, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.alertYesNo(YLocs.YetaWF_Panels.RemovePanelConfirm, YLocs.YetaWF_Panels.RemovePanelTitle, (): void => {
                    $YetaWF.Forms.submitTemplate(this.Control, false, PanelInfoEditComponent.TEMPLATENAME, YConfigs.YetaWF_Panels.Action_Remove, this.getPanelIndex().toString());
                });
                return false;
            });
            // Insert button click
            $YetaWF.registerEventHandler(this.Insert, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, PanelInfoEditComponent.TEMPLATENAME, YConfigs.YetaWF_Panels.Action_Insert, this.getPanelIndex().toString());
                return false;
            });
            // Add button click
            $YetaWF.registerEventHandler(this.Add, "click", null, (ev: MouseEvent): boolean => {
                $YetaWF.Forms.submitTemplate(this.Control, true, PanelInfoEditComponent.TEMPLATENAME, YConfigs.YetaWF_Panels.Action_Add, this.getPanelIndex().toString());
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

    $YetaWF.registerPanelSwitched((panel: HTMLElement): void => {
        var panelInfo = YetaWF.ComponentBaseDataImpl.getControlFromTagCond<PanelInfoEditComponent>(panel, PanelInfoEditComponent.SELECTOR);
        if (!panelInfo) return;
        var tabActive = $YetaWF.getElement1BySelector("input[name$='_ActiveTab']", [panelInfo.Control]) as HTMLInputElement;
        tabActive.value = $YetaWF.getAttribute(panel, "data-tab");
        panelInfo.updateButtons();
    });
}