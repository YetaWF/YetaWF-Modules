/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

namespace YetaWF {
    export interface IVolatile {
        YetaWF_Search: YetaWF_Search.IPackageVolatiles;
    }
    export interface IConfigs {
        YetaWF_Search: YetaWF_Search.IPackageConfigs;
    }
}

namespace YetaWF_Search {

    export interface IPackageVolatiles {
        HighLight: boolean;
    }
    export interface IPackageConfigs {
        UrlArg: string;
    }

    export class Search {

        public static on: boolean = true;

        public static highlightSearch(): void {
            ($(".yModule") as any).removeHighlight();
            if (YVolatile.Basics.EditModeActive) return; // never in edit mode

            var offButton = $YetaWF.getElement1BySelectorCond(".YetaWF_Search_SearchControl a[data-name='Off']");
            if (!offButton || offButton.style.display === "none") return;

            var uri = $YetaWF.parseUrl(window.location.href);
            var kwdsString = uri.getSearch(YConfigs.YetaWF_Search.UrlArg);
            if (kwdsString.length === 0) return;
            var kwds = kwdsString.split(",");
            ($(".yPane .yModule") as any).highlight(kwds);
        }
        public static setButtons(): void {
            var onButton = $YetaWF.getElement1BySelectorCond(".YetaWF_Search_SearchControl a[data-name='On']");
            if (!onButton) return;
            var offButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='Off']");
            if (Search.on) {
                if ($YetaWF.parseUrl(window.location.href).hasSearch(YConfigs.YetaWF_Search.UrlArg)) {
                    if (YVolatile.YetaWF_Search && YVolatile.YetaWF_Search.HighLight) {
                        offButton.style.display = "";
                        onButton.style.display = "none";
                    } else {
                        offButton.style.display = "none";
                        onButton.style.display = "";
                    }
                    return;
                }
            }
            onButton.style.display = "none";
            offButton.style.display = "none";
        }
    }

    // Form postback - highlight new stuff
    if ($YetaWF.FormsAvailable()) {
        $YetaWF.Forms.addPostSubmitHandler(false/*!InPartialView*/, {
            form: null,
            callback: (entry: YetaWF.SubmitHandlerEntry):void => {
                Search.setButtons();
                Search.highlightSearch();
            },
            userdata: null
        });
    }
    // page or page content update - highlight new stuff
    $YetaWF.addWhenReady((tag: HTMLElement): void => {
        Search.setButtons();
        Search.highlightSearch();
    });
    // Handles events turning the addon on/off (used for dynamic content)
    $YetaWF.registerContentChange((addonGuid: string, on: boolean): void => {
        if (addonGuid === "f7202e79-30bc-43ea-8d7a-12218785207b") {
            Search.on = on;
        }
    });

    $YetaWF.registerEventHandlerBody("click", ".YetaWF_Search_SearchControl a[data-name='On']", (ev: MouseEvent): boolean => {

        var onButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='On']");
        var offButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='Off']");
        onButton.style.display = "none";
        offButton.style.display = "";
        Search.highlightSearch();
        YVolatile.YetaWF_Search.HighLight = true;
        $.ajax({
            "url": "/YetaWF_Search/SearchControlModule/Switch",
            "type": "post",
            "data": `Value=true&${YConfigs.Basics.ModuleGuid}=${encodeURIComponent($YetaWF.getModuleGuidFromTag(onButton))}`
        });
        return false;
    });
    $YetaWF.registerEventHandlerBody("click", ".YetaWF_Search_SearchControl a[data-name='Off']", (ev: MouseEvent): boolean => {

        var onButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='On']");
        var offButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='Off']");
        offButton.style.display = "none";
        onButton.style.display = "";

        ($(".yModule") as any).removeHighlight();
        YVolatile.YetaWF_Search.HighLight = false;
        $.ajax({
            "url": "/YetaWF_Search/SearchControlModule/Switch",
            "type": "post",
            "data": `Value=false&${YConfigs.Basics.ModuleGuid}=${encodeURIComponent($YetaWF.getModuleGuidFromTag(offButton))}`
        });
        return false;
    });
}