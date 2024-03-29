/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export interface IPackageLocs {
        CopyToClip: string;
    }

    export class BooleanTextEditComponent extends YetaWF.ComponentBase {

        public static readonly TEMPLATE: string = "yt_booleantext";
        public static readonly SELECTOR: string = ".yt_booleantext.t_edit";

    }

    BooleanTextEditComponent.register(BooleanTextEditComponent.TEMPLATE, BooleanTextEditComponent.SELECTOR, false, {
        ControlType: ControlTypeEnum.Input,
        ChangeEvent: null,
        GetValue: (control: HTMLInputElement): string | null => {
            return control.checked ? "true" : "false";
        },
        Enable: (control: HTMLInputElement, enable: boolean, clearOnDisable: boolean): void => {
            YetaWF_BasicsImpl.elementEnableToggle(control, enable);
            if (clearOnDisable)
                control.checked = false;
        },
    });
}
