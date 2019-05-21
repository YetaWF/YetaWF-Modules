"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    //$$$$ NEED ComponentBaseNoDataImpl
    var TextAreaSourceOnlyComponent = /** @class */ (function () {
        function TextAreaSourceOnlyComponent() {
        }
        TextAreaSourceOnlyComponent.initAll = function (tag) {
            if (TextAreaSourceOnlyComponent.clip != null)
                return;
            var elems = $YetaWF.getElementsBySelector(".yt_textareasourceonly_copy", [tag]);
            if (elems.length === 0)
                return;
            TextAreaSourceOnlyComponent.clip = new ClipboardJS(".yt_textareasourceonly_copy", {
                target: function (trigger) {
                    return trigger.previousElementSibling;
                },
            });
            TextAreaSourceOnlyComponent.clip.on("success", function (e) {
                $YetaWF.confirm(YLocs.YetaWF_ComponentsHTML.CopyToClip);
            });
        };
        TextAreaSourceOnlyComponent.clip = null;
        return TextAreaSourceOnlyComponent;
    }());
    YetaWF_ComponentsHTML.TextAreaSourceOnlyComponent = TextAreaSourceOnlyComponent;
    $YetaWF.addWhenReady(TextAreaSourceOnlyComponent.initAll);
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=TextAreaSourceOnly.js.map
