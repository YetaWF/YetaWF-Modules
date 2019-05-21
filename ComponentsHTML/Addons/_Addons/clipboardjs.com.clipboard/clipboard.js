"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var ClipboardSupport = /** @class */ (function () {
        function ClipboardSupport() {
        }
        ClipboardSupport.initAll = function (tag) {
            if (ClipboardSupport.clip != null)
                return;
            var elems = $YetaWF.getElementsBySelector(".yt_text_copy", [tag]);
            if (elems.length === 0)
                return;
            ClipboardSupport.clip = new ClipboardJS(".yt_text_copy", {
                target: function (trigger) {
                    return trigger.previousElementSibling;
                },
            });
            ClipboardSupport.clip.on("success", function (e) {
                $YetaWF.confirm(YLocs.YetaWF_ComponentsHTML.CopyToClip);
            });
        };
        ClipboardSupport.clip = null;
        return ClipboardSupport;
    }());
    YetaWF_ComponentsHTML.ClipboardSupport = ClipboardSupport;
    /* handle copy icon */
    $YetaWF.addWhenReady(ClipboardSupport.initAll);
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Clipboard.js.map
