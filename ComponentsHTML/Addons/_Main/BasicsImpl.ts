/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* Basics implementation required by YetaWF */

namespace YetaWF {
    export interface IConfigs {
        YetaWF_ComponentsHTML: YetaWF_ComponentsHTML.IPackageConfigs;
    }
}
namespace YetaWF_ComponentsHTML {
    export interface IPackageConfigs {
        SVG_fas_multiply: string; // a close button
    }
}

namespace YetaWF_ComponentsHTML {

    enum Severity {
        Info,
        Warning,
        Error,
    }

    interface ToastEntry {
        Severity: Severity;
        Title: string;
        Text: string;
        Timeout: number;
        CanClose: boolean;
        Name: string | undefined;
        EntryDiv: HTMLDivElement | null;
    }

    export class BasicsImpl implements YetaWF.IBasicsImpl {

        public static readonly ToastDivSelector: string = "#ytoast";
        public static readonly DefaultTimeout:number = 7000;

        ToastDiv: HTMLDivElement|null = null;

        // PAGE INITIALIZATION
        // PAGE INITIALIZATION
        // PAGE INITIALIZATION

        /** Called when a new full page has been loaded and needs to be initialized */
        public initFullPage(): void {  }

        // LOADING
        // LOADING
        // LOADING

        private loading: boolean = false;

        public get isLoading(): boolean {
            return this.loading;
        }

        public setLoading(on?: boolean): void {
            if (on !== false) {
                this.loading = true;
                LoadingSupport.show();
            } else {
                this.loading = false;
                LoadingSupport.hide();
            }
        }

        // MESSAGES
        // MESSAGES
        // MESSAGES

