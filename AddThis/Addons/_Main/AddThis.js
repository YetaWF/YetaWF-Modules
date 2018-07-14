/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/AddThis#License */

// Handles events turning the addon on/off (used for dynamic content)
YetaWF_Basics.registerContentChange(function (addonGuid, on) {
    if (addonGuid == 'd790d324-ec41-419d-abba-fdb03652cd9d')
        $('div.addthis-smartlayers').toggle(on);
});