/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class BooleanEditComponent extends YetaWF.ComponentBase {

        public static readonly TEMPLATE: string = "yt_boolean";
        public static readonly SELECTOR: string = ".yt_boolean.t_edit";

    }
    BooleanEditComponent.register(BooleanEditComponent.TEMPLATE, BooleanEditComponent.SELECTOR, false, {
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
