/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/PageEdit#License */

/* Page Control */

$(document).ready(function () {

    var fadeTime = 250;

    var $mod = $('#' + YConfigs.YetaWF_PageEdit.PageControlMod);

    // Page icon
    var $pagebutton = $('#yPageControlButton');
    $($pagebutton).on("click", function () {
        if ($mod.is(':visible')) {
            $mod.fadeOut(fadeTime);
            YVolatile.Basics.PageControlVisible = false;
        } else {
            $mod.fadeIn(fadeTime);
            YVolatile.Basics.PageControlVisible = true;
        }
    });

    if (YVolatile.Basics.PageControlVisible)
        $mod.show();

});

