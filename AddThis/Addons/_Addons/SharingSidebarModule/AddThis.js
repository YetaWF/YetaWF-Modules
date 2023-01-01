"use strict";
/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/AddThis#License */
// Handles events turning the addon on/off (used for dynamic content)
$YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, function (ev) {
    var addonGuid = ev.detail.addonGuid;
    var on = ev.detail.on;
    if (addonGuid === "d790d324-ec41-419d-abba-fdb03652cd9d") {
        var addThis = $YetaWF.getElement1BySelector("div.addthis-smartlayers");
        if (on)
            addThis.style.display = "block";
        else
            addThis.style.display = "none";
    }
    return true;
});

//# sourceMappingURL=AddThis.js.map
