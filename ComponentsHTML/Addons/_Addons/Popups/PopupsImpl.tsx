/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* Popups implementation required by YetaWF */

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

interface Document {
    YPopupWindowActive: HTMLDivElement | null;
    YPopupWindowStatic: boolean;
    YPopupDragDropInProgress: boolean;
    YPopupXOffset: number;
    YPopupYOffset: number;
}

namespace YetaWF_ComponentsHTML {

    export class PopupsImpl implements YetaWF.IPopupsImpl {

        public static POPUPID: string = "ypopup";

        /**
         * Close the popup - this can only be used by code that is running within the popup (not the parent document/page)
         */
        public closePopup(forceReload?: boolean): void {
            if (YVolatile.Basics.IsInPopup) {
                let win = window.parent;
                var forced = (forceReload === true);
                if (forced)
                    $YetaWF.reloadPage(true, win);
                PopupsImpl.internalClosePopup();
            }
        }

        /**
         * Close the popup - this can only be used by code that is running on the main page (not within the popup)
         */
        public closeInnerPopup(): void {
            PopupsImpl.internalClosePopup();
        }

        private static internalClosePopup(): void {
            // with unified page sets there may actually not be a parent, but window.parent returns itself in this case anyway
            let win = window.parent;
            let popup = win.document.YPopupWindowActive;

            let overlay = $YetaWF.getElement1BySelectorCond("#ypopupOverlay", [window.parent.document.body]);
            if (overlay)
                overlay.remove();

            win.document.body.style.overflow = "";
            (win as any).YVolatile.Basics.IsInPopup = false; // we're no longer in a popup
            win.document.YPopupDragDropInProgress = false;
            win.document.YPopupXOffset = 0;
            win.document.YPopupYOffset = 0;
            win.document.YPopupWindowActive = null;

            if (popup) {
                $YetaWF.processClearDiv(popup);
                popup.remove();
            }
        }

        /**
         * Opens a dynamic popup, usually a div added to the current document.
         */
        public openDynamicPopup(result: YetaWF.ContentResult, done: (dialog: HTMLElement) => void): void {

            // we're already in a popup
            PopupsImpl.internalClosePopup();

            let popup = <div id={PopupsImpl.POPUPID} tabindex="-1" role="dialog" class="yPopupDyn" aria-describedby="ypopupContent" aria-labelledby="ypopupTitle" style="display:none">
                <div class="t_titlebar">
                    <div id="ypopupTitle" class="t_title">{result.PageTitle}</div>
                    <button type="button" class="y_buttonlite t_close"></button>
                </div>
                <div id="ypopupContent" class="t_content"></div>
            </div> as HTMLDivElement;

            $YetaWF.elementAddClass(popup, YVolatile.Skin.PopupCss);

            // mark that a popup is active
            (document as any).expando = true;
            document.YPopupWindowActive = popup;
            document.YPopupWindowStatic = false;
            document.YPopupDragDropInProgress = false;
            document.YPopupXOffset = 0;
            document.YPopupYOffset = 0;
            YVolatile.Basics.IsInPopup = true; // we're now in a popup

            let content = $YetaWF.getElement1BySelector("#ypopupContent", [popup]);

            // add pane content
            const contentLength = result.Content.length;
            for (let i = 0; i < contentLength; i++) {
                // add the pane
                let pane = <div class="yPane"></div> as HTMLDivElement;
                $YetaWF.elementAddClass(pane, result.Content[i].Pane);
                pane.innerHTML = result.Content[i].HTML;
                content.appendChild(pane);
            }

            PopupsImpl.addOverlay();
            document.body.style.overflow = "hidden";

            // Create the window
            document.body.appendChild(popup);
            PopupsImpl.reposition();

            PopupsImpl.setupDragDrop();

            $YetaWF.setLoading(false);

            done(popup);

            // handle close button
            let closeButton = $YetaWF.getElement1BySelector(".t_titlebar button", [popup]);
            // icon used fas-multiply
            closeButton.innerHTML = YConfigs.YetaWF_ComponentsHTML.SVG_fas_multiply;//close button image
            $YetaWF.registerEventHandler(closeButton, "click", null, (ev: MouseEvent): boolean => {
                PopupsImpl.internalClosePopup();
                return false;
            });

            $YetaWF.registerEventHandler(popup, "keydown", null, (ev: KeyboardEvent): boolean => {
                if (ev.key === "Escape") {
                    PopupsImpl.internalClosePopup();
                    return false;
                }
                return true;
            });
        }

