/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Licensing */
// If this javascript snippet is included, that means we're displaying the chat.
var ActiveEngage_Conversation;
(function (ActiveEngage_Conversation) {
    var SkinPageView2Module = (function () {
        function SkinPageView2Module() {
        }
        /**
         * Initializes the module instance.
         */
        SkinPageView2Module.prototype.init = function () {
            var pv2 = this;
            YetaWF_Basics.addWhenReady(function (section) {
                pv2.initSection(section);
            });
            YetaWF_Basics.RegisterContentChange(function (event, addonGuid, on) {
                if (addonGuid === SkinPageView2Module.MODULEGUID) {
                    SkinPageView2Module.on = on;
                }
            });
            YetaWF_Basics.RegisterNewPage(function (event, url) {
                pv2.showInvite(true);
                if (typeof ActivEngage !== "undefined" && ActivEngage !== undefined) {
                    if (typeof ActivEngage.recordPageView !== "undefined" && ActivEngage.recordPageView !== undefined)
                        ActivEngage.recordPageView({ "href": url });
                }
            });
        };
        /**
         * Show/hide chat invite
         * @param True to show, false to hide.
         */
        SkinPageView2Module.prototype.showInvite = function (show) {
            var body = document.querySelector("body");
            if (!body)
                return;
            var inviteElem = document.querySelector("#ae-launcher-container");
            if (!inviteElem)
                return;
            var invite = show;
            if (invite) {
                var inclCss = YConfigs.ActivEngage_Conversation.IncludedPagesCss;
                var exclCss = YConfigs.ActivEngage_Conversation.ExcludedPagesCss;
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
            if (invite)
                inviteElem.style.display = "";
            else
                inviteElem.style.display = "none";
        };
        /**
         * Initializes all chat invite elements in the specified tag.
         * @param tag - an element that was just updated that may contain chat invite elements.
         */
        SkinPageView2Module.prototype.initSection = function (tag) {
            this.showInvite(SkinPageView2Module.on);
        };
        SkinPageView2Module.MODULEGUID = "c063e089-aff3-44e4-ac44-063911853579";
        SkinPageView2Module.on = true;
        return SkinPageView2Module;
    }());
    var pv2 = new SkinPageView2Module();
    pv2.init();
})(ActiveEngage_Conversation || (ActiveEngage_Conversation = {}));
