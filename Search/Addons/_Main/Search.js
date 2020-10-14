"use strict";
/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */
var YetaWF_Search;
(function (YetaWF_Search) {
    var Search = /** @class */ (function () {
        function Search() {
        }
        Search.highlightSearch = function () {
            $(".yModule").removeHighlight();
            if (YVolatile.Basics.EditModeActive)
                return; // never in edit mode
            var offButton = $YetaWF.getElement1BySelectorCond(".YetaWF_Search_SearchControl a[data-name='Off']");
            if (!offButton || offButton.style.display === "none")
                return;
            var uri = $YetaWF.parseUrl(window.location.href);
            var kwdsString = uri.getSearch(YConfigs.YetaWF_Search.UrlArg);
            if (kwdsString.length === 0)
                return;
            var kwds = kwdsString.split(",");
            $(".yPane .yModule").highlight(kwds);
        };
        Search.setButtons = function () {
            var onButton = $YetaWF.getElement1BySelectorCond(".YetaWF_Search_SearchControl a[data-name='On']");
            if (!onButton)
                return;
            var offButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='Off']");
            if (Search.on) {
                if ($YetaWF.parseUrl(window.location.href).hasSearch(YConfigs.YetaWF_Search.UrlArg)) {
                    if (YVolatile.YetaWF_Search && YVolatile.YetaWF_Search.HighLight) {
                        offButton.style.display = "";
                        onButton.style.display = "none";
                    }
                    else {
                        offButton.style.display = "none";
                        onButton.style.display = "";
                    }
                    return;
                }
            }
            onButton.style.display = "none";
            offButton.style.display = "none";
        };
        Search.on = true;
        return Search;
    }());
    YetaWF_Search.Search = Search;
    // Form postback - highlight new stuff
    if ($YetaWF.FormsAvailable()) {
        $YetaWF.Forms.addPostSubmitHandler(false /*!InPartialView*/, {
            form: null,
            callback: function (entry) {
                Search.setButtons();
                Search.highlightSearch();
            },
            userdata: null
        });
    }
    // page or page content update - highlight new stuff
    $YetaWF.addWhenReady(function (tag) {
        Search.setButtons();
        Search.highlightSearch();
    });
    // Handles events turning the addon on/off (used for dynamic content)
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, function (ev) {
        var addonGuid = ev.detail.addonGuid;
        var on = ev.detail.on;
        if (addonGuid === "f7202e79-30bc-43ea-8d7a-12218785207b") {
            Search.on = on;
        }
        return true;
    });
    $YetaWF.registerEventHandlerBody("click", ".YetaWF_Search_SearchControl a[data-name='On']", function (ev) {
        var onButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='On']");
        var offButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='Off']");
        onButton.style.display = "none";
        offButton.style.display = "";
        Search.highlightSearch();
        YVolatile.YetaWF_Search.HighLight = true;
        $.ajax({
            "url": "/YetaWF_Search/SearchControlModule/Switch",
            "type": "post",
            "data": "Value=true&" + YConfigs.Basics.ModuleGuid + "=" + encodeURIComponent($YetaWF.getModuleGuidFromTag(onButton))
        });
        return false;
    });
    $YetaWF.registerEventHandlerBody("click", ".YetaWF_Search_SearchControl a[data-name='Off']", function (ev) {
        var onButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='On']");
        var offButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='Off']");
        offButton.style.display = "none";
        onButton.style.display = "";
        $(".yModule").removeHighlight();
        YVolatile.YetaWF_Search.HighLight = false;
        $.ajax({
            "url": "/YetaWF_Search/SearchControlModule/Switch",
            "type": "post",
            "data": "Value=false&" + YConfigs.Basics.ModuleGuid + "=" + encodeURIComponent($YetaWF.getModuleGuidFromTag(offButton))
        });
        return false;
    });
})(YetaWF_Search || (YetaWF_Search = {}));

//# sourceMappingURL=Search.js.map