        private static addOverlay(): void {
            let overlay = <div id="ypopupOverlay"></div> as HTMLDivElement;
            document.body.appendChild(overlay);
        }

        public static reposition(): void {

            // with unified page sets there may actually not be a parent, but window.parent returns itself in this case anyway
            let win = window.parent;
            win.document.YPopupDragDropInProgress = false;

            let popup = win.document.YPopupWindowActive;
            if (!popup) return;

            let content = $YetaWF.getElement1BySelectorCond("#ypopupContent", [popup]);

            let popupWidth: number | undefined;
            let popupHeight: number | undefined;
            if (win.document.YPopupWindowStatic) {
                // only inner window knows popup width/height for a static popup
                let iframe = $YetaWF.getElement1BySelector("iframe", [popup]) as HTMLIFrameElement;
                let yVolatile: YetaWF.IVolatile = (iframe.contentWindow as any).YVolatile;
                if (yVolatile) {
                    let skin = (iframe.contentWindow as any).YVolatile.Skin;
                    if (skin) {
                        popupWidth = skin.PopupWidth;
                        popupHeight = skin.PopupHeight;
                    }
                }
            } else {
                popupWidth = YVolatile.Skin.PopupWidth;
                popupHeight = YVolatile.Skin.PopupHeight;
            }
            if (popupWidth === undefined || popupHeight === undefined)
                return; // popup dimensions not yet known. We'll get another call later.

            let width: number;
            if (win.innerWidth <= popupWidth || win.innerHeight <= popupHeight) {
                width = win.innerWidth;
                popup.style.width = `${win.innerWidth}px`;
                popup.style.height = `${win.innerHeight}px`;
                if (content)
                    content.style.maxHeight = "none";
                popup.style.left = "0px";
                popup.style.top = "0px";
                popup.style.display = "";
            } else {
                width = popupWidth;
                popup.style.width = `${popupWidth}px`;
                if (!win.document.YPopupWindowStatic) {
                    popup.style.height = "auto";
                    if (content)
                        content.style.maxHeight = `${win.innerHeight * 3 / 4}px`;
                } else {
                    popup.style.height = `${popupHeight}px`;
                    if (content)
                        content.style.maxHeight = "none";
                }
                // center
                popup.style.display = "";
                let drect = popup.getBoundingClientRect();
                let left = (win.innerWidth - drect.width) / 2;
                let top = (win.innerHeight - drect.height) / 2;
                popup.style.left = `${left}px`;  // or + win.pageXOffset if position:absolute
                popup.style.top = `${top}px`; //  + win.pageYOffset
            }
            if (!win.document.YPopupWindowStatic)
                $YetaWF.setCondense(popup, width);
        }

        private static setupDragDrop(): void {
            let win = window.parent;
            let popup = win.document.YPopupWindowActive;
            if (!popup) return;
            $YetaWF.registerEventHandler(popup, "mousedown", ".t_titlebar", (ev: MouseEvent): boolean => {
                let drect = popup!.getBoundingClientRect();
                win.document.YPopupXOffset  = ev.clientX - drect.left;
                win.document.YPopupYOffset = ev.clientY - drect.top;
                win.document.YPopupDragDropInProgress = true;
                return false;
            });
        }

        /**
         * Open a static popup, usually a popup based on iframe.
         */
        public openStaticPopup(url: string): void {

            let win = window.parent;

            // we're already in a popup
            if (win.document.YPopupWindowActive != null) {
                // we handle links within a popup by replacing the current popup page with the new page
                if (win.document.YPopupWindowStatic) {
                    let popup = win.document.YPopupWindowActive;
                    let iframe = $YetaWF.getElement1BySelector("iframe", [popup]) as HTMLIFrameElement;
                    iframe.src = url;
                    return;
                } else {
                    // we had a dynamic popup, close it and build static popup
                    PopupsImpl.internalClosePopup();
                }
            }

            let popup = <div id={PopupsImpl.POPUPID} tabindex="-1" role="dialog" class="yPopup" aria-labelledby="ypopupTitle" style="display:none">
                <div class="t_titlebar">
                    <span id="ypopupTitle" class="t_title">...</span>
                    <button type="button" class="y_buttonlite t_close"></button>
                </div>
                <iframe title="(???)" frameborder="0"></iframe>
            </div> as HTMLDivElement;

            // mark that a popup is active
            (document as any).expando = true;
            document.YPopupWindowActive = popup;
            document.YPopupWindowStatic = true;
            document.YPopupDragDropInProgress = false;
            YVolatile.Basics.IsInPopup = true; // we're now in a popup
            win.document.YPopupDragDropInProgress = false;

            let iframe = $YetaWF.getElement1BySelector("iframe", [popup]) as HTMLIFrameElement;
            iframe.onload = (ev: Event): boolean => {
                let title = $YetaWF.getElementById("ypopupTitle");
                if (iframe.contentDocument)
                    title.innerText = iframe.contentDocument.title;
                $YetaWF.setLoading(false);
                return true;
            };
            iframe.src = url;

            PopupsImpl.addOverlay();
            document.body.style.overflow = "hidden";

            // Create the window
            document.body.appendChild(popup);
            PopupsImpl.reposition();

            PopupsImpl.setupDragDrop();

            // handle close button
            let closeButton = $YetaWF.getElement1BySelector(".t_titlebar button", [popup]);
            closeButton.innerHTML = YConfigs.YetaWF_ComponentsHTML.SVG_fas_multiply;//close button image
            $YetaWF.registerEventHandler(closeButton, "click", null, (ev: MouseEvent): boolean => {
                PopupsImpl.internalClosePopup();
                return false;
            });
        }

