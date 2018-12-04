/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface ControlData {
        Id: string; // id of the property list div
        Controls: string[];
        Dependents: Dependent[];
    }
    interface Dependent {
        Prop: string; // Name of property
        Disable: boolean; // defines whether the control is disabled instead of hidden

        Values: ValueEntry[];
    }
    interface ValueEntry {
        ControlProp: string; // name of controlling property (ProcIf)
        ValueType: ValueTypeEnum;
        ValueObject: any;
    }
    enum ValueTypeEnum {
        EqualIntValue = 0,
        EqualNull = 100,
        EqualNonNull = 101,
    }

    interface ControlItem {
        Name: string;
        ControlType: ControlTypeEnum;
        Object: DropDownListEditComponent | HTMLInputElement | HTMLSelectElement;
    }
    enum ControlTypeEnum {
        Input = 0,
        Select = 1,
        KendoSelect = 2,
    }

    export class PropertyListComponent {

        private Control: HTMLDivElement;
        private ControlData: ControlData;
        private ControllingControls: ControlItem[] = [];

        constructor(controlId: string, controlData: ControlData) {

            this.Control = $YetaWF.getElementById(controlId) as HTMLDivElement;
            this.ControlData = controlData;

            // Handle change events
            var controlData = this.ControlData;
            for (let control of controlData.Controls) {
                var controlItem = this.getControlItem(control);
                this.ControllingControls.push(controlItem);
                switch (controlItem.ControlType) {
                    case ControlTypeEnum.Input:
                        $YetaWF.registerMultipleEventHandlers((controlItem.Object as HTMLInputElement), ["change", "input"], null, (ev: Event): boolean => {
                            this.update();
                            return false;
                        });
                        break;
                    case ControlTypeEnum.Select:
                        $YetaWF.registerEventHandler((controlItem.Object as HTMLSelectElement), "change", null, (ev: Event): boolean => {
                            this.update();
                            return false;
                        });
                        break;
                    case ControlTypeEnum.KendoSelect:
                        (controlItem.Object as DropDownListEditComponent).Control.addEventListener("dropdownlist_change", (evt: Event): void => {
                            this.update();
                        });
                        break;
                }
            }

            // Initialize initial form
            this.update();
        }

        private getControlItem(control: string): ControlItem {
            var elemSel = $YetaWF.getElement1BySelectorCond(`.t_row.t_${control.toLowerCase()} select[name$='${control}']`, [this.Control]) as HTMLSelectElement | null;
            if (elemSel) {
                var kendoSelect = YetaWF.ComponentBaseDataImpl.getControlFromTagCond<DropDownListEditComponent>(elemSel, DropDownListEditComponent.SELECTOR);
                if (kendoSelect) {
                    // Kendo
                    return { Name: control, ControlType: ControlTypeEnum.KendoSelect, Object: kendoSelect };
                } else {
                    // Native
                    return { Name: control, ControlType: ControlTypeEnum.Select, Object: elemSel };
                }
            } else {
                var elemInp = $YetaWF.getElement1BySelectorCond(`.t_row.t_${control.toLowerCase()} input[name$='${control}']`, [this.Control]) as HTMLInputElement | null;
                if (elemInp) {
                    return { Name: control, ControlType: ControlTypeEnum.Input, Object: elemInp };
                }
            }
            throw `No control found for ${control}`;
        }

        /**
         * Update all dependent fields.
         */
        private update(): void {

            var found = false;

            // for each dependent, verify that all it's conditions are true
            var deps = this.ControlData.Dependents;
            for (let dep of deps) {

                var depRow = $YetaWF.getElement1BySelector(`.t_row.t_${dep.Prop.toLowerCase()}`, [this.Control]);// the propertylist row affected

                var valid = true; // we assume valid unless we find a non-matching entry

                for (let value of dep.Values) {

                    // get the controlling control's value
                    var ctrlIndex = this.ControlData.Controls.indexOf(value.ControlProp);
                    if (ctrlIndex < 0)
                        throw `Dependent ${dep.Prop} references controlling control ${value.ControlProp} which doesn't exist`;
                    var controlItem = this.ControllingControls[ctrlIndex];

                    var controlValue;
                    switch (controlItem.ControlType) {
                        case ControlTypeEnum.Input:
                            var inputElem = controlItem.Object as HTMLInputElement;
                            if (inputElem.type.toLowerCase() === "checkbox") {
                                controlValue = inputElem.checked ? "1" : "0";
                            } else {
                                controlValue = inputElem.value;
                            }
                            break;
                        case ControlTypeEnum.Select:
                            controlValue = (controlItem.Object as HTMLSelectElement).value;
                            break;
                        case ControlTypeEnum.KendoSelect:
                            controlValue = (controlItem.Object as DropDownListEditComponent).value;
                            break;
                    }

                    // test condition
                    switch (value.ValueType) {
                        case ValueTypeEnum.EqualIntValue:
                            // need one matching value
                            var intValues = value.ValueObject as number[];
                            var found = false;
                            for (let intValue of intValues) {
                                if (intValue === Number(controlValue)) {
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                                valid = false;
                            break;
                        case ValueTypeEnum.EqualNonNull:
                            if (!controlValue || controlValue.length === 0)
                                valid = false;
                            break;
                        case ValueTypeEnum.EqualNull:
                            if (controlValue)
                                valid = false;
                            break;
                    }
                    if (!valid)
                        break;
                }

                if (dep.Disable) {
                    $YetaWF.elementAndChildrenEnableToggle(depRow, valid);
                } else {
                    if (valid) {
                        depRow.style.display = "";
                        $YetaWF.processActivateDivs([depRow]);// init any controls that just became visible
                    } else
                        depRow.style.display = "none";
                }
                var affected = $YetaWF.getElementsBySelector("input,select,textarea", [depRow]);
                if (valid) {
                    for (let e of affected)
                        $YetaWF.elementRemoveClass(e, YConfigs.Forms.CssFormNoValidate);
                } else {
                    for (let e of affected)
                        $YetaWF.elementAddClass(e, YConfigs.Forms.CssFormNoValidate);
                }
            }
        }

        public static tabInitjQuery(tabCtrlId: string, activeTab: number, activeTabId: string):void {
            var tabCtrl = $YetaWF.getElementById(tabCtrlId);
            $YetaWF.elementAddClass(tabCtrl, "t_jquery");
            $(tabCtrl).tabs({ //jQuery-ui use
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

