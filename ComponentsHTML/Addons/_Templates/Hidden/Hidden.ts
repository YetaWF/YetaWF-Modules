/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class HiddenComponent extends YetaWF.ComponentBase {

        public static readonly TEMPLATE: string = "yt_hidden";
        public static readonly SELECTOR: string = ".yt_hidden";

    }

    HiddenComponent.register(HiddenComponent.TEMPLATE, HiddenComponent.SELECTOR, false, {
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
}
