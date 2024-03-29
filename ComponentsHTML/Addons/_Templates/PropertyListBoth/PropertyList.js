"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var PropertyListStyleEnum;
    (function (PropertyListStyleEnum) {
        PropertyListStyleEnum[PropertyListStyleEnum["Tabbed"] = 0] = "Tabbed";
        PropertyListStyleEnum[PropertyListStyleEnum["Boxed"] = 1] = "Boxed";
        PropertyListStyleEnum[PropertyListStyleEnum["BoxedWithCategories"] = 2] = "BoxedWithCategories";
        PropertyListStyleEnum[PropertyListStyleEnum["BoxedWithHeaders"] = 3] = "BoxedWithHeaders";
    })(PropertyListStyleEnum || (PropertyListStyleEnum = {}));
    var PropertyListComponent = /** @class */ (function (_super) {
        __extends(PropertyListComponent, _super);
        function PropertyListComponent(controlId, setup, controlData) {
            var _this = _super.call(this, controlId, PropertyListComponent.TEMPLATE, PropertyListComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: function (control, enable, clearOnDisable) {
                    // if this propertylist is within a template, delegate enable/disable to that template
                    var parentElem = control.Control.parentElement;
                    if (!parentElem)
                        return;
                    var template = YetaWF.ComponentBase.getTemplateFromTagCond(parentElem);
                    if (!template)
                        return;
                    var controlItem = ControlsHelper.getControlItemFromTemplate(template);
                    ControlsHelper.enableToggle(controlItem, enable, clearOnDisable);
                    control.update(); // update all dependent fields
                    // don't submit contents if disabled
                    $YetaWF.elementRemoveClass(control.Control, YConfigs.Forms.CssFormNoSubmitContents);
                    if (!enable)
                        $YetaWF.elementAddClass(control.Control, YConfigs.Forms.CssFormNoSubmitContents);
                },
            }) || this;
            _this.MasonryElem = null;
            _this.MinWidth = 0;
            _this.CurrWidth = 0;
            _this.ColumnDefIndex = -1;
            _this.ControlData = controlData;
            _this.Setup = setup;
            // column handling
            if (_this.Setup.Style === PropertyListStyleEnum.Boxed || _this.Setup.Style === PropertyListStyleEnum.BoxedWithCategories || _this.Setup.Style === PropertyListStyleEnum.BoxedWithHeaders) {
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
            }
            // expand/collapse handling
            if (_this.Setup.Style === PropertyListStyleEnum.Boxed || _this.Setup.Style === PropertyListStyleEnum.BoxedWithCategories || _this.Setup.Style === PropertyListStyleEnum.BoxedWithHeaders) {
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
                    var item = ControlsHelper.getControlItemByName(control, _this.Control);
                    switch (item.ControlType) {
                        case YetaWF_ComponentsHTML.ControlTypeEnum.Input:
                            $YetaWF.registerMultipleEventHandlers([item.Template], ["change", "input"], null, function (ev) {
                                _this.update();
                                return true;
                            });
                            break;
                        case YetaWF_ComponentsHTML.ControlTypeEnum.Select:
                            $YetaWF.registerEventHandler(item.Template, "change", null, function (ev) {
                                _this.update();
                                return true;
                            });
                            break;
                        case YetaWF_ComponentsHTML.ControlTypeEnum.TextArea:
                            $YetaWF.registerMultipleEventHandlers([item.Template], ["change", "input"], null, function (ev) {
                                _this.update();
                                return true;
                            });
                            break;
                        case YetaWF_ComponentsHTML.ControlTypeEnum.Div:
                        case YetaWF_ComponentsHTML.ControlTypeEnum.Hidden:
                            break;
                        default:
                        case YetaWF_ComponentsHTML.ControlTypeEnum.Template:
                            if (!item.ChangeEvent)
                                throw "No ChangeEvent for control type ".concat(item.ControlType);
                            var control_1 = $YetaWF.getObjectData(item.Template);
                            $YetaWF.registerCustomEventHandler(control_1.Control, item.ChangeEvent, null, function (evt) {
                                _this.update();
                                return true;
                            });
                            break;
                    }
                }
            }
            // Initialize initial form
            _this.update();
            _this.resize();
            $YetaWF.registerCustomEventHandler(_this.Control, "propertylist_relayout", null, function (ev) {
                _this.layout();
                return false;
            });
            /**
             * Collapse whichever box is expanded
             */
            $YetaWF.registerCustomEventHandler(_this.Control, "propertylist_collapse", null, function (ev) {
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
            var winRect = this.Control.getBoundingClientRect();
            if (winRect.width < this.MinWidth) {
                this.destroyMasonry();
            }
            else if (!this.MasonryElem || winRect.width !== this.CurrWidth) {
                var newIndex = this.getColumnDefIndex();
                if (this.ColumnDefIndex !== newIndex) {
                    this.destroyMasonry();
                    this.MasonryElem = this.createMasonry();
                }
            }
            var haveExpandedBox = $YetaWF.getElement1BySelectorCond(".t_proptable .t_propexpanded");
            this.toggleFormButtons(!haveExpandedBox);
        };
        PropertyListComponent.prototype.expandCollapseBox = function (box) {
            var isExpanded = $YetaWF.elementHasClass(box, "t_propexpanded");
            this.destroyMasonry();
            if (!$YetaWF.elementHasClass(box, "t_propexpandable"))
                return;
            if (isExpanded) {
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
            $YetaWF.sendContainerResizeEvent(box);
            this.resize();
        };
        PropertyListComponent.prototype.expandBox = function (box) {
            var boxes = $YetaWF.getElementsBySelector(".t_proptable", [this.Control]);
            for (var _i = 0, boxes_2 = boxes; _i < boxes_2.length; _i++) {
                var b = boxes_2[_i];
                $YetaWF.elementRemoveClasses(b, ["t_propexpanded", "t_propcollapsed", "t_prophide"]);
                if (b !== box)
                    $YetaWF.elementAddClass(b, "t_prophide");
            }
            $YetaWF.elementAddClass(box, "t_propexpanded");
            // hide apply/save/cancel buttons while expanded
            this.toggleFormButtons(false);
            $YetaWF.sendContainerResizeEvent(box);
        };
        PropertyListComponent.prototype.toggleFormButtons = function (show) {
            var form = $YetaWF.Forms.getForm(this.Control);
            // make the form submit/nosubmit
            $YetaWF.elementRemoveClass(form, "yform-nosubmit");
            if (!show)
                $YetaWF.elementAddClass(form, "yform-nosubmit");
            // show/hide buttons
            var buttonList = $YetaWF.getElementsBySelector(".t_detailsbuttons", [form]);
            for (var _i = 0, buttonList_1 = buttonList; _i < buttonList_1.length; _i++) {
                var buttons = buttonList_1[_i];
                buttons.style.display = show ? "block" : "none";
            }
        };
        PropertyListComponent.prototype.createMasonry = function () {
            var winRect = this.Control.getBoundingClientRect();
            this.CurrWidth = winRect.width;
            this.ColumnDefIndex = this.getColumnDefIndex();
            var cols = this.Setup.ColumnStyles[this.ColumnDefIndex].Columns;
            $YetaWF.elementAddClass(this.Control, "t_col".concat(cols));
            $YetaWF.elementAddClass(this.Control, "t_hasmasonry");
            var boxes = $YetaWF.getElementsBySelector(".t_proptable", [this.Control]);
            for (var _i = 0, boxes_3 = boxes; _i < boxes_3.length; _i++) {
                var b = boxes_3[_i];
                $YetaWF.elementRemoveClasses(b, ["t_propexpanded", "t_propcollapsed"]);
                $YetaWF.elementAddClass(b, "t_propcollapsed");
            }
            return new Masonry(this.Control, {
                itemSelector: ".t_proptable",
                gutter: 20,
                horizontalOrder: false,
                transitionDuration: "0.1s",
                resize: false,
                initLayout: true
                //columnWidth: 200
            });
        };
        PropertyListComponent.prototype.destroyMasonry = function () {
            if (this.MasonryElem) {
                this.CurrWidth = 0;
                this.ColumnDefIndex = -1;
                this.MasonryElem.destroy();
                this.MasonryElem = null;
                $YetaWF.elementRemoveClasses(this.Control, ["t_hasmasonry", "t_col1", "t_col2", "t_col3", "t_col4", "t_col5"]);
            }
            var boxes = $YetaWF.getElementsBySelector(".t_proptable", [this.Control]);
            for (var _i = 0, boxes_4 = boxes; _i < boxes_4.length; _i++) {
                var b = boxes_4[_i];
                $YetaWF.elementRemoveClasses(b, ["t_propexpanded", "t_propcollapsed", "t_prophide"]);
            }
        };
        PropertyListComponent.prototype.getColumnDefIndex = function () {
            var winRect = this.Control.getBoundingClientRect();
            var width = winRect.width;
            var index = -1;
            for (var _i = 0, _a = this.Setup.ColumnStyles; _i < _a.length; _i++) {
                var style = _a[_i];
                if (width < style.MinWindowSize)
                    return index;
                ++index;
            }
            return index;
        };
        /**
         * Update all dependent fields (forms). May be a propertylist embedded in another propertylist.
         */
        PropertyListComponent.prototype.update = function () {
            if (!this.ControlData)
                return;
            var form = $YetaWF.Forms.getForm(this.Control);
            // for each dependent, verify that all its conditions are true
            var deps = this.ControlData.Dependents;
            for (var _i = 0, deps_1 = deps; _i < deps_1.length; _i++) {
                var dep = deps_1[_i];
                var depRow = $YetaWF.getElement1BySelectorCond(".t_row.t_".concat(dep.PropShort.toLowerCase()), [this.Control]); // the propertylist row affected
                if (!depRow)
                    continue;
                var hidden = false;
                for (var _a = 0, _b = dep.HideValues; _a < _b.length; _a++) { // hidden hides only, it never makes it visible (use process for that instead)
                    var expr = _b[_a];
                    switch (expr.Op) {
                        case YetaWF_ComponentsHTML.OpEnum.HideIf:
                        case YetaWF_ComponentsHTML.OpEnum.HideIfSupplied: {
                            var valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(null, form, expr.Op, expr.ExprList);
                            if (valid) {
                                this.toggle(dep, depRow, false, false, false);
                                hidden = true;
                                break;
                            }
                            break;
                        }
                        case YetaWF_ComponentsHTML.OpEnum.HideIfNot:
                        case YetaWF_ComponentsHTML.OpEnum.HideIfNotSupplied: {
                            var valid = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(null, form, expr.Op, expr.ExprList);
                            if (!valid) {
                                this.toggle(dep, depRow, false, false, false);
                                hidden = true;
                                break;
                            }
                            break;
                        }
                        default:
                            throw "Unexpected Op ".concat(expr.Op, " in update(HideValues)");
                    }
                    if (hidden)
                        break;
                }
                if (!hidden) {
                    var process = true;
                    var disable = false;
                    var clearOnDisable = false;
                    if (dep.ProcessValues.length > 0) {
                        process = false;
                        for (var _c = 0, _d = dep.ProcessValues; _c < _d.length; _c++) {
                            var expr = _d[_c];
                            switch (expr.Op) {
                                case YetaWF_ComponentsHTML.OpEnum.ProcessIf:
                                case YetaWF_ComponentsHTML.OpEnum.ProcessIfSupplied: {
                                    var v = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(null, form, expr.Op, expr.ExprList);
                                    if (v)
                                        process = true;
                                    else {
                                        disable = expr.Disable;
                                        clearOnDisable = expr.ClearOnDisable;
                                    }
                                    break;
                                }
                                case YetaWF_ComponentsHTML.OpEnum.ProcessIfNot:
                                case YetaWF_ComponentsHTML.OpEnum.ProcessIfNotSupplied: {
                                    var v = YetaWF_ComponentsHTML.ValidatorHelper.evaluateExpressionList(null, form, expr.Op, expr.ExprList);
                                    if (!v)
                                        process = true;
                                    else {
                                        disable = expr.Disable;
                                        clearOnDisable = expr.ClearOnDisable;
                                    }
                                    break;
                                }
                                default:
                                    throw "Unexpected Op ".concat(expr.Op, " in update(ProcessValues)");
                            }
                            if (process)
                                break;
                        }
                    }
                    if (process) {
                        this.toggle(dep, depRow, true, false, false);
                    }
                    else {
                        if (disable)
                            this.toggle(dep, depRow, true, true, clearOnDisable);
                        else
                            this.toggle(dep, depRow, false, false, false);
                    }
                }
            }
        };
        /**
         * Update all dependent fields (forms). Will find the outermost propertylist and update fields.
         */
        PropertyListComponent.updatePropertyListsFromTag = function (elem) {
            var propertyList = this.getTopMostPropertyListFromTagCond(elem);
            if (!propertyList)
                return;
            propertyList.update(); // update all dependent fields of topmost propertylist
            // and all contained property lists
            var proplists = $YetaWF.getElementsBySelector(PropertyListComponent.SELECTOR, [propertyList.Control]);
            for (var _i = 0, proplists_1 = proplists; _i < proplists_1.length; _i++) {
                var proplist = proplists_1[_i];
                var list = YetaWF.ComponentBaseDataImpl.getControlFromTag(proplist, PropertyListComponent.SELECTOR);
                list.update();
            }
        };
        PropertyListComponent.getTopMostPropertyListFromTagCond = function (elem) {
            var curr = elem;
            var topMost = null;
            for (; curr;) {
                var propertyList = YetaWF.ComponentBaseDataImpl.getControlFromTagCond(curr, PropertyListComponent.SELECTOR);
                if (!propertyList)
                    break;
                topMost = propertyList;
                curr = propertyList.parentElement;
            }
            return topMost;
        };
        PropertyListComponent.prototype.toggle = function (dep, depRow, show, disable, clearOnDisable) {
            // hides/shows rows (doesn't change the status or clear validation if the status is already correct)
            var clearVal = false;
            if (show) {
                if ($YetaWF.elementHasClass(depRow, "t_hidden")) {
                    $YetaWF.elementRemoveClass(depRow, "t_hidden");
                    clearVal = true;
                }
                if (disable) {
                    if (!$YetaWF.elementHasClass(depRow, "t_disabled")) {
                        $YetaWF.elementAddClass(depRow, "t_disabled");
                        clearVal = true;
                    }
                }
                else {
                    if ($YetaWF.elementHasClass(depRow, "t_disabled")) {
                        $YetaWF.elementRemoveClass(depRow, "t_disabled");
                        clearVal = true;
                    }
                }
                if (clearVal)
                    $YetaWF.sendActivateDivEvent([depRow]); // init any controls that just became visible
            }
            else {
                if (!$YetaWF.elementHasClass(depRow, "t_hidden")) {
                    $YetaWF.elementRemoveClass(depRow, "t_disabled");
                    $YetaWF.elementAddClass(depRow, "t_hidden");
                    clearVal = true;
                }
            }
            if (clearVal)
                $YetaWF.Forms.clearValidation(depRow);
            var controlItemDef = ControlsHelper.getControlItemByNameCond(dep.Prop, depRow); // there may not be an actual control, just a row with displayed info
            if (controlItemDef) {
                if (show) {
                    ControlsHelper.enableToggle(controlItemDef, !disable, clearOnDisable);
                }
                else {
                    ControlsHelper.enableToggle(controlItemDef, false, clearOnDisable);
                }
            }
        };
        PropertyListComponent.isRowVisible = function (tag) {
            var row = $YetaWF.elementClosestCond(tag, ".t_row");
            if (!row)
                return false;
            return !$YetaWF.elementHasClass(row, "t_hidden");
        };
        PropertyListComponent.isRowEnabled = function (tag) {
            var row = $YetaWF.elementClosestCond(tag, ".t_row");
            if (!row)
                return false;
            return !$YetaWF.elementHasClass(row, "t_disabled");
        };
        PropertyListComponent.relayout = function (container) {
            var ctrls = $YetaWF.getElementsBySelector(".yt_propertylist.t_boxedcat,.yt_propertylist.t_boxed", [container]);
            for (var _i = 0, ctrls_1 = ctrls; _i < ctrls_1.length; _i++) {
                var ctrl = ctrls_1[_i];
                var propertyList = YetaWF.ComponentBaseDataImpl.getControlFromTag(ctrl, PropertyListComponent.SELECTOR);
                propertyList.update();
                var event = document.createEvent("Event");
                event.initEvent("propertylist_collapse", false, true);
                ctrl.dispatchEvent(event);
            }
        };
        PropertyListComponent.prototype.expandBoxByCategory = function (name) {
            var box = $YetaWF.getElement1BySelector(".t_proptable.t_propexpandable.t_boxpanel-".concat(name.toLocaleLowerCase()), [this.Control]);
            this.expandCollapseBox(box);
            this.update();
        };
        PropertyListComponent.prototype.hideBoxesByCategory = function (names) {
            for (var _i = 0, names_1 = names; _i < names_1.length; _i++) {
                var name_1 = names_1[_i];
                this.hideBoxByCategory(name_1);
            }
        };
        PropertyListComponent.prototype.hideBoxByCategory = function (name) {
            var box = $YetaWF.getElement1BySelectorCond(".t_proptable.t_propexpandable.t_boxpanel-".concat(name.toLocaleLowerCase()), [this.Control]);
            if (!box)
                return;
            $YetaWF.elementRemoveClasses(box, ["t_propsuppress"]);
            $YetaWF.elementAddClasses(box, ["t_propsuppress", YConfigs.Forms.CssFormNoSubmitContents]);
        };
        PropertyListComponent.prototype.showBoxesByCategory = function (names) {
            for (var _i = 0, names_2 = names; _i < names_2.length; _i++) {
                var name_2 = names_2[_i];
                this.showBoxByCategory(name_2);
            }
        };
        PropertyListComponent.prototype.showBoxByCategory = function (name) {
            var box = $YetaWF.getElement1BySelectorCond(".t_proptable.t_propexpandable.t_boxpanel-".concat(name.toLocaleLowerCase()), [this.Control]);
            if (!box)
                return;
            $YetaWF.elementRemoveClasses(box, ["t_propsuppress", YConfigs.Forms.CssFormNoSubmitContents]);
        };
        PropertyListComponent.prototype.layout = function () {
            var _this = this;
            this.setLayout();
            if (this.MasonryElem) {
                setTimeout(function () {
                    if (_this.MasonryElem)
                        _this.MasonryElem.layout();
                }, 20);
            }
        };
        PropertyListComponent.prototype.resize = function () {
            if (this.Setup.Style === PropertyListStyleEnum.Boxed || this.Setup.Style === PropertyListStyleEnum.BoxedWithCategories || this.Setup.Style === PropertyListStyleEnum.BoxedWithHeaders) {
                this.layout();
            }
        };
        PropertyListComponent.TEMPLATE = "yt_propertylist";
        PropertyListComponent.SELECTOR = ".yt_propertylist";
        return PropertyListComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.PropertyListComponent = PropertyListComponent;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        var proplists = $YetaWF.getElementsBySelector(PropertyListComponent.SELECTOR);
        for (var _i = 0, proplists_2 = proplists; _i < proplists_2.length; _i++) {
            var proplist = proplists_2[_i];
            if ($YetaWF.elementHas(ev.detail.container, proplist)) {
                var list = YetaWF.ComponentBaseDataImpl.getControlFromTag(proplist, PropertyListComponent.SELECTOR);
                list.resize();
            }
        }
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTENTRESIZED, null, function (ev) {
        var proplists = $YetaWF.getElementsBySelector(PropertyListComponent.SELECTOR);
        for (var _i = 0, proplists_3 = proplists; _i < proplists_3.length; _i++) {
            var proplist = proplists_3[_i];
            if ($YetaWF.elementHas(proplist, ev.detail.tag)) {
                var list = YetaWF.ComponentBaseDataImpl.getControlFromTag(proplist, PropertyListComponent.SELECTOR);
                list.resize();
            }
        }
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        var proplists = $YetaWF.getElementsBySelector(PropertyListComponent.SELECTOR, ev.detail.containers);
        for (var _i = 0, proplists_4 = proplists; _i < proplists_4.length; _i++) {
            var proplist = proplists_4[_i];
            var list = YetaWF.ComponentBaseDataImpl.getControlFromTag(proplist, PropertyListComponent.SELECTOR);
            list.resize();
        }
        return true;
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=PropertyList.js.map
