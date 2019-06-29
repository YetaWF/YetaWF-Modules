/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

namespace YetaWF_Identity {

    interface UserIdSetup {
        GridAllId: string;
        HiddenId: string;
        NameId: string;
        NoUser: string;
    }

    export class UserIdEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_yetawf_identity_userid";
        public static readonly SELECTOR: string = ".yt_yetawf_identity_userid.t_large.t_edit";

        private Setup: UserIdSetup;
        private GridAll: YetaWF_ComponentsHTML.Grid;
        private buttonClear: HTMLImageElement;
        private inputHidden: HTMLInputElement;
        private inputName: HTMLInputElement;

        constructor(controlId: string, setup: UserIdSetup) {
            super(controlId, UserIdEditComponent.TEMPLATE, UserIdEditComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,//return control.inputHidden.value;
                Enable: null,
            });

            this.Setup = setup;

            this.GridAll = YetaWF.ComponentBaseDataImpl.getControlById(this.Setup.GridAllId, YetaWF_ComponentsHTML.Grid.SELECTOR);
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
                if (index < 0) return;
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

