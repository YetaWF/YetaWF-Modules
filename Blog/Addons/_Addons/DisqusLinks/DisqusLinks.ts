/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

// If this javascript snippet is included, that means we're displaying disquslinks.

var DISQUSWIDGETS: any;

namespace YetaWF_Blog {

    class SkinDisqusLinksModule {

        static readonly MODULEGUID: string = "776adfcd-da5f-4926-b29d-4c06353266c0";

        static on: boolean = true;

    }

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, (ev: CustomEvent<YetaWF.DetailsAddonChanged>): boolean => {
        let addonGuid = ev.detail.addonGuid;
        let on = ev.detail.on;
        if (addonGuid === SkinDisqusLinksModule.MODULEGUID) {
            SkinDisqusLinksModule.on = on;
        }
        return true;
    });

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: CustomEvent<YetaWF.DetailsEventNavPageLoaded>): boolean => {
        if (SkinDisqusLinksModule.on && DISQUSWIDGETS)
            DISQUSWIDGETS.getCount({ reset: true });
        return true;
    });
}
