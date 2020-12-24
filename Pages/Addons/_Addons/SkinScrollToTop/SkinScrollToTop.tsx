/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

namespace YetaWF_Pages {

    export class ScrollUp {

        public static On: boolean = true;
        public static MinScrollDistance: number = 300;  // Distance from top before showing element (in pixels)
        public static SCROLLTIME: number = 300; // time to scroll to top (in ms)
        public static SCROLLSTEPTIME: number = 15; // time between incremental scrolling (in ms)
        public static MININCREMENT: number = 100; // minimum scrolling distance during incremental scrolling

        private static readonly SCROLLTOPID: string = "YetaWF_Pages_ScrollTop";

        public static init(): void {
            let scrollText= ""; //RFFU
            let topDiv = $YetaWF.getElement1BySelectorCond(`#${ScrollUp.SCROLLTOPID}`);
            if (!topDiv) {
                topDiv = <a href="#top" id={ScrollUp.SCROLLTOPID} title={scrollText} /> as HTMLDivElement;
                document.body.appendChild(topDiv);

                $YetaWF.registerEventHandler(topDiv, "click", null, (ev:MouseEvent): boolean => {
                    ScrollUp.scrollToTop();
                    return false;
                });
            }
            ScrollUp.evalScrollTop();
        }
        public static destroy(): void {
            let topDiv = $YetaWF.getElement1BySelectorCond(`#${ScrollUp.SCROLLTOPID}`);
            topDiv?.remove();
        }
        public static evalScrollTop(): void {
            if (!ScrollUp.On) return;
            let topDiv = $YetaWF.getElement1BySelectorCond(`#${ScrollUp.SCROLLTOPID}`);
            if (!topDiv) return;
            let rect = document.body.getBoundingClientRect();
            if (rect.y < - this.MinScrollDistance)
                topDiv.style.display = "";
            else
                topDiv.style.display = "none";
        }
        private static scrollToTop(): void {
            let topDiv = $YetaWF.getElement1BySelectorCond(`#${ScrollUp.SCROLLTOPID}`);
            if (!topDiv) return;
            let rect = document.body.getBoundingClientRect();
            let top = - rect.y;
            let incr = top / (ScrollUp.SCROLLTIME / ScrollUp.SCROLLSTEPTIME);
            if (incr < ScrollUp.MININCREMENT)
                incr = ScrollUp.MININCREMENT;
            let interval = setInterval((): void => {
                top -= incr;
                if (top <= 0) {
                    top = 0;
                    clearInterval(interval);
                }
                window.scroll(window.screenX, top);
            }, ScrollUp.SCROLLSTEPTIME);
        }
    }

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, (ev: CustomEvent<YetaWF.DetailsEventContainerScroll>): boolean => {
        ScrollUp.evalScrollTop();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: Event): boolean => {
        ScrollUp.evalScrollTop();
        return true;
    });

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: CustomEvent<YetaWF.DetailsEventNavPageLoaded>): boolean => {
        if (ScrollUp.On) {
            ScrollUp.init();
        } else {
            ScrollUp.destroy();
        }
        return true;
    });

    // Handles events turning the addon on/off (used for dynamic content)
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, (ev: CustomEvent<YetaWF.DetailsAddonChanged>): boolean => {
        let addonGuid = ev.detail.addonGuid;
        let on = ev.detail.on;
        if (addonGuid === "2a4e6f13-24a0-45c1-8a42-f1072e6ac7de") {
            ScrollUp.On = on;
        }
        return true;
    });
}