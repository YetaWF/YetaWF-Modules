/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SyntaxHighlighter#License */

declare var hljs: any;

namespace YetaWF_SyntaxHighlighter {

    class HighlightJSModule {

        static readonly MODULEGUID: string = "25068AC6-BA74-4644-8B46-9D7FEC291E45";

        static on: boolean = true;

        public init(): void {

            $YetaWF.registerContentChange((addonGuid: string, on: boolean): void => {
                if (addonGuid === HighlightJSModule.MODULEGUID) {
                    HighlightJSModule.on = on;
                }
            });

            $YetaWF.addWhenReady((tag: HTMLElement) : void => {
                if (HighlightJSModule.on)
                    this.highlight(tag);
            });
        }

        private highlight(tag: HTMLElement) : void {
            var elems = $YetaWF.getElementsBySelector("pre code,pre", [tag]);
            for (var elem of elems) {
                hljs.highlightBlock(elem);
            }
        }

    }

    export var HighlightJS: HighlightJSModule = new HighlightJSModule();
}

