/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

var YetaWF_Pages_ListOfUserNames = {};

YetaWF_Pages_ListOfUserNames.init = function ($divid, $list, $listAll, $userName) {
    $listAll.on('jqGridSelectRow', function (e, rowid) {
        var $grid = $(this);
        var name = $grid.jqGrid('getCell', rowid, 'RawUserName');
        $userName.val(name);
        YetaWF_Grid.setAddButtonStatus($userName);
    });
    YetaWF_Grid.setAddButtonStatus($userName);
}
