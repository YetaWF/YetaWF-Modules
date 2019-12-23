"use strict";
/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
var YetaWF_PageEdit;
(function (YetaWF_PageEdit) {
    var PageControlModule = /** @class */ (function (_super) {
        __extends(PageControlModule, _super);
        function PageControlModule(id) {
            var _this = _super.call(this, id, PageControlModule.SELECTOR, null) || this;
            _this.FadeTime = 250;
            _this.PageControlMod = $YetaWF.getElementById(YConfigs.YetaWF_PageEdit.PageControlMod);
            var pagebutton = $YetaWF.getElementById("tid_pagecontrolbutton");
            $YetaWF.registerEventHandler(pagebutton, "click", null, function (ev) {
                _this.toggleControlPanel();
                return false;
            });
            // on page load, show control panel if wanted
            if (YVolatile.Basics.PageControlVisible) {
                _this.PageControlMod.style.display = "block";
                ComponentsHTMLHelper.processPropertyListVisible(_this.PageControlMod);
            }
            // handle Page Settings, Remove Current Page, W3C Validation - this is needed in case we're in a unified page set
            // in which case the original pageguid and url in the module actions have changed
            // when a new page becomes active, update the module actions reflecting the new page/url
            // also update all hidden fields with the new current page guid
            $YetaWF.addWhenReady(function (tag) {
                if ($YetaWF.isInPopup()) {
                    if (YVolatile.Basics.PageControlVisible) {
                        YVolatile.Basics.PageControlVisible = false;
                        _this.toggleControlPanel();
                    }
                    return;
                }
                var pagebutton = $YetaWF.getElementByIdCond("tid_pagecontrolbutton");
                if (pagebutton) {
                    if (YVolatile.Basics.TemporaryPage) {
                        if (YVolatile.Basics.PageControlVisible) {
                            YVolatile.Basics.PageControlVisible = false;
                            _this.toggleControlPanel();
                        }
                        pagebutton.style.display = "none";
                    }
                    else {
                        pagebutton.style.display = "block";
                    }
                }
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
            });
            return _this;
        }
        PageControlModule.prototype.toggleControlPanel = function () {
            if (!this.PageControlMod)
                return;
            if ($YetaWF.isVisible(this.PageControlMod)) {
                YVolatile.Basics.PageControlVisible = false;
                ComponentsHTMLHelper.fadeOut(this.PageControlMod, this.FadeTime);
            }
            else {
                YVolatile.Basics.PageControlVisible = true;
                ComponentsHTMLHelper.fadeIn(this.PageControlMod, this.FadeTime);
                ComponentsHTMLHelper.processPropertyListVisible(this.Module);
            }
        };
        PageControlModule.SELECTOR = ".YetaWF_PageEdit_PageControl";
        return PageControlModule;
    }(YetaWF.ModuleBaseNoDataImpl));
    YetaWF_PageEdit.PageControlModule = PageControlModule;
})(YetaWF_PageEdit || (YetaWF_PageEdit = {}));

//# sourceMappingURL=PageControl.js.map
