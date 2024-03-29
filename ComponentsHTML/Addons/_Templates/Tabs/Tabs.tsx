/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export interface IPackageLocs {
        Loading: string;
    }

    interface TabsSetup {
        ActiveTabIndex: number;
        ActiveTabHiddenId: string;
        ContextMenu: boolean;
    }

    export class TabsComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_tabs";
        public static readonly SELECTOR: string = ".yt_tabs";
        public static readonly EVENTSWITCHED: string = "tabs_switched";
        public static readonly EVENTCONTEXTMENU: string = "tabs_contextmenu";

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

            $YetaWF.registerEventHandler(this.Control, "click", `#${this.ControlId} > ul.t_tabstrip > li.t_tab`, (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem;
                let index = Number($YetaWF.getAttribute(li, "data-tab"));
                this.activatePane(index);
                return false;
            });
            $YetaWF.registerEventHandler(this.Control, "keydown", `#${this.ControlId} > ul.t_tabstrip > li`, (ev: KeyboardEvent): boolean => {
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
                if (index >= 0 && index < this.tabCount) {
                    this.activatePane(index);
                    return false;
                }
                return true;
            });
            if (this.Setup.ContextMenu) {
                $YetaWF.registerEventHandler(this.Control, "contextmenu", `#${this.ControlId} > ul.t_tabstrip > li`, (ev: MouseEvent): boolean => {
                    let li = ev.__YetaWFElem;
                    let index = Number($YetaWF.getAttribute(li, "data-tab"));
                    this.activatePane(index);
                    ev.preventDefault();
                    $YetaWF.sendCustomEvent(this.Control, TabsComponent.EVENTCONTEXTMENU);
                    return false;
                });
            }
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

            for (let tab of tabs) {
                $YetaWF.elementRemoveClass(tab, "t_tabactive");
                $YetaWF.setAttribute(tab, "tabindex", "-1");
                $YetaWF.setAttribute(tab, "aria-selected", "false");
                $YetaWF.setAttribute(tab, "aria-expanded", "false");
            }
            $YetaWF.elementAddClass(activeTab, "t_tabactive");
            $YetaWF.setAttribute(activeTab, "tabindex", "0");
            $YetaWF.setAttribute(activeTab, "aria-selected", "true");
            $YetaWF.setAttribute(activeTab, "aria-expanded", "true");

            for (let panel of panels) {
                $YetaWF.setAttribute(panel, "aria-hidden", "true");
                panel.style.display = "none";
            }
            $YetaWF.setAttribute(activePanel, "aria-hidden", "false");
            activePanel.style.display = "";

            this.ActiveTabHidden.value = index.toString();

            activeTab.focus();

            $YetaWF.sendActivateDivEvent([activePanel]);
            $YetaWF.sendPanelSwitchedEvent(activePanel);

            $YetaWF.sendCustomEvent(this.Control, TabsComponent.EVENTSWITCHED);
        }

        get activeTab(): number {
            return Number(this.ActiveTabHidden.value);
        }
        get tabCount(): number {
            return this.tabs.length;
        }
        get tabs(): HTMLLIElement[] {
            let tabs = $YetaWF.getElementsBySelector(`#${this.ControlId} > ul.t_tabstrip > li`) as HTMLLIElement[];
            return tabs;
        }
        get currentTab(): HTMLLIElement {
            let index = this.activeTab;
            let tabs = this.tabs;
            if (index < 0 || index >= tabs.length)
                throw `tab index ${index} invalid`;
            return tabs[index];
        }

        /**
         * Adds a tab/pane and returns the DIV that can be used to add contents.
         */
        public add(caption: string, tooltip?: string): HTMLDivElement {
            let tabstrip = $YetaWF.getElement1BySelector(`#${this.ControlId} > ul.t_tabstrip`) as HTMLUListElement;
            let total = this.tabs.length;
            let tab =
                <li role="tab" tabindex="-1" class="t_tab" aria-selected="false" aria-expanded="false">
                    <a data-tooltip={tooltip} role="presentation" tabindex="-1" class="t_tabanchor">
                        {caption}
                    </a>
                </li> as HTMLLIElement;
            tabstrip.appendChild(tab);

            let pane =
                 <div class="t_proptable t_cat t_tabpanel" role="tabpanel" style="display:none" aria-hidden="true">
                     <div class="t_contents t_loading">{YLocs.YetaWF_ComponentsHTML.Loading}</div>
                 </div> as HTMLDivElement;
            this.Control.appendChild(pane);

            this.resequenceTabs();

            this.activatePane(total);

            return $YetaWF.getElement1BySelector(".t_contents", [pane]) as HTMLDivElement;
        }

        public remove(index: number): void {
            let tabs = this.tabs;
            if (index < 0 || index >= tabs.length)
                throw `tab index ${index} invalid`;

            $YetaWF.processClearDiv(tabs[index]);
            tabs[index].remove();

            let panes = $YetaWF.getElementsBySelector(`#${this.ControlId} > div.t_tabpanel`) as HTMLDivElement[];
            $YetaWF.processClearDiv(panes[index]);
            panes[index].remove();

            this.resequenceTabs();

            // find a tab to activate
            tabs = this.tabs;
            if (index >= tabs.length)
                --index;
            if (index >= 0 && index < tabs.length)
                this.activatePane(index);
        }

        private resequenceTabs(): void {
            let count = 0;
            for (let tab of this.tabs) {
                let tabId = `${this.ControlId}_tab${count}`;
                let tabIdLb = `${tabId}_lb`;
                $YetaWF.setAttribute(tab, "data-tab", count.toString());
                $YetaWF.setAttribute(tab, "aria-controls", tabId);
                $YetaWF.setAttribute(tab, "aria-labelledby", tabIdLb);
                let anchor = $YetaWF.getElement1BySelector("a", [tab]) as HTMLAnchorElement;
                anchor.href = `#${tabId}`;
                anchor.id = `#${tabIdLb}`;
                ++count;
            }
            count = 0;
            let panes = $YetaWF.getElementsBySelector(`#${this.ControlId} > div.t_tabpanel`) as HTMLDivElement[];
            for (let pane of panes) {
                let tabId = `${this.ControlId}_tab${count}`;
                let tabIdLb = `${tabId}_lb`;
                $YetaWF.setAttribute(pane, "data-tab", count.toString());
                pane.id = tabId;
                $YetaWF.setAttribute(pane, "aria-labelledby", tabIdLb);

                let loadingDiv = $YetaWF.getElement1BySelectorCond(".t_contents.t_loading", [pane]);
                if (loadingDiv)
                    loadingDiv.id = `${tabId}_content`;

                ++count;
            }
        }
    }

    // The property list needs a bit of special love when it's made visible. Because panels have no width/height
    // while the propertylist is not visible, when a propertylist is made visible using show(),
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
                $YetaWF.sendActivateDivEvent([panel]);
                $YetaWF.sendPanelSwitchedEvent(panel);
            }
        }
    });
}
