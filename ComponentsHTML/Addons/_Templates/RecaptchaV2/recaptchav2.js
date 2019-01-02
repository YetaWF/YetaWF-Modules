"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var RecaptchaV2 = /** @class */ (function () {
        function RecaptchaV2() {
        }
        RecaptchaV2.recaptchaInit = function (id) {
            var recaptcha = $YetaWF.getElementById(id);
            RecaptchaV2.onLoad(recaptcha);
        };
        RecaptchaV2.onLoad = function (tag) {
            if (typeof grecaptcha === "undefined" || !grecaptcha.render) {
                // keep trying until grecaptcha is available
                setTimeout(function () {
                    RecaptchaV2.onLoad(tag);
                }, 100);
                return;
            }
            grecaptcha.render(tag, {
                "sitekey": YConfigs.YetaWF_ComponentsHTML.SiteKey,
                "theme": YConfigs.YetaWF_ComponentsHTML.Theme,
                "size": YConfigs.YetaWF_ComponentsHTML.Size,
            });
        };
        return RecaptchaV2;
    }());
    YetaWF_ComponentsHTML.RecaptchaV2 = RecaptchaV2;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=recaptchav2.js.map
