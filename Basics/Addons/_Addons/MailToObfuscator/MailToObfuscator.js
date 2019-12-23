"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */
var YetaWF_Basics;
(function (YetaWF_Basics) {
    var MailtoObfuscatorSkinModule = /** @class */ (function () {
        function MailtoObfuscatorSkinModule() {
        }
        MailtoObfuscatorSkinModule.on = true;
        return MailtoObfuscatorSkinModule;
    }());
    YetaWF_Basics.MailtoObfuscatorSkinModule = MailtoObfuscatorSkinModule;
    // http://stackoverflow.com/questions/483212/effective-method-to-hide-email-from-spam-bots
    $YetaWF.addWhenReady(function (tag) {
        if (MailtoObfuscatorSkinModule.on) {
            // find all <a> YGenMailTo tags and format an email address
            var elems = $YetaWF.getElementsBySelector("a.YGenMailTo", [tag]);
            for (var _i = 0, elems_1 = elems; _i < elems_1.length; _i++) {
                var elem = elems_1[_i];
                var addr = $YetaWF.getAttribute(elem, "data-name") + "@" + $YetaWF.getAttribute(elem, "data-domain");
                var s = "mailto:" + addr;
                var subj = $YetaWF.getAttributeCond(elem, "data-subject");
                if (subj) {
                    subj = subj.replace(" ", "+");
                    s += "?subject=" + encodeURI(subj);
                }
                var text = $YetaWF.getAttributeCond(elem, "data-text");
                if (!text)
                    elem.innerText = addr;
                else
                    elem.innerText = text;
                elem.href = s;
            }
        }
    });
    // Handles events turning the addon on/off (used for dynamic content)
    $YetaWF.registerContentChange(function (addonGuid, on) {
        if (addonGuid === "749d0ca9-75e5-40b8-82e3-466a11d3b1d2") {
            MailtoObfuscatorSkinModule.on = on;
        }
    });
})(YetaWF_Basics || (YetaWF_Basics = {}));

//# sourceMappingURL=MailToObfuscator.js.map