        /**
         * Displays an informational message, usually in a popup.
         */
        public message(message: string, title?: string, onOK?: () => void, options?: YetaWF.MessageOptions): void {
            if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.Popups || YVolatile.Basics.ForcePopup) {
                this.alert(message, title || YLocs.Basics.DefaultSuccessTitle, onOK, options);
            } else {
                if (!options)
                    options = { encoded: false };
                if (options.canClose === undefined) options.canClose = true;
                if (options.autoClose === undefined) options.autoClose = BasicsImpl.DefaultTimeout;
                this.addToast(Severity.Info, title ?? YLocs.Basics.DefaultSuccessTitle, message, options);
                if (onOK) onOK();
            }
        }
        /**
         * Displays an warning message, usually in a popup.
         */
        public warning(message: string, title?: string, onOK?: () => void, options?: YetaWF.MessageOptions): void {
            if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.Popups || YVolatile.Basics.ForcePopup) {
                this.alert(message, title || YLocs.Basics.DefaultAlertTitle, onOK);
            } else {
                if (!options)
                    options = { encoded: false };
                if (options.canClose === undefined) options.canClose = true;
                if (options.autoClose === undefined) options.autoClose = BasicsImpl.DefaultTimeout;
                this.addToast(Severity.Warning, title ?? YLocs.Basics.DefaultAlertTitle, message, options);
                if (onOK) onOK();
            }
        }
        /**
         * Displays an error message, usually in a popup.
         */
        public error(message: string, title?: string, onOK?: () => void, options?: YetaWF.MessageOptions): void {
            if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.Popups || YVolatile.Basics.ForcePopup) {
                this.alert(message, title || YLocs.Basics.DefaultErrorTitle, onOK);
            } else {
                if (!options)
                    options = { encoded: false };
                if (options.canClose === undefined) options.canClose = true;
                if (options.autoClose === undefined) options.autoClose = BasicsImpl.DefaultTimeout;
                this.addToast(Severity.Error, title ?? YLocs.Basics.DefaultErrorTitle, message, options);
                if (onOK) onOK();
            }
        }
        /**
         * Displays a confirmation message, usually in a popup.
         */
        public confirm(message: string, title?: string, onOK?: () => void, options?: YetaWF.MessageOptions): void {
            if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.Popups || YVolatile.Basics.ForcePopup) {
                this.alert(message, title || YLocs.Basics.DefaultSuccessTitle, onOK);
            } else {
                if (!options)
                    options = { encoded: false };
                if (options.canClose === undefined) options.canClose = true;
                if (options.autoClose === undefined) options.autoClose = BasicsImpl.DefaultTimeout;
                this.addToast(Severity.Info, title ?? YLocs.Basics.DefaultSuccessTitle, message, options);
                if (onOK) onOK();
            }
        }
        /**
         * Displays an alert message, usually in a popup.
         */
        private alert(message: string, title?: string, onOK?: () => void, options?: YetaWF.MessageOptions): void {

            if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.Popups || YVolatile.Basics.ForcePopup) {

                YVolatile.Basics.ForcePopup = false;

                // check if we already have a popup (and close it)
                DialogClass.close();

                options = options || { encoded: false };
                if (!options.encoded) {
                    // change (+nl) to <br/>
                    let escElement: HTMLDivElement = document.createElement("div");
                    escElement.innerText = message;
                    let s = escElement.innerHTML;
                    escElement.remove();
                    message = s.replace(/\(\+nl\)/g, "<br/>");
                }

                DialogClass.open({
                    width: YConfigs.Basics.DefaultAlertWaitWidth,
                    height: YConfigs.Basics.DefaultAlertWaitHeight,
                    title: title ?? YLocs.Basics.DefaultAlertTitle,
                    textHTML: message,
                    closeText: YLocs.Basics.CloseButtonText,
                    close: (): void => {
                        if (onOK)
                            onOK();
                    },
                    buttons: [
                        {
                            text: YLocs.Basics.OKButtonText,
                            click: (): void => {
                                if (onOK)
                                    onOK();
                            },
                        },
                    ],
                });
            } else {
                if (!options)
                    options = { encoded: false };
                if (options.canClose === undefined) options.canClose = true;
                if (options.autoClose === undefined) options.autoClose = BasicsImpl.DefaultTimeout;
                this.addToast(Severity.Info, title ?? "Success", message, options);
                if (onOK) onOK();
            }
        }

        /**
         * Displays an alert message with Yes/No buttons, usually in a popup.
         */
        public alertYesNo(message: string, title?: string, onYes?: () => void, onNo?: () => void, options?: YetaWF.MessageOptions): void {

            // change (+nl) to <br/>
            let escElement: HTMLDivElement = document.createElement("div");
            escElement.innerText = message;
            let s = escElement.innerHTML;
            escElement.remove();
            message = s.replace(/\(\+nl\)/g, "<br/>");

            DialogClass.open({
                width: YConfigs.Basics.DefaultAlertYesNoWidth,
                height: YConfigs.Basics.DefaultAlertYesNoHeight,
                title: title ?? YLocs.Basics.DefaultAlertYesNoTitle,
                textHTML: message,
                closeText: YLocs.Basics.CloseButtonText,
                close: ():void => {
                    if (onNo)
                        onNo();
                },
                buttons: [
                    {
                        text: YLocs.Basics.YesButtonText,
                        click: (): void => {
                            if (onYes)
                                onYes();
                        },
                    },
                    {
                        text: YLocs.Basics.NoButtonText,
                        click: (): void => {
                            if (onNo)
                                onNo();
                        },
                    }
                ],
            });
        }

        /**
         * Displays a "Please Wait" message
         */
        public pleaseWait(message?: string, title?: string): void {

            let escElement: HTMLDivElement = document.createElement("div");
            escElement.innerText = message ?? YLocs.Basics.PleaseWaitText;
            message = escElement.innerHTML;
            escElement.remove();

            // show and center the window
            DialogClass.openSimple({
                width: YConfigs.Basics.DefaultPleaseWaitWidth,
                height: YConfigs.Basics.DefaultPleaseWaitHeight,
                title: title ?? YLocs.Basics.PleaseWaitTitle,
                textHTML: message,
            });
        }
        /**
         * Closes the "Please Wait" message (if any).
         */
        public pleaseWaitClose(): void {
            DialogClass.close();
        }
        /**
         * Closes any open overlays, menus, dropdownlists, tooltips, etc. (Popup windows are not handled and are explicitly closed using $YetaWF.Popups)
         */
        public closeOverlays(): void {

            // all MenuUL menus
            if (MenuULComponent)
                MenuULComponent.closeMenus();

            // tooltips
            ToolTipsHTMLHelper.removeTooltips();

            // dropdowns
            if (DropDownListEditComponent)
                DropDownListEditComponent.closeDropdowns();

            // Close any open menus
            if (YetaWF_ComponentsHTML.MenuComponent)
                YetaWF_ComponentsHTML.MenuComponent.closeAllMenus();
        }
        /**
         * Enable/disable an element.
         * Some child items need some extra settings when disabled=disabled isn't enough.
         * Also used to update visual styles to reflect the status.
         */
        public elementEnableToggle(elem: HTMLElement, enable: boolean): void {

            elem.removeAttribute("disabled");
            if (!enable)
                elem.setAttribute("disabled", "disabled");

            if (elem.tagName === "A") {
                $YetaWF.elementRemoveClass(elem, "t_disabled");
                if (!enable)
                    $YetaWF.elementAddClass(elem, "t_disabled");
            }

            // mark submit/nosubmit
            if (enable) {
                if ($YetaWF.elementHasClass(elem, "yform-nosubmit-temp"))
                    $YetaWF.elementRemoveClasses(elem, ["yform-novalidate", "yform-nosubmit-temp", "yform-nosubmit"]);
            } else {
                if (!$YetaWF.elementHasClass(elem, "yform-nosubmit"))
                    $YetaWF.elementAddClasses(elem, ["yform-novalidate", "yform-nosubmit-temp", "yform-nosubmit"]);
            }
        }

        /**
         * Returns whether the element is enabled.
         */
        public isEnabled(elem: HTMLElement): boolean {
            if ($YetaWF.getAttributeCond(elem, "disabled") != null)
                return false;
            if ($YetaWF.elementHasClass(elem, "t_disabled"))
                return false;
            return true;
        }

        /**
         * Given an element, returns the owner (typically a module) that owns the element.
         * The DOM hierarchy may not reflect this ownership, for example with popup menus which are appended to the <body> tag, but are owned by specific modules.
         */
        public getOwnerFromTag(tag: HTMLElement): HTMLElement | null {
            if (MenuULComponent)
                return MenuULComponent.getOwnerFromTag(tag);
            return null;
        }

        /**
         * Returns whether a message popup dialog is currently active.
         */
        public messagePopupActive(): boolean {
            return DialogClass.isActive;
        }

        // TOAST

        private Toasts: ToastEntry[] = [];

        private addToast(severity: Severity, title: string, message: string, options: YetaWF.MessageOptions) : void {

            let entry: ToastEntry = {
                Severity: severity,
                Title: title,
                Text: message,
                CanClose: options.canClose === true,
                Timeout: options.autoClose ? options.autoClose : 0,
                Name: options.name,
                EntryDiv: null,
            };

            if (entry.Name) {
                let found = this.Toasts.find((toast: ToastEntry): boolean => {
                    return entry.Name === toast.Name;
                });
                if (found) {
                    if (entry.Text === found.Text) // same text, leave as is
                        return;
                    else {
                        // new text, replace
                        this.removeToast(found.EntryDiv!, found);
                    }
                }
            }
            if (!entry.Text) // empty text
                return;

            this.Toasts.push(entry);
            let entryDiv = document.createElement("div");
            entry.EntryDiv = entryDiv;
            $YetaWF.setAttribute(entryDiv, "aria-atomic", "true");
            let html = "";
            if (title)
                html += `<div class='t_title'>${$YetaWF.htmlEscape(title)}</div>`;
            if (options.canClose)
                //close button image
                html += `<div class='t_close' aria-label='Close'>${YConfigs.YetaWF_ComponentsHTML.SVG_fas_multiply}</div>`;
            if (message) {
                if (!options.encoded) {
                    // change \n to <br/>
                    let s = $YetaWF.htmlEscape(message);
                    s = s.replace(/\(\+nl\)/g, "<br/>");
                    html += `<div class='t_message'>${s}</div>`;
                } else {
                    html += `<div class='t_message'>${message}</div>`;
                }
            }
            entryDiv.innerHTML = html;
            let toastDiv = this.getToastDiv();
            toastDiv.appendChild(entryDiv);

            $YetaWF.elementAddClass(entryDiv, "t_entry");
            switch (severity) {
                default:
                case Severity.Info:
                    $YetaWF.setAttribute(entryDiv, "role", "status");
                    $YetaWF.setAttribute(entryDiv, "aria-live", "polite");
                    $YetaWF.elementAddClass(entryDiv, "t_info");
                    break;
                case Severity.Warning:
                    $YetaWF.setAttribute(entryDiv, "role", "alert");
                    $YetaWF.setAttribute(entryDiv, "aria-live", "assertive");
                    $YetaWF.elementAddClass(entryDiv, "t_warning");
                    break;
                case Severity.Error:
                    $YetaWF.setAttribute(entryDiv, "role", "alert");
                    $YetaWF.setAttribute(entryDiv, "aria-live", "assertive");
                    $YetaWF.elementAddClass(entryDiv, "t_error");
                    break;
            }

            if (options.canClose) {
                $YetaWF.registerEventHandler(entryDiv, "click", ".t_close", (ev: MouseEvent): boolean => {
                    this.removeToast(entryDiv, entry);
                    return false;
                });
            }

            if (options.autoClose) {
                setTimeout((): void => {
                    this.removeToast(entryDiv, entry);
                }, options.autoClose);
            }
        }
        private removeToast(entryDiv: HTMLElement, entry: ToastEntry): void {
            entryDiv.remove();
            this.Toasts = this.Toasts.filter((e: ToastEntry): boolean => { return e !== entry; });
            if (this.Toasts.length === 0) {
                let toastDiv = $YetaWF.getElement1BySelectorCond(BasicsImpl.ToastDivSelector) as HTMLDivElement | null;
                if (toastDiv)
                    toastDiv.remove();
            }
        }
        private getToastDiv(): HTMLDivElement {
            // if we're in an iframe popup, find outer window to add toast div
            // if we're not in an iframe, window.parent simply returns window, so we're all good
            let doc = window.parent.document;

            let toastDiv = $YetaWF.getElement1BySelectorCond(BasicsImpl.ToastDivSelector, [doc.body]) as HTMLDivElement | null;
            if (!toastDiv) {
                toastDiv = document.createElement("div");
                if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.ToastRight)
                    $YetaWF.elementAddClass(toastDiv, "t_right");
                else
                    $YetaWF.elementAddClass(toastDiv, "t_left");
                toastDiv.id = "ytoast";
                $YetaWF.setAttribute(toastDiv, "aria-live", "polite");
                $YetaWF.setAttribute(toastDiv, "aria-atomic", "true");
                doc.body.appendChild(toastDiv);
            }
            return toastDiv;
        }
    }
}

// tslint:disable-next-line:variable-name
var YetaWF_BasicsImpl: YetaWF.IBasicsImpl = new YetaWF_ComponentsHTML.BasicsImpl();
