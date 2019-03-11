"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */
// If this javascript snippet is included, that means we're displaying disquslinks.
var DISQUSWIDGETS;
var YetaWF_Blog;
(function (YetaWF_Blog) {
    var SkinDisqusLinksModule = /** @class */ (function () {
        function SkinDisqusLinksModule() {
        }
        /**
         * Initializes the module instance.
         */
        SkinDisqusLinksModule.prototype.init = function () {
            $YetaWF.registerContentChange(function (addonGuid, on) {
                if (addonGuid === SkinDisqusLinksModule.MODULEGUID) {
                    SkinDisqusLinksModule.on = on;
                }
            });
            $YetaWF.addWhenReady(function (tag) {
                if (SkinDisqusLinksModule.on && DISQUSWIDGETS)
                    DISQUSWIDGETS.getCount({ reset: true });
            });
        };
        SkinDisqusLinksModule.MODULEGUID = "776adfcd-da5f-4926-b29d-4c06353266c0";
        SkinDisqusLinksModule.on = true;
        return SkinDisqusLinksModule;
    }());
    var disqusLinks = new SkinDisqusLinksModule();
    disqusLinks.init();
})(YetaWF_Blog || (YetaWF_Blog = {}));

//# sourceMappingURL=DisqusLinks.js.map
