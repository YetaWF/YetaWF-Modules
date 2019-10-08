/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

namespace YetaWF_Panels {

    interface Setup {
        Resize: boolean;
        ActiveCss: string;
    }

    export class PageBarInfoComponent extends YetaWF.ComponentBaseNoDataImpl {

        public static readonly TEMPLATE: string = "yt_panels_pagebarinfo";
        public static readonly SELECTOR: string = ".yt_panels_pagebarinfo.t_display";

        private Setup: Setup;

        constructor(controlId: string, setup: Setup) {
            super(controlId, PageBarInfoComponent.TEMPLATE, PageBarInfoComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });
            this.Setup = setup;
            if (this.Setup.Resize)
                this.resize();

            // Link click, activate entry
            $YetaWF.registerEventHandler(this.Control, "click", ".t_list a", (ev: MouseEvent): boolean => {
                let entries = $YetaWF.getElementsBySelector(".t_entry", [this.Control]);
                for (let entry of entries)
                    $YetaWF.elementRemoveClassList(entry, this.Setup.ActiveCss);
                let entry = $YetaWF.elementClosest(ev.__YetaWFElem, ".t_entry");
                $YetaWF.elementAddClassList(entry, this.Setup.ActiveCss);
                return true;
            });
            if (this.Setup.Resize) {
                ($(window) as any).smartresize((): void => {
                    this.resize();
                });
            }
        }
        private resize(): void {
            // Resize the page bar in height so we fill the remaining page height
            // While this is possible in css also, it can't be done without knowing the structure of the page, which we can't assume in this page bar
            // so we just do it at load time (and when the window is resized).
            let winHeight = window.innerHeight;
            let docRect = document.body.getBoundingClientRect();

            let h = docRect.height - winHeight;
            let ctrlRect = this.Control.getBoundingClientRect();
            this.Control.style.height = `${ctrlRect.height - h}px`;
        }
    }
}
