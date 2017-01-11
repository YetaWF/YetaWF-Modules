/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Search#License */

// SEARCH CONTROL - ENABLE DISABLE HIGHLIGHTING
// SEARCH CONTROL - ENABLE DISABLE HIGHLIGHTING
// SEARCH CONTROL - ENABLE DISABLE HIGHLIGHTING

$(document).ready(function () {
    function highlightSearch() {
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
                $('.yModule').removeHighlight();
                highlightSearch();
            },
            userdata: null,
        });
    }
});

// SEARCH RESULTS
// SEARCH RESULTS
// SEARCH RESULTS

$(document).ready(function () {

    // Add search terms to each url so we can highlight them when the page is displayed
    $('.YetaWF_Search_SearchResults').each(function () {
        // each search results module (there really should only be one)
        var $mod = $(this);
        var kwds = $('input[name="SearchTerms"]').val();
        $('.t_url a', $mod).each(function () {
            // update each url with the keywords
            var $this = $(this);
            var uri = $this.uri();
            uri.removeSearch(YConfigs.YetaWF_Search.UrlArg);
            uri.addSearch(YConfigs.YetaWF_Search.UrlArg, kwds);
        });
    });
});