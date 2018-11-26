/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    //interface Setup {
    //}

    export class PageSelectionEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly SELECTOR: string = "div.yt_pageselection.t_edit";

        private SelectPage: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private APage: HTMLAnchorElement;
        private DivLink: HTMLDivElement;

        constructor(controlId: string/*, setup: Setup*/) {
            super(controlId);
            //this.Setup = setup;

            this.SelectPage = YetaWF.ComponentBaseDataImpl.getControlFromSelector("select", DropDownListEditComponent.SELECTOR, [this.Control]);
            this.DivLink = $YetaWF.getElement1BySelector(".t_link", [this.Control]) as HTMLDivElement;
            this.APage = $YetaWF.getElement1BySelector("a", [this.DivLink]) as HTMLAnchorElement;

            this.SelectPage.Control.addEventListener("dropdownlist_change", (evt: Event): void => {
                this.updatePage(this.SelectPage.value, "");
            });
            this.updatePage(this.SelectPage.value, "");
        }
        private updatePage(pageGuid: string|undefined|null, desc: string): void {
            if (pageGuid === undefined || pageGuid === null || pageGuid === "" || pageGuid === "00000000-0000-0000-0000-000000000000") {
                desc = "";
                this.DivLink.style.display = "none";
                //$desc.hide();
            } else {
                this.DivLink.style.display = "inline";
                //$desc.show();
            }
            this.APage.href = `/!Page/${pageGuid}`;  // Globals.PageUrl
            //$desc.text(desc);
        }
    }
    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        PageSelectionEditComponent.clearDiv(tag, PageSelectionEditComponent.SELECTOR);
    });
}
