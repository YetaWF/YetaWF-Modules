/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Basics#License */

$(document).ready(function () {
    'use string';
    var $alert = $('.YetaWF_Basics_AlertDisplay');
    if ($alert.length == 0) return;
    $('.t_close img', $alert).on('click', function () {
        var ajaxurl = $('.t_close', $alert).attr('data-ajaxurl');
        if (ajaxurl == undefined) throw "No ajax url specified";/*DEBUG*/
        $alert.hide();
        $.ajax({
            url: ajaxurl,
            cache: false, type: 'POST',
            dataType: 'html',
        });
    });
});
