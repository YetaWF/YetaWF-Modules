"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */
var YetaWF_SyntaxHighlighter;
(function (YetaWF_SyntaxHighlighter) {
    var HighlightJSModule = /** @class */ (function () {
        function HighlightJSModule() {
        }
        HighlightJSModule.highlight = function (tag) {
            if (HighlightJSModule.on) {
                var elems = $YetaWF.getElementsBySelector("pre code,pre", [tag]);
                for (var _i = 0, elems_1 = elems; _i < elems_1.length; _i++) {
                    var elem = elems_1[_i];
                    try {
                        hljs.highlightBlock(elem);
                    }
                    catch (e) { }
                }
            }
        };
        HighlightJSModule.MODULEGUID = "25068ac6-ba74-4644-8b46-9d7fec291e45";
        HighlightJSModule.on = true;
        return HighlightJSModule;
    }());
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        for (var _i = 0, _a = ev.detail.containers; _i < _a.length; _i++) {
            var container = _a[_i];
            HighlightJSModule.highlight(container);
        }
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, function (ev) {
        var addonGuid = ev.detail.addonGuid;
        var on = ev.detail.on;
        if (addonGuid === HighlightJSModule.MODULEGUID) {
            HighlightJSModule.on = on;
        }
        return true;
    });
})(YetaWF_SyntaxHighlighter || (YetaWF_SyntaxHighlighter = {}));

//# sourceMappingURL=Init.js.map
