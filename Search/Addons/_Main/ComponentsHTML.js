"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var ComponentsHTML = /** @class */ (function () {
        function ComponentsHTML() {
            // PropertyListVisible
            // PropertyListVisible
            // PropertyListVisible
            this.PropertyListVisibleHandlers = [];
        }
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
