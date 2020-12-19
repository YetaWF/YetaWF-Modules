/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
        ClearOnDisable: boolean;
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
                Enable: (control: PropertyListComponent, enable: boolean, clearOnDisable: boolean): void => {
                    // if this propertylist is within a template, delegate enable/disable to that template
                    let parentElem = control.Control.parentElement;
                    if (!parentElem)
                        return;
                    let template = YetaWF.ComponentBase.getTemplateFromTagCond(parentElem);
                    if (!template)
                        return;
                    let controlItem = ControlsHelper.getControlItemFromTemplate(template);
                    ControlsHelper.enableToggle(controlItem, enable, clearOnDisable);
                    control.update();// update all dependent fields
                    // don't submit contents if disabled
                    $YetaWF.elementRemoveClass(control.Control, YConfigs.Forms.CssFormNoSubmitContents);
                    if (!enable)
                        $YetaWF.elementAddClass(control.Control, YConfigs.Forms.CssFormNoSubmitContents);
                },
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
                                return true;
                            });
                            break;
                        case ControlTypeEnum.Select:
                            $YetaWF.registerEventHandler((item.Template as HTMLSelectElement), "change", null, (ev: Event): boolean => {
                                this.update();
                                return true;
                            });
                            break;
                        case ControlTypeEnum.TextArea:
                            $YetaWF.registerMultipleEventHandlers([(item.Template as HTMLTextAreaElement)], ["change", "input"], null, (ev: Event): boolean => {
                                this.update();
                                return true;
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
                            $YetaWF.registerCustomEventHandler(control.Control, item.ChangeEvent, null, (evt: Event): boolean => {
                                this.update();
                                return true;
                            });
                            break;
                    }
                }
            }

            // Initialize initial form
            this.update();
            this.resize();

            $YetaWF.registerCustomEventHandler(this.Control, "propertylist_relayout", null, (ev: Event): boolean => {
                this.layout();
                return false;
            });
            /**
             * Collapse whichever box is expanded
             */
            $YetaWF.registerCustomEventHandler(this.Control, "propertylist_collapse", null, (ev: Event): boolean => {
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
            let winRect = this.Control.getBoundingClientRect();
            if (winRect.width < this.MinWidth) {
                this.destroyMasonry();
            } else if (!this.MasonryElem || winRect.width !== this.CurrWidth) {
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

            $YetaWF.sendContainerResizeEvent(box);
            this.resize();
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

            $YetaWF.sendContainerResizeEvent(box);
        }

        private toggleFormButtons(show: boolean): void {
            let form = $YetaWF.Forms.getForm(this.Control);
            // make the form submit/nosubmit
            $YetaWF.elementRemoveClass(form, "yform-nosubmit");
            if (!show)
                $YetaWF.elementAddClass(form, "yform-nosubmit");
            // show/hide buttons
            let buttonList = $YetaWF.getElementsBySelector(".t_detailsbuttons", [form]);
            for (let buttons of buttonList)
                buttons.style.display = show ? "block" : "none";
        }

        private createMasonry(): Masonry {
            let winRect = this.Control.getBoundingClientRect();
            this.CurrWidth = winRect.width;
            this.ColumnDefIndex = this.getColumnDefIndex();
            let cols = this.Setup.ColumnStyles[this.ColumnDefIndex].Columns;
            $YetaWF.elementAddClass(this.Control, `t_col${cols}`);

            let boxes = $YetaWF.getElementsBySelector(".t_proptable", [this.Control]);
            for (let b of boxes) {
                $YetaWF.elementRemoveClasses(b, ["t_propexpanded", "t_propcollapsed"]);
                $YetaWF.elementAddClass(b, "t_propcollapsed");
            }

            let expcolls = $YetaWF.getElementsBySelector(".t_proptable .t_boxexpcoll", [this.Control]);
            for (let expcoll of expcolls) {
                $YetaWF.elementRemoveClasses(expcoll, ["t_hide", "t_show"]);
                $YetaWF.elementAddClass(expcoll, "t_show");
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
            let winRect = this.Control.getBoundingClientRect();
            let width = winRect.width;
            let index = -1;
            for (let style of this.Setup.ColumnStyles) {
                if (width < style.MinWindowSize)
                    return index;
                ++index;
            }
            return index;
        }

        /**
         * Update all dependent fields (forms). May be a propertylist embedded in another propertylist.
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
                                this.toggle(dep, depRow, false, false, false);
                                hidden = true;
                                break;
                            }
                            break;
                        }
                        case OpEnum.HideIfNot:
                        case OpEnum.HideIfNotSupplied: {
                            let valid = ValidatorHelper.evaluateExpressionList(null, form, expr.Op, expr.ExprList);
                            if (!valid) {
                                this.toggle(dep, depRow, false, false, false);
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
                    let clearOnDisable = false;
                    if (dep.ProcessValues.length > 0) {
                        process = false;
                        for (let expr of dep.ProcessValues) {
                            switch (expr.Op) {
                                case OpEnum.ProcessIf:
                                case OpEnum.ProcessIfSupplied: {
                                    let v = ValidatorHelper.evaluateExpressionList(null, form, expr.Op, expr.ExprList);
                                    if (v)
                                        process = true;
                                    else {
                                        disable = expr.Disable;
                                        clearOnDisable = expr.ClearOnDisable;
                                    }
                                    break;
                                }
                                case OpEnum.ProcessIfNot:
                                case OpEnum.ProcessIfNotSupplied: {
                                    let v = ValidatorHelper.evaluateExpressionList(null, form, expr.Op, expr.ExprList);
                                    if (!v)
                                        process = true;
                                    else {
                                        disable = expr.Disable;
                                        clearOnDisable = expr.ClearOnDisable;
                                    }
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
                        this.toggle(dep, depRow, true, false, false);
                    } else {
                        if (disable)
                            this.toggle(dep, depRow, true, true, clearOnDisable);
                        else
                            this.toggle(dep, depRow, false, false, false);
                    }
                }
            }
        }

        /**
         * Update all dependent fields (forms). Will find the outermost propertylist and update fields.
         */
        public static updatePropertyListsFromTag(elem: HTMLElement): void {
            let propertyList = this.getTopMostPropertyListFromTagCond(elem);
            if (!propertyList) return;

            propertyList.update();// update all dependent fields of topmost propertylist
            // and all contained property lists
            let proplists = $YetaWF.getElementsBySelector(PropertyListComponent.SELECTOR, [propertyList.Control]);
            for (let proplist of proplists) {
                let list = YetaWF.ComponentBaseDataImpl.getControlFromTag<PropertyListComponent>(proplist, PropertyListComponent.SELECTOR);
                list.update();
            }
        }

        private static getTopMostPropertyListFromTagCond(elem: HTMLElement): PropertyListComponent|null {
            let curr: HTMLElement | null = elem;
            let topMost: PropertyListComponent | null = null;
            for (; curr;) {
                let propertyList = YetaWF.ComponentBaseDataImpl.getControlFromTagCond<PropertyListComponent>(curr, PropertyListComponent.SELECTOR);
                if (!propertyList) break;
                topMost = propertyList;
                curr = propertyList.parentElement;
            }
            return topMost;
        }

        private toggle(dep: Dependent, depRow: HTMLElement, show: boolean, disable: boolean, clearOnDisable: boolean): void {

            // hides/shows rows (doesn't change the status or clear validation if the status is already correct)
            let clearVal = false;
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
                } else {
                    if ($YetaWF.elementHasClass(depRow, "t_disabled")) {
                        $YetaWF.elementRemoveClass(depRow, "t_disabled");
                        clearVal = true;
                    }
                }
                if (clearVal)
                    $YetaWF.sendActivateDivEvent([depRow]);// init any controls that just became visible
            } else {
                if (!$YetaWF.elementHasClass(depRow, "t_hidden")) {
                    $YetaWF.elementRemoveClass(depRow, "t_disabled");
                    $YetaWF.elementAddClass(depRow, "t_hidden");
                    clearVal = true;
                }
            }
            if (clearVal)
                $YetaWF.Forms.clearValidation(depRow);

            let controlItemDef = ControlsHelper.getControlItemByNameCond(dep.Prop, depRow);// there may not be an actual control, just a row with displayed info
            if (controlItemDef) {
                if (show) {
                    ControlsHelper.enableToggle(controlItemDef, !disable, clearOnDisable);
                } else {
                    ControlsHelper.enableToggle(controlItemDef, false, clearOnDisable);
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

        public static relayout(container: HTMLElement): void {
            let ctrls = $YetaWF.getElementsBySelector(".yt_propertylist.t_boxedcat,.yt_propertylist.t_boxed", [container]);
            for (let ctrl of ctrls) {
                let propertyList = YetaWF.ComponentBaseDataImpl.getControlFromTag<PropertyListComponent>(ctrl, PropertyListComponent.SELECTOR);
                propertyList.update();
                var event = document.createEvent("Event");
                event.initEvent("propertylist_collapse", false, true);
                ctrl.dispatchEvent(event);
            }
        }
        public expandBoxByCategory(name: string): void {
            let box = $YetaWF.getElement1BySelector(`.t_proptable.t_propexpandable.t_boxpanel-${name.toLocaleLowerCase()}`, [this.Control]);
            this.expandCollapseBox(box);
            this.update();
        }
        public hideBoxesByCategory(names: string[]): void {
            for (let name of names)
                this.hideBoxByCategory(name);
        }
        public hideBoxByCategory(name: string): void {
            let box = $YetaWF.getElement1BySelectorCond(`.t_proptable.t_propexpandable.t_boxpanel-${name.toLocaleLowerCase()}`, [this.Control]);
            if (!box) return;
            $YetaWF.elementRemoveClasses(box, ["t_propsuppress"]);
            $YetaWF.elementAddClasses(box, ["t_propsuppress", YConfigs.Forms.CssFormNoSubmitContents]);
        }
        public showBoxesByCategory(names: string[]): void {
            for (let name of names)
                this.showBoxByCategory(name);
        }
        public showBoxByCategory(name: string): void {
            let box = $YetaWF.getElement1BySelectorCond(`.t_proptable.t_propexpandable.t_boxpanel-${name.toLocaleLowerCase()}`, [this.Control]);
            if (!box) return;
            $YetaWF.elementRemoveClasses(box, ["t_propsuppress", YConfigs.Forms.CssFormNoSubmitContents]);
        }
        public layout(): void {
            this.setLayout();
            if (this.MasonryElem) {
                setTimeout((): void => {
                    if (this.MasonryElem)
                        this.MasonryElem.layout!();
                }, 20);
            }
        }
        public resize(): void {
            if (this.Setup.Style === PropertyListStyleEnum.Boxed || this.Setup.Style === PropertyListStyleEnum.BoxedWithCategories) {
                this.layout();
            }
        }
    }

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: CustomEvent<YetaWF.DetailsEventContainerResize>): boolean => {
        let proplists = $YetaWF.getElementsBySelector(PropertyListComponent.SELECTOR);
        for (let proplist of proplists) {
            if ($YetaWF.elementHas(ev.detail.container, proplist)) {
                let list = YetaWF.ComponentBaseDataImpl.getControlFromTag<PropertyListComponent>(proplist, PropertyListComponent.SELECTOR);
                list.resize();
            }
        }
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: CustomEvent<YetaWF.DetailsEventNavPageLoaded>): boolean => {
        let proplists = $YetaWF.getElementsBySelector(PropertyListComponent.SELECTOR, ev.detail.containers);
        for (let proplist of proplists) {
            let list = YetaWF.ComponentBaseDataImpl.getControlFromTag<PropertyListComponent>(proplist, PropertyListComponent.SELECTOR);
            list.resize();
        }
        return true;
    });
}

