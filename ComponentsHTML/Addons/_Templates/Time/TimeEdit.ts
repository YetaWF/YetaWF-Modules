/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export interface IPackageVolatiles {
        TimeFormat: string;
    }

    export class TimeEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_time";
        public static readonly SELECTOR: string = ".yt_time.t_edit";

        Hidden: HTMLInputElement;
        Time: HTMLInputElement;
        kendoTimePicker: kendo.ui.TimePicker;

        constructor(controlId: string /*, setup: TimeEditSetup*/) {
            super(controlId, TimeEditComponent.TEMPLATE, TimeEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: "time_change",
                GetValue: (control: TimeEditComponent): string | null => {
                    return control.valueText;
                },
                Enable: (control: TimeEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    YetaWF_BasicsImpl.elementEnableToggle(control.Hidden, enable);
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }, false, (tag: HTMLElement, control: TimeEditComponent): void => {
                control.kendoTimePicker.destroy();
            });

            this.Hidden = $YetaWF.getElement1BySelector("input[type=\"hidden\"]", [this.Control]) as HTMLInputElement;
            this.Time = $YetaWF.getElement1BySelector("input[name=\"dtpicker\"]", [this.Control]) as HTMLInputElement;

            $(this.Time).kendoTimePicker({
                animation: false,
                format: YVolatile.YetaWF_ComponentsHTML.TimeFormat,
                culture: YVolatile.Basics.Language,
                change: (ev: kendo.ui.TimePickerEvent): void => {
                    var kdPicker: kendo.ui.TimePicker = ev.sender;
                    var val: Date = kdPicker.value();
                    if (val == null)
                        this.setHiddenText(kdPicker.element.val() as string);
                    else
                        this.setHidden(val);
                    FormsSupport.validateElement(this.Hidden);
                    var event = document.createEvent("Event");
                    event.initEvent("time_change", true, true);
                    this.Control.dispatchEvent(event);
                }
            });
            this.kendoTimePicker = $(this.Time).data("kendoTimePicker");
            this.setHidden(this.kendoTimePicker.value());
        }
        private setHidden(dateVal: Date | null): void {
            var s: string = "";
            if (dateVal != null)
                s = dateVal.toISOString();
            this.Hidden.setAttribute("value", s);
        }
        private setHiddenText(dateVal: string | null): void {
            this.Hidden.setAttribute("value", dateVal ? dateVal : "");
        }
        get value(): Date {
            return new Date(this.Hidden.value);
        }
        get valueText(): string {
            return this.Hidden.value;
        }
        set value(val: Date) {
            this.setHidden(val);
            this.kendoTimePicker.value(val);
        }
        public clear(): void {
            this.setHiddenText("");
            this.kendoTimePicker.value("");
        }
        public enable(enabled: boolean): void {
            this.kendoTimePicker.enable(enabled);
        }
    }
}

