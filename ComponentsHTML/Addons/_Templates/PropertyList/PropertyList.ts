/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

// copied from @types/masonry-layout (original has exports= which doesn't work, check future updates)
declare class Masonry {
    constructor(options?: Masonry.Options);
    constructor(selector: string | Element, options?: Masonry.Options);

    masonry?(): void;
    masonry?(eventName: string, listener: any): void;

    // layout
    layout?(): void;
    layoutItems?(items: any[], isStill?: boolean): void;
    stamp?(elements: any[]): void;
    unstamp?(elements: any[]): void;

    // add and remove items
    appended?(elements: any[]): void;
    prepended?(elements: any[]): void;
    addItems?(elements: any[]): void;
    remove?(elements: any[]): void;

    // events
    on?(eventName: string, listener: any): void;
    off?(eventName: string, listener: any): void;
    once?(eventName: string, listener: any): void;

    // utilities
    reloadItems?(): void;
    destroy?(): void;
    getItemElements?(): any[];
    data?(element: Element): Masonry;
}

declare namespace Masonry {
    interface Options {
        // layout
        itemSelector?: string;
        columnWidth?: any;
        percentPosition?: boolean;
        gutter?: any;
        stamp?: string;
        fitWidth?: boolean;
        originLeft?: boolean;
        originTop?: boolean;
        horizontalOrder?: boolean;

        // setup
        containerStyle?: {};
        transitionDuration?: any;
        resize?: boolean;
        initLayout?: boolean;
    }
}

namespace YetaWF_ComponentsHTML {

    interface ControlData {
        Id: string; // id of the property list div
        Controls: string[];
        Dependents: Dependent[];
    }
    interface Dependent {
        Prop: string; // Full name of property
        PropShort: string; // Name of property

        ProcessValues: ExprEntry[];
        HideValues: ExprEntry[];
    }
    interface ExprEntry {
        Op:YetaWF_ComponentsHTML.OpEnum;
        Disable: boolean;
        ExprList: YetaWF_ComponentsHTML.Expr[];
    }
    interface PropertyListSetup {
        Style: PropertyListStyleEnum;
        ColumnStyles: PropertyListColumnDef[];
        ExpandableList: string[];
        InitialExpanded: boolean;
    }
    enum PropertyListStyleEnum {
        Tabbed = 0,
        Boxed = 1,
        BoxedWithCategories = 2,
    }
    interface PropertyListColumnDef {
        MinWindowSize: number;
        Columns: number;
    }

    export class PropertyListComponent extends YetaWF.ComponentBaseDataImpl {

        private ControlData: ControlData | null;

        public static readonly TEMPLATE: string = "yt_propertylist";
        public static readonly SELECTOR: string = ".yt_propertylist";

        private Setup: PropertyListSetup;
        private MasonryElem: Masonry | null = null;
        private MinWidth: number = 0;
        private CurrWidth: number = 0;
        private ColumnDefIndex: number = -1;

        constructor(controlId: string, setup: PropertyListSetup, controlData: ControlData) {
            super(controlId, PropertyListComponent.TEMPLATE, PropertyListComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });

            this.ControlData = controlData;
            this.Setup = setup;

            // column handling
            if (this.Setup.Style === PropertyListStyleEnum.Boxed || this.Setup.Style === PropertyListStyleEnum.BoxedWithCategories) {
                this.MinWidth = this.Setup.ColumnStyles.length > 0 ? this.Setup.ColumnStyles[0].MinWindowSize : 0;
                if (this.Setup.InitialExpanded) {
                    let box = $YetaWF.getElement1BySelector(".t_propexpanded", [this.Control]);
                    this.expandBox(box);
                } else {
                    this.ColumnDefIndex = this.getColumnDefIndex();
                    if (this.ColumnDefIndex >= 0)
                        this.MasonryElem = this.createMasonry();
                }
                setInterval(() => {
                    if (this.MasonryElem)
                        this.MasonryElem.layout!();
                }, 1000);
            }

            // expand/collapse handling
            if (this.Setup.Style === PropertyListStyleEnum.Boxed || this.Setup.Style === PropertyListStyleEnum.BoxedWithCategories) {
                $YetaWF.registerEventHandler(this.Control, "click", ".t_boxexpcoll.t_show", (ev: Event): boolean => {
                    this.expandCollapseBox($YetaWF.elementClosest(ev.__YetaWFElem, ".t_proptable"));
                    return false;
                });
            }

