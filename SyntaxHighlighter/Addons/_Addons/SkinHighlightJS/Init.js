/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

var YetaWF_SyntaxHighlighterHighlightJS = {};

var YetaWF_SyntaxHighlighter_HighlightJS = {};

YetaWF_SyntaxHighlighter_HighlightJS.Init = function () {
    'use strict';
};
YetaWF_SyntaxHighlighterHighlightJS.highlight = function ($tag) {
    'use strict';
    $('pre code,pre', $tag).each(function (i, block) {
        hljs.highlightBlock(block);
    });
};

YetaWF_SyntaxHighlighter_HighlightJS.on = true; // initial state

// Handles events turning the addon on/off (used for dynamic content)
$(document).on('YetaWF_Basics_Addon', function (event, addonGuid, on) {
    if (addonGuid == '25068AC6-BA74-4644-8B46-9D7FEC291E45')
        YetaWF_SyntaxHighlighter_HighlightJS.on = on;
});
YetaWF_Basics.whenReady.push({
    callback: function ($tag) {
        if (YetaWF_SyntaxHighlighter_HighlightJS.on)
            YetaWF_SyntaxHighlighterHighlightJS.highlight($tag);
    }
});
