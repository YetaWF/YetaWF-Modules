"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */
var YetaWF_ComponentsHTML;
(function (YetaWF_ComponentsHTML) {
    var ComponentsHTML = /** @class */ (function () {
        function ComponentsHTML() {
        }
        /**
         * Translate a jqury event to a native event.
         * Native events are normally not needed if all events are handled within ComponentsHTML, but there are a few exceptions,
         * like submit/apply which are handled by the framework and must generate native events.
         * The same event cannot be handled using jquery also, as it will fire 2x because of the native translation.
         * If it is necessary to handle the event using jquery also, use ev.__YetaWF to determine whether it's a native event.
         */
        ComponentsHTML.prototype.jQueryToNativeEvent = function ($elem, eventName) {
            $elem.on(eventName, function (ev) {
                if (!ev.originalEvent || !ev.originalEvent.__YetaWF) {
                    var nev = new Event(eventName, { bubbles: true, cancelable: true });
                    nev.__YetaWF = true; // to avaid handling it again
                    $elem.each(function (index, elem) {
                        elem.dispatchEvent(nev);
                    });
                }
            });
        };
        return ComponentsHTML;
    }());
    YetaWF_ComponentsHTML.ComponentsHTML = ComponentsHTML;
})(YetaWF_ComponentsHTML || (YetaWF_ComponentsHTML = {}));
var ComponentsHTML = new YetaWF_ComponentsHTML.ComponentsHTML();

//# sourceMappingURL=ComponentsHTML.js.map
