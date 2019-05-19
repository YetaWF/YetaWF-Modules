"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var ValidityEnum;
    (function (ValidityEnum) {
        ValidityEnum[ValidityEnum["Valid"] = 0] = "Valid";
        ValidityEnum[ValidityEnum["ValidDisabled"] = 1] = "ValidDisabled";
        ValidityEnum[ValidityEnum["Invalid"] = 2] = "Invalid";
        ValidityEnum[ValidityEnum["InvalidDisabled"] = 3] = "InvalidDisabled";
    })(ValidityEnum || (ValidityEnum = {}));
    var ControlTypeEnum;
    (function (ControlTypeEnum) {
        ControlTypeEnum[ControlTypeEnum["Input"] = 0] = "Input";
        ControlTypeEnum[ControlTypeEnum["Select"] = 1] = "Select";
        ControlTypeEnum[ControlTypeEnum["KendoSelect"] = 2] = "KendoSelect";
        ControlTypeEnum[ControlTypeEnum["Hidden"] = 3] = "Hidden";
    })(ControlTypeEnum || (ControlTypeEnum = {}));
    var PropertyListStyleEnum;
    (function (PropertyListStyleEnum) {
        PropertyListStyleEnum[PropertyListStyleEnum["Tabbed"] = 0] = "Tabbed";
        PropertyListStyleEnum[PropertyListStyleEnum["Boxed"] = 1] = "Boxed";
        PropertyListStyleEnum[PropertyListStyleEnum["BoxedWithCategories"] = 2] = "BoxedWithCategories";
    })(PropertyListStyleEnum || (PropertyListStyleEnum = {}));
    var PropertyListComponent = /** @class */ (function (_super) {
        __extends(PropertyListComponent, _super);
        function PropertyListComponent(controlId, setup, controlData) {
            var _this = _super.call(this, controlId) || this;
            _this.ControllingControls = [];
            _this.MasonryElem = null;
            _this.MinWidth = 0;
            _this.CurrWidth = 0;
            _this.ColumnDefIndex = -1;
            _this.ControlData = controlData;
            _this.Setup = setup;
            // column handling
            if (_this.Setup.Style === PropertyListStyleEnum.Boxed || _this.Setup.Style === PropertyListStyleEnum.BoxedWithCategories) {
                _this.MinWidth = _this.Setup.ColumnStyles.length > 0 ? _this.Setup.ColumnStyles[0].MinWindowSize : 0;
                if (_this.Setup.InitialExpanded) {
                    var box = $YetaWF.getElement1BySelector(".t_propexpanded", [_this.Control]);
                    _this.expandBox(box);
                }
                else {
                    _this.ColumnDefIndex = _this.getColumnDefIndex();
                    if (_this.ColumnDefIndex >= 0)
                        _this.MasonryElem = _this.createMasonry();
                }
                setInterval(function () {
                    if (_this.MasonryElem)
                        _this.MasonryElem.layout();
                }, 1000);
            }
            // expand/collapse handling
            if (_this.Setup.Style === PropertyListStyleEnum.Boxed || _this.Setup.Style === PropertyListStyleEnum.BoxedWithCategories) {
                $YetaWF.registerEventHandler(_this.Control, "click", ".t_boxexpcoll", function (ev) {
                    _this.expandCollapseBox($YetaWF.elementClosest(ev.__YetaWFElem, ".t_proptable"));
                    return false;
                });
            }
            // Handle change events
            if (_this.ControlData) {
                var controlData = _this.ControlData;
                for (var _i = 0, _a = controlData.Controls; _i < _a.length; _i++) {
                    var control = _a[_i];
                    var controlItem = _this.getControlItem(control);
                    _this.ControllingControls.push(controlItem);
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
            }
            // Initialize initial form
            _this.update();
            $YetaWF.registerEventHandlerWindow("resize", null, function (ev) {
                if (_this.MasonryElem) {
                    _this.setLayout();
                    if (_this.MasonryElem)
                        _this.MasonryElem.layout();
                }
                return true;
            });
            $YetaWF.registerCustomEventHandler(_this, "propertylist_relayout", function (ev) {
                _this.setLayout();
                if (_this.MasonryElem)
                    _this.MasonryElem.layout();
                return false;
            });
            /**
             * Collapse whichever box is expanded
             */
            $YetaWF.registerCustomEventHandler(_this, "propertylist_collapse", function (ev) {
                _this.setLayout();
                var box = $YetaWF.getElement1BySelectorCond(".t_propexpanded", [_this.Control]);
                if (box) {
                    _this.collapseBox(box);
                }
                if (_this.MasonryElem)
                    _this.MasonryElem.layout();
                return false;
            });
            return _this;
        }
        PropertyListComponent.prototype.setLayout = function () {
            if (window.innerWidth < this.MinWidth) {
                this.destroyMasonry();
            }
            else if (!this.MasonryElem || window.innerWidth !== this.CurrWidth) {
                var newIndex = this.getColumnDefIndex();
                if (this.ColumnDefIndex !== newIndex) {
                    this.destroyMasonry();
                    this.MasonryElem = this.createMasonry();
                }
            }
        };
        PropertyListComponent.prototype.expandCollapseBox = function (box) {
            this.destroyMasonry();
            if (!$YetaWF.elementHasClass(box, "t_propexpandable"))
                return;
            if ($YetaWF.elementHasClass(box, "t_propexpanded")) {
                // box can collapse
                this.collapseBox(box);
                this.MasonryElem = this.createMasonry();
            }
            else {
                // box can expand
                this.expandBox(box);
            }
        };
        PropertyListComponent.prototype.collapseBox = function (box) {
            var boxes = $YetaWF.getElementsBySelector(".t_proptable", [this.Control]);
            for (var _i = 0, boxes_1 = boxes; _i < boxes_1.length; _i++) {
                var b = boxes_1[_i];
                $YetaWF.elementRemoveClasses(b, ["t_propexpanded", "t_propcollapsed", "t_prophide"]);
                $YetaWF.elementAddClass(b, "t_propcollapsed");
            }
            // show apply/save/cancel buttons again
            this.toggleFormButtons(true);
        };
        PropertyListComponent.prototype.expandBox = function (box) {
            var boxes = $YetaWF.getElementsBySelector(".t_proptable", [this.Control]);
            for (var _i = 0, boxes_2 = boxes; _i < boxes_2.length; _i++) {
                var b = boxes_2[_i];
                $YetaWF.elementRemoveClasses(b, ["t_propexpanded", "t_propcollapsed"]);
                if (b !== box)
                    $YetaWF.elementAddClass(b, "t_prophide");
            }
            $YetaWF.elementAddClass(box, "t_propexpanded");
            // hide apply/save/cancel buttons while expanded
            this.toggleFormButtons(false);
        };
        PropertyListComponent.prototype.toggleFormButtons = function (show) {
            var form = $YetaWF.Forms.getForm(this.Control);
            // make the form submit/nosubmit
            $YetaWF.elementRemoveClass(form, YConfigs.Forms.CssFormNoSubmit);
            if (!show)
                $YetaWF.elementAddClass(form, YConfigs.Forms.CssFormNoSubmit);
            // show/hide buttons
            var buttonList = $YetaWF.getElementsBySelector(".t_detailsbuttons", [form]);
            for (var _i = 0, buttonList_1 = buttonList; _i < buttonList_1.length; _i++) {
                var buttons = buttonList_1[_i];
                buttons.style.display = show ? "block" : "none";
            }
        };
        PropertyListComponent.prototype.createMasonry = function () {
            this.CurrWidth = window.innerWidth;
            this.ColumnDefIndex = this.getColumnDefIndex();
            var cols = this.Setup.ColumnStyles[this.ColumnDefIndex].Columns;
            $YetaWF.elementAddClass(this.Control, "t_col" + cols);
            return new Masonry(this.Control, {
                itemSelector: ".t_proptable",
                horizontalOrder: true,
                transitionDuration: "0.1s",
                resize: false,
                initLayout: true
                //columnWidth: 200
            });
        };
        PropertyListComponent.prototype.destroyMasonry = function () {
            this.CurrWidth = 0;
            this.ColumnDefIndex = -1;
            if (this.MasonryElem) {
                this.MasonryElem.destroy();
                this.MasonryElem = null;
                $YetaWF.elementRemoveClass(this.Control, "t_col1");
                $YetaWF.elementRemoveClass(this.Control, "t_col2");
                $YetaWF.elementRemoveClass(this.Control, "t_col3");
                $YetaWF.elementRemoveClass(this.Control, "t_col4");
                $YetaWF.elementRemoveClass(this.Control, "t_col5");
            }
        };
        PropertyListComponent.prototype.getColumnDefIndex = function () {
            var width = window.innerWidth;
            var index = -1;
            for (var _i = 0, _a = this.Setup.ColumnStyles; _i < _a.length; _i++) {
                var style = _a[_i];
                if (width < style.MinWindowSize)
                    return index;
                ++index;
            }
            return index;
        };
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
            var elemInp = $YetaWF.getElement1BySelectorCond(".t_row.t_" + control.toLowerCase() + " input[name$='" + control + "']", [this.Control]);
            if (elemInp) {
                return { Name: control, ControlType: ControlTypeEnum.Input, Object: elemInp };
            }
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
            if (!this.ControlData)
                return;
            var form = $YetaWF.Forms.getForm(this.Control);
            // for each dependent, verify that all its conditions are true
            var deps = this.ControlData.Dependents;
            for (var _i = 0, deps_1 = deps; _i < deps_1.length; _i++) {
                var dep = deps_1[_i];
                var depRow = $YetaWF.getElement1BySelectorCond(".t_row.t_" + dep.Prop.toLowerCase(), [this.Control]); // the propertylist row affected
                if (!depRow)
                    continue;
                var hidden = false;
                for (var _a = 0, _b = dep.HideValues; _a < _b.length; _a++) { // hidden hides only, it never makes it visible (use process for that instead)
                    var expr = _b[_a];
                    var validity = this.getValidity(form, dep, expr);
                    switch (validity) {
                        case ValidityEnum.ValidDisabled:
                            this.toggle(dep, depRow, false, true);
                            hidden = true;
                            break;
                        case ValidityEnum.Valid:
                            this.toggle(dep, depRow, false, false);
                            hidden = true;
                            break;
                        default:
                            break;
                    }
                }
                if (!hidden) {
                    var validity = ValidityEnum.Valid;
                    if (dep.ProcessValues.length > 0) {
                        validity = ValidityEnum.Invalid;
                        for (var _c = 0, _d = dep.ProcessValues; _c < _d.length; _c++) {
                            var expr = _d[_c];
                            var v = this.getValidity(form, dep, expr);
                            switch (v) {
                                case ValidityEnum.ValidDisabled:
                                case ValidityEnum.Valid:
                                    validity = ValidityEnum.Valid;
                                    break;
                                case ValidityEnum.InvalidDisabled:
                                    if (validity === ValidityEnum.Invalid)
                                        validity = ValidityEnum.InvalidDisabled;
                                    break;
                                case ValidityEnum.Invalid:
                                    break;
                                default:
                                    break;
                            }
                            if (validity === ValidityEnum.Valid)
                                break;
                        }
                    }
                    switch (validity) {
                        //case ValidityEnum.ValidDisabled:
                        case ValidityEnum.Valid:
                            this.toggle(dep, depRow, true, false);
                            break;
                        case ValidityEnum.InvalidDisabled:
                            this.toggle(dep, depRow, true, true);
                            break;
                        case ValidityEnum.Invalid:
                            this.toggle(dep, depRow, false, false);
                            break;
                        default:
                            break;
                    }
                }
            }
        };
        PropertyListComponent.prototype.toggle = function (dep, depRow, show, disable) {
            $YetaWF.Forms.clearValidation(depRow);
            if (show) {
                depRow.style.display = "";
                $YetaWF.processActivateDivs([depRow]); // init any controls that just became visible
            }
            else {
                depRow.style.display = "none";
            }
            //var affected = $YetaWF.getElementsBySelector(`input[name='${dep.Prop}'], select[name='${dep.Prop}'], textarea[name='${dep.Prop}']`, [depRow]);
            var affected = $YetaWF.getElementsBySelector("input,select,textarea", [depRow]);
            if (show) {
                if (disable) {
                    for (var _i = 0, affected_1 = affected; _i < affected_1.length; _i++) {
                        var e = affected_1[_i];
                        $YetaWF.elementEnableToggle(e, false);
                        $YetaWF.elementAddClasses(e, [YConfigs.Forms.CssFormNoValidate, YConfigs.Forms.CssFormNoSubmit]);
                    }
                }
                else {
                    for (var _a = 0, affected_2 = affected; _a < affected_2.length; _a++) {
                        var e = affected_2[_a];
                        $YetaWF.elementEnableToggle(e, true);
                        $YetaWF.elementRemoveClasses(e, [YConfigs.Forms.CssFormNoValidate, YConfigs.Forms.CssFormNoSubmit]);
                    }
                }
            }
            else {
                for (var _b = 0, affected_3 = affected; _b < affected_3.length; _b++) {
                    var e = affected_3[_b];
                    $YetaWF.elementEnableToggle(e, false);
                    $YetaWF.elementAddClasses(e, [YConfigs.Forms.CssFormNoValidate, YConfigs.Forms.CssFormNoSubmit]);
                }
            }
        };
        PropertyListComponent.prototype.getValidity = function (form, dep, exprEntry) {
            for (var _i = 0, _a = exprEntry.ExprList; _i < _a.length; _i++) {
                var expr = _a[_i];
                switch (exprEntry.Op) {
                    case YetaWF_ComponentsHTML.OpEnum.HideIf:
                    case YetaWF_ComponentsHTML.OpEnum.ProcessIf:
                        if (YetaWF_ComponentsHTML.ValidatorHelper.isExprValid(expr, form))
                            return exprEntry.Disable ? ValidityEnum.ValidDisabled : ValidityEnum.Valid;
                        break;
                    case YetaWF_ComponentsHTML.OpEnum.ProcessIfNot:
                        if (!YetaWF_ComponentsHTML.ValidatorHelper.isExprValid(expr, form))
                            return exprEntry.Disable ? ValidityEnum.ValidDisabled : ValidityEnum.Valid;
                    case YetaWF_ComponentsHTML.OpEnum.ProcessIfSupplied:
                        if (!YetaWF_ComponentsHTML.ValidatorHelper.isExprSupplied(expr, form))
                            return exprEntry.Disable ? ValidityEnum.ValidDisabled : ValidityEnum.Valid;
                    case YetaWF_ComponentsHTML.OpEnum.ProcessIfNotSupplied:
                        if (YetaWF_ComponentsHTML.ValidatorHelper.isExprSupplied(expr, form))
                            return exprEntry.Disable ? ValidityEnum.ValidDisabled : ValidityEnum.Valid;
                        if (YetaWF_ComponentsHTML.ValidatorHelper.isExprValid(expr, form))
                            return exprEntry.Disable ? ValidityEnum.ValidDisabled : ValidityEnum.Valid;
                        break;
                    default:
                        throw "Invalid Op " + exprEntry.Op + " in getValidity";
                }
            }
            return exprEntry.Disable ? ValidityEnum.InvalidDisabled : ValidityEnum.Invalid;
        };
        PropertyListComponent.relayout = function (container) {
            var ctrls = $YetaWF.getElementsBySelector(".yt_propertylistboxedcat,.yt_propertylistboxed", [container]);
            for (var _i = 0, ctrls_1 = ctrls; _i < ctrls_1.length; _i++) {
                var ctrl = ctrls_1[_i];
                var event = document.createEvent("Event");
                event.initEvent("propertylist_collapse", false, true);
                ctrl.dispatchEvent(event);
            }
        };
        PropertyListComponent.tabInitjQuery = function (tabCtrlId, activeTab, activeTabId) {
            ComponentsHTMLHelper.MUSTHAVE_JQUERYUI();
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
    }(YetaWF.ComponentBaseImpl));
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
