"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */
var YetaWF_SyntaxHighlighter;
(function (YetaWF_SyntaxHighlighter) {
    var HighlightJSModule = /** @class */ (function () {
        function HighlightJSModule() {
        }
        HighlightJSModule.prototype.init = function () {
            var _this = this;
            $YetaWF.registerContentChange(function (addonGuid, on) {
                if (addonGuid === HighlightJSModule.MODULEGUID) {
                    HighlightJSModule.on = on;
                }
            });
            $YetaWF.addWhenReady(function (tag) {
                if (HighlightJSModule.on)
                    _this.highlight(tag);
            });
        };
        HighlightJSModule.prototype.highlight = function (tag) {
            var elems = $YetaWF.getElementsBySelector("pre code,pre", [tag]);
            for (var _i = 0, elems_1 = elems; _i < elems_1.length; _i++) {
                var elem = elems_1[_i];
                hljs.highlightBlock(elem);
            }
        };
        HighlightJSModule.MODULEGUID = "25068AC6-BA74-4644-8B46-9D7FEC291E45";
        HighlightJSModule.on = true;
        return HighlightJSModule;
    }());
    YetaWF_SyntaxHighlighter.HighlightJS = new HighlightJSModule();
})(YetaWF_SyntaxHighlighter || (YetaWF_SyntaxHighlighter = {}));
