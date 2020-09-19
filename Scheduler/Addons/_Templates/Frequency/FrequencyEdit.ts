/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

namespace YetaWF_Scheduler {

    export class FrequencyEdit extends YetaWF.ComponentBaseDataImpl  {

        public static readonly TEMPLATE: string = "yt_yetawf_scheduler_frequency";
        public static readonly SELECTOR: string = ".yt_yetawf_scheduler_frequency.t_edit";

        private Value: YetaWF_ComponentsHTML.IntValueEditComponent;
        private Units: YetaWF_ComponentsHTML.DropDownListEditComponent;

        public constructor(controlId: string/*, setup: FrequencyEditSetup*/) {
            super(controlId, FrequencyEdit.TEMPLATE, FrequencyEdit.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: (control: FrequencyEdit): string | null => {
                    return control.Value.value?.toString();// not quite, doesn't include units
                },
                Enable: (control: FrequencyEdit, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                    if (clearOnDisable)
                        control.clear();
                },
            });

            this.Value = YetaWF.ComponentBaseDataImpl.getControlFromSelector("[name$='.Value']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [this.Control]);
            this.Units = YetaWF.ComponentBaseDataImpl.getControlFromSelector("[name$='.TimeUnits']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Control]);
        }

        public clear(): void {
            this.Value.clear();
            this.Units.clear();
        }
        public enable(enabled: boolean): void {
            this.Value.enable(enabled);
            this.Units.enable(enabled);
        }
    }
}