/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Input,
                ChangeEvent: null,
                GetValue: (control: HTMLInputElement): string | null => {
                    return control.value;
                },
                Enable: (control: HTMLInputElement, enable: boolean, clearOnDisable: boolean): void => {
                    YetaWF_BasicsImpl.elementEnableToggle(control, enable);
                    if (clearOnDisable)
                        control.value = "";
                },
            });
            //this.Setup = setup;
            if ($YetaWF.getAttributeCond(this.Control, "placeholder")) {
                $YetaWF.registerEventHandler(this.Control, "focus", null, (ev: FocusEvent): boolean => {
                    let ph = $YetaWF.getAttributeCond(this.Control, "placeholder");
                    if (ph) {
                        this.Control.removeAttribute("placeholder");
                        $YetaWF.setAttribute(this.Control, "data-placeholder", ph);
                    }
                    return true;
                });
                $YetaWF.registerEventHandler(this.Control, "blur", null, (ev: FocusEvent): boolean => {
                    let ph = $YetaWF.getAttributeCond(this.Control, "data-placeholder");
                    if (ph) {
                        this.Control.removeAttribute("data-placeholder");
                        $YetaWF.setAttribute(this.Control, "placeholder", ph);
                    }
                    return true;
                });
            }
        }
    }
    TextEditComponent.register(TextEditComponent.TEMPLATE, TextEditComponent.SELECTOR, false, {
        ControlType: ControlTypeEnum.Input,
        ChangeEvent: null,
        GetValue: (control: HTMLInputElement): string | null => {
            return control.value;
        },
        Enable: (control: HTMLInputElement, enable: boolean, clearOnDisable: boolean): void => {
            YetaWF_BasicsImpl.elementEnableToggle(control, enable);
            if (clearOnDisable)
                control.value = "";
        },
    });
    let t = TextEditComponent.getTemplateDefinition(TextEditComponent.TEMPLATE);
    TextEditComponent.register("yt_text", ".yt_text.t_edit", t.HasData, t.UserData, t.Display, t.DestroyControl);
    TextEditComponent.register("yt_text10", ".yt_text10.t_edit", t.HasData, t.UserData, t.Display, t.DestroyControl);
    TextEditComponent.register("yt_text20", ".yt_text20.t_edit", t.HasData, t.UserData, t.Display, t.DestroyControl);
    TextEditComponent.register("yt_text40", ".yt_text40.t_edit", t.HasData, t.UserData, t.Display, t.DestroyControl);
    TextEditComponent.register("yt_text80", ".yt_text80.t_edit", t.HasData, t.UserData, t.Display, t.DestroyControl);
}
