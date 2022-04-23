"use strict";
/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */
var YetaWF_Pages;
(function (YetaWF_Pages) {
    var ScrollUp = /** @class */ (function () {
        function ScrollUp() {
        }
        ScrollUp.init = function () {
            var scrollText = ""; //RFFU
            var topDiv = $YetaWF.getElement1BySelectorCond("#".concat(ScrollUp.SCROLLTOPID));
            if (!topDiv) {
                topDiv = $YetaWF.createElement("a", { href: "#top", id: ScrollUp.SCROLLTOPID, title: scrollText });
                document.body.appendChild(topDiv);
                $YetaWF.registerEventHandler(topDiv, "click", null, function (ev) {
                    ScrollUp.scrollToTop();
                    return false;
                });
            }
            ScrollUp.evalScrollTop();
        };
        ScrollUp.destroy = function () {
            var topDiv = $YetaWF.getElement1BySelectorCond("#".concat(ScrollUp.SCROLLTOPID));
            topDiv === null || topDiv === void 0 ? void 0 : topDiv.remove();
        };
        ScrollUp.evalScrollTop = function () {
            if (!ScrollUp.On)
                return;
            var topDiv = $YetaWF.getElement1BySelectorCond("#".concat(ScrollUp.SCROLLTOPID));
            if (!topDiv)
                return;
            var rect = document.body.getBoundingClientRect();
            if (rect.y < -this.MinScrollDistance)
                topDiv.style.display = "";
            else
                topDiv.style.display = "none";
        };
        ScrollUp.scrollToTop = function () {
            var topDiv = $YetaWF.getElement1BySelectorCond("#".concat(ScrollUp.SCROLLTOPID));
            if (!topDiv)
                return;
            var rect = document.body.getBoundingClientRect();
            var top = -rect.y;
            var incr = top / (ScrollUp.SCROLLTIME / ScrollUp.SCROLLSTEPTIME);
            if (incr < ScrollUp.MININCREMENT)
                incr = ScrollUp.MININCREMENT;
            var interval = setInterval(function () {
                top -= incr;
                if (top <= 0) {
                    top = 0;
                    clearInterval(interval);
                }
                window.scroll(window.screenX, top);
            }, ScrollUp.SCROLLSTEPTIME);
        };
        ScrollUp.On = true;
        ScrollUp.MinScrollDistance = 300; // Distance from top before showing element (in pixels)
        ScrollUp.SCROLLTIME = 300; // time to scroll to top (in ms)
        ScrollUp.SCROLLSTEPTIME = 15; // time between incremental scrolling (in ms)
        ScrollUp.MININCREMENT = 100; // minimum scrolling distance during incremental scrolling
        ScrollUp.SCROLLTOPID = "YetaWF_Pages_ScrollTop";
        return ScrollUp;
    }());
    YetaWF_Pages.ScrollUp = ScrollUp;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, function (ev) {
        if (ev.detail.container === document.body)
            ScrollUp.evalScrollTop();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, function (ev) {
        if (ev.detail.container === document.body)
            ScrollUp.evalScrollTop();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        if (ScrollUp.On) {
            ScrollUp.init();
        }
        else {
            ScrollUp.destroy();
        }
        return true;
    });
    // Handles events turning the addon on/off (used for dynamic content)
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, function (ev) {
        var addonGuid = ev.detail.addonGuid;
        var on = ev.detail.on;
        if (addonGuid === "2a4e6f13-24a0-45c1-8a42-f1072e6ac7de") {
            ScrollUp.On = on;
        }
        return true;
    });
})(YetaWF_Pages || (YetaWF_Pages = {}));

//# sourceMappingURL=SkinScrollToTop.js.map
