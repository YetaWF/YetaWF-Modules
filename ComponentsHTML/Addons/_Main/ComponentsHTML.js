"use strict";
/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var ComponentsHTML = /** @class */ (function () {
        function ComponentsHTML() {
            // Loader
            // Loader
            // Loader
            // PropertyListVisible
            // PropertyListVisible
            // PropertyListVisible
            this.PropertyListVisibleHandlers = [];
        }
        ComponentsHTML.prototype.MUSTHAVE_JQUERYUI = function () {
            if (!YVolatile.YetaWF_ComponentsHTML.jqueryUI)
                throw "jquery-ui is required but has not been loaded";
        };
        ComponentsHTML.prototype.REQUIRES_JQUERYUI = function (run) {
            if (!YVolatile.YetaWF_ComponentsHTML.jqueryUI) {
                // tslint:disable-next-line:no-debugger
                debugger;
                YVolatile.YetaWF_ComponentsHTML.jqueryUI = true;
                $YetaWF.ContentHandling.loadAddons([
                    { AreaName: "YetaWF_ComponentsHTML", ShortName: "jqueryui", Argument1: null },
                    { AreaName: "YetaWF_ComponentsHTML", ShortName: "jqueryui-themes", Argument1: YVolatile.YetaWF_ComponentsHTML.jqueryUITheme }
                ], function () {
                    console.log("Done");
                    run();
                });
            }
            else {
                run();
            }
        };
        ComponentsHTML.prototype.MUSTHAVE_KENDOUI = function () {
            if (!YVolatile.YetaWF_ComponentsHTML.kendoUI)
                throw "Kendo UI is required but has not been loaded";
        };
        ComponentsHTML.prototype.REQUIRES_KENDOUI = function (run) {
            if (!YVolatile.YetaWF_ComponentsHTML.kendoUI) {
                // tslint:disable-next-line:no-debugger
                debugger;
                YVolatile.YetaWF_ComponentsHTML.kendoUI = true;
                $YetaWF.ContentHandling.loadAddons([
                    { AreaName: "YetaWF_ComponentsHTML", ShortName: "telerik.com.Kendo_UI_Core", Argument1: YVolatile.YetaWF_ComponentsHTML.kendoUITheme }
                ], function () {
                    console.log("Done");
                    run();
                });
            }
            else {
                run();
            }
        };
        /**
         * Register a callback to be called when a propertylist become visible.
         */
        ComponentsHTML.prototype.registerPropertyListVisible = function (callback) {
            this.PropertyListVisibleHandlers.push({ callback: callback });
        };
        /**
         * Called to call all registered callbacks when a propertylist become visible.
         */
        ComponentsHTML.prototype.processPropertyListVisible = function (tag) {
            for (var _i = 0, _a = this.PropertyListVisibleHandlers; _i < _a.length; _i++) {
                var entry = _a[_i];
                entry.callback(tag);
            }
        };
        // Fade in/out
        // Fade in/out
        // Fade in/out
        ComponentsHTML.prototype.fadeIn = function (elem, ms) {
            elem.style.opacity = "0";
            if (ms) {
                var opacity = 0;
                elem.style.display = "block";
                this.processPropertyListVisible(elem);
                var timer_1 = setInterval(function () {
                    opacity += 50 / ms;
                    if (opacity >= 1) {
                        clearInterval(timer_1);
                        opacity = 1;
                    }
                    elem.style.opacity = opacity.toString();
                }, 50);
            }
            else {
                elem.style.opacity = "1";
            }
        };
        ComponentsHTML.prototype.fadeOut = function (elem, ms) {
            var _this = this;
            elem.style.opacity = "1";
            if (ms) {
                var opacity = 1;
                var timer_2 = setInterval(function () {
                    opacity -= 50 / ms;
                    if (opacity <= 0) {
                        clearInterval(timer_2);
                        opacity = 0;
                        elem.style.display = "none";
                        _this.processPropertyListVisible(elem);
                    }
                    elem.style.opacity = opacity.toString();
                }, 50);
            }
            else {
                elem.style.opacity = "0";
            }
        };
        return ComponentsHTML;
    }());
    YetaWF_ComponentsHTML.ComponentsHTML = ComponentsHTML;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
var ComponentsHTMLHelper = new YetaWF_ComponentsHTML.ComponentsHTML();

//# sourceMappingURL=ComponentsHTML.js.map
