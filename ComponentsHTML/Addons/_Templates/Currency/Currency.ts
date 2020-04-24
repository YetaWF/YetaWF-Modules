/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export interface IPackageVolatiles {
        CurrencyFormat: string;
    }

    interface CurrencyEditSetup {
        Min: number;
        Max: number;
        ReadOnly: boolean;
    }

    export class CurrencyEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_currency";
        public static readonly SELECTOR: string = ".yt_currency.t_edit";
        public static readonly EVENT: string = "currency_change";

        kendoNumericTextBox: kendo.ui.NumericTextBox;
        Currency: HTMLInputElement;

        constructor(controlId: string, setup: CurrencyEditSetup) {
            super(controlId, CurrencyEditComponent.TEMPLATE, CurrencyEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: CurrencyEditComponent.EVENT,
                GetValue: (control: CurrencyEditComponent): string | null => {
                    return control.value ? control.value.toString() : null;
                },
                Enable: (control: CurrencyEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                }
            }, false, (tag: HTMLElement, control: CurrencyEditComponent): void => {
               control.kendoNumericTextBox.destroy();
            });

            this.Currency = $YetaWF.getElement1BySelector("input", [this.Control]) as HTMLInputElement;

            $(this.Currency).kendoNumericTextBox({
                format: YVolatile.YetaWF_ComponentsHTML.CurrencyFormat,
                min: setup.Min, max: setup.Max,
                culture: YVolatile.Basics.Language,
                change: (e: kendo.ui.NumericTextBoxChangeEvent): void => {
                    $(this.Control).trigger("change");
                    var event = document.createEvent("Event");
                    event.initEvent(CurrencyEditComponent.EVENT, true, true);
                    this.Control.dispatchEvent(event);
                    FormsSupport.validateElement(this.Currency);
                }
            });
            this.kendoNumericTextBox = $(this.Currency).data("kendoNumericTextBox");
            if (setup.ReadOnly) this.kendoNumericTextBox.readonly(true);
        }

        public get value(): number {
            return this.kendoNumericTextBox.value();
        }
        public set value(value:number) {
            this.kendoNumericTextBox.value(value);
        }
        public enable(enable: boolean): void {
            this.kendoNumericTextBox.enable(enable);
        }
        public clear(): void {
            this.kendoNumericTextBox.value("");
        }
    }
}

