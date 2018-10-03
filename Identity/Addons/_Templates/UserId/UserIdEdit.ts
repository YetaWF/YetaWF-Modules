/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

namespace YetaWF_Identity {

    interface UserIdSetup {
        GridAllId: string;
        HiddenId: string;
        NameId: string;
        NoUser: string;
    }

    export class UserIdEditComponent extends YetaWF.ComponentBase<HTMLDivElement> {

        private Setup: UserIdSetup;
        private GridAll: YetaWF_ComponentsHTML.Grid;
        private buttonClear: HTMLImageElement;
        private inputHidden: HTMLInputElement;
        private inputName: HTMLInputElement;

        constructor(controlId: string, setup: UserIdSetup) {
            super(controlId);
            this.Setup = setup;

            this.GridAll = YetaWF_ComponentsHTML.Grid.getControlById(this.Setup.GridAllId);
            this.inputHidden = $YetaWF.getElementById(this.Setup.HiddenId) as HTMLInputElement;
            this.inputName = $YetaWF.getElementById(this.Setup.NameId) as HTMLInputElement;
            this.buttonClear = $YetaWF.getElement1BySelector(".t_clear", [this.Control]) as HTMLImageElement;

            $YetaWF.registerEventHandler(this.buttonClear, "click", null, (ev: MouseEvent): boolean => {
                this.inputHidden.value = "0";
                this.inputName.value = "";
                return false;
            });
            this.GridAll.Control.addEventListener("grid_selectionchange", (evt: Event): void => {
                var index = this.GridAll.SelectedIndex();
                var tr = this.GridAll.GetTR(index);
                var tdName = tr.children[0] as HTMLTableCellElement;
                var inputUserId = $YetaWF.getElement1BySelector("input[name$='.UserId']", [tdName]) as HTMLInputElement;
                var name = tdName.innerText.trim();
                this.inputName.value = name;
                this.inputHidden.value = inputUserId.value;
            });
        }
    }
}

