/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class SSNEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_ssn";
        public static readonly SELECTOR: string = ".yt_ssn.t_edit";
        public static readonly EVENTCHANGE: string = "ssn_change";

        Hidden: HTMLInputElement;
        SSN: HTMLInputElement;
        kendoMaskedTextBox: kendo.ui.MaskedTextBox;

        constructor(controlId: string) {
            super(controlId, SSNEditComponent.TEMPLATE, SSNEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: SSNEditComponent.EVENTCHANGE,
                GetValue: (control: SSNEditComponent): string | null => {
                    return control.valueRaw;
                },
                Enable: (control: SSNEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    YetaWF_BasicsImpl.elementEnableToggle(control.Hidden, enable);
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                }
            }, false, (tag: HTMLElement, control: SSNEditComponent): void => {
                control.kendoMaskedTextBox.destroy();
            });

            this.Hidden = $YetaWF.getElement1BySelector("input[type=\"hidden\"]", [this.Control]) as HTMLInputElement;
            this.SSN = $YetaWF.getElement1BySelector("input[name=\"ssninput\"]", [this.Control]) as HTMLInputElement;

            $(this.SSN).kendoMaskedTextBox({
                mask: "000-00-0000",
                change: (ev: kendo.ui.MaskedTextBoxChangeEvent): void => {
                    let kdMask: kendo.ui.MaskedTextBox = ev.sender;
                    let val = kdMask.raw();
                    this.setHidden(val);
                    FormsSupport.validateElement(this.Hidden);
                    var event = document.createEvent("Event");
                    event.initEvent(SSNEditComponent.EVENTCHANGE, true, true);
                    this.Control.dispatchEvent(event);
                }
            });
            this.kendoMaskedTextBox = $(this.SSN).data("kendoMaskedTextBox");
            this.setHidden(this.kendoMaskedTextBox.raw());
        }
        private setHidden(val: string | null): void {
            this.Hidden.setAttribute("value", val??"");
        }
        get valueRaw(): string {
            return this.kendoMaskedTextBox.raw();
        }
        get valueText(): string {
            return this.kendoMaskedTextBox.value();
        }
        set value(val: string) {
            this.setHidden(val);
            this.kendoMaskedTextBox.value(val);
        }
        public clear(): void {
            this.setHidden(null);
            this.kendoMaskedTextBox.value("");
        }
        public enable(enabled: boolean): void {
            this.kendoMaskedTextBox.enable(enabled);
        }
    }
}

