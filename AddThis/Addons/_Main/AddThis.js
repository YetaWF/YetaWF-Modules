/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/AddThis#License */

// Handles events turning the addon on/off (used for dynamic content)
$(document).on('YetaWF_Basics_Addon', function (event, addonGuid, on) {
    if (addonGuid == 'd790d324-ec41-419d-abba-fdb03652cd9d')
        $('div.addthis-smartlayers').toggle(on);
});