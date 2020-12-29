"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
/* initialize buttons (bootstrap and/or jquery-ui) */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var Buttons = /** @class */ (function () {
        function Buttons() {
        }
        Buttons.initButtons = function (tag) {
            // let buttons = $YetaWF.getElementsBySelector(`button,input[type=submit],input[type=button],input[type=reset],input[type=file],a[${YConfigs.Basics.CssAttrActionButton}]`, [tag]);
            // for (let button of buttons) {
            //     if (!$YetaWF.elementHasClass(button, "yt_dropdownbutton")) {
            //         $YetaWF.elementAddClass(button, "y_button");
            //         $YetaWF.setAttribute(button, "role", "button");
            //     }
            // }
            // Diagnostics, find old bootstrap and kendo buttons, which shouldn't be used any longer.
            if (YConfigs.Basics.DEBUGBUILD) {
                var invalids = $YetaWF.getElementsBySelector(".y_bootstrap");
                if (invalids.length)
                    throw "Elements with .y_bootstrap found - no longer supported";
                invalids = $YetaWF.getElementsBySelector(".btn,.btn-primary");
                if (invalids.length)
                    throw "Elements with .btn,.btn-primary found - no longer supported";
                invalids = $YetaWF.getElementsBySelector(".y_jqueryui");
                if (invalids.length)
                    throw "Elements with .y_jqueryui found - no longer supported";
                invalids = $YetaWF.getElementsBySelector(".ui-button");
                if (invalids.length)
                    throw "Elements with .ui-button found - no longer supported";
            }
        };
        return Buttons;
    }());
    YetaWF_ComponentsHTML.Buttons = Buttons;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        for (var _i = 0, _a = ev.detail.containers; _i < _a.length; _i++) {
            var container = _a[_i];
            YetaWF_ComponentsHTML.Buttons.initButtons(container);
        }
        return true;
    });
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));

//# sourceMappingURL=Buttons.js.map
