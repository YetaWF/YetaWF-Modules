"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TinyBar#License */
var YetaWF_TinyBar;
(function (YetaWF_TinyBar) {
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        var entries = $YetaWF.getElementsBySelector(".YetaWF_TinyBar_TinyBar ul.t_lang > li > a");
        if (entries.length > 0) {
            var uri = $YetaWF.parseUrl(window.location.toString());
            for (var _i = 0, entries_1 = entries; _i < entries_1.length; _i++) {
                var entry = entries_1[_i];
                var uriEntry = $YetaWF.parseUrl(entry.href);
                var lang = uriEntry.getSearch(YConfigs.Basics.Link_Language);
                uri.replaceSearch(YConfigs.Basics.Link_Language, lang);
                entry.href = uri.toUrl();
            }
        }
        return true;
    });
})(YetaWF_TinyBar || (YetaWF_TinyBar = {}));

//# sourceMappingURL=TinyBar.js.map
