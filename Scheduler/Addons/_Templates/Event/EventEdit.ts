/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

namespace YetaWF_Scheduler {

    export class EventEditComponent {

        private Control: HTMLElement;
        private DropDown: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private Name: HTMLInputElement;
        private ImplementingAssembly: HTMLInputElement;
        private ImplementingType: HTMLInputElement;
        private ElemImplementingAssembly: HTMLElement;
        private ElemImplementingType: HTMLElement;
        private ElemDescription: HTMLElement;

        constructor(controlId: string) {
            this.Control = $YetaWF.getElementById(controlId) as HTMLElement;
            this.DropDown = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector("select[name$='.DropDown']", [this.Control]);
            this.Name = $YetaWF.getElement1BySelector("input[name$='.Name']", [this.Control]) as HTMLInputElement;
            this.ImplementingAssembly = $YetaWF.getElement1BySelector("input[name$='.ImplementingAssembly']", [this.Control]) as HTMLInputElement;
            this.ImplementingType = $YetaWF.getElement1BySelector("input[name$='.ImplementingType']", [this.Control]) as HTMLInputElement;
            this.ElemImplementingAssembly = $YetaWF.getElement1BySelector(".t_implasm", [this.Control]);
            this.ElemImplementingType = $YetaWF.getElement1BySelector(".t_impltype", [this.Control]);
            this.ElemDescription = $YetaWF.getElement1BySelector(".t_description", [this.Control]);

            this.DropDown.Control.addEventListener("dropdownlist_change", (evt: Event): void => {
                var args = this.DropDown.value.split(",");
                this.Name.value = args[0];
                this.ImplementingAssembly.value = args[2];
                this.ImplementingType.value = args[1];

                this.ElemImplementingAssembly.innerText = args[2];
                this.ElemImplementingType.innerText = args[1];

                var tip = this.DropDown.getToolTip(this.DropDown.selectedIndex);
                this.ElemDescription.innerText = tip || "";
            });
        }
    }
}
