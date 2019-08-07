/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export class DayTimeRangeEditComponent extends YetaWF.ComponentBaseNoDataImpl {

        public static readonly TEMPLATE: string = "yt_daytimerange";
        public static readonly SELECTOR: string = ".yt_daytimerange.t_edit";

        private Additional: HTMLInputElement;
        private AddDiv: HTMLDivElement;
        private Closed: HTMLInputElement;
        private ClosedDiv: HTMLDivElement;
        private StartDiv: HTMLDivElement;
        private EndDiv: HTMLDivElement;
        private Start2Div: HTMLDivElement;
        private End2Div: HTMLDivElement;
        private Start: HTMLInputElement;
        private End: HTMLInputElement;
        private Start2: HTMLInputElement;
        private End2: HTMLInputElement;

        constructor(controlId: string/*, setup: DayTimeRangeEditSetup*/) {
            super(controlId, DayTimeRangeEditComponent.TEMPLATE, DayTimeRangeEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Div,
                ChangeEvent: null,
                GetValue: null,
                Enable: (control: DateTimeEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    alert("HI");
                    //control.enable(enable);
                    //if (!enable && clearOnDisable)
                    //    control.clear();
                },
            });

            this.Additional = $YetaWF.getElement1BySelector("input[name$='.Additional']", [this.Control]) as HTMLInputElement;
            this.AddDiv = $YetaWF.getElement1BySelector(".t_add", [this.Control]) as HTMLDivElement;
            this.Closed = $YetaWF.getElement1BySelector("input[name$='.Closed']", [this.Control]) as HTMLInputElement;
            this.ClosedDiv = $YetaWF.getElement1BySelector(".t_closed", [this.Control]) as HTMLDivElement;
            this.StartDiv = $YetaWF.getElement1BySelector(".t_from", [this.Control]) as HTMLDivElement;
            this.EndDiv = $YetaWF.getElement1BySelector(".t_to", [this.Control]) as HTMLDivElement;
            this.Start2Div = $YetaWF.getElement1BySelector(".t_from2", [this.Control]) as HTMLDivElement;
            this.End2Div = $YetaWF.getElement1BySelector(".t_to2", [this.Control]) as HTMLDivElement;

            this.Start = $YetaWF.getElement1BySelector("input[name$='.Start']", [this.Control]) as HTMLInputElement;
            this.End = $YetaWF.getElement1BySelector("input[name$='.End']", [this.Control]) as HTMLInputElement;
            this.Start2 = $YetaWF.getElement1BySelector("input[name$='.Start2']", [this.Control]) as HTMLInputElement;
            this.End2 = $YetaWF.getElement1BySelector("input[name$='.End2']", [this.Control]) as HTMLInputElement;

            this.toggleRanges();

            $YetaWF.registerEventHandler(this.Additional, "change", null, (ev: Event): boolean => {
                this.toggleRanges();
                return false;
            });
            $YetaWF.registerEventHandler(this.Closed, "change", null, (ev: Event): boolean => {
                this.toggleRanges();
                return false;
            });
        }

        private NoSubmit: string = `yform-nosubmit yform-novalidate`;

        private toggleRanges(): void {
            if (this.Closed.checked) {
                this.StartDiv.style.display = "none";
                this.EndDiv.style.display = "none";
                this.Start2Div.style.display = "none";
                this.End2Div.style.display = "none";
                this.AddDiv.style.display = "none";
                $YetaWF.elementAddClassList(this.Start, this.NoSubmit);
                $YetaWF.elementAddClassList(this.End, this.NoSubmit);
                $YetaWF.elementAddClassList(this.Start2, this.NoSubmit);
                $YetaWF.elementAddClassList(this.End2, this.NoSubmit);
            } else {
                this.StartDiv.style.display = "";
                this.EndDiv.style.display = "";
                this.AddDiv.style.display = "";
                $YetaWF.elementRemoveClassList(this.Start, this.NoSubmit);
                $YetaWF.elementRemoveClassList(this.End, this.NoSubmit);
                if (this.Additional.checked) {
                    this.Start2Div.style.display = "";
                    this.End2Div.style.display = "";
                    $YetaWF.elementRemoveClassList(this.Start2, this.NoSubmit);
                    $YetaWF.elementRemoveClassList(this.End2, this.NoSubmit);
                    this.ClosedDiv.style.display = "none";
                } else {
                    this.Start2Div.style.display = "none";
                    this.End2Div.style.display = "none";
                    $YetaWF.elementAddClassList(this.Start2, this.NoSubmit);
                    $YetaWF.elementAddClassList(this.End2, this.NoSubmit);
                    this.ClosedDiv.style.display = "";
                }
            }
        }
    }
}

$.validator.unobtrusive.adapters.add("daytimerangeto", (options: any): void => {
    options.rules["daytimerangeto"] = {};
    options.messages["daytimerangeto"] = options.message;
});

$.validator.addMethod("daytimerangeto", (value: any, element: HTMLElement, parameters: any): boolean => {

    if ($YetaWF.elementHasClass(element, "yform-novalidate")) return true;

    const elem = element as HTMLInputElement;
    const isRange1 = $YetaWF.elementClosestCond(elem, ".t_to") != null;
    const control = $YetaWF.elementClosestCond(elem, ".yt_daytimerange");
    if (control == null) return false;

    var fromRange: HTMLInputElement;
    if (isRange1)
        fromRange = $YetaWF.getElement1BySelector("input[name$='.Start']", [control]) as HTMLInputElement;
    else
        fromRange = $YetaWF.getElement1BySelector("input[name$='.Start2']", [control]) as HTMLInputElement;
    try {
        const dtTo = new Date(elem.value);
        const dtFrom  = new Date(fromRange.value);
        if (dtTo >= dtFrom) return true;
    } finally { }

    return false;
});

$.validator.unobtrusive.adapters.add("daytimerangefrom2", (options: any): void => {
    options.rules["daytimerangefrom2"] = {};
    options.messages["daytimerangefrom2"] = options.message;
});

$.validator.addMethod("daytimerangefrom2", (value: any, element: HTMLElement, parameters: any): boolean => {

    if ($YetaWF.elementHasClass(element, "yform-novalidate")) return true;

    const elem = element as HTMLInputElement;
    const control = $YetaWF.elementClosestCond(elem, ".yt_daytimerange");
    if (control == null) return false;

    const endRange1 = $YetaWF.getElement1BySelector("input[name$='.End']", [control]) as HTMLInputElement;
    try {
        const dtFrom2 = new Date(elem.value);
        const dtTo1 = new Date(endRange1.value);
        if (dtFrom2 >= dtTo1) return true;
    } finally { }

    return false;
});

