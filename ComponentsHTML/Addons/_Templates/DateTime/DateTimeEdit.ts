/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export interface IPackageVolatiles {
        DateTimeFormat: string;
    }

    interface DateTimeEditSetup {
        Min: Date;
        Max: Date;
    }

    export class DateTimeEditComponent extends YetaWF.ComponentBase<HTMLInputElement> {

        public static readonly SELECTOR: string = ".yt_datetime.t_edit";

        Hidden: HTMLInputElement;
        Date: HTMLInputElement;
        kendoDateTimePicker: kendo.ui.DateTimePicker;

        constructor(controlId: string, setup: DateTimeEditSetup) {
            super(controlId);

            $YetaWF.addObjectDataById(controlId, this);

            this.Hidden = $YetaWF.getElement1BySelector("input[type=\"hidden\"]", [this.Control]) as HTMLInputElement;
            this.Date = $YetaWF.getElement1BySelector("input[name=\"dtpicker\"]", [this.Control]) as HTMLInputElement;

            $(this.Date).kendoDateTimePicker({
                animation: false,
                format: YVolatile.YetaWF_ComponentsHTML.DateTimeFormat,
                min: setup.Min, max: setup.Max,
                culture: YVolatile.Basics.Language,
                change: (ev: kendo.ui.DateTimePickerEvent): void => {
                    var kdPicker: kendo.ui.DateTimePicker = ev.sender;
                    var val: Date = kdPicker.value();
                    if (val == null)
                        this.setHiddenText(kdPicker.element.val() as string);
                    else
                        this.setHidden(val);
                    FormsSupport.validateElement(this.Hidden);
                }
            });
            this.kendoDateTimePicker = $(this.Date).data("kendoDateTimePicker");
            this.setHidden(this.kendoDateTimePicker.value());

            this.Date.addEventListener("change", (event: Event): void => {
                var val: Date = this.kendoDateTimePicker.value();
                if (val == null)
                    this.setHiddenText((event.target as HTMLInputElement).value);
                else
                    this.setHidden(val);
                FormsSupport.validateElement(this.Hidden);
            }, false);
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
        public destroy(): void {
            this.kendoDateTimePicker.destroy();
            $YetaWF.removeObjectDataById(this.Control.id);
        }

        public static getControlFromTag(elem: HTMLElement): DateTimeEditComponent { return super.getControlBaseFromTag<DateTimeEditComponent>(elem, DateTimeEditComponent.SELECTOR); }
        public static getControlFromSelector(selector: string, tags: HTMLElement[]): DateTimeEditComponent { return super.getControlBaseFromSelector<DateTimeEditComponent>(selector, DateTimeEditComponent.SELECTOR, tags); }
        public static getControlById(id: string): DateTimeEditComponent { return super.getControlBaseById<DateTimeEditComponent>(id, DateTimeEditComponent.SELECTOR); }
    }

    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        YetaWF.ComponentBase.clearDiv<DateTimeEditComponent>(tag, DateTimeEditComponent.SELECTOR, (control: DateTimeEditComponent): void => {
            control.destroy();
        });
    });
}

