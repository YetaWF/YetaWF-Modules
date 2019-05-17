/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export interface IPackageVolatiles {
        CurrencyFormat: string;
    }

    interface CurrencyEditSetup {
        Min: number;
        Max: number;
    }

    export class CurrencyEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly SELECTOR: string = ".yt_currency.t_edit";

        kendoNumericTextBox: kendo.ui.NumericTextBox;
        Currency: HTMLInputElement;

        constructor(controlId: string, setup: CurrencyEditSetup) {
            super(controlId);

            this.Currency = $YetaWF.getElement1BySelector("input", [this.Control]) as HTMLInputElement;

            $(this.Currency).kendoNumericTextBox({
                format: YVolatile.YetaWF_ComponentsHTML.CurrencyFormat,
                min: setup.Min, max: setup.Max,
                culture: YVolatile.Basics.Language
            });
            this.kendoNumericTextBox = $(this.Control).data("kendoNumericTextBox");
        }
    }

    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        YetaWF.ComponentBaseDataImpl.clearDiv<CurrencyEditComponent>(tag, CurrencyEditComponent.SELECTOR, (control: CurrencyEditComponent): void => {
            control.kendoNumericTextBox.destroy();
        });
    });
}

