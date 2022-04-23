/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface Setup {
        AjaxUrl: string;
    }
    interface Lists {
        PagesHTML: string;
        PopupsHTML: string;
    }

    export class SkinEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_skin";
        public static readonly SELECTOR: string = ".yt_skin.t_edit";

        private Setup: Setup;
        private SelectCollection: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private SelectPageFile: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private SelectPopupFile: YetaWF_ComponentsHTML.DropDownListEditComponent;

        constructor(controlId: string, setup: Setup) {
            super(controlId, SkinEditComponent.TEMPLATE, SkinEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: (control: SkinEditComponent): string | null => { return null; },
                Enable: (control: SkinEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                },
            });

            this.Setup = setup;

            this.SelectCollection = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name$='.Collection']", DropDownListEditComponent.SELECTOR, [this.Control]);
            this.SelectPageFile = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name$='.PageFileName']", DropDownListEditComponent.SELECTOR, [this.Control]);
            this.SelectPopupFile = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select[name$='.PopupFileName']", DropDownListEditComponent.SELECTOR, [this.Control]);

            this.SelectCollection.Control.addEventListener(YetaWF_ComponentsHTML.DropDownListEditComponent.EVENTCHANGE, (evt: Event): void => {
                let data = { SkinCollection: this.SelectCollection.value };

                let uri = $YetaWF.parseUrl(this.Setup.AjaxUrl);
                uri.addSearchSimpleObject(data);

                $YetaWF.post(uri.toUrl(), uri.toFormData(), (success: boolean, lists: Lists): void =>{
                    if (success) {
                        this.SelectPageFile.setOptionsHTML(lists.PagesHTML);
                        this.SelectPopupFile.setOptionsHTML(lists.PopupsHTML);
                    }
                });
            });
        }
        public enable(enabled: boolean): void {
            this.SelectCollection.enable(enabled);
            this.SelectPageFile.enable(enabled);
            this.SelectPopupFile.enable(enabled);
        }
    }
}

