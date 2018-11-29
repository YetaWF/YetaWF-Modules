/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

declare var ClipboardJS: any;

namespace YetaWF_ComponentsHTML {

    export interface IPackageLocs {
        CopyToClip: string;
    }

    export class TextAreaSourceOnlyComponent {

        public static clip: any = null;

        public static initAll(tag: HTMLElement): void {

            if (TextAreaSourceOnlyComponent.clip != null) return;
            var elems = $YetaWF.getElementsBySelector(".yt_textareasourceonly_copy", [tag]);
            if (elems.length === 0) return;

            TextAreaSourceOnlyComponent.clip = new ClipboardJS(".yt_textareasourceonly_copy", {
                target: (trigger: HTMLElement): Element | null => {
                    return trigger.previousElementSibling;
                },
            });
            TextAreaSourceOnlyComponent.clip.on("success", (e: any): void => {
                $YetaWF.confirm(YLocs.YetaWF_ComponentsHTML.CopyToClip);
            });
        }

    }

    $YetaWF.addWhenReady(TextAreaSourceOnlyComponent.initAll);
}
