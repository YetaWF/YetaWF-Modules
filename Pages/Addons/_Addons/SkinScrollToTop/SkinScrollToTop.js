/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

var YetaWF_Pages_ScrollUp = {};
var _YetaWF_Pages_ScrollUp = {};
_YetaWF_Pages_ScrollUp.on = true;

YetaWF_Pages_ScrollUp.init = function() {
    $.scrollUp({
        scrollName: 'yScrollToTop',
        scrollText: ''
    });
};

// Handles events turning the addon on/off (used for dynamic content)
$YetaWF.registerContentChange(function (addonGuid, on) {
    if (addonGuid == '2a4e6f13-24a0-45c1-8a42-f1072e6ac7de') {
        _YetaWF_Pages_ScrollUp.on = on;
    }
});
$YetaWF.addWhenReady(function (tag) {
    if (_YetaWF_Pages_ScrollUp.on) {
        YetaWF_Pages_ScrollUp.init();
    } else {
        $.scrollUp.destroy();
    }
});
