/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TinyBar#License */

namespace YetaWF_TinyBar {

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: CustomEvent<YetaWF.DetailsEventNavPageLoaded>): boolean => {

        let entries = $YetaWF.getElementsBySelector(".YetaWF_TinyBar_TinyBar ul.t_lang > li > a") as HTMLAnchorElement[];
        if (entries.length > 0) {
            const uri = $YetaWF.parseUrl(window.location.toString());
            for (const entry of entries) {
                const uriEntry = $YetaWF.parseUrl(entry.href);
                const lang = uriEntry.getSearch(YConfigs.Basics.Link_Language);
                uri.replaceSearch(YConfigs.Basics.Link_Language, lang);
                entry.href = uri.toUrl();
            }
        }
        return true;
    });

}
