/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

var YetaWF_Search = {};

// SEARCH CONTROL - ENABLE DISABLE HIGHLIGHTING
// SEARCH CONTROL - ENABLE DISABLE HIGHLIGHTING
// SEARCH CONTROL - ENABLE DISABLE HIGHLIGHTING

$(document).ready(function () {
    function highlightSearch() {
        $('.yModule').removeHighlight();
        if (YVolatile.Basics.EditModeActive) return; // never in edit mode
        var kwdsString = $(".YetaWF_Search_SearchControl input[name='SearchTerms']").val();
        var kwds = eval(kwdsString);
        $('.yPane .yModule').highlight(kwds);
    }
    $("body").on("click", ".YetaWF_Search_SearchControl a[data-name='On']", function () {
        $(".YetaWF_Search_SearchControl a[data-name='On']").toggle(false);
        $(".YetaWF_Search_SearchControl a[data-name='Off']").toggle(true);
        highlightSearch();
    });
    $("body").on("click", ".YetaWF_Search_SearchControl a[data-name='Off']", function () {
        $(".YetaWF_Search_SearchControl a[data-name='Off']").toggle(false);
        $(".YetaWF_Search_SearchControl a[data-name='On']").toggle(true);
        $('.yModule').removeHighlight();
    });
    if ($(".YetaWF_Search_SearchControl a[data-name='Off']:visible").length > 0) {
        highlightSearch();
    }
    if (typeof YetaWF_Forms !== 'undefined' && YetaWF_Forms != undefined) {
        YetaWF_Forms.addPostSubmitHandler(0/*!InPartialView*/, {
            form: null,
            callback: function (entry) {
                highlightSearch();
            },
            userdata: null,
        });
    }
});

// SEARCH RESULTS
// SEARCH RESULTS
// SEARCH RESULTS

YetaWF_Search.initResults = function($tag) {
    // Add search terms to each url so we can highlight them when the page is displayed
    $('.YetaWF_Search_SearchResults', $tag).each(function () {
        // each search results module (there really should only be one)
        var $mod = $(this);
        var kwds = $('.YetaWF_Search_SearchControl input[name="SearchTerms"]').val();
        $('.t_desc a', $mod).each(function () {
            // update each url with the keywords
            var $this = $(this);
            var uri = $this.uri();
            uri.removeSearch(YConfigs.YetaWF_Search.UrlArg);
            uri.addSearch(YConfigs.YetaWF_Search.UrlArg, kwds);
        });
    });
};

YetaWF_Basics.whenReady.push({
    callback: YetaWF_Search.initResults
});