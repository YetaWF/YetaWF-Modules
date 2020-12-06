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
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        if (MailtoObfuscatorSkinModule.on) {
            for (var _i = 0, _a = ev.detail.containers; _i < _a.length; _i++) {
                var tag = _a[_i];
                // find all <a> YGenMailTo tags and format an email address
                var elems = $YetaWF.getElementsBySelector("a.YGenMailTo", [tag]);
                for (var _b = 0, elems_1 = elems; _b < elems_1.length; _b++) {
                    var elem = elems_1[_b];
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
        }
        return true;
    });
    // Handles events turning the addon on/off (used for dynamic content)
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, function (ev) {
        var addonGuid = ev.detail.addonGuid;
        var on = ev.detail.on;
        if (addonGuid === "749d0ca9-75e5-40b8-82e3-466a11d3b1d2") {
            MailtoObfuscatorSkinModule.on = on;
        }
        return true;
    });
})(YetaWF_Basics || (YetaWF_Basics = {}));

//# sourceMappingURL=MailToObfuscator.js.map
