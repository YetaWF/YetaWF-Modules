/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

// Kendo UI menu use

namespace YetaWF_ComponentsHTML {

    export interface IPackageLocs {
        GridTotals: string;
        GridTotal0: string;
        GridTotalNone: string;
    }

    interface GridSetup {
        CanSort: boolean;
        CanFilter: boolean;
        CanReorder: boolean;
        ShowPager: boolean;
        FieldName: string;
        AjaxUrl: string;
        StaticData: any[] | null;
        Page: number;
        PageSize: number;
        Records: number;
        Pages: number;
        Columns: GridColumnDefinition[];
        FilterMenusHTML: string;
        MinColumnWidth: number;
        SaveSettingsColumnWidthsUrl: string;
        ExtraData: any;
        HoverCss: string;
        HighlightCss: string;
        DisabledCss: string;
        RowHighlightCss: string;
        RowDragDropHighlightCss: string;
        SortActiveCss: string;
        SettingsModuleGuid: string;
        HighlightOnClick: boolean;

        DeletedMessage: string;
        DeleteConfirmationMessage: string;
        DeletedColumnDisplay: string;

        NoSubmitContents: boolean;
    }
    interface GridColumnDefinition {
        Name: string;
        Sortable: boolean;
        Sort: SortByEnum;
        OnlySubmitWhenChecked: boolean;
        Locked: boolean;
        FilterOp: FilterOptionEnum | null;
        FilterType: string;
        FilterId: string;
        MenuId: string;
    }
    enum SortByEnum {
        NotSpecified = 0,
        Ascending = 1,
        Descending = 2
    }
    enum FilterOptionEnum {
        Equal = 1,
        NotEqual = 2,
        LessThan = 3,
        LessEqual = 4,
        GreaterThan = 5,
        GreaterEqual = 6,
        StartsWith = 7,
        NotStartsWith = 8,
        Contains = 9,
        NotContains = 10,
        Endswith = 11,
        NotEndswith = 12,
        All = 0xFFFF
    }
    interface OverrideColumnFilter {
        ColIndex: number;
        FilterOp: FilterOptionEnum;
    }
    interface GridPartialResult {
        Records: number;
        TBody: string;
        Pages: number;
        Page: number;
        PageSize: number;
    }
    enum FilterBoolEnum {
        All = 0,
        Yes = 1,
        No = 2
    }

    export class Grid extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_grid";
        public static readonly SELECTOR: string = ".yt_grid";

        private Setup: GridSetup;

        private BtnReload: HTMLDivElement | null = null;
        private BtnSearch: HTMLDivElement | null = null;
        private BtnTop: HTMLDivElement | null = null;
        private BtnPrev: HTMLDivElement | null = null;
        private BtnNext: HTMLDivElement | null = null;
        private BtnBottom: HTMLDivElement | null = null;
        private FilterBar: HTMLDivElement | null = null;
        private PagerTotals: HTMLDivElement | null = null;
        private InputPage: YetaWF_ComponentsHTML.IntValueEditComponent | null = null;
        private SelectPageSize: YetaWF_ComponentsHTML.DropDownListEditComponent | null = null;
        private ColumnResizeBar: HTMLElement | null = null;
        private ColumnResizeHeader: HTMLTableHeaderCellElement | null = null;
        private TBody: HTMLElement;
        private LoadingDiv: HTMLDivElement | null = null;
        private static CurrentControl: Grid | null = null;// current control during grid resize
        private SubmitCheckCol: number = -1;// column with checkbox determining whether to submit record
        private reloadInProgress: boolean = false;
        private reorderingInProgress: boolean = false;
        private reorderingRowElement: HTMLTableRowElement | null = null;

        constructor(controlId: string, setup: GridSetup) {
            super(controlId, Grid.TEMPLATE, Grid.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: (control: Grid): string | null => {
                    let total = control.GetTotalRecords();
                    if (!total) return null;
                    return total.toString();
                },
                Enable: (control: Grid, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                    // clearOnDisable not supported
                },
            }, false, (tag: HTMLElement, control: Grid): void => {
                control.internalDestroy();
            });


            this.Setup = setup;

            ComponentsHTMLHelper.MUSTHAVE_JQUERYUI();

            this.TBody = $YetaWF.getElement1BySelector("tbody", [this.Control]);

            if (this.Setup.ShowPager) {
                this.BtnReload = $YetaWF.getElement1BySelectorCond(".tg_reload", [this.Control]) as HTMLDivElement | null;
                this.BtnSearch = $YetaWF.getElement1BySelectorCond(".tg_search", [this.Control]) as HTMLDivElement | null;
                this.BtnTop = $YetaWF.getElement1BySelectorCond(".tg_pgtop", [this.Control]) as HTMLDivElement | null;
                this.BtnPrev = $YetaWF.getElement1BySelectorCond(".tg_pgprev", [this.Control]) as HTMLDivElement | null;
                this.BtnNext = $YetaWF.getElement1BySelectorCond(".tg_pgnext", [this.Control]) as HTMLDivElement | null;
                this.BtnBottom = $YetaWF.getElement1BySelectorCond(".tg_pgbottom", [this.Control]) as HTMLDivElement | null;
                this.PagerTotals = $YetaWF.getElement1BySelectorCond(".tg_totals", [this.Control]) as HTMLDivElement | null;
                if (this.Setup.PageSize) {
                    this.InputPage = YetaWF.ComponentBaseDataImpl.getControlFromSelector<IntValueEditComponent>("input[name$='.__Page']", IntValueEditComponent.SELECTOR, [this.Control]);
                    this.SelectPageSize = YetaWF.ComponentBaseDataImpl.getControlFromSelector<DropDownListEditComponent>("select[name$='.__PageSelection']", DropDownListEditComponent.SELECTOR, [this.Control]);
                }
            }
            this.FilterBar = $YetaWF.getElement1BySelectorCond(".tg_filter", [this.Control]) as HTMLDivElement | null;

            this.updateStatus();

