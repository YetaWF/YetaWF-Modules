/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class CurrencyEditComponent extends NumberEditComponentBase {

        public static readonly TEMPLATE: string = "yt_currency";
        public static readonly SELECTOR: string = ".yt_currency.t_edit";

        // events duplicated from NumberEditComponentBase to avoid changes in component users
        public static readonly EVENT: string = "number_changespin";// combines change and spin
        public static readonly EVENTCHANGE: string = "number_change";
        public static readonly EVENTSPIN: string = "number_spin";

        constructor(controlId: string, setup: NumberSetup) {
            super(controlId, setup, CurrencyEditComponent.TEMPLATE, CurrencyEditComponent.SELECTOR);
        }
    }
}

