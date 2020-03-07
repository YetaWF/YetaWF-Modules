/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

// Kendo UI menu use

namespace YetaWF_ComponentsHTML {

    export enum TabStyleEnum {
        JQuery = 0,
        Kendo = 1,
    }

    interface TabsSetup {
        TabStyle: TabStyleEnum;
        ActiveTabIndex: number;
        ActiveTabHiddenId: string;
    }
    export class TabsComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_tabs";
        public static readonly SELECTOR: string = ".yt_tabs";

        private Setup: TabsSetup;
        private ActiveTabHidden: HTMLInputElement | null = null;

        constructor(controlId: string, setup: TabsSetup) {
            super(controlId, TabsComponent.TEMPLATE, TabsComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });
            this.Setup = setup;

            if (this.Setup.ActiveTabHiddenId)
                this.ActiveTabHidden = $YetaWF.getElementById(this.Setup.ActiveTabHiddenId) as HTMLInputElement;

            if (this.Setup.TabStyle === TabStyleEnum.JQuery) {
                ComponentsHTMLHelper.MUSTHAVE_JQUERYUI();

                $(this.Control).tabs({ //jQuery-ui use
                    active: this.Setup.ActiveTabIndex,
                    activate: (ev: Event, ui: JQueryUI.TabsActivationUIParams): void => {
                        if (ui.newPanel !== undefined) {
                            $YetaWF.processActivateDivs([ui.newPanel[0]]);
                            $YetaWF.processPanelSwitched(ui.newPanel[0]);
                            if (this.ActiveTabHidden)
                                this.ActiveTabHidden.value = (ui.newTab.length > 0) ? (ui.newTab.attr("data-tab")||"-1") : "-1";
                        }
                    }
                });

            } else if (this.Setup.TabStyle === TabStyleEnum.Kendo) {

                ComponentsHTMLHelper.MUSTHAVE_KENDOUI();

                // mark the active tab with .k-state-active before initializing the tabstrip
                let tabs = $YetaWF.getElementsBySelector(`#${this.ControlId}>ul>li`);
                for (let tab of tabs) {
                    $YetaWF.elementRemoveClass(tab, "k-state-active");
                }
                $YetaWF.elementAddClass(tabs[this.Setup.ActiveTabIndex], "k-state-active");

                // init tab control
                $(this.Control).kendoTabStrip({
                    animation: false,
                    activate: (ev: kendo.ui.TabStripActivateEvent): void => {
                        if (ev.contentElement !== undefined) {
                            $YetaWF.processActivateDivs([ev.contentElement as HTMLElement]);
                            $YetaWF.processPanelSwitched(ev.contentElement as HTMLElement);
                            if (this.ActiveTabHidden)
                                this.ActiveTabHidden.value = $(ev.item as HTMLElement).attr("data-tab") as string;
                        }
                    }
                }).data("kendoTabStrip");

            } else
                throw `Invalid tab style ${this.Setup.TabStyle}`;

        }
    }
}
