/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/// <reference types="kendo-ui" />

namespace YetaWF_ComponentsHTML {

    export interface IPackageVolatiles {
        DateTimeFormat: string;
    }

    interface DateTimeEditSetup {
        Min: Date;
        Max: Date;
    }

    export class DateTimeEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_datetime";
        public static readonly SELECTOR: string = ".yt_datetime.t_edit";

        Hidden: HTMLInputElement;
        Date: HTMLInputElement;
        kendoDateTimePicker: kendo.ui.DateTimePicker;

        constructor(controlId: string, setup: DateTimeEditSetup) {
            super(controlId, DateTimeEditComponent.TEMPLATE, DateTimeEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: "datetime_change",
                GetValue: (control: DateTimeEditComponent): string | null => {
                    return control.valueText;
                },
                Enable: (control: DateTimeEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    YetaWF_BasicsImpl.elementEnableToggle(control.Hidden, enable);
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }, false, (tag: HTMLElement, control: DateTimeEditComponent): void => {
                control.kendoDateTimePicker.destroy();
            });

            this.Hidden = $YetaWF.getElement1BySelector("input[type=\"hidden\"]", [this.Control]) as HTMLInputElement;
            this.Date = $YetaWF.getElement1BySelector("input[name=\"dtpicker\"]", [this.Control]) as HTMLInputElement;

            $(this.Date).kendoDateTimePicker({
                animation: false,
                format: YVolatile.YetaWF_ComponentsHTML.DateTimeFormat,
                min: setup.Min, max: setup.Max,
                culture: YVolatile.Basics.Language,
                // close: (e: kendo.ui.DateTimePickerCloseEvent): void => {//used for css debugging only
                //     e.preventDefault();
                // },
                change: (ev: kendo.ui.DateTimePickerEvent): void => {
                    var kdPicker: kendo.ui.DateTimePicker = ev.sender;
                    var val: Date = kdPicker.value();
                    if (val == null)
                        this.setHiddenText(kdPicker.element.val() as string);
                    else
                        this.setHidden(val);
                    FormsSupport.validateElement(this.Hidden);
                    var event = document.createEvent("Event");
                    event.initEvent("datetime_change", true, true);
                    this.Control.dispatchEvent(event);
                }
            });
            this.kendoDateTimePicker = $(this.Date).data("kendoDateTimePicker");
            this.setHidden(this.kendoDateTimePicker.value());
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
            this.kendoDateTimePicker.value(val);
        }
        public clear(): void {
            this.setHiddenText("");
            this.kendoDateTimePicker.value("");
        }
        public enable(enabled: boolean): void {
            this.kendoDateTimePicker.enable(enabled);
        }
        public close(): void {
            this.kendoDateTimePicker.close("date");
            this.kendoDateTimePicker.close("time");
        }
        public static closeAll(): void {
            let ctrls = $YetaWF.getElementsBySelector(DateTimeEditComponent.SELECTOR);
            for (let ctrl of ctrls) {
                let c = DateTimeEditComponent.getControlFromTag<DateTimeEditComponent>(ctrl, DateTimeEditComponent.SELECTOR);
                c.close();
            }
        }
    }
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, (ev: CustomEvent<YetaWF.DetailsEventContainerScroll>): boolean => {
        DateTimeEditComponent.closeAll();
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: CustomEvent<YetaWF.DetailsEventContainerResize>): boolean => {
        DateTimeEditComponent.closeAll();
        return true;
    });
}

