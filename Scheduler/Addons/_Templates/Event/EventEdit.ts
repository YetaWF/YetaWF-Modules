/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

namespace YetaWF_Scheduler {

    export class EventEditComponent extends YetaWF.ComponentBaseNoDataImpl {

        public static readonly TEMPLATE: string = "yt_yetawf_scheduler_event";
        public static readonly SELECTOR: string = ".yt_yetawf_scheduler_event.t_edit";

        private DropDown: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private Name: HTMLInputElement;
        private ImplementingAssembly: HTMLInputElement;
        private ImplementingType: HTMLInputElement;
        private ElemImplementingAssembly: HTMLElement;
        private ElemImplementingType: HTMLElement;
        private ElemDescription: HTMLElement;

        constructor(controlId: string) {
            super(controlId, EventEditComponent.TEMPLATE, EventEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: (control: EventEditComponent): string | null => {
                    return control.DropDown.value;
                },
                Enable: (control: EventEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    control.DropDown.enable(enable);
                },
            });

            this.DropDown = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name$='.DropDown']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Control]);
            this.Name = $YetaWF.getElement1BySelector("input[name$='.Name']", [this.Control]) as HTMLInputElement;
            this.ImplementingAssembly = $YetaWF.getElement1BySelector("input[name$='.ImplementingAssembly']", [this.Control]) as HTMLInputElement;
            this.ImplementingType = $YetaWF.getElement1BySelector("input[name$='.ImplementingType']", [this.Control]) as HTMLInputElement;
            this.ElemImplementingAssembly = $YetaWF.getElement1BySelector(".t_implasm", [this.Control]);
            this.ElemImplementingType = $YetaWF.getElement1BySelector(".t_impltype", [this.Control]);
            this.ElemDescription = $YetaWF.getElement1BySelector(".t_description", [this.Control]);

            this.update();

            this.DropDown.Control.addEventListener("dropdownlist_change", (evt: Event): void => {
                this.update();
            });
        }
        private update(): void {
            let val = this.DropDown.value;
            let args = val.split(",");
            this.Name.value = args[0];
            this.ImplementingAssembly.value = args[2];
            this.ImplementingType.value = args[1];

            this.ElemImplementingAssembly.innerText = args[2];
            this.ElemImplementingType.innerText = args[1];

            let tip = this.DropDown.getToolTip(this.DropDown.selectedIndex);
            this.ElemDescription.innerText = tip || "";
        }
    }
}
