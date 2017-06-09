/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

var _YetaWF_Basics_Alert = {};
_YetaWF_Basics_Alert.dismissed = false;
_YetaWF_Basics_Alert.on = true;

// if this javascript snippet is included, that means we're displaying the alert
// the alert is displayed until dismissed or if the page doesn't reference this module (dynamic content)

// handle close click
$('body').on('click', '.YetaWF_Basics_AlertDisplay .t_close img', function () {
    'use string';
    _YetaWF_Basics_Alert.dismissed = true;
    var $alert = $('.YetaWF_Basics_AlertDisplay');
    if ($alert.length == 0) throw ".YetaWF_Basics_AlertDisplay not found";/*DEBUG*/
    var ajaxurl = $('.t_close', $alert).attr('data-ajaxurl');
    if (ajaxurl == undefined) throw "No ajax url specified";/*DEBUG*/
    $alert.hide();
    $.ajax({
        url: ajaxurl,
        cache: false, type: 'POST',
        dataType: 'html',
    });
});

// Handles events turning the addon on/off (used for dynamic content)
$(document).on('YetaWF_Basics_Addon', function (event, addonGuid, on) {
    if (addonGuid == '24b7dc07-e96a-409d-911f-47bffd38d0fc') {
        _YetaWF_Basics_Alert.on = on;
    }
});
YetaWF_Basics.whenReady.push({
    callback: function ($tag) {
        var $alert = $('.YetaWF_Basics_AlertDisplay');
        if ($alert.length == 0) throw ".YetaWF_Basics_AlertDisplay not found";/*DEBUG*/
        $alert.toggle(!_YetaWF_Basics_Alert.dismissed && _YetaWF_Basics_Alert.on);
    }
});
