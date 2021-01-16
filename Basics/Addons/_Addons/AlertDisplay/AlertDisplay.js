"use strict";
/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */
// If this javascript snippet is included, that means we're displaying the alert.
// The alert is displayed until dismissed or if the page doesn't reference this module (dynamic content).
var YetaWF_Basics;
(function (YetaWF_Basics) {
    var AlertDisplayModule = /** @class */ (function () {
        function AlertDisplayModule() {
            $YetaWF.registerEventHandlerBody("click", ".YetaWF_Basics_AlertDisplay .t_close a", function (ev) {
                AlertDisplayModule.dismissed = true;
                var alert = $YetaWF.getElement1BySelector(".YetaWF_Basics_AlertDisplay");
                var close = $YetaWF.getElement1BySelector(".t_close", [alert]);
                var ajaxurl = $YetaWF.getAttribute(close, "data-ajaxurl");
                alert.style.display = "none";
                // Save alert status server side
                var request = new XMLHttpRequest();
                request.open("POST", ajaxurl, true);
                request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                request.send("");
                // we don't care about the result of this request
                return false;
            });
        }
        AlertDisplayModule.prototype.initAlert = function () {
            var alert = $YetaWF.getElement1BySelector(".YetaWF_Basics_AlertDisplay");
            if (!AlertDisplayModule.dismissed && AlertDisplayModule.on)
                alert.style.display = "";
            else
                alert.style.display = "none";
        };
        AlertDisplayModule.MODULEGUID = "24b7dc07-e96a-409d-911f-47bffd38d0fc";
        AlertDisplayModule.dismissed = false;
        AlertDisplayModule.on = true;
        return AlertDisplayModule;
    }());
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, function (ev) {
        AlertDisplay.initAlert();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, function (ev) {
        var addonGuid = ev.detail.addonGuid;
        var on = ev.detail.on;
        if (addonGuid === AlertDisplayModule.MODULEGUID) {
            AlertDisplayModule.on = on;
        }
        return true;
    });
    var AlertDisplay = new AlertDisplayModule();
})(YetaWF_Basics || (YetaWF_Basics = {}));

//# sourceMappingURL=AlertDisplay.js.map
