"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */
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
var YetaWF_Panels;
(function (YetaWF_Panels) {
    var StyleEnum;
    (function (StyleEnum) {
        StyleEnum[StyleEnum["Tabs"] = 0] = "Tabs";
        StyleEnum[StyleEnum["AccordionjQuery"] = 1] = "AccordionjQuery";
        StyleEnum[StyleEnum["AccordionKendo"] = 2] = "AccordionKendo";
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
            if (_this.Setup.Style === StyleEnum.AccordionKendo) {
                $YetaWF.registerEventHandler(_this.Control, "click", "ul.t_acckendo > li", function (ev) {
                    var liActive = ev.__YetaWFElem;
                    var contentActive = $YetaWF.getElement1BySelector(".k-content", [liActive]);
                    var activeShown = _this.isShown(contentActive);
                    var lis = $YetaWF.getElementsBySelector("ul.t_acckendo > li", [_this.Control]);
                    for (var _i = 0, lis_1 = lis; _i < lis_1.length; _i++) {
                        var li = lis_1[_i];
                        var liContent = $YetaWF.getElement1BySelector(".k-content", [li]);
                        _this.khide(li, liContent);
                    }
                    if (!activeShown)
                        _this.kshow(liActive, contentActive);
                    return false;
                });
            }
            else if (_this.Setup.Style === StyleEnum.AccordionjQuery) {
                $YetaWF.registerEventHandler(_this.Control, "click", "h3", function (ev) {
                    var hActive = ev.__YetaWFElem;
                    var contentActive = hActive.nextElementSibling;
                    var activeShown = _this.isShown(contentActive);
                    var headers = $YetaWF.getElementsBySelector("h3", [_this.Control]);
                    for (var _i = 0, headers_1 = headers; _i < headers_1.length; _i++) {
                        var header = headers_1[_i];
                        _this.jhide(header, header.nextElementSibling);
                    }
                    if (!activeShown)
                        _this.jshow(hActive, hActive.nextElementSibling);
                    return false;
                });
            }
            // Kendo hover
            $YetaWF.registerEventHandler(_this.Control, "mousemove", "ul.t_acckendo > li .k-header", function (ev) {
                var li = ev.__YetaWFElem;
                $YetaWF.elementAddClass(li, "k-state-hover");
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "mouseout", "ul.t_acckendo > li .k-header", function (ev) {
                var li = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(li, "k-state-hover");
                return true;
            });
            // jqueryui hover
            $YetaWF.registerEventHandler(_this.Control, "mousemove", ".t_accjquery > h3", function (ev) {
                var header = ev.__YetaWFElem;
                $YetaWF.elementAddClass(header, "ui-state-hover");
                return true;
            });
            $YetaWF.registerEventHandler(_this.Control, "mouseout", ".t_accjquery > h3", function (ev) {
                var header = ev.__YetaWFElem;
                $YetaWF.elementRemoveClass(header, "ui-state-hover");
                return true;
            });
            return _this;
        }
        // Kendo
        PanelInfoComponent.prototype.khide = function (li, tag) {
            this.collapse(tag);
            $YetaWF.setAttribute(tag, "aria-hidden", "true");
            $YetaWF.elementRemoveClasses(li, ["k-state-active", "k-state-highlight"]);
            $YetaWF.setAttribute(li, "aria-expanded", "false");
            $YetaWF.setAttribute(li, "aria-selected", "false");
            var header = $YetaWF.getElement1BySelector(".k-header", [li]);
            $YetaWF.elementRemoveClass(header, "k-state-selected");
            var icon = $YetaWF.getElement1BySelector(".k-icon", [li]);
            $YetaWF.elementRemoveClasses(icon, ["k-i-arrow-60-up", "k-panelbar-collapse", "k-i-arrow-60-down", "k-panelbar-expand"]);
            $YetaWF.elementAddClasses(icon, ["k-i-arrow-60-down", "k-panelbar-expand"]);
        };
        PanelInfoComponent.prototype.kshow = function (li, tag) {
            $YetaWF.setAttribute(tag, "aria-hidden", "false");
            this.expand(tag);
            $YetaWF.elementRemoveClasses(li, ["k-state-active", "k-state-highlight"]);
            $YetaWF.elementAddClasses(li, ["k-state-active", "k-state-highlight"]);
            $YetaWF.setAttribute(li, "aria-expanded", "true");
            $YetaWF.setAttribute(li, "aria-selected", "true");
            var header = $YetaWF.getElement1BySelector(".k-header", [li]);
            $YetaWF.elementRemoveClass(header, "k-state-selected");
            $YetaWF.elementAddClass(header, "k-state-selected");
            var icon = $YetaWF.getElement1BySelector(".k-icon", [li]);
            $YetaWF.elementRemoveClasses(icon, ["k-i-arrow-60-up", "k-panelbar-collapse", "k-i-arrow-60-down", "k-panelbar-expand"]);
            $YetaWF.elementAddClasses(icon, ["k-i-arrow-60-up", "k-panelbar-collapse"]);
        };
        PanelInfoComponent.prototype.isShown = function (tag) {
            return tag.style.display === "block" || tag.style.display === "";
        };
        // jQuery-ui
        PanelInfoComponent.prototype.jhide = function (header, tag) {
            this.collapse(tag);
            $YetaWF.elementRemoveClasses(header, ["ui-accordion-header-active", "ui-state-active", "ui-accordion-header-collapsed", "ui-corner-all"]);
            $YetaWF.elementAddClasses(header, ["ui-accordion-header-collapsed", "ui-corner-all"]);
            $YetaWF.setAttribute(header, "aria-expanded", "false");
            $YetaWF.setAttribute(header, "aria-selected", "false");
            $YetaWF.setAttribute(header, "tabindex", "-1");
            $YetaWF.elementRemoveClass(tag, "ui-accordion-content-active");
            $YetaWF.setAttribute(tag, "aria-hidden", "true");
        };
        PanelInfoComponent.prototype.jshow = function (header, tag) {
            this.expand(tag);
            $YetaWF.elementRemoveClasses(header, ["ui-accordion-header-active", "ui-state-active", "ui-accordion-header-collapsed", "ui-corner-all"]);
            $YetaWF.elementAddClasses(header, ["ui-accordion-header-active", "ui-state-active"]);
            $YetaWF.setAttribute(header, "aria-expanded", "true");
            $YetaWF.setAttribute(header, "aria-selected", "true");
            $YetaWF.setAttribute(header, "tabindex", "0");
            $YetaWF.elementRemoveClass(tag, "ui-accordion-content-active");
            $YetaWF.elementAddClass(tag, "ui-accordion-content-active");
            $YetaWF.setAttribute(tag, "aria-hidden", "false");
        };
        // helper
        PanelInfoComponent.prototype.expand = function (tag) {
            var steps = PanelInfoComponent.MAXTIME / PanelInfoComponent.INCRTIME;
            var incr = window.innerHeight / steps;
            var height = incr;
            tag.style.maxHeight = height + "px";
            tag.style.display = "block";
            var timer = setInterval(function () {
                var rect = tag.getBoundingClientRect();
                height += incr;
                tag.style.maxHeight = height + "px";
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
            tag.style.maxHeight = height + "px";
            if (height > 0) {
                var timer_1 = setInterval(function () {
                    var rect = tag.getBoundingClientRect();
                    height -= incr;
                    if (height <= 0)
                        height = 0;
                    tag.style.maxHeight = height + "px";
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
