"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */
var YetaWF_Pages;
(function (YetaWF_Pages) {
    var ScrollUp = /** @class */ (function () {
        function ScrollUp() {
        }
        ScrollUp.init = function () {
            $.scrollUp({
                scrollName: "yScrollToTop",
                scrollText: ""
            });
        };
        ScrollUp.destroy = function () {
            $.scrollUp.destroy();
        };
        ScrollUp.On = true;
        return ScrollUp;
    }());
    YetaWF_Pages.ScrollUp = ScrollUp;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        if (ScrollUp.On) {
            ScrollUp.init();
        }
        else {
            ScrollUp.destroy();
        }
        return true;
    });
    // Handles events turning the addon on/off (used for dynamic content)
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, function (ev) {
        var addonGuid = ev.detail.addonGuid;
        var on = ev.detail.on;
        if (addonGuid === "2a4e6f13-24a0-45c1-8a42-f1072e6ac7de") {
            ScrollUp.On = on;
        }
        return true;
    });
})(YetaWF_Pages || (YetaWF_Pages = {}));

//# sourceMappingURL=SkinScrollToTop.js.map
