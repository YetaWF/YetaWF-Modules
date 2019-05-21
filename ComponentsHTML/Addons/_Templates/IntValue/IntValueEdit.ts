/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface IntValueSetup {
        Min: number;
        Max: number;
        Step: number;
        NoEntryText: string;
    }

    export class IntValueEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_intvalue_base";
        public static readonly SELECTOR: string = "input.yt_intvalue_base.t_edit.k-input[name]";

        private InputControl: HTMLInputElement;

        public readonly kendoNumericTextBox: kendo.ui.NumericTextBox | null = null;

        constructor(controlId: string, setup: IntValueSetup) {
            super(controlId, IntValueEditComponent.TEMPLATE, IntValueEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: "intvalue_change",
                GetValue: (control: IntValueEditComponent): string | null => {
                    return control.value.toString();
                },
                Enable: (control: IntValueEditComponent, enable: boolean): void => {
                    control.enable(enable);
                },
            }, false, (tag: HTMLElement, control: IntValueEditComponent): void => {
                if (control.kendoNumericTextBox)
                    control.kendoNumericTextBox.destroy();
            });

            this.InputControl = this.Control as HTMLInputElement;

            $(this.InputControl).kendoNumericTextBox({
                decimals: 0, format: "n0",
                min: setup.Min, max: setup.Max,
                placeholder: setup.NoEntryText,
                step: setup.Step,
                downArrowText: "",
                upArrowText: "",
                change: (e: kendo.ui.NumericTextBoxChangeEvent): void => {
                    $(this.Control).trigger("change");
                    var event = document.createEvent("Event");
                    event.initEvent("intvalue_change", true, true);
                    this.Control.dispatchEvent(event);
                    FormsSupport.validateElement(this.Control);
                }
            });
            this.kendoNumericTextBox = $(this.InputControl).data("kendoNumericTextBox");
        }

        get value(): number {
            return parseInt(this.InputControl.value, 10);
        }
        set value(val: number) {
            if (this.kendoNumericTextBox == null) {
                this.InputControl.value = val.toString();
            } else {
                this.kendoNumericTextBox.value(val);
            }
        }
        public clear(): void {
            if (this.kendoNumericTextBox == null) {
                this.InputControl.value = "";
            } else {
                this.kendoNumericTextBox.value("");
            }
        }
        public enable(enabled: boolean): void {
            if (this.kendoNumericTextBox == null) {
                $YetaWF.elementEnableToggle(this.InputControl, enabled);
            } else {
                this.kendoNumericTextBox.enable(enabled);
            }
        }
    }
}

