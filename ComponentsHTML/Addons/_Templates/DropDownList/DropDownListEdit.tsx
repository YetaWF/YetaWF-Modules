/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface DropDownListEditSetup {
        AdjustWidth: boolean;
        DropDownWidthFactor: number;
        DropDownHeightFactor: number;
    }

    interface AjaxData {
        OptionsHTML: string;
        ExtraData: string;
    }

    export class DropDownListEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_dropdownlist_base";
        public static readonly SELECTOR: string = ".yt_dropdownlist_base.t_edit";
        public static readonly EVENTCHANGE: string = "dropdownlist_change";

        public static readonly POPUPID: string = "yDDPopup";
        private static readonly DEFAULTHEIGHT: number = 200;

        private Setup: DropDownListEditSetup;

        private Input: HTMLDivElement;
        private Select: HTMLSelectElement;
        private Container: HTMLDivElement;
        private Popup: HTMLElement | null = null;
        private Enabled: boolean = true;
        private DropDownWidth: number = 0;

        private Focused: boolean = false;

        constructor(controlId: string, setup: DropDownListEditSetup) {
            super(controlId, DropDownListEditComponent.TEMPLATE, DropDownListEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: DropDownListEditComponent.EVENTCHANGE,
                GetValue: (control: DropDownListEditComponent): string | null => {
                    return control.value;
                },
                Enable: (control: DropDownListEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }, false, (tag: HTMLElement, control: DropDownListEditComponent): void => {

            });

            this.Setup = setup;

            this.Input = $YetaWF.getElement1BySelector(".t_input", [this.Control]) as HTMLDivElement;
            this.Select = $YetaWF.getElement1BySelector("select", [this.Control]) as HTMLSelectElement;
            this.Container = $YetaWF.getElement1BySelector(".t_container", [this.Control]) as HTMLDivElement;

            this.optionsUpdated();

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
                        } else if (key === "Tab") {
                            this.closePopup();
                            return true;
                        } else if (key.length === 1) {
                            // find an entry starting with the character pressed
                            const opts = this.Select.options;
                            const len = opts.length;
                            for (let i = this.selectedIndex + 1 ; i < len; ++i) {
                                key = key.toLowerCase();
                                if (opts[i].text.toLowerCase().startsWith(key)) {
                                    this.selectedIndex = i;
                                    return true;
                                }
                            }
                            if (this.selectedIndex > 0) {
                                let end = this.selectedIndex;
                                for (let i = 0; i < end; ++i) {
                                    key = key.toLowerCase();
                                    if (opts[i].text.toLowerCase().startsWith(key)) {
                                        this.selectedIndex = i;
                                        return true;
                                    }
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
            event.initEvent(DropDownListEditComponent.EVENTCHANGE, true, true);
            this.Control.dispatchEvent(event);
            FormsSupport.validateElement(this.Select);
        }

        private optionsUpdated(): void {
            this.DropDownWidth = this.calcMaxStringLength();
            if (this.Setup.AdjustWidth) {
                this.Control.style.width = `${this.DropDownWidth}px`;
            } else {
                this.Control.style.minWidth = `${this.DropDownWidth}px`;
            }
        }

        private openPopup(): void {
            if (this.Popup) {
                this.closePopup();
                return;
            }

            DropDownListEditComponent.closeDropdowns();
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
            let html = "";
            for (let i = 0; i < len; ++i) {
                let o = opts[i];
                let tt = o.getAttribute(YConfigs.Basics.CssTooltip);
                if (tt)
                    tt = ` ${YConfigs.Basics.CssTooltip}="${tt}"`;
                else
                    tt = "";
                html += `<li tabindex="-1" role="option" unselectable="on" class="k-item" data-index="${i}"${tt}>${o.innerHTML}</li>`;
            }
            ul.innerHTML = html;

            let style = window.getComputedStyle(this.Control);
            this.Popup.style.font = style.font;
            this.Popup.style.fontStyle = style.fontStyle;
            this.Popup.style.fontWeight = style.fontWeight;
            this.Popup.style.fontSize = style.fontSize;

            DropDownListEditComponent.positionPopup(this.Popup);

            document.body.append(this.Popup);
            this.selectPopupItem();
            this.Control.setAttribute("aria-expanded", "true");

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
            this.Control.setAttribute("aria-expanded", "false");
        }

        public static positionPopup(popup: HTMLElement): void {
            if (!popup) return;
            let ownerId = $YetaWF.getAttribute(popup, "data-owner");
            let control = DropDownListEditComponent.getControlById<DropDownListEditComponent>(ownerId, DropDownListEditComponent.SELECTOR);
            let scroller = $YetaWF.getElement1BySelector(".k-list-scroller", [popup]);

            // resize to fit

            let controlRect = control.Control.getBoundingClientRect();
            let desiredHeight = control.Setup.DropDownHeightFactor * DropDownListEditComponent.DEFAULTHEIGHT;
            let desiredWidth = control.Setup.DropDownWidthFactor * control.DropDownWidth;
            let bottomAvailable = window.innerHeight - controlRect.bottom;
            let topAvailable = controlRect.top;

            let top = 0, bottom = 0;

            // Top/bottom position and height calculation
            let useTop = true;
            if (bottomAvailable < desiredHeight && topAvailable > bottomAvailable) {
                useTop = false;
                bottom = window.innerHeight - controlRect.top;
                if (topAvailable < desiredHeight)
                    desiredHeight = topAvailable;
            } else {
                top = controlRect.bottom;
                if (bottomAvailable < desiredHeight)
                    bottomAvailable = desiredHeight;
            }

            // Left/Width calculation

            let left = 0, right = 0;
            let useLeft = true;
            if (desiredWidth > window.innerWidth) {
                useLeft = false;
                left = 0;
                desiredWidth = window.innerWidth;
            } else {
                if (controlRect.left + desiredWidth > window.innerWidth) {
                    useLeft = false;
                    right = 0;
                } else if (controlRect.left < 0) {
                    left = 0;
                } else {
                    left = controlRect.left;
                }
            }

            // set left, top, width on #yDDPopup
            if (useTop) {
                popup.style.top = `${top + window.pageYOffset}px`;
            } else {
                popup.style.bottom = `${bottom - window.pageYOffset}px`;
            }
            if (useLeft) {
                popup.style.left = `${left + window.pageXOffset}px`;
            } else {
                popup.style.right = `${right - window.pageXOffset}px`;
            }
            popup.style.width = `${desiredWidth}px`;

            scroller.style.maxHeight = `${desiredHeight}px`;
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
                if (rectElem.bottom > rectContainer.bottom) li.scrollIntoView({ behavior: "auto", block: "nearest", inline: "end" });
                if (rectElem.top < rectContainer.top) li.scrollIntoView({ behavior: "auto", block: "nearest", inline: "start" });
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
            let popup = $YetaWF.getElementByIdCond(DropDownListEditComponent.POPUPID);
            if (!popup) return;
            let ownerId = $YetaWF.getAttribute(popup, "data-owner");
            let control = DropDownListEditComponent.getControlById<DropDownListEditComponent>(ownerId, DropDownListEditComponent.SELECTOR);
            control.closePopup();
        }

        private calcMaxStringLength(): number {

            let elem = <div style="position:absolute;visibility:hidden;white-space:nowrap"></div> as HTMLElement;
            document.body.append(elem);
            // copy font attributes
            let style = window.getComputedStyle(this.Control);
            elem.style.font = style.font;
            elem.style.fontStyle = style.fontStyle;
            elem.style.fontWeight = style.fontWeight;
            elem.style.fontSize = style.fontSize;

            let width: number = 0;

            const opts = this.Select.options;
            const len = opts.length;
            for (let i = 0; i < len; ++i) {
                elem.innerHTML = opts[i].innerHTML;
                let w = elem.clientWidth;
                if (w > width)
                    width = w;
            }

            // extra for dropdown selector
            elem.innerText = "MMMM";// 4 characters
            width += elem.clientWidth;

            elem.remove();
            return width;
        }

        public ajaxUpdate(data: any, ajaxUrl: string, onSuccess?: (data: any) => void, onFailure?: (result: string) => void): void {

            $YetaWF.setLoading(true);

            var uri = $YetaWF.parseUrl(ajaxUrl);
            uri.addSearchSimpleObject(data);

            var request: XMLHttpRequest = new XMLHttpRequest();
            request.open("POST", ajaxUrl);
            request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.setRequestHeader("X-Requested-With", "XMLHttpRequest");
            request.onreadystatechange = (ev: Event): any => {
                if (request.readyState === 4 /*DONE*/) {
                    $YetaWF.setLoading(false);
                    var retVal = $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, this.Control, undefined, undefined, (data: AjaxData): void => {

                        this.Select.innerHTML = data.OptionsHTML;
                        this.optionsUpdated();

                        if (onSuccess) {
                            onSuccess(data);
                        } else {
                            this.selectedIndex = 0;
                        }
                    });
                    if (!retVal) {
                        if (onFailure)
                            onFailure(request.responseText);
                    }
                }
            };
            request.send(uri.toFormData());
        }
    }

    // handle submit/apply
    $YetaWF.registerCustomEventHandlerDocument(DropDownListEditComponent.EVENTCHANGE, `.ysubmitonchange ${DropDownListEditComponent.SELECTOR}`, (ev: Event): boolean => {
        $YetaWF.Forms.submitOnChange(ev.target as HTMLElement);
        return false;
    });
    $YetaWF.registerCustomEventHandlerDocument(DropDownListEditComponent.EVENTCHANGE, `.yapplyonchange ${DropDownListEditComponent.SELECTOR}`, (ev: Event): boolean => {
        $YetaWF.Forms.applyOnChange(ev.target as HTMLElement);
        return false;
    });
    $YetaWF.registerCustomEventHandlerDocument(DropDownListEditComponent.EVENTCHANGE, `.yreloadonchange ${DropDownListEditComponent.SELECTOR}`, (ev: Event): boolean => {
        $YetaWF.Forms.reloadOnChange(ev.target as HTMLElement);
        return false;
    });

    // close dropdown when clicking outside
    $YetaWF.registerEventHandlerBody("click", null, (ev: MouseEvent): boolean => {
        DropDownListEditComponent.closeDropdowns();
        return true;
    });

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, (ev: Event): boolean => {
        DropDownListEditComponent.closeDropdowns();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: Event): boolean => {
        DropDownListEditComponent.closeDropdowns();
        return true;
    });
}

