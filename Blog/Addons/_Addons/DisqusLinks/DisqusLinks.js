"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */
// If this javascript snippet is included, that means we're displaying disquslinks.
var DISQUSWIDGETS;
var YetaWF_Blog;
(function (YetaWF_Blog) {
    var SkinDisqusLinksModule = /** @class */ (function () {
        function SkinDisqusLinksModule() {
        }
        SkinDisqusLinksModule.MODULEGUID = "776adfcd-da5f-4926-b29d-4c06353266c0";
        SkinDisqusLinksModule.on = true;
        return SkinDisqusLinksModule;
    }());
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, function (ev) {
        var addonGuid = ev.detail.addonGuid;
        var on = ev.detail.on;
        if (addonGuid === SkinDisqusLinksModule.MODULEGUID) {
            SkinDisqusLinksModule.on = on;
        }
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        if (SkinDisqusLinksModule.on && DISQUSWIDGETS)
            DISQUSWIDGETS.getCount({ reset: true });
        return true;
    });
})(YetaWF_Blog || (YetaWF_Blog = {}));

//# sourceMappingURL=DisqusLinks.js.map
