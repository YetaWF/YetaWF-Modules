/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
        SizeStyle: SizeStyleEnum;
        Columns: GridColumnDefinition[];
        SaveSettingsColumnWidthsUrl: string;
        SaveSettingsColumnSelectionUrl: string;
        ExtraData: any;
        HighlightCss: string;
        DisabledCss: string;
        RowHighlightCss: string;
        RowDragDropHighlightCss: string;
        SortActiveCss: string;
        SettingsModuleGuid: string;
        HighlightOnClick: boolean;

        PanelHeaderSearchColumns: string[];

        DeletedMessage: string;
        DeleteConfirmationMessage: string;
        DeletedColumnDisplay: string;

        NoSubmitContents: boolean;
    }
    enum SizeStyleEnum {
        SizeGiven = 0,
        SizeToFit = 1,
        SizeAuto = 2,
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
        Visible: boolean;
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

    interface UserIdFilterData {
        UIHint: string;
        FilterOp: string;
        UserId: number;
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
        ColumnVisibility: boolean[];
    }
    enum FilterBoolEnum {
        All = 0,
        Yes = 1,
        No = 2
    }

    export class Grid extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_grid";
        public static readonly SELECTOR: string = ".yt_grid";
        public static readonly EVENTSELECT: string = "grid_selectionchange";
        public static readonly EVENTDBLCLICK: string = "grid_dblclick";
        public static readonly EVENTDRAGDROPDONE: string = "grid_dragdropdone";
        public static readonly EVENTDRAGDROPCANCEL: string = "grid_dragdropcancel";

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
        private SelectPanelPageSize: YetaWF_ComponentsHTML.DropDownListEditComponent | null = null;
        private InputPanelSearch: YetaWF_ComponentsHTML.SearchEditComponent | null = null;
        private ColumnSelection: YetaWF_ComponentsHTML.CheckListEditComponent | null = null;
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
            });

            this.Setup = setup;

            this.TBody = $YetaWF.getElement1BySelector("tbody", [this.Control]);
            this.convertToPix();

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
                    this.SelectPageSize = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond<DropDownListEditComponent>(".tg_pager select[name$='.__PageSelection']", DropDownListEditComponent.SELECTOR, [this.Control]);
                    this.SelectPanelPageSize = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond<DropDownListEditComponent>(".yGridPanelTitle select[name='__PageSelection']", DropDownListEditComponent.SELECTOR, [this.Control]);
                }
                if (SearchEditComponent) // searchedit may not be in use
                    this.InputPanelSearch = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond<SearchEditComponent>(".yGridPanelTitle [name='__Search']", SearchEditComponent.SELECTOR, [this.Control]);
                if (CheckListEditComponent) // checklist may not be in use
                    this.ColumnSelection = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond<CheckListEditComponent>(".yGridPanelTitle [name='__ColumnSelection']", CheckListEditComponent.SELECTOR, [this.Control]);
            }
            this.FilterBar = $YetaWF.getElement1BySelectorCond(".tg_filter", [this.Control]) as HTMLDivElement | null;

            this.updateStatus();

            $YetaWF.getElement1BySelector(".tg_table", [this.Control]).addEventListener("scroll", (ev: Event): any => {
                $YetaWF.sendContainerScrollEvent(this.Control);
                return true;
            });

            $YetaWF.registerEventHandler(this.Control, "mouseover", ".tg_resize", (ev: MouseEvent): boolean => {
                // don't allow mouseover to propagate and close tooltips
                $YetaWF.closeOverlays();
                return false;
            });
            $YetaWF.registerEventHandler(this.Control, "click", ".tg_resize", (ev: MouseEvent): boolean => {
                return false;
            });
            // Show/hide filter bar with search button
            if (this.Setup.CanFilter && this.BtnSearch && this.FilterBar) {
                $YetaWF.registerEventHandler(this.BtnSearch, "click", null, (ev: MouseEvent): boolean => {
                    let filterBar = this.FilterBar as HTMLElement;
                    if ($YetaWF.isVisible(filterBar)) {
                        filterBar.style.display = "none";
                        this.clearFilters();
                        if (this.InputPanelSearch)
                            this.InputPanelSearch.value = "";
                        this.reload(0);
                    } else {
                        filterBar.style.display = "";
                        $YetaWF.sendActivateDivEvent([filterBar]);
                    }
                    return false;
                });
            }
            // Reload
            if (this.BtnReload) {
                $YetaWF.registerEventHandler(this.BtnReload, "click", null, (ev: MouseEvent): boolean => {
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
                    if (this.Setup.Page >= 0)
                        this.reload(0);
                    return false;
                });
            }
            if (this.BtnPrev) {
                $YetaWF.registerEventHandler(this.BtnPrev, "click", null, (ev: MouseEvent): boolean => {
                    let page = this.Setup.Page - 1;
                    if (page >= 0)
                        this.reload(page);
                    return false;
                });
            }
            if (this.BtnNext) {
                $YetaWF.registerEventHandler(this.BtnNext, "click", null, (ev: MouseEvent): boolean => {
                    let page = this.Setup.Page + 1;
                    if (page < this.Setup.Pages)
                        this.reload(page);
                    return false;
                });
            }
            if (this.BtnBottom) {
                $YetaWF.registerEventHandler(this.BtnBottom, "click", null, (ev: MouseEvent): boolean => {
                    let page = this.Setup.Pages - 1;
                    if (page >= 0)
                        this.reload(page);
                    return false;
                });
            }
            // Page input
            if (this.InputPage) {
                $YetaWF.registerEventHandler(this.InputPage.Control, "keydown", null, (ev: KeyboardEvent): boolean => {
                    if (ev.keyCode === 13 && this.InputPage) { // Return
                        let page = this.InputPage.value - 1;
                        this.reload(page);
                        return false;
                    }
                    return true;
                });
            }
            //  search input
            if (this.InputPanelSearch && this.Setup.PanelHeaderSearchColumns) {
                this.InputPanelSearch.Control.addEventListener(SearchEditComponent.EVENTCLICK, (evt: Event): void => {
                    this.clearFilters();
                    this.reload(0, undefined, undefined, undefined, undefined, undefined, this.Setup.PanelHeaderSearchColumns, this.InputPanelSearch!.value);
                });
            }
            // column selection
            if (this.Setup.SettingsModuleGuid && this.ColumnSelection) {
                this.ColumnSelection.Control.addEventListener(CheckListEditComponent.EVENTCHANGE, (evt: Event): void => {

                    let uri = $YetaWF.parseUrl(this.Setup.SaveSettingsColumnSelectionUrl);
                    uri.addSearch("SettingsModuleGuid", this.Setup.SettingsModuleGuid);

                    // build query args
                    let entries = this.ColumnSelection!.getValueEntries();
                    let colOffIndex = 0;
                    let colOnIndex = 0;
                    let colIndex = 0;
                    for (let entry of entries) {
                        if (entry.Checked) {
                            if (!this.Setup.Columns[colIndex].Visible)
                                uri.addSearch(`ColumnsOn[${colOnIndex++}]`, entry.Name);
                        } else {
                            if (this.Setup.Columns[colIndex].Visible)
                                uri.addSearch(`ColumnsOff[${colOffIndex++}]`, entry.Name);
                        }
                        colIndex++;
                    }

                    let request: XMLHttpRequest = new XMLHttpRequest();
                    request.open("POST", this.Setup.SaveSettingsColumnSelectionUrl, true);
                    request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
                    request.onreadystatechange = (ev: Event): any => {
                        if (request.readyState === 4 /*DONE*/) {
                            this.setReloading(false);
                            $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, undefined, undefined, (result: string): void => {
                                if (!this.Setup.StaticData) {
                                    this.reload(0);
                                } else {
                                    let colVis:boolean[] = [];
                                    for (let entry of entries)
                                        colVis.push(entry.Checked);
                                    this.updateColumnHeaders(colVis);
                                }
                            });
                        }
                    };
                    request.send(uri.toFormData());
                });
            }
            // pagesize selection
            if (this.SelectPageSize) {
                this.SelectPageSize.Control.addEventListener(DropDownListEditComponent.EVENTCHANGE, (evt: Event): void => {
                    if (this.SelectPageSize)
                        this.reload(0, Number(this.SelectPageSize.value));
                });
            }
            if (this.SelectPanelPageSize) {
                this.SelectPanelPageSize.Control.addEventListener(DropDownListEditComponent.EVENTCHANGE, (evt: Event): void => {
                    if (this.SelectPanelPageSize)
                        this.reload(0, Number(this.SelectPanelPageSize.value));
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
                        let colIndex = Array.prototype.indexOf.call((ev.__YetaWFElem.parentElement as HTMLElement).children, ev.__YetaWFElem);
                        if (colIndex < 0 || colIndex >= this.Setup.Columns.length) throw `Invalid column index ${colIndex} - max is ${this.Setup.Columns.length}`;/*DEBUG*/
                        let col = this.Setup.Columns[colIndex];
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
                    let button = ev.__YetaWFElem;
                    let filter = $YetaWF.elementClosest(button, ".tg_filter");
                    let head = $YetaWF.elementClosest(button, "th");
                    let colIndex = Array.prototype.indexOf.call(filter.children, head);
                    let ulElem = $YetaWF.getElementById(this.Setup.Columns[colIndex].MenuId);

                    if (!YetaWF_ComponentsHTML.MenuULComponent.closeMenus()) {
                        let menuDiv = ulElem.cloneNode(true) as HTMLUListElement;
                        menuDiv.id = `${ulElem.id}_live`;
                        $YetaWF.elementAddClass(menuDiv, "yt_grid_menu");
                        document.body.appendChild(menuDiv);
                        new YetaWF_ComponentsHTML.MenuULComponent(menuDiv.id, {
                            "Owner": this.FilterBar!,
                            "AutoOpen": true, "AutoRemove": true,
                            "AttachTo": button, "Dynamic": true,
                            "Click": (liElem: HTMLLIElement): void => {
                                this.menuSelected(liElem, colIndex);
                            },
                        });
                    }
                    return false;
                });
                $YetaWF.registerEventHandler(this.FilterBar, "mousedown", ".tg_fmenu", (ev: MouseEvent): boolean => {
                    return false;
                });

                $YetaWF.registerEventHandler(this.FilterBar, "click", ".tg_fclear", (ev: MouseEvent): boolean => {
                    let filter = $YetaWF.elementClosest(ev.__YetaWFElem, ".tg_filter");
                    let head = $YetaWF.elementClosest(ev.__YetaWFElem, "th");
                    let colIndex = Array.prototype.indexOf.call(filter.children, head);
                    this.clearColSortValue(colIndex);
                    this.reload(0);
                    if (this.InputPanelSearch)
                        this.InputPanelSearch.value = "";
                    return false;
                });
                this.addDirectFilterHandlers();
            }
            // Delete action (static only)
            if (this.Setup.StaticData) {
                $YetaWF.registerEventHandler(this.Control, "click", "[name='DeleteAction']", (ev: MouseEvent): boolean => {
                    if (!this.Setup.StaticData) return true;
                    // find the record number to delete
                    let trElem = $YetaWF.elementClosest(ev.__YetaWFElem, "tr");
                    let recNum = Number($YetaWF.getAttribute(trElem, "data-origin"));

                    let message = this.Setup.DeleteConfirmationMessage;
                    let colName = this.Setup.DeletedColumnDisplay;
                    if (message) {
                        if (colName) {
                            let text = this.Setup.StaticData[recNum][colName];
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
                return this.handleSelect(ev.__YetaWFElem, false);
            });
            $YetaWF.registerEventHandler(this.TBody, "dblclick", "tr:not(.tg_emptytr)", (ev: MouseEvent): boolean => {
                return this.handleSelect(ev.__YetaWFElem, true);
            });
            $YetaWF.registerEventHandler(this.TBody, "focusin", "tr:not(.tg_emptytr)", (ev: Event): boolean => {
                let elem = ev.__YetaWFElem;
                $YetaWF.elementToggleClass(elem, this.Setup.RowHighlightCss, true);
                return true;
            });
            $YetaWF.registerEventHandler(this.TBody, "focusout", "tr:not(.tg_emptytr)", (ev: Event): boolean => {
                let elem = ev.__YetaWFElem;
                $YetaWF.elementToggleClass(elem, this.Setup.RowHighlightCss, false);
                return true;
            });

            $YetaWF.registerEventHandler(this.Control, "keydown", null, (ev: KeyboardEvent): boolean => {
                if (!document.activeElement || document.activeElement.tagName !== "TR")
                    return true;
                if (this.Setup.HighlightOnClick) {
                    let key = ev.key;
                    if (key === "ArrowDown" || key === "Down") {
                        let index = this.SelectedIndex();
                        this.SetSelectedIndex(index < 0 ? 0 : ++index);
                        index = this.SelectedIndex();
                        if (index >= 0) this.GetTR(index).focus();
                        this.sendEventSelect();
                        return false;
                    } else if (key === "ArrowUp" || key === "Up") {
                        let index = this.SelectedIndex();
                        this.SetSelectedIndex(index < 0 ? this.GetTotalRecords() - 1 : --index);
                        index = this.SelectedIndex();
                        if (index >= 0) this.GetTR(index).focus();
                        this.sendEventSelect();
                        return false;
                    } else if (key === "Home") {
                        this.SetSelectedIndex(0);
                        let index = this.SelectedIndex();
                        if (index >= 0) this.GetTR(index).focus();
                        this.sendEventSelect();
                        return false;
                    } else if (key === "End") {
                        this.SetSelectedIndex(this.GetTotalRecords() - 1);
                        let index = this.SelectedIndex();
                        if (index >= 0) this.GetTR(index).focus();
                        this.sendEventSelect();
                        return false;
                    }
                }
                return true;
            });

            // Drag & drop
            $YetaWF.registerEventHandlerBody("mousemove", null, (ev: MouseEvent): boolean => {
                if (this.reorderingInProgress) {

                    //console.log("Reordering...")

                    let rect = this.TBody.getBoundingClientRect();
                    if (ev.clientX < rect.left || ev.clientX > rect.left + rect.width ||
                        ev.clientY < rect.top || ev.clientY > rect.top + rect.height) {

                        this.cancelDragDrop();
                        return true;
                    }
                    let sel = this.SelectedIndex();
                    if (sel < 0) {
                        this.cancelDragDrop();
                        return true;
                    }

                    let insert = this.HitTestInsert(ev.clientX, ev.clientY);
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
                        let tr = $YetaWF.elementClosest(ev.__YetaWFElem, "tr");
                        let recNum = Number($YetaWF.getAttribute(tr, "data-origin"));
                        let val = (ev.__YetaWFElem as HTMLInputElement).checked;
                        this.Setup.StaticData[recNum][this.Setup.Columns[this.SubmitCheckCol].Name] = val;
                        //$YetaWF.elementToggleClass(tr, YConfigs.Forms.CssFormNoSubmitContents, !val);
                        return false;
                    });
                }
            }
            $YetaWF.registerEventHandler(this.Control, "click",".yGridPanelExpColl button", (ev: MouseEvent): boolean => {
                if ($YetaWF.elementHasClass(this.Control, "t_expanded")) {
                    this.setExpandCollapseStatus(false);
                    return false;
                } else if ($YetaWF.elementHasClass(this.Control, "t_collapsed")) {
                    this.setExpandCollapseStatus(true);
                    return false;
                }
                return true;
            });
        }

        private saveExpandCollapseStatus(expanded: boolean) : void {
            // send save request, we don't care about the response
            let uri = $YetaWF.parseUrl("/YetaWF_ComponentsHTML/GridPanelSaveSettings/SaveExpandCollapse");
            uri.addSearch("SettingsModuleGuid", this.Setup.SettingsModuleGuid);
            uri.addSearch("Expanded", expanded.toString());

            let request: XMLHttpRequest = new XMLHttpRequest();
            request.open("POST", uri.toUrl(), true);
            request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
            request.send(/*uri.toFormData()*/);
        }
        private setExpandCollapseStatus(expand: boolean) : void {
            if (!expand && $YetaWF.elementHasClass(this.Control, "t_expanded")) {
                $YetaWF.elementRemoveClasses(this.Control, ["t_expanded", "t_collapsed"]);
                $YetaWF.elementAddClass(this.Control, "t_collapsed");
                this.saveExpandCollapseStatus(false);
            } else if (expand && $YetaWF.elementHasClass(this.Control, "t_collapsed")) {
                $YetaWF.elementRemoveClasses(this.Control, ["t_expanded", "t_collapsed"]);
                $YetaWF.elementAddClass(this.Control, "t_expanded");
                this.saveExpandCollapseStatus(true);
            }
        }

        // handle submit local data
        public handlePreSubmitLocalData(form: HTMLFormElement) : void {
            if (this.Setup.StaticData && this.Setup.NoSubmitContents) {
                this.submitLocalData(form);
            }
        }

        private sendEventDblClick(): void {
            $YetaWF.sendCustomEvent(this.Control, Grid.EVENTDBLCLICK);
        }
        private sendEventSelect(): void {
            $YetaWF.sendCustomEvent(this.Control, Grid.EVENTSELECT);
        }
        private sendEventDragDropDone(): void {
            $YetaWF.sendCustomEvent(this.Control, Grid.EVENTDRAGDROPDONE);
        }
        private sendEventDragDropCancel(): void {
            $YetaWF.sendCustomEvent(this.Control, Grid.EVENTDRAGDROPCANCEL);
        }

        // selection
        private handleSelect(clickedElem: HTMLElement, doubleClick: boolean): boolean {
            if (this.Setup.HighlightOnClick) {
                if (clickedElem.parentElement !== this.TBody) {
                    // something in a row was clicked (nested grid), find the real row
                    for (; ;) {
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
                    if (!doubleClick)
                        return true;
                } else {
                    let trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
                    for (let tr of trs)
                        $YetaWF.elementToggleClass(tr, this.Setup.RowHighlightCss, false);
                    $YetaWF.elementToggleClass(clickedElem, this.Setup.RowHighlightCss, true);
                }

                if (doubleClick)
                    this.sendEventDblClick();
                else
                    this.sendEventSelect();
                return true;
            }
            return true;
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

            this.sendEventDragDropCancel();
        }
        private doneDragDrop(): void {
            if (this.reorderingRowElement) {
                $YetaWF.elementToggleClass(this.reorderingRowElement, this.Setup.RowHighlightCss, true);
                $YetaWF.elementToggleClass(this.reorderingRowElement, this.Setup.RowDragDropHighlightCss, false);
                this.reorderingRowElement = null;
            }
            this.reorderingInProgress = false;
            //console.log("Reordering ended")

            this.sendEventDragDropDone();
        }
        // OnlySubmitWhenChecked
        private setInitialSubmitStatus(): void {
            if (!this.Setup.StaticData || !this.Setup.NoSubmitContents) return;
            //let trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
            //for (let tr of trs) {
            //    let recNum = Number($YetaWF.getAttribute(tr, "data-origin"));
            //    let val = this.Setup.StaticData[recNum][this.Setup.Columns[this.SubmitCheckCol].Name];
            //    $YetaWF.elementToggleClass(tr, YConfigs.Forms.CssFormNoSubmitContents, !val);
            //}
        }
        private getSubmitCheckCol(): number {
            let colIndex: number = -1;
            let cols = this.Setup.Columns.filter((col: GridColumnDefinition, index: number, cols: GridColumnDefinition[]): boolean => {
                if (!col.OnlySubmitWhenChecked) return false;
                colIndex = index;
                return true;
            });
            if (cols.length > 1)
                throw "More than one column marked OnlySubmitWhenChecked";
            return colIndex;
        }
        private submitLocalData(form: HTMLFormElement): void {
            if (!this.Setup.StaticData) return;
            let div = `<div class='${$YetaWF.Forms.DATACLASS}' style='display:none'>`;
            // retrieve all rows and add input/select fields to data div, resequence to make mvc serialization of lists work
            let trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
            let row = 0;
            let re1 = new RegExp("\\[[0-9]+\\]", "gim");
            for (let tr of trs) {
                let recNum = Number($YetaWF.getAttribute(tr, "data-origin"));
                let val = this.Setup.StaticData[recNum][this.Setup.Columns[this.SubmitCheckCol].Name];
                if (val) { // add record if the checkbox is selected
                    let copied = false;
                    let inputs = $YetaWF.getElementsBySelector("input,select", [tr]);
                    for (let input of inputs) {
                        let name = $YetaWF.getAttributeCond(input, "name");
                        if (name) {
                            let copy = input.cloneNode() as HTMLElement;
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
                form.insertAdjacentHTML("beforeend", div);
        }
        // sorting
        private clearSorts(): void {
            let colIndex = 0;
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
            let ths = $YetaWF.getElementsBySelector(".tg_header th", [this.Control]);
            let th = ths[colIndex];
            let asc = $YetaWF.getElement1BySelector(".tg_sorticon .tg_sortasc", [th]);
            let desc = $YetaWF.getElement1BySelector(".tg_sorticon .tg_sortdesc", [th]);
            let both = $YetaWF.getElement1BySelector(".tg_sorticon .tg_sortboth", [th]);
            $YetaWF.elementToggleClass(asc, this.Setup.SortActiveCss, sortBy === SortByEnum.Ascending);
            $YetaWF.elementToggleClass(desc, this.Setup.SortActiveCss, sortBy === SortByEnum.Descending);
            $YetaWF.elementToggleClass(both, this.Setup.SortActiveCss, sortBy === SortByEnum.NotSpecified);
        }
        private getSortColumn(): GridColumnDefinition | null {
            for (let col of this.Setup.Columns) {
                if (col.Sortable && col.Sort !== SortByEnum.NotSpecified)
                    return col;
            }
            return null;
        }
        // Resizing
        // Convert all ch units to pixels in column headers
        private convertToPix(): void {
            const avgChar = this.calcCharWidth();
            let ths = $YetaWF.getElementsBySelector(".tg_header th", [this.Control]);
            for (let th of ths) {
                let wstyle = th.style.width;
                if (wstyle.endsWith("ch")) {
                    let w = parseFloat(wstyle) + 2;// we'll add some for padding
                    w *= avgChar;
                    th.style.width = `${w}px`;
                }
            }
            if (this.Setup.SizeStyle === SizeStyleEnum.SizeGiven) {
                let total = 0;
                for (let th of ths) {
                    let w = parseFloat(th.style.width);
                    total += w;
                }
                let table = $YetaWF.getElement1BySelector("table", [this.Control]);
                table.style.width = `${total}px`;
            }
        }
        private calcCharWidth(): number {
            const text = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            let elem = <div style="position:absolute;visibility:hidden;white-space:nowrap">{text}</div> as HTMLElement;

            // copy font settings
            let td = $YetaWF.getElement1BySelector(".tg_table table td", [this.Control]);// there is always a td element, even if it's empty
            let style = window.getComputedStyle(td);
            elem.style.font = style.font;
            elem.style.fontStyle = style.fontStyle;
            elem.style.fontWeight = style.fontWeight;
            elem.style.fontSize = style.fontSize;

            document.body.appendChild(elem);
            let width = elem.clientWidth / text.length;
            elem.remove();
            return width;
        }


        private static resizeColumn(ev: MouseEvent): boolean {
            let currentControl = Grid.CurrentControl;
            if (currentControl && currentControl.ColumnResizeHeader) {
                let rect = currentControl.ColumnResizeHeader.getBoundingClientRect();
                let actualWidth = rect.width;
                let newActualWidth = ev.clientX - rect.left;
                let givenWidth = Number(currentControl.ColumnResizeHeader.style.width.replace("px",""));
                let diff = newActualWidth - actualWidth; // <0 shring, >0 expand
                let newGivenWidth = givenWidth + diff;
                currentControl.ColumnResizeHeader.style.width = `${newGivenWidth}px`;
            }
            return false;
        }
        private static resizeColumnDone(ev: MouseEvent): boolean {
            let currentControl = Grid.CurrentControl;
            if (currentControl && currentControl.ColumnResizeBar && currentControl.ColumnResizeHeader && currentControl.Setup.SaveSettingsColumnWidthsUrl) {
                document.body.style.cursor = "default";
                window.removeEventListener("mousemove", this.resizeColumn, false);
                window.removeEventListener("mouseup", this.resizeColumnDone, false);

                // save column widths after user resizes
                if (currentControl.Setup.SettingsModuleGuid) {
                    // send save request, we don't care about the response
                    let uri = $YetaWF.parseUrl(currentControl.Setup.SaveSettingsColumnWidthsUrl);
                    uri.addSearch("SettingsModuleGuid", currentControl.Setup.SettingsModuleGuid);
                    let colIndex = Array.prototype.indexOf.call((currentControl.ColumnResizeHeader.parentElement as HTMLElement).children, currentControl.ColumnResizeHeader);
                    uri.addSearch("Columns[0].Key", currentControl.Setup.Columns[colIndex].Name);
                    uri.addSearch("Columns[0].Value", parseInt(currentControl.ColumnResizeHeader.style.width, 10));

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

            ev.preventDefault();
            ev.stopPropagation();
            return false;
        }

        // reloading
        private reload(page: number, newPageSize?: number, overrideColFilter?: OverrideColumnFilter, overrideExtraData?: any|null|undefined, sort?: boolean, done?: () => void, searchCols?: string[], searchText?: string): void {

            if (!this.reloadInProgress) {

                this.setReloading(true);

                if (page < 0) page = 0;

                if (this.Setup.StaticData && !sort) {
                    // show/hide selected rows
                    if (this.Setup.PageSize > 0) {
                        let trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
                        for (let tr of trs) {
                            tr.setAttribute("style", "display:none");
                        }
                        let len = trs.length;
                        let count = 0;
                        for (let i = page * this.Setup.PageSize; i < len; ++i) {
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
                    this.setExpandCollapseStatus(true);
                } else {
                    // fetch data from servers
                    let uri = $YetaWF.parseUrl(this.Setup.AjaxUrl);
                    uri.addSearch("fieldPrefix", this.Setup.FieldName);
                    uri.addSearch("skip", page * this.Setup.PageSize);
                    uri.addSearch("take", newPageSize || this.Setup.PageSize);
                    if (this.Setup.ExtraData)
                        uri.addSearchSimpleObject(this.Setup.ExtraData);
                    // sort order
                    let col = this.getSortColumn();
                    if (col) {
                        uri.addSearch("sort[0].field", col.Name);
                        uri.addSearch("sort[0].order", (col.Sort === SortByEnum.Descending ? 1 : 0));
                        // also add as "sorts" for controllers that prefer this name
                        uri.addSearch("sorts[0].field", col.Name);
                        uri.addSearch("sorts[0].order", (col.Sort === SortByEnum.Descending ? 1 : 0));
                    }
                    // filters
                    if (searchCols) {
                        let fcount = 0;
                        for (let searchCol of searchCols) {
                            if (searchText) {
                                uri.addSearch(`filters[${fcount}].field`, searchCol);
                                uri.addSearch(`filters[${fcount}].operator`, "Contains");
                                uri.addSearch(`filters[${fcount}].valueAsString`, searchText);
                                ++fcount;
                            }
                        }
                        if (fcount > 0)
                            uri.addSearch("Search", "True");
                    } else {
                        let colIndex = 0;
                        let fcount = 0;
                        for (let col of this.Setup.Columns) {
                            let val = this.getColSortValue(colIndex);
                            if (val) {
                                if (col.FilterType === "complex") {
                                    uri.addSearch(`filters[${fcount}].field`, col.Name);
                                    uri.addSearch(`filters[${fcount}].operator`, "Complex");
                                    uri.addSearch(`filters[${fcount}].valueAsString`, val);
                                    ++fcount;
                                } else {
                                    let oper = col.FilterOp;
                                    if (overrideColFilter && overrideColFilter.ColIndex === colIndex)
                                        oper = overrideColFilter.FilterOp;
                                    if (oper != null) {
                                        uri.addSearch(`filters[${fcount}].field`, col.Name);
                                        uri.addSearch(`filters[${fcount}].operator`, this.GetFilterOpString(oper));
                                        uri.addSearch(`filters[${fcount}].valueAsString`, val);
                                        ++fcount;
                                    }
                                }
                            }
                            ++colIndex;
                        }
                    }
                    uri.addFormInfo(this.Control);
                    let uniqueIdCounters: YetaWF.UniqueIdInfo = { UniqueIdPrefix: `${this.ControlId}gr`, UniqueIdPrefixCounter: 0, UniqueIdCounter: 0 };
                    uri.addSearch(YConfigs.Forms.UniqueIdCounters, JSON.stringify(uniqueIdCounters));

                    if (this.Setup.StaticData)
                        uri.addSearch("data", JSON.stringify(this.Setup.StaticData));

                    let request: XMLHttpRequest = new XMLHttpRequest();
                    request.open("POST", this.Setup.AjaxUrl);
                    request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                    request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
                    request.onreadystatechange = (ev: Event): any => {
                        if (request.readyState === 4 /*DONE*/) {
                            this.setReloading(false);
                            $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, undefined, undefined, (result: string): void => {
                                let partial: GridPartialResult = JSON.parse(request.responseText);
                                $YetaWF.processClearDiv(this.TBody);
                                this.TBody.innerHTML = "";
                                $YetaWF.appendMixedHTML(this.TBody, partial.TBody, true);
                                // We have to update column headers based on the data we got in case of a reload as the user may have multiple windows for the same session
                                this.updateColumnHeaders(partial.ColumnVisibility);// for visibility
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
                                this.setExpandCollapseStatus(true);
                                this.sendEventSelect();
                            });
                        }
                    };
                    let data = uri.toFormData();
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
        private updateColumnHeaders(columnVisibility: boolean[]): void {
            // show/hide columns
            let thsH = $YetaWF.getElementsBySelector(".tg_header th", [this.Control]);
            let thsF = $YetaWF.getElementsBySelector(".tg_filter th", [this.Control]);
            if (this.Setup.StaticData) {
                let trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]);
                for (let tr of trs) {
                    let tds = $YetaWF.getElementsBySelector("td", [tr]);
                    let colIndex = 0;
                    for (let entry of columnVisibility) {
                        if (!entry) {
                            tds[colIndex].style.display = "none";
                        } else {
                            tds[colIndex].style.display = "";
                        }
                        ++colIndex;
                    }
                }
            }
            let colIndex = 0;
            for (let entry of columnVisibility) {
                this.Setup.Columns[colIndex].Visible = entry;
                if (!entry) {
                    if (thsH.length)
                        thsH[colIndex].style.display = "none";
                    if (thsF.length)
                        thsF[colIndex].style.display = "none";
                } else {
                    if (thsH.length)
                        thsH[colIndex].style.display = "";
                    if (thsF.length)
                        thsF[colIndex].style.display = "";
                }
                ++colIndex;
            }
            if (this.ColumnSelection)
                this.ColumnSelection!.replaceValues(columnVisibility);
        }
        private updateStatus(): void {
            if (this.PagerTotals) {
                let totals: string;
                if (this.Setup.Records === 0)
                    totals = YLocs.YetaWF_ComponentsHTML.GridTotalNone;
                else {
                    if (this.Setup.PageSize === 0) {
                        let first = 1;
                        let last = this.Setup.Records;
                        if (first > last)
                            totals = YLocs.YetaWF_ComponentsHTML.GridTotal0.format(this.Setup.Records);
                        else
                            totals = YLocs.YetaWF_ComponentsHTML.GridTotals.format(first, last, this.Setup.Records);
                    } else {
                        let first = this.Setup.Page * this.Setup.PageSize + 1;
                        let last = first + this.Setup.PageSize - 1;
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
            if (this.BtnTop) $YetaWF.elementEnableToggle(this.BtnTop, this.Setup.Page > 0);
            if (this.BtnPrev) $YetaWF.elementEnableToggle(this.BtnPrev, this.Setup.Page > 0);
            if (this.BtnNext) $YetaWF.elementEnableToggle(this.BtnNext, this.Setup.Page < this.Setup.Pages - 1);
            if (this.BtnBottom) $YetaWF.elementEnableToggle(this.BtnBottom, this.Setup.Page < this.Setup.Pages - 1);

            // show/hide "No Records"
            if (this.Setup.StaticData) {
                if (this.Setup.Records === 0) {
                    $YetaWF.getElement1BySelector("tr.tg_emptytr", [this.TBody]).style.display = "";
                } else {
                    $YetaWF.getElement1BySelector("tr.tg_emptytr", [this.TBody]).style.display = "none";
                }
            }

            // show hide filter clear buttons

            if (this.Setup.CanFilter && this.FilterBar) {
                let colIndex = 0;
                for (let col of this.Setup.Columns) {
                    if (col.FilterId) {
                        let val = this.getColSortValue(colIndex);
                        let elem = $YetaWF.getElementById(col.FilterId) as HTMLInputElement;// the value element
                        let fentry = $YetaWF.elementClosest(elem, ".tg_fentry");// the container for this filter
                        let btn = $YetaWF.getElement1BySelectorCond(".tg_fclear", [fentry]);// clear button
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
        }
        private updatePage(): void {
            if (this.Setup.PageSize > 0)
                this.Setup.Pages = Math.max(1, Math.floor((this.Setup.Records - 1) / this.Setup.PageSize) + 1);
        }

        // Filtering
        private clearFilters(): void {
            let colIndex = 0;
            for (let {} of this.Setup.Columns) {
                this.clearColSortValue(colIndex);
                ++colIndex;
            }
            this.updateStatus();
        }
        private clearFilterMenuHighlights(ulElem: HTMLElement): void {
            let menuLis = $YetaWF.getElementsBySelector(`li.${this.Setup.HighlightCss}`, [ulElem]);
            for (let menuLi of menuLis)
                $YetaWF.elementRemoveClass(menuLi, this.Setup.HighlightCss);
        }
        public menuSelected(menuElem: HTMLElement, colIndex: number): void {
            // update column structure
            let sel = Number($YetaWF.getAttribute(menuElem, "data-sel"));
            // new filter
            let overrideColFilter: OverrideColumnFilter = {
                ColIndex: colIndex,
                FilterOp: sel
            };
            this.reload(0, undefined, overrideColFilter, undefined, undefined, (): void => {
                // clear all highlights
                let ulElem = $YetaWF.elementClosest(menuElem, "ul");
                this.clearFilterMenuHighlights(ulElem);
                // highlight new selection
                $YetaWF.elementToggleClass(menuElem, this.Setup.HighlightCss, true);
                // update button with new sort icon
                if (this.FilterBar) {
                    let icon = $YetaWF.getElement1BySelector(".t_fmenuicon", [menuElem]).innerHTML;
                    let thsFilter = $YetaWF.getElementsBySelector("th", [this.FilterBar]);
                    let btn = $YetaWF.getElement1BySelector(".tg_fmenu", [thsFilter[colIndex]]);
                    btn.innerHTML = icon;
                }
                // update column structures
                this.Setup.Columns[colIndex].FilterOp = sel;
            });
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
                        $YetaWF.registerCustomEventHandlerDocument(DropDownListEditComponent.EVENTCHANGE, `#${col.FilterId}`, (ev: Event): boolean => {
                            this.reload(0);
                            if (this.InputPanelSearch)
                                this.InputPanelSearch.value = "";
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
                        let elem = $YetaWF.getElementById(col.FilterId);
                        $YetaWF.registerEventHandler(elem, "keydown", null, (ev: KeyboardEvent): boolean => {
                            if (ev.keyCode === 13) { // Return
                                this.reload(0);
                                if (this.InputPanelSearch)
                                    this.InputPanelSearch.value = "";
                                return false;
                            }
                            return true;
                        });
                        break;
                    }
                    case "complex": {
                        // handle invoking popup
                        let elem = $YetaWF.getElementById(col.FilterId) as HTMLInputElement;// the value element
                        let fctrls = $YetaWF.elementClosest(elem, ".tg_fctrls");// the container for this filter
                        let ffilter = $YetaWF.getElement1BySelector(".tg_ffilter", [fctrls]);// the filter button
                        let urlElem = $YetaWF.getElement1BySelector("input[name='Url']", [fctrls]) as HTMLInputElement;
                        $YetaWF.registerEventHandler(ffilter, "click", null, (ev: Event): boolean => {
                            let url = urlElem.value;
                            // invoke popup passing the data and filterid as arguments
                            let uri = new YetaWF.Url();
                            uri.parse(url);
                            uri.removeSearch("FilterId");
                            uri.addSearch("FilterId", col.FilterId);
                            uri.removeSearch("Data");
                            uri.addSearch("Data", elem.value);
                            if ($YetaWF.Popups.openPopup(uri.toUrl(), false, true))
                                return false;
                            return true;
                        });
                        break;
                    }
                }
            }
        }
        public static updateComplexFilter(filterId: string, data: UserIdFilterData | null): void {
            let elem = $YetaWF.getElementById(filterId) as HTMLInputElement;// the value element
            if (data) {
                let fctrls = $YetaWF.elementClosest(elem, ".tg_fctrls");// the container for this filter
                let uiHint = $YetaWF.getElement1BySelector("input[name='UIHint']", [fctrls]) as HTMLInputElement;// uihint name
                data.UIHint = uiHint.value;// set the uiHint
                elem.value = JSON.stringify(data);
            } else
                elem.value = "";
            let grid: Grid = YetaWF.ComponentBaseDataImpl.getControlFromTag(elem, YetaWF_ComponentsHTML.Grid.SELECTOR);
            if (grid.InputPanelSearch)
                grid.InputPanelSearch.value = "";
            grid.reload(0);
        }

        private getColSortValue(colIndex: number): string | null {
            let col = this.Setup.Columns[colIndex];
            switch (col.FilterType) {
                case null:
                    return null;
                case "bool": {
                    let dd: YetaWF_ComponentsHTML.DropDownListEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DropDownListEditComponent.SELECTOR);
                    let boolVal: FilterBoolEnum = Number(dd.value);
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
                    let edit = $YetaWF.getElementById(col.FilterId) as HTMLInputElement;
                    return edit.value;
                }
                case "dynenum": {
                    let dd: YetaWF_ComponentsHTML.DropDownListEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DropDownListEditComponent.SELECTOR);
                    if (dd.value === "-1")
                        return null;
                    return dd.value;
                }
                case "decimal":
                    let dec: YetaWF_ComponentsHTML.DecimalEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DecimalEditComponent.SELECTOR);
                    return dec.valueText;
                case "datetime":
                    let datetime: YetaWF_ComponentsHTML.DateTimeEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DateTimeEditComponent.SELECTOR);
                    return datetime.valueText;
                case "date":
                    let date: YetaWF_ComponentsHTML.DateEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DateEditComponent.SELECTOR);
                    return date.valueText;
                case "enum": {
                    let dd: YetaWF_ComponentsHTML.DropDownListEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DropDownListEditComponent.SELECTOR);
                    if (dd.value === "-1")
                        return null;
                    return dd.value;
                }
                case "complex": {
                    let edit = $YetaWF.getElementById(col.FilterId) as HTMLInputElement;
                    return edit.value;
                }
                default:
                    throw `Unexpected filter type ${col.FilterType} for column ${colIndex}`;
            }
        }
        private clearColSortValue(colIndex: number): void {
            let col = this.Setup.Columns[colIndex];
            switch (col.FilterType) {
                case null:
                    break;
                case "bool": {
                    let dd: YetaWF_ComponentsHTML.DropDownListEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DropDownListEditComponent.SELECTOR);
                    dd.clear();
                    break;
                }
                case "long":
                case "text":
                case "guid": {
                    let edit = $YetaWF.getElementById(col.FilterId) as HTMLInputElement;
                    edit.value = "";
                    break;
                }
                case "dynenum": {
                    let dd: YetaWF_ComponentsHTML.DropDownListEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DropDownListEditComponent.SELECTOR);
                    dd.value = "-1";
                    break;
                }
                case "decimal":
                    let dec: YetaWF_ComponentsHTML.DecimalEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DecimalEditComponent.SELECTOR);
                    dec.clear();
                    break;
                case "datetime":
                    let datetime: YetaWF_ComponentsHTML.DateTimeEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DateTimeEditComponent.SELECTOR);
                    datetime.clear();
                    break;
                case "date":
                    let date: YetaWF_ComponentsHTML.DateEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DateEditComponent.SELECTOR);
                    date.clear();
                    break;
                case "enum": {
                    let dd: YetaWF_ComponentsHTML.DropDownListEditComponent = YetaWF.ComponentBaseDataImpl.getControlById(col.FilterId, DropDownListEditComponent.SELECTOR);
                    dd.value = "-1";
                    break;
                }
                case "complex": {
                    let edit = $YetaWF.getElementById(col.FilterId) as HTMLInputElement;
                    edit.value = "";
                    break;
                }
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
            let message = this.Setup.DeletedMessage;
            if(message) {
                if (colName) {
                    let text = this.Setup.StaticData[recNum][colName];
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
            let trs = $YetaWF.getElementsBySelector("tr[data-origin]", [this.TBody]) as HTMLTableRowElement[];
            for (let tr of trs) {
                let orig = Number($YetaWF.getAttribute(tr, "data-origin"));
                if (orig >= recNum) {
                    $YetaWF.setAttribute(tr, "data-origin", (orig-1).toString());
                    // update all indexes for input/select fields to match record origin (TODO: check whether we should only update last index in field)
                    this.renumberFields(tr, orig, orig - 1);
                }
            }
        }
        private resequence(): void {
            // resequence origin
            let trs = $YetaWF.getElementsBySelector("tr[data-origin]", [this.TBody]) as HTMLTableRowElement[];
            let index = 0;
            for (let tr of trs) {
                let orig = Number($YetaWF.getAttribute(tr, "data-origin"));
                $YetaWF.setAttribute(tr, "data-origin", index.toString());
                // update all indexes for input/select fields to match record origin (TODO: check whether we should only update last index in field)
                this.renumberFields(tr, orig, index);
                ++index;
            }
        }
        private renumberFields(tr: HTMLTableRowElement, origNum: number, newNum: number) : void {
            let inps = $YetaWF.getElementsBySelector("input[name],select[name]", [tr]);
            for (let inp of inps) {
                let name = $YetaWF.getAttribute(inp, "name");
                name = name.replace(`[${origNum}]`, `[${newNum}]`);
                $YetaWF.setAttribute(inp, "name", name);
            }
        }

        // API
        // API
        // API

        public enable(enable: boolean): void {
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
            let lastTr = this.TBody.lastChild as HTMLTableRowElement;
            let origin = this.Setup.StaticData.length;
            $YetaWF.setAttribute(lastTr, "data-origin", origin.toString());
            this.renumberFields(lastTr, 0, origin);
            this.Setup.StaticData.push(staticData);
            this.Setup.Records++;
            this.updatePage();
            this.reload(Math.max(0, this.Setup.Pages - 1));
            this.updateStatus();
        }
        public AddRecords(trs: string[], staticData: any): void {
            if (!this.Setup.StaticData) throw "Static grids only";
            for (let tr of trs) {
                $YetaWF.appendMixedHTML(this.TBody, tr, true);
                let lastTr = this.TBody.lastChild as HTMLTableRowElement;
                let origin = this.Setup.StaticData.length;
                $YetaWF.setAttribute(lastTr, "data-origin", origin.toString());
                this.renumberFields(lastTr, 0, origin);
                this.Setup.StaticData.push(staticData);
                this.Setup.Records++;
            }
            this.updatePage();
            this.reload(Math.max(0, this.Setup.Pages - 1));
            this.updateStatus();
        }
        public ReplaceRecord(index: number, tr: string, staticData: any): void {
            if (!this.Setup.StaticData) throw "Static grids only";
            if (index < 0 || index >= this.Setup.StaticData.length) throw `Index ${index} out of bounds`;
            let trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]) as HTMLTableRowElement[];
            // insert the new tr
            let indexTr = trs[index];
            $YetaWF.insertMixedHTML(indexTr, tr, true);
            // remove the existing row element
            this.TBody.removeChild(indexTr);
            // renumber
            trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]) as HTMLTableRowElement[];
            indexTr = trs[index];
            this.renumberFields(indexTr, 0, index);
            $YetaWF.setAttribute(indexTr, "data-origin", index.toString());
            // replace the static data record
            this.Setup.StaticData[index] = staticData;
            this.updatePage();
            this.reload(Math.max(0, this.Setup.Pages - 1));
            this.updateStatus();
        }
        public RemoveRecord(index: number): void {
            if (!this.Setup.StaticData) throw "Static grids only";
            if (index < 0 || index >= this.Setup.StaticData.length) throw `Index ${index} out of bounds`;
            let tr = $YetaWF.getElement1BySelector(`tr[data-origin='${index.toString()}']`, [this.TBody]);
            tr.remove();
            this.Setup.StaticData.splice(index, 1);
            this.Setup.Records--;
            this.resequenceDelete(index);
            this.updatePage();
            this.reload(Math.max(0, this.Setup.Pages - 1));
            this.updateStatus();
        }
        public Clear(): void {
            if (!this.Setup.StaticData) throw "Static grids only";
            let trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]) as HTMLTableRowElement[];
            for (let tr of trs)
                tr.remove();
            this.Setup.StaticData = [];
            this.Setup.Records = 0;
            this.updatePage();
            this.reload(Math.max(0, this.Setup.Pages - 1));
            this.updateStatus();
        }
        private moveRawRecord(sel: number, index: number): void { // tr index (not data-origin index)
            if (!this.Setup.StaticData) throw "Static grids only";
            if (sel < 0 || sel >= this.Setup.StaticData.length) throw `Index sel=${sel} out of bounds`;
            if (index < 0 || index > this.Setup.StaticData.length) throw `Index index=${index} out of bounds`;
            if (index === sel || index === sel + 1) return;// nothing to move

            let trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]) as HTMLTableRowElement[];
            let selTr = trs[sel];
            // remove the static data record
            let data = this.Setup.StaticData[sel];
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
            let sel = $YetaWF.getElement1BySelectorCond(`tr.${this.Setup.RowHighlightCss},tr.${this.Setup.RowDragDropHighlightCss}`, [this.TBody]);
            if (sel == null) return -1;
            let trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]) as HTMLTableRowElement[];
            let rowIndex = Array.prototype.indexOf.call(trs, sel);
            return rowIndex;
        }
        public SetSelectedIndex(index: number): void {
            let trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]) as HTMLTableRowElement[];
            this.ClearSelection();
            if (index < 0 || index >= trs.length) return;
            $YetaWF.elementToggleClass(trs[index], this.Setup.RowHighlightCss, true);
        }
        public ClearSelection(): void {
            let sel = $YetaWF.getElement1BySelectorCond(`tr.${this.Setup.RowHighlightCss},tr.${this.Setup.RowDragDropHighlightCss}`, [this.TBody]);
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
            if (this.Setup.StaticData)
                ++index;// the first row in an <no records> indicator
            if (index < 0 || index >= this.TBody.children.length) throw `Index ${index} out of bounds`;
            return this.TBody.children[index] as HTMLTableRowElement;
        }
        public HitTest(x: number, y: number): number {
            if (!this.Setup.StaticData) throw "Static grids only";
            let trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]) as HTMLTableRowElement[];
            let index = 0;
            for (let tr of trs) {
                let rect = tr.getBoundingClientRect();
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
            let trs = $YetaWF.getElementsBySelector("tr:not(.tg_emptytr)", [this.TBody]) as HTMLTableRowElement[];
            let index = 0;
            for (let tr of trs) {
                let rect = tr.getBoundingClientRect();
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
        /**
         * Reloads the grid in its entirety using the provided data, extradata. The extradata is only saved in the grid if reloading is successful.
         * The callback is called if the grid is successfully reloaded.
         */
        public ReloadStatic(data: any, overrideExtraData?: any, successful?: () => void): void {
            if (!this.Setup.StaticData) throw "Static grids only";
            this.Clear();
            this.Setup.StaticData = data;
            this.reload(0, undefined, undefined, overrideExtraData, true/*sort forced rerendering*/, (): void => {
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
                            let tr = $YetaWF.elementClosest(check, "tr");
                            let recNum = Number($YetaWF.getAttribute(tr, "data-origin"));
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
                            let tr = $YetaWF.elementClosest(check, "tr");
                            let recNum = Number($YetaWF.getAttribute(tr, "data-origin"));
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

    // handle submit local data
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Forms.EVENTPRESUBMIT, null, (ev: CustomEvent<YetaWF.DetailsPreSubmit>): boolean => {
        let grids = YetaWF.ComponentBaseDataImpl.getControls<Grid>(Grid.SELECTOR, [ev.detail.form]);
        for (let grid of grids) {
            grid.handlePreSubmitLocalData(ev.detail.form);
        }
        return true;
    });
}
