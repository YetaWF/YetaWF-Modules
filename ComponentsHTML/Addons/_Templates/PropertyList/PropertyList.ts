/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface ControlData {
        Id: string; // id of the property list div
        Dependents: Dependent[];
        Controls: string[];
    }
    interface Dependent {
        Prop: string; // Name of property
        ControlProp: string; // name of controlling property (ProcIf)
        Disable: boolean; // defines wheter the control is disabled instead of hidden
        IntValues: number[];
    }

    export class PropertyListComponent {

        private Control: HTMLDivElement;
        private ControlData: ControlData;

        constructor(controlId: string, controlData: ControlData) {
            this.Control = $YetaWF.getElementById(controlId) as HTMLDivElement;
            this.ControlData = controlData;

            // Handle change events
            var controlData = this.ControlData;
            for (let control of controlData.Controls) {
                var elemSel = $YetaWF.getElement1BySelectorCond(`.t_row.t_${control.toLowerCase()} select[name$='${control}']`, [this.Control]) as HTMLSelectElement | null;
                if (elemSel) {
                    $YetaWF.registerEventHandler(elemSel, "change", null, (ev: Event): boolean => {
                        if (!elemSel) return true;
                        this.changeSelect(elemSel);
                        return false;
                    });
                    elemSel.addEventListener("dropdownlist_change", (evt: Event): void => {
                        if (!elemSel) return;
                        this.changeSelect(elemSel);
                    });
                }
                var elemInp = $YetaWF.getElement1BySelectorCond(`.t_row.t_${control.toLowerCase()} input[name$='${control}']`, [this.Control]) as HTMLInputElement | null;
                if (elemInp) {
                    $YetaWF.registerEventHandler(elemInp, "change", null, (ev: Event): boolean => {
                        if (!elemInp) return true;
                        this.changeInput(elemInp);
                        return false;
                    });
                }
            }

            // Initialize initial form
            for (let control of controlData.Controls) {
                var elemSel = $YetaWF.getElement1BySelectorCond(`.t_row.t_${control.toLowerCase()} select[name$='${control}']`, [this.Control]) as HTMLSelectElement | null;
                if (elemSel)
                    this.changeSelect(elemSel);
                var elemInp = $YetaWF.getElement1BySelectorCond(`.t_row.t_${control.toLowerCase()} input[name$='${control}']`, [this.Control]) as HTMLInputElement | null;
                if (elemInp)
                    this.changeInput(elemInp);
            }
        }
        private changeSelect(elem: HTMLSelectElement): void {
            var name = $YetaWF.getAttribute(elem, "name"); // name of controlling item (an enum)
            this.update(name, elem.value);
        }
        private changeInput(elem: HTMLInputElement): void {
            var name = $YetaWF.getAttribute(elem, "name"); // name of controlling item (an enum)
            var value = elem.value;
            if (elem.type.toLowerCase() === "checkbox") {
                value = elem.checked ? "1" : "0";
            }
            this.update(name, value);
        }
        private update(name: string, value: string): void {

            var deps = this.ControlData.Dependents;
            for (let dep of deps) {
                if (name === dep.ControlProp || name.endsWith("." + dep.ControlProp)) { // this entry is for the controlling item?
                    var row = $YetaWF.getElement1BySelector(`.t_row.t_${dep.Prop.toLowerCase()}`, [this.Control]);// the propertylist row affected
                    var intValue = Number(value);
                    var found = false;
                    for (let v of dep.IntValues) {
                        if (v === intValue) {
                            found = true;
                            break;
                        }
                    }
                    if (dep.Disable) {
                        $YetaWF.elementEnableToggle(row, found);
                    } else {
                        if (found)
                            row.style.display = "";
                        else
                            row.style.display = "none";
                        $YetaWF.processActivateDivs([row]);// init any controls that just became visible
                    }
                    var affected = $YetaWF.getElementsBySelector("input,select,textarea", [row]);
                    if (found) {
                        for (let e of affected)
                            $YetaWF.elementRemoveClass(e, YConfigs.Forms.CssFormNoValidate);
                    } else {
                        for (let e of affected)
                            $YetaWF.elementAddClass(e, YConfigs.Forms.CssFormNoValidate);
                    }
                }
            }
        }

        public static tabInitjQuery(tabCtrlId: string, activeTab: number, activeTabId: string):void {
            var tabCtrl = $YetaWF.getElementById(tabCtrlId);
            $YetaWF.elementAddClass(tabCtrl, "t_jquery");
            $(tabCtrl).tabs({
                active: activeTab,
                activate: (ev: Event, ui: JQueryUI.TabsActivationUIParams): void => {
                    if (ui.newPanel !== undefined) {
                        $YetaWF.processActivateDivs([ui.newPanel[0]]);
                        $YetaWF.processPanelSwitched(ui.newPanel[0]);
                        if (activeTabId) {
                            $(`#${activeTabId}`).val( (ui.newTab.length > 0) ?  Number(ui.newTab.attr("data-tab")) : -1);
                        }
                    }
                }
            });
        }
        public static tabInitKendo(tabCtrlId: string, activeTab: number, activeTabId: string):void {
            // mark the active tab with .k-state-active before initializing the tabstrip
            var tabs = $YetaWF.getElementsBySelector(`#${tabCtrlId}>ul>li`);
            for (let tab of tabs) {
                $YetaWF.elementRemoveClass(tab, "k-state-active");
            }
            $YetaWF.elementAddClass(tabs[activeTab], "k-state-active");

            // init tab control
            var tabCtrl = $YetaWF.getElementById(tabCtrlId);
            $YetaWF.elementAddClass(tabCtrl, "t_kendo");
            $(tabCtrl).kendoTabStrip({
                animation: false,
                activate: (ev: kendo.ui.TabStripActivateEvent): void  => {
                    if (ev.contentElement !== undefined) {
                        $YetaWF.processActivateDivs([ev.contentElement as HTMLElement]);
                        $YetaWF.processPanelSwitched(ev.contentElement as HTMLElement);
                        if (activeTabId)
                            $(`#${activeTabId}`).val( $(ev.item as HTMLElement).attr("data-tab") as string );
                    }
                }
            }).data("kendoTabStrip");
        }


    }

    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        var list = $YetaWF.getElementsBySelector(".yt_propertylisttabbed.t_jquery", [tag]);
        for (let el of list) {
            var tabsJq = $(el);
            if (!tabsJq) throw "No jquery ui object found";/*DEBUG*/
            tabsJq.tabs("destroy");
        }
        list = $YetaWF.getElementsBySelector(".yt_propertylisttabbed.t_kendo", [tag]);
        for (let el of list) {
            var tabsKn = $(el).data("kendoTabStrip");
            if (!tabsKn) throw "No kendo object found";/*DEBUG*/
            tabsKn.destroy();
        }
    });

    // The property list needs a bit of special love when it's made visible. Because panels have no width/height
    // while the propertylist is not visible (jquery implementation), when a propertylist is made visible using show(),
    // the default panel is not sized correctly. If you explicitly show() a propertylist that has never been visible,
    // call the following to cause the propertylist to be resized correctly:
    // ComponentsHTML.processPropertyListVisible(div);
    // div is any HTML element - all items (including child items) are checked for propertylists.

    ComponentsHTMLHelper.registerPropertyListVisible((tag: HTMLElement): void => {
        // jquery tabs
        var tabsJq = $YetaWF.getElementsBySelector(".ui-tabs", [tag]);
        for (let tabJq of tabsJq) {
            var id = tabJq.id;
            if (id === undefined) throw "No id on tab control";/*DEBUG*/
            var tabidJq = Number($(tabJq).tabs("option", "active"));
            if (tabidJq >= 0) {
                var panel = $YetaWF.getElement1BySelector(`#${id}_tab${tabidJq}`, [tabJq]);
                $YetaWF.processActivateDivs([panel]);
                $YetaWF.processPanelSwitched(panel);
            }
        }
        // kendo tabs
        var tabsKn = $YetaWF.getElementsBySelector(".k-widget.k-tabstrip", [tag]);
        for (let tabKn of tabsKn) {
            var id = tabKn.id;
            if (id === undefined) throw "No id on tab control";/*DEBUG*/
            var ts = $(tabKn).data("kendoTabStrip");
            var tabidKn = Number(ts.select().attr("data-tab"));
            if (tabidKn >= 0) {
                var panel = $YetaWF.getElement1BySelector(`#${id}-tab${+tabidKn + 1}`, [tabKn]);
                $YetaWF.processActivateDivs([panel]);
                $YetaWF.processPanelSwitched(panel);
            }
        }
    });
}

