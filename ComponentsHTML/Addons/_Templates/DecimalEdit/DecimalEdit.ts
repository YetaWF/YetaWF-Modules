/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/// <reference types="kendo-ui" />

namespace YetaWF_ComponentsHTML {

    interface DecimalEditSetup {
        Min: number;
        Max: number;
        PlaceHolder: string|null;
    }

    export class DecimalEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_decimal";
        public static readonly SELECTOR: string = "input.yt_decimal.t_edit.k-input[name]";

        kendoNumericTextBox: kendo.ui.NumericTextBox;

        constructor(controlId: string, setup: DecimalEditSetup) {
            super(controlId, DecimalEditComponent.TEMPLATE, DecimalEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: "decimal_change",
                GetValue: (control: DecimalEditComponent): string | null => {
                    return control.valueText;
                },
                Enable: (control: DecimalEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }, false, (tag: HTMLElement, control: DecimalEditComponent): void => {
                control.kendoNumericTextBox.destroy();
            });

            $(this.Control).kendoNumericTextBox({
                format: "0.00",
                min: setup.Min, max: setup.Max,
                culture: YVolatile.Basics.Language,
                downArrowText: "",
                upArrowText: "",
                placeholder: setup.PlaceHolder??undefined,
                change: (e: kendo.ui.NumericTextBoxChangeEvent): void => {
                    $(this.Control).trigger("change");
                    var event = document.createEvent("Event");
                    event.initEvent("decimal_change", true, true);
                    this.Control.dispatchEvent(event);
                    FormsSupport.validateElement(this.Control);
                }
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
}