        public static pageLoad(): void {

            if (YVolatile.Basics.IsInPopup) {
                PopupsImpl.reposition();

                let win = window.parent;

                /**
                 * Handle Escape key in iframe for static popups
                 */
                if (win.document.YPopupWindowStatic) {
                    document.body.addEventListener("keydown", (ev: KeyboardEvent): boolean => {
                        if (ev.key === "Escape") {
                            PopupsImpl.internalClosePopup();
                            return false;
                        }
                        return true;
                    });
                }
            }
        }

        public static handleMouseMove(clientX: number, clientY: number): boolean {
            // with unified page sets there may actually not be a parent, but window.parent returns itself in this case anyway
            let win = window.parent;
            if (win.document.YPopupDragDropInProgress) {

                let popup = win.document.YPopupWindowActive;
                if (!popup) return true;

                let drect = popup.getBoundingClientRect();

                if ($YetaWF.elementHas(document.body, popup)) {
                    // outer window
                    // console.debug(`handleMouseMove x ${clientX} y ${clientY} ${drect.left},${drect.top}${drect.width},${drect.height}`);
                } else {
                    // inner iframe window
                    // console.debug(`adjust handleMouseMove x ${clientX} y ${clientY} ${drect.left},${drect.top}${drect.width},${drect.height}`);
                    // we're handling a mousemove for a static popup
                    // adjust the mouse coordinates
                    clientX += drect.left;
                    clientY += drect.top;

                    // adjust clientY for title
                    let title = $YetaWF.getElement1BySelector(".t_titlebar", [popup]);
                    clientY += title.clientHeight;

                }
                let left = clientX - win.document.YPopupXOffset;
                if (left + drect.width > win.innerWidth) left = win.innerWidth - drect.width;
                if (left < 0) left = 0;
                let top = clientY - win.document.YPopupYOffset;
                if (top + drect.height > win.innerHeight) top = win.innerHeight - drect.height;
                if (top < 0) top = 0;
                popup.style.left = `${left}px`;
                popup.style.top = `${top}px`;
                return false;
            }
            return true;
        }
    }

    $YetaWF.registerEventHandlerBody("mousemove", null, (ev: MouseEvent): boolean => {
        return PopupsImpl.handleMouseMove(ev.clientX, ev.clientY);
    });
    $YetaWF.registerEventHandlerBody("mouseup", null, (ev: MouseEvent): boolean => {
        let win = window.parent;
        if (win.document.YPopupDragDropInProgress) {
            win.document.YPopupDragDropInProgress = false;
            return false;
        }
        return true;
    });

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, (ev: CustomEvent<YetaWF.DetailsEventContainerScroll>): boolean => {
        if (ev.detail.container === document.body)
            PopupsImpl.reposition();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: CustomEvent<YetaWF.DetailsEventContainerResize>): boolean => {
        if (ev.detail.container === document.body)
            PopupsImpl.reposition();
        return true;
    });

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: CustomEvent<YetaWF.DetailsEventNavPageLoaded>): boolean => {
        PopupsImpl.reposition();
        return true;
    });
}

var YetaWF_PopupsImpl: YetaWF.IPopupsImpl = new YetaWF_ComponentsHTML.PopupsImpl();

// use to resize static popup after full page load, once popup skin is known
YetaWF_ComponentsHTML.PopupsImpl.pageLoad();
