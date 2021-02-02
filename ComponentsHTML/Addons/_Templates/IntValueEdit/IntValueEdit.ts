/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class IntValueEditComponent extends NumberEditComponentBase {

        public static readonly TEMPLATE: string = "yt_intvalue_base";
        public static readonly SELECTOR: string = ".yt_intvalue_base.t_edit";

        // events duplicated from NumberEditComponentBase to avoid changes in component users
        public static readonly EVENT: string = "number_changespin";// combines change and spin
        public static readonly EVENTCHANGE: string = "number_change";
        public static readonly EVENTSPIN: string = "number_spin";

        constructor(controlId: string, setup: NumberSetup) {
            super(controlId, setup, IntValueEditComponent.TEMPLATE, IntValueEditComponent.SELECTOR);
        }
    }
}

