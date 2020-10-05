/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
}
