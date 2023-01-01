"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */
var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (Object.prototype.hasOwnProperty.call(b, p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        if (typeof b !== "function" && b !== null)
            throw new TypeError("Class extends value " + String(b) + " is not a constructor or null");
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
            var pagebutton = $YetaWF.getElement1BySelector(".t_controlpanel", [_this.Module]);
            $YetaWF.registerEventHandler(pagebutton, "click", null, function (ev) {
                _this.toggleControlPanel();
                _this.updateURL();
                return false;
            });
            // on page load, show control panel if wanted
            if (YVolatile.Basics.PageControlVisible) {
                _this.PageControlMod.style.display = "block";
                ComponentsHTMLHelper.processPropertyListVisible(_this.PageControlMod);
            }
            return _this;
        }
        PageControlModule.prototype.updateURL = function () {
            var uri = new YetaWF.Url();
            uri.parse(window.location.href);
            uri.removeSearch("!Pagectl");
            if (YVolatile.Basics.PageControlVisible)
                uri.addSearch("!Pagectl", "y");
            $YetaWF.setUrl(uri.toUrl());
        };
        PageControlModule.prototype.toggleControlPanel = function () {
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
        PageControlModule.prototype.updateControlPanel = function () {
            if ($YetaWF.isInPopup()) {
                if (YVolatile.Basics.PageControlVisible) {
                    YVolatile.Basics.PageControlVisible = false;
                    this.toggleControlPanel();
                }
                return;
            }
            var pagebutton = $YetaWF.getElement1BySelector(".t_controlpanel", [this.Module]);
            if (pagebutton) {
                if (YVolatile.Basics.TemporaryPage) {
                    if (YVolatile.Basics.PageControlVisible) {
                        YVolatile.Basics.PageControlVisible = false;
                        this.toggleControlPanel();
                    }
                    pagebutton.style.display = "none";
                }
                else {
                    pagebutton.style.display = "block";
                }
            }
            var ps = $YetaWF.getElement1BySelectorCond("a[data-name='PageSettings']", [this.Module]);
            if (ps) {
                var uri = $YetaWF.parseUrl(ps.href);
                uri.removeSearch("PageGuid");
                uri.addSearch("PageGuid", YVolatile.Basics.PageGuid);
                ps.href = uri.toUrl();
            }
            // Export Page
            var ep = $YetaWF.getElement1BySelectorCond("a[data-name='ExportPage']", [this.Module]);
            if (ep) {
                var uri = $YetaWF.parseUrl(ep.href);
                uri.removeSearch("PageGuid");
                uri.addSearch("PageGuid", YVolatile.Basics.PageGuid);
                ep.href = uri.toUrl();
            }
            // Remove Page
            var rp = $YetaWF.getElement1BySelectorCond("a[data-name='RemovePage']", [this.Module]);
            if (rp) {
                var uri = $YetaWF.parseUrl(rp.href);
                uri.removeSearch("PageGuid");
                uri.addSearch("PageGuid", YVolatile.Basics.PageGuid);
                rp.href = uri.toUrl();
            }
            // W3C validation
            var w3c = $YetaWF.getElement1BySelectorCond("a[data-name='W3CValidate']", [this.Module]);
            if (w3c)
                w3c.href = YConfigs.YetaWF_PageEdit.W3CUrl.format(window.location);
            var hidden = $YetaWF.getElementsBySelector("input[name='CurrentPageGuid'][type='hidden']", [this.Module]);
            for (var _i = 0, hidden_1 = hidden; _i < hidden_1.length; _i++) {
                var h = hidden_1[_i];
                h.value = YVolatile.Basics.PageGuid;
            }
        };
        PageControlModule.SELECTOR = ".YetaWF_PageEdit_PageControl";
        return PageControlModule;
    }(YetaWF.ModuleBaseDataImpl));
    YetaWF_PageEdit.PageControlModule = PageControlModule;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        var mods = YetaWF.ModuleBaseDataImpl.getModules(PageControlModule.SELECTOR);
        for (var _i = 0, mods_1 = mods; _i < mods_1.length; _i++) {
            var mod = mods_1[_i];
            mod.updateControlPanel();
        }
        return true;
    });
})(YetaWF_PageEdit || (YetaWF_PageEdit = {}));

//# sourceMappingURL=PageControl.js.map
