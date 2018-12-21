"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var ValueTypeEnum;
    (function (ValueTypeEnum) {
        ValueTypeEnum[ValueTypeEnum["EqualIntValue"] = 0] = "EqualIntValue";
        ValueTypeEnum[ValueTypeEnum["EqualStringValue"] = 1] = "EqualStringValue";
        ValueTypeEnum[ValueTypeEnum["EqualNull"] = 100] = "EqualNull";
        ValueTypeEnum[ValueTypeEnum["EqualNonNull"] = 101] = "EqualNonNull";
    })(ValueTypeEnum || (ValueTypeEnum = {}));
    var ControlTypeEnum;
    (function (ControlTypeEnum) {
        ControlTypeEnum[ControlTypeEnum["Input"] = 0] = "Input";
        ControlTypeEnum[ControlTypeEnum["Select"] = 1] = "Select";
        ControlTypeEnum[ControlTypeEnum["KendoSelect"] = 2] = "KendoSelect";
        ControlTypeEnum[ControlTypeEnum["Hidden"] = 3] = "Hidden";
    })(ControlTypeEnum || (ControlTypeEnum = {}));
    var PropertyListComponent = /** @class */ (function () {
        function PropertyListComponent(controlId, controlData) {
            var _this = this;
            this.ControllingControls = [];
            this.Control = $YetaWF.getElementById(controlId);
            this.ControlData = controlData;
            // Handle change events
            var controlData = this.ControlData;
            for (var _i = 0, _a = controlData.Controls; _i < _a.length; _i++) {
                var control = _a[_i];
                var controlItem = this.getControlItem(control);
                this.ControllingControls.push(controlItem);
                switch (controlItem.ControlType) {
                    case ControlTypeEnum.Input:
                        $YetaWF.registerMultipleEventHandlers([controlItem.Object], ["change", "input"], null, function (ev) {
                            _this.update();
                            return false;
                        });
                        break;
                    case ControlTypeEnum.Select:
                        $YetaWF.registerEventHandler(controlItem.Object, "change", null, function (ev) {
                            _this.update();
                            return false;
                        });
                        break;
                    case ControlTypeEnum.KendoSelect:
                        $YetaWF.registerCustomEventHandler(controlItem.Object, "dropdownlist_change", function (evt) {
                            _this.update();
                        });
                        break;
                    case ControlTypeEnum.Hidden:
                        break;
                }
            }
            // Initialize initial form
            this.update();
        }
        PropertyListComponent.prototype.getControlItem = function (control) {
            var elemSel = $YetaWF.getElement1BySelectorCond(".t_row.t_" + control.toLowerCase() + " select[name$='" + control + "']", [this.Control]);
            if (elemSel) {
                var kendoSelect = YetaWF.ComponentBaseDataImpl.getControlFromTagCond(elemSel, YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR);
                if (kendoSelect) {
                    // Kendo
                    return { Name: control, ControlType: ControlTypeEnum.KendoSelect, Object: kendoSelect };
                }
                else {
                    // Native
                    return { Name: control, ControlType: ControlTypeEnum.Select, Object: elemSel };
                }
            }
            else {
                var elemInp = $YetaWF.getElement1BySelectorCond(".t_row.t_" + control.toLowerCase() + " input[name$='" + control + "']", [this.Control]);
                if (elemInp) {
                    return { Name: control, ControlType: ControlTypeEnum.Input, Object: elemInp };
                }
            }
            // try hidden field
            var elemHid = $YetaWF.getElement1BySelectorCond("input[name$='" + control + "'][type='hidden']", [this.Control]);
            if (elemHid) {
                return { Name: control, ControlType: ControlTypeEnum.Hidden, Object: elemHid };
            }
            throw "No control found for " + control;
        };
        /**
         * Update all dependent fields.
         */
        PropertyListComponent.prototype.update = function () {
            // for each dependent, verify that all its conditions are true
            var deps = this.ControlData.Dependents;
            for (var _i = 0, deps_1 = deps; _i < deps_1.length; _i++) {
                var dep = deps_1[_i];
                var depRow = $YetaWF.getElement1BySelectorCond(".t_row.t_" + dep.Prop.toLowerCase(), [this.Control]); // the propertylist row affected
                if (!depRow)
                    continue;
                var valid = false; // we assume not valid unless we find a matching entry
                for (var _a = 0, _b = dep.Values; _a < _b.length; _a++) {
                    var value = _b[_a];
                    // get the controlling control's value
                    var ctrlIndex = this.ControlData.Controls.indexOf(value.ControlProp);
                    if (ctrlIndex < 0)
                        throw "Dependent " + dep.Prop + " references controlling control " + value.ControlProp + " which doesn't exist";
                    var controlItem = this.ControllingControls[ctrlIndex];
                    var controlValue;
                    switch (controlItem.ControlType) {
                        case ControlTypeEnum.Input:
                            var inputElem = controlItem.Object;
                            var controlRow = $YetaWF.elementClosest(inputElem, ".t_row");
                            if (controlRow.style.display === "") {
                                if (inputElem.type.toLowerCase() === "checkbox") {
                                    controlValue = inputElem.checked ? "1" : "0";
                                }
                                else {
                                    controlValue = inputElem.value;
                                }
                                valid = true;
                            }
                            break;
                        case ControlTypeEnum.Select:
                            var selectElem = controlItem.Object;
                            var controlRow = $YetaWF.elementClosest(selectElem, ".t_row");
                            if (controlRow.style.display === "") {
                                controlValue = selectElem.value;
                                valid = true;
                            }
                            break;
                        case ControlTypeEnum.KendoSelect:
                            var dropdownList = controlItem.Object;
                            var controlRow = $YetaWF.elementClosest(dropdownList.Control, ".t_row");
                            if (controlRow.style.display === "") {
                                controlValue = dropdownList.value;
                                valid = true;
                            }
                            break;
                        case ControlTypeEnum.Hidden:
                            var inputElem = controlItem.Object;
                            controlValue = inputElem.value;
                            valid = true;
                            break;
                    }
                    if (valid) {
                        // test condition
                        switch (value.ValueType) {
                            case ValueTypeEnum.EqualIntValue:
                                // need one matching value
                                var intValues = value.ValueObject;
                                var found = false;
                                for (var _c = 0, intValues_1 = intValues; _c < intValues_1.length; _c++) {
                                    var intValue = intValues_1[_c];
                                    if (intValue === Number(controlValue)) {
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                    valid = false;
                                break;
                            case ValueTypeEnum.EqualStringValue:
                                // need one matching value
                                var strValues = value.ValueObject;
                                var found = false;
                                for (var _d = 0, strValues_1 = strValues; _d < strValues_1.length; _d++) {
                                    var strValue = strValues_1[_d];
                                    if (strValue === controlValue) {
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
                    }
                    if (valid)
                        break;
                }
                if (dep.Disable) {
                    $YetaWF.elementAndChildrenEnableToggle(depRow, valid);
                }
                else {
                    if (valid) {
                        depRow.style.display = "";
                        $YetaWF.processActivateDivs([depRow]); // init any controls that just became visible
                    }
                    else
                        depRow.style.display = "none";
                }
                var affected = $YetaWF.getElementsBySelector("input,select,textarea", [depRow]);
                if (valid) {
                    for (var _e = 0, affected_1 = affected; _e < affected_1.length; _e++) {
                        var e = affected_1[_e];
                        $YetaWF.elementRemoveClass(e, YConfigs.Forms.CssFormNoValidate);
                    }
                }
                else {
                    for (var _f = 0, affected_2 = affected; _f < affected_2.length; _f++) {
                        var e = affected_2[_f];
                        $YetaWF.elementAddClass(e, YConfigs.Forms.CssFormNoValidate);
                    }
                }
            }
        };
        PropertyListComponent.tabInitjQuery = function (tabCtrlId, activeTab, activeTabId) {
            var tabCtrl = $YetaWF.getElementById(tabCtrlId);
            $YetaWF.elementAddClass(tabCtrl, "t_jquery");
            $(tabCtrl).tabs({
                active: activeTab,
                activate: function (ev, ui) {
                    if (ui.newPanel !== undefined) {
                        $YetaWF.processActivateDivs([ui.newPanel[0]]);
                        $YetaWF.processPanelSwitched(ui.newPanel[0]);
                        if (activeTabId) {
                            $("#" + activeTabId).val((ui.newTab.length > 0) ? Number(ui.newTab.attr("data-tab")) : -1);
                        }
                    }
                }
            });
        };
        PropertyListComponent.tabInitKendo = function (tabCtrlId, activeTab, activeTabId) {
            // mark the active tab with .k-state-active before initializing the tabstrip
            var tabs = $YetaWF.getElementsBySelector("#" + tabCtrlId + ">ul>li");
            for (var _i = 0, tabs_1 = tabs; _i < tabs_1.length; _i++) {
                var tab = tabs_1[_i];
                $YetaWF.elementRemoveClass(tab, "k-state-active");
            }
            $YetaWF.elementAddClass(tabs[activeTab], "k-state-active");
            // init tab control
            var tabCtrl = $YetaWF.getElementById(tabCtrlId);
            $YetaWF.elementAddClass(tabCtrl, "t_kendo");
            $(tabCtrl).kendoTabStrip({
                animation: false,
                activate: function (ev) {
                    if (ev.contentElement !== undefined) {
                        $YetaWF.processActivateDivs([ev.contentElement]);
                        $YetaWF.processPanelSwitched(ev.contentElement);
                        if (activeTabId)
                            $("#" + activeTabId).val($(ev.item).attr("data-tab"));
                    }
                }
            }).data("kendoTabStrip");
        };
        return PropertyListComponent;
    }());
    YetaWF_ComponentsHTML.PropertyListComponent = PropertyListComponent;
    $YetaWF.registerClearDiv(function (tag) {
        var list = $YetaWF.getElementsBySelector(".yt_propertylisttabbed.t_jquery", [tag]);
        for (var _i = 0, list_1 = list; _i < list_1.length; _i++) {
            var el = list_1[_i];
            var tabsJq = $(el);
            if (!tabsJq)
                throw "No jquery ui object found"; /*DEBUG*/
            tabsJq.tabs("destroy");
        }
        list = $YetaWF.getElementsBySelector(".yt_propertylisttabbed.t_kendo", [tag]);
        for (var _a = 0, list_2 = list; _a < list_2.length; _a++) {
            var el = list_2[_a];
            var tabsKn = $(el).data("kendoTabStrip");
            if (!tabsKn)
                throw "No kendo object found"; /*DEBUG*/
            tabsKn.destroy();
        }
    });
    // The property list needs a bit of special love when it's made visible. Because panels have no width/height
    // while the propertylist is not visible (jquery implementation), when a propertylist is made visible using show(),
    // the default panel is not sized correctly. If you explicitly show() a propertylist that has never been visible,
    // call the following to cause the propertylist to be resized correctly:
    // ComponentsHTML.processPropertyListVisible(div);
    // div is any HTML element - all items (including child items) are checked for propertylists.
    ComponentsHTMLHelper.registerPropertyListVisible(function (tag) {
        // jquery tabs
        var tabsJq = $YetaWF.getElementsBySelector(".ui-tabs", [tag]);
        for (var _i = 0, tabsJq_1 = tabsJq; _i < tabsJq_1.length; _i++) {
            var tabJq = tabsJq_1[_i];
            var id = tabJq.id;
            if (id === undefined)
                throw "No id on tab control"; /*DEBUG*/
            var tabidJq = Number($(tabJq).tabs("option", "active"));
            if (tabidJq >= 0) {
                var panel = $YetaWF.getElement1BySelector("#" + id + "_tab" + tabidJq, [tabJq]);
                $YetaWF.processActivateDivs([panel]);
                $YetaWF.processPanelSwitched(panel);
            }
        }
        // kendo tabs
        var tabsKn = $YetaWF.getElementsBySelector(".k-widget.k-tabstrip", [tag]);
        for (var _a = 0, tabsKn_1 = tabsKn; _a < tabsKn_1.length; _a++) {
            var tabKn = tabsKn_1[_a];
            var id = tabKn.id;
            if (id === undefined)
                throw "No id on tab control"; /*DEBUG*/
            var ts = $(tabKn).data("kendoTabStrip");
            var tabidKn = Number(ts.select().attr("data-tab"));
            if (tabidKn >= 0) {
                var panel = $YetaWF.getElement1BySelector("#" + id + "-tab" + (+tabidKn + 1), [tabKn]);
                $YetaWF.processActivateDivs([panel]);
                $YetaWF.processPanelSwitched(panel);
            }
        }
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=PropertyList.js.map
