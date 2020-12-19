/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class ProgressBarComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_progressbar";
        public static readonly SELECTOR: string = ".yt_progressbar.t_display";

        private static MAXTIME: number = 0.5;
        private static INCRTIME: number = 0.02;

        private ValueDiv: HTMLDivElement;
        private Value: number;
        private Min: number;
        private Max: number;
        private Timer: number = 0;
        private ShownValue: number = 0;
        private NewValue: number = 0;

        constructor(controlId: string) {
            super(controlId, ProgressBarComponent.TEMPLATE, ProgressBarComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: (control: ProgressBarComponent): string | null => {
                    return control.value.toString();
                },
                Enable: null,
            });

            this.ValueDiv = $YetaWF.getElement1BySelector("div.ui-progressbar-value", [this.Control]) as HTMLDivElement;

            this.Min = Number($YetaWF.getAttribute(this.Control, "aria-valuemin"));
            this.Max = Number($YetaWF.getAttribute(this.Control, "aria-valuemax"));
            this.Value = Number($YetaWF.getAttribute(this.Control, "aria-valuenow"));
            this.ShownValue = this.Value;
        }

        private setValue(newValue: number): void {

            this.Value = newValue;
            this.NewValue = newValue;

            let step = (this.Max - this.Min) / (ProgressBarComponent.MAXTIME / ProgressBarComponent.INCRTIME);
            if (!this.Timer) {
                this.Timer = setInterval((): void => {
                    let newValue: number;
                    if (this.NewValue > this.ShownValue) {
                        newValue = this.ShownValue + step;
                        if (newValue >= this.Value) {
                            newValue = this.Value;
                            clearInterval(this.Timer);
                            this.Timer = 0;
                        }
                    } else { //if (this.NewValue <= this.Value) {
                        newValue = this.ShownValue - step;
                        if (newValue <= this.Value) {
                            newValue = this.Value;
                            clearInterval(this.Timer);
                            this.Timer = 0;
                        }
                    }
                    this.ShownValue = newValue;
                    this.ValueDiv.style.width = `${newValue}%`;
                }, ProgressBarComponent.INCRTIME);
            }
        }

        // API

        get value(): number {
            return this.Value;
        }
        set value(val: number) {
            if (val > this.Max)
                val = this.Max;
            if (val < this.Min)
                val = this.Min;
            this.setValue(val);
        }
        public show(): void {
            this.Control.style.display = "";
        }
        public hide(): void {
            this.Control.style.display = "none";
        }
    }
}
