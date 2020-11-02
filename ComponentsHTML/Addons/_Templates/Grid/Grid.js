"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
// Kendo UI menu use
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var SizeStyleEnum;
    (function (SizeStyleEnum) {
        SizeStyleEnum[SizeStyleEnum["SizeGiven"] = 0] = "SizeGiven";
        SizeStyleEnum[SizeStyleEnum["SizeToFit"] = 1] = "SizeToFit";
        SizeStyleEnum[SizeStyleEnum["SizeAuto"] = 2] = "SizeAuto";
    })(SizeStyleEnum || (SizeStyleEnum = {}));
    var SortByEnum;
    (function (SortByEnum) {
        SortByEnum[SortByEnum["NotSpecified"] = 0] = "NotSpecified";
        SortByEnum[SortByEnum["Ascending"] = 1] = "Ascending";
        SortByEnum[SortByEnum["Descending"] = 2] = "Descending";
    })(SortByEnum || (SortByEnum = {}));
    var FilterOptionEnum;
    (function (FilterOptionEnum) {
        FilterOptionEnum[FilterOptionEnum["Equal"] = 1] = "Equal";
        FilterOptionEnum[FilterOptionEnum["NotEqual"] = 2] = "NotEqual";
        FilterOptionEnum[FilterOptionEnum["LessThan"] = 3] = "LessThan";
        FilterOptionEnum[FilterOptionEnum["LessEqual"] = 4] = "LessEqual";
        FilterOptionEnum[FilterOptionEnum["GreaterThan"] = 5] = "GreaterThan";
        FilterOptionEnum[FilterOptionEnum["GreaterEqual"] = 6] = "GreaterEqual";
        FilterOptionEnum[FilterOptionEnum["StartsWith"] = 7] = "StartsWith";
        FilterOptionEnum[FilterOptionEnum["NotStartsWith"] = 8] = "NotStartsWith";
        FilterOptionEnum[FilterOptionEnum["Contains"] = 9] = "Contains";
        FilterOptionEnum[FilterOptionEnum["NotContains"] = 10] = "NotContains";
        FilterOptionEnum[FilterOptionEnum["Endswith"] = 11] = "Endswith";
        FilterOptionEnum[FilterOptionEnum["NotEndswith"] = 12] = "NotEndswith";
        FilterOptionEnum[FilterOptionEnum["All"] = 65535] = "All";
    })(FilterOptionEnum || (FilterOptionEnum = {}));
    var FilterBoolEnum;
    (function (FilterBoolEnum) {
        FilterBoolEnum[FilterBoolEnum["All"] = 0] = "All";
        FilterBoolEnum[FilterBoolEnum["Yes"] = 1] = "Yes";
        FilterBoolEnum[FilterBoolEnum["No"] = 2] = "No";
    })(FilterBoolEnum || (FilterBoolEnum = {}));
    var Grid = /** @class */ (function (_super) {
        __extends(Grid, _super);
        function Grid(controlId, setup) {
            var _this = _super.call(this, controlId, Grid.TEMPLATE, Grid.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: function (control) {
                    var total = control.GetTotalRecords();
                    if (!total)
                        return null;
                    return total.toString();
                },
                Enable: function (control, enable, clearOnDisable) {
                    control.enable(enable);
                    // clearOnDisable not supported
                },
            }) || this;
            _this.BtnReload = null;
            _this.BtnSearch = null;
            _this.BtnTop = null;
            _this.BtnPrev = null;
            _this.BtnNext = null;
            _this.BtnBottom = null;
            _this.FilterBar = null;
            _this.PagerTotals = null;
            _this.InputPage = null;
            _this.SelectPageSize = null;
            _this.ColumnResizeBar = null;
            _this.ColumnResizeHeader = null;
            _this.LoadingDiv = null;
            _this.SubmitCheckCol = -1; // column with checkbox determining whether to submit record
            _this.reloadInProgress = false;
            _this.reorderingInProgress = false;
            _this.reorderingRowElement = null;
            _this.Setup = setup;
            _this.TBody = $YetaWF.getElement1BySelector("tbody", [_this.Control]);
            _this.convertToPix();
            if (_this.Setup.ShowPager) {
                _this.BtnReload = $YetaWF.getElement1BySelectorCond(".tg_reload", [_this.Control]);
                _this.BtnSearch = $YetaWF.getElement1BySelectorCond(".tg_search", [_this.Control]);
                _this.BtnTop = $YetaWF.getElement1BySelectorCond(".tg_pgtop", [_this.Control]);
                _this.BtnPrev = $YetaWF.getElement1BySelectorCond(".tg_pgprev", [_this.Control]);
                _this.BtnNext = $YetaWF.getElement1BySelectorCond(".tg_pgnext", [_this.Control]);
                _this.BtnBottom = $YetaWF.getElement1BySelectorCond(".tg_pgbottom", [_this.Control]);
                _this.PagerTotals = $YetaWF.getElement1BySelectorCond(".tg_totals", [_this.Control]);
                if (_this.Setup.PageSize) {
                    _this.InputPage = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name$='.__Page']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [_this.Control]);
                    _this.SelectPageSize = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name$='.__PageSelection']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [_this.Control]);
                }
            }
            _this.FilterBar = $YetaWF.getElement1BySelectorCond(".tg_filter", [_this.Control]);
            _this.updateStatus();
            $YetaWF.getElement1BySelector(".tg_table", [_this.Control]).addEventListener("scroll", function (ev) {
                $YetaWF.sendContainerScrollEvent(_this.Control);
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "mouseover", ".tg_header th, .tg_filter .tg_button, .tg_pager .tg_button", function (ev) {
                if (!$YetaWF.elementHasClass(ev.__YetaWFElem, _this.Setup.HoverCss))
                    $YetaWF.elementAddClass(ev.__YetaWFElem, _this.Setup.HoverCss);
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "mouseout", ".tg_header th, .tg_filter .tg_button, .tg_pager .tg_button", function (ev) {
                $YetaWF.elementRemoveClass(ev.__YetaWFElem, _this.Setup.HoverCss);
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "mouseover", ".tg_resize", function (ev) {
                // don't allow mouseover to propagate and close tooltips
                $YetaWF.closeOverlays();
                return false;
            });
            $YetaWF.registerEventHandler(_this.Control, "click", ".tg_resize", function (ev) {
                return false;
            });
            // Show/hide filter bar with search button
            if (_this.Setup.CanFilter && _this.BtnSearch && _this.FilterBar) {
                $YetaWF.registerEventHandler(_this.BtnSearch, "click", null, function (ev) {
                    var filterBar = _this.FilterBar;
                    if ($YetaWF.isVisible(filterBar)) {
                        filterBar.style.display = "none";
                        _this.clearFilters();
                        _this.reload(0);
                    }
                    else {
                        filterBar.style.display = "";
                        $YetaWF.sendActivateDivEvent([filterBar]);
                    }
                    return false;
                });
            }
            // Reload
            if (_this.BtnReload) {
                $YetaWF.registerEventHandler(_this.BtnReload, "click", null, function (ev) {
                    if (!$YetaWF.elementHasClass(ev.__YetaWFElem, _this.Setup.DisabledCss))
                        _this.reload(_this.Setup.Page);
                    return false;
                });
                $YetaWF.registerModuleRefresh(_this.Control, function (mod) {
                    _this.reload(_this.Setup.Page);
                });
            }
            if (!_this.Setup.StaticData)
                _this.LoadingDiv = $YetaWF.getElement1BySelectorCond(".tg_loading", [_this.Control]);
            // Nav buttons
            if (_this.BtnTop) {
                $YetaWF.registerEventHandler(_this.BtnTop, "click", null, function (ev) {
                    if (!$YetaWF.elementHasClass(ev.__YetaWFElem, _this.Setup.DisabledCss)) {
                        if (_this.Setup.Page >= 0)
                            _this.reload(0);
                    }
                    return false;
                });
            }
            if (_this.BtnPrev) {
                $YetaWF.registerEventHandler(_this.BtnPrev, "click", null, function (ev) {
                    if (!$YetaWF.elementHasClass(ev.__YetaWFElem, _this.Setup.DisabledCss)) {
                        var page = _this.Setup.Page - 1;
                        if (page >= 0)
                            _this.reload(page);
                    }
                    return false;
                });
            }
            if (_this.BtnNext) {
                $YetaWF.registerEventHandler(_this.BtnNext, "click", null, function (ev) {
                    if (!$YetaWF.elementHasClass(ev.__YetaWFElem, _this.Setup.DisabledCss)) {
                        var page = _this.Setup.Page + 1;
                        if (page < _this.Setup.Pages)
                            _this.reload(page);
                    }
                    return false;
                });
            }
            if (_this.BtnBottom) {
                $YetaWF.registerEventHandler(_this.BtnBottom, "click", null, function (ev) {
                    if (!$YetaWF.elementHasClass(ev.__YetaWFElem, _this.Setup.DisabledCss)) {
                        var page = _this.Setup.Pages - 1;
                        if (page >= 0)
                            _this.reload(page);
                    }
                    return false;
                });
            }
            // Page input
            if (_this.InputPage) {
                $YetaWF.registerEventHandler(_this.InputPage.Control, "keydown", null, function (ev) {
                    if (ev.keyCode === 13 && _this.InputPage) { // Return
                        var page = _this.InputPage.value - 1;
                        _this.reload(page);
                        return false;
                    }
                    return true;
                });
            }
            // pagesize selection
            if (_this.SelectPageSize) {
                _this.SelectPageSize.Control.addEventListener(YetaWF_ComponentsHTML.DropDownListEditComponent.EVENTCHANGE, function (evt) {
                    if (_this.SelectPageSize)
                        _this.reload(0, Number(_this.SelectPageSize.value));
                });
            }
            // Column resizing
            $YetaWF.registerEventHandler(_this.Control, "mousedown", ".tg_resize", function (ev) {
                if (!_this.reloadInProgress) {
                    Grid.CurrentControl = _this;
                    _this.ColumnResizeBar = ev.__YetaWFElem;
                    _this.ColumnResizeHeader = $YetaWF.elementClosest(_this.ColumnResizeBar, "th");
                    document.body.style.cursor = "col-resize";
                    window.addEventListener("mousemove", Grid.resizeColumn, false);
                    window.addEventListener("mouseup", Grid.resizeColumnDone, false);
                }
                return false;
            });
            // Sorting
            if (_this.Setup.CanSort) {
                $YetaWF.registerEventHandler(_this.Control, "click", ".tg_header th", function (ev) {
                    if (!_this.reloadInProgress) {
                        var colIndex = Array.prototype.indexOf.call(ev.__YetaWFElem.parentElement.children, ev.__YetaWFElem);
                        if (colIndex < 0 || colIndex >= _this.Setup.Columns.length)
                            throw "Invalid column index " + colIndex + " - max is " + _this.Setup.Columns.length; /*DEBUG*/
                        var col = _this.Setup.Columns[colIndex];
                        if (col.Sortable) {
                            if (col.Sort === SortByEnum.NotSpecified) {
                                _this.clearSorts();
                                _this.setSortOrder(col, colIndex, SortByEnum.Ascending);
                            }
                            else if (col.Sort === SortByEnum.Descending) {
                                _this.setSortOrder(col, colIndex, SortByEnum.Ascending);
                            }
                            else {
                                _this.setSortOrder(col, colIndex, SortByEnum.Descending);
                            }
                            _this.reload(0, undefined, undefined, undefined, true);
                        }
                    }
                    return false;
                });
            }
            // Filtering
            if (_this.Setup.CanFilter && _this.FilterBar) {
                $YetaWF.registerEventHandler(_this.FilterBar, "click", ".tg_fmenu", function (ev) {
                    var button = ev.__YetaWFElem;
                    var filter = $YetaWF.elementClosest(button, ".tg_filter");
                    var head = $YetaWF.elementClosest(button, "th");
                    var colIndex = Array.prototype.indexOf.call(filter.children, head);
                    var ulElem = $YetaWF.getElementById(_this.Setup.Columns[colIndex].MenuId);
                    if (!YetaWF_ComponentsHTML.MenuULComponent.closeMenus()) {
                        var menuDiv = ulElem.cloneNode(true);
                        menuDiv.id = ulElem.id + "_live";
                        $YetaWF.elementAddClass(menuDiv, "yt_grid_menu");
                        document.body.appendChild(menuDiv);
                        new YetaWF_ComponentsHTML.MenuULComponent(menuDiv.id, {
                            "AutoOpen": true, "AutoRemove": true, "AttachTo": button, "Dynamic": true,
                            "Click": function (liElem) {
                                _this.menuSelected(liElem, colIndex);
                            },
                        });
                    }
                    return false;
                });
                $YetaWF.registerEventHandler(_this.FilterBar, "mousedown", ".tg_fmenu", function (ev) {
                    return false;
                });
                $YetaWF.registerEventHandler(_this.FilterBar, "click", ".tg_fclear", function (ev) {
                    var filter = $YetaWF.elementClosest(ev.__YetaWFElem, ".tg_filter");
                    var head = $YetaWF.elementClosest(ev.__YetaWFElem, "th");
                    var colIndex = Array.prototype.indexOf.call(filter.children, head);
                    _this.clearColSortValue(colIndex);
                    _this.reload(0);
                    return false;
                });
                _this.addDirectFilterHandlers();
            }
            // Delete action (static only)
            if (_this.Setup.StaticData) {
                $YetaWF.registerEventHandler(_this.Control, "click", "[name='DeleteAction']", function (ev) {
                    if (!_this.Setup.StaticData)
                        return true;
                    // find the record number to delete
                    var trElem = $YetaWF.elementClosest(ev.__YetaWFElem, "tr");
                    var recNum = Number($YetaWF.getAttribute(trElem, "data-origin"));
                    var message = _this.Setup.DeleteConfirmationMessage;
                    var colName = _this.Setup.DeletedColumnDisplay;
                    if (message) {
                        if (colName) {
                            var text = _this.Setup.StaticData[recNum][colName];
                            message = message.format(text);
                        }
                        $YetaWF.alertYesNo(message, undefined, function () {
                            _this.removeRecord(trElem, recNum, colName);
                        });
                    }
                    else
                        _this.removeRecord(trElem, recNum, colName);
                    return false;
                });
            }
            // Selection
            $YetaWF.registerEventHandler(_this.TBody, "mousedown", "tr:not(.tg_emptytr)", function (ev) {
                return _this.handleSelect(ev.__YetaWFElem, false);
            });
            $YetaWF.registerEventHandler(_this.TBody, "dblclick", "tr:not(.tg_emptytr)", function (ev) {
                return _this.handleSelect(ev.__YetaWFElem, true);
            });
            $YetaWF.registerEventHandler(_this.TBody, "focusin", "tr:not(.tg_emptytr)", function (ev) {
                var elem = ev.__YetaWFElem;
                $YetaWF.elementToggleClass(elem, _this.Setup.RowHighlightCss, true);
                return true;
            });
            $YetaWF.registerEventHandler(_this.TBody, "focusout", "tr:not(.tg_emptytr)", function (ev) {
                var elem = ev.__YetaWFElem;
                $YetaWF.elementToggleClass(elem, _this.Setup.RowHighlightCss, false);
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "keydown", null, function (ev) {
                if (!document.activeElement || document.activeElement.tagName !== "TR")
                    return true;
                if (_this.Setup.HighlightOnClick) {
                    var key = ev.key;
                    if (key === "ArrowDown" || key === "Down") {
                        var index = _this.SelectedIndex();
                        _this.SetSelectedIndex(index < 0 ? 0 : ++index);
                        index = _this.SelectedIndex();
                        if (index >= 0)
                            _this.GetTR(index).focus();
                        _this.sendEventSelect();
                        return false;
                    }
                    else if (key === "ArrowUp" || key === "Up") {
                        var index = _this.SelectedIndex();
                        _this.SetSelectedIndex(index < 0 ? _this.GetTotalRecords() - 1 : --index);
                        index = _this.SelectedIndex();
                        if (index >= 0)
                            _this.GetTR(index).focus();
                        _this.sendEventSelect();
                        return false;
                    }
                    else if (key === "Home") {
                        _this.SetSelectedIndex(0);
                        var index = _this.SelectedIndex();
                        if (index >= 0)
                            _this.GetTR(index).focus();
                        _this.sendEventSelect();
                        return false;
                    }
                    else if (key === "End") {
                        _this.SetSelectedIndex(_this.GetTotalRecords() - 1);
                        var index = _this.SelectedIndex();
                        if (index >= 0)
                            _this.GetTR(index).focus();
                        _this.sendEventSelect();
                        return false;
                    }
                }
                return true;
            });
            // Drag & drop
            $YetaWF.registerEventHandlerBody("mousemove", null, function (ev) {
                if (_this.reorderingInProgress) {
                    //console.log("Reordering...")
                    var rect = _this.TBody.getBoundingClientRect();
                    if (ev.clientX < rect.left || ev.clientX > rect.left + rect.width ||
                        ev.clientY < rect.top || ev.clientY > rect.top + rect.height) {
                        _this.cancelDragDrop();
                        return true;
                    }
                    var sel = _this.SelectedIndex();
                    if (sel < 0) {
                        _this.cancelDragDrop();
                        return true;
                    }
                    var insert = _this.HitTestInsert(ev.clientX, ev.clientY);
                    //console.log(`insert = ${insert}  sel = ${sel}`);
                    if (insert === sel || insert === sel + 1)
                        return true; // nothing to move
                    _this.moveRawRecord(sel, insert);
                }
                return true;
            });
            $YetaWF.registerEventHandler(_this.TBody, "mouseup", null, function (ev) {
                if (_this.reorderingInProgress) {
                    _this.doneDragDrop();
                }
                return true;
            });
            // OnlySubmitWhenChecked
            if (_this.Setup.StaticData && _this.Setup.NoSubmitContents) {
                _this.SubmitCheckCol = _this.getSubmitCheckCol();
                if (_this.SubmitCheckCol >= 0) {
                    _this.setInitialSubmitStatus();
                    // update static data with new checkbox value
                    $YetaWF.registerEventHandler(_this.TBody, "change", "tr td:nth-child(" + (_this.SubmitCheckCol + 1) + ") input[type='checkbox']", function (ev) {
                        if (!_this.Setup.StaticData)
                            return true;
                        var tr = $YetaWF.elementClosest(ev.__YetaWFElem, "tr");
                        var recNum = Number($YetaWF.getAttribute(tr, "data-origin"));
                        var val = ev.__YetaWFElem.checked;
                        _this.Setup.StaticData[recNum][_this.Setup.Columns[_this.SubmitCheckCol].Name] = val;
                        //$YetaWF.elementToggleClass(tr, YConfigs.Forms.CssFormNoSubmitContents, !val);
                        return false;
                    });
                }
                // handle submit local data
                $YetaWF.Forms.addPreSubmitHandler(true, {
                    form: $YetaWF.Forms.getForm(_this.Control),
                    callback: function (entry) {
                        _this.submitLocalData(entry);
                    },
                    userdata: _this
                });
            }
            return _this;
        }
        Grid.prototype.sendEventDblClick = function () {
            $YetaWF.sendCustomEvent(this.Control, Grid.EVENTDBLCLICK);
        };
        Grid.prototype.sendEventSelect = function () {
            $YetaWF.sendCustomEvent(this.Control, Grid.EVENTSELECT);
        };
        Grid.prototype.sendEventDragDropDone = function () {
            $YetaWF.sendCustomEvent(this.Control, Grid.EVENTDRAGDROPDONE);
        };
        Grid.prototype.sendEventDragDropCancel = function () {
            $YetaWF.sendCustomEvent(this.Control, Grid.EVENTDRAGDROPCANCEL);
        };
        // selection
        Grid.prototype.handleSelect = function (clickedElem, doubleClick) {
            if (this.Setup.HighlightOnClick) {
                if (clickedElem.parentElement !== this.TBody) {
                    // something in a row was clicked (nested grid), find the real row
                    for (;;) {
                        if (clickedElem.parentElement == null)
                            return true;
                        clickedElem = clickedElem.parentElement;
                        if (clickedElem.tagName === "TR" && clickedElem.parentElement === this.TBody)
                            break;
                    }
                }
                if ($YetaWF.elementHasClass(clickedElem, this.Setup.RowHighlightCss)) {
                    if (this.Setup.CanReorder && this.Setup.StaticData && this.Setup.StaticData.length > 1) {
                        // reordering
                        this.reorderingRowElement = clickedElem;
                        this.reorderingInProgress = true;
                        //console.log("Reordering starting");
                        $YetaWF.elementToggleClass(this.reorderingRowElement, this.Setup.RowHighlightCss, false);
                        $YetaWF.elementToggleClass(this.reorderingRowElement, this.Setup.RowDragDropHighlightCss, true);
                        return false;
                    }
                    if (!doubleClick)
                        return true;
                }
                else {
                    var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
                    for (var _i = 0, trs_1 = trs; _i < trs_1.length; _i++) {
                        var tr = trs_1[_i];
                        $YetaWF.elementToggleClass(tr, this.Setup.RowHighlightCss, false);
                    }
                    $YetaWF.elementToggleClass(clickedElem, this.Setup.RowHighlightCss, true);
                }
                if (doubleClick)
                    this.sendEventDblClick();
                else
                    this.sendEventSelect();
                return true;
            }
            return true;
        };
        // Drag&drop
        Grid.prototype.cancelDragDrop = function () {
            if (this.reorderingRowElement) {
                $YetaWF.elementToggleClass(this.reorderingRowElement, this.Setup.RowHighlightCss, true);
                $YetaWF.elementToggleClass(this.reorderingRowElement, this.Setup.RowDragDropHighlightCss, false);
                this.reorderingRowElement = null;
            }
            this.reorderingInProgress = false;
            //console.log("Reordering canceled - left boundary")
            this.sendEventDragDropCancel();
        };
        Grid.prototype.doneDragDrop = function () {
            if (this.reorderingRowElement) {
                $YetaWF.elementToggleClass(this.reorderingRowElement, this.Setup.RowHighlightCss, true);
                $YetaWF.elementToggleClass(this.reorderingRowElement, this.Setup.RowDragDropHighlightCss, false);
                this.reorderingRowElement = null;
            }
            this.reorderingInProgress = false;
            //console.log("Reordering ended")
            this.sendEventDragDropDone();
        };
        // OnlySubmitWhenChecked
        Grid.prototype.setInitialSubmitStatus = function () {
            if (!this.Setup.StaticData || !this.Setup.NoSubmitContents)
                return;
            //let trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
            //for (let tr of trs) {
            //    let recNum = Number($YetaWF.getAttribute(tr, "data-origin"));
            //    let val = this.Setup.StaticData[recNum][this.Setup.Columns[this.SubmitCheckCol].Name];
            //    $YetaWF.elementToggleClass(tr, YConfigs.Forms.CssFormNoSubmitContents, !val);
            //}
        };
        Grid.prototype.getSubmitCheckCol = function () {
            var colIndex = -1;
            var cols = this.Setup.Columns.filter(function (col, index, cols) {
                if (!col.OnlySubmitWhenChecked)
                    return false;
                colIndex = index;
                return true;
            });
            if (cols.length > 1)
                throw "More than one column marked OnlySubmitWhenChecked";
            return colIndex;
        };
        Grid.prototype.submitLocalData = function (entry) {
            if (!this.Setup.StaticData)
                return;
            var div = "<div class='" + $YetaWF.Forms.DATACLASS + "' style='display:none'>";
            // retrieve all rows and add input/select fields to data div, resequence to make mvc serialization of lists work
            var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
            var row = 0;
            var re1 = new RegExp("\\[[0-9]+\\]", "gim");
            for (var _i = 0, trs_2 = trs; _i < trs_2.length; _i++) {
                var tr = trs_2[_i];
                var recNum = Number($YetaWF.getAttribute(tr, "data-origin"));
                var val = this.Setup.StaticData[recNum][this.Setup.Columns[this.SubmitCheckCol].Name];
                if (val) { // add record if the checkbox is selected
                    var copied = false;
                    var inputs = $YetaWF.getElementsBySelector("input,select", [tr]);
                    for (var _a = 0, inputs_1 = inputs; _a < inputs_1.length; _a++) {
                        var input = inputs_1[_a];
                        var name_1 = $YetaWF.getAttributeCond(input, "name");
                        if (name_1) {
                            var copy = input.cloneNode();
                            // replace name with serialized name[row] so mvc serialization works
                            name_1 = name_1.replace(re1, "[" + row.toString() + "]");
                            $YetaWF.setAttribute(copy, "name", name_1);
                            div += copy.outerHTML;
                            copied = true;
                        }
                    }
                    if (copied)
                        ++row;
                }
            }
            div += "</div>";
            if (row > 0)
                entry.form.insertAdjacentHTML("beforeend", div);
        };
        // sorting
        Grid.prototype.clearSorts = function () {
            var colIndex = 0;
            for (var _i = 0, _a = this.Setup.Columns; _i < _a.length; _i++) {
                var col = _a[_i];
                if (col.Sortable) {
                    col.Sort = SortByEnum.NotSpecified;
                    this.setSortOrder(col, colIndex, SortByEnum.NotSpecified);
                }
                ++colIndex;
            }
        };
        Grid.prototype.setSortOrder = function (col, colIndex, sortBy) {
            for (var _i = 0, _a = this.Setup.Columns; _i < _a.length; _i++) {
                var col_1 = _a[_i];
                if (col_1.Sortable)
                    col_1.Sort = SortByEnum.NotSpecified;
            }
            col.Sort = sortBy;
            // turn indicators in header on or off
            var ths = $YetaWF.getElementsBySelector(".tg_header th", [this.Control]);
            var th = ths[colIndex];
            var asc = $YetaWF.getElement1BySelector(".tg_sorticon .tg_sortasc", [th]);
            var desc = $YetaWF.getElement1BySelector(".tg_sorticon .tg_sortdesc", [th]);
            var both = $YetaWF.getElement1BySelector(".tg_sorticon .tg_sortboth", [th]);
            $YetaWF.elementToggleClass(asc, this.Setup.SortActiveCss, sortBy === SortByEnum.Ascending);
            $YetaWF.elementToggleClass(desc, this.Setup.SortActiveCss, sortBy === SortByEnum.Descending);
            $YetaWF.elementToggleClass(both, this.Setup.SortActiveCss, sortBy === SortByEnum.NotSpecified);
        };
        Grid.prototype.getSortColumn = function () {
            for (var _i = 0, _a = this.Setup.Columns; _i < _a.length; _i++) {
                var col = _a[_i];
                if (col.Sortable && col.Sort !== SortByEnum.NotSpecified)
                    return col;
            }
            return null;
        };
        // Resizing
        // Convert all ch units to pixels in column headers
        Grid.prototype.convertToPix = function () {
            var avgChar = this.calcCharWidth();
            var ths = $YetaWF.getElementsBySelector(".tg_header th", [this.Control]);
            for (var _i = 0, ths_1 = ths; _i < ths_1.length; _i++) {
                var th = ths_1[_i];
                var wstyle = th.style.width;
                if (wstyle.endsWith("ch")) {
                    var w = parseFloat(wstyle) + 2; // we'll add some for padding
                    w *= avgChar;
                    th.style.width = w + "px";
                }
            }
            if (this.Setup.SizeStyle === SizeStyleEnum.SizeGiven) {
                var total = 0;
                for (var _a = 0, ths_2 = ths; _a < ths_2.length; _a++) {
                    var th = ths_2[_a];
                    var w = parseFloat(th.style.width);
                    total += w;
                }
                var table = $YetaWF.getElement1BySelector("table", [this.Control]);
                table.style.width = total + "px";
            }
        };
        Grid.prototype.calcCharWidth = function () {
            var text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var elem = $YetaWF.createElement("div", { style: "position:absolute;visibility:hidden;white-space:nowrap" }, text);
            // copy font settings
            var td = $YetaWF.getElement1BySelector(".tg_table table td", [this.Control]); // there is always a td element, even if it's empty
            var style = window.getComputedStyle(td);
            elem.style.font = style.font;
            elem.style.fontStyle = style.fontStyle;
            elem.style.fontWeight = style.fontWeight;
            elem.style.fontSize = style.fontSize;
            document.body.appendChild(elem);
            var width = elem.clientWidth / text.length;
            elem.remove();
            return width;
        };
        Grid.resizeColumn = function (ev) {
            var currentControl = Grid.CurrentControl;
            if (currentControl && currentControl.ColumnResizeHeader) {
                var rect = currentControl.ColumnResizeHeader.getBoundingClientRect();
                var actualWidth = rect.width;
                var newActualWidth = ev.clientX - rect.left;
                var givenWidth = Number(currentControl.ColumnResizeHeader.style.width.replace("px", ""));
                var diff = newActualWidth - actualWidth; // <0 shring, >0 expand
                var newGivenWidth = givenWidth + diff;
                currentControl.ColumnResizeHeader.style.width = newGivenWidth + "px";
            }
            return false;
        };
        Grid.resizeColumnDone = function (ev) {
            var currentControl = Grid.CurrentControl;
            if (currentControl && currentControl.ColumnResizeBar && currentControl.ColumnResizeHeader && currentControl.Setup.SaveSettingsColumnWidthsUrl) {
                document.body.style.cursor = "default";
                window.removeEventListener("mousemove", this.resizeColumn, false);
                window.removeEventListener("mouseup", this.resizeColumnDone, false);
                // save column widths after user resizes
                if (currentControl.Setup.SettingsModuleGuid) {
                    // send save request, we don't care about the response
                    var uri = $YetaWF.parseUrl(currentControl.Setup.SaveSettingsColumnWidthsUrl);
                    uri.addSearch("SettingsModuleGuid", currentControl.Setup.SettingsModuleGuid);
                    var colIndex = Array.prototype.indexOf.call(currentControl.ColumnResizeHeader.parentElement.children, currentControl.ColumnResizeHeader);
                    uri.addSearch("Columns[0].Key", currentControl.Setup.Columns[colIndex].Name);
                    uri.addSearch("Columns[0].Value", parseInt(currentControl.ColumnResizeHeader.style.width, 10));
                    var request = new XMLHttpRequest();
                    request.open("POST", currentControl.Setup.SaveSettingsColumnWidthsUrl, true);
                    request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
                    //request.overrideMimeType("application/text");// would help firefox understand this isn't xml, but it's not standard, oh well
                    request.send(uri.toFormData());
                }
            }
            if (Grid.CurrentControl) {
                Grid.CurrentControl.ColumnResizeBar = null;
                Grid.CurrentControl.ColumnResizeHeader = null;
            }
            Grid.CurrentControl = null;
            ev.preventDefault();
            ev.stopPropagation();
            return false;
        };
        // reloading
        Grid.prototype.reload = function (page, newPageSize, overrideColFilter, overrideExtraData, sort, done) {
            var _this = this;
            if (!this.reloadInProgress) {
                this.setReloading(true);
                if (page < 0)
                    page = 0;
                if (this.Setup.StaticData && !sort) {
                    // show/hide selected rows
                    if (this.Setup.PageSize > 0) {
                        var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
                        for (var _i = 0, trs_3 = trs; _i < trs_3.length; _i++) {
                            var tr = trs_3[_i];
                            tr.setAttribute("style", "display:none");
                        }
                        var len = trs.length;
                        var count = 0;
                        for (var i = page * this.Setup.PageSize; i < len; ++i) {
                            if (count >= (newPageSize || this.Setup.PageSize))
                                break;
                            trs[i].removeAttribute("style");
                            // init any controls that just became visible
                            $YetaWF.sendActivateDivEvent([trs[i]]);
                            ++count;
                        }
                    }
                    this.Setup.Page = page;
                    if (this.InputPage)
                        this.InputPage.value = this.Setup.Page + 1;
                    this.updateStatus();
                    this.setReloading(false);
                }
                else {
                    // fetch data from servers
                    var uri = $YetaWF.parseUrl(this.Setup.AjaxUrl);
                    uri.addSearch("fieldPrefix", this.Setup.FieldName);
                    uri.addSearch("skip", page * this.Setup.PageSize);
                    uri.addSearch("take", newPageSize || this.Setup.PageSize);
                    if (this.Setup.ExtraData)
                        uri.addSearchSimpleObject(this.Setup.ExtraData);
                    // sort order
                    var col = this.getSortColumn();
                    if (col) {
                        uri.addSearch("sort[0].field", col.Name);
                        uri.addSearch("sort[0].order", (col.Sort === SortByEnum.Descending ? 1 : 0));
                        // also add as "sorts" for controllers that prefer this name
                        uri.addSearch("sorts[0].field", col.Name);
                        uri.addSearch("sorts[0].order", (col.Sort === SortByEnum.Descending ? 1 : 0));
                    }
                    // filters
                    var colIndex = 0;
                    var fcount = 0;
                    for (var _a = 0, _b = this.Setup.Columns; _a < _b.length; _a++) {
                        var col_2 = _b[_a];
                        var val = this.getColSortValue(colIndex);
                        if (val) {
                            if (col_2.FilterType === "complex") {
                                uri.addSearch("filters[" + fcount + "].field", col_2.Name);
                                uri.addSearch("filters[" + fcount + "].operator", "Complex");
                                uri.addSearch("filters[" + fcount + "].valueAsString", val);
                                ++fcount;
                            }
                            else {
                                var oper = col_2.FilterOp;
                                if (overrideColFilter && overrideColFilter.ColIndex === colIndex)
                                    oper = overrideColFilter.FilterOp;
                                if (oper != null) {
                                    uri.addSearch("filters[" + fcount + "].field", col_2.Name);
                                    uri.addSearch("filters[" + fcount + "].operator", this.GetFilterOpString(oper));
                                    uri.addSearch("filters[" + fcount + "].valueAsString", val);
                                    ++fcount;
                                }
                            }
                        }
                        ++colIndex;
                    }
                    uri.addFormInfo(this.Control);
                    var uniqueIdCounters = { UniqueIdPrefix: this.ControlId + "gr", UniqueIdPrefixCounter: 0, UniqueIdCounter: 0 };
                    uri.addSearch(YConfigs.Forms.UniqueIdCounters, JSON.stringify(uniqueIdCounters));
                    if (this.Setup.StaticData)
                        uri.addSearch("data", JSON.stringify(this.Setup.StaticData));
                    var request_1 = new XMLHttpRequest();
                    request_1.open("POST", this.Setup.AjaxUrl);
                    request_1.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    request_1.setRequestHeader("X-Requested-With", "XMLHttpRequest");
                    request_1.onreadystatechange = function (ev) {
                        if (request_1.readyState === 4 /*DONE*/) {
                            _this.setReloading(false);
                            $YetaWF.processAjaxReturn(request_1.responseText, request_1.statusText, request_1, undefined, undefined, function (result) {
                                var partial = JSON.parse(request_1.responseText);
                                $YetaWF.processClearDiv(_this.TBody);
                                _this.TBody.innerHTML = "";
                                $YetaWF.appendMixedHTML(_this.TBody, partial.TBody, true);
                                _this.Setup.Records = partial.Records;
                                _this.Setup.Pages = partial.Pages;
                                _this.Setup.Page = partial.Page;
                                _this.Setup.PageSize = partial.PageSize;
                                if (_this.InputPage)
                                    _this.InputPage.value = _this.Setup.Page + 1;
                                if (_this.Setup.NoSubmitContents) {
                                    _this.SubmitCheckCol = _this.getSubmitCheckCol();
                                    if (_this.SubmitCheckCol >= 0)
                                        _this.setInitialSubmitStatus();
                                }
                                _this.updateStatus();
                                if (done)
                                    done();
                                _this.sendEventSelect();
                            });
                        }
                    };
                    var data = uri.toFormData();
                    request_1.send(data);
                }
            }
        };
        Grid.prototype.setReloading = function (on) {
            this.reloadInProgress = on;
            $YetaWF.setLoading(on);
            if (this.LoadingDiv) {
                if (on)
                    this.LoadingDiv.setAttribute("style", "");
                else
                    this.LoadingDiv.setAttribute("style", "display:none");
            }
        };
        Grid.prototype.updateStatus = function () {
            if (this.PagerTotals) {
                var totals = void 0;
                if (this.Setup.Records === 0)
                    totals = YLocs.YetaWF_ComponentsHTML.GridTotalNone;
                else {
                    if (this.Setup.PageSize === 0) {
                        var first = 1;
                        var last = this.Setup.Records;
                        if (first > last)
                            totals = YLocs.YetaWF_ComponentsHTML.GridTotal0.format(this.Setup.Records);
                        else
                            totals = YLocs.YetaWF_ComponentsHTML.GridTotals.format(first, last, this.Setup.Records);
                    }
                    else {
                        var first = this.Setup.Page * this.Setup.PageSize + 1;
                        var last = first + this.Setup.PageSize - 1;
                        if (first > last)
                            totals = YLocs.YetaWF_ComponentsHTML.GridTotal0.format(this.Setup.Records);
                        else {
                            first = Math.max(0, first);
                            last = Math.min(last, this.Setup.Records);
                            totals = YLocs.YetaWF_ComponentsHTML.GridTotals.format(first, last, this.Setup.Records);
                        }
                    }
                }
                this.PagerTotals.innerHTML = "<span>" + totals + "</span>";
            }
            if (this.BtnTop)
                $YetaWF.elementToggleClass(this.BtnTop, this.Setup.DisabledCss, this.Setup.Page <= 0);
            if (this.BtnPrev)
                $YetaWF.elementToggleClass(this.BtnPrev, this.Setup.DisabledCss, this.Setup.Page <= 0);
            if (this.BtnNext)
                $YetaWF.elementToggleClass(this.BtnNext, this.Setup.DisabledCss, this.Setup.Page >= this.Setup.Pages - 1);
            if (this.BtnBottom)
                $YetaWF.elementToggleClass(this.BtnBottom, this.Setup.DisabledCss, this.Setup.Page >= this.Setup.Pages - 1);
            // show/hide "No Records"
            if (this.Setup.StaticData) {
                if (this.Setup.Records === 0) {
                    $YetaWF.getElement1BySelector("tr.tg_emptytr", [this.TBody]).style.display = "";
                }
                else {
                    $YetaWF.getElement1BySelector("tr.tg_emptytr", [this.TBody]).style.display = "none";
                }
            }
            // show hide filter clear buttons
            if (this.Setup.CanFilter && this.FilterBar) {
                var colIndex = 0;
                for (var _i = 0, _a = this.Setup.Columns; _i < _a.length; _i++) {
                    var col = _a[_i];
                    if (col.FilterId) {
                        var val = this.getColSortValue(colIndex);
                        var elem = $YetaWF.getElementById(col.FilterId); // the value element
                        var fentry = $YetaWF.elementClosest(elem, ".tg_fentry"); // the container for this filter
                        var btn = $YetaWF.getElement1BySelectorCond(".tg_fclear", [fentry]); // clear button
                        if (btn) {
                            if (val)
                                btn.style.display = "";
                            else
                                btn.style.display = "none";
                        }
                    }
                    ++colIndex;
                }
            }
        };
        Grid.prototype.updatePage = function () {
            if (this.Setup.PageSize > 0)
                this.Setup.Pages = Math.max(1, Math.floor((this.Setup.Records - 1) / this.Setup.PageSize) + 1);
        };
        // Filtering
        Grid.prototype.clearFilters = function () {
            var colIndex = 0;
            for (var _i = 0, _a = this.Setup.Columns; _i < _a.length; _i++) {
                var _b = _a[_i];
                this.clearColSortValue(colIndex);
                ++colIndex;
            }
            this.updateStatus();
        };
        Grid.prototype.clearFilterMenuHighlights = function (ulElem) {
            var menuLis = $YetaWF.getElementsBySelector("li." + this.Setup.HighlightCss, [ulElem]);
            for (var _i = 0, menuLis_1 = menuLis; _i < menuLis_1.length; _i++) {
                var menuLi = menuLis_1[_i];
                $YetaWF.elementRemoveClass(menuLi, this.Setup.HighlightCss);
            }
        };
        Grid.prototype.menuSelected = function (menuElem, colIndex) {
            var _this = this;
            // update column structure
            var sel = Number($YetaWF.getAttribute(menuElem, "data-sel"));
            // new filter
            var overrideColFilter = {
                ColIndex: colIndex,
                FilterOp: sel
            };
            this.reload(0, undefined, overrideColFilter, undefined, undefined, function () {
                // clear all highlights
                var ulElem = $YetaWF.elementClosest(menuElem, "ul");
                _this.clearFilterMenuHighlights(ulElem);
                // highlight new selection
                $YetaWF.elementToggleClass(menuElem, _this.Setup.HighlightCss, true);
                // update button with new sort icon
                if (_this.FilterBar) {
                    var icon = $YetaWF.getElement1BySelector(".t_fmenuicon", [menuElem]).innerHTML;
                    var thsFilter = $YetaWF.getElementsBySelector("th", [_this.FilterBar]);
                    var btn = $YetaWF.getElement1BySelector(".tg_fmenu", [thsFilter[colIndex]]);
                    btn.innerHTML = icon;
                }
                // update column structures
                _this.Setup.Columns[colIndex].FilterOp = sel;
            });
        };
        Grid.prototype.addDirectFilterHandlers = function () {
            var _this = this;
            var _loop_1 = function (col) {
                switch (col.FilterType) {
                    default:
                        break;
                    case "bool":
                    case "enum":
                    case "dynenum":
                        // handle selection change
                        $YetaWF.registerCustomEventHandlerDocument(YetaWF_ComponentsHTML.DropDownListEditComponent.EVENTCHANGE, "#" + col.FilterId, function (ev) {
                            _this.reload(0);
                            return false;
                        });
                        break;
                    case "long":
                    case "decimal":
                    case "datetime":
                    case "date":
                    case "text":
                    case "guid": {
                        // handle return key
                        var elem = $YetaWF.getElementById(col.FilterId);
                        $YetaWF.registerEventHandler(elem, "keydown", null, function (ev) {
                            if (ev.keyCode === 13) { // Return
                                _this.reload(0);
                                return false;
                            }
                            return true;
                        });
                        break;
                    }
                    case "complex": {
                        // handle invoking popup
                        var elem_1 = $YetaWF.getElementById(col.FilterId); // the value element
                        var fctrls = $YetaWF.elementClosest(elem_1, ".tg_fctrls"); // the container for this filter
                        var ffilter = $YetaWF.getElement1BySelector(".tg_ffilter", [fctrls]); // the filter button
                        var urlElem_1 = $YetaWF.getElement1BySelector("input[name='Url']", [fctrls]);
                        $YetaWF.registerEventHandler(ffilter, "click", null, function (ev) {
                            var url = urlElem_1.value;
                            // invoke popup passing the data and filterid as arguments
                            var uri = new YetaWF.Url();
                            uri.parse(url);
                            uri.removeSearch("FilterId");
                            uri.addSearch("FilterId", col.FilterId);
                            uri.removeSearch("Data");
                            uri.addSearch("Data", elem_1.value);
                            if ($YetaWF.Popups.openPopup(uri.toUrl(), false, true))
                                return false;
                            return true;
                        });
                        break;
                    }
                }
            };
            for (var _i = 0, _a = this.Setup.Columns; _i < _a.length; _i++) {
                var col = _a[_i];
                _loop_1(col);
            }
        };
        Grid.updateComplexFilter = function (filterId, data) {
            var elem = $YetaWF.getElementById(filterId); // the value element
            if (data) {
                var fctrls = $YetaWF.elementClosest(elem, ".tg_fctrls"); // the container for this filter
                var uiHint = $YetaWF.getElement1BySelector("input[name='UIHint']", [fctrls]); // uihint name
                data.UIHint = uiHint.value; // set the uiHint
                elem.value = JSON.stringify(data);
            }
            else
                elem.value = "";
            var grid = YetaWF.ComponentBaseDataImpl.getControlFromTag(elem, YetaWF_ComponentsHTML.Grid.SELECTOR);
            grid.reload(0);
        };
        Grid.prototype.getColSortValue = function (colIndex) {
            var col = this.Setup.Columns[colIndex];
            switch (col.FilterType) {
                case null:
                    return null;
                case "bool": {
                    var dd = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR);
                    var boolVal = Number(dd.value);
                    switch (boolVal) {
                        default:
                        case FilterBoolEnum.All: return null;
                        case FilterBoolEnum.Yes: return "True";
                        case FilterBoolEnum.No: return "False";
                    }
                }
                case "long":
                case "text":
                case "guid": {
                    var edit = $YetaWF.getElementById(col.FilterId);
                    return edit.value;
                }
                case "dynenum": {
                    var dd = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR);
                    if (dd.value === "-1")
                        return null;
                    return dd.value;
                }
                case "decimal":
                    var dec = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, YetaWF_ComponentsHTML.DecimalEditComponent.SELECTOR);
                    return dec.valueText;
                case "datetime":
                    var datetime = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, YetaWF_ComponentsHTML.DateTimeEditComponent.SELECTOR);
                    return datetime.valueText;
                case "date":
                    var date = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, YetaWF_ComponentsHTML.DateEditComponent.SELECTOR);
                    return date.valueText;
                case "enum": {
                    var dd = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR);
                    if (dd.value === "-1")
                        return null;
                    return dd.value;
                }
                case "complex": {
                    var edit = $YetaWF.getElementById(col.FilterId);
                    return edit.value;
                }
                default:
                    throw "Unexpected filter type " + col.FilterType + " for column " + colIndex;
            }
        };
        Grid.prototype.clearColSortValue = function (colIndex) {
            var col = this.Setup.Columns[colIndex];
            switch (col.FilterType) {
                case null:
                    break;
                case "bool": {
                    var dd = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR);
                    dd.clear();
                    break;
                }
                case "long":
                case "text":
                case "guid": {
                    var edit = $YetaWF.getElementById(col.FilterId);
                    edit.value = "";
                    break;
                }
                case "dynenum": {
                    var dd = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR);
                    dd.value = "-1";
                    break;
                }
                case "decimal":
                    var dec = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, YetaWF_ComponentsHTML.DecimalEditComponent.SELECTOR);
                    dec.clear();
                    break;
                case "datetime":
                    var datetime = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, YetaWF_ComponentsHTML.DateTimeEditComponent.SELECTOR);
                    datetime.clear();
                    break;
                case "date":
                    var date = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, YetaWF_ComponentsHTML.DateEditComponent.SELECTOR);
                    date.clear();
                    break;
                case "enum": {
                    var dd = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR);
                    dd.value = "-1";
                    break;
                }
                case "complex": {
                    var edit = $YetaWF.getElementById(col.FilterId);
                    edit.value = "";
                    break;
                }
                default:
                    throw "Unexpected filter type " + col.FilterType + " for column " + colIndex;
            }
        };
        Grid.prototype.GetFilterOpString = function (op) {
            switch (op) {
                case FilterOptionEnum.Equal: return "==";
                case FilterOptionEnum.GreaterEqual: return ">=";
                case FilterOptionEnum.GreaterThan: return ">";
                case FilterOptionEnum.LessEqual: return "<=";
                case FilterOptionEnum.LessThan: return "<";
                case FilterOptionEnum.NotEqual: return "!=";
                case FilterOptionEnum.StartsWith: return "StartsWith";
                case FilterOptionEnum.NotStartsWith: return "NotStartsWith";
                case FilterOptionEnum.Contains: return "Contains";
                case FilterOptionEnum.NotContains: return "NotContains";
                case FilterOptionEnum.Endswith: return "EndsWith";
                case FilterOptionEnum.NotEndswith: return "NotEndsWith";
            }
            throw "Unexpected filter op " + op;
        };
        // add/remove (static grid)
        Grid.prototype.removeRecord = function (trElem, recNum, colName) {
            if (!this.Setup.StaticData)
                throw "Static grids only";
            // get the message to display (if any)
            var message = this.Setup.DeletedMessage;
            if (message) {
                if (colName) {
                    var text = this.Setup.StaticData[recNum][colName];
                    message = message.format(text);
                }
            }
            // remove the record
            this.RemoveRecord(recNum);
            // show the message
            if (message)
                $YetaWF.message(message);
        };
        Grid.prototype.resequenceDelete = function (recNum) {
            // resequence origin
            var trs = $YetaWF.getElementsBySelector("tr[data-origin]", [this.TBody]);
            for (var _i = 0, trs_4 = trs; _i < trs_4.length; _i++) {
                var tr = trs_4[_i];
                var orig = Number($YetaWF.getAttribute(tr, "data-origin"));
                if (orig >= recNum) {
                    $YetaWF.setAttribute(tr, "data-origin", (orig - 1).toString());
                    // update all indexes for input/select fields to match record origin (TODO: check whether we should only update last index in field)
                    this.renumberFields(tr, orig, orig - 1);
                }
            }
        };
        Grid.prototype.resequence = function () {
            // resequence origin
            var trs = $YetaWF.getElementsBySelector("tr[data-origin]", [this.TBody]);
            var index = 0;
            for (var _i = 0, trs_5 = trs; _i < trs_5.length; _i++) {
                var tr = trs_5[_i];
                var orig = Number($YetaWF.getAttribute(tr, "data-origin"));
                $YetaWF.setAttribute(tr, "data-origin", index.toString());
                // update all indexes for input/select fields to match record origin (TODO: check whether we should only update last index in field)
                this.renumberFields(tr, orig, index);
                ++index;
            }
        };
        Grid.prototype.renumberFields = function (tr, origNum, newNum) {
            var inps = $YetaWF.getElementsBySelector("input[name],select[name]", [tr]);
            for (var _i = 0, inps_1 = inps; _i < inps_1.length; _i++) {
                var inp = inps_1[_i];
                var name_2 = $YetaWF.getAttribute(inp, "name");
                name_2 = name_2.replace("[" + origNum + "]", "[" + newNum + "]");
                $YetaWF.setAttribute(inp, "name", name_2);
            }
        };
        // API
        // API
        // API
        Grid.prototype.enable = function (enable) {
            // TODO: This currently only works with jqueryui class
            $YetaWF.elementRemoveClass(this.Control, this.Setup.DisabledCss);
            if (!enable)
                $YetaWF.elementAddClass(this.Control, this.Setup.DisabledCss);
        };
        Object.defineProperty(Grid.prototype, "FieldName", {
            get: function () {
                return this.Setup.FieldName;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(Grid.prototype, "StaticData", {
            get: function () {
                if (!this.Setup.StaticData)
                    throw "Static grids only";
                return this.Setup.StaticData;
            },
            enumerable: false,
            configurable: true
        });
        Object.defineProperty(Grid.prototype, "ExtraData", {
            get: function () {
                return this.Setup.ExtraData;
            },
            enumerable: false,
            configurable: true
        });
        Grid.prototype.AddRecord = function (tr, staticData) {
            if (!this.Setup.StaticData)
                throw "Static grids only";
            $YetaWF.appendMixedHTML(this.TBody, tr, true);
            var lastTr = this.TBody.lastChild;
            var origin = this.Setup.StaticData.length;
            $YetaWF.setAttribute(lastTr, "data-origin", origin.toString());
            this.renumberFields(lastTr, 0, origin);
            this.Setup.StaticData.push(staticData);
            this.Setup.Records++;
            this.updatePage();
            this.reload(Math.max(0, this.Setup.Pages - 1));
            this.updateStatus();
        };
        Grid.prototype.AddRecords = function (trs, staticData) {
            if (!this.Setup.StaticData)
                throw "Static grids only";
            for (var _i = 0, trs_6 = trs; _i < trs_6.length; _i++) {
                var tr = trs_6[_i];
                $YetaWF.appendMixedHTML(this.TBody, tr, true);
                var lastTr = this.TBody.lastChild;
                var origin_1 = this.Setup.StaticData.length;
                $YetaWF.setAttribute(lastTr, "data-origin", origin_1.toString());
                this.renumberFields(lastTr, 0, origin_1);
                this.Setup.StaticData.push(staticData);
                this.Setup.Records++;
            }
            this.updatePage();
            this.reload(Math.max(0, this.Setup.Pages - 1));
            this.updateStatus();
        };
        Grid.prototype.ReplaceRecord = function (index, tr, staticData) {
            if (!this.Setup.StaticData)
                throw "Static grids only";
            if (index < 0 || index >= this.Setup.StaticData.length)
                throw "Index " + index + " out of bounds";
            var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
            // insert the new tr
            var indexTr = trs[index];
            $YetaWF.insertMixedHTML(indexTr, tr, true);
            // remove the existing row element
            this.TBody.removeChild(indexTr);
            // renumber
            trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
            indexTr = trs[index];
            this.renumberFields(indexTr, 0, index);
            $YetaWF.setAttribute(indexTr, "data-origin", index.toString());
            // replace the static data record
            this.Setup.StaticData[index] = staticData;
            this.updatePage();
            this.reload(Math.max(0, this.Setup.Pages - 1));
            this.updateStatus();
        };
        Grid.prototype.RemoveRecord = function (index) {
            if (!this.Setup.StaticData)
                throw "Static grids only";
            if (index < 0 || index >= this.Setup.StaticData.length)
                throw "Index " + index + " out of bounds";
            var tr = $YetaWF.getElement1BySelector("tr[data-origin='" + index.toString() + "']", [this.TBody]);
            tr.remove();
            this.Setup.StaticData.splice(index, 1);
            this.Setup.Records--;
            this.resequenceDelete(index);
            this.updatePage();
            this.reload(Math.max(0, this.Setup.Pages - 1));
            this.updateStatus();
        };
        Grid.prototype.Clear = function () {
            if (!this.Setup.StaticData)
                throw "Static grids only";
            var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
            for (var _i = 0, trs_7 = trs; _i < trs_7.length; _i++) {
                var tr = trs_7[_i];
                tr.remove();
            }
            this.Setup.StaticData = [];
            this.Setup.Records = 0;
            this.updatePage();
            this.reload(Math.max(0, this.Setup.Pages - 1));
            this.updateStatus();
        };
        Grid.prototype.moveRawRecord = function (sel, index) {
            if (!this.Setup.StaticData)
                throw "Static grids only";
            if (sel < 0 || sel >= this.Setup.StaticData.length)
                throw "Index sel=" + sel + " out of bounds";
            if (index < 0 || index > this.Setup.StaticData.length)
                throw "Index index=" + index + " out of bounds";
            if (index === sel || index === sel + 1)
                return; // nothing to move
            var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
            var selTr = trs[sel];
            // remove the static data record
            var data = this.Setup.StaticData[sel];
            this.Setup.StaticData.splice(sel, 1);
            // remove the table row element
            this.TBody.removeChild(selTr);
            // insert the static data record at the new position
            if (index > sel)
                --index;
            if (index >= this.Setup.StaticData.length) {
                this.Setup.StaticData.push(data);
                this.TBody.appendChild(selTr);
            }
            else {
                this.Setup.StaticData.splice(index, 0, data);
                this.TBody.insertBefore(selTr, this.TBody.children[index + 1]); // take tg_empty into account
            }
            this.resequence();
            this.updatePage();
            this.updateStatus();
        };
        Grid.prototype.SelectedIndex = function () {
            var sel = $YetaWF.getElement1BySelectorCond("tr." + this.Setup.RowHighlightCss + ",tr." + this.Setup.RowDragDropHighlightCss, [this.TBody]);
            if (sel == null)
                return -1;
            var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
            var rowIndex = Array.prototype.indexOf.call(trs, sel);
            return rowIndex;
        };
        Grid.prototype.SetSelectedIndex = function (index) {
            var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
            this.ClearSelection();
            if (index < 0 || index >= trs.length)
                return;
            $YetaWF.elementToggleClass(trs[index], this.Setup.RowHighlightCss, true);
        };
        Grid.prototype.ClearSelection = function () {
            var sel = $YetaWF.getElement1BySelectorCond("tr." + this.Setup.RowHighlightCss + ",tr." + this.Setup.RowDragDropHighlightCss, [this.TBody]);
            if (sel) {
                $YetaWF.elementToggleClass(sel, this.Setup.RowHighlightCss, false);
                $YetaWF.elementToggleClass(sel, this.Setup.RowDragDropHighlightCss, false);
            }
        };
        Grid.prototype.GetTotalRecords = function () {
            return this.Setup.Records;
        };
        Grid.prototype.GetRecord = function (index) {
            if (!this.Setup.StaticData)
                throw "Static grids only";
            if (index < 0 || index >= this.Setup.StaticData.length)
                throw "Index " + index + " out of bounds";
            return this.Setup.StaticData[index];
        };
        Grid.prototype.GetTR = function (index) {
            if (this.Setup.StaticData)
                ++index; // the first row in an <no records> indicator
            if (index < 0 || index >= this.TBody.children.length)
                throw "Index " + index + " out of bounds";
            return this.TBody.children[index];
        };
        Grid.prototype.HitTest = function (x, y) {
            if (!this.Setup.StaticData)
                throw "Static grids only";
            var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
            var index = 0;
            for (var _i = 0, trs_8 = trs; _i < trs_8.length; _i++) {
                var tr = trs_8[_i];
                var rect = tr.getBoundingClientRect();
                if (x < rect.left || x > rect.left + rect.width)
                    return -1;
                if (y < rect.top)
                    return -1;
                if (y < rect.top + rect.height)
                    return index;
                ++index;
            }
            return -1;
        };
        Grid.prototype.HitTestInsert = function (x, y) {
            if (!this.Setup.StaticData)
                throw "Static grids only";
            var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
            var index = 0;
            for (var _i = 0, trs_9 = trs; _i < trs_9.length; _i++) {
                var tr = trs_9[_i];
                var rect = tr.getBoundingClientRect();
                if (x < rect.left || x > rect.left + rect.width)
                    return -1;
                if (y < rect.top)
                    return -1;
                if (y < rect.top + rect.height / 2)
                    return index;
                ++index;
                if (y < rect.top + rect.height)
                    return index;
            }
            return -1;
        };
        /**
         * Reloads the grid in its entirety using the provided extradata. The extradata is only saved in the grid if reloading is successful.
         * The callback is called if the grid is successfully reloaded.
         */
        Grid.prototype.ReloadAll = function (overrideExtraData, successful) {
            var _this = this;
            if (this.Setup.StaticData)
                throw "Ajax grids only";
            this.reload(0, undefined, undefined, overrideExtraData, false, function () {
                // successful
                if (overrideExtraData)
                    _this.Setup.ExtraData = overrideExtraData;
                if (successful)
                    successful();
            });
        };
        /**
         * Reloads the grid in its entirety using the provided data, extradata. The extradata is only saved in the grid if reloading is successful.
         * The callback is called if the grid is successfully reloaded.
         */
        Grid.prototype.ReloadStatic = function (data, overrideExtraData, successful) {
            var _this = this;
            if (!this.Setup.StaticData)
                throw "Static grids only";
            this.Clear();
            this.Setup.StaticData = data;
            this.reload(0, undefined, undefined, overrideExtraData, true /*sort forced rerendering*/, function () {
                // successful
                if (overrideExtraData)
                    _this.Setup.ExtraData = overrideExtraData;
                if (successful)
                    successful();
            });
        };
        Grid.ReloadFromId = function (id) {
            var grid = YetaWF.ComponentBaseDataImpl.getControlById(id, Grid.SELECTOR);
            grid.reload(0);
        };
        /* Set all check boxes in a static grid control */
        Grid.prototype.SetCheckBoxes = function (set) {
            if (this.Setup.StaticData && this.Setup.NoSubmitContents) {
                this.SubmitCheckCol = this.getSubmitCheckCol();
                if (this.SubmitCheckCol >= 0) {
                    var checks = $YetaWF.getElementsBySelector("td:nth-child(" + (this.SubmitCheckCol + 1) + ") input[type='checkbox']", [this.Control]);
                    for (var _i = 0, checks_1 = checks; _i < checks_1.length; _i++) {
                        var check = checks_1[_i];
                        if (!check.disabled) {
                            var tr = $YetaWF.elementClosest(check, "tr");
                            var recNum = Number($YetaWF.getAttribute(tr, "data-origin"));
                            this.Setup.StaticData[recNum][this.Setup.Columns[this.SubmitCheckCol].Name] = set;
                            check.checked = set;
                        }
                    }
                }
            }
        };
        /* returns whether all checkboxes are selected  in a static grid control */
        Grid.prototype.GetAllCheckBoxesSelected = function () {
            if (this.Setup.StaticData && this.Setup.NoSubmitContents) {
                this.SubmitCheckCol = this.getSubmitCheckCol();
                if (this.SubmitCheckCol >= 0) {
                    var checks = $YetaWF.getElementsBySelector("td:nth-child(" + (this.SubmitCheckCol + 1) + ") input[type='checkbox']", [this.Control]);
                    for (var _i = 0, checks_2 = checks; _i < checks_2.length; _i++) {
                        var check = checks_2[_i];
                        if (!check.disabled) {
                            var tr = $YetaWF.elementClosest(check, "tr");
                            var recNum = Number($YetaWF.getAttribute(tr, "data-origin"));
                            var set = this.Setup.StaticData[recNum][this.Setup.Columns[this.SubmitCheckCol].Name];
                            if (!set)
                                return false;
                        }
                    }
                    return true;
                }
            }
            throw "GetAllCheckBoxesSelected not available";
        };
        Grid.TEMPLATE = "yt_grid";
        Grid.SELECTOR = ".yt_grid";
        Grid.EVENTSELECT = "grid_selectionchange";
        Grid.EVENTDBLCLICK = "grid_dblclick";
        Grid.EVENTDRAGDROPDONE = "grid_dragdropdone";
        Grid.EVENTDRAGDROPCANCEL = "grid_dragdropcancel";
        Grid.CurrentControl = null; // current control during grid resize
        return Grid;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.Grid = Grid;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Grid.js.map
