/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    //interface Setup {
    //}

    export class TimeSpanEditComponent {

        public Control: HTMLDivElement;
        public Hidden: HTMLInputElement;
        public InputDays: IntValueEditComponent;
        public InputHours: IntValueEditComponent;
        public InputMins: IntValueEditComponent;
        public InputSecs: IntValueEditComponent;

        constructor(controlId: string/*, setup: Setup*/) {
            this.Control = $YetaWF.getElementById(controlId) as HTMLDivElement;
            this.Hidden = $YetaWF.getElement1BySelector("input[type='hidden']", [this.Control]) as HTMLInputElement;

            this.InputDays = YetaWF.ComponentBaseDataImpl.getControlFromSelector<IntValueEditComponent>("input[name$='Days']", IntValueEditComponent.SELECTOR, [this.Control]);
            this.InputHours = YetaWF.ComponentBaseDataImpl.getControlFromSelector<IntValueEditComponent>("input[name$='Hours']", IntValueEditComponent.SELECTOR, [this.Control]);
            this.InputMins = YetaWF.ComponentBaseDataImpl.getControlFromSelector<IntValueEditComponent>("input[name$='Minutes']", IntValueEditComponent.SELECTOR, [this.Control]);
            this.InputSecs = YetaWF.ComponentBaseDataImpl.getControlFromSelector<IntValueEditComponent>("input[name$='Seconds']", IntValueEditComponent.SELECTOR, [this.Control]);

            // capture changes in all edit controls
            this.InputDays.Control.addEventListener("intvalue_change", (evt: Event): void => {
                this.updateValue();
            });
            this.InputHours.Control.addEventListener("intvalue_change", (evt: Event): void => {
                this.updateValue();
            });
            this.InputMins.Control.addEventListener("intvalue_change", (evt: Event): void => {
                this.updateValue();
            });
            this.InputSecs.Control.addEventListener("intvalue_change", (evt: Event): void => {
                this.updateValue();
            });
        }
        private updateValue() {
            this.Hidden.value = `${this.InputDays.value}.${this.InputHours.value}:${this.InputMins.value}:${this.InputSecs.value}`;
        }
    }
}
