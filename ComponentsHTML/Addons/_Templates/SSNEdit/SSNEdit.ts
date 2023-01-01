/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class SSNEditComponent extends MaskedEditComponent {

        public static readonly TEMPLATE: string = "yt_ssn";
        public static readonly SELECTOR: string = ".yt_ssn.t_edit";
        public static readonly EVENTCHANGE: string = "ssn_change";

        constructor(controlId: string, setup: MaskedEdit) {
            super(controlId, setup, SSNEditComponent.TEMPLATE, SSNEditComponent.SELECTOR, SSNEditComponent.EVENTCHANGE);
        }
    }
}

