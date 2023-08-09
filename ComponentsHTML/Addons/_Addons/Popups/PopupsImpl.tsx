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
            const win = window.parent;
            const popup = win.document.YPopupWindowActive;
            if (!popup) return;

            const overlay = $YetaWF.getElement1BySelectorCond("#ypopupOverlay", [window.parent.document.body]);
            if (!overlay) return;

            popup.style.opacity = "0";// animation
            overlay.style.opacity = "0";// animation
            setTimeout((): void => {
                win.document.body.style.overflow = "";
                (win as any).YVolatile.Basics.IsInPopup = false; // we're no longer in a popup
                win.document.YPopupDragDropInProgress = false;
                win.document.YPopupXOffset = 0;
                win.document.YPopupYOffset = 0;
                win.document.YPopupWindowActive = null;

                $YetaWF.processClearDiv(popup);
                popup.remove();

                overlay.remove();
            }, 300);
        }

        /**
         * Opens a dynamic popup, usually a div added to the current document.
         */
        public openDynamicPopup(result: YetaWF.ContentResult, done: (dialog: HTMLElement) => void): void {

            // we're already in a popup
            PopupsImpl.internalClosePopup();

            const popup = <div id={PopupsImpl.POPUPID} tabindex="-1" role="dialog" aria-describedby="ypopupContent" aria-labelledby="ypopupTitle" style="display:none">
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
            popup.style.opacity = "0";
            PopupsImpl.reposition();
            popup.style.opacity = "1";

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
            let overlay = <div id="ypopupOverlay" style="opacity:0"></div> as HTMLDivElement;
            document.body.appendChild(overlay);
            $YetaWF.forceRedraw(overlay);
            overlay.style.opacity = "";//animation to css defined value
        }

        public static reposition(): void {

            // with unified page sets there may actually not be a parent, but window.parent returns itself in this case anyway
            let win = window.parent;
            win.document.YPopupDragDropInProgress = false;

            let popup = win.document.YPopupWindowActive;
            if (!popup) return;

            let content = $YetaWF.getElement1BySelectorCond("#ypopupContent", [popup]);

            let popupWidth = YVolatile.Skin.PopupWidth;
            let popupHeight = YVolatile.Skin.PopupHeight;
            let popupMaxHeight = YVolatile.Skin.PopupMaxHeight;
            if (popupWidth === undefined || popupHeight === undefined)
                return; // popup dimensions not yet known. We'll get another call later.

            let width: number;
            if (win.innerWidth <= popupWidth || (popupHeight > 0 && win.innerHeight <= popupHeight) || (popupMaxHeight > 0 && win.innerHeight <= popupMaxHeight)) {
                width = win.innerWidth;
                popup.style.width = `${win.innerWidth}px`;
                popup.style.height = popup.style.minHeight = `${win.innerHeight}px`;
                popup.style.maxHeight = "initial";
                if (content)
                    content.style.height = "initial";
                popup.style.left = "0px";
                popup.style.top = "0px";
                popup.style.display = "";
            } else {
                width = popupWidth;
                popup.style.width = `${popupWidth}px`;
                if (popupHeight) {
                    popup.style.height = "auto";
                    if (content) {
                        content.style.height = `${popupHeight}px`;
                        content.style.maxHeight = "initial";
                    }
                } else {
                    if (content) {
                        content.style.height = "auto";
                        content.style.maxHeight = popupMaxHeight ? `${popupMaxHeight}px` : `${win.innerHeight * 3 / 4}px`;
                    }
                }
                // center
                popup.style.display = "";
                let drect = popup.getBoundingClientRect();
                let left = (win.innerWidth - drect.width) / 2;
                let top = (win.innerHeight - drect.height) / 2;
                popup.style.left = `${left}px`;  // or + win.pageXOffset if position:absolute
                popup.style.top = `${top}px`; //  + win.pageYOffset
            }
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

        public static pageLoad(): void {

            if (YVolatile.Basics.IsInPopup) {
                PopupsImpl.reposition();

                // Handle Escape key
                document.body.addEventListener("keydown", (ev: KeyboardEvent): boolean => {
                    if (ev.key === "Escape") {
                        PopupsImpl.internalClosePopup();
                        return false;
                    }
                    return true;
                });
            }
        }

        public static handleMouseMove(clientX: number, clientY: number): boolean {
            // with unified page sets there may actually not be a parent, but window.parent returns itself in this case anyway
            let win = window.parent;
            if (win.document.YPopupDragDropInProgress) {

                let popup = win.document.YPopupWindowActive;
                if (!popup) return true;

                let drect = popup.getBoundingClientRect();
                // outer window
                // console.debug(`handleMouseMove x ${clientX} y ${clientY} ${drect.left},${drect.top}${drect.width},${drect.height}`);
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
