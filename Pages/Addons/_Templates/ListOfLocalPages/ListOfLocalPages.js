/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

var YetaWF_Pages_ListOfLocalPages = {};

YetaWF_Pages_ListOfLocalPages.init = function ($divid, $list, $listAll, $url) {
    $listAll.on('jqGridSelectRow', function (e, rowid) {
        var $grid = $(this);
        var url = $grid.jqGrid('getCell', rowid, 'RawUrl');
        YetaWF_Url.Update($url, url);
        YetaWF_Grid.setAddButtonStatus(YetaWF_Url.RetrieveControl($url));
    });
    YetaWF_Grid.setAddButtonStatus(YetaWF_Url.RetrieveControl($url));
}
