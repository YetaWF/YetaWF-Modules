"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */
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
var YetaWF_Panels;
(function (YetaWF_Panels) {
    var StyleEnum;
    (function (StyleEnum) {
        StyleEnum[StyleEnum["Tabs"] = 0] = "Tabs";
        StyleEnum[StyleEnum["Accordion"] = 1] = "Accordion";
    })(StyleEnum = YetaWF_Panels.StyleEnum || (YetaWF_Panels.StyleEnum = {}));
    var PanelInfoComponent = /** @class */ (function (_super) {
        __extends(PanelInfoComponent, _super);
        function PanelInfoComponent(controlId, setup) {
            var _this = _super.call(this, controlId, PanelInfoComponent.TEMPLATE, PanelInfoComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }) || this;
            _this.Setup = setup;
            if (_this.Setup.Style === StyleEnum.Accordion) {
                $YetaWF.registerEventHandler(_this.Control, "click", "h2", function (ev) {
                    var hActive = ev.__YetaWFElem;
                    var contentActive = hActive.nextElementSibling;
                    var activeShown = _this.isShown(contentActive);
                    var headers = $YetaWF.getElementsBySelector("h2", [_this.Control]);
                    for (var _i = 0, headers_1 = headers; _i < headers_1.length; _i++) {
                        var header = headers_1[_i];
                        _this.hide(header, header.nextElementSibling);
                    }
                    if (!activeShown)
                        _this.show(hActive, hActive.nextElementSibling);
                    return false;
                });
            }
            return _this;
        }
        PanelInfoComponent.prototype.hide = function (header, tag) {
            this.collapse(tag);
            $YetaWF.elementRemoveClasses(header, ["t_active", "t_collapsed"]);
            $YetaWF.elementAddClass(header, "t_collapsed");
            $YetaWF.setAttribute(header, "aria-expanded", "false");
            $YetaWF.setAttribute(header, "aria-selected", "false");
            $YetaWF.setAttribute(header, "tabindex", "-1");
            $YetaWF.elementRemoveClass(tag, "t_active");
            $YetaWF.setAttribute(tag, "aria-hidden", "true");
        };
        PanelInfoComponent.prototype.show = function (header, tag) {
            this.expand(tag);
            $YetaWF.elementRemoveClasses(header, ["t_active", "t_collapsed"]);
            $YetaWF.elementAddClass(header, "t_active");
            $YetaWF.setAttribute(header, "aria-expanded", "true");
            $YetaWF.setAttribute(header, "aria-selected", "true");
            $YetaWF.setAttribute(header, "tabindex", "0");
            $YetaWF.elementRemoveClass(tag, "t_active");
            $YetaWF.elementAddClass(tag, "t_active");
            $YetaWF.setAttribute(tag, "aria-hidden", "false");
        };
        PanelInfoComponent.prototype.isShown = function (tag) {
            return tag.style.display === "block" || tag.style.display === "";
        };
        // helper
        PanelInfoComponent.prototype.expand = function (tag) {
            var steps = PanelInfoComponent.MAXTIME / PanelInfoComponent.INCRTIME;
            var incr = window.innerHeight / steps;
            var height = incr;
            tag.style.maxHeight = "".concat(height, "px");
            tag.style.display = "block";
            var timer = setInterval(function () {
                var rect = tag.getBoundingClientRect();
                height += incr;
                tag.style.maxHeight = "".concat(height, "px");
                var newRect = tag.getBoundingClientRect();
                if (rect.height >= newRect.height) {
                    clearInterval(timer);
                    $YetaWF.sendActivateDivEvent([tag]);
                }
            }, PanelInfoComponent.INCRTIME);
        };
        PanelInfoComponent.prototype.collapse = function (tag) {
            var steps = PanelInfoComponent.MAXTIME / PanelInfoComponent.INCRTIME;
            var rect = tag.getBoundingClientRect();
            var incr = rect.height / steps;
            var height = rect.height - incr;
            if (height <= 0)
                height = 0;
            tag.style.maxHeight = "".concat(height, "px");
            if (height > 0) {
                var timer_1 = setInterval(function () {
                    var rect = tag.getBoundingClientRect();
                    height -= incr;
                    if (height <= 0)
                        height = 0;
                    tag.style.maxHeight = "".concat(height, "px");
                    var newRect = tag.getBoundingClientRect();
                    if (rect.height <= newRect.height || rect.height <= 0) {
                        clearInterval(timer_1);
                        tag.style.display = "none";
                    }
                }, PanelInfoComponent.INCRTIME);
            }
        };
        PanelInfoComponent.TEMPLATE = "yt_panels_panelinfo";
        PanelInfoComponent.SELECTOR = ".yt_panels_panelinfo.t_display";
        PanelInfoComponent.TEMPLATENAME = "YetaWF_Panels_PanelInfo";
        PanelInfoComponent.MAXTIME = 0.6;
        PanelInfoComponent.INCRTIME = 0.03;
        return PanelInfoComponent;
    }(YetaWF.ComponentBaseDataImpl));
    YetaWF_Panels.PanelInfoComponent = PanelInfoComponent;
})(YetaWF_Panels || (YetaWF_Panels = {}));

//# sourceMappingURL=PanelInfo.js.map
