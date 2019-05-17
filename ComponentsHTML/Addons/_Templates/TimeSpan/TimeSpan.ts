/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    //interface Setup {
    //}

    export class TimeSpanEditComponent {

        public Control: HTMLDivElement;
        public Hidden: HTMLInputElement;
        public InputDays: IntValueEditComponent|null;
        public InputHours: IntValueEditComponent | null;
        public InputMins: IntValueEditComponent | null;
        public InputSecs: IntValueEditComponent | null;

        constructor(controlId: string/*, setup: Setup*/) {
            this.Control = $YetaWF.getElementById(controlId) as HTMLDivElement;
            this.Hidden = $YetaWF.getElement1BySelector("input[type='hidden']", [this.Control]) as HTMLInputElement;

            this.InputDays = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond<IntValueEditComponent>("input[name$='Days']", IntValueEditComponent.SELECTOR, [this.Control]);
            this.InputHours = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond<IntValueEditComponent>("input[name$='Hours']", IntValueEditComponent.SELECTOR, [this.Control]);
            this.InputMins = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond<IntValueEditComponent>("input[name$='Minutes']", IntValueEditComponent.SELECTOR, [this.Control]);
            this.InputSecs = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond<IntValueEditComponent>("input[name$='Seconds']", IntValueEditComponent.SELECTOR, [this.Control]);

            // capture changes in all edit controls
            if (this.InputDays) {
                this.InputDays.Control.addEventListener("intvalue_change", (evt: Event): void => {
                    this.updateValue();
                });
            }
            if (this.InputHours) {
                this.InputHours.Control.addEventListener("intvalue_change", (evt: Event): void => {
                    this.updateValue();
                });
            }
            if (this.InputMins) {
                this.InputMins.Control.addEventListener("intvalue_change", (evt: Event): void => {
                    this.updateValue();
                });
            }
            if (this.InputSecs) {
                this.InputSecs.Control.addEventListener("intvalue_change", (evt: Event): void => {
                    this.updateValue();
                });
            }
        }
        private updateValue(): void {
            if (this.InputDays && this.InputHours && this.InputMins && this.InputSecs) {
                this.Hidden.value = `${this.InputDays.value}.${this.InputHours.value}:${this.InputMins.value}:${this.InputSecs.value}`;
            } else if (this.InputDays && this.InputHours && this.InputMins) {
                this.Hidden.value = `${this.InputDays.value}.${this.InputHours.value}:${this.InputMins.value}`;
            } if (this.InputHours && this.InputMins && this.InputSecs) {
                this.Hidden.value = `${this.InputHours.value}:${this.InputMins.value}:${this.InputSecs.value}`;
            } else if (this.InputHours && this.InputMins) {
                this.Hidden.value = `${this.InputHours.value}:${this.InputMins.value}`;
            }
        }
    }
}
