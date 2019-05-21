/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

declare var ClipboardJS: any;

namespace YetaWF_ComponentsHTML {

    export interface IPackageLocs {
        CopyToClip: string;
    }

    export class TextEditComponent extends YetaWF.ComponentBaseNoDataImpl {

        public static readonly TEMPLATE: string = "yt_text_base";
        public static readonly SELECTOR: string = ".yt_text_base.t_edit";

        constructor(controlId: string /*, setup: TextEditSetup*/) {
            super(controlId, TextEditComponent.TEMPLATE, TextEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Input,
                ChangeEvent: null,
                GetValue: (control: HTMLInputElement): string | null => {
                    return control.value;
                },
                Enable: (control: HTMLInputElement, enable: boolean): void => {
                    control.removeAttribute("disabled");
                    $YetaWF.elementRemoveClass(control, "k-state-disabled");
                    if (!enable) {
                        control.setAttribute("disabled", "disabled");
                        $YetaWF.elementAddClass(control, "k-state-disabled");
                    }
                },
            });
            // synonyms
            let t = YetaWF.ComponentBaseDataImpl.getTemplateDefinition(TextEditComponent.TEMPLATE);
            this.registerTemplate("yt_text", t.Selector, t.UserData, t.Display, t.DestroyControl);
            this.registerTemplate("yt_text10", t.Selector, t.UserData, t.Display, t.DestroyControl);
            this.registerTemplate("yt_text20", t.Selector, t.UserData, t.Display, t.DestroyControl);
            this.registerTemplate("yt_text40", t.Selector, t.UserData, t.Display, t.DestroyControl);
            this.registerTemplate("yt_text80", t.Selector, t.UserData, t.Display, t.DestroyControl);
        }
    }
}
