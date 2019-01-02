/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface DecimalEditSetup {
        Min: number;
        Max: number;
        NoEntryText: string;
    }

    export class DecimalEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly SELECTOR: string = "input.yt_decimal.t_edit.k-input[name]";

        kendoNumericTextBox: kendo.ui.NumericTextBox;

        constructor(controlId: string, setup: DecimalEditSetup) {
            super(controlId);

            $(this.Control).kendoNumericTextBox({
                format: "0.00",
                min: setup.Min, max: setup.Max,
                culture: YVolatile.Basics.Language,
                downArrowText: "",
                upArrowText: ""
            });
            this.kendoNumericTextBox = $(this.Control).data("kendoNumericTextBox");
        }

        get value(): number | null {
            return this.kendoNumericTextBox.value();
        }
        get valueText(): string  {
            return this.value ? this.value.toString() : "";
        }
        set value(val: number | null) {
            if (val == null)
                this.kendoNumericTextBox.value("");
            else
                this.kendoNumericTextBox.value(val);
        }
        public clear(): void {
            this.kendoNumericTextBox.value("");
        }
        public enable(enabled: boolean): void {
            this.kendoNumericTextBox.enable(enabled);
        }
    }

    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        YetaWF.ComponentBaseDataImpl.clearDiv<DecimalEditComponent>(tag, DecimalEditComponent.SELECTOR, (control: DecimalEditComponent): void => {
            control.kendoNumericTextBox.destroy();
        });
    });
}

