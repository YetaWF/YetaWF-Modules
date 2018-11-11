/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface Setup {
        AjaxUrl: string;
    }

    export class PageSkinEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly SELECTOR: string = ".yt_pageskin.t_edit, .yt_popupskin.t_edit";

        private Setup: Setup;
        private SelectCollection: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private SelectFile: YetaWF_ComponentsHTML.DropDownListEditComponent;

        constructor(controlId: string, setup: Setup) {
            super(controlId);
            this.Setup = setup;

            this.SelectCollection = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector("select[name$='.Collection']", [this.Control]);
            this.SelectFile = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector("select[name$='.FileName']", [this.Control]);

            this.SelectCollection.Control.addEventListener("dropdownlist_change", (evt: Event): void => {
                var data = { SkinCollection: this.SelectCollection.value };
                this.SelectFile.ajaxUpdate(data, this.Setup.AjaxUrl);
            });
        }
    }

    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        PageSkinEditComponent.clearDiv(tag, PageSkinEditComponent.SELECTOR);
    });
}

