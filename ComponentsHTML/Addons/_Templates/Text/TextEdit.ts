/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

declare var ClipboardJS: any;

namespace YetaWF_ComponentsHTML {

    export interface IPackageLocs {
        CopyToClip: string;
    }

    export class TextEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static clip: any = null;

        public static initAll(tag: HTMLElement):void {

            if (TextEditComponent.clip != null) return;
            var elems = $YetaWF.getElementsBySelector(".yt_text_copy", [tag]);
            if (elems.length == 0) return;

            TextEditComponent.clip = new ClipboardJS(".yt_text_copy", {
                target: (trigger: HTMLElement) => {
                    return trigger.previousElementSibling;
                },
            });
            TextEditComponent.clip.on("success", (e: any): void => {
                $YetaWF.confirm(YLocs.YetaWF_ComponentsHTML.CopyToClip);
            });
        }

    }

    $YetaWF.addWhenReady(TextEditComponent.initAll);
}
