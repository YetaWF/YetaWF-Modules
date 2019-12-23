/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

// This saves functions that are in conflict with other libs (notably Bootstrap)
// the matching postfixup.js must run last

var YetaWF_Core_jqueryui = {};

+function ($) {
    'use strict';
    YetaWF_Core_jqueryui.button = $.fn.button;
}(jQuery);