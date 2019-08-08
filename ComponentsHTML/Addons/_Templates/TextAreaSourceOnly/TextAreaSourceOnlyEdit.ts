/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

declare var ClipboardJS: any;

namespace YetaWF_ComponentsHTML {

    export interface IPackageLocs {
        CopyToClip: string;
    }

    export class TextAreaSourceOnlyEditComponent extends YetaWF.ComponentBaseNoDataImpl {

        public static readonly TEMPLATE: string = "yt_textareasourceonly";
        public static readonly SELECTOR: string = ".yt_textareasourceonly.t_edit";

        public static clip: any = null;

        constructor(controlId: string /*, setup: TextAreaSourceOnlyEditSetup*/) {
            super(controlId, TextAreaSourceOnlyEditComponent.TEMPLATE, TextAreaSourceOnlyEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.TextArea,
                ChangeEvent: null,
                GetValue: (control: HTMLTextAreaElement): string | null => {
                    return control.value;
                },
                Enable: (control: HTMLTextAreaElement, enable: boolean, clearOnDisable: boolean): void => {
                    $YetaWF.elementEnableToggle(control, enable);
                    if (clearOnDisable)
                        control.value = "";
                },
            });
            //this.Setup = setup;

        }

        public static initAll(tag: HTMLElement): void {

            if (TextAreaSourceOnlyEditComponent.clip != null) return;
            //var elems = $YetaWF.getElementsBySelector(".yt_textareasourceonly_copy", [tag]);
            //if (elems.length === 0) return;

            TextAreaSourceOnlyEditComponent.clip = new ClipboardJS(".yt_textareasourceonly_copy", {
                target: (trigger: HTMLElement): Element | null => {
                    return trigger.previousElementSibling;
                },
            });
            TextAreaSourceOnlyEditComponent.clip.on("success", (e: any): void => {
                $YetaWF.confirm(YLocs.YetaWF_ComponentsHTML.CopyToClip);
            });
        }

    }

    $YetaWF.addWhenReady(TextAreaSourceOnlyEditComponent.initAll);
}
