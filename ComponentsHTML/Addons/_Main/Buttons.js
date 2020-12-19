"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
/* initialize buttons (bootstrap and/or jquery-ui) */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var Buttons = /** @class */ (function () {
        function Buttons() {
        }
        Buttons.initButtons = function (tag) {
            if (YVolatile.Skin.Bootstrap && YVolatile.Skin.BootstrapButtons) {
                // bootstrap
                var buttons = $YetaWF.getElementsBySelector("input[type=submit],input[type=button],input[type=reset],input[type=file],a[" + YConfigs.Basics.CssAttrActionButton + "]", [tag]);
                for (var _i = 0, buttons_1 = buttons; _i < buttons_1.length; _i++) {
                    var button = buttons_1[_i];
                    if (!$YetaWF.elementHasClass(button, "y_jqueryui") && !$YetaWF.elementHasClass(button, "btn"))
                        $YetaWF.elementAddClasses(button, ["btn", "btn-primary"]);
                }
                buttons = $YetaWF.getElementsBySelector("button", [tag]);
                for (var _a = 0, buttons_2 = buttons; _a < buttons_2.length; _a++) {
                    var button = buttons_2[_a];
                    if (!$YetaWF.elementHasClass(button, "y_jqueryui") && !$YetaWF.elementHasClass(button, "btn") && !$YetaWF.elementHasClass(button, "yt_dropdownbutton"))
                        $YetaWF.elementAddClasses(button, ["btn"]);
                }
                // explicitly marked for jquery
                buttons = $YetaWF.getElementsBySelector("input[type=submit].y_jqueryui,input[type=button].y_jqueryui,input[type=reset].y_jqueryui,input[type=file].y_jqueryui,button.y_jqueryui,a[" + YConfigs.Basics.CssAttrActionButton + "].y_jqueryui", [tag]);
                for (var _b = 0, buttons_3 = buttons; _b < buttons_3.length; _b++) {
                    var button = buttons_3[_b];
                    $YetaWF.elementAddClasses(button, ["ui-button", "ui-corner-all", "ui-widget"]);
                    $YetaWF.setAttribute(button, "role", "button");
                }
            }
            else {
                var buttons = $YetaWF.getElementsBySelector("input[type=submit],input[type=button],input[type=reset],input[type=file],button,a[" + YConfigs.Basics.CssAttrActionButton + "]", [tag]);
                for (var _c = 0, buttons_4 = buttons; _c < buttons_4.length; _c++) {
                    var button = buttons_4[_c];
                    if (!$YetaWF.elementHasClass(button, "y_bootstrap")) {
                        $YetaWF.elementAddClasses(button, ["ui-button", "ui-corner-all", "ui-widget"]);
                        $YetaWF.setAttribute(button, "role", "button");
                    }
                }
                // explicitly marked for bootstrap
                buttons = $YetaWF.getElementsBySelector("input[type=submit].y_bootstrap,input[type=button].y_bootstrap,input[type=reset].y_bootstrap,input[type=file].y_bootstrap,button.y_bootstrap,a[" + YConfigs.Basics.CssAttrActionButton + "].y_bootstrap", [tag]);
                for (var _d = 0, buttons_5 = buttons; _d < buttons_5.length; _d++) {
                    var button = buttons_5[_d];
                    $YetaWF.elementAddClasses(button, ["btn", "btn-primary"]);
                }
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
