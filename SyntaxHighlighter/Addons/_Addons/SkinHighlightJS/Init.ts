/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

declare var hljs: any;

namespace YetaWF_SyntaxHighlighter {

    class HighlightJSModule {

        static readonly MODULEGUID: string = "25068ac6-ba74-4644-8b46-9d7fec291e45";

        static on: boolean = true;

        public static highlight(tag: HTMLElement) : void {
            if (HighlightJSModule.on) {
                var elems = $YetaWF.getElementsBySelector("pre code,pre", [tag]);
                for (var elem of elems) {
                    try {
                        hljs.highlightBlock(elem);
                    } catch (e) { }
                }
            }
        }
    }

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTADDONCHANGED, null, (ev: CustomEvent<YetaWF.DetailsAddonChanged>): boolean => {
        let addonGuid = ev.detail.addonGuid;
        let on = ev.detail.on;
        if (addonGuid === HighlightJSModule.MODULEGUID) {
            HighlightJSModule.on = on;
        }
        return true;
    });
    $YetaWF.addWhenReady((tag: HTMLElement): void => {
        HighlightJSModule.highlight(tag);
    });
}

