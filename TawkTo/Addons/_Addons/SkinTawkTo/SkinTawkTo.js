/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TawkTo#License */
// If this javascript snippet is included, that means we're displaying the chat.
var Tawk_API;
var ActiveEngage_Conversation;
(function (ActiveEngage_Conversation) {
    var SkinTawkToModule = (function () {
        function SkinTawkToModule() {
        }
        /**
         * Initializes the module instance.
         */
        SkinTawkToModule.prototype.init = function () {
            var tawkto = this;
            YetaWF_Basics.addWhenReady(function (section) {
                tawkto.initSection(section);
            });
            YetaWF_Basics.RegisterContentChange(function (event, addonGuid, on) {
                if (addonGuid === SkinTawkToModule.MODULEGUID) {
                    SkinTawkToModule.on = on;
                }
            });
            YetaWF_Basics.RegisterNewPage(function (event, url) {
                tawkto.showInvite(true);
                // Functionality not available in Tawk.to to record a new page
                //if (typeof ActivEngage !== "undefined" && ActivEngage !== undefined) {
                //    if (typeof ActivEngage.recordPageView !== "undefined" && ActivEngage.recordPageView !== undefined)
                //        ActivEngage.recordPageView({ "href": url });
                //}
            });
        };
        /**
         * Show/hide chat invite
         * @param True to show, false to hide.
         */
        SkinTawkToModule.prototype.showInvite = function (show) {
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
            if (!invite) {
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
        /**
         * Initializes all chat invite elements in the specified tag.
         * @param tag - an element that was just updated that may contain chat invite elements.
         */
        SkinTawkToModule.prototype.initSection = function (tag) {
            this.showInvite(SkinTawkToModule.on);
        };
        SkinTawkToModule.MODULEGUID = "c063e089-aff3-44e4-ac44-063911853579";
        SkinTawkToModule.on = true;
        return SkinTawkToModule;
    }());
    var tawkto = new SkinTawkToModule();
    tawkto.init();
})(ActiveEngage_Conversation || (ActiveEngage_Conversation = {}));

//# sourceMappingURL=SkinTawkTo.js.map
