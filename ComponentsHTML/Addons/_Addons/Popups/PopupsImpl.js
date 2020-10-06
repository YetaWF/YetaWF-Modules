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
                var win = window.parent;
                var forced = (forceReload === true);
                if (forced)
                    $YetaWF.reloadPage(true, win);
                PopupsImpl.internalClosePopup();
            }
        };
        /**
         * Close the popup - this can only be used by code that is running on the main page (not within the popup)
         */
        PopupsImpl.prototype.closeInnerPopup = function () {
            PopupsImpl.internalClosePopup();
        };
        PopupsImpl.internalClosePopup = function () {
            // with unified page sets there may actually not be a parent, but window.parent returns itself in this case anyway
            var win = window.parent;
            var popup = win.document.YPopupWindowActive;
            var overlay = $YetaWF.getElement1BySelectorCond(".ui-widget-overlay.ui-front", [win.document.body]);
            if (overlay)
                overlay.remove();
            win.document.body.style.overflow = "";
            win.YVolatile.Basics.IsInPopup = false; // we're no longer in a popup
            win.document.YPopupDragDropInProgress = false;
            win.document.YPopupXOffset = 0;
            win.document.YPopupYOffset = 0;
            win.document.YPopupWindowActive = null;
            if (popup) {
                $YetaWF.processClearDiv(popup);
                popup.remove();
            }
        };
        /**
         * Opens a dynamic popup, usually a div added to the current document.
         */
        PopupsImpl.prototype.openDynamicPopup = function (result, done) {
            // we're already in a popup
            PopupsImpl.internalClosePopup();
            var popup = $YetaWF.createElement("div", { id: PopupsImpl.POPUPID, tabindex: "-1", role: "dialog", class: "yPopupDyn ui-dialog ui-corner-all ui-widget ui-widget-content ui-front ui-dialog-buttons ui-draggable", "aria-describedby": "ypopupContent", "aria-labelledby": "ypopupTitle", style: "display:none" },
                $YetaWF.createElement("div", { class: "ui-dialog-titlebar ui-corner-all ui-widget-header ui-helper-clearfix ui-draggable-handle" },
                    $YetaWF.createElement("span", { id: "ypopupTitle", class: "ui-dialog-title" }, result.PageTitle),
                    $YetaWF.createElement("button", { type: "button", class: "ui-button ui-corner-all ui-widget ui-button-icon-only ui-dialog-titlebar-close" },
                        $YetaWF.createElement("span", { class: "ui-button-icon ui-icon ui-icon-closethick" }),
                        $YetaWF.createElement("span", { class: "ui-button-icon-space" }, " "),
                        "Close")),
                $YetaWF.createElement("div", { id: "ypopupContent", class: "ui-dialog-content ui-widget-content" }));
            $YetaWF.elementAddClass(popup, YVolatile.Skin.PopupCss);
            // mark that a popup is active
            document.expando = true;
            document.YPopupWindowActive = popup;
            document.YPopupWindowStatic = false;
            document.YPopupDragDropInProgress = false;
            document.YPopupXOffset = 0;
            document.YPopupYOffset = 0;
            YVolatile.Basics.IsInPopup = true; // we're now in a popup
            var content = $YetaWF.getElement1BySelector("#ypopupContent", [popup]);
            // add pane content
            var contentLength = result.Content.length;
            for (var i = 0; i < contentLength; i++) {
                // add the pane
                var pane = $YetaWF.createElement("div", { class: "yPane" });
                $YetaWF.elementAddClass(pane, result.Content[i].Pane);
                pane.innerHTML = result.Content[i].HTML;
                content.append(pane);
            }
            PopupsImpl.addOverlay();
            document.body.style.overflow = "hidden";
            // Create the window
            document.body.append(popup);
            PopupsImpl.reposition();
            PopupsImpl.setupDragDrop();
            $YetaWF.setLoading(false);
            done(popup);
            // handle close button
            var closeButton = $YetaWF.getElement1BySelector(".ui-dialog-titlebar button", [popup]);
            $YetaWF.registerEventHandler(closeButton, "click", null, function (ev) {
                PopupsImpl.internalClosePopup();
                return false;
            });
            $YetaWF.registerEventHandler(popup, "keydown", null, function (ev) {
                if (ev.key === "Escape") {
                    PopupsImpl.internalClosePopup();
                    return false;
                }
                return true;
            });
        };
        PopupsImpl.addOverlay = function () {
            var overlay = $YetaWF.createElement("div", { class: "ui-widget-overlay ui-front" });
            document.body.append(overlay);
        };
        PopupsImpl.reposition = function () {
            // with unified page sets there may actually not be a parent, but window.parent returns itself in this case anyway
            var win = window.parent;
            win.document.YPopupDragDropInProgress = false;
            var popup = win.document.YPopupWindowActive;
            if (!popup)
                return;
            var content = $YetaWF.getElement1BySelectorCond("#ypopupContent", [popup]);
            var popupWidth;
            var popupHeight;
            if (win.document.YPopupWindowStatic) {
                // only inner window knows popup width/height for a static popup
                var iframe = $YetaWF.getElement1BySelector("iframe", [popup]);
                var yVolatile = iframe.contentWindow.YVolatile;
                if (yVolatile) {
                    var skin = iframe.contentWindow.YVolatile.Skin;
                    if (skin) {
                        popupWidth = skin.PopupWidth;
                        popupHeight = skin.PopupHeight;
                    }
                }
            }
            else {
                popupWidth = YVolatile.Skin.PopupWidth;
                popupHeight = YVolatile.Skin.PopupHeight;
            }
            if (popupWidth === undefined || popupHeight === undefined)
                return; // popup dimensions not yet known. We'll get another call later.
            var width;
            if (win.innerWidth <= popupWidth || win.innerHeight <= popupHeight) {
                width = win.innerWidth;
                popup.style.width = win.innerWidth + "px";
                popup.style.height = win.innerHeight + "px";
                if (content)
                    content.style.maxHeight = "none";
                popup.style.left = "0px";
                popup.style.top = "0px";
                popup.style.display = "";
            }
            else {
                width = popupWidth;
                popup.style.width = popupWidth + "px";
                if (!win.document.YPopupWindowStatic) {
                    popup.style.height = "auto";
                    if (content)
                        content.style.maxHeight = win.innerHeight * 3 / 4 + "px";
                }
                else {
                    popup.style.height = popupHeight + "px";
                    if (content)
                        content.style.maxHeight = "none";
                }
                // center
                popup.style.display = "";
                var drect = popup.getBoundingClientRect();
                var left = (win.innerWidth - drect.width) / 2;
                var top_1 = (win.innerHeight - drect.height) / 2;
                popup.style.left = left + "px"; // or + win.pageXOffset if position:absolute
                popup.style.top = top_1 + "px"; //  + win.pageYOffset
            }
            if (!win.document.YPopupWindowStatic)
                $YetaWF.setCondense(popup, width);
        };
        PopupsImpl.setupDragDrop = function () {
            var win = window.parent;
            var popup = win.document.YPopupWindowActive;
            if (!popup)
                return;
            $YetaWF.registerEventHandler(popup, "mousedown", ".ui-dialog-titlebar", function (ev) {
                var drect = popup.getBoundingClientRect();
                win.document.YPopupXOffset = ev.clientX - drect.left;
                win.document.YPopupYOffset = ev.clientY - drect.top;
                win.document.YPopupDragDropInProgress = true;
                return false;
            });
        };
        /**
         * Open a static popup, usually a popup based on iframe.
         */
        PopupsImpl.prototype.openStaticPopup = function (url) {
            var win = window.parent;
            // we're already in a popup
            if (win.document.YPopupWindowActive != null) {
                // we handle links within a popup by replacing the current popup page with the new page
                if (win.document.YPopupWindowStatic) {
                    var popup_1 = win.document.YPopupWindowActive;
                    var iframe_1 = $YetaWF.getElement1BySelector("iframe", [popup_1]);
                    iframe_1.src = url;
                    return;
                }
                else {
                    // we had a dynamic popup, close it and build static popup
                    PopupsImpl.internalClosePopup();
                }
            }
            var popup = $YetaWF.createElement("div", { id: PopupsImpl.POPUPID, tabindex: "-1", role: "dialog", class: "yPopup ui-dialog ui-corner-all ui-widget ui-widget-content ui-front ui-dialog-buttons ui-draggable", "aria-labelledby": "ypopupTitle", style: "display:none" },
                $YetaWF.createElement("div", { class: "ui-dialog-titlebar ui-corner-all ui-widget-header ui-helper-clearfix ui-draggable-handle" },
                    $YetaWF.createElement("span", { id: "ypopupTitle", class: "ui-dialog-title" }, "..."),
                    $YetaWF.createElement("button", { type: "button", class: "ui-button ui-corner-all ui-widget ui-button-icon-only ui-dialog-titlebar-close" },
                        $YetaWF.createElement("span", { class: "ui-button-icon ui-icon ui-icon-closethick" }),
                        $YetaWF.createElement("span", { class: "ui-button-icon-space" }, " "),
                        "Close")),
                $YetaWF.createElement("iframe", { title: "(???)", frameborder: "0", class: "ui-dialog-content ui-widget-content" }));
            // mark that a popup is active
            document.expando = true;
            document.YPopupWindowActive = popup;
            document.YPopupWindowStatic = true;
            document.YPopupDragDropInProgress = false;
            YVolatile.Basics.IsInPopup = true; // we're now in a popup
            win.document.YPopupDragDropInProgress = false;
            var iframe = $YetaWF.getElement1BySelector("iframe", [popup]);
            iframe.onload = function (ev) {
                var title = $YetaWF.getElementById("ypopupTitle");
                if (iframe.contentDocument)
                    title.innerText = iframe.contentDocument.title;
                $YetaWF.setLoading(false);
                return true;
            };
            iframe.src = url;
            PopupsImpl.addOverlay();
            document.body.style.overflow = "hidden";
            // Create the window
            document.body.append(popup);
            PopupsImpl.reposition();
            PopupsImpl.setupDragDrop();
            // handle close button
            var closeButton = $YetaWF.getElement1BySelector(".ui-dialog-titlebar button", [popup]);
            $YetaWF.registerEventHandler(closeButton, "click", null, function (ev) {
                PopupsImpl.internalClosePopup();
                return false;
            });
        };
        PopupsImpl.pageLoad = function () {
            if (YVolatile.Basics.IsInPopup) {
                PopupsImpl.reposition();
                var win = window.parent;
                /**
                 * Handle Escape key in iframe for static popups
                 */
                if (win.document.YPopupWindowStatic) {
                    document.body.addEventListener("keydown", function (ev) {
                        if (ev.key === "Escape") {
                            PopupsImpl.internalClosePopup();
                            return false;
                        }
                        return true;
                    });
                }
            }
        };
        PopupsImpl.handleMouseMove = function (clientX, clientY) {
            // with unified page sets there may actually not be a parent, but window.parent returns itself in this case anyway
            var win = window.parent;
            if (win.document.YPopupDragDropInProgress) {
                var popup = win.document.YPopupWindowActive;
                if (!popup)
                    return true;
                var drect = popup.getBoundingClientRect();
                if ($YetaWF.elementHas(document.body, popup)) {
                    // outer window
                    console.log("handleMouseMove x " + clientX + " y " + clientY + " " + drect.left + "," + drect.top + drect.width + "," + drect.height);
                }
                else {
                    // inner iframe window
                    console.log("adjust handleMouseMove x " + clientX + " y " + clientY + " " + drect.left + "," + drect.top + drect.width + "," + drect.height);
                    // we're handling a mousemove for a static popup
                    // adjust the mouse coordinates
                    clientX += drect.left;
                    clientY += drect.top;
                    // adjust clientY for title
                    var title = $YetaWF.getElement1BySelector(".ui-dialog-titlebar", [popup]);
                    clientY += title.clientHeight;
                }
                var left = clientX - win.document.YPopupXOffset;
                if (left + drect.width > win.innerWidth)
                    left = win.innerWidth - drect.width;
                if (left < 0)
                    left = 0;
                var top_2 = clientY - win.document.YPopupYOffset;
                if (top_2 + drect.height > win.innerHeight)
                    top_2 = win.innerHeight - drect.height;
                if (top_2 < 0)
                    top_2 = 0;
                popup.style.left = left + "px";
                popup.style.top = top_2 + "px";
                return false;
            }
            return true;
        };
        PopupsImpl.POPUPID = "ypopup";
        return PopupsImpl;
    }());
    YetaWF_ComponentsHTML.PopupsImpl = PopupsImpl;
    $YetaWF.registerEventHandlerBody("mousemove", null, function (ev) {
        return PopupsImpl.handleMouseMove(ev.clientX, ev.clientY);
    });
    $YetaWF.registerEventHandlerBody("mouseup", null, function (ev) {
        var win = window.parent;
        if (win.document.YPopupDragDropInProgress) {
            win.document.YPopupDragDropInProgress = false;
            return false;
        }
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, function (ev) {
        PopupsImpl.reposition();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        PopupsImpl.reposition();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        PopupsImpl.reposition();
        return true;
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
// tslint:disable-next-line:variable-name
var YetaWF_PopupsImpl = new YetaWF_ComponentsHTML.PopupsImpl();
// use to resize static popup after full page load, once popup skin is known
YetaWF_ComponentsHTML.PopupsImpl.pageLoad();

//# sourceMappingURL=PopupsImpl.js.map