            $YetaWF.registerEventHandler(this.Control, "mouseover", ".tg_header th, .tg_filter .tg_button, .tg_pager .tg_button", (ev: MouseEvent): boolean => {
                if (!$YetaWF.elementHasClass(ev.__YetaWFElem, this.Setup.HoverCss))
                    $YetaWF.elementAddClass(ev.__YetaWFElem, this.Setup.HoverCss);
                return true;
            });
            $YetaWF.registerEventHandler(this.Control, "mouseout", ".tg_header th, .tg_filter .tg_button, .tg_pager .tg_button", (ev: MouseEvent): boolean => {
                $YetaWF.elementRemoveClass(ev.__YetaWFElem, this.Setup.HoverCss);
                return true;
            });
            $YetaWF.registerEventHandler(this.Control, "mouseover", ".tg_resize", (ev: MouseEvent): boolean => {
                // don't allow mouseover to propagate and close tooltips
                $YetaWF.closeOverlays();
                return false;
            });
            // Show/hide filter bar with search button
            if (this.Setup.CanFilter && this.BtnSearch && this.FilterBar) {
                $YetaWF.registerEventHandler(this.BtnSearch, "click", null, (ev: MouseEvent): boolean => {
                    var filterBar = this.FilterBar as HTMLElement;
                    if ($YetaWF.isVisible(filterBar))
                        filterBar.style.display = "none";
                    else {
                        filterBar.style.display = "";
                        $YetaWF.processActivateDivs([filterBar]);
                    }
                    return false;
                });
            }
            // Reload
            if (this.BtnReload) {
                $YetaWF.registerEventHandler(this.BtnReload, "click", null, (ev: MouseEvent): boolean => {
                    if (!$YetaWF.elementHasClass(ev.__YetaWFElem, this.Setup.DisabledCss))
                        this.reload(this.Setup.Page);
                    return false;
                });
                $YetaWF.registerModuleRefresh(this.Control, (mod: HTMLElement): void => {
                    this.reload(this.Setup.Page);
                });
            }
            if (!this.Setup.StaticData)
                this.LoadingDiv = $YetaWF.getElement1BySelectorCond(".tg_loading", [this.Control]) as HTMLDivElement;
            // Nav buttons
            if (this.BtnTop) {
                $YetaWF.registerEventHandler(this.BtnTop, "click", null, (ev: MouseEvent): boolean => {
                    if (!$YetaWF.elementHasClass(ev.__YetaWFElem, this.Setup.DisabledCss)) {
                        if (this.Setup.Page >= 0)
                            this.reload(0);
                    }
                    return false;
                });
            }
            if (this.BtnPrev) {
                $YetaWF.registerEventHandler(this.BtnPrev, "click", null, (ev: MouseEvent): boolean => {
                    if (!$YetaWF.elementHasClass(ev.__YetaWFElem, this.Setup.DisabledCss)) {
                        var page = this.Setup.Page - 1;
                        if (page >= 0)
                            this.reload(page);
                    }
                    return false;
                });
            }
            if (this.BtnNext) {
                $YetaWF.registerEventHandler(this.BtnNext, "click", null, (ev: MouseEvent): boolean => {
                    if (!$YetaWF.elementHasClass(ev.__YetaWFElem, this.Setup.DisabledCss)) {
                        var page = this.Setup.Page + 1;
                        if (page < this.Setup.Pages)
                            this.reload(page);
                    }
                    return false;
                });
            }
            if (this.BtnBottom) {
                $YetaWF.registerEventHandler(this.BtnBottom, "click", null, (ev: MouseEvent): boolean => {
                    if (!$YetaWF.elementHasClass(ev.__YetaWFElem, this.Setup.DisabledCss)) {
                        var page = this.Setup.Pages - 1;
                        if (page >= 0)
                            this.reload(page);
                    }
                    return false;
                });
            }
            // Page input
            if (this.InputPage) {
                $YetaWF.registerEventHandler(this.InputPage.Control, "keydown", null, (ev: KeyboardEvent): boolean => {
                    if (ev.keyCode === 13 && this.InputPage) { // Return
                        var page = this.InputPage.value - 1;
                        this.reload(page);
                        return false;
                    }
                    return true;
                });
            }
            // pagesize selection
            if (this.SelectPageSize) {
                this.SelectPageSize.Control.addEventListener("dropdownlist_change", (evt: Event): void => {
                    if (this.SelectPageSize)
                        this.reload(0, Number(this.SelectPageSize.value));
                });
            }
            // Column resizing
            $YetaWF.registerEventHandler(this.Control, "mousedown", ".tg_resize", (ev: MouseEvent): boolean => {
                if (!this.reloadInProgress) {
                    Grid.CurrentControl = this;
                    this.ColumnResizeBar = ev.__YetaWFElem;
                    this.ColumnResizeHeader = $YetaWF.elementClosest(this.ColumnResizeBar, "th") as HTMLTableHeaderCellElement;
                    document.body.style.cursor = "col-resize";
                    window.addEventListener("mousemove", Grid.resizeColumn, false);
                    window.addEventListener("mouseup", Grid.resizeColumnDone, false);
                }
                return false;
            });
            // Sorting
            if (this.Setup.CanSort) {
                $YetaWF.registerEventHandler(this.Control, "click", ".tg_header th", (ev: MouseEvent): boolean => {
                    if (!this.reloadInProgress) {
                        var colIndex = Array.prototype.indexOf.call((ev.__YetaWFElem.parentElement as HTMLElement).children, ev.__YetaWFElem);
                        if (colIndex < 0 || colIndex >= this.Setup.Columns.length) throw `Invalid column index ${colIndex} - max is ${this.Setup.Columns.length}`;/*DEBUG*/
                        var col = this.Setup.Columns[colIndex];
                        if (col.Sortable) {
                            if (col.Sort === SortByEnum.NotSpecified) {
                                this.clearSorts();
                                this.setSortOrder(col, colIndex, SortByEnum.Ascending);
                            } else if (col.Sort === SortByEnum.Descending) {
                                this.setSortOrder(col, colIndex, SortByEnum.Ascending);
                            } else {
                                this.setSortOrder(col, colIndex, SortByEnum.Descending);
                            }
                            this.reload(0, undefined, undefined, undefined, true);
                        }
                    }
                    return false;
                });
            }
            // Filtering
            if (this.Setup.CanFilter && this.FilterBar) {
                $YetaWF.registerEventHandler(this.FilterBar, "click", ".tg_fmenu", (ev: MouseEvent): boolean => {
                    var filter = $YetaWF.elementClosest(ev.__YetaWFElem, ".tg_filter");
                    var head = $YetaWF.elementClosest(ev.__YetaWFElem, "th");
                    var colIndex = Array.prototype.indexOf.call(filter.children, head);
                    var ulElem = $YetaWF.getElementById(this.Setup.Columns[colIndex].MenuId);
                    if ($YetaWF.isVisible(ulElem))
                        $(ulElem).hide();
                    else {
                        $YetaWF.closeOverlays();
                        $(ulElem).show();
                        $(ulElem).position({ //jQuery-ui use
                            my: "left top",
                            at: "left bottom",
                            of: $(ev.__YetaWFElem),
                            collision: "flip"
                        });
                    }
                    return false;
                });
                $YetaWF.registerEventHandler(this.FilterBar, "click", ".tg_fclear", (ev: MouseEvent): boolean => {
                    var filter = $YetaWF.elementClosest(ev.__YetaWFElem, ".tg_filter");
                    var head = $YetaWF.elementClosest(ev.__YetaWFElem, "th");
                    var colIndex = Array.prototype.indexOf.call(filter.children, head);
                    this.clearColSortValue(colIndex);
                    this.reload(0);
                    return false;
                });
                $YetaWF.registerEventHandlerBody("mousedown", null, (ev: MouseEvent): boolean => {
                    if (ev.which !== 1) return true;
                    var menus = $YetaWF.getElementsBySelector(".yt_grid_menus ul.k-menu");
                    for (let menu of menus) {
                        if ($YetaWF.isVisible(menu)) {
                            setTimeout(() => {
                                $(menu).hide();
                            }, 200);
                        }
                    }
                    return true;
                });
                this.addDirectFilterHandlers();

                $YetaWF.addWhenReadyOnce((tag: HTMLElement): void => {
                    $YetaWF.appendMixedHTML(document.body, `
<div id='${this.ControlId}_menus' class='yt_grid_menus' data-grid='${this.ControlId}'>
    ${this.Setup.FilterMenusHTML}
</div>`);
                });
            }
            // Delete action (static only)
            if (this.Setup.StaticData) {
                $YetaWF.registerEventHandler(this.Control, "click", "[name='DeleteAction']", (ev: MouseEvent): boolean => {
                    if (!this.Setup.StaticData) return true;
                    // find the record number to delete
                    var trElem = $YetaWF.elementClosest(ev.__YetaWFElem, "tr");
                    var recNum = Number($YetaWF.getAttribute(trElem, "data-origin"));

                    var message = this.Setup.DeleteConfirmationMessage;
                    var colName = this.Setup.DeletedColumnDisplay;
                    if (message) {
                        if (colName) {
                            var text = this.Setup.StaticData[recNum][colName];
                            message = message.format(text);
                        }
                        $YetaWF.alertYesNo(message, undefined, (): void => {
                            this.removeRecord(trElem, recNum, colName);
                        });
                    } else
                        this.removeRecord(trElem, recNum, colName);
                    return false;
                });
            }
            // Selection
            $YetaWF.registerEventHandler(this.TBody, "mousedown", "tr:not(.tg_emptytr)", (ev: MouseEvent): boolean => {
                var clickedElem = ev.__YetaWFElem;
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
                            this.reorderingRowElement = clickedElem as HTMLTableRowElement;
                            this.reorderingInProgress = true;
                            //console.log("Reordering starting");
                            $YetaWF.elementToggleClass(this.reorderingRowElement, this.Setup.RowHighlightCss, false);
                            $YetaWF.elementToggleClass(this.reorderingRowElement, this.Setup.RowDragDropHighlightCss, true);
                            return false;
                        }
                        return true;
                    }
                    var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
                    for (let tr of trs)
                        $YetaWF.elementToggleClass(tr, this.Setup.RowHighlightCss, false);
                    $YetaWF.elementToggleClass(clickedElem, this.Setup.RowHighlightCss, true);

