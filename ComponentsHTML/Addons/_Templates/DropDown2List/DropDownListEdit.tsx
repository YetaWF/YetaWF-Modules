/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    //$$$ interface AjaxData {
    //    data: kendo.data.DataSource;
    //    tooltips: string[] | null;
    //}

    export class DropDown2ListEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_dropdown2list_base";
        public static readonly SELECTOR: string = ".yt_dropdown2list_base.t_edit";
        public static readonly EVENTCHANGE: string = "dropdown2list_change";

        public static readonly POPUPID: string = "yDDPopup";

        private Input: HTMLDivElement;
        private Select: HTMLSelectElement;
        private Container: HTMLDivElement;
        private Popup: HTMLElement | null = null;
        private Enabled: boolean = true;

        private Focused: boolean = false;

        constructor(controlId: string) {
            super(controlId, DropDown2ListEditComponent.TEMPLATE, DropDown2ListEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: DropDown2ListEditComponent.EVENTCHANGE,
                GetValue: (control: DropDown2ListEditComponent): string | null => {
                    return control.value;
                },
                Enable: (control: DropDown2ListEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }, false, (tag: HTMLElement, control: DropDown2ListEditComponent): void => {

            });

            this.Input = $YetaWF.getElement1BySelector(".t_input", [this.Control]) as HTMLDivElement;
            this.Select = $YetaWF.getElement1BySelector("select", [this.Control]) as HTMLSelectElement;
            this.Container = $YetaWF.getElement1BySelector(".t_container", [this.Control]) as HTMLDivElement;

            let width = this.calcMaxStringLength();
            this.Control.style.width = `${width}px`;

            $YetaWF.registerEventHandler(this.Container, "mouseenter", null, (ev: MouseEvent): boolean => {
                if (this.Enabled) {
                    $YetaWF.elementRemoveClass(this.Container, "k-state-hover");
                    $YetaWF.elementAddClass(this.Container, "k-state-hover");
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Container, "mouseleave", null, (ev: MouseEvent): boolean => {
                if (this.Enabled)
                    $YetaWF.elementRemoveClass(this.Container, "k-state-hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.Container, "click", null, (ev: MouseEvent): boolean => {
                if (this.Enabled)
                    this.openPopup();
                return false;
            });

            $YetaWF.registerEventHandler(this.Control, "focusin", null, (ev: FocusEvent): boolean => {
                if (this.Enabled) {
                    $YetaWF.elementRemoveClass(this.Container, "k-state-focused");
                    $YetaWF.elementAddClass(this.Container, "k-state-focused");
                    this.Focused = true;
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Control, "focusout", null, (ev: FocusEvent): boolean => {
                //$$$ this.closePopup();
                $YetaWF.elementRemoveClass(this.Container, "k-state-focused");
                this.Focused = false;
                return true;
            });
            $YetaWF.registerEventHandler(this.Control, "keydown", null, (ev: KeyboardEvent): boolean => {
                if (this.Enabled) {
                    let key = ev.key;
                    if (ev.altKey) {
                        if (key === "ArrowDown" || key === "Down" || key === "ArrowRight" || key === "Right") {
                            this.openPopup();
                            return false;
                        }
                    } else {
                        if (key === "ArrowDown" || key === "Down" || key === "ArrowRight" || key === "Right") {
                            ++this.selectedIndex;
                            return false;
                        } else if (key === "ArrowUp" || key === "Up" || key === "ArrowLeft" || key === "Left") {
                            --this.selectedIndex;
                            return false;
                        } else if (key === "Home") {
                            this.selectedIndex = 0;
                            return false;
                        } else if (key === "End") {
                            let total = this.totalItems;
                            this.selectedIndex = total - 1;
                            return false;
                        } else if (key === "Escape") {
                            this.closePopup();
                            return false;
                        } else if (key.length === 1) {
                            // find an entry starting with the character pressed
                            const opts = this.Select.options;
                            const len = opts.length;
                            for (let i = this.selectedIndex + 1 ; i < len; ++i) {
                                key = key.toLowerCase();
                                if (opts[i].text.toLowerCase().startsWith(key)) {
                                    this.selectedIndex = i;
                                    break;
                                }
                            }
                        }
                    }
                }
                return true;
            });
        }

        get value(): string {
            return this.Select.value;
        }
        set value(val: string) {
            this.Select.value = val;
        }
        get selectedIndex(): number {
            return this.Select.selectedIndex;
        }
        set selectedIndex(index: number) {
            let total = this.totalItems;
            if (index < 0 || index >= total)
                return;
            this.Select.selectedIndex = index;
            this.Select.options[index].selected = true;

            this.clearSelectedPopupItem();
            this.selectPopupItem();
            this.Input.innerText = this.Select.options[index].text;
            this.sendChangeEvent();
       }
        get totalItems(): number {
            return this.Select.options.length;
        }

        // retrieve the tooltip for the nth item (index) in the dropdown list
        public getToolTip(index: number): string | null {
            let total = this.totalItems;
            if (index < 0 || index >= total) return null;
            let opt = this.Select.options[index];
            let tt = $YetaWF.getAttributeCond(opt, YConfigs.Basics.CssTooltip);
            return tt;
        }
        public clear(): void {
            this.closePopup();
            this.selectedIndex = 0;
        }
        public enable(enabled: boolean): void {
            this.closePopup();
            $YetaWF.elementEnableToggle(this.Select, enabled);
            $YetaWF.elementEnableToggle(this.Container, enabled);
            $YetaWF.elementRemoveClass(this.Container, "k-state-disabled");
            this.Control.removeAttribute("tabindex");
            if (!enabled) {
                $YetaWF.elementAddClass(this.Container, "k-state-disabled");
                $YetaWF.setAttribute(this.Control, "aria-disabled", "true");
            } else {
                $YetaWF.setAttribute(this.Control, "tabindex", "0");
                $YetaWF.setAttribute(this.Control, "aria-disabled", "false");
            }
            this.Enabled = enabled;
        }
        public sendChangeEvent(): void {
            var event = document.createEvent("Event");
            event.initEvent(DropDown2ListEditComponent.EVENTCHANGE, true, true);
            this.Control.dispatchEvent(event);
            FormsSupport.validateElement(this.Select);
        }

        private openPopup(): void {
            if (this.Popup) {
                this.closePopup();
                return;
            }
            this.Popup =
                <div id="yDDPopup" data-owner={this.ControlId} class="k-animation-container" aria-hidden="false">
                    <div class="k-list-container k-popup k-group k-reset" data-role="popup" aria-hidden="false">
                        <div class="k-list-scroller" unselectable="on">
                            <ul unselectable="on" class="k-list k-reset" tabindex="-1" aria-hidden="true" aria-live="off" data-role="staticlist" role="listbox">
                            </ul>
                        </div>
                    </div>
                </div> as HTMLElement;

            let ul = $YetaWF.getElement1BySelector("ul", [this.Popup]);
            const opts = this.Select.options;
            const len = opts.length;
            for (let i = 0; i < len; ++i) {
                ul.append(<li tabindex="-1" role="option" unselectable="on" class="k-item" data-index={i}>{opts[i].innerText}</li>);
            }
            DropDown2ListEditComponent.positionPopup(this.Popup);

            document.body.append(this.Popup);
            this.selectPopupItem();

            $YetaWF.registerEventHandler(this.Popup, "click", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem as HTMLLIElement;
                let index = Number($YetaWF.getAttribute(li, "data-index"));
                this.selectedIndex = index;
                this.closePopup();
                return false;
            });
            $YetaWF.registerEventHandler(this.Popup, "mouseover", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "k-state-hover");
                $YetaWF.elementAddClass(li, "k-state-hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.Popup, "mouseout", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "k-state-hover");
                return true;
            });
        }
        public closePopup(): void {
            if (!this.Popup) return;
            this.Popup.remove();
            this.Popup = null;
        }

        public static positionPopup(popup: HTMLElement): void {
            if (!popup) return;
            let ownerId = $YetaWF.getAttribute(popup, "data-owner");
            let dd = $YetaWF.getElementById(ownerId);
            let rect = dd.getBoundingClientRect();
            let top = window.pageYOffset + rect.bottom;
            let left = window.pageXOffset + rect.left;
            let width = rect.width;

            // set left, top, width on #yDDPopup
            popup.style.width = `${width}px`;
            popup.style.top = `${top}px`;
            popup.style.left = `${left}px`;

            // resize based on number of entries wanted
            let scroller = $YetaWF.getElement1BySelector(".k-list-scroller", [popup]);

            scroller.style.height = "200px";//$$$$
        }

        private selectPopupItem(): void {
            const index = this.Select.selectedIndex;
            if (index < 0) return;

            if (this.Popup) {
                let ul = $YetaWF.getElement1BySelector("ul", [this.Popup]);
                let li = ul.children[index] as HTMLLIElement;
                $YetaWF.elementRemoveClasses(li, ["k-state-selected", "k-state-focused"]);
                $YetaWF.elementAddClass(li, "k-state-selected");
                let ariaId = $YetaWF.getAttribute(this.Control, "aria-activedescendant");
                li.id = ariaId;
                $YetaWF.setAttribute(li, "aria-selected", "true");
                if (this.Focused)
                    $YetaWF.elementAddClass(li, "k-state-focused");

                let scroller = $YetaWF.getElement1BySelector(".k-list-scroller", [this.Popup]);

                let rectElem = li.getBoundingClientRect();
                let rectContainer = scroller.getBoundingClientRect();
                if (rectElem.bottom > rectContainer.bottom) li.scrollIntoView(false);
                if (rectElem.top < rectContainer.top) li.scrollIntoView();
            }
        }

        private clearSelectedPopupItem(): void {
            if (this.Popup) {
                let lis = $YetaWF.getElementsBySelector("ul li.k-state-selected", [this.Popup]);
                for (const li of lis) {
                    $YetaWF.elementRemoveClasses(li, ["k-state-selected", "k-state-focused"]);
                    $YetaWF.setAttribute(li, "aria-selected", "false");
                    li.id = "";
                }
            }
            this.Input.innerText = "";
        }

        public static closeDropdowns(): void {
            let popup = $YetaWF.getElementByIdCond(DropDown2ListEditComponent.POPUPID);
            if (!popup) return;
            let ownerId = $YetaWF.getAttribute(popup, "data-owner");
            let control = DropDown2ListEditComponent.getControlById<DropDown2ListEditComponent>(ownerId, DropDown2ListEditComponent.SELECTOR);
            control.closePopup();
        }


        private calcMaxStringLength(): number {

            let elem = <div style="position:absolute;visibility:hidden;white-space:nowrap"></div> as HTMLElement;
            this.Control.appendChild(elem);

            let width: number = 0;

            const opts = this.Select.options;
            const len = opts.length;
            for (let i = 0; i < len; ++i) {
                elem.innerHTML = opts[i].innerHTML;
                let w = elem.clientWidth;
                if (w > width)
                    width = w;
            }

            // extra for padding and dropdown selector
            elem.innerText = "MMMM";// 4 characters
            width += elem.clientWidth;

            elem.remove();
            return width;
        }

        public ajaxUpdate(data: any, ajaxUrl: string, onSuccess?: (data: any) => void, onFailure?: (result: string) => void): void {

            //$YetaWF.setLoading(true);

            //var uri = $YetaWF.parseUrl(ajaxUrl);
            //uri.addSearchSimpleObject(data);

            //var request: XMLHttpRequest = new XMLHttpRequest();
            //request.open("POST", ajaxUrl);
            //request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            //request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
            //request.onreadystatechange = (ev: Event): any => {
            //    if (request.readyState === 4 /*DONE*/) {
            //        $YetaWF.setLoading(false);
            //        var retVal = $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, this.Control, undefined, undefined, (data: AjaxData): void => {

            //            // $(this.Control).val(null);
            //            $(this.Control).kendoDropDown2List({
            //                dataTextField: "t",
            //                dataValueField: "v",
            //                dataSource: data.data,
            //                autoWidth: true,
            //                change: (): void => {
            //                    this.sendChangeEvent();
            //                }
            //            });
            //            this.Setup.ToolTips = data.tooltips;
            //            this.KendoDropDown2List = $(this.Control).data("kendoDropDown2List");

            //            if (onSuccess) {
            //                onSuccess(data);
            //            } else {
            //                this.value = "";
            //                this.sendChangeEvent();
            //            }
            //        });
            //        if (!retVal) {
            //            if (onFailure)
            //                onFailure(request.responseText);
            //        }
            //    }
            //};
            //request.send(uri.toFormData());
        }
    }

    // handle submit/apply
    $YetaWF.registerCustomEventHandlerDocument(DropDown2ListEditComponent.EVENTCHANGE, `.ysubmitonchange ${DropDown2ListEditComponent.SELECTOR}`, (ev: Event): boolean => {
        $YetaWF.Forms.submitOnChange(ev.target as HTMLElement);
        return false;
    });
    $YetaWF.registerCustomEventHandlerDocument(DropDown2ListEditComponent.EVENTCHANGE, `.yapplyonchange ${DropDown2ListEditComponent.SELECTOR}`, (ev: Event): boolean => {
        $YetaWF.Forms.applyOnChange(ev.target as HTMLElement);
        return false;
    });
    $YetaWF.registerCustomEventHandlerDocument(DropDown2ListEditComponent.EVENTCHANGE, `.yreloadonchange ${DropDown2ListEditComponent.SELECTOR}`, (ev: Event): boolean => {
        $YetaWF.Forms.reloadOnChange(ev.target as HTMLElement);
        return false;
    });

    // close dropdown when clicking outside
    $YetaWF.registerEventHandlerBody("click", null, (ev: MouseEvent): boolean => {
        DropDown2ListEditComponent.closeDropdowns();
        return true;
    });

    // reposition dropdown when window size changes
    ($(window) as any).smartresize((): void => {
        let popup = $YetaWF.getElementByIdCond(DropDown2ListEditComponent.POPUPID);
        if (popup)
            DropDown2ListEditComponent.positionPopup(popup);
    });
}

