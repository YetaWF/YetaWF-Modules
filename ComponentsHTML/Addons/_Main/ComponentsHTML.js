"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
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
                YVolatile.YetaWF_ComponentsHTML.jqueryUI = true;
                $YetaWF.ContentHandling.loadAddons([
                    { AreaName: "YetaWF_ComponentsHTML", ShortName: "jqueryui-themes", Argument1: YVolatile.YetaWF_ComponentsHTML.jqueryUITheme }
                ], function () {
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
                YVolatile.YetaWF_ComponentsHTML.kendoUI = true;
                $YetaWF.ContentHandling.loadAddons([
                    { AreaName: "YetaWF_ComponentsHTML", ShortName: "telerik.com.Kendo_UI_Core", Argument1: YVolatile.YetaWF_ComponentsHTML.kendoUITheme }
                ], function () {
                    run();
                });
            }
            else {
                run();
            }
        };
        /**
         * Register a callback to be called when a propertylist becomes visible.
         */
        ComponentsHTML.prototype.registerPropertyListVisible = function (callback) {
            this.PropertyListVisibleHandlers.push({ callback: callback });
        };
        /**
         * Called to call all registered callbacks when a propertylist becomes visible.
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
        ComponentsHTML.prototype.cancelFadeInOut = function (cancelable) {
            cancelable.Canceled = true;
            cancelable.Active = false;
        };
        ComponentsHTML.prototype.isActiveFadeInOut = function (cancelable) {
            return cancelable.Active;
        };
        ComponentsHTML.prototype.clearFadeInOut = function (cancelable) {
            if (cancelable) {
                cancelable.Canceled = false;
                cancelable.Active = false;
            }
        };
        ComponentsHTML.prototype.fadeIn = function (elem, ms, cancelable) {
            var _this = this;
            elem.style.opacity = "0";
            if (cancelable) {
                cancelable.Canceled = false;
                cancelable.Active = true;
            }
            if (ms) {
                var opacity = 0;
                elem.style.display = "block";
                this.processPropertyListVisible(elem);
                var timer_1 = setInterval(function () {
                    if (cancelable && cancelable.Canceled) {
                        _this.clearFadeInOut(cancelable);
                        return;
                    }
                    opacity += 20 / ms;
                    if (opacity >= 1) {
                        clearInterval(timer_1);
                        opacity = 1;
                        _this.clearFadeInOut(cancelable);
                    }
                    elem.style.opacity = opacity.toString();
                }, 20);
            }
            else {
                elem.style.opacity = "1";
                this.clearFadeInOut(cancelable);
            }
        };
        ComponentsHTML.prototype.fadeOut = function (elem, ms, done, cancelable) {
            var _this = this;
            elem.style.opacity = "1";
            if (cancelable) {
                cancelable.Canceled = false;
                cancelable.Active = true;
            }
            if (ms) {
                var opacity = 1;
                var timer_2 = setInterval(function () {
                    if (cancelable && cancelable.Canceled) {
                        _this.clearFadeInOut(cancelable);
                        return;
                    }
                    opacity -= 20 / ms;
                    if (opacity <= 0) {
                        clearInterval(timer_2);
                        opacity = 0;
                        elem.style.display = "none";
                        _this.clearFadeInOut(cancelable);
                        _this.processPropertyListVisible(elem);
                        if (done)
                            done();
                    }
                    elem.style.opacity = opacity.toString();
                }, 20);
            }
            else {
                elem.style.opacity = "0";
                this.clearFadeInOut(cancelable);
                if (done)
                    done();
            }
        };
        return ComponentsHTML;
    }());
    YetaWF_ComponentsHTML.ComponentsHTML = ComponentsHTML;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
var ComponentsHTMLHelper = new YetaWF_ComponentsHTML.ComponentsHTML();

//# sourceMappingURL=ComponentsHTML.js.map
