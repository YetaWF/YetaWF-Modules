/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface DropDownSearchEditSetup {
        AjaxUrl: string;
        AdjustWidth: boolean;
        DropDownWidthFactor: number;
        DropDownHeightFactor: number;
    }

    interface SelectionItem {
        Value: any;
        Text: string;
        Tooltip: string|null;
    }

    enum SendSelectEnum {
        No = 0,
        Yes = 1,
        ChangeSinceOpen = 2, // send change event if open or selection has changed since opened/closed
    }

    export class DropDownSearchEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_dropdownsearch_base";
        public static readonly SELECTOR: string = ".yt_dropdownsearch_base.t_edit";
        public static readonly EVENTCHANGE: string = "dropdownsearch_change";

        public static readonly INPUTWAIT: number = 300;

        public static readonly POPUPID: string = "yDSPopup";
        private static readonly DEFAULTHEIGHT: number = 200;

        private Setup: DropDownSearchEditSetup;

        private Input: HTMLInputElement;
        private Hidden: HTMLInputElement;
        private InputTimeout: number = 0;
        private LastValue: string = "";

        private Popup: HTMLElement | null = null;

        private Enabled: boolean = true;
        private Focused: boolean = false;
        private SelectedIndex: number = -1;
        private MouseSelectedIndex: number = -1;

        constructor(controlId: string, setup: DropDownSearchEditSetup) {
            super(controlId, DropDownSearchEditComponent.TEMPLATE, DropDownSearchEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: DropDownSearchEditComponent.EVENTCHANGE,
                GetValue: (control: DropDownSearchEditComponent): string | null => {
                    return control.value;
                },
                Enable: (control: DropDownSearchEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }, false, (tag: HTMLElement, control: DropDownSearchEditComponent): void => {
                control.closePopup(SendSelectEnum.No);
                control.clearInputTimeout();
            });

            this.Setup = setup;

            this.Input = $YetaWF.getElement1BySelector("input[type='text']", [this.Control]) as HTMLInputElement;
            this.Hidden = $YetaWF.getElement1BySelector("input[type='hidden']", [this.Control]) as HTMLInputElement;

            $YetaWF.registerEventHandler(this.Input, "input", null, (ev: Event): boolean => {
                this.clearInputTimeout();
                this.InputTimeout = setTimeout(():void => {
                    this.Hidden.value = "";
                    this.updateDropDown();
                }, DropDownSearchEditComponent.INPUTWAIT);
                return true;
            });

            $YetaWF.registerEventHandler(this.Control, "mouseenter", null, (ev: MouseEvent): boolean => {
                if (this.Enabled) {
                    $YetaWF.elementRemoveClass(this.Control, "t_hover");
                    $YetaWF.elementAddClass(this.Control, "t_hover");
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Control, "mouseleave", null, (ev: MouseEvent): boolean => {
                if (this.Enabled)
                    $YetaWF.elementRemoveClass(this.Control, "t_hover");
                return true;
            });

            $YetaWF.registerEventHandler(this.Control, "focusin", null, (ev: FocusEvent): boolean => {
                if (this.Enabled) {
                    $YetaWF.elementRemoveClass(this.Control, "t_focused");
                    $YetaWF.elementAddClass(this.Control, "t_focused");
                    this.Focused = true;
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Control, "focusout", null, (ev: FocusEvent): boolean => {
                $YetaWF.elementRemoveClass(this.Control, "t_focused");
                if (this.Hidden.value === "")
                    this.Input.value = "";
                this.Focused = false;
                this.closePopup(SendSelectEnum.ChangeSinceOpen);
                return true;
            });
            $YetaWF.registerEventHandler(this.Control, "keydown", null, (ev: KeyboardEvent): boolean => {
                if (this.Enabled) {
                    let key = ev.key;
                    if (ev.altKey) {
                        if (this.Popup) {
                            if (key === "ArrowUp" || key === "ArrowLeft") {
                                this.closePopup(SendSelectEnum.Yes);
                                return false;
                            }
                        } else {
                            if (key === "ArrowDown" || key === "ArrowRight") {
                                this.updateDropDown();
                                return false;
                            }
                        }
                    } else {
                        if (this.Popup) {
                            if (key === "ArrowDown" || key === "ArrowRight") {
                                this.setSelectedIndex(this.SelectedIndex+1);
                                return false;
                            } else if (key === "ArrowUp" || key === "ArrowLeft") {
                                if (this.SelectedIndex < 0)
                                    this.setSelectedIndex(this.totalItems - 1);
                                else
                                    this.setSelectedIndex(this.SelectedIndex-1);
                                return false;
                            } else if (key === "Home") {
                                this.setSelectedIndex(0);
                                return false;
                            } else if (key === "End") {
                                this.setSelectedIndex(this.totalItems - 1);
                                return false;
                            } else if (key === "Escape") {
                                this.closePopup(SendSelectEnum.No);
                                return false;
                            } else if (key === "Tab") {
                                this.closePopup(SendSelectEnum.ChangeSinceOpen);
                            } else if (key === "Enter") {
                                this.closePopup(SendSelectEnum.ChangeSinceOpen);
                                return false;
                            }
                        }
                    }
                }
                return true;
            });
        }

        private updateDropDown(): void {

            let value = this.Input.value;
            if (this.Popup) {
                if (value === this.LastValue) return;
            }
            var uri = $YetaWF.parseUrl(this.Setup.AjaxUrl);
            uri.addFormInfo(this.Control);
            uri.addSearch("Search", value);
            $YetaWF.post(uri.toUrl(), uri.toFormData(), (success: boolean, data: SelectionItem[]): void => {
                if (success) {
                    this.LastValue = value;
                    this.openPopup(data);
                }
            });
        }

        private clearInputTimeout(): void {
            if (this.InputTimeout) clearTimeout(this.InputTimeout);
            this.InputTimeout = 0;
        }

        private setSelectedIndex(index: number): void {
            if (!this.Popup) return;
            let total = this.totalItems;
            if (index < 0 || index >= total)
                return;
            this.clearSelectedPopupItem();
            this.selectPopupItem(index);
        }

        get totalItems(): number {
            if (!this.Popup) return 0;
            let lis = $YetaWF.getElementsBySelector("ul li", [this.Popup]);
            return lis.length;
        }

        public sendChangeEvent(): void {
            $YetaWF.sendCustomEvent(this.Control, DropDownSearchEditComponent.EVENTCHANGE);
            FormsSupport.validateElement(this.Hidden);
        }

        private openPopup(list: SelectionItem[]): void {

            this.SelectedIndex = -1;

            if (this.Popup)
                this.closePopup(SendSelectEnum.No);
            if (list.length === 0) return;

            DropDownSearchEditComponent.closeDropdowns();
            this.Popup =
                <div id={DropDownSearchEditComponent.POPUPID} data-owner={this.ControlId} aria-hidden="false">
                    <div class="t_container" data-role="popup" aria-hidden="false">
                        <div class="t_scroller" unselectable="on">
                            <ul unselectable="on" class="t_list" tabindex="-1" aria-hidden="true" aria-live="off" data-role="staticlist" role="listbox">
                            </ul>
                        </div>
                    </div>
                </div> as HTMLElement;

            let ul = $YetaWF.getElement1BySelector("ul", [this.Popup]);
            const len = list.length;
            for (let i = 0; i < len; ++i) {
                let o = list[i];
                let tt = o.Tooltip;
                let li = <li tabindex="-1" role="option" unselectable="on" class="t_item" data-index={i} data-value={o.Value} data-tooltip={tt}>{o.Text}</li>;
                ul.appendChild(li);
            }

            let style = window.getComputedStyle(this.Control);
            this.Popup.style.font = style.font;
            this.Popup.style.fontStyle = style.fontStyle;
            this.Popup.style.fontWeight = style.fontWeight;
            this.Popup.style.fontSize = style.fontSize;

            document.body.appendChild(this.Popup);
            this.Control.setAttribute("aria-expanded", "true");

            this.positionPopup(this.Popup);

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
            if (this.Popup) {
                if (sendEvent === SendSelectEnum.Yes || sendEvent === SendSelectEnum.ChangeSinceOpen) {
                    if (this.SelectedIndex !== -1) {
                        let lis = $YetaWF.getElementsBySelector("ul li", [this.Popup]);
                        let value = $YetaWF.getAttribute(lis[this.SelectedIndex], "data-value");

                        this.Input.value = lis[this.SelectedIndex].innerText;
                        this.Hidden.value = value;
                    } else {
                        this.Input.value = "";
                        this.Hidden.value = "";
                    }
                    this.SelectedIndex = -1;
                    this.sendChangeEvent();
                }
                this.Popup.remove();
                this.Popup = null;
                this.Control.setAttribute("aria-expanded", "false");
            } else {
                if (this.Hidden.value === "")
                    this.Input.value = "";
                this.sendChangeEvent();
            }
        }

        public positionPopup(popup: HTMLElement): void {
            if (!popup) return;
            let ownerId = $YetaWF.getAttribute(popup, "data-owner");
            let control = DropDownSearchEditComponent.getControlById<DropDownSearchEditComponent>(ownerId, DropDownSearchEditComponent.SELECTOR);
            let scroller = $YetaWF.getElement1BySelector(".t_scroller", [popup]);

            let controlRect = control.Control.getBoundingClientRect();
            let desiredHeight = control.Setup.DropDownHeightFactor * DropDownSearchEditComponent.DEFAULTHEIGHT;
            let desiredWidth = control.Setup.DropDownWidthFactor * controlRect.width;
            scroller.style.maxHeight = `${desiredHeight}px`;
            popup.style.width = `${desiredWidth}px`;

            $YetaWF.positionLeftAlignedBelow(this.Control, popup);
        }

        private selectPopupItem(index: number): void {

            this.SelectedIndex = index;

            if (!this.Popup) return;
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

        private clearSelectedPopupItem(): void {
            if (this.Popup) {
                let lis = $YetaWF.getElementsBySelector("ul li.t_selected", [this.Popup]);
                for (const li of lis) {
                    $YetaWF.elementRemoveClasses(li, ["t_selected", "t_focused"]);
                    $YetaWF.setAttribute(li, "aria-selected", "false");
                    li.id = "";
                }
            }
        }

        public static closeDropdowns(): void {
            let popup = $YetaWF.getElementByIdCond(DropDownSearchEditComponent.POPUPID);
            if (!popup) return;
            let ownerId = $YetaWF.getAttribute(popup, "data-owner");
            let control = DropDownSearchEditComponent.getControlById<DropDownSearchEditComponent>(ownerId, DropDownSearchEditComponent.SELECTOR);
            control.closePopup(SendSelectEnum.No);
        }

        // API

        get value(): string {
            return this.Hidden.value;
        }
        set value(val: string) {
            this.Hidden.value = val;
        }
        get text(): string {
            return this.Input.value;
        }
        set text(val: string) {
            this.Input.value = val;
        }
        public clear(): void {
            this.closePopup(SendSelectEnum.No);
            this.Input.value = "";
            this.Hidden.value = "";
        }
        get enabled(): boolean {
            return this.Enabled;
        }
        public enable(enabled: boolean): void {
            this.closePopup(SendSelectEnum.No);
            $YetaWF.elementEnableToggle(this.Input, enabled);
            $YetaWF.elementRemoveClass(this.Control, "t_disabled");
            if (!enabled)
                $YetaWF.elementAddClass(this.Control, "t_disabled");
            this.Enabled = enabled;
        }
    }

    // close dropdown when clicking outside
    $YetaWF.registerEventHandlerBody("click", null, (ev: MouseEvent): boolean => {
        DropDownSearchEditComponent.closeDropdowns();
        return true;
    });

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, (ev: Event): boolean => {
        DropDownSearchEditComponent.closeDropdowns();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: Event): boolean => {
        DropDownSearchEditComponent.closeDropdowns();
        return true;
    });
}

