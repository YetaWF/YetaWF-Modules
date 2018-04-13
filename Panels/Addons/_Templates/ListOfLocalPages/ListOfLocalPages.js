/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

var YetaWF_Panels_ListOfLocalPages = {};

YetaWF_Panels_ListOfLocalPages.init = function ($divid, $list, $listAll, $url) {
    $listAll.on('jqGridSelectRow', function (e, rowid) {
        var $grid = $(this);
        var url = $grid.jqGrid('getCell', rowid, 'RawUrl');
        YetaWF_Url.Update($url, url);
        YetaWF_Grid.setAddButtonStatus(YetaWF_Url.RetrieveControl($url));
    });
    YetaWF_Grid.setAddButtonStatus(YetaWF_Url.RetrieveControl($url));
}
