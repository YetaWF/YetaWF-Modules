"use strict";
/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */
var YetaWF_Search;
(function (YetaWF_Search) {
    var Search = /** @class */ (function () {
        function Search() {
        }
        Search.highlightSearch = function () {
            var mods = $YetaWF.getElementsBySelector(".yModule");
            Search.removeHighlight(mods);
            if (YVolatile.Basics.EditModeActive)
                return; // never in edit mode
            var offButton = $YetaWF.getElement1BySelectorCond(".YetaWF_Search_SearchControl a[data-name='Off']");
            if (!offButton || offButton.style.display === "none")
                return;
            var uri = $YetaWF.parseUrl(window.location.href);
            var kwdsString = uri.getSearch(YConfigs.YetaWF_Search.UrlArg);
            if (kwdsString.length === 0)
                return;
            var kwds = kwdsString.split(",");
            Search.highlight(mods, kwds, false);
        };
        Search.setButtons = function () {
            var onButton = $YetaWF.getElement1BySelectorCond(".YetaWF_Search_SearchControl a[data-name='On']");
            if (!onButton)
                return;
            var offButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='Off']");
            if (Search.on) {
                if ($YetaWF.parseUrl(window.location.href).hasSearch(YConfigs.YetaWF_Search.UrlArg)) {
                    if (YVolatile.YetaWF_Search && YVolatile.YetaWF_Search.HighLight) {
                        offButton.style.display = "";
                        onButton.style.display = "none";
                    }
                    else {
                        offButton.style.display = "none";
                        onButton.style.display = "";
                    }
                    return;
                }
            }
            onButton.style.display = "none";
            offButton.style.display = "none";
        };
        // highlighting code from http://johannburkard.de/blog/programming/javascript/highlight-javascript-text-higlighting-jquery-plugin.html
        // removed jquery dependency
        Search.highlight = function (elems, pat, ignore) {
            if (pat.length) {
                for (var _i = 0, elems_1 = elems; _i < elems_1.length; _i++) {
                    var elem = elems_1[_i];
                    Search.innerHighlight(elem, pat, ignore);
                }
            }
        };
        Search.removeHighlight = function (tags) {
            for (var _i = 0, tags_1 = tags; _i < tags_1.length; _i++) {
                var tag = tags_1[_i];
                var spans = $YetaWF.getElementsBySelector("span.highlight", [tag]);
                for (var _a = 0, spans_1 = spans; _a < spans_1.length; _a++) {
                    var span = spans_1[_a];
                    var parent_1 = span.parentNode;
                    var content = document.createTextNode(span.innerText);
                    parent_1.replaceChild(content, span);
                    parent_1.normalize();
                }
            }
        };
        Search.replaceDiacritics = function (str) {
            var diacritics = [
                [/[\u00c0-\u00c6]/g, "A"],
                [/[\u00e0-\u00e6]/g, "a"],
                [/[\u00c7]/g, "C"],
                [/[\u00e7]/g, "c"],
                [/[\u00c8-\u00cb]/g, "E"],
                [/[\u00e8-\u00eb]/g, "e"],
                [/[\u00cc-\u00cf]/g, "I"],
                [/[\u00ec-\u00ef]/g, "i"],
                [/[\u00d1|\u0147]/g, "N"],
                [/[\u00f1|\u0148]/g, "n"],
                [/[\u00d2-\u00d8|\u0150]/g, "O"],
                [/[\u00f2-\u00f8|\u0151]/g, "o"],
                [/[\u0160]/g, "S"],
                [/[\u0161]/g, "s"],
                [/[\u00d9-\u00dc]/g, "U"],
                [/[\u00f9-\u00fc]/g, "u"],
                [/[\u00dd]/g, "Y"],
                [/[\u00fd]/g, "y"]
            ];
            // eslint-disable-next-line @typescript-eslint/prefer-for-of
            for (var i = 0; i < diacritics.length; i++) {
                str = str.replace(diacritics[i][0], diacritics[i][1]);
            }
            return str;
        };
        Search.innerHighlight = function (node, pat, ignore) {
            var skip = 0;
            if (node.nodeType === 3) {
                var textNode = node;
                var patternCount = pat.length;
                for (var ii = 0; ii < patternCount; ii++) {
                    var currentTerm = (ignore ? this.replaceDiacritics(pat[ii]) : pat[ii]).toUpperCase();
                    var pos = (ignore ? this.replaceDiacritics(textNode.data) : textNode.data).toUpperCase().indexOf(currentTerm);
                    if (pos >= 0) {
                        var spannode = document.createElement("span");
                        spannode.className = "highlight";
                        var middlebit = textNode.splitText(pos);
                        /*let endbit =*/ middlebit.splitText(currentTerm.length);
                        var middleclone = middlebit.cloneNode(true);
                        spannode.appendChild(middleclone);
                        middlebit.parentNode.replaceChild(spannode, middlebit);
                        skip = 1;
                    }
                }
            }
            else if (node.nodeType === 1) {
                var elemNode = node;
                if (elemNode.childNodes && !/(script|style)/i.test(elemNode.tagName)) {
                    if (!$YetaWF.elementHasClass(elemNode, "yNoHighlight")) {
                        for (var i = 0; i < elemNode.childNodes.length; ++i) {
                            i += this.innerHighlight(elemNode.childNodes[i], pat, ignore);
                        }
                    }
                }
            }
            return skip;
        };
        Search.on = true;
        return Search;
    }());
    YetaWF_Search.Search = Search;
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        Search.setButtons();
        Search.highlightSearch();
        return true;
    });
    // Handles events turning the addon on/off (used for dynamic content)
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, function (ev) {
        var addonGuid = ev.detail.addonGuid;
        var on = ev.detail.on;
        if (addonGuid === "f7202e79-30bc-43ea-8d7a-12218785207b") {
            Search.on = on;
        }
        return true;
    });
    $YetaWF.registerEventHandlerBody("click", ".YetaWF_Search_SearchControl a[data-name='On']", function (ev) {
        var onButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='On']");
        var offButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='Off']");
        onButton.style.display = "none";
        offButton.style.display = "";
        Search.highlightSearch();
        YVolatile.YetaWF_Search.HighLight = true;
        var request = new XMLHttpRequest();
        request.open("POST", "/YetaWF_Search/SearchControlModule/Switch", true);
        request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
        request.send("Value=true&".concat(YConfigs.Basics.ModuleGuid, "=").concat(encodeURIComponent($YetaWF.getModuleGuidFromTag(onButton))));
        return false;
    });
    $YetaWF.registerEventHandlerBody("click", ".YetaWF_Search_SearchControl a[data-name='Off']", function (ev) {
        var onButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='On']");
        var offButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='Off']");
        offButton.style.display = "none";
        onButton.style.display = "";
        var mods = $YetaWF.getElementsBySelector(".yModule");
        Search.removeHighlight(mods);
        YVolatile.YetaWF_Search.HighLight = false;
        var request = new XMLHttpRequest();
        request.open("POST", "/YetaWF_Search/SearchControlModule/Switch", true);
        request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
        request.send("Value=false&".concat(YConfigs.Basics.ModuleGuid, "=").concat(encodeURIComponent($YetaWF.getModuleGuidFromTag(offButton))));
        return false;
    });
})(YetaWF_Search || (YetaWF_Search = {}));

//# sourceMappingURL=Search.js.map
