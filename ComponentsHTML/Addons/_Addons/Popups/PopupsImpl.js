"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
            if (!popup)
                return;
            var overlay = $YetaWF.getElement1BySelectorCond("#ypopupOverlay", [window.parent.document.body]);
            if (!overlay)
                return;
            popup.style.opacity = "0"; // animation
            overlay.style.opacity = "0"; // animation
            setTimeout(function () {
                win.document.body.style.overflow = "";
                win.YVolatile.Basics.IsInPopup = false; // we're no longer in a popup
                win.document.YPopupDragDropInProgress = false;
                win.document.YPopupXOffset = 0;
                win.document.YPopupYOffset = 0;
                win.document.YPopupWindowActive = null;
                $YetaWF.processClearDiv(popup);
                popup.remove();
                overlay.remove();
            }, 300);
        };
        /**
         * Opens a dynamic popup, usually a div added to the current document.
         */
        PopupsImpl.prototype.openDynamicPopup = function (result, done) {
            // we're already in a popup
            PopupsImpl.internalClosePopup();
            var popup = $YetaWF.createElement("div", { id: PopupsImpl.POPUPID, tabindex: "-1", role: "dialog", "aria-describedby": "ypopupContent", "aria-labelledby": "ypopupTitle", style: "display:none" },
                $YetaWF.createElement("div", { class: "t_titlebar" },
                    $YetaWF.createElement("div", { id: "ypopupTitle", class: "t_title" }, result.PageTitle),
                    $YetaWF.createElement("button", { type: "button", class: "y_buttonlite t_close" })),
                $YetaWF.createElement("div", { id: "ypopupContent", class: "t_content" }));
            $YetaWF.elementAddClass(popup, YVolatile.Skin.PopupCss);
            // mark that a popup is active
            document.expando = true;
            document.YPopupWindowActive = popup;
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
                content.appendChild(pane);
            }
            PopupsImpl.addOverlay();
            document.body.style.overflow = "hidden";
            // Create the window
            document.body.appendChild(popup);
            popup.style.opacity = "0";
            PopupsImpl.reposition();
            popup.style.opacity = "1";
            PopupsImpl.setupDragDrop();
            $YetaWF.setLoading(false);
            done(popup);
            // handle close button
            var closeButton = $YetaWF.getElement1BySelector(".t_titlebar button", [popup]);
            // icon used fas-multiply
            closeButton.innerHTML = YConfigs.YetaWF_ComponentsHTML.SVG_fas_multiply; //close button image
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
            var overlay = $YetaWF.createElement("div", { id: "ypopupOverlay", style: "opacity:0" });
            document.body.appendChild(overlay);
            $YetaWF.forceRedraw(overlay);
            overlay.style.opacity = ""; //animation to css defined value
        };
        PopupsImpl.reposition = function () {
            // with unified page sets there may actually not be a parent, but window.parent returns itself in this case anyway
            var win = window.parent;
            win.document.YPopupDragDropInProgress = false;
            var popup = win.document.YPopupWindowActive;
            if (!popup)
                return;
            var content = $YetaWF.getElement1BySelectorCond("#ypopupContent", [popup]);
            var popupWidth = YVolatile.Skin.PopupWidth;
            var popupHeight = YVolatile.Skin.PopupHeight;
            var popupMaxHeight = YVolatile.Skin.PopupMaxHeight;
            if (popupWidth === undefined || popupHeight === undefined)
                return; // popup dimensions not yet known. We'll get another call later.
            var width;
            if (win.innerWidth <= popupWidth || (popupHeight > 0 && win.innerHeight <= popupHeight) || (popupMaxHeight > 0 && win.innerHeight <= popupMaxHeight)) {
                width = win.innerWidth;
                popup.style.width = "".concat(win.innerWidth, "px");
                popup.style.height = popup.style.minHeight = "".concat(win.innerHeight, "px");
                popup.style.maxHeight = "initial";
                if (content)
                    content.style.height = "initial";
                popup.style.left = "0px";
                popup.style.top = "0px";
                popup.style.display = "";
            }
            else {
                width = popupWidth;
                popup.style.width = "".concat(popupWidth, "px");
                if (popupHeight) {
                    popup.style.height = "auto";
                    if (content)
                        content.style.height = content.style.maxHeight = "".concat(popupHeight, "px");
                }
                else {
                    if (content) {
                        content.style.height = "auto";
                        content.style.maxHeight = popupMaxHeight ? "".concat(popupMaxHeight, "px") : "".concat(win.innerHeight * 3 / 4, "px");
                    }
                }
                // center
                popup.style.display = "";
                var drect = popup.getBoundingClientRect();
                var left = (win.innerWidth - drect.width) / 2;
                var top_1 = (win.innerHeight - drect.height) / 3;
                popup.style.left = "".concat(left, "px"); // or + win.pageXOffset if position:absolute
                popup.style.top = "".concat(top_1, "px"); //  + win.pageYOffset
            }
            $YetaWF.setCondense(popup, width);
        };
        PopupsImpl.setupDragDrop = function () {
            var win = window.parent;
            var popup = win.document.YPopupWindowActive;
            if (!popup)
                return;
            $YetaWF.registerEventHandler(popup, "mousedown", ".t_titlebar", function (ev) {
                var drect = popup.getBoundingClientRect();
                win.document.YPopupXOffset = ev.clientX - drect.left;
                win.document.YPopupYOffset = ev.clientY - drect.top;
                win.document.YPopupDragDropInProgress = true;
                return false;
            });
        };
        PopupsImpl.pageLoad = function () {
            if (YVolatile.Basics.IsInPopup) {
                PopupsImpl.reposition();
                // Handle Escape key
                document.body.addEventListener("keydown", function (ev) {
                    if (ev.key === "Escape") {
                        PopupsImpl.internalClosePopup();
                        return false;
                    }
                    return true;
                });
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
                // outer window
                // console.debug(`handleMouseMove x ${clientX} y ${clientY} ${drect.left},${drect.top}${drect.width},${drect.height}`);
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
                popup.style.left = "".concat(left, "px");
                popup.style.top = "".concat(top_2, "px");
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
        if (ev.detail.container === document.body)
            PopupsImpl.reposition();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        if (ev.detail.container === document.body)
            PopupsImpl.reposition();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        PopupsImpl.reposition();
        return true;
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
var YetaWF_PopupsImpl = new YetaWF_ComponentsHTML.PopupsImpl();
// use to resize static popup after full page load, once popup skin is known
YetaWF_ComponentsHTML.PopupsImpl.pageLoad();

//# sourceMappingURL=PopupsImpl.js.map
