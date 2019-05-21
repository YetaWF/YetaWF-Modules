/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export interface IPackageLocs {
        CopyToClip: string;
    }

    export class BooleanEditComponent extends YetaWF.ComponentBaseNoDataImpl {

        public static readonly TEMPLATE: string = "yt_boolean";
        public static readonly SELECTOR: string = ".yt_boolean.t_edit";

        constructor(controlId: string /*, setup: BooleanEditSetup*/) {
            super(controlId, BooleanEditComponent.TEMPLATE, BooleanEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Input,
                ChangeEvent: null,
                GetValue: (control: HTMLInputElement): string | null => {
                    return control.checked ? "true" : "false";
                },
                Enable: (control: HTMLInputElement, enable: boolean): void => {
                    if (enable) {
                        control.setAttribute("disabled", "disabled");
                        $YetaWF.elementRemoveClass(control, "k-state-disabled");
                    } else {
                        control.removeAttribute("disabled");
                        $YetaWF.elementAddClass(control, "k-state-disabled");
                    }
                },
            });
        }
    }
}
