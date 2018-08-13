 /* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export interface IPackageVolatiles {
        DateFormat: string;
    }

    export class DateEditComponent extends YetaWF.ComponentBase<HTMLElement> {

        public static readonly SELECTOR: string = ".yt_date.t_edit";

        DatePicker: HTMLInputElement;
        KendoDatePicker: kendo.ui.DatePicker;
        Hidden: HTMLInputElement;

        constructor(controlId: string) {
            super(controlId);

            $YetaWF.addObjectDataById(controlId, this);

            this.DatePicker = $YetaWF.getElement1BySelector("input[name='dtpicker']", [this.Control]) as HTMLInputElement;
            this.Hidden = $YetaWF.getElement1BySelector("input[type='hidden']", [this.Control]) as HTMLInputElement;
            var sd: Date = new Date(1900, 1 - 1, 1);
            var y = this.DatePicker.getAttribute("data-min-y");
            if (y != null) sd = new Date(Number(y), Number(this.DatePicker.getAttribute("data-min-m")) - 1, Number(this.DatePicker.getAttribute("data-min-d")));
            y = this.DatePicker.getAttribute("data-max-y");
            var ed: Date = new Date(2199, 12 - 1, 31);
            if (y != null) ed = new Date(Number(y), Number(this.DatePicker.getAttribute("data-max-m")) - 1, Number(this.DatePicker.getAttribute("data-max-d")));
            var thisObj: DateEditComponent = this;
            $(this.DatePicker).kendoDatePicker({
                animation: false,
                format: YVolatile.YetaWF_ComponentsHTML.DateFormat,
                min: sd, max: ed,
                culture: YVolatile.Basics.Language,
                change: (ev: kendo.ui.DatePickerEvent): void => {
                    var kdPicker: kendo.ui.DatePicker = ev.sender;
                    var val: Date = kdPicker.value();
                    if (val == null)
                        thisObj.setHiddenText(kdPicker.element.val() as string);
                    else
                        thisObj.setHiddenValue(val);
                    FormsSupport.validateElement(this.Hidden);

                    var event = document.createEvent('Event');
                    event.initEvent("date_change", false, true);
                    this.Control.dispatchEvent(event);
                }
            });
            this.KendoDatePicker = $(this.DatePicker).data("kendoDatePicker") as kendo.ui.DatePicker;
            this.setHiddenValue(this.KendoDatePicker.value());

            this.DatePicker.addEventListener("change", (event: Event): void => {
                var val: Date = this.KendoDatePicker.value();
                if (val == null)
                    thisObj.setHiddenText((event.target as HTMLInputElement).value);
                else
                    thisObj.setHiddenValue(val);
                FormsSupport.validateElement(this.Hidden);
            }, false);
        }

        private setHiddenText(dateVal: string | null): void {
            this.Hidden.setAttribute("value", dateVal ? dateVal : "");
        }
        private setHiddenValue(dateVal: Date): void {
            var s: string = "";
            if (dateVal != null)
                s = dateVal.toUTCString();
            this.Hidden.setAttribute("value", s);
        }

        get value(): string {
            return this.DatePicker.value;
        }

        public clear() {
            this.KendoDatePicker.value('');
        }
        public enable(enabled: boolean) {
            this.KendoDatePicker.enable(enabled);
        }
        public destroy() {
            this.KendoDatePicker.destroy();
            $YetaWF.removeObjectDataById(this.Control.id);
        }

        public static getControlFromTag(elem: HTMLElement): DateEditComponent { return super.getControlBaseFromTag<DateEditComponent>(elem, DateEditComponent.SELECTOR); }
        public static getControlFromSelector(selector: string, tags: HTMLElement[]): DateEditComponent { return super.getControlBaseFromSelector<DateEditComponent>(selector, DateEditComponent.SELECTOR, tags); }
    }

    export class DateGridComponent {

        Grid: HTMLElement;

        constructor (gridId: string, elem: HTMLElement) {

            this.Grid = $YetaWF.getElementById(gridId);
            // Build a kendo date picker
            // We have to add it next to the jqgrid provided input field elem
            // We can't use the jqgrid provided element as a kendodatepicker because jqgrid gets confused and
            // uses the wrong sorting option. So we add the datepicker next to the "official" input field (which we hide)
            var dtPick: HTMLElement = <input name="dtpicker" />;
            elem.insertAdjacentElement("afterend", dtPick);
            // Hide the jqgrid provided input element (we update the date in this hidden element)
            elem.style.display = "none";

            // init date picker
            $(dtPick).kendoDatePicker({
                animation: false,
                format: YVolatile.YetaWF_ComponentsHTML.DateFormat,
                //sb.Append("min: sd, max: ed,");
                culture: YVolatile.Basics.Language,
                change: (ev: kendo.ui.DatePickerEvent): void => {
                    var kdPicker: kendo.ui.DatePicker = ev.sender;
                    var val: Date = kdPicker.value();
                    var s: string = "";
                    if (val !== null) {
                        var utcDate:Date = new Date(Date.UTC(val.getFullYear(), val.getMonth(), val.getDate(), 0, 0, 0));
                        s = utcDate.toUTCString();
                    }
                    elem.setAttribute("value", s);
                }
            });

            dtPick.addEventListener("keydown", (event: KeyboardEvent): void => {
                if (event.keyCode === 13)
                    (this.Grid as any).triggerToolbar();
            }, false);
        }
    }

    // A <div> is being emptied. Destroy all date pickers the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        YetaWF.ComponentBase.clearDiv<DateEditComponent>(tag, DateEditComponent.SELECTOR, (control: DateEditComponent): void => {
            control.destroy();
        });
    });
}

