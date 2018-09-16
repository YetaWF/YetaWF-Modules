/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface DecimalEditSetup {
        Min: number;
        Max: number;
        NoEntryText: string;
    }

    export class DecimalEditComponent extends YetaWF.ComponentBase<HTMLInputElement> {

        public static readonly SELECTOR: string = "input.yt_decimal.t_edit.k-input[name]";

        kendoNumericTextBox: kendo.ui.NumericTextBox;

        constructor(controlId: string, setup: DecimalEditSetup) {
            super(controlId);

            $YetaWF.addObjectDataById(controlId, this);

            $(this.Control).kendoNumericTextBox({
                format: "0.00",
                min: setup.Min, max: setup.Max,
                culture: YVolatile.Basics.Language,
                downArrowText: "",
                upArrowText: ""
            });
            this.kendoNumericTextBox = $(this.Control).data("kendoNumericTextBox");
        }

        get value(): number {
            return parseFloat(this.Control.value);
        }
        get valueText(): string  {
            return this.Control.value;
        }
        set value(val: number) {
            this.kendoNumericTextBox.value(val);
        }
        public clear(): void {
            this.kendoNumericTextBox.value("");
        }
        public enable(enabled: boolean): void {
            this.kendoNumericTextBox.enable(enabled);
        }
        public destroy(): void {
            this.kendoNumericTextBox.destroy();
            $YetaWF.removeObjectDataById(this.Control.id);
        }

        public static getControlFromTag(elem: HTMLElement): DecimalEditComponent { return super.getControlBaseFromTag<DecimalEditComponent>(elem, DecimalEditComponent.SELECTOR); }
        public static getControlFromSelector(selector: string, tags: HTMLElement[]): DecimalEditComponent { return super.getControlBaseFromSelector<DecimalEditComponent>(selector, DecimalEditComponent.SELECTOR, tags); }
        public static getControlById(id: string): DecimalEditComponent { return super.getControlBaseById<DecimalEditComponent>(id, DecimalEditComponent.SELECTOR); }
    }

    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        YetaWF.ComponentBase.clearDiv<DecimalEditComponent>(tag, DecimalEditComponent.SELECTOR, (control: DecimalEditComponent): void => {
            control.destroy();
        });
    });
}

