/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

var YetaWF_Core_RecaptchaV2 = {};

YetaWF_Core_RecaptchaV2.onLoad = function ($tag) {
    'use strict';
    if (typeof grecaptcha === 'undefined' || !grecaptcha.render) {
        // keep trying until grecaptcha is available
        setTimeout(function() {
            YetaWF_Core_RecaptchaV2.onLoad($tag);
        }, 100);
        return;
    }
    $('.yt_recaptchav2', $tag).each(function () {
        grecaptcha.render(this, {
            'sitekey': YConfigs.YetaWF_ComponentsHTML.SiteKey,
            'theme': YConfigs.YetaWF_ComponentsHTML.Theme,
            'size': YConfigs.YetaWF_ComponentsHTML.Size,
        });
    });
};

YetaWF_Basics.addWhenReady(function (tag) {
    YetaWF_Core_RecaptchaV2.onLoad($(tag));
});
