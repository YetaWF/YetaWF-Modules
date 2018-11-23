"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var TextEditComponent = /** @class */ (function () {
        function TextEditComponent() {
        }
        TextEditComponent.initAll = function (tag) {
            if (TextEditComponent.clip != null)
                return;
            var elems = $YetaWF.getElementsBySelector(".yt_text_copy", [tag]);
            if (elems.length === 0)
                return;
            TextEditComponent.clip = new ClipboardJS(".yt_text_copy", {
                target: function (trigger) {
                    return trigger.previousElementSibling;
                },
            });
            TextEditComponent.clip.on("success", function (e) {
                $YetaWF.confirm(YLocs.YetaWF_ComponentsHTML.CopyToClip);
            });
        };
        TextEditComponent.clip = null;
        return TextEditComponent;
    }());
    YetaWF_ComponentsHTML.TextEditComponent = TextEditComponent;
    $YetaWF.addWhenReady(TextEditComponent.initAll);
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=TextEdit.js.map
