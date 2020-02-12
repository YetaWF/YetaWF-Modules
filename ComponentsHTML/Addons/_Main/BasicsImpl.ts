/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* Basics implementation required by YetaWF */

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
    }

    export class BasicsImpl implements YetaWF.IBasicsImpl {

        public static readonly DefaultTimeout:number = 7000;

        ToastDiv: HTMLDivElement|null = null;

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
                ($ as any).prettyLoader.show();
            } else {
                this.loading = false;
                ($ as any).prettyLoader.hide();
            }
        }

        // MESSAGES
        // MESSAGES
        // MESSAGES

        /**
         * Displays an informational message, usually in a popup.
         */
        public message(message: string, title?: string, onOK?: () => void, options?: YetaWF.MessageOptions): void {
            if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.Popups)
                this.alert(message, title || YLocs.Basics.DefaultSuccessTitle, onOK, options);
            else {
                if (!options)
                    options = { encoded: false };
                if (options.canClose === undefined) options.canClose = true;
                if (options.autoClose === undefined) options.autoClose = BasicsImpl.DefaultTimeout;
                this.addToast(Severity.Info, title ?? YLocs.Basics.DefaultSuccessTitle, message, options);
                if (onOK) onOK();
            }
        }
        /**
         * Displays an error message, usually in a popup.
         */
        public error(message: string, title?: string, onOK?: () => void, options?: YetaWF.MessageOptions): void {
            if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.Popups)
                this.alert(message, title || YLocs.Basics.DefaultErrorTitle, onOK);
            else {
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
            if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.Popups)
                this.alert(message, title || YLocs.Basics.DefaultSuccessTitle, onOK);
            else {
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
        public alert(message: string, title?: string, onOK?: () => void, options?: YetaWF.MessageOptions): void {

            if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.Popups) {

                ComponentsHTMLHelper.REQUIRES_JQUERYUI((): void => {

                    // check if we already have a popup (and close it)
                    this.closeAlert(onOK);

                    $("body").prepend("<div id='yalert'></div>");
                    const $dialog = $("#yalert");

                    options = options || { encoded: false };

                    if (!options.encoded) {
                        // change \n to <br/>
                        $dialog.text(message);
                        var s = $dialog.html();
                        s = s.replace(/\(\+nl\)/g, "<br/>");
                        $dialog.html(s);
                    } else {
                        $dialog.html(message);
                    }

                    if (title === undefined)
                        title = YLocs.Basics.DefaultAlertTitle;

                    $dialog.dialog({ //jQuery-ui use
                        autoOpen: true,
                        modal: true,
                        width: YConfigs.Basics.DefaultAlertWaitWidth,
                        height: YConfigs.Basics.DefaultAlertWaitHeight === 0 ? "auto" : YConfigs.Basics.DefaultAlertWaitHeight,
                        closeOnEscape: true,
                        closeText: YLocs.Basics.CloseButtonText,
                        close: (event: Event, ui: JQueryUI.DialogUIParams): void => this.closeAlert(onOK),
                        draggable: true,
                        resizable: false,
                        title: title,
                        buttons: [{
                            text: YLocs.Basics.OKButtonText,
                            click: (eventObject: JQueryEventObject): any => {
                                $dialog.dialog("close");
                            }
                        }]
                    });
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

        private closeAlert(onOK?: () => void): void {
            const $dialog = $("#yalert");
            if ($dialog.length === 0) return;
            if ($dialog.attr("data-closing")) return;
            $dialog.attr("data-closing", 1);
            const endFunc = onOK;
            onOK = undefined; // clear this so close function doesn't call onOK handler also
            $dialog.dialog("close");
            $dialog.dialog("destroy");
            $dialog.remove();
            if (endFunc)
                endFunc();
        }

        /**
         * Displays an alert message with Yes/No buttons, usually in a popup.
         */
        public alertYesNo(message: string, title?: string, onYes?: () => void, onNo?: () => void, options?: YetaWF.MessageOptions): void {

            ComponentsHTMLHelper.REQUIRES_JQUERYUI((): void => {

                const $body = $("body");
                $body.prepend("<div id='yalert'></div>");
                const $dialog = $("#yalert", $body);

                // change \n to <br/>
                $dialog.text(message);
                var s = $dialog.html();
                s = s.replace(/\(\+nl\)/g, "<br/>");
                $dialog.html(s);

                if (title === undefined)
                    title = YLocs.Basics.DefaultAlertYesNoTitle;

                $dialog.dialog({ //jQuery-ui use
                    autoOpen: true,
                    modal: true,
                    width: YConfigs.Basics.DefaultAlertYesNoWidth,
                    height: YConfigs.Basics.DefaultAlertYesNoHeight === 0 ? "auto" : YConfigs.Basics.DefaultAlertYesNoHeight,
                    closeOnEscape: true,
                    closeText: YLocs.Basics.CloseButtonText,
                    close: (event: Event, ui: JQueryUI.DialogUIParams): void => {
                        $dialog.dialog("destroy");
                        $dialog.remove();
                        if (onNo !== undefined)
                            onNo();
                    },
                    draggable: true,
                    resizable: false,
                    title: title,
                    buttons: [
                        {
                            text: YLocs.Basics.YesButtonText,
                            click: (eventObject: JQueryEventObject): any => {
                                const endFunc = onYes;
                                onYes = undefined;// clear this so close function doesn't try do call these
                                onNo = undefined;
                                $dialog.dialog("destroy");
                                $dialog.remove();
                                if (endFunc)
                                    endFunc();
                            }
                        },
                        {
                            text: YLocs.Basics.NoButtonText,
                            click: (eventObject: JQueryEventObject): any => {
                                const endFunc = onNo;
                                onYes = undefined;// clear this so close function doesn't try do call these
                                onNo = undefined;
                                $dialog.dialog("destroy");
                                $dialog.remove();
                                if (endFunc)
                                    endFunc();
                            }
                        }
                    ],
                });
            });
        }
        /**
         * Displays a "Please Wait" message
         */
        public pleaseWait(message?: string, title?: string): void {

            ComponentsHTMLHelper.REQUIRES_KENDOUI((): void => {

                // insert <div id="yplwait"></div> at top of page for the window
                // this is automatically removed when destroy() is called
                $("body").prepend("<div id='yplwait'></div>");
                const $popupwin = $("#yplwait");
                var popup: kendo.ui.Window | null = null;

                if (message === undefined)
                    message = YLocs.Basics.PleaseWaitText;
                if (title === undefined)
                    title = YLocs.Basics.PleaseWaitTitle;
                $popupwin.text(<string>message);

                // Create the window
                $popupwin.kendoWindow({
                    actions: [],
                    width: YConfigs.Basics.DefaultPleaseWaitWidth,
                    height: YConfigs.Basics.DefaultPleaseWaitHeight,
                    draggable: true,
                    iframe: true,
                    modal: true,
                    resizable: false,
                    title: $YetaWF.htmlEscape(title),
                    visible: false,
                    close: (event: kendo.ui.WindowCloseEvent): void => {
                        var popup: kendo.ui.Window | null = $popupwin.data("kendoWindow");
                        popup.destroy();
                        popup = null;
                    },
                });

                // show and center the window
                popup = $popupwin.data("kendoWindow");
                popup.open().center();

            });
        }
        /**
         * Closes the "Please Wait" message (if any).
         */
        public pleaseWaitClose(): void {
            const $popupwin = $("#yplwait");
            if ($popupwin.length === 0) return;
            const popup = $popupwin.data("kendoWindow");
            popup.destroy();
        }
        /**
         * Closes any open overlays, menus, dropdownlists, tooltips, etc. (Popup windows are not handled and are explicitly closed using $YetaWF.Popups)
         */
        public closeOverlays(): void {

            // Close open bootstrap nav menus (if any) by clicking on the page
            $("body").trigger("click");
            // Close any open kendo menus (if any)
            const $menus = $(".k-menu");
            $menus.each((index: number, element: HTMLElement): void => {
                const menu = $(element).data("kendoMenu");
                menu.close("li.k-item");
            });
            // Close any open smartmenus
            try {
                ($(".YetaWF_Menus") as any).collapse("hide");
            } catch (e) { }

            // tooltips
            ToolTipsHTMLHelper.removeTooltips();
        }
        /**
         * Enable/disable an element.
         * Some child items need some extra settings when disabled=disabled isn't enough.
         * Also used to update visual styles to reflect the status.
         */
        public elementEnableToggle(elem: HTMLElement, enable: boolean): void {

            if (YVolatile.YetaWF_ComponentsHTML.jqueryUI && $YetaWF.elementHasClass(elem, "ui-button")) {
                // Handle buttons
                // jquery-ui button
                elem.removeAttribute("disabled");
                if (!enable)
                    elem.setAttribute("disabled", "disabled");
                $(elem).button(enable ? "enable" : "disable");
            } else if (TextEditComponent && $YetaWF.elementHasClass(elem, TextEditComponent.TEMPLATE)) { // using template name as class name
                // Handle text/input
                elem.removeAttribute("disabled");
                if (!enable)
                    elem.setAttribute("disabled", "disabled");
                $YetaWF.elementRemoveClass(elem, "k-state-disabled");
                if (!enable)
                    $YetaWF.elementAddClass(elem, "k-state-disabled");
            } else if (TextAreaSourceOnlyEditComponent && $YetaWF.elementHasClass(elem, TextAreaSourceOnlyEditComponent.TEMPLATE)) { // using template name as class name
                elem.removeAttribute("readonly");
                $YetaWF.elementRemoveClass(elem, "k-state-disabled");
                if (!enable) {
                    elem.setAttribute("readonly", "readonly");
                    $YetaWF.elementAddClass(elem, "k-state-disabled");
                }
            } else {
                elem.removeAttribute("disabled");
                if (!enable)
                    elem.setAttribute("disabled", "disabled");
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

        // TOAST

        private Toasts: ToastEntry[] = [];

        private addToast(severity: Severity, title: string, message: string, options: YetaWF.MessageOptions) : void {
            let toastDiv = this.getToastDiv();

            let entry = {
                Severity: severity,
                Title: title,
                Text: message,
                CanClose: options.canClose === true,
                Timeout: options.autoClose ? options.autoClose : 0
            };
            this.Toasts.push(entry);
            let entryDiv = document.createElement("div");
            let html = "";
            if (title)
                html += `<div class='t_title'>${$YetaWF.htmlEscape(title)}</div>`;
            if (options.canClose)
                html += `<div class='t_close'></div>`;
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
            toastDiv.appendChild(entryDiv);

            $YetaWF.elementAddClass(entryDiv, "t_entry");
            switch (severity) {
                default:
                case Severity.Info:
                    $YetaWF.elementAddClass(entryDiv, "t_info");
                    break;
                case Severity.Warning:
                    $YetaWF.elementAddClass(entryDiv, "t_warning");
                    break;
                case Severity.Error:
                    $YetaWF.elementAddClass(entryDiv, "t_error");
                    break;
            }

            if (options.canClose) {
                $YetaWF.registerEventHandler(entryDiv, "click", ".t_close", (ev: MouseEvent): boolean => {
                    entryDiv.remove();
                    this.Toasts = this.Toasts.filter((e:ToastEntry):boolean => { return e !== entry; });
                    return false;
                });
            }

            if (options.autoClose) {
                setTimeout(() => {
                    entryDiv.remove();
                    this.Toasts = this.Toasts.filter((e:ToastEntry):boolean => { return e !== entry; });
                }, options.autoClose);
            }
        }
        private getToastDiv(): HTMLDivElement {
            let toastDiv = $YetaWF.getElement1BySelectorCond("#ytoast") as HTMLDivElement|null;
            if (!toastDiv) {
                toastDiv = document.createElement("div");
                if (YConfigs.Basics.MessageType === YetaWF.MessageTypeEnum.ToastRight)
                    $YetaWF.elementAddClass(toastDiv, "t_right");
                else
                    $YetaWF.elementAddClass(toastDiv, "t_left");
                toastDiv.id = "ytoast";
                document.body.appendChild(toastDiv);
            }
            return toastDiv;
        }
    }
}

// tslint:disable-next-line:variable-name
var YetaWF_BasicsImpl: YetaWF.IBasicsImpl = new YetaWF_ComponentsHTML.BasicsImpl();
