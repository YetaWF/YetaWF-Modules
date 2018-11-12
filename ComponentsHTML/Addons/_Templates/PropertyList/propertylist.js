"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var PropertyListComponent = /** @class */ (function () {
        function PropertyListComponent(controlId, controlData) {
            var _this = this;
            this.Control = $YetaWF.getElementById(controlId);
            this.ControlData = controlData;
            // Handle change events
            var controlData = this.ControlData;
            for (var _i = 0, _a = controlData.Controls; _i < _a.length; _i++) {
                var control = _a[_i];
                var elemSel = $YetaWF.getElement1BySelectorCond(".t_row.t_" + control.toLowerCase() + " select[name$='" + control + "']", [this.Control]);
                if (elemSel) {
                    $YetaWF.registerEventHandler(elemSel, "change", null, function (ev) {
                        if (!elemSel)
                            return true;
                        _this.changeSelect(elemSel);
                        return false;
                    });
                    elemSel.addEventListener("dropdownlist_change", function (evt) {
                        if (!elemSel)
                            return;
                        _this.changeSelect(elemSel);
                    });
                }
                var elemInp = $YetaWF.getElement1BySelectorCond(".t_row.t_" + control.toLowerCase() + " input[name$='" + control + "']", [this.Control]);
                if (elemInp) {
                    $YetaWF.registerEventHandler(elemInp, "change", null, function (ev) {
                        if (!elemInp)
                            return true;
                        _this.changeInput(elemInp);
                        return false;
                    });
                }
            }
            // Initialize initial form
            for (var _b = 0, _c = controlData.Controls; _b < _c.length; _b++) {
                var control = _c[_b];
                var elemSel = $YetaWF.getElement1BySelectorCond(".t_row.t_" + control.toLowerCase() + " select[name$='" + control + "']", [this.Control]);
                if (elemSel)
                    this.changeSelect(elemSel);
                var elemInp = $YetaWF.getElement1BySelectorCond(".t_row.t_" + control.toLowerCase() + " input[name$='" + control + "']", [this.Control]);
                if (elemInp)
                    this.changeInput(elemInp);
            }
        }
        PropertyListComponent.prototype.changeSelect = function (elem) {
            var name = $YetaWF.getAttribute(elem, "name"); // name of controlling item (an enum)
            this.update(name, elem.value);
        };
        PropertyListComponent.prototype.changeInput = function (elem) {
            var name = $YetaWF.getAttribute(elem, "name"); // name of controlling item (an enum)
            var value = elem.value;
            if (elem.type.toLowerCase() === "checkbox") {
                value = elem.checked ? "1" : "0";
            }
            this.update(name, value);
        };
        PropertyListComponent.prototype.update = function (name, value) {
            var deps = this.ControlData.Dependents;
            for (var _i = 0, deps_1 = deps; _i < deps_1.length; _i++) {
                var dep = deps_1[_i];
                if (name === dep.ControlProp || name.endsWith("." + dep.ControlProp)) { // this entry is for the controlling item?
                    var row = $YetaWF.getElement1BySelector(".t_row.t_" + dep.Prop.toLowerCase(), [this.Control]); // the propertylist row affected
                    var intValue = Number(value);
                    var found = false;
                    for (var _a = 0, _b = dep.IntValues; _a < _b.length; _a++) {
                        var v = _b[_a];
                        if (v === intValue) {
                            found = true;
                            break;
                        }
                    }
                    if (dep.Disable) {
                        $YetaWF.elementEnableToggle(row, found);
                    }
                    else {
                        if (found)
                            row.style.display = "";
                        else
                            row.style.display = "none";
                        $YetaWF.processActivateDivs([row]); // init any controls that just became visible
                    }
                    var affected = $YetaWF.getElementsBySelector("input,select,textarea", [row]);
                    if (found) {
                        for (var _c = 0, affected_1 = affected; _c < affected_1.length; _c++) {
                            var e = affected_1[_c];
                            $YetaWF.elementRemoveClass(e, YConfigs.Forms.CssFormNoValidate);
                        }
                    }
                    else {
                        for (var _d = 0, affected_2 = affected; _d < affected_2.length; _d++) {
                            var e = affected_2[_d];
                            $YetaWF.elementAddClass(e, YConfigs.Forms.CssFormNoValidate);
                        }
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
