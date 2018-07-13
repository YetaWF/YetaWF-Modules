"use strict";
/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Licensing */
// If this javascript snippet is included, that means we're displaying the alert.
// The alert is displayed until dismissed or if the page doesn't reference this module (dynamic content).
var YetaWF_Basics_Mods;
(function (YetaWF_Basics_Mods) {
    var AlertDisplayModule = /** @class */ (function () {
        function AlertDisplayModule() {
        }
        /**
         * Initializes the module instance.
         */
        AlertDisplayModule.prototype.init = function () {
            document.addEventListener("click", this.handleClick);
            YetaWF_Basics.addWhenReady(function (section) {
                alert.initSection(section);
            });
            YetaWF_Basics.registerContentChange(function (event, addonGuid, on) {
                if (addonGuid === AlertDisplayModule.MODULEGUID) {
                    AlertDisplayModule.on = on;
                }
            });
        };
        /**
         * Initializes all alert elements in the specified tag.
         * @param tag - an element containing Alert elements.
         */
        AlertDisplayModule.prototype.initSection = function (tag) {
            var alert = document.querySelector(".YetaWF_Basics_AlertDisplay");
            if (!alert)
                throw ".YetaWF_Basics_AlertDisplay not found"; /*DEBUG*/
            if (!AlertDisplayModule.dismissed && AlertDisplayModule.on)
                alert.style.display = "";
            else
                alert.style.display = "none";
        };
        /**
         * Handles the click on the image to close the Alert.
         * @param event
         */
        AlertDisplayModule.prototype.handleClick = function (event) {
            if (!YetaWF_Basics.elementMatches(event.srcElement, ".YetaWF_Basics_AlertDisplay .t_close img"))
                return;
            AlertDisplayModule.dismissed = true;
            var alert = document.querySelector(".YetaWF_Basics_AlertDisplay");
            if (!alert)
                throw ".YetaWF_Basics_AlertDisplay not found"; /*DEBUG*/
            var close = alert.querySelector(".t_close");
            if (!close)
                throw "No .t_close element found"; /*DEBUG*/
            var ajaxurl = close.getAttribute("data-ajaxurl");
            if (!ajaxurl)
                throw "No ajax url specified"; /*DEBUG*/
            alert.style.display = "none";
            // Save alert status server side
            var request = new XMLHttpRequest();
            request.open("POST", ajaxurl, true);
            request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            request.send("");
            // we don't care about the result of this request
        };
        AlertDisplayModule.MODULEGUID = "24b7dc07-e96a-409d-911f-47bffd38d0fc";
        AlertDisplayModule.dismissed = false;
        AlertDisplayModule.on = true;
        return AlertDisplayModule;
    }());
    var alert = new AlertDisplayModule();
    alert.init();
})(YetaWF_Basics_Mods || (YetaWF_Basics_Mods = {}));

//# sourceMappingURL=AlertDisplay.js.map
