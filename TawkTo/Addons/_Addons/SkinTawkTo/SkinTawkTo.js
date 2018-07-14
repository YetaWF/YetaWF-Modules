"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TawkTo#License */
// If this javascript snippet is included, that means we're displaying the chat.
var Tawk_API;
var YetaWF_TawkTo;
(function (YetaWF_TawkTo) {
    var SkinTawkToModule = /** @class */ (function () {
        function SkinTawkToModule() {
        }
        /**
         * Initializes the module instance.
         */
        SkinTawkToModule.prototype.init = function () {
            var tawkto = this;
            YetaWF_Basics.registerContentChange(function (addonGuid, on) {
                if (addonGuid === SkinTawkToModule.MODULEGUID) {
                    SkinTawkToModule.on = on;
                }
            });
            YetaWF_Basics.registerNewPage(function (url) {
                tawkto.showInvite(SkinTawkToModule.on);
                if (SkinTawkToModule.on) {
                    // Functionality not available in Tawk.to to record a new page
                    //if (typeof ActivEngage !== "undefined" && ActivEngage !== undefined) {
                    //    if (typeof ActivEngage.recordPageView !== "undefined" && ActivEngage.recordPageView !== undefined)
                    //        ActivEngage.recordPageView({ "href": url });
                    //}
                }
            });
        };
        /**
         * Show/hide chat invite
         * @param True to show, false to hide.
         */
        SkinTawkToModule.prototype.showInvite = function (show) {
            if (!Tawk_API || !Tawk_API.showWidget)
                return; // not yet initialized
            var body = document.querySelector("body");
            if (!body)
                return;
            var invite = show;
            if (invite) {
                var inclCss = YConfigs.YetaWF_TawkTo.IncludedPagesCss;
                var exclCss = YConfigs.YetaWF_TawkTo.ExcludedPagesCss;
                if (inclCss && inclCss.length > 0) {
                    // only included css pages show the chat invite
                    invite = false;
                    if (inclCss) {
                        var csses = inclCss.split(" ");
                        for (var _i = 0, csses_1 = csses; _i < csses_1.length; _i++) {
                            var css = csses_1[_i];
                            if (YetaWF_Basics.elementHasClass(body, css)) {
                                invite = true;
                                break;
                            }
                        }
                    }
                }
                // now check if page is explicitly excluded
                if (exclCss && invite) {
                    csses = exclCss.split(" ");
                    for (var _a = 0, csses_2 = csses; _a < csses_2.length; _a++) {
                        css = csses_2[_a];
                        if (YetaWF_Basics.elementHasClass(body, css)) {
                            invite = false;
                            break;
                        }
                    }
                }
            }
            if (!invite && show) {
                if (Tawk_API.isVisitorEngaged())
                    invite = true;
            }
            if (invite) {
                Tawk_API.showWidget();
            }
            else {
                Tawk_API.hideWidget();
            }
        };
        SkinTawkToModule.MODULEGUID = "c063e089-aff3-44e4-ac44-063911853579";
        SkinTawkToModule.on = true;
        return SkinTawkToModule;
    }());
    var tawkto = new SkinTawkToModule();
    tawkto.init();
})(YetaWF_TawkTo || (YetaWF_TawkTo = {}));

//# sourceMappingURL=SkinTawkTo.js.map
