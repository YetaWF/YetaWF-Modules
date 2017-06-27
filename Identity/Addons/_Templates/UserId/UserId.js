/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

var YetaWF_Identity_UserId = {};

YetaWF_Identity_UserId.init = function ($div, $hidden, $name, $list) {
    $list.on('jqGridSelectRow', function (e, rowid) {
        var $grid = $(this);
        var userid = $grid.jqGrid('getCell', rowid, 'RawUserId');
        $hidden.val(userid);
        var name = $grid.jqGrid('getCell', rowid, 'RawUserName');
        $name.val(name);
    });
    $('.t_clear', $div).on('click', function () {
        $hidden.val(0);
        $name.val($name.attr('data-nouser'));
    });
}
