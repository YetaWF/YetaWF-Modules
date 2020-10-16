"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TawkTo#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
// If this javascript snippet is included, that means we're displaying the chat.
// tslint:disable-next-line:variable-name
var Tawk_API;
var YetaWF_TawkTo;
(function (YetaWF_TawkTo) {
    var SkinTawkToModule = /** @class */ (function (_super) {
        __extends(SkinTawkToModule, _super);
        function SkinTawkToModule(id) {
            var _this = _super.call(this, id, SkinTawkToModule.SELECTOR, null) || this;
            $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
                _this.showInvite(SkinTawkToModule.on);
                if (SkinTawkToModule.on) {
                    // Functionality not available in Tawk.to to record a new page
                    //if (typeof ActivEngage !== "undefined" && ActivEngage !== undefined) {
                    //    if (typeof ActivEngage.recordPageView !== "undefined" && ActivEngage.recordPageView !== undefined)
                    //        ActivEngage.recordPageView({ "href": url });
                    //}
                }
                return true;
            });
            return _this;
        }
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
                            if ($YetaWF.elementHasClass(body, css)) {
                                invite = true;
                                break;
                            }
                        }
                    }
                }
                // now check if page is explicitly excluded
                if (exclCss && invite) {
                    var csses = exclCss.split(" ");
                    for (var _a = 0, csses_2 = csses; _a < csses_2.length; _a++) {
                        var css = csses_2[_a];
                        if ($YetaWF.elementHasClass(body, css)) {
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
        SkinTawkToModule.SELECTOR = ".YetaWF_TawkTo_FileDocumentShow";
        SkinTawkToModule.MODULEGUID = "c063e089-aff3-44e4-ac44-063911853579";
        SkinTawkToModule.on = true;
        return SkinTawkToModule;
    }(YetaWF.ModuleBaseNoDataImpl));
    YetaWF_TawkTo.SkinTawkToModule = SkinTawkToModule;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, function (ev) {
        var addonGuid = ev.detail.addonGuid;
        var on = ev.detail.on;
        if (addonGuid === SkinTawkToModule.MODULEGUID) {
            SkinTawkToModule.on = on;
        }
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTBEFOREPRINT, null, function (ev) {
        if (Tawk_API) {
            Tawk_API.hideWidget();
        }
        return false;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTAFTERPRINT, null, function (ev) {
        if (Tawk_API) {
            Tawk_API.showWidget();
        }
        return false;
    });
})(YetaWF_TawkTo || (YetaWF_TawkTo = {}));

//# sourceMappingURL=SkinTawkTo.js.map
