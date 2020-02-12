/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* Popups implementation required by YetaWF */

interface Document {
    YPopupWindowActive: kendo.ui.Window | null;
}

namespace YetaWF_ComponentsHTML {

    export class PopupsImpl implements YetaWF.IPopupsImpl {

        /**
         * Close the popup - this can only be used by code that is running within the popup (not the parent document/page)
         */
        public closePopup(forceReload?: boolean): void {
            if (YVolatile.Basics.IsInPopup) {
                var forced = (forceReload === true);
                if (forced)
                    $YetaWF.reloadPage(true, window.parent);
                // with unified page sets there may actually not be a parent, but window.parent returns itself in this case anyway
                var popup: kendo.ui.Window | null = window.parent.document.YPopupWindowActive;
                (window.parent as any).YVolatile.Basics.IsInPopup = false;
                window.parent.document.YPopupWindowActive = null;
                PopupsImpl.internalClosePopup(popup, true);
            }
        }

        /**
         * Close the popup - this can only be used by code that is running on the main page (not within the popup)
         */
        public closeInnerPopup(): void {
            var popup: kendo.ui.Window | null = document.YPopupWindowActive;
            PopupsImpl.internalClosePopup(popup, true);

        }

        private static internalClosePopup(popup: kendo.ui.Window | null, close: boolean): void {
            if (popup) {
                if (close)
                    popup.close();
                popup.destroy();
            }
            document.YPopupWindowActive = null;
            YVolatile.Basics.IsInPopup = false; // we're no longer in a popup
        }

        /**
         * Opens a dynamic popup, usually a div added to the current document.
         */
        public openDynamicPopup(result: YetaWF.ContentResult, done:(dialog:HTMLElement) => void): void {

            ComponentsHTMLHelper.REQUIRES_KENDOUI((): void => {

                // we're already in a popup
                PopupsImpl.closeDynamicPopup();

                YVolatile.Basics.IsInPopup = true; // we're in a popup

                // insert <div id="ypopup" class='yPopupDyn'></div> at top of page for the popup window
                // this is automatically removed when destroy() is called
                $("body").prepend("<div id='ypopup' class='yPopupDyn'></div>");
                var $popupwin = $("#ypopup");
                $popupwin.addClass(YVolatile.Skin.PopupCss);

                // add pane content
                var contentLength = result.Content.length;
                for (var i = 0; i < contentLength; i++) {
                    // add the pane
                    var $pane = $("<div class='yPane'></div>").addClass(result.Content[i].Pane);
                    $pane.append(result.Content[i].HTML);
                    $popupwin.append($pane);
                }

                var popup: kendo.ui.Window | null = null;

                var acts: string[] = [];
                if (YVolatile.Skin.PopupMaximize)
                    acts.push("Maximize");
                acts.push("Close");

                // Create the window
                $popupwin.kendoWindow({
                    actions: acts,
                    width: YVolatile.Skin.PopupWidth,
                    height: "auto",
                    maxHeight: YVolatile.Skin.PopupHeight,
                    draggable: true,
                    iframe: false,
                    modal: true,
                    resizable: false,
                    title: result.PageTitle,
                    visible: false,
                    close: (e: kendo.ui.WindowCloseEvent): void => {
                        PopupsImpl.closeDynamicPopup();
                    },
                    animation: false,
                    refresh: (e: kendo.ui.WindowEvent): void => { // page complete
                        $YetaWF.setLoading(false);
                    },
                    error: (e: kendo.ui.WindowErrorEvent): void => {
                        $YetaWF.setLoading(false);
                        $YetaWF.error("Request failed with status " + e.status);
                    }
                });

                // show and center the window
                popup = $popupwin.data("kendoWindow");
                popup.center().open();

                // mark that a popup is active
                (document as any).expando = true;
                document.YPopupWindowActive = popup;

                $YetaWF.setCondense($popupwin[0], YVolatile.Skin.PopupWidth);

                done($popupwin[0]);
            });
        }

