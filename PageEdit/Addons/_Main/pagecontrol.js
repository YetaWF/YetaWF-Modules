/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

/* Page Control */

$(document).ready(function () {
    'use strict';

    var fadeTime = 250;

    var $mod = $('#' + YConfigs.YetaWF_PageEdit.PageControlMod);

    // Page icon
    var $pagebutton = $('#yPageControlButton');
    $pagebutton.on("click", function () {
        if ($mod.is(':visible')) {
            $mod.fadeOut(fadeTime);
            YVolatile.Basics.PageControlVisible = false;
        } else {
            $mod.fadeIn(fadeTime);
            $('body').trigger('YetaWF_PropertyList_Visible', $mod);
            YVolatile.Basics.PageControlVisible = true;
        }
    });
    // on page load, show control panel if wanted
    if (YVolatile.Basics.PageControlVisible) {
        $mod.show();
        $('body').trigger('YetaWF_PropertyList_Visible', $mod);
    }
});

// handle Page Settings, Remove Current Page, W3C Validation - this is needed in case we're in a unified page set
// in which case the original pageguid and url in the module actions have changed
// when a new page becomes active, update the module actions reflecting the new page/url
// also update all hidden fields with the new current page guid
YetaWF_Basics.whenReady.push({
    callback: function ($tag) {
        // disregards $tag as this callback is used when a page or page content has been rendered
        'use strict';

        // Page Settings
        if (!Y_InPopup()) {
            var $ps = $('.YetaWF_PageEdit_PageControl a[data-name="PageSettings"]');
            if ($ps.length > 0) {
                $ps.each(function () {
                    var uri = new URI(this.href);
                    uri.removeSearch('PageGuid');
                    uri.addSearch('PageGuid', YVolatile.Basics.PageGuid);
                    $(this).attr("href", uri.toString());
                });
            }
            // Remove Page
            var $rp = $('.YetaWF_PageEdit_PageControl a[data-name="RemovePage"]');
            if ($rp.length > 0) {
                $rp.each(function () {
                    var uri = new URI(this.href);
                    uri.removeSearch('PageGuid');
                    uri.addSearch('PageGuid', YVolatile.Basics.PageGuid);
                    $(this).attr("href", uri.toString());
                });
            }
            // W3C validation
            var $w3c = $('.YetaWF_PageEdit_PageControl a[data-name="W3CValidate"]');
            if ($w3c.length > 0)
                $w3c.each(function () {
                    $(this).attr("href", YConfigs.YetaWF_PageEdit.W3CUrl.format(window.location));
                });

            // all hidden fields
            $('.YetaWF_PageEdit_PageControl input[name="CurrentPageGuid"][type="hidden"]').val(YVolatile.Basics.PageGuid);
        }
    }
});


