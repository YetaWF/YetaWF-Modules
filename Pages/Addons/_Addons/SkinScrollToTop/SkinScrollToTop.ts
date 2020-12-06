/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

namespace YetaWF_Pages {

    export class ScrollUp {

        public static On: boolean = true;

        public static init(): void {
            ($ as any).scrollUp({
                scrollName: "yScrollToTop",
                scrollText: ""
            });
        }
        public static destroy(): void {
            ($ as any).scrollUp.destroy();
        }
    }

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