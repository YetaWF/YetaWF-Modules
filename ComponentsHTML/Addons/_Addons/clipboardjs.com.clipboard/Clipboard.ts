/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

declare var ClipboardJS: any;

namespace YetaWF_ComponentsHTML {

    export interface IPackageLocs {
        CopyToClip: string;
    }

    export class ClipboardSupport {

        public static clipText: any = null;
        public static clipTextArea: any = null;

        public static initAll(tag: HTMLElement):void {

            if (ClipboardSupport.clipText != null && ClipboardSupport.clipTextArea) return;

            ClipboardSupport.clipText = new ClipboardJS(".yt_text_copy", {
                target: (trigger: HTMLElement):Element|null => {
                    return trigger.previousElementSibling;
                },
            });
            ClipboardSupport.clipTextArea = new ClipboardJS(".yt_textareasourceonly_copy", {
                target: (trigger: HTMLElement): Element | null => {
                    return trigger.previousElementSibling;
                },
            });
            ClipboardSupport.clipText.on("success", (e: any): void => {
                $YetaWF.confirm(YLocs.YetaWF_ComponentsHTML.CopyToClip);
            });
            ClipboardSupport.clipTextArea.on("success", (e: any): void => {
                $YetaWF.confirm(YLocs.YetaWF_ComponentsHTML.CopyToClip);
            });
        }
    }

    /* handle copy icon */
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: CustomEvent<YetaWF.DetailsEventNavPageLoaded>): boolean => {
        for (let container of ev.detail.containers)
            ClipboardSupport.initAll(container);
        return true;
    });
}
