/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface Setup {
        AjaxUrl: string;
    }

    export class PageSkinEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_pageskin";
        public static readonly SELECTOR: string = ".yt_pageskin.t_edit";

        private Setup: Setup;
        private SelectCollection: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private SelectFile: YetaWF_ComponentsHTML.DropDownListEditComponent;

        constructor(controlId: string, setup: Setup) {
            super(controlId, PageSkinEditComponent.TEMPLATE, PageSkinEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: (control: PageSkinEditComponent): string | null => {
                    return control.SelectFile.value;
                },
                Enable: (control: PageSkinEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                },
            });

            this.Setup = setup;

            this.SelectCollection = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name$='.Collection']", DropDownListEditComponent.SELECTOR, [this.Control]);
            this.SelectFile = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name$='.FileName']", DropDownListEditComponent.SELECTOR, [this.Control]);

            this.SelectCollection.Control.addEventListener("dropdownlist_change", (evt: Event): void => {
                var data = { SkinCollection: this.SelectCollection.value };
                this.SelectFile.ajaxUpdate(data, this.Setup.AjaxUrl);
            });
        }
        public enable(enabled: boolean): void {
            this.SelectCollection.enable(enabled);
            this.SelectFile.enable(enabled);
        }
    }
}

