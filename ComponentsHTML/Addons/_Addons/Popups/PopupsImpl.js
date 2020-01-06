"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var PopupsImpl = /** @class */ (function () {
        function PopupsImpl() {
        }
        /**
         * Close the popup - this can only be used by code that is running within the popup (not the parent document/page)
         */
        PopupsImpl.prototype.closePopup = function (forceReload) {
            if (YVolatile.Basics.IsInPopup) {
                var forced = (forceReload === true);
                if (forced)
                    $YetaWF.reloadPage(true, window.parent);
                // with unified page sets there may actually not be a parent, but window.parent returns itself in this case anyway
                PopupsImpl.internalClosePopup();
            }
        };
        /**
         * Close the popup - this can only be used by code that is running on the main page (not within the popup)
         */
        PopupsImpl.prototype.closeInnerPopup = function () {
            PopupsImpl.internalClosePopup();
        };
        PopupsImpl.internalClosePopup = function (close) {
            var popup = document.YPopupWindowActive;
            if (popup) {
                if (close)
                    popup.close();
                popup.destroy();
            }
            document.YPopupWindowActive = null;
            YVolatile.Basics.IsInPopup = false; // we're no longer in a popup
        };
        /**
         * Opens a dynamic popup, usually a div added to the current document.
         */
        PopupsImpl.prototype.openDynamicPopup = function (result, done) {
            ComponentsHTMLHelper.REQUIRES_KENDOUI(function () {
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
                var popup = null;
                var acts = [];
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
                    close: function (e) {
                        PopupsImpl.closeDynamicPopup();
                    },
                    animation: false,
                    refresh: function (e) {
                        $YetaWF.setLoading(false);
                    },
                    error: function (e) {
                        $YetaWF.setLoading(false);
                        $YetaWF.error("Request failed with status " + e.status);
                    }
                });
                // show and center the window
                popup = $popupwin.data("kendoWindow");
                popup.center().open();
                // mark that a popup is active
                document.expando = true;
                document.YPopupWindowActive = popup;
                $YetaWF.setCondense($popupwin[0], YVolatile.Skin.PopupWidth);
                done($popupwin[0]);
            });
        };
        PopupsImpl.closeDynamicPopup = function () {
            var popupElem = $YetaWF.getElement1BySelectorCond("#ypopup");
            if (popupElem) {
                $YetaWF.processClearDiv(popupElem);
                var popup = $(popupElem).data("kendoWindow");
                // don't call internalClosePopup, otherwise we get close event
                popup.destroy(); // don't close, just destroy
            }
            document.YPopupWindowActive = null;
            YVolatile.Basics.IsInPopup = false; // we're no longer in a popup
        };
        /**
         * Open a static popup, usually a popup based on iframe.
         */
        PopupsImpl.prototype.openStaticPopup = function (url) {
            ComponentsHTMLHelper.REQUIRES_KENDOUI(function () {
                // we're already in a popup
                if ($YetaWF.isInPopup()) {
                    // we handle links within a popup by replacing the current popup page with the new page
                    $YetaWF.setLoading(true);
                    var $popupwin_1 = $("#ypopup", $(window.parent.document));
                    if ($popupwin_1.length === 0)
                        throw "Couldn't find popup window"; /*DEBUG*/
                    var iframeDomElement = $popupwin_1.children("iframe")[0];
                    if (iframeDomElement) {
                        // we aleady have a static popup
                        iframeDomElement.src = url;
                        return;
                    }
                    else {
                        // we had a dynamic popup, close it and build static popup
                        PopupsImpl.closeDynamicPopup();
                    }
                }
                YVolatile.Basics.IsInPopup = true; // we're in a popup
                // insert <div id="ypopup"></div> at top of page for the popup window
                // this is automatically removed when destroy() is called
                $("body").prepend("<div id='ypopup'></div>");
                var $popupwin = $("#ypopup");
                var popup = null;
                var acts = [];
                acts.push("Maximize"); // always show the maximize button - hide it based on popup skin options
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
                    title: " ",
                    visible: false,
                    content: url,
                    close: function (e) {
                        var popup = $popupwin.data("kendoWindow");
                        popup.destroy();
                        popup = null;
                        document.YPopupWindowActive = null;
                        YVolatile.Basics.IsInPopup = false;
                    },
                    animation: false,
                    refresh: function (e) {
                        var iframeDomElement = $popupwin.children("iframe")[0];
                        if (iframeDomElement && iframeDomElement.contentDocument && popup) {
                            var iframeDocumentObject = iframeDomElement.contentDocument;
                            popup.title(iframeDocumentObject.title);
                        }
                        $YetaWF.setLoading(false);
                    },
                    error: function (e) {
                        $YetaWF.setLoading(false);
                        $YetaWF.error("Request failed with status " + e.status);
                    }
                });
                // show and center the window
                popup = $popupwin.data("kendoWindow");
                //do not open the window here - the loaded content opens it because it knows the desired size
                //popup.center().open();
                // mark that a popup is active
                document.expando = true;
                document.YPopupWindowActive = popup;
            });
        };
        return PopupsImpl;
    }());
    YetaWF_ComponentsHTML.PopupsImpl = PopupsImpl;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
// tslint:disable-next-line:variable-name
var YetaWF_PopupsImpl = new YetaWF_ComponentsHTML.PopupsImpl();

//# sourceMappingURL=PopupsImpl.js.map