            // Handle change events
            if (this.ControlData) {
            var controlData = this.ControlData;
                for (let control of controlData.Controls) {
                    let item = ControlsHelper.getControlItemByName(control, this.Control);
                    switch (item.ControlType) {
                        case ControlTypeEnum.Input:
                            $YetaWF.registerMultipleEventHandlers([(item.Template as HTMLInputElement)], ["change", "input"], null, (ev: Event): boolean => {
                                this.update();
                                return false;
                            });
                            break;
                        case ControlTypeEnum.Select:
                            $YetaWF.registerEventHandler((item.Template as HTMLSelectElement), "change", null, (ev: Event): boolean => {
                                this.update();
                                return false;
                            });
                            break;
                        case ControlTypeEnum.TextArea:
                            $YetaWF.registerMultipleEventHandlers([(item.Template as HTMLTextAreaElement)], ["change", "input"], null, (ev: Event): boolean => {
                                this.update();
                                return false;
                            });
                            break;
                        case ControlTypeEnum.Div:
                        case ControlTypeEnum.Hidden:
                            break;
                        default:
                        case ControlTypeEnum.Template:
                            if (!item.ChangeEvent)
                                throw `No ChangeEvent for control type ${item.ControlType}`;
                            let control = $YetaWF.getObjectData(item.Template) as YetaWF.ComponentBaseDataImpl;
                            $YetaWF.registerCustomEventHandler(control, item.ChangeEvent, (evt: Event): void => {
                                this.update();
                            });
                            break;
                    }
                }
            }

            // Initialize initial form
            this.update();

            $YetaWF.registerEventHandlerWindow("resize", null, (ev: UIEvent) => {
                if (this.MasonryElem) {
                    this.setLayout();
                    if (this.MasonryElem)
                        this.MasonryElem.layout!();
                } else {
                    this.setLayout();
                }
                return true;
            });

            $YetaWF.registerCustomEventHandler(this, "propertylist_relayout", (ev: Event): boolean => {
                this.setLayout();
                if (this.MasonryElem)
                    this.MasonryElem.layout!();
                return false;
            });
            /**
             * Collapse whichever box is expanded
             */
            $YetaWF.registerCustomEventHandler(this, "propertylist_collapse", (ev: Event): boolean => {
                this.setLayout();
                let box = $YetaWF.getElement1BySelectorCond(".t_propexpanded", [this.Control]);
                if (box) {
                    this.collapseBox(box);
                }
                if (this.MasonryElem)
                    this.MasonryElem.layout!();
                return false;
            });
        }

        private setLayout(): void {
            if (window.innerWidth < this.MinWidth) {
                this.destroyMasonry();
            } else if (!this.MasonryElem || window.innerWidth !== this.CurrWidth) {
                let newIndex = this.getColumnDefIndex();
                if (this.ColumnDefIndex !== newIndex) {
                    this.destroyMasonry();
                    this.MasonryElem = this.createMasonry();
                }
            }
            let haveExpandedBox = $YetaWF.getElement1BySelectorCond(".t_proptable .t_propexpanded");
            this.toggleFormButtons(!haveExpandedBox);
        }

        private expandCollapseBox(box: HTMLElement): void {

            let isExpanded = $YetaWF.elementHasClass(box, "t_propexpanded");

            this.destroyMasonry();

            if (!$YetaWF.elementHasClass(box, "t_propexpandable"))
                return;

            if (isExpanded) {
                // box can collapse
                this.collapseBox(box);

                this.MasonryElem = this.createMasonry();
            } else {
                // box can expand
                this.expandBox(box);
            }
        }
        private collapseBox(box: HTMLElement): void {
            let boxes = $YetaWF.getElementsBySelector(".t_proptable", [this.Control]);
            for (let b of boxes) {
                $YetaWF.elementRemoveClasses(b, ["t_propexpanded", "t_propcollapsed", "t_prophide"]);
                $YetaWF.elementAddClass(b, "t_propcollapsed");
                let expcoll = $YetaWF.getElement1BySelector(".t_boxexpcoll", [b]);
                $YetaWF.elementRemoveClasses(expcoll, ["t_hide", "t_show"]);
                $YetaWF.elementAddClass(expcoll, "t_show");
            }
            // show apply/save/cancel buttons again
            this.toggleFormButtons(true);
        }
        private expandBox(box: HTMLElement): void {
            let boxes = $YetaWF.getElementsBySelector(".t_proptable", [this.Control]);
            for (let b of boxes) {
                $YetaWF.elementRemoveClasses(b, ["t_propexpanded", "t_propcollapsed", "t_prophide"]);
                if (b !== box)
                    $YetaWF.elementAddClass(b, "t_prophide");
                let expcoll = $YetaWF.getElement1BySelector(".t_boxexpcoll", [b]);
                $YetaWF.elementRemoveClasses(expcoll, ["t_hide", "t_show"]);
            }
            $YetaWF.elementAddClass(box, "t_propexpanded");
            let expcoll = $YetaWF.getElement1BySelector(".t_boxexpcoll", [box]);
            $YetaWF.elementAddClass(expcoll, "t_show");

            // hide apply/save/cancel buttons while expanded
            this.toggleFormButtons(false);
        }

