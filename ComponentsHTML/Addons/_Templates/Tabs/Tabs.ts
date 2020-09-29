/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
        public static readonly EVENTSWITCHED: string = "tabs_switched";

        private Setup: TabsSetup;
        private ActiveTabHidden: HTMLInputElement;

        constructor(controlId: string, setup: TabsSetup) {
            super(controlId, TabsComponent.TEMPLATE, TabsComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: TabsComponent.EVENTSWITCHED,
                GetValue: (control: TabsComponent): string | null => {
                    return control.activeTab.toString();
                },
                Enable: (control: TabsComponent, enable: boolean, clearOnDisable: boolean): void => { },
            });
            this.Setup = setup;

            this.ActiveTabHidden = $YetaWF.getElementById(this.Setup.ActiveTabHiddenId) as HTMLInputElement;

            $YetaWF.registerEventHandler(this.Control, "click", "ul.t_tabstrip li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem;
                let index = Number($YetaWF.getAttribute(li, "data-tab"));
                this.activatePane(index);
                return false;
            });
            $YetaWF.registerEventHandler(this.Control, "keydown", "ul.t_tabstrip li", (ev: KeyboardEvent): boolean => {
                let index = this.activeTab;
                let key = ev.key;
                if (key === "ArrowDown" || key === "Down" || key === "ArrowRight" || key === "Right") {
                    ++index;
                } else if (key === "ArrowUp" || key === "Up" || key === "ArrowLeft" || key === "Left") {
                    --index;
                } else if (key === "Home") {
                    index = 0;
                } else if (key === "End") {
                    index = this.tabCount-1;
                } else
                    return true;
                if (index >= 0 && index < this.tabCount)
                    this.activatePane(index);
                return false;
            });
            $YetaWF.registerEventHandler(this.Control, "focusin", null, (ev: FocusEvent): boolean => {
                let curentTab = this.currentTab;
                if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery) {
                    for (let tab of this.tabs)
                        $YetaWF.elementRemoveClass(tab, "ui-state-focus");
                    $YetaWF.elementAddClass(curentTab, "ui-state-focus");
                } else {
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Control, "focusout", null, (ev: FocusEvent): boolean => {
                if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery) {
                    for (let tab of this.tabs)
                        $YetaWF.elementRemoveClass(tab, "ui-state-focus");
                } else {
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Control, "mousemove", "ul.t_tabstrip li", (ev: MouseEvent): boolean => {
                let curentTab = ev.__YetaWFElem as HTMLLIElement;
                if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery) {
                    for (let tab of this.tabs)
                        $YetaWF.elementRemoveClass(tab, "ui-state-hover");
                    $YetaWF.elementAddClass(curentTab, "ui-state-hover");
                } else {
                    for (let tab of this.tabs)
                        $YetaWF.elementRemoveClass(tab, "k-state-hover");
                    $YetaWF.elementAddClass(curentTab, "k-state-hover");
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Control, "mouseout", "ul.t_tabstrip li", (ev: MouseEvent): boolean => {
                if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery) {
                    for (let tab of this.tabs)
                        $YetaWF.elementRemoveClass(tab, "ui-state-hover");
                } else {
                    for (let tab of this.tabs)
                        $YetaWF.elementRemoveClass(tab, "k-state-hover");
                }
                return true;
            });
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
            let tabCount = this.tabCount;
            if (tabCount === 0)
                throw "No panes found";
            if (index < 0 || index >= tabCount)
                throw `tab pane ${index} requested - ${tabCount} tabs present`;
            let tabs = this.tabs;
            let panels = $YetaWF.getElementsBySelector(`#${this.ControlId} > .t_tabpanel`) as HTMLLIElement[];
            if (panels.length !== tabCount)
                throw `Mismatched number of tabs (${tabCount}) and panels (${panels.length}) `;

            let activeTab = tabs[index];
            let activePanel = panels[index];
            if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.JQuery) {

                for (let tab of tabs) {
                    $YetaWF.elementRemoveClasses(tab, ["ui-tabs-active", "ui-state-active"]);
                    $YetaWF.setAttribute(tab, "tabindex", "-1");
                    $YetaWF.setAttribute(tab, "aria-selected", "false");
                    $YetaWF.setAttribute(tab, "aria-expanded", "false");
                }
                $YetaWF.elementAddClasses(activeTab, ["ui-tabs-active", "ui-state-active"]);
                $YetaWF.setAttribute(activeTab, "tabindex", "0");
                $YetaWF.setAttribute(activeTab, "aria-selected", "true");
                $YetaWF.setAttribute(activeTab, "aria-expanded", "true");

                for (let panel of panels) {
                    $YetaWF.setAttribute(panel, "aria-hidden", "true");
                    panel.style.display = "none";
                }
                $YetaWF.setAttribute(activePanel, "aria-hidden", "false");
                activePanel.style.display = "";

            } else if (YVolatile.Forms.TabStyle === YetaWF.TabStyleEnum.Kendo) {

                for (let tab of tabs) {
                    $YetaWF.elementRemoveClasses(tab, ["k-state-active", "k-tab-on-top"]);
                    $YetaWF.setAttribute(tab, "aria-selected", "false");
                    tab.removeAttribute("id");
                }
                $YetaWF.elementAddClasses(activeTab, ["k-state-active", "k-tab-on-top"]);
                $YetaWF.setAttribute(activeTab, "aria-selected", "true");

                let ariaId = this.Control.getAttribute("aria-activedescendant");
                activeTab.id = ariaId as string;

                for (let panel of panels) {
                    $YetaWF.elementRemoveClasses(panel, ["k-state-active"]);
                    $YetaWF.setAttribute(panel, "aria-hidden", "true");
                    $YetaWF.setAttribute(panel, "aria-expanded", "false");
                    panel.style.display = "none";
                }
                $YetaWF.elementAddClass(activePanel, "k-state-active");
                $YetaWF.setAttribute(activePanel, "aria-hidden", "false");
                $YetaWF.setAttribute(activePanel, "aria-expanded", "true");
                activePanel.style.display = "block";

            } else
                throw `Unknown tab style ${YVolatile.Forms.TabStyle}`;/*DEBUG*/

            this.ActiveTabHidden.value = index.toString();

            $YetaWF.processActivateDivs([activePanel]);
            $YetaWF.processPanelSwitched(activePanel);

            $YetaWF.sendCustomEvent(this.Control, TabsComponent.EVENTSWITCHED);
        }

        get activeTab(): number {
            return Number(this.ActiveTabHidden.value);
        }
        get tabCount(): number {
            return this.tabs.length;
        }
        get tabs(): HTMLLIElement[] {
            let tabs = $YetaWF.getElementsBySelector("ul.t_tabstrip > li", [this.Control]) as HTMLLIElement[];
            return tabs;
        }
        get currentTab(): HTMLLIElement {
            let index = this.activeTab;
            let tabs = this.tabs;
            if (index < 0 || index >= tabs.length)
                throw `tab index ${index} invalid`;
            return tabs[index];
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
            let index = tab.activeTab;
            if (index >= 0) {
                let panel = $YetaWF.getElement1BySelector(`#${tab.ControlId}_tab${index}`, [tab.Control]);
                $YetaWF.processActivateDivs([panel]);
                $YetaWF.processPanelSwitched(panel);
            }
        }
    });
}
