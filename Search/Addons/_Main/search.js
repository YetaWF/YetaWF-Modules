/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

var YetaWF_Search = {};
var _YetaWF_Search = {
    on: true
};

// SEARCH CONTROL - ENABLE DISABLE HIGHLIGHTING
// SEARCH CONTROL - ENABLE DISABLE HIGHLIGHTING
// SEARCH CONTROL - ENABLE DISABLE HIGHLIGHTING

_YetaWF_Search.highlightSearch = function () {
    $('.yModule').removeHighlight();
    if (YVolatile.Basics.EditModeActive) return; // never in edit mode
    if ($(".YetaWF_Search_SearchControl a[data-name='Off']:visible").length == 0) return;
    var uri = YetaWF_Basics.parseUrl(window.location.href);
    var kwdsString = uri.getSearch[YConfigs.YetaWF_Search.UrlArg];
    if (kwdsString.length == 0) return;
    var kwds = kwdsString.split(',');
    $('.yPane .yModule').highlight(kwds);
};
$(document).on("click", ".YetaWF_Search_SearchControl a[data-name='On']", function () {
    $(".YetaWF_Search_SearchControl a[data-name='On']").toggle(false);
    $(".YetaWF_Search_SearchControl a[data-name='Off']").toggle(true);
    _YetaWF_Search.highlightSearch();
    YVolatile.YetaWF_Search.HighLight = true;
    $.ajax({
        'url': '/YetaWF_Search/SearchControlModule/Switch',
        'type': 'post',
        'data': 'Value=true&' + YConfigs.Basics.ModuleGuid + "=" + encodeURIComponent(YetaWF_Basics.getModuleGuidFromTag(this)),
    });
});
$(document).on("click", ".YetaWF_Search_SearchControl a[data-name='Off']", function () {
    $(".YetaWF_Search_SearchControl a[data-name='Off']").toggle(false);
    $(".YetaWF_Search_SearchControl a[data-name='On']").toggle(true);
    $('.yModule').removeHighlight();
    YVolatile.YetaWF_Search.HighLight = false;
    $.ajax({
        'url': '/YetaWF_Search/SearchControlModule/Switch',
        'type': 'post',
        'data': 'Value=false&' + YConfigs.Basics.ModuleGuid + "=" + encodeURIComponent(YetaWF_Basics.getModuleGuidFromTag(this)),
    });
});
_YetaWF_Search.setButtons = function() {
    if (_YetaWF_Search.on) {
        if (YetaWF_Basics.parseUrl(window.location.href).hasSearch(YConfigs.YetaWF_Search.UrlArg)) {
            if (YVolatile.YetaWF_Search.HighLight) {
                $(".YetaWF_Search_SearchControl a[data-name='Off']").show();
                $(".YetaWF_Search_SearchControl a[data-name='On']").hide();
            } else {
                $(".YetaWF_Search_SearchControl a[data-name='Off']").hide();
                $(".YetaWF_Search_SearchControl a[data-name='On']").show();
            }
            return;
        }
    }
    $(".YetaWF_Search_SearchControl a[data-name='Off']").hide();
    $(".YetaWF_Search_SearchControl a[data-name='On']").hide();
}

// Form postback - highlight new stuff
if (typeof YetaWF_Forms !== 'undefined' && YetaWF_Forms != undefined) {
    YetaWF_Forms.addPostSubmitHandler(0/*!InPartialView*/, {
        form: null,
        callback: function (entry) {
            _YetaWF_Search.setButtons();
            _YetaWF_Search.highlightSearch();
        },
        userdata: null,
    });
}

// page or page content update - highlight new stuff
YetaWF_Basics.addWhenReady(function (tag) {
    _YetaWF_Search.setButtons();
    _YetaWF_Search.highlightSearch();
});

// Handles events turning the addon on/off (used for dynamic content)
$(document).on('YetaWF_Basics_Addon', function (event, addonGuid, on) {
    if (addonGuid == 'f7202e79-30bc-43ea-8d7a-12218785207b') {
        _YetaWF_Search.on = on;
    }
});