        private toggleFormButtons(show: boolean): void {
            let form = $YetaWF.Forms.getForm(this.Control);
            // make the form submit/nosubmit
            $YetaWF.elementRemoveClass(form, YConfigs.Forms.CssFormNoSubmit);
            if (!show)
                $YetaWF.elementAddClass(form, YConfigs.Forms.CssFormNoSubmit);
            // show/hide buttons
            let buttonList = $YetaWF.getElementsBySelector(".t_detailsbuttons", [form]);
            for (let buttons of buttonList)
                buttons.style.display = show ? "block" : "none";
        }

        private createMasonry(): Masonry {
            this.CurrWidth = window.innerWidth;
            this.ColumnDefIndex = this.getColumnDefIndex();
            let cols = this.Setup.ColumnStyles[this.ColumnDefIndex].Columns;
            $YetaWF.elementAddClass(this.Control, `t_col${cols}`);

            let boxes = $YetaWF.getElementsBySelector(".t_proptable", [this.Control]);
            for (let b of boxes) {
                $YetaWF.elementRemoveClasses(b, ["t_propexpanded", "t_propcollapsed", "t_prophide"]);
                $YetaWF.elementAddClass(b, "t_propcollapsed");
            }

            let expcolls = $YetaWF.getElementsBySelector(".t_proptable .t_boxexpcoll", [this.Control]);
            for (let expcoll of expcolls) {
                $YetaWF.elementRemoveClasses(expcoll, ["t_hide", "t_show"]);
                $YetaWF.elementAddClass(expcoll, "t_show");
            }

            return new Masonry(this.Control, {
                itemSelector: ".t_proptable",
                horizontalOrder: true,
                transitionDuration: "0.1s",
                resize: false,
                initLayout: true
                //columnWidth: 200
            });
        }
        private destroyMasonry(): void {
            if (this.MasonryElem) {
                this.CurrWidth = 0;
                this.ColumnDefIndex = -1;
                this.MasonryElem.destroy!();
                this.MasonryElem = null;

                $YetaWF.elementRemoveClass(this.Control, "t_col1");
                $YetaWF.elementRemoveClass(this.Control, "t_col2");
                $YetaWF.elementRemoveClass(this.Control, "t_col3");
                $YetaWF.elementRemoveClass(this.Control, "t_col4");
                $YetaWF.elementRemoveClass(this.Control, "t_col5");

            }
            let boxes = $YetaWF.getElementsBySelector(".t_proptable", [this.Control]);
            for (let b of boxes) {
                $YetaWF.elementRemoveClasses(b, ["t_propexpanded", "t_propcollapsed", "t_prophide"]);
                $YetaWF.elementAddClass(b, "t_propcollapsed");
            }

            let expcolls = $YetaWF.getElementsBySelector(".t_proptable .t_boxexpcoll", [this.Control]);
            for (let expcoll of expcolls) {
                $YetaWF.elementRemoveClasses(expcoll, ["t_show", "t_hide"]);
                $YetaWF.elementAddClass(expcoll, "t_hide");
            }
        }
        private getColumnDefIndex(): number {
            let width = window.innerWidth;
            let index = -1;
            for (let style of this.Setup.ColumnStyles) {
                if (width < style.MinWindowSize)
                     return index;
                 ++index;
            }
            return index;
        }

