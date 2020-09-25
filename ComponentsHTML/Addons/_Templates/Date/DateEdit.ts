/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export interface IPackageVolatiles {
        DateFormat: string;
    }

    interface DateEditSetup {
        Min: Date;
        Max: Date;
    }

    export class DateEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_date";
        public static readonly SELECTOR: string = ".yt_date.t_edit";
        public static readonly EVENT: string = "date_change";

        Hidden: HTMLInputElement;
        Date: HTMLInputElement;
        kendoDatePicker: kendo.ui.DatePicker;

        constructor(controlId: string, setup: DateEditSetup) {
            super(controlId, DateEditComponent.TEMPLATE, DateEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: DateEditComponent.EVENT,
                GetValue: (control: DateEditComponent): string | null => {
                    return control.valueText;
                },
                Enable: (control: DateEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    YetaWF_BasicsImpl.elementEnableToggle(control.Hidden, enable);
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                }
            }, false, (tag: HTMLElement, control: DateEditComponent): void => {
                control.kendoDatePicker.destroy();
            });

            this.Hidden = $YetaWF.getElement1BySelector("input[type=\"hidden\"]", [this.Control]) as HTMLInputElement;
            this.Date = $YetaWF.getElement1BySelector("input[name=\"dtpicker\"]", [this.Control]) as HTMLInputElement;

            $(this.Date).kendoDatePicker({
                animation: false,
                format: YVolatile.YetaWF_ComponentsHTML.DateFormat,
                min: setup.Min, max: setup.Max,
                culture: YVolatile.Basics.Language,
                change: (ev: kendo.ui.DatePickerEvent): void => {
                    var kdPicker: kendo.ui.DatePicker = ev.sender;
                    var val: Date = kdPicker.value();
                    if (val == null)
                        this.setHiddenText(kdPicker.element.val() as string);
                    else
                        this.setHidden(val);
                    FormsSupport.validateElement(this.Hidden);
                    var event = document.createEvent("Event");
                    event.initEvent(DateEditComponent.EVENT, true, true);
                    this.Control.dispatchEvent(event);
                }
            });
            this.kendoDatePicker = $(this.Date).data("kendoDatePicker");
            this.setHidden(this.kendoDatePicker.value());
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
            this.kendoDatePicker.value(val);
        }
        public clear(): void {
            this.setHiddenText("");
            this.kendoDatePicker.value("");
        }
        public enable(enabled: boolean): void {
            this.kendoDatePicker.enable(enabled);
        }
        public close(): void {
            this.kendoDatePicker.close();
        }
        public static closeAll(): void {
            let ctrls = $YetaWF.getElementsBySelector(DateEditComponent.SELECTOR);
            for (let ctrl of ctrls) {
                let c = DateEditComponent.getControlFromTag<DateEditComponent>(ctrl, DateEditComponent.SELECTOR);
                c.close();
            }
        }
    }
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, (ev: Event): boolean => {
        DateEditComponent.closeAll();
        return true;
    });
    ($(window) as any).smartresize((): void => {
        DateEditComponent.closeAll();
    });
    window.addEventListener("scroll", (ev: Event): any => {
        DateEditComponent.closeAll();
        return true;
    });
}