        private static closeDynamicPopup(): void {
            let popup: kendo.ui.Window | null = null;
            let popupElem = $YetaWF.getElement1BySelectorCond("#ypopup");
            if (popupElem) {
                $YetaWF.processClearDiv(popupElem);
                popup = $(popupElem).data("kendoWindow") as kendo.ui.Window;
            }
            // don't call internalClosePopup, otherwise we get close event
            PopupsImpl.internalClosePopup(popup, false);
        }


        /**
         * Open a static popup, usually a popup based on iframe.
         */
        public openStaticPopup(url: string): void {

            ComponentsHTMLHelper.REQUIRES_KENDOUI((): void => {

                // we're already in a popup
                if ($YetaWF.isInPopup()) {
                    // we handle links within a popup by replacing the current popup page with the new page
                    $YetaWF.setLoading(true);
                    let $popupwin = $("#ypopup", $(window.parent.document));
                    if ($popupwin.length === 0) throw "Couldn't find popup window";/*DEBUG*/
                    let iframeDomElement = $popupwin.children("iframe")[0] as HTMLIFrameElement;
                    if (iframeDomElement) {
                        // we aleady have a static popup
                        iframeDomElement.src = url;
                        return;
                    } else {
                        // we had a dynamic popup, close it and build static popup
                        PopupsImpl.closeDynamicPopup();
                    }
                }

                YVolatile.Basics.IsInPopup = true; // we're in a popup

                // insert <div id="ypopup"></div> at top of page for the popup window
                // this is automatically removed when destroy() is called
                $("body").prepend("<div id='ypopup'></div>");
                var $popupwin = $("#ypopup") as JQuery<HTMLElement>;
                var popup: kendo.ui.Window | null = null;

                var acts: string[] = [];
                acts.push("Maximize");// always show the maximize button - hide it based on popup skin options
                acts.push("Close");

                // Create the window
                $popupwin.kendoWindow({
                    actions: acts,
                    width: YConfigs.Popups.DefaultPopupWidth,
                    height: YConfigs.Popups.DefaultPopupHeight,
                    draggable: true,
                    iframe: true,
                    modal: true,
                    resizable: false,
                    title: " ", //title is set later once contents are available
                    visible: false,
                    content: url as kendo.ui.WindowContent, //Hello, this is not really WindowContent, but d.ts needs WindowContent
                    close: (e: kendo.ui.WindowCloseEvent): void => {
                        var popup: kendo.ui.Window | null = $popupwin.data("kendoWindow");
                        // don't call internalClosePopup, otherwise we get close event
                        PopupsImpl.internalClosePopup(popup, false);
                    },
                    animation: false,
                    refresh: (e: kendo.ui.WindowEvent): void => { // page complete
                        var iframeDomElement: HTMLIFrameElement | null = $popupwin.children("iframe")[0] as HTMLIFrameElement;
                        if (iframeDomElement && iframeDomElement.contentDocument && popup) {
                            var iframeDocumentObject: Document = iframeDomElement.contentDocument;
                            popup.title(iframeDocumentObject.title);
                        }
                        $YetaWF.setLoading(false);
                    },
                    error: (e: kendo.ui.WindowErrorEvent): void => {
                        $YetaWF.setLoading(false);
                        $YetaWF.error("Request failed with status " + e.status);
                    }
                });

                // show and center the window
                popup = $popupwin.data("kendoWindow");
                //do not open the window here - the loaded content opens it because it knows the desired size
                //popup.center().open();

                // mark that a popup is active
                (document as any).expando = true;
                document.YPopupWindowActive = popup;
            });
        }
    }
}

// tslint:disable-next-line:variable-name
var YetaWF_PopupsImpl: YetaWF.IPopupsImpl = new YetaWF_ComponentsHTML.PopupsImpl();

