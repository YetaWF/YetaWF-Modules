/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

var YetaWF_ModuleEdit_AllowedUsers = {};

YetaWF_ModuleEdit_AllowedUsers.init = function ($listAll, $userName) {
    $listAll.on('jqGridSelectRow', function (e, rowid) {
        var $grid = $(this);
        var name = $grid.jqGrid('getCell', rowid, 'RawUserName');
        $userName.val(name);
        YetaWF_Grid.setAddButtonStatus($userName);
    });
    YetaWF_Grid.setAddButtonStatus($userName);
}
