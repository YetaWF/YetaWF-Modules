/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* Basics implementation required by YetaWF */

namespace YetaWF_ComponentsHTML {

    export class BasicsImpl implements YetaWF.IBasicsImpl {

        // LOADING
        // LOADING
        // LOADING

        public setLoading(on?: boolean): void {
            if (on !== false) {
                ($ as any).prettyLoader.show();
            } else {
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
            this.alert(message, title|| YLocs.Basics.DefaultSuccessTitle, onOK, options);
        }
        /**
         * Displays an error message, usually in a popup.
         */
        public error(message: string, title?: string, onOK?: () => void, options?: YetaWF.MessageOptions): void {
            this.alert(message, title || YLocs.Basics.DefaultErrorTitle, onOK);
        }
        /**
         * Displays a confirmation message, usually in a popup.
         */
        public confirm(message: string, title?: string, onOK?: () => void, options?: YetaWF.MessageOptions): void {
            this.alert(message, title || YLocs.Basics.DefaultSuccessTitle, onOK);
        }
        /**
         * Displays an alert message, usually in a popup.
         */
        public alert(message: string, title?: string, onOK?: () => void, options?: YetaWF.MessageOptions): void {
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

            $dialog.dialog({
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

            $dialog.dialog({
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
        }
        /**
         * Displays a "Please Wait" message
         */
        public pleaseWait(message?: string, title?: string): void {
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
         * Closes any open overlays, menus, dropdownlists, etc. (Popup windows are not handled and are explicitly closed using $YetaWF.Popups)
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
        }
    }
}

// tslint:disable-next-line:variable-name
var YetaWF_BasicsImpl: YetaWF.IBasicsImpl = new YetaWF_ComponentsHTML.BasicsImpl();
