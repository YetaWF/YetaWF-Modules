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
                this.activateEntry(ev.__YetaWFElem);
                return true;
            });
            // keyboard
            $YetaWF.registerEventHandler(this.Control, "keydown", ".yt_panels_pagebarinfo_list", (ev: KeyboardEvent): boolean => {
                let index = this.activeEntry;
                if (index < 0) index = 0;
                let key = ev.key;
                if (key === "ArrowDown" || key === "Down" || key === "ArrowRight" || key === "Right") {
                    ++index;
                } else if (key === "ArrowUp" || key === "Up" || key === "ArrowLeft" || key === "Left") {
                    --index;
                } else if (key === "Home") {
                    index = 0;
                } else if (key === "End") {
                    index = this.count - 1;
                } else
                    return true;
                if (index >= 0 && index < this.count) {
                    this.activeEntry = index;
                    return false;
                }
                return true;
            });
            // scrolling
            $YetaWF.registerEventHandler($YetaWF.getElement1BySelector(".t_area", [this.Control]), "scroll", null, (ev: Event): boolean => {
                $YetaWF.sendContainerScrollEvent(this.Control);
                return true;
            });
        }

        private activateEntry(tag: HTMLElement): void {
            let entry = $YetaWF.elementClosestCond(tag, ".yt_panels_pagebarinfo_list .t_entry");
            if (!entry)
                return;
            let entries = $YetaWF.getElementsBySelector(".yt_panels_pagebarinfo_list .t_entry", [this.Control]);
            for (let e of entries)
                $YetaWF.elementRemoveClassList(e, this.Setup.ActiveCss);
            $YetaWF.elementAddClassList(entry, this.Setup.ActiveCss);
            let anchor = $YetaWF.getElement1BySelector(".t_link a", [entry]);
            anchor.focus();
            anchor.click();
        }

        public get count(): number {
            return this.entries.length;
        }
        public get entries(): HTMLElement[] {
            return $YetaWF.getElementsBySelector(".yt_panels_pagebarinfo_list .t_entry", [this.Control]);
        }
        public get activeEntry(): number {
            let entries = this.entries;
            let active = $YetaWF.getElement1BySelectorCond(`.yt_panels_pagebarinfo_list .t_entry.t_active`, [this.Control]);
            if (!active)
                return -1;
            let index = entries.indexOf(active);
            return index;
        }
        public set activeEntry(index: number) {
            let entries = this.entries;
            if (index < 0 || index >= entries.length) throw `Panel index ${index} is invalid`;
            this.activateEntry(entries[index]);
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
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: CustomEvent<YetaWF.DetailsEventContainerResize>): boolean => {
        let ctrlDivs = $YetaWF.getElementsBySelector(PageBarInfoComponent.SELECTOR);
        for (let ctrlDiv of ctrlDivs) {
            if ($YetaWF.elementHas(ev.detail.container, ctrlDiv)) {
                let mod = PageBarInfoComponent.getControlFromTag<PageBarInfoComponent>(ctrlDiv, PageBarInfoComponent.SELECTOR);
                mod.resize();
            }
        }
        return true;
    });

}
