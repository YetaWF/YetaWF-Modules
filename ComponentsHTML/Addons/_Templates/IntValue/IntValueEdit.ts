 /* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface IntValueSetup {
        Min: number;
        Max: number;
        Step: number;
        NoEntryText: string;
    }

    export class IntValueEditComponent extends YetaWF.ComponentBase<HTMLInputElement> {

        public static readonly SELECTOR: string = "input.yt_intvalue_base.t_edit.k-input[name]";

        kendoNumericTextBox: kendo.ui.NumericTextBox | null = null;

        constructor(controlId: string, setup: IntValueSetup) {
            super(controlId);

            $YetaWF.addObjectDataById(controlId, this);

            $(this.Control).kendoNumericTextBox({
                decimals: 0, format: "n0",
                min: setup.Min, max: setup.Max,
                placeholder: setup.NoEntryText,
                step: setup.Step,
                downArrowText: "",
                upArrowText: ""
            });
            this.kendoNumericTextBox = $(this.Control).data("kendoNumericTextBox");
        }

        get value(): number {
            return parseInt(this.Control.value);
        }
        set value(val: number) {
            if (this.kendoNumericTextBox == null) {
                this.Control.value = val.toString();
            } else {
                this.kendoNumericTextBox.value(val);
            }
        }
        public clear(): void {
            if (this.kendoNumericTextBox == null) {
                this.Control.value = "";
            } else {
                this.kendoNumericTextBox.value("");
            }
        }
        public enable(enabled: boolean): void {
            if (this.kendoNumericTextBox == null) {
                $YetaWF.elementEnableToggle(this.Control, enabled);
            } else {
                this.kendoNumericTextBox.enable(enabled);
            }
        }
        public destroy(): void {
            if (this.kendoNumericTextBox)
                this.kendoNumericTextBox.destroy();
            $YetaWF.removeObjectDataById(this.Control.id);
        }

        public static getControlFromTag(elem: HTMLElement): IntValueEditComponent { return super.getControlBaseFromTag<IntValueEditComponent>(elem, IntValueEditComponent.SELECTOR); }
        public static getControlFromSelector(selector: string, tags: HTMLElement[]): IntValueEditComponent { return super.getControlBaseFromSelector<IntValueEditComponent>(selector, IntValueEditComponent.SELECTOR, tags); }
    }

    // A <div> is being emptied. Destroy all IntValues the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        YetaWF.ComponentBase.clearDiv<IntValueEditComponent>(tag, IntValueEditComponent.SELECTOR, (control: IntValueEditComponent): void => {
            control.destroy();
        });
    });
}

