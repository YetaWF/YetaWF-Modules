"use strict";
/* Copyright � 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */
var YetaWF_PageEdit;
(function (YetaWF_PageEdit) {
    var PageControlModule = /** @class */ (function () {
        function PageControlModule() {
        }
        PageControlModule.prototype.init = function () {
            $YetaWF.addWhenReadyOnce(function () {
                var fadeTime = 250;
                var mod = $YetaWF.getElementByIdCond(YConfigs.YetaWF_PageEdit.PageControlMod);
                if (mod) {
                    // Page icon
                    var pagebutton = $YetaWF.getElementById("yPageControlButton");
                    $YetaWF.registerEventHandler(pagebutton, "click", null, function (ev) {
                        if ($YetaWF.isVisible(mod)) {
                            YVolatile.Basics.PageControlVisible = false;
                            ComponentsHTML.fadeOut(mod, fadeTime);
                        }
                        else {
                            YVolatile.Basics.PageControlVisible = true;
                            ComponentsHTML.fadeIn(mod, fadeTime);
                        }
                        return false;
                    });
                    // on page load, show control panel if wanted
                    if (YVolatile.Basics.PageControlVisible) {
                        mod.style.display = "block";
                        ComponentsHTML.processPropertyListVisible(mod);
                    }
                }
            });
            // handle Page Settings, Remove Current Page, W3C Validation - this is needed in case we're in a unified page set
            // in which case the original pageguid and url in the module actions have changed
            // when a new page becomes active, update the module actions reflecting the new page/url
            // also update all hidden fields with the new current page guid
            $YetaWF.addWhenReady(function (tag) {
                // disregards tag as this callback is used when a page or page content has been rendered
                // Page Settings
                var pagebutton = $YetaWF.getElementByIdCond("yPageControlButton");
                if (pagebutton) {
                    if (YVolatile.Basics.TemporaryPage) {
                        if (YVolatile.Basics.PageControlVisible) {
                            var mod = $YetaWF.getElementById(YConfigs.YetaWF_PageEdit.PageControlMod);
                            YVolatile.Basics.PageControlVisible = false;
                            mod.style.display = "none";
                        }
                        pagebutton.style.display = "none";
                    }
                    else {
                        pagebutton.style.display = "block";
                    }
                }
                if (!$YetaWF.isInPopup()) {
                    var ps = $YetaWF.getElement1BySelectorCond(".YetaWF_PageEdit_PageControl a[data-name='PageSettings']");
                    if (ps) {
                        var uri = $YetaWF.parseUrl(ps.href);
                        uri.removeSearch("PageGuid");
                        uri.addSearch("PageGuid", YVolatile.Basics.PageGuid);
                        ps.href = uri.toUrl();
                    }
                    // Export Page
                    var ep = $YetaWF.getElement1BySelectorCond(".YetaWF_PageEdit_PageControl a[data-name='ExportPage']");
                    if (ep) {
                        var uri = $YetaWF.parseUrl(ep.href);
                        uri.removeSearch("PageGuid");
                        uri.addSearch("PageGuid", YVolatile.Basics.PageGuid);
                        ep.href = uri.toUrl();
                    }
                    // Remove Page
                    var rp = $YetaWF.getElement1BySelectorCond(".YetaWF_PageEdit_PageControl a[data-name='RemovePage']");
                    if (rp) {
                        var uri = $YetaWF.parseUrl(rp.href);
                        uri.removeSearch("PageGuid");
                        uri.addSearch("PageGuid", YVolatile.Basics.PageGuid);
                        rp.href = uri.toUrl();
                    }
                    // W3C validation
                    var w3c = $YetaWF.getElement1BySelectorCond(".YetaWF_PageEdit_PageControl a[data-name='W3CValidate']");
                    if (w3c)
                        w3c.href = YConfigs.YetaWF_PageEdit.W3CUrl.format(window.location);
                    var hidden = $YetaWF.getElementsBySelector(".YetaWF_PageEdit_PageControl input[name='CurrentPageGuid'][type='hidden']");
                    for (var _i = 0, hidden_1 = hidden; _i < hidden_1.length; _i++) {
                        var h = hidden_1[_i];
                        h.value = YVolatile.Basics.PageGuid;
                    }
                }
            });
        };
        return PageControlModule;
    }());
    var pageEdit = new PageControlModule();
    pageEdit.init();
})(YetaWF_PageEdit || (YetaWF_PageEdit = {}));

//# sourceMappingURL=PageControl.js.map
