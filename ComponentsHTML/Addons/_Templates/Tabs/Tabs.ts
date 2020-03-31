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
        public static readonly EVENT: string = "tabs_change";

        private Setup: TabsSetup;
        private ActiveTabHidden: HTMLInputElement | null = null;

        constructor(controlId: string, setup: TabsSetup) {
            super(controlId, TabsComponent.TEMPLATE, TabsComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: TabsComponent.EVENT,
                GetValue: (control: TabsComponent): string | null => {
                    return control.activePane.toString();
                },
                Enable: (control: TabsComponent, enable: boolean, clearOnDisable: boolean): void => { },
            }, false, (tag: HTMLElement, control: TabsComponent): void => {
                control.internalDestroy();
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
                            let index = Number((ui.newTab.length > 0) ? (ui.newTab.attr("data-tab") || "-1") : "-1");
                            if (this.ActiveTabHidden)
                                this.ActiveTabHidden.value = index.toString();
                            this.sendEvent();
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
                            this.sendEvent();
                        }
                    }
                }).data("kendoTabStrip");

            } else
                throw `Invalid tab style ${this.Setup.TabStyle}`;
        }
        private sendEvent(): void {
            let event = document.createEvent("Event");
            event.initEvent(TabsComponent.EVENT, true, true);
            this.Control.dispatchEvent(event);
        }

        public internalDestroy(): void {
            if (this.Setup.TabStyle === TabStyleEnum.JQuery) {
                $(this.Control).tabs("destroy");
            } else if (this.Setup.TabStyle === TabStyleEnum.Kendo) {
                let tab = $(this.Control).data("kendoTabStrip");
                if (!tab) throw "No kendo object found";
                tab.destroy();
            } else
                throw `Invalid tab style ${this.Setup.TabStyle}`;
        }

        // API

        /* Activate the pane that contains the specified element. The element does not need to be present. */
        public activatePaneByTag(tag: HTMLElement): void {
            if (!$YetaWF.elementHas(this.Control, tag))
                return;
            let ttabpanel = $YetaWF.elementClosestCond(tag, "div.t_tabpanel");
            if (!ttabpanel)
                return;
            let index = ttabpanel.getAttribute("data-tab") as number | null;
            if (index == null)
                throw "We found a panel in a tab control without panel number (data-tab attribute).";
            this.activatePane(index);
        }
        /* Activate the pane by 0-based index. */
        public activatePane(index: number): void {
            let panels = $YetaWF.getElementsBySelector("ul.t_tabstrip > li", [this.Control]);
            if (panels.length === 0)
                throw "No panes found";
            if (index < 0 || index >= panels.length)
                throw `tab pane ${index} requested - ${panels.length} tabs present`;
            if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery)
                $(this.Control).tabs("option", "active", index);
            else if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.Kendo) {
                $(this.Control).data("kendoTabStrip").activateTab($(panels[index]));
            } else
                throw `Unknown tab style ${YVolatile.Forms.TabStyle}`;/*DEBUG*/
        }
        get activePane(): number {
            if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery)
                return Number($(this.Control).tabs("option", "active"));
            else if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.Kendo) {
                var ts = $(this.Control).data("kendoTabStrip");
                return Number(ts.select().attr("data-tab"));
            } else
                throw `Unknown tab style ${YVolatile.Forms.TabStyle}`;/*DEBUG*/
        }
    }

    // The property list needs a bit of special love when it's made visible. Because panels have no width/height
    // while the propertylist is not visible (jquery implementation), when a propertylist is made visible using show(),
    // the default panel is not sized correctly. If you explicitly show() a propertylist that has never been visible,
    // call the following to cause the propertylist to be resized correctly:
    // ComponentsHTML.processPropertyListVisible(div);
    // div is any HTML element - all items (including child items) are checked for propertylists.

    ComponentsHTMLHelper.registerPropertyListVisible((tag: HTMLElement): void => {
        var tabsTags = $YetaWF.getElementsBySelector(TabsComponent.SELECTOR, [tag]);
        for (let tabTag of tabsTags) {
            let tab = YetaWF.ComponentBaseDataImpl.getControlFromTag<TabsComponent>(tabTag, TabsComponent.SELECTOR);
            let index = tab.activePane;
            if (index >= 0) {
                let panel = $YetaWF.getElement1BySelector(`#${tab.ControlId}_tab${index}`, [tab.Control]);
                $YetaWF.processActivateDivs([panel]);
                $YetaWF.processPanelSwitched(panel);
            }
        }
    });
}
