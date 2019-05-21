/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface Setup {
        AjaxUrl: string;
    }

    export class PageSkinEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_pageskin";
        public static readonly TEMPLATE2: string = "yt_pageskin";
        public static readonly SELECTOR: string = ".yt_pageskin.t_edit, .yt_popupskin.t_edit";

        private Setup: Setup;
        private SelectCollection: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private SelectFile: YetaWF_ComponentsHTML.DropDownListEditComponent;

        constructor(controlId: string, setup: Setup) {
            super(controlId, PageSkinEditComponent.TEMPLATE, PageSkinEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: "dropdownlist_change",
                GetValue: (control: PageSkinEditComponent): string | null => {
                    return null;//$$$control.SelectPage.value;
                },
                Enable: (control: PageSkinEditComponent, enable: boolean): void => {
                    //$$$control.enable(enable)
                },
            });
            this.registerTemplate(PageSkinEditComponent.TEMPLATE2, PageSkinEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: "dropdownlist_change",
                GetValue: (control: PageSkinEditComponent): string | null => {
                    return null;//$$$control.SelectPage.value;
                },
                Enable: (control: PageSkinEditComponent, enable: boolean): void => {
                    //$$$control.enable(enable)
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
    }
}

