"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    /**
     * Implements a stand-alone tooltip.
     */
    var Tooltips = /** @class */ (function () {
        function Tooltips() {
            var _this = this;
            this.TOOLTIPCLASS = "yTooltip";
            this.TOOLTIPACTIVEELEMCLASS = "yTooltipActive";
            this.fadeInTime = 200;
            this.fadeOutTime = 200;
            this.activeTooltipElem = null;
            this.activeTooltip = null;
            this.CancelObject = { Canceled: false, Active: false };
            var noTooltips = this.getNoTooltipSelectors(YVolatile.Basics.CssNoTooltips);
            var noTTImgSel = this.buildNoTT("img", noTooltips);
            var noTTASel = this.buildNoTT("a", noTooltips);
            var noTTISel = this.buildNoTT("i", noTooltips);
            var a2 = YConfigs.Basics.CssTooltip;
            var a3 = YConfigs.Basics.CssTooltipSpan;
            var ttMisc = "th[" + a2 + "],span[" + a3 + "],li[" + a2 + "],div[" + a2 + "]";
            var selectors = "label,input,a,button," + noTTImgSel + "," + noTTASel + "," + noTTISel + "," + ttMisc;
            $YetaWF.registerMultipleEventHandlersBody(["mouseover", "click"], "" + selectors, function (ev) {
                var elem = ev.__YetaWFElem;
                for (;;) {
                    if (!elem)
                        return true;
                    if (ev.type !== "click" && elem === _this.activeTooltipElem)
                        return true;
                    if (!$YetaWF.elementMatches(elem, ":hover") && $YetaWF.elementMatches(elem, ":focus"))
                        return true;
                    if ($YetaWF.getAttributeCond(elem, "disabled"))
                        return true;
                    var s = $YetaWF.getAttributeCond(elem, YConfigs.Basics.CssTooltip) || $YetaWF.getAttributeCond(elem, YConfigs.Basics.CssTooltipSpan) || $YetaWF.getAttributeCond(elem, "title");
                    if (s) {
                        _this.showTooltip(elem, s);
                        return true;
                    }
                    if (elem.tagName !== "IMG" && elem.tagName !== "I")
                        break;
                    // we're in an IMG or I tag, find enclosing A (if any) and try again
                    elem = $YetaWF.elementClosestCond(elem, noTTASel);
                    if (!elem)
                        return true;
                    // if the a link is a menu, don't show a tooltip for the image because the tooltip would be in a bad location
                    if ($YetaWF.elementClosestCond(elem, ".k-menu"))
                        return true;
                }
                // nothing so far, check <a> to external site
                if (elem.tagName === "A") {
                    var anchor = elem;
                    var href = anchor.href;
                    if (href === undefined || href.startsWith("javascript") || href.startsWith("#") || href.startsWith("mailto:"))
                        return true;
                    if (anchor.target === "_blank") {
                        var uri = $YetaWF.parseUrl(href);
                        _this.showTooltip(elem, YLocs.Basics.OpenNewWindowTT.format(uri.getHostName()));
                        return true;
                    }
                }
                return true;
            });
            $YetaWF.registerEventHandlerBody("mousedown", null, function (ev) {
                _this.removeTooltips();
                return true;
            });
            $YetaWF.registerEventHandlerBody("mouseout", "." + this.TOOLTIPACTIVEELEMCLASS, function (ev) {
                if (_this.activeTooltip && _this.activeTooltipElem && (ev.__YetaWFElem === _this.activeTooltipElem && !_this.activeTooltipElem.contains(ev.relatedTarget))) {
                    var elem_1 = _this.activeTooltip;
                    if (ComponentsHTMLHelper.isActiveFadeInOut(_this.CancelObject)) {
                        ComponentsHTMLHelper.cancelFadeInOut(_this.CancelObject);
                        _this.removeTooltips();
                    }
                    else {
                        ComponentsHTMLHelper.fadeOut(_this.activeTooltip, _this.fadeOutTime, function () {
                            if (elem_1 === _this.activeTooltip)
                                _this.removeTooltips();
                        }, _this.CancelObject);
                    }
                }
                return true;
            });
        }
        Tooltips.prototype.getNoTooltipSelectors = function (noTooltips) {
            var sel = [];
            var classes = noTooltips.split(" ");
            for (var _i = 0, classes_1 = classes; _i < classes_1.length; _i++) {
                var cls = classes_1[_i];
                var c = cls.trim();
                if (c.length > 0) {
                    sel.push(c);
                }
            }
            return sel;
        };
        Tooltips.prototype.buildNoTT = function (sel, noTooltips) {
            var s = "" + sel;
            for (var _i = 0, noTooltips_1 = noTooltips; _i < noTooltips_1.length; _i++) {
                var n = noTooltips_1[_i];
                s += ":not(." + n + ")";
            }
            return s;
        };
        Tooltips.prototype.showTooltip = function (elem, text) {
            if (this.activeTooltipElem) {
                if (elem === this.activeTooltipElem)
                    return;
                this.removeTooltips();
            }
            if (!$YetaWF.elementHas(document.body, elem)) {
                // the element requested is not part of the document body (any longer)
                this.removeTooltips();
                return;
            }
            var title = $YetaWF.getAttributeCond(elem, "title");
            if (title) {
                $YetaWF.setAttribute(elem, YConfigs.Basics.CssTooltip, title);
                elem.removeAttribute("title");
            }
            $YetaWF.elementAddClass(elem, this.TOOLTIPACTIVEELEMCLASS);
            this.activeTooltipElem = elem;
            var tooltip = document.createElement("div");
            tooltip.className = this.TOOLTIPCLASS;
            tooltip.appendChild(document.createTextNode(text));
            $YetaWF.setAttribute(tooltip, "role", "tooltip");
            var firstChild = document.body.firstChild;
            if (!firstChild)
                return;
            firstChild.parentElement.insertBefore(tooltip, firstChild);
            this.activeTooltip = tooltip;
            var winHeight = (window.innerHeight || document.documentElement.clientHeight);
            var winWidth = (window.innerWidth || document.documentElement.clientWidth);
            var winXOffset = window.pageXOffset;
            var winYOffset = window.pageYOffset;
            var elemRect = elem.getBoundingClientRect();
            var ttTop;
            if ($YetaWF.elementHasClass(elem, "y_ttvcenter"))
                ttTop = elemRect.top + elemRect.height / 2;
            else
                ttTop = elemRect.top + elemRect.height;
            var ttLeft;
            if ($YetaWF.elementHasClass(elem, "y_tthcenter"))
                ttLeft = elemRect.left + elemRect.width / 2;
            else
                ttLeft = elemRect.left + elemRect.width;
            // briefly show tooltip so we get the width & height
            tooltip.style.display = "block";
            var tooltipRect = tooltip.getBoundingClientRect();
            tooltip.style.display = "none";
            var ttWidth = tooltipRect.width;
            var ttHeight = tooltipRect.height;
            // check if it fits below
            if (elemRect.bottom + ttHeight <= winHeight) {
                // all is well, it fits below
            }
            else if (elemRect.top - ttHeight >= 0 || elemRect.top + elemRect.height < winHeight / 2) {
                // flip to top
                ttTop = elemRect.top - ttHeight;
            }
            else {
                // default to bottom - it just doesn't fit
            }
            // make it fit if it extends beyond right edge of window
            if (ttLeft + ttWidth + 35 > winWidth) {
                var diff = (ttLeft + ttWidth + 35) - winWidth;
                ttLeft -= diff;
            }
            tooltip.setAttribute("style", "top:" + (winYOffset + ttTop) + "px;left:" + (winXOffset + ttLeft) + "px;width:" + ttWidth + "px");
            if (ComponentsHTMLHelper.isActiveFadeInOut(this.CancelObject)) {
                ComponentsHTMLHelper.cancelFadeInOut(this.CancelObject);
                tooltip.style.display = "block";
                tooltip.style.opacity = "1";
            }
            else {
                ComponentsHTMLHelper.fadeIn(tooltip, this.fadeInTime, this.CancelObject);
            }
        };
        // API
        Tooltips.prototype.removeTooltips = function () {
            if (this.activeTooltip)
                this.activeTooltip.remove();
            if (this.activeTooltipElem)
                $YetaWF.elementRemoveClass(this.activeTooltipElem, this.TOOLTIPACTIVEELEMCLASS);
            this.activeTooltipElem = null;
            this.activeTooltip = null;
        };
        return Tooltips;
    }());
    YetaWF_ComponentsHTML.Tooltips = Tooltips;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
var ToolTipsHTMLHelper = new YetaWF_ComponentsHTML.Tooltips();

//# sourceMappingURL=Tooltips.js.map
