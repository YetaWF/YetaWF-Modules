/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

declare var ClipboardJS: any;

namespace YetaWF_ComponentsHTML {

    export interface IPackageLocs {
        CopyToClip: string;
    }

    export class ClipboardSupport {

        public static clip: any = null;

        public static initAll(tag: HTMLElement):void {

            if (ClipboardSupport.clip != null) return;
            var elems = $YetaWF.getElementsBySelector(".yt_text_copy", [tag]);
            if (elems.length === 0) return;

            ClipboardSupport.clip = new ClipboardJS(".yt_text_copy", {
                target: (trigger: HTMLElement):Element|null => {
                    return trigger.previousElementSibling;
                },
            });
            ClipboardSupport.clip.on("success", (e: any): void => {
                $YetaWF.confirm(YLocs.YetaWF_ComponentsHTML.CopyToClip);
            });
        }
    }

    /* handle copy icon */
    $YetaWF.addWhenReady(ClipboardSupport.initAll);
}
