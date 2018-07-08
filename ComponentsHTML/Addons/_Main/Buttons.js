"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
/* initialize buttons (bootstrap and/or jquery-ui) */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var Buttons = /** @class */ (function () {
        function Buttons() {
        }
        Buttons.init = function ($tag) {
            if (YVolatile.Skin.Bootstrap && YVolatile.Skin.BootstrapButtons) {
                // bootstrap
                $("input[type=submit],input[type=button],input[type=reset],input[type=file]", $tag).not('.y_jqueryui,.btn').addClass('btn btn-primary');
                $("button", $tag).not('.y_jqueryui,.yt_actionicons,.btn').addClass('btn');
                $("a[" + YConfigs.Basics.CssAttrActionButton + "]", $tag).not('.y_jqueryui,.btn').addClass('btn btn-primary'); // action link as a button
                // explicitly marked for jquery
                $("input[type=submit].y_jqueryui,input[type=button].y_jqueryui,input[type=reset].y_jqueryui,input[type=file].y_jqueryui,button.y_jqueryui", $tag).button();
                $("a[" + YConfigs.Basics.CssAttrActionButton + "].y_jqueryui", $tag).button(); // action link as a button
            }
            else {
                // jquery-ui
                $("input[type=submit],input[type=button],input[type=reset],input[type=file],button", $tag).not('.y_bootstrap').button(); // beautify all buttons
                $("a[" + YConfigs.Basics.CssAttrActionButton + "]", $tag).not('.y_bootstrap').button(); // action link as a button
                // explicitly marked for bootstrap
                $("input[type=submit].y_bootstrap,input[type=button].y_bootstrap,input[type=reset].y_bootstrap,input[type=file].y_bootstrap", $tag).addClass('btn btn-primary');
                $("button.y_bootstrap", $tag).addClass('btn btn-primary');
                $("a[" + YConfigs.Basics.CssAttrActionButton + "].y_bootstrap", $tag).addClass('btn btn-primary'); // action link as a button
            }
        };
        return Buttons;
    }());
    YetaWF_ComponentsHTML.Buttons = Buttons;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
;
YetaWF_Basics.whenReady.push({
    callback: YetaWF_ComponentsHTML.Buttons.init
});