        /**
         * Update all dependent fields (forms).
         */
        public update(): void {

            if (!this.ControlData) return;

            let form = $YetaWF.Forms.getForm(this.Control);

            // for each dependent, verify that all its conditions are true
            var deps = this.ControlData.Dependents;
            for (let dep of deps) {

                var depRow = $YetaWF.getElement1BySelectorCond(`.t_row.t_${dep.PropShort.toLowerCase()}`, [this.Control]);// the propertylist row affected
                if (!depRow)
                    continue;

                var hidden = false;
                for (let expr of dep.HideValues) {// hidden hides only, it never makes it visible (use process for that instead)
                    switch (expr.Op) {
                        case OpEnum.HideIf:
                        case OpEnum.HideIfSupplied: {
                            let valid = ValidatorHelper.evaluateExpressionList(null, form, expr.Op, expr.ExprList);
                            if (valid) {
                                this.toggle(dep, depRow, false, false);
                                hidden = true;
                                break;
                            }
                            break;
                        }
                        case OpEnum.HideIfNot:
                        case OpEnum.HideIfNotSupplied: {
                            let valid = ValidatorHelper.evaluateExpressionList(null, form, expr.Op, expr.ExprList);
                            if (!valid) {
                                this.toggle(dep, depRow, false, false);
                                hidden = true;
                                break;
                            }
                            break;
                        }
                        default:
                            throw `Unexpected Op ${expr.Op} in update(HideValues)`;
                    }
                    if (hidden)
                        break;
                }
                if (!hidden) {
                    let process = true;
                    let disable = false;
                    if (dep.ProcessValues.length > 0) {
                        process = false;
                        for (let expr of dep.ProcessValues) {
                            switch (expr.Op) {
                                case OpEnum.ProcessIf:
                                case OpEnum.ProcessIfSupplied: {
                                    let v = ValidatorHelper.evaluateExpressionList(null, form, expr.Op, expr.ExprList);
                                    if (v)
                                        process = true;
                                    else
                                        disable = expr.Disable;
                                    break;
                                }
                                case OpEnum.ProcessIfNot:
                                case OpEnum.ProcessIfNotSupplied: {
                                    let v = ValidatorHelper.evaluateExpressionList(null, form, expr.Op, expr.ExprList);
                                    if (!v)
                                        process = true;
                                    else
                                        disable = expr.Disable;
                                    break;
                                }
                                default:
                                    throw `Unexpected Op ${expr.Op} in update(ProcessValues)`;
                            }
                            if (process)
                                break;
                        }
                    }
                    if (process) {
                        this.toggle(dep, depRow, true, false);
                    } else {
                        if (disable)
                            this.toggle(dep, depRow, true, true);
                        else
                            this.toggle(dep, depRow, false, false);
                    }
                }
            }
        }

        private toggle(dep: Dependent, depRow: HTMLElement, show: boolean, disable: boolean): void {
            $YetaWF.Forms.clearValidation(depRow);
            $YetaWF.elementRemoveClass(depRow, "t_disabled");
            $YetaWF.elementRemoveClass(depRow, "t_hidden");
            if (show) {
                if (disable)
                    $YetaWF.elementAddClass(depRow, "t_disabled");
                $YetaWF.processActivateDivs([depRow]);// init any controls that just became visible
            } else {
                $YetaWF.elementAddClass(depRow, "t_hidden");
            }

            let controlItemDef = ControlsHelper.getControlItemByNameCond(dep.Prop, depRow);// there may not be an actual control, just a row with displayed info
            if (controlItemDef) {
                if (show) {
                    ControlsHelper.enableToggle(controlItemDef, !disable);
                } else {
                    ControlsHelper.enableToggle(controlItemDef, false);
                }
            }
        }

        public static isRowVisible(tag: HTMLElement): boolean {
            var row = $YetaWF.elementClosestCond(tag, ".t_row");
            if (!row) return false;
            return !$YetaWF.elementHasClass(row, "t_hidden");
        }
        public static isRowEnabled(tag: HTMLElement): boolean {
            var row = $YetaWF.elementClosestCond(tag, ".t_row");
            if (!row) return false;
            return !$YetaWF.elementHasClass(row, "t_disabled");
        }

        public static relayout(container:HTMLElement): void {
            let ctrls = $YetaWF.getElementsBySelector(".yt_propertylist.t_boxedcat,.yt_propertylist.t_boxed", [container]);
            for (let ctrl of ctrls) {
                var event = document.createEvent("Event");
                event.initEvent("propertylist_collapse", false, true);
                ctrl.dispatchEvent(event);
            }
        }

        public static tabInitjQuery(tabCtrlId: string, activeTab: number, activeTabId: string):void {
            ComponentsHTMLHelper.MUSTHAVE_JQUERYUI();
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
        var list = $YetaWF.getElementsBySelector(".yt_propertylist.t_tabbed.t_jquery", [tag]);
        for (let el of list) {
            var tabsJq = $(el);
            if (!tabsJq) throw "No jquery ui object found";/*DEBUG*/
            tabsJq.tabs("destroy");
        }
        list = $YetaWF.getElementsBySelector(".yt_propertylist.t_tabbed.t_kendo", [tag]);
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

