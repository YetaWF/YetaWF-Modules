/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

namespace YetaWF_Panels {

    interface Setup {
        Resize: boolean;
        ActiveCss: string;
    }

    export class PageBarInfoComponent extends YetaWF.ComponentBaseDataImpl {

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
            this.resize();

            // Link click, activate entry
            $YetaWF.registerEventHandler(this.Control, "click", ".yt_panels_pagebarinfo_list a", (ev: MouseEvent): boolean => {
                let entry = $YetaWF.elementClosestCond(ev.__YetaWFElem, ".yt_panels_pagebarinfo_list .t_entry");
                if (!entry)
                    return true;
                let entries = $YetaWF.getElementsBySelector(".yt_panels_pagebarinfo_list .t_entry", [this.Control]);
                for (let e of entries)
                    $YetaWF.elementRemoveClassList(e, this.Setup.ActiveCss);
                $YetaWF.elementAddClassList(entry, this.Setup.ActiveCss);
                return true;
            });
        }
        public resize(): void {
            if (!this.Setup.Resize) return;
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
    ($(window) as any).smartresize((): void => {
        let ctrlDivs = $YetaWF.getElementsBySelector(PageBarInfoComponent.SELECTOR);
        for (let ctrlDiv of ctrlDivs) {
            let mod = PageBarInfoComponent.getControlFromTag<PageBarInfoComponent>(ctrlDiv, PageBarInfoComponent.SELECTOR);
            mod.resize();
        }
    });

}
