"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var Tooltips = /** @class */ (function () {
        function Tooltips() {
        }
        Tooltips.init = function () {
            var a1 = YVolatile.Basics.CssNoTooltips;
            var a2 = YConfigs.Basics.CssTooltip;
            var a3 = YConfigs.Basics.CssTooltipSpan;
            var selectors = "img:not(\"" + a1 + "\"),label,input:not(\".ui-button-disabled\"),a:not(\"" + a1 + ",.ui-button-disabled\"),i,.ui-jqgrid span[" + a2 + "],span[" + a3 + "],li[" + a2 + "],div[" + a2 + "]";
            var ddsel = ".k-list-container.k-popup li[data-offset-index]";
            $("body").tooltip({
                items: (selectors + "," + ddsel),
                content: function (a, b, c) {
                    var $this = $(this);
                    if ($this.is(ddsel)) {
                        // dropdown list - find who owns this and get the matching tooltip
                        // this is a bit hairy - we save all the tooltips for a dropdown list in a variable
                        // named ..id.._tooltips. The popup/dropdown is named ..id..-list so we deduce the
                        // variable name from the popup/dropdown. This is going to break at some point...
                        var ttindex = $this.attr("data-offset-index");
                        if (ttindex === undefined)
                            return null;
                        var $container = $this.closest(".k-list-container.k-popup");
                        if ($container.length !== 1)
                            return null;
                        var id = $container.attr("id");
                        if (!id)
                            return null;
                        id = id.replace("-list", "");
                        var tip = YetaWF_TemplateDropDownList.getTitleFromId(id, ttindex);
                        if (tip == null)
                            return null;
                        return $YetaWF.htmlEscape(tip);
                    }
                    for (;;) {
                        if (!$this.is(":hover") && $this.is(":focus"))
                            return null;
                        if ($this.attr("disabled") !== undefined)
                            return null;
                        var s = $this.attr(YConfigs.Basics.CssTooltip);
                        if (s)
                            return $YetaWF.htmlEscape(s);
                        s = $this.attr(YConfigs.Basics.CssTooltipSpan);
                        if (s)
                            return $YetaWF.htmlEscape(s);
                        s = $this.attr("title");
                        if (s !== undefined)
                            return $YetaWF.htmlEscape(s);
                        if ($this[0].tagName !== "IMG" && $this[0].tagName !== "I")
                            break;
                        // we're in an IMG or I tag, find enclosing A (if any) and try again
                        $this = $this.closest("a:not(\"" + YVolatile.Basics.CssNoTooltips + "\")");
                        if ($this.length === 0)
                            return null;
                        // if the a link is a menu, don't show a tooltip for the image because the tooltip would be in a bad location
                        if ($this.closest(".k-menu").length > 0)
                            return null;
                    }
                    if ($this[0].tagName === "A") {
                        var href = $this[0].href;
                        if (href === undefined || href.startsWith("javascript") || href.startsWith("#") || href.startsWith("mailto:"))
                            return null;
                        var target = $this[0].target;
                        if (target === "_blank") {
                            var uri = $YetaWF.parseUrl(href);
                            return $YetaWF.htmlEscape(YLocs.Basics.OpenNewWindowTT.format(uri.getHostName()));
                        }
                    }
                    return null;
                },
                position: { my: "left top", at: "right bottom", collision: "flipfit" }
            });
        };
        Tooltips.removeTooltips = function () {
            $(".ui-tooltip").remove();
        };
        return Tooltips;
    }());
    YetaWF_ComponentsHTML.Tooltips = Tooltips;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
$(document).ready(function () {
    YetaWF_ComponentsHTML.Tooltips.init();
});
$("body").on("mousedown", "a", function () {
    // when we click on an <a> link, we don't want the next tooltip
    // this may be a bug because after clicking an a link, the tooltip will be created (again?) so we want to suppress this
    // Repro steps (without hack): right click on an a link (that COULD have a tooltip) and open a new tab/window. On return to this page we'll get a tooltip
    YetaWF_ComponentsHTML.Tooltips.removeTooltips();
});

//# sourceMappingURL=Tooltips.js.map
