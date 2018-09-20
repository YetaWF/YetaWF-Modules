/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

var YetaWF_Identity_ListOfUserNames = {};

YetaWF_Identity_ListOfUserNames.init = function ($listAll, $userName) {
    $listAll.on('jqGridSelectRow', function (e, rowid) {
        var $grid = $(this);
        var name = $grid.jqGrid('getCell', rowid, 'RawUserName');
        $userName.val(name);
        YetaWF_Grid.setAddButtonStatus($userName);
    });
    YetaWF_Grid.setAddButtonStatus($userName);
}