                    var event = document.createEvent("Event");
                    event.initEvent("grid_selectionchange", true, true);
                    this.Control.dispatchEvent(event);
                    return false;
                }
                return true;
            });
            // Drag & drop
            $YetaWF.registerEventHandlerBody("mousemove", null, (ev: MouseEvent): boolean => {
                if (this.reorderingInProgress) {

                    //console.log("Reordering...")

                    var rect = this.TBody.getBoundingClientRect();
                    if (ev.clientX < rect.left || ev.clientX > rect.left + rect.width ||
                        ev.clientY < rect.top || ev.clientY > rect.top + rect.height) {

                        this.cancelDragDrop();
                        return true;
                    }
                    var sel = this.SelectedIndex();
                    if (sel < 0) {
                        this.cancelDragDrop();
                        return true;
                    }

                    var insert = this.HitTestInsert(ev.clientX, ev.clientY);
                    //console.log(`insert = ${insert}  sel = ${sel}`);
                    if (insert === sel || insert === sel + 1)
                        return true;// nothing to move

                    this.moveRawRecord(sel, insert);
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.TBody, "mouseup", null, (ev: MouseEvent): boolean => {
                if (this.reorderingInProgress) {

                    this.doneDragDrop();

                }
                return true;
            });
            // OnlySubmitWhenChecked
            if (this.Setup.StaticData && this.Setup.NoSubmitContents) {
                this.SubmitCheckCol = this.getSubmitCheckCol();
                if (this.SubmitCheckCol >= 0) {
                    this.setInitialSubmitStatus();
                    // update static data with new checkbox value
                    $YetaWF.registerEventHandler(this.TBody, "change", `tr td:nth-child(${this.SubmitCheckCol + 1}) input[type='checkbox']`, (ev: Event): boolean => {
                        if (!this.Setup.StaticData) return true;
                        var tr = $YetaWF.elementClosest(ev.__YetaWFElem, "tr");
                        var recNum = Number($YetaWF.getAttribute(tr, "data-origin"));
                        var val = (ev.__YetaWFElem as HTMLInputElement).checked;
                        this.Setup.StaticData[recNum][this.Setup.Columns[this.SubmitCheckCol].Name] = val;
                        //$YetaWF.elementToggleClass(tr, YConfigs.Forms.CssFormNoSubmitContents, !val);
                        return false;
                    });
                }
                // handle submit local data
                $YetaWF.Forms.addPreSubmitHandler(true, {
                    form: $YetaWF.Forms.getForm(this.Control),
                    callback: (entry: YetaWF.SubmitHandlerEntry): void => {
                        this.submitLocalData(entry);
                    },
                    userdata: this
                });
            }
        }
        // Drag&drop
        private cancelDragDrop(): void {
            if (this.reorderingRowElement) {
                $YetaWF.elementToggleClass(this.reorderingRowElement, this.Setup.RowHighlightCss, true);
                $YetaWF.elementToggleClass(this.reorderingRowElement, this.Setup.RowDragDropHighlightCss, false);
                this.reorderingRowElement = null;
            }
            this.reorderingInProgress = false;
            //console.log("Reordering canceled - left boundary")

            var event = document.createEvent("Event");
            event.initEvent("grid_dragdropcancel", true, true);
            this.Control.dispatchEvent(event);
        }
        private doneDragDrop(): void {
            if (this.reorderingRowElement) {
                $YetaWF.elementToggleClass(this.reorderingRowElement, this.Setup.RowHighlightCss, true);
                $YetaWF.elementToggleClass(this.reorderingRowElement, this.Setup.RowDragDropHighlightCss, false);
                this.reorderingRowElement = null;
            }
            this.reorderingInProgress = false;
            //console.log("Reordering ended")

            var event = document.createEvent("Event");
            event.initEvent("grid_dragdropdone", true, true);
            this.Control.dispatchEvent(event);
        }
        // OnlySubmitWhenChecked
        private setInitialSubmitStatus(): void {
            if (!this.Setup.StaticData || !this.Setup.NoSubmitContents) return;
            //var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
            //for (let tr of trs) {
            //    var recNum = Number($YetaWF.getAttribute(tr, "data-origin"));
            //    var val = this.Setup.StaticData[recNum][this.Setup.Columns[this.SubmitCheckCol].Name];
            //    $YetaWF.elementToggleClass(tr, YConfigs.Forms.CssFormNoSubmitContents, !val);
            //}
        }
        private getSubmitCheckCol(): number {
            var colIndex: number = -1;
            var cols = this.Setup.Columns.filter((col: GridColumnDefinition, index: number, cols: GridColumnDefinition[]): boolean => {
                if (!col.OnlySubmitWhenChecked) return false;
                colIndex = index;
                return true;
            });
            if (cols.length > 1)
                throw "More than one column marked OnlySubmitWhenChecked";
            return colIndex;
        }
        private submitLocalData(entry: YetaWF.SubmitHandlerEntry): void {
            if (!this.Setup.StaticData) return;
            var div = `<div class='${$YetaWF.Forms.DATACLASS}' style='display:none'>`;
            // retrieve all rows and add input/select fields to data div, resequence to make mvc serialization of lists work
            var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
            var row = 0;
            var re1 = new RegExp("\\[[0-9]+\\]", "gim");
            for (let tr of trs) {
                var recNum = Number($YetaWF.getAttribute(tr, "data-origin"));
                var val = this.Setup.StaticData[recNum][this.Setup.Columns[this.SubmitCheckCol].Name];
                if (val) { // add record if the checkbox is selected
                    var copied = false;
                    var inputs = $YetaWF.getElementsBySelector("input,select", [tr]);
                    for (let input of inputs) {
                        var name = $YetaWF.getAttributeCond(input, "name");
                        if (name) {
                            var copy = input.cloneNode() as HTMLElement;
                            // replace name with serialized name[row] so mvc serialization works
                            name = name.replace(re1, `[${row.toString()}]`);
                            $YetaWF.setAttribute(copy, "name", name);
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
                (entry.form as HTMLElement).insertAdjacentHTML("beforeend", div);
        }
        // sorting
        private clearSorts(): void {
            var colIndex = 0;
            for (let col of this.Setup.Columns) {
                if (col.Sortable) {
                    col.Sort = SortByEnum.NotSpecified;
                    this.setSortOrder(col, colIndex, SortByEnum.NotSpecified);
                }
                ++colIndex;
            }
        }
        private setSortOrder(col: GridColumnDefinition, colIndex: number, sortBy: SortByEnum): void {
            for (let col of this.Setup.Columns) {
                if (col.Sortable)
                    col.Sort = SortByEnum.NotSpecified;
            }
            col.Sort = sortBy;
            // turn indicators in header on or off
            var ths = $YetaWF.getElementsBySelector(".tg_header th", [this.Control]);
            var th = ths[colIndex];
            var asc = $YetaWF.getElement1BySelector(".tg_sorticon .tg_sortasc", [th]);
            var desc = $YetaWF.getElement1BySelector(".tg_sorticon .tg_sortdesc", [th]);
            $YetaWF.elementToggleClass(asc, this.Setup.SortActiveCss, sortBy === SortByEnum.Ascending);
            $YetaWF.elementToggleClass(desc, this.Setup.SortActiveCss, sortBy === SortByEnum.Descending);
        }
        private getSortColumn(): GridColumnDefinition | null {
            for (let col of this.Setup.Columns) {
                if (col.Sortable && col.Sort !== SortByEnum.NotSpecified)
                    return col;
            }
            return null;
        }
        // Resizing
        private static resizeColumn(ev: MouseEvent): boolean {
            var currentControl = Grid.CurrentControl;
            if (currentControl && currentControl.ColumnResizeHeader) {
                var rect = currentControl.ColumnResizeHeader.getBoundingClientRect();
                let actualWidth = rect.width;
                var newActualWidth = ev.clientX - rect.left;
                let givenWidth = Number(currentControl.ColumnResizeHeader.style.width.replace("px",""));
                let diff = newActualWidth - actualWidth; // <0 shring, >0 expand
                let newGivenWidth = givenWidth + diff;
                currentControl.ColumnResizeHeader.style.width = `${newGivenWidth}px`;
            }
            return false;
        }
        private static resizeColumnDone(ev: MouseEvent): boolean {
            var currentControl = Grid.CurrentControl;
            if (currentControl && currentControl.ColumnResizeBar && currentControl.ColumnResizeHeader && currentControl.Setup.SaveSettingsColumnWidthsUrl) {
                document.body.style.cursor = "default";
                window.removeEventListener("mousemove", this.resizeColumn, false);
                window.removeEventListener("mouseup", this.resizeColumnDone, false);

                // save column widths after user resizes
                if (currentControl.Setup.SettingsModuleGuid) {
                    // send save request, we don't care about the response
                    let uri = $YetaWF.parseUrl(currentControl.Setup.SaveSettingsColumnWidthsUrl);
                    uri.addSearch("SettingsModuleGuid", currentControl.Setup.SettingsModuleGuid);
                    var colIndex = Array.prototype.indexOf.call((currentControl.ColumnResizeHeader.parentElement as HTMLElement).children, currentControl.ColumnResizeHeader);
                    uri.addSearch("Columns[0].Key", currentControl.Setup.Columns[colIndex].Name);
                    uri.addSearch("Columns[0].Value", parseInt((currentControl.ColumnResizeHeader.style.width as string).replace("px", ""), 0));

                    let request: XMLHttpRequest = new XMLHttpRequest();
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
            return false;
        }

        // reloading
        private reload(page: number, newPageSize?: number, overrideColFilter?: OverrideColumnFilter, overrideExtraData?: any|null|undefined, sort?: boolean, done?: () => void): void {

            if (!this.reloadInProgress) {

                this.setReloading(true);

                if (page < 0) page = 0;

                if (this.Setup.StaticData && !sort) {
                    // show/hide selected rows
                    if (this.Setup.PageSize > 0) {
                        var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
                        for (let tr of trs) {
                            tr.setAttribute("style", "display:none");
                        }
                        var len = trs.length;
                        var count = 0;
                        for (var i = page * this.Setup.PageSize; i < len; ++i) {
                            if (count >= (newPageSize || this.Setup.PageSize))
                                break;
                            trs[i].removeAttribute("style");
                            // init any controls that just became visible
                            $YetaWF.processActivateDivs([trs[i]]);
                            ++count;
                        }
                    }
                    this.Setup.Page = page;
                    if (this.InputPage)
                        this.InputPage.value = this.Setup.Page + 1;
                    this.updateStatus();
                    this.setReloading(false);
                } else {
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
                    for (let col of this.Setup.Columns) {
                        var val = this.getColSortValue(colIndex);
                        if (val !== null && val !== "") {
                            var oper = col.FilterOp;
                            if (overrideColFilter && overrideColFilter.ColIndex === colIndex)
                                oper = overrideColFilter.FilterOp;
                            if (oper != null) {
                                uri.addSearch(`filters[${fcount}].field`, col.Name);
                                uri.addSearch(`filters[${fcount}].operator`, this.GetFilterOpString(oper));
                                uri.addSearch(`filters[${fcount}].valueAsString`, val);
                                ++fcount;
                            }
                        }
                        ++colIndex;
                    }
                    uri.addFormInfo(this.Control);
                    let uniqueIdCounters: YetaWF.UniqueIdInfo = { UniqueIdPrefix: `${this.ControlId}gr`, UniqueIdPrefixCounter: 0, UniqueIdCounter: 0 };
                    uri.addSearch(YConfigs.Forms.UniqueIdCounters, JSON.stringify(uniqueIdCounters));

                    if (this.Setup.StaticData)
                        uri.addSearch("data", JSON.stringify(this.Setup.StaticData));

                    var request: XMLHttpRequest = new XMLHttpRequest();
                    request.open("POST", this.Setup.AjaxUrl);
                    request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
                    request.onreadystatechange = (ev: Event): any => {
                        if (request.readyState === 4 /*DONE*/) {
                            this.setReloading(false);
                            $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, undefined, undefined, (result: string) => {
                                var partial: GridPartialResult = JSON.parse(request.responseText);
                                $YetaWF.processClearDiv(this.TBody);
                                this.TBody.innerHTML = "";
                                $YetaWF.appendMixedHTML(this.TBody, partial.TBody, true);
                                this.Setup.Records = partial.Records;
                                this.Setup.Pages = partial.Pages;
                                this.Setup.Page = partial.Page;
                                this.Setup.PageSize = partial.PageSize;
                                if (this.InputPage)
                                    this.InputPage.value = this.Setup.Page + 1;
                                if (this.Setup.NoSubmitContents) {
                                    this.SubmitCheckCol = this.getSubmitCheckCol();
                                    if (this.SubmitCheckCol >= 0)
                                        this.setInitialSubmitStatus();
                                }
                                this.updateStatus();
                                if (done)
                                    done();
                            });
                        }
                    };
                    var data = uri.toFormData();
                    request.send(data);
                }
            }
        }
        private setReloading(on: boolean): void {
            this.reloadInProgress = on;
            $YetaWF.setLoading(on);
            if (this.LoadingDiv) {
                if (on)
                    this.LoadingDiv.setAttribute("style", "");
                else
                    this.LoadingDiv.setAttribute("style", "display:none");
            }
        }
        private updateStatus(): void {
            if (this.PagerTotals) {
                var totals: string;
                if (this.Setup.Records === 0)
                    totals = YLocs.YetaWF_ComponentsHTML.GridTotalNone;
                else {
                    if (this.Setup.PageSize === 0) {
                        var first = 1;
                        last = this.Setup.Records;
                        if (first > last)
                            totals = YLocs.YetaWF_ComponentsHTML.GridTotal0.format(this.Setup.Records);
                        else
                            totals = YLocs.YetaWF_ComponentsHTML.GridTotals.format(first, last, this.Setup.Records);
                    } else {
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
                this.PagerTotals.innerHTML = `<span>${totals}</span>`;
            }
            if (this.BtnTop) $YetaWF.elementToggleClass(this.BtnTop, this.Setup.DisabledCss, this.Setup.Page <= 0);
            if (this.BtnPrev) $YetaWF.elementToggleClass(this.BtnPrev, this.Setup.DisabledCss, this.Setup.Page <= 0);
            if (this.BtnNext) $YetaWF.elementToggleClass(this.BtnNext, this.Setup.DisabledCss, this.Setup.Page >= this.Setup.Pages - 1);
            if (this.BtnBottom) $YetaWF.elementToggleClass(this.BtnBottom, this.Setup.DisabledCss, this.Setup.Page >= this.Setup.Pages - 1);

            // show/hide "No Records"
            if (this.Setup.StaticData) {
                if (this.Setup.Records === 0) {
                    $YetaWF.getElement1BySelector("tr.tg_emptytr", [this.TBody]).style.display = "";
                } else {
                    $YetaWF.getElement1BySelector("tr.tg_emptytr", [this.TBody]).style.display = "none";
                }
            }
        }
        private updatePage(): void {
            if (this.Setup.PageSize > 0)
                this.Setup.Pages = Math.max(1, Math.floor((this.Setup.Records - 1) / this.Setup.PageSize) + 1);
        }

        // Filtering
        private clearFilterMenuHighlights(ulElem: HTMLElement): void {
            var menuLis = $YetaWF.getElementsBySelector(`li.${this.Setup.HighlightCss}`, [ulElem]);
            for (let menuLi of menuLis)
                $YetaWF.elementRemoveClass(menuLi, this.Setup.HighlightCss);
        }
        public menuSelected(menuElem: HTMLElement, colIndex: number): void {
            // update column structure
            var sel = Number($YetaWF.getAttribute(menuElem, "data-sel"));
            // new filter
            var overrideColFilter: OverrideColumnFilter = {
                ColIndex: colIndex,
                FilterOp: sel
            };
            this.reload(0, undefined, overrideColFilter, undefined, undefined, () => {
                // clear all highlights
                var ulElem = $YetaWF.elementClosest(menuElem, "ul");
                this.clearFilterMenuHighlights(ulElem);
                // highlight new selection
                $YetaWF.elementToggleClass(menuElem, this.Setup.HighlightCss, true);
                // update button with new sort icon
                if (this.FilterBar) {
                    var icon = $YetaWF.getElement1BySelector(".t_fmenuicon", [menuElem]).innerHTML;
                    var thsFilter = $YetaWF.getElementsBySelector("th", [this.FilterBar]);
                    var btn = $YetaWF.getElement1BySelector(".tg_fmenu", [thsFilter[colIndex]]);
                    btn.innerHTML = icon;
                }
                // update column structures
                this.Setup.Columns[colIndex].FilterOp = sel;
            });
        }
        public static menuSelected(menuElem: HTMLElement, colIndex: number): void {
            var popups = $YetaWF.elementClosest(menuElem, ".yt_grid_menus");
            var gridId = $YetaWF.getAttribute(popups, "data-grid");
            var grid: Grid = YetaWF.ComponentBaseDataImpl.getControlById(gridId, YetaWF_ComponentsHTML.Grid.SELECTOR);
            grid.menuSelected(menuElem, colIndex);
        }
        private addDirectFilterHandlers(): void {
            for (let col of this.Setup.Columns) {
                switch (col.FilterType) {
                    default:
                        break;
                    case "bool":
                    case "enum":
                    case "dynenum":
                        // handle selection change
                        $YetaWF.registerCustomEventHandlerDocument("dropdownlist_change", `#${col.FilterId}`, (ev: Event): boolean => {
                            this.reload(0);
                            return false;
                        });
                        break;
                    case "long":
                    case "decimal":
                    case "datetime":
                    case "date":
                    case "text":
                    case "guid":
                        // handle return key
                        var elem = $YetaWF.getElementById(col.FilterId);
                        $YetaWF.registerEventHandler(elem, "keydown", null, (ev: KeyboardEvent): boolean => {
                            if (ev.keyCode === 13) { // Return
                                this.reload(0);
                                return false;
                            }
                            return true;
                        });
                }
            }
        }
        private getColSortValue(colIndex: number): string | null {
            var col = this.Setup.Columns[colIndex];
            switch (col.FilterType) {
                case null:
                    return null;
                case "bool":
                    var dd: YetaWF_ComponentsHTML.DropDownListEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DropDownListEditComponent.SELECTOR);
                    var boolVal: FilterBoolEnum = Number(dd.value);
                    switch (boolVal) {
                        default:
                        case FilterBoolEnum.All: return null;
                        case FilterBoolEnum.Yes: return "True";
                        case FilterBoolEnum.No: return "False";
                    }
                case "long":
                case "text":
                case "guid":
                    var edit = $YetaWF.getElementById(col.FilterId) as HTMLInputElement;
                    return edit.value;
                case "dynenum":
                    var dd: YetaWF_ComponentsHTML.DropDownListEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DropDownListEditComponent.SELECTOR);
                    if (dd.value === "-1")
                        return null;
                    return dd.value;
                case "decimal":
                    var dec: YetaWF_ComponentsHTML.DecimalEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DecimalEditComponent.SELECTOR);
                    return dec.valueText;
                case "datetime":
                    var datetime: YetaWF_ComponentsHTML.DateTimeEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DateTimeEditComponent.SELECTOR);
                    return datetime.valueText;
                case "date":
                    var date: YetaWF_ComponentsHTML.DateEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DateEditComponent.SELECTOR);
                    return date.valueText;
                case "enum":
                    var dd: YetaWF_ComponentsHTML.DropDownListEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DropDownListEditComponent.SELECTOR);
                    if (dd.value === "-1")
                        return null;
                    return dd.value;
                default:
                    throw `Unexpected filter type ${col.FilterType} for column ${colIndex}`;
            }
        }
        private clearColSortValue(colIndex: number): void {
            var col = this.Setup.Columns[colIndex];
            switch (col.FilterType) {
                case null:
                    break;
                case "bool":
                    var dd: YetaWF_ComponentsHTML.DropDownListEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DropDownListEditComponent.SELECTOR);
                    dd.clear();
                    break;
                case "long":
                case "text":
                case "guid":
                    var edit = $YetaWF.getElementById(col.FilterId) as HTMLInputElement;
                    edit.value = "";
                    break;
                case "dynenum":
                    var dd: YetaWF_ComponentsHTML.DropDownListEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DropDownListEditComponent.SELECTOR);
                    dd.value = "-1";
                    break;
                case "decimal":
                    var dec: YetaWF_ComponentsHTML.DecimalEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DecimalEditComponent.SELECTOR);
                    dec.clear();
                    break;
                case "datetime":
                    var datetime: YetaWF_ComponentsHTML.DateTimeEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DateTimeEditComponent.SELECTOR);
                    datetime.clear();
                    break;
                case "date":
                    var date: YetaWF_ComponentsHTML.DateEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DateEditComponent.SELECTOR);
                    date.clear();
                    break;
                case "enum":
                    var dd: YetaWF_ComponentsHTML.DropDownListEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DropDownListEditComponent.SELECTOR);
                    dd.value = "-1";
                    break;
                default:
                    throw `Unexpected filter type ${col.FilterType} for column ${colIndex}`;
            }
        }
        private GetFilterOpString(op: FilterOptionEnum): string {
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
            throw `Unexpected filter op ${op}`;
        }
        // add/remove (static grid)
        private removeRecord(trElem: HTMLElement, recNum: number, colName: string) : void {
            if (!this.Setup.StaticData) throw "Static grids only";
            // get the message to display (if any)
            var message = this.Setup.DeletedMessage;
            if(message) {
                if (colName) {
                    var text = this.Setup.StaticData[recNum][colName];
                    message = message.format(text);
                }
            }

            // remove the record
            this.RemoveRecord(recNum);

            // show the message
            if(message)
                    $YetaWF.message(message);
        }
        private resequenceDelete(recNum: number): void {
            // resequence origin
            var trs = $YetaWF.getElementsBySelector("tr[data-origin]", [this.TBody]) as HTMLTableRowElement[];
            for (let tr of trs) {
                var orig = Number($YetaWF.getAttribute(tr, "data-origin"));
                if (orig >= recNum) {
                    $YetaWF.setAttribute(tr, "data-origin", (orig-1).toString());
                    // update all indexes for input/select fields to match record origin (TODO: check whether we should only update last index in field)
                    this.renumberFields(tr, orig, orig - 1);
                }
            }
        }
        private resequence(): void {
            // resequence origin
            var trs = $YetaWF.getElementsBySelector("tr[data-origin]", [this.TBody]) as HTMLTableRowElement[];
            var index = 0;
            for (let tr of trs) {
                var orig = Number($YetaWF.getAttribute(tr, "data-origin"));
                $YetaWF.setAttribute(tr, "data-origin", index.toString());
                // update all indexes for input/select fields to match record origin (TODO: check whether we should only update last index in field)
                this.renumberFields(tr, orig, index);
                ++index;
            }
        }
        private renumberFields(tr: HTMLTableRowElement, origNum: Number, newNum: Number) : void {
            var inps = $YetaWF.getElementsBySelector("input[name],select[name]", [tr]);
            for (let inp of inps) {
                var name = $YetaWF.getAttribute(inp, "name");
                name = name.replace(`[${origNum}]`, `[${newNum}]`);
                $YetaWF.setAttribute(inp, "name", name);
            }
        }

        public internalDestroy(): void {
            if (this.Setup.CanFilter) {
                // close all menus
                var menuDiv = $YetaWF.getElementById(`${this.ControlId}_menus`);
                var menus = $YetaWF.getElementsBySelector(".tg_fentry .k-menu", [menuDiv]);
                for (let menu of menus) {
                    var menuData = $(menu).data("kendoMenu");
                    menuData.destroy();
                }
                // remove all menus
                menuDiv.remove();
            }
        }

        // API
        // API
        // API

        public enable(enable: boolean): void {
            // TODO: This currently only works with jqueryui class
            $YetaWF.elementRemoveClass(this.Control, this.Setup.DisabledCss);
            if (!enable)
                $YetaWF.elementAddClass(this.Control, this.Setup.DisabledCss);
        }

        get FieldName(): string {
            return this.Setup.FieldName;
        }
        get StaticData(): any[] {
            if (!this.Setup.StaticData) throw "Static grids only";
            return this.Setup.StaticData;
        }
        get ExtraData(): any {
            return this.Setup.ExtraData;
        }
        public AddRecord(tr: string, staticData: any): void {
            if (!this.Setup.StaticData) throw "Static grids only";
            $YetaWF.appendMixedHTML(this.TBody, tr, true);
            var lastTr = this.TBody.lastChild as HTMLTableRowElement;
            var origin = this.Setup.StaticData.length;
            $YetaWF.setAttribute(lastTr, "data-origin", origin.toString());
            this.renumberFields(lastTr, 0, origin);
            this.Setup.StaticData.push(staticData);
            this.Setup.Records++;
            this.updatePage();
            this.reload(Math.max(0, this.Setup.Pages - 1));
            this.updateStatus();
        }
        public RemoveRecord(index: number): void {
            if (!this.Setup.StaticData) throw "Static grids only";
            if (index < 0 || index >= this.Setup.StaticData.length) throw `Index ${index} out of bounds`;
            var tr = $YetaWF.getElement1BySelector(`tr[data-origin='${index.toString()}']`, [this.TBody]);
            tr.remove();
            this.Setup.StaticData.splice(index, 1);
            this.Setup.Records--;
            this.resequenceDelete(index);
            this.updatePage();
            this.reload(Math.max(0, this.Setup.Pages - 1));
            this.updateStatus();
        }
        private moveRawRecord(sel: number, index: number): void { // tr index (not data-origin index)
            if (!this.Setup.StaticData) throw "Static grids only";
            if (sel < 0 || sel >= this.Setup.StaticData.length) throw `Index sel=${sel} out of bounds`;
            if (index < 0 || index > this.Setup.StaticData.length) throw `Index index=${index} out of bounds`;
            if (index === sel || index === sel + 1) return;// nothing to move

            var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]) as HTMLTableRowElement[];
            var selTr = trs[sel];
            // remove the static data record
            var data = this.Setup.StaticData[sel];
            this.Setup.StaticData.splice(sel, 1);
            // remove the table row element
            this.TBody.removeChild(selTr);
            // insert the static data record at the new position
            if (index > sel)--index;
            if (index >= this.Setup.StaticData.length) {
                this.Setup.StaticData.push(data);
                this.TBody.appendChild(selTr);
            } else {
                this.Setup.StaticData.splice(index, 0, data);
                this.TBody.insertBefore(selTr, this.TBody.children[index+1]); // take tg_empty into account
            }
            this.resequence();
            this.updatePage();
            this.updateStatus();
        }
        public SelectedIndex(): number {
            var sel = $YetaWF.getElement1BySelectorCond(`tr.${this.Setup.RowHighlightCss},tr.${this.Setup.RowDragDropHighlightCss}`, [this.TBody]);
            if (sel == null) return -1;
            var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]) as HTMLTableRowElement[];
            var rowIndex = Array.prototype.indexOf.call(trs, sel);
            return rowIndex;
        }
        public ClearSelection(): void {
            var sel = $YetaWF.getElement1BySelectorCond(`tr.${this.Setup.RowHighlightCss},tr.${this.Setup.RowDragDropHighlightCss}`, [this.TBody]);
            if (sel) {
                $YetaWF.elementToggleClass(sel, this.Setup.RowHighlightCss, false);
                $YetaWF.elementToggleClass(sel, this.Setup.RowDragDropHighlightCss, false);
            }
        }
        public GetTotalRecords(): number {
            return this.Setup.Records;
        }
        public GetRecord(index: number): any {
            if (!this.Setup.StaticData) throw "Static grids only";
            if (index < 0 || index >= this.Setup.StaticData.length) throw `Index ${index} out of bounds`;
            return this.Setup.StaticData[index];
        }
        public GetTR(index: number): HTMLTableRowElement {
            if (this.Setup.StaticData) throw "Ajax grids only";
            if (index < 0 || index >= this.TBody.children.length) throw `Index ${index} out of bounds`;
            return this.TBody.children[index] as HTMLTableRowElement;
        }
        public HitTest(x: number, y: number): number {
            if (!this.Setup.StaticData) throw "Static grids only";
            var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]) as HTMLTableRowElement[];
            var index = 0;
            for (let tr of trs) {
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
        }
        public HitTestInsert(x: number, y: number): number {
            if (!this.Setup.StaticData) throw "Static grids only";
            var trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]) as HTMLTableRowElement[];
            var index = 0;
            for (let tr of trs) {
                var rect = tr.getBoundingClientRect();
                if (x < rect.left || x > rect.left + rect.width)
                    return -1;
                if (y < rect.top)
                    return -1;
                if (y < rect.top + rect.height/2)
                    return index;
                ++index;
                if (y < rect.top + rect.height)
                    return index;
            }
            return -1;
        }
        /**
         * Reloads the grid in its entirety using the provided extradata. The extradata is only saved in the grid if reloading is successful.
         * The callback is called if the grid is successfully reloaded.
         */
        public ReloadAll(overrideExtraData?: any, successful?: () => void): void {
            if (this.Setup.StaticData) throw "Ajax grids only";
            this.reload(0, undefined, undefined, overrideExtraData, false, (): void => {
                // successful
                if (overrideExtraData)
                    this.Setup.ExtraData = overrideExtraData;
                if (successful)
                    successful();
            });
        }
        public static ReloadFromId(id: string):void {
            let grid = YetaWF.ComponentBaseDataImpl.getControlById<Grid>(id, Grid.SELECTOR);
            grid.reload(0);
        }
        /* Set all check boxes in a static grid control */
        public SetCheckBoxes(set: boolean): void {
            if (this.Setup.StaticData && this.Setup.NoSubmitContents) {
                this.SubmitCheckCol = this.getSubmitCheckCol();
                if (this.SubmitCheckCol >= 0) {
                    let checks = $YetaWF.getElementsBySelector(`td:nth-child(${this.SubmitCheckCol + 1}) input[type='checkbox']`, [this.Control]) as HTMLInputElement[];
                    for (let check of checks) {
                        if (!check.disabled) {
                            var tr = $YetaWF.elementClosest(check, "tr");
                            var recNum = Number($YetaWF.getAttribute(tr, "data-origin"));
                            this.Setup.StaticData[recNum][this.Setup.Columns[this.SubmitCheckCol].Name] = set;
                            check.checked = set;
                        }
                    }
                }
            }
        }
        /* returns whether all checkboxes are selected  in a static grid control */
        public GetAllCheckBoxesSelected(): boolean {
            if (this.Setup.StaticData && this.Setup.NoSubmitContents) {
                this.SubmitCheckCol = this.getSubmitCheckCol();
                if (this.SubmitCheckCol >= 0) {
                    let checks = $YetaWF.getElementsBySelector(`td:nth-child(${this.SubmitCheckCol + 1}) input[type='checkbox']`, [this.Control]) as HTMLInputElement[];
                    for (let check of checks) {
                        if (!check.disabled) {
                            var tr = $YetaWF.elementClosest(check, "tr");
                            var recNum = Number($YetaWF.getAttribute(tr, "data-origin"));
                            let set = this.Setup.StaticData[recNum][this.Setup.Columns[this.SubmitCheckCol].Name];
                            if (!set)
                                return false;
                        }
                    }
                    return true;
                }
            }
            throw "GetAllCheckBoxesSelected not available";
        }
    }
}
