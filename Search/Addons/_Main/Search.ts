/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

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

            let mods = $YetaWF.getElementsBySelector(".yModule");
            Search.removeHighlight(mods);

            if (YVolatile.Basics.EditModeActive) return; // never in edit mode

            let offButton = $YetaWF.getElement1BySelectorCond(".YetaWF_Search_SearchControl a[data-name='Off']");
            if (!offButton || offButton.style.display === "none") return;

            let uri = $YetaWF.parseUrl(window.location.href);
            let kwdsString = uri.getSearch(YConfigs.YetaWF_Search.UrlArg);
            if (kwdsString.length === 0) return;
            let kwds = kwdsString.split(",");

            Search.highlight(mods, kwds, false);
        }
        public static setButtons(): void {
            let onButton = $YetaWF.getElement1BySelectorCond(".YetaWF_Search_SearchControl a[data-name='On']");
            if (!onButton) return;
            let offButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='Off']");
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

        // highlighting code from http://johannburkard.de/blog/programming/javascript/highlight-javascript-text-higlighting-jquery-plugin.html
        // removed jquery dependency

        private static highlight(elems: HTMLElement[], pat: string[], ignore: boolean): void {
            if (pat.length) {
                for (let elem of elems) {
                    Search.innerHighlight(elem, pat, ignore);
                }
            }
        }
        public static removeHighlight(tags: HTMLElement[]): void {
            for (let tag of tags) {
                let spans = $YetaWF.getElementsBySelector("span.highlight", [tag]);
                for (let span of spans) {
                    let parent = span.parentNode!;
                    let content = document.createTextNode(span.innerText);
                    parent.replaceChild(content, span);
                    parent.normalize();
                }
            }
        }

        private static replaceDiacritics(str: string ): string {
            let diacritics = [
                [ /[\u00c0-\u00c6]/g, "A" ],
                [ /[\u00e0-\u00e6]/g, "a" ],
                [ /[\u00c7]/g, "C" ],
                [ /[\u00e7]/g, "c" ],
                [ /[\u00c8-\u00cb]/g, "E" ],
                [ /[\u00e8-\u00eb]/g, "e" ],
                [ /[\u00cc-\u00cf]/g, "I" ],
                [ /[\u00ec-\u00ef]/g, "i" ],
                [ /[\u00d1|\u0147]/g, "N" ],
                [ /[\u00f1|\u0148]/g, "n" ],
                [ /[\u00d2-\u00d8|\u0150]/g, "O" ],
                [ /[\u00f2-\u00f8|\u0151]/g, "o" ],
                [ /[\u0160]/g, "S" ],
                [ /[\u0161]/g, "s" ],
                [ /[\u00d9-\u00dc]/g, "U" ],
                [ /[\u00f9-\u00fc]/g, "u" ],
                [ /[\u00dd]/g, "Y" ],
                [ /[\u00fd]/g, "y" ]
            ];

            // eslint-disable-next-line @typescript-eslint/prefer-for-of
            for (let i = 0; i < diacritics.length; i++) {
                str = str.replace(diacritics[i][0], diacritics[i][1] as string);
            }
            return str;
        }
        private static innerHighlight(node: ChildNode, pat: string[], ignore: boolean): number {

            let skip = 0;
            if (node.nodeType === 3) {
                let textNode = node as Text;
                let patternCount = pat.length;
                for (let ii = 0; ii < patternCount; ii++) {
                    let currentTerm = (ignore ? this.replaceDiacritics(pat[ii]) : pat[ii]).toUpperCase();
                    let pos = (ignore ?  this.replaceDiacritics(textNode.data) : textNode.data).toUpperCase().indexOf(currentTerm);
                    if (pos >= 0) {
                        let spannode = document.createElement("span");
                        spannode.className = "highlight";
                        let middlebit = textNode.splitText(pos);
                        /*let endbit =*/ middlebit.splitText(currentTerm.length);
                        let middleclone = middlebit.cloneNode(true);
                        spannode.appendChild(middleclone);
                        middlebit.parentNode!.replaceChild(spannode, middlebit);
                        skip = 1;
                    }
                }
            } else if (node.nodeType === 1) {
                let elemNode = node as Element;
                if (elemNode.childNodes && !/(script|style)/i.test(elemNode.tagName)) {
                    if (!$YetaWF.elementHasClass(elemNode, "yNoHighlight")) {
                        for (let i = 0; i < elemNode.childNodes.length; ++i) {
                            i += this.innerHighlight(elemNode.childNodes[i], pat, ignore);
                        }
                    }
                }
            }
            return skip;
        }
    }

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: CustomEvent<YetaWF.DetailsEventNavPageLoaded>): boolean => {
        Search.setButtons();
        Search.highlightSearch();
        return true;
    });
    // Handles events turning the addon on/off (used for dynamic content)
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, (ev: CustomEvent<YetaWF.DetailsAddonChanged>): boolean => {
        let addonGuid = ev.detail.addonGuid;
        let on = ev.detail.on;
        if (addonGuid === "f7202e79-30bc-43ea-8d7a-12218785207b") {
            Search.on = on;
        }
        return true;
    });

    $YetaWF.registerEventHandlerBody("click", ".YetaWF_Search_SearchControl a[data-name='On']", (ev: MouseEvent): boolean => {

        let onButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='On']");
        let offButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='Off']");
        onButton.style.display = "none";
        offButton.style.display = "";
        Search.highlightSearch();

        YVolatile.YetaWF_Search.HighLight = true;

        let request: XMLHttpRequest = new XMLHttpRequest();
        request.open("POST", "/YetaWF_Search/SearchControlModule/Switch", true);
        request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
        request.send(`Value=true&${YConfigs.Basics.ModuleGuid}=${encodeURIComponent($YetaWF.getModuleGuidFromTag(onButton))}`);
        return false;
    });
    $YetaWF.registerEventHandlerBody("click", ".YetaWF_Search_SearchControl a[data-name='Off']", (ev: MouseEvent): boolean => {

        let onButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='On']");
        let offButton = $YetaWF.getElement1BySelector(".YetaWF_Search_SearchControl a[data-name='Off']");
        offButton.style.display = "none";
        onButton.style.display = "";

        let mods = $YetaWF.getElementsBySelector(".yModule");
        Search.removeHighlight(mods);

        YVolatile.YetaWF_Search.HighLight = false;

        let request: XMLHttpRequest = new XMLHttpRequest();
        request.open("POST", "/YetaWF_Search/SearchControlModule/Switch", true);
        request.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
        request.send(`Value=false&${YConfigs.Basics.ModuleGuid}=${encodeURIComponent($YetaWF.getModuleGuidFromTag(offButton))}`);
        return false;
    });
}