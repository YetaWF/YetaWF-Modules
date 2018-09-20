/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

var YetaWF_PageEdit_AllowedUsers = {};

YetaWF_PageEdit_AllowedUsers.init = function ($listAll, $userName) {
    $listAll.on('jqGridSelectRow', function (e, rowid) {
        var $grid = $(this);
        var name = $grid.jqGrid('getCell', rowid, 'RawUserName');
        $userName.val(name);
        YetaWF_Grid.setAddButtonStatus($userName);
    });
    YetaWF_Grid.setAddButtonStatus($userName);
}
