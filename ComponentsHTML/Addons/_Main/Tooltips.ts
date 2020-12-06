/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    /**
     * Implements a stand-alone tooltip so we don't need jqueryui/bootstrap
     */
    export class Tooltips {

        private readonly TOOLTIPCLASS: string = "yTooltip";
        private readonly TOOLTIPACTIVEELEMCLASS: string = "yTooltipActive";
        private readonly fadeInTime: number = 200;
        private readonly fadeOutTime: number = 200;

        private activeTooltipElem: HTMLElement | null = null;
        private activeTooltip: HTMLElement | null = null;

        private CancelObject: CancelableFadeInOut = { Canceled: false, Active: false };

        public constructor() {

            const noTooltips = this.getNoTooltipSelectors(YVolatile.Basics.CssNoTooltips);
            const noTTImgSel = this.buildNoTT("img", noTooltips);
            const noTTASel = this.buildNoTT("a", noTooltips);
            const noTTISel = this.buildNoTT("i", noTooltips);
            const a2 = YConfigs.Basics.CssTooltip;
            const a3 = YConfigs.Basics.CssTooltipSpan;
            const noTTMisc = `.ui-jqgrid span[${a2}],th[${a2}],span[${a3}],li[${a2}],div[${a2}]`;

            const selectors = `label,input:not(.ui-button-disabled),a:not(.ui-button-disabled),button:not(.ui-button-disabled),${noTTImgSel},${noTTASel},${noTTISel},${noTTMisc}`;

            $YetaWF.registerMultipleEventHandlersBody(["mouseover", "click"], `${selectors}`, (ev: Event): boolean => {

                var elem: HTMLElement | null = ev.__YetaWFElem;

                for (; ;) {
                    if (!elem)
                        return true;
                    if (ev.type !== "click" && elem === this.activeTooltipElem)
                        return true;
                    if (!$YetaWF.elementMatches(elem, ":hover") && $YetaWF.elementMatches(elem, ":focus"))
                        return true;
                    if ($YetaWF.getAttributeCond(elem, "disabled"))
                        return true;
                    var s = $YetaWF.getAttributeCond(elem, YConfigs.Basics.CssTooltip) || $YetaWF.getAttributeCond(elem, YConfigs.Basics.CssTooltipSpan) || $YetaWF.getAttributeCond(elem, "title");
                    if (s) {
                        this.showTooltip(elem, s);
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
                    const anchor = elem as HTMLAnchorElement;
                    const href = anchor.href;
                    if (href === undefined || href.startsWith("javascript") || href.startsWith("#") || href.startsWith("mailto:"))
                        return true;
                    if (anchor.target === "_blank") {
                        const uri = $YetaWF.parseUrl(href);
                        this.showTooltip(elem, YLocs.Basics.OpenNewWindowTT.format(uri.getHostName()));
                        return true;
                    }
                }
                return true;
            });
            $YetaWF.registerEventHandlerBody("mousedown", null, (ev: Event): boolean => {
                this.removeTooltips();
                return true;
            });
            $YetaWF.registerEventHandlerBody("mouseout", `.${this.TOOLTIPACTIVEELEMCLASS}`, (ev: MouseEvent): boolean => {
                if (this.activeTooltip && this.activeTooltipElem && (ev.__YetaWFElem === this.activeTooltipElem && !this.activeTooltipElem.contains(ev.relatedTarget as HTMLElement))) {
                    let elem = this.activeTooltip;
                    if (ComponentsHTMLHelper.isActiveFadeInOut(this.CancelObject)) {
                        ComponentsHTMLHelper.cancelFadeInOut(this.CancelObject);
                        this.removeTooltips();
                    } else {
                        ComponentsHTMLHelper.fadeOut(this.activeTooltip, this.fadeOutTime, () : void => {
                            if (elem === this.activeTooltip)
                                this.removeTooltips();
                        }, this.CancelObject);
                    }
                }
                return true;
            });
        }
        private getNoTooltipSelectors(noTooltips: string): string[] {
            var sel: string[] = [];
            var classes = noTooltips.split(" ");
            for (const cls of classes) {
                let c = cls.trim();
                if (c.length > 0) {
                    sel.push(c);
                }
            }
            return sel;
        }
        private buildNoTT(sel: string, noTooltips: string[]): string {
            var s = "";
            for (const n of noTooltips) {
                if (s.length)
                    s += ",";
                s += `${sel}:not(${n})`;
            }
            return s;
        }

        private showTooltip(elem: HTMLElement, text: string): void {

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

            let title = $YetaWF.getAttributeCond(elem, "title");
            if (title) {
                $YetaWF.setAttribute(elem, YConfigs.Basics.CssTooltip, title);
                elem.removeAttribute("title");
            }

            $YetaWF.elementAddClass(elem, this.TOOLTIPACTIVEELEMCLASS);
            this.activeTooltipElem = elem;

            let tooltip = document.createElement("div");
            tooltip.className = this.TOOLTIPCLASS;
            tooltip.appendChild(document.createTextNode(text));
            $YetaWF.setAttribute(tooltip, "role", "tooltip");

            let firstChild = document.body.firstChild;
            if (!firstChild) return;
            firstChild.parentElement!.insertBefore(tooltip, firstChild);

            this.activeTooltip = tooltip;

            let winHeight = (window.innerHeight || document!.documentElement!.clientHeight);
            let winWidth = (window.innerWidth || document!.documentElement!.clientWidth);
            let winXOffset = window.pageXOffset;
            let winYOffset = window.pageYOffset;

            let elemRect = elem.getBoundingClientRect();
            let ttTop;
            if ($YetaWF.elementHasClass(elem, "y_ttvcenter"))
                ttTop = elemRect.top + elemRect.height/2;
            else
                ttTop = elemRect.top + elemRect.height;
            let ttLeft;
            if ($YetaWF.elementHasClass(elem, "y_tthcenter"))
                ttLeft = elemRect.left + elemRect.width/2;
            else
                ttLeft = elemRect.left + elemRect.width;

            // briefly show tooltip so we get the width & height
            tooltip.style.display = "block";
            let tooltipRect = tooltip.getBoundingClientRect();
            tooltip.style.display = "none";
            let ttWidth = tooltipRect.width;
            let ttHeight = tooltipRect.height;

            // check if it fits below
            if (elemRect.bottom + ttHeight <= winHeight) {
                // all is well, it fits below
            } else if (elemRect.top - ttHeight >= 0 || elemRect.top + elemRect.height < winHeight / 2) {
                // flip to top
                ttTop = elemRect.top - ttHeight;
            } else {
                // default to bottom - it just doesn't fit
            }
            // make it fit if it extends beyond right edge of window
            if (ttLeft + ttWidth + 35 > winWidth) {
                let diff = (ttLeft + ttWidth + 35) - winWidth;
                ttLeft -= diff;
            }

            tooltip.setAttribute("style", `top:${winYOffset + ttTop}px;left:${winXOffset + ttLeft}px;width:${ttWidth}px`);
            if (ComponentsHTMLHelper.isActiveFadeInOut(this.CancelObject)) {
                ComponentsHTMLHelper.cancelFadeInOut(this.CancelObject);
                tooltip.style.display = "block";
                tooltip.style.opacity = "1";
            } else {
                ComponentsHTMLHelper.fadeIn(tooltip, this.fadeInTime, this.CancelObject);
            }
        }

        // API

        public removeTooltips(): void {
            if (this.activeTooltip)
                this.activeTooltip.remove();
            if (this.activeTooltipElem)
                $YetaWF.elementRemoveClass(this.activeTooltipElem, this.TOOLTIPACTIVEELEMCLASS);
            this.activeTooltipElem = null;
            this.activeTooltip = null;
        }
    }
}

var ToolTipsHTMLHelper = new YetaWF_ComponentsHTML.Tooltips();
