/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface DropDownListEditSetup {
        AdjustWidth: boolean;
        DropDownWidthFactor: number;
        DropDownHeightFactor: number;
    }

    export interface DropDownListAjaxData {
        OptionsHTML: string;
        ExtraData: string;
    }

    enum SendSelectEnum {
        No = 0,
        Yes = 1,
        ChangeSinceOpen = 2, // send change event if open or selection has changed since opened/closed
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
        private Focused: boolean = false;
        private DropDownWidth: number = 0;
        private IndexOnOpen: number = -1;
        private MouseSelectedIndex: number = -1;

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
                control.closePopup(SendSelectEnum.No);
            });

            this.Setup = setup;

            this.Input = $YetaWF.getElement1BySelector(".t_input", [this.Control]) as HTMLDivElement;
            this.Select = $YetaWF.getElement1BySelector("select", [this.Control]) as HTMLSelectElement;
            this.Container = $YetaWF.getElement1BySelector(".t_container", [this.Control]) as HTMLDivElement;

            this.Enabled = ! $YetaWF.elementHasClass(this.Container, "t_disabled");

            this.optionsUpdated();

            $YetaWF.registerEventHandler(this.Container, "mouseenter", null, (ev: MouseEvent): boolean => {
                if (this.Enabled) {
                    $YetaWF.elementRemoveClass(this.Container, "t_hover");
                    $YetaWF.elementAddClass(this.Container, "t_hover");
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Container, "mouseleave", null, (ev: MouseEvent): boolean => {
                if (this.Enabled)
                    $YetaWF.elementRemoveClass(this.Container, "t_hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.Container, "click", null, (ev: MouseEvent): boolean => {
                if (this.Enabled)
                    this.openPopup();
                return false;
            });

            $YetaWF.registerEventHandler(this.Control, "focusin", null, (ev: FocusEvent): boolean => {
                if (this.Enabled) {
                    $YetaWF.elementRemoveClass(this.Container, "t_focused");
                    $YetaWF.elementAddClass(this.Container, "t_focused");
                    this.Focused = true;
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Control, "focusout", null, (ev: FocusEvent): boolean => {
                $YetaWF.elementRemoveClass(this.Container, "t_focused");
                this.Focused = false;
                this.closePopup(SendSelectEnum.ChangeSinceOpen);
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
                        if (key === "ArrowUp" || key === "Up" || key === "ArrowLeft" || key === "Left") {
                            this.closePopup(SendSelectEnum.Yes);
                            return false;
                        }
                    } else {
                        if (key === "ArrowDown" || key === "Down" || key === "ArrowRight" || key === "Right") {
                            this.setSelectedIndex(this.selectedIndex+ 1);
                            return false;
                        } else if (key === "ArrowUp" || key === "Up" || key === "ArrowLeft" || key === "Left") {
                            this.setSelectedIndex(this.selectedIndex-1);
                            return false;
                        } else if (key === "Home") {
                            this.setSelectedIndex(0);
                            return false;
                        } else if (key === "End") {
                            let total = this.totalItems;
                            this.setSelectedIndex(total - 1);
                            return false;
                        } else if (key === "Escape") {
                            if (this.isOpen) {
                                this.closePopup(SendSelectEnum.No);
                                return false;
                            }
                            return true;
                        } else if (key === "Tab") {
                            this.closePopup(SendSelectEnum.ChangeSinceOpen);
                            return true;
                        } else if (key === "Enter") {
                            if (this.Popup) {
                                this.closePopup(SendSelectEnum.ChangeSinceOpen);
                                return false;
                            }
                        } else if (key.length === 1) {
                            // find an entry starting with the character pressed
                            const opts = this.Select.options;
                            const len = opts.length;
                            for (let i = this.selectedIndex + 1 ; i < len; ++i) {
                                key = key.toLowerCase();
                                if (opts[i].text.toLowerCase().startsWith(key)) {
                                    this.setSelectedIndex(i);
                                    return true;
                                }
                            }
                            if (this.selectedIndex > 0) {
                                let end = this.selectedIndex;
                                for (let i = 0; i < end; ++i) {
                                    key = key.toLowerCase();
                                    if (opts[i].text.toLowerCase().startsWith(key)) {
                                        this.setSelectedIndex(i);
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

            this.clearSelectedPopupItem();
            this.selectPopupItem();
            if (this.selectedIndex >= 0)
                this.Input.innerText = this.Select.options[this.selectedIndex].text;
        }
        get selectedIndex(): number {
            return this.Select.selectedIndex;
        }
        private setSelectedIndex(index: number, event?: boolean): void {
            let total = this.totalItems;
            if (index < 0 || index >= total)
                return;
            this.Select.selectedIndex = index;
            this.Select.options[index].selected = true;

            this.clearSelectedPopupItem();
            this.selectPopupItem();
            this.Input.innerText = this.Select.options[index].text;

            if (event !== false && !this.isOpen)
                this.sendChangeEvent();
        }
        get totalItems(): number {
            return this.Select.options.length;
        }

        get isOpen(): boolean {
            return this.Popup != null;
        }

        public setOptionsHTML(optionsHTML: string): void {
            this.Select.innerHTML = optionsHTML;
            this.optionsUpdated();

            this.setSelectedIndex(0);
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
            this.closePopup(SendSelectEnum.No);
            this.setSelectedIndex(0, false);
        }
        public enable(enabled: boolean): void {
            this.closePopup(SendSelectEnum.No);
            $YetaWF.elementEnableToggle(this.Select, enabled);
            $YetaWF.elementEnableToggle(this.Container, enabled);
            $YetaWF.elementRemoveClass(this.Container, "t_disabled");
            this.Control.removeAttribute("tabindex");
            if (!enabled) {
                $YetaWF.elementAddClass(this.Container, "t_disabled");
                $YetaWF.setAttribute(this.Control, "aria-disabled", "true");
            } else {
                $YetaWF.setAttribute(this.Control, "tabindex", "0");
                $YetaWF.setAttribute(this.Control, "aria-disabled", "false");
            }
            this.Enabled = enabled;
        }
        public sendChangeEvent(): void {
            $YetaWF.sendCustomEvent(this.Control, DropDownListEditComponent.EVENTCHANGE);
            FormsSupport.validateElement(this.Select);
        }

        private optionsUpdated(): void {
            this.DropDownWidth = this.calcMaxStringLength();
            if (this.Setup.AdjustWidth) {
                this.Control.style.width = `${this.DropDownWidth}px`;
            //} else {
            //    this.Control.style.minWidth = `${this.DropDownWidth}px`;
            }
        }

        private openPopup(): void {
            if (this.Popup) {
                this.closePopup(SendSelectEnum.No);
                return;
            }

            this.IndexOnOpen = this.selectedIndex;

            DropDownListEditComponent.closeDropdowns();
            this.Popup =
                <div id={DropDownListEditComponent.POPUPID} data-owner={this.ControlId} aria-hidden="false">
                    <div class="t_container" data-role="popup" aria-hidden="false">
                        <div class="t_scroller" unselectable="on">
                            <ul unselectable="on" class="t_list" tabindex="-1" aria-hidden="true" aria-live="off" data-role="staticlist" role="listbox">
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
                html += `<li tabindex="-1" role="option" unselectable="on" class="t_item" data-index="${i}"${tt}>${o.innerHTML}</li>`;
            }
            ul.innerHTML = html;

            let style = window.getComputedStyle(this.Control);
            this.Popup.style.font = style.font;
            this.Popup.style.fontStyle = style.fontStyle;
            this.Popup.style.fontWeight = style.fontWeight;
            this.Popup.style.fontSize = style.fontSize;

            DropDownListEditComponent.positionPopup(this.Popup);

            document.body.appendChild(this.Popup);
            this.selectPopupItem();
            this.Control.setAttribute("aria-expanded", "true");

            $YetaWF.registerEventHandler(this.Popup, "mousedown", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem as HTMLLIElement;
                let index = Number($YetaWF.getAttribute(li, "data-index"));
                this.MouseSelectedIndex = index;
                return false;
            });
            $YetaWF.registerEventHandler(this.Popup, "mouseup", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem as HTMLLIElement;
                let index = Number($YetaWF.getAttribute(li, "data-index"));
                if (this.MouseSelectedIndex === index) {
                    this.setSelectedIndex(index);
                    this.closePopup(SendSelectEnum.Yes);
                } else {
                    this.MouseSelectedIndex = -1;
                }
                return false;
            });
            $YetaWF.registerEventHandler(this.Popup, "mouseover", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "t_hover");
                $YetaWF.elementAddClass(li, "t_hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.Popup, "mouseout", "ul li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "t_hover");
                return true;
            });
        }
        public closePopup(sendEvent: SendSelectEnum): void {
            ToolTipsHTMLHelper.removeTooltips();
            if (!this.Popup) {
                if (sendEvent === SendSelectEnum.ChangeSinceOpen && this.IndexOnOpen !== -1 && this.selectedIndex !== this.IndexOnOpen) {
                    this.IndexOnOpen = -1;
                    this.sendChangeEvent();
                }
            } else {
                this.Popup.remove();
                this.Popup = null;
                this.Control.setAttribute("aria-expanded", "false");

                if (sendEvent === SendSelectEnum.Yes) {
                    this.IndexOnOpen = -1;
                    this.sendChangeEvent();
                } else if (sendEvent === SendSelectEnum.ChangeSinceOpen && this.IndexOnOpen !== -1 && this.IndexOnOpen !== this.selectedIndex) {
                    this.IndexOnOpen = -1;
                    this.sendChangeEvent();
                }
            }
        }

        public static positionPopup(popup: HTMLElement): void {
            if (!popup) return;
            let ownerId = $YetaWF.getAttribute(popup, "data-owner");
            let control = DropDownListEditComponent.getControlById<DropDownListEditComponent>(ownerId, DropDownListEditComponent.SELECTOR);
            let scroller = $YetaWF.getElement1BySelector(".t_scroller", [popup]);

            // resize to fit

            let controlRect = control.Control.getBoundingClientRect();
            let desiredHeight = control.Setup.DropDownHeightFactor * DropDownListEditComponent.DEFAULTHEIGHT;
            let desiredWidth = control.Setup.DropDownWidthFactor * controlRect.width;

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
                $YetaWF.elementRemoveClasses(li, ["t_selected", "t_focused"]);
                $YetaWF.elementAddClass(li, "t_selected");
                let ariaId = $YetaWF.getAttribute(this.Control, "aria-activedescendant");
                li.id = ariaId;
                $YetaWF.setAttribute(li, "aria-selected", "true");
                if (this.Focused)
                    $YetaWF.elementAddClass(li, "t_focused");

                let scroller = $YetaWF.getElement1BySelector(".t_scroller", [this.Popup]);

                let rectElem = li.getBoundingClientRect();
                let rectContainer = scroller.getBoundingClientRect();
                if (rectElem.bottom > rectContainer.bottom) li.scrollIntoView({ behavior: "auto", block: "nearest", inline: "end" });
                if (rectElem.top < rectContainer.top) li.scrollIntoView({ behavior: "auto", block: "nearest", inline: "start" });
            }
        }

        private clearSelectedPopupItem(): void {
            if (this.Popup) {
                let lis = $YetaWF.getElementsBySelector("ul li.t_selected", [this.Popup]);
                for (const li of lis) {
                    $YetaWF.elementRemoveClasses(li, ["t_selected", "t_focused"]);
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
            control.closePopup(SendSelectEnum.No);
        }

        private calcMaxStringLength(): number {

            let elem = <div style="position:absolute;visibility:hidden;white-space:nowrap"></div> as HTMLElement;
            document.body.appendChild(elem);
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
            elem.innerText = "MMM";// 3 characters
            width += elem.clientWidth;
            width += 4;// fudge factor

            elem.remove();
            return width;
        }

        public ajaxUpdate(data: any, ajaxUrl: string, onSuccess?: (data: any) => void, onFailure?: (result: string) => void): void {

            this.closePopup(SendSelectEnum.No);

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
                    var retVal = $YetaWF.processAjaxReturn(request.responseText, request.statusText, request, this.Control, undefined, undefined, (data: DropDownListAjaxData): void => {

                        this.setOptionsHTML(data.OptionsHTML);

                        if (onSuccess)
                            onSuccess(data);
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

