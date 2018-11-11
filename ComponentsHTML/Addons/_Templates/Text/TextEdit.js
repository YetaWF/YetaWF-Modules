"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    }
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var TextEditComponent = /** @class */ (function (_super) {
        __extends(TextEditComponent, _super);
        function TextEditComponent() {
            return _super !== null && _super.apply(this, arguments) || this;
        }
        TextEditComponent.initAll = function (tag) {
            if (TextEditComponent.clip != null)
                return;
            var elems = $YetaWF.getElementsBySelector(".yt_text_copy", [tag]);
            if (elems.length == 0)
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
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_ComponentsHTML.TextEditComponent = TextEditComponent;
    $YetaWF.addWhenReady(TextEditComponent.initAll);
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=TextEdit.js.map
