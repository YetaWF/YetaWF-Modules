/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

namespace YetaWF_Panels {

    export enum StyleEnum {
        Tabs = 0,
        AccordionjQuery = 1,
        AccordionKendo = 2,
    }
    interface Setup {
        Style: StyleEnum;
        ActiveIndex: number;
    }

    export class PanelInfoComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_panels_panelinfo";
        public static readonly SELECTOR: string = ".yt_panels_panelinfo.t_display";
        public static TEMPLATENAME: string = "YetaWF_Panels_PanelInfo";

        private static MAXTIME: number = 0.6;
        private static INCRTIME: number = 0.03;

        private Setup: Setup;

        constructor(controlId: string, setup: Setup) {
            super(controlId, PanelInfoComponent.TEMPLATE, PanelInfoComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });
            this.Setup = setup;

            if (this.Setup.Style === StyleEnum.AccordionKendo) {
                $YetaWF.registerEventHandler(this.Control, "click", "ul.t_acckendo > li", (ev: MouseEvent): boolean => {
                    let liActive = ev.__YetaWFElem as HTMLLIElement;
                    let contentActive = $YetaWF.getElement1BySelector(".k-content", [liActive]);
                    let activeShown = this.isShown(contentActive);
                    let lis = $YetaWF.getElementsBySelector("ul.t_acckendo > li", [this.Control]) as HTMLLIElement[];
                    for (let li of lis) {
                        let liContent = $YetaWF.getElement1BySelector(".k-content", [li]);
                        this.khide(li, liContent);
                    }
                    if (!activeShown)
                        this.kshow(liActive, contentActive);
                    return false;
                });
            } else if (this.Setup.Style === StyleEnum.AccordionjQuery) {
                $YetaWF.registerEventHandler(this.Control, "click", "h3", (ev: MouseEvent): boolean => {
                    let hActive = ev.__YetaWFElem as HTMLElement;
                    let contentActive = hActive.nextElementSibling as HTMLElement;
                    let activeShown = this.isShown(contentActive);
                    let headers = $YetaWF.getElementsBySelector("h3", [this.Control]) as HTMLLIElement[];
                    for (let header of headers)
                        this.jhide(header, header.nextElementSibling as HTMLElement);
                    if (!activeShown)
                        this.jshow(hActive, hActive.nextElementSibling as HTMLElement);
                    return false;
                });
            }

            // Kendo hover
            $YetaWF.registerEventHandler(this.Control, "mousemove", "ul.t_acckendo > li .k-header", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem as HTMLLIElement;
                $YetaWF.elementAddClass(li, "k-state-hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.Control, "mouseout", "ul.t_acckendo > li .k-header", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem as HTMLLIElement;
                $YetaWF.elementRemoveClass(li, "k-state-hover");
                return true;
            });

            // jqueryui hover
            $YetaWF.registerEventHandler(this.Control, "mousemove", ".t_accjquery > h3", (ev: MouseEvent): boolean => {
                let header = ev.__YetaWFElem as HTMLElement;
                $YetaWF.elementAddClass(header, "ui-state-hover");
                return true;
            });
            $YetaWF.registerEventHandler(this.Control, "mouseout", ".t_accjquery > h3", (ev: MouseEvent): boolean => {
                let header = ev.__YetaWFElem as HTMLElement;
                $YetaWF.elementRemoveClass(header, "ui-state-hover");
                return true;
            });
        }

        // Kendo

        private khide(li: HTMLLIElement, tag: HTMLElement): void {
            this.collapse(tag);
            $YetaWF.setAttribute(tag, "aria-hidden", "true");

            $YetaWF.elementRemoveClasses(li, ["k-state-active", "k-state-highlight"]);
            $YetaWF.setAttribute(li, "aria-expanded", "false");
            $YetaWF.setAttribute(li, "aria-selected", "false");

            let header = $YetaWF.getElement1BySelector(".k-header", [li]);
            $YetaWF.elementRemoveClass(header, "k-state-selected");
            let icon = $YetaWF.getElement1BySelector(".k-icon", [li]);
            $YetaWF.elementRemoveClasses(icon, ["k-i-arrow-60-up", "k-panelbar-collapse", "k-i-arrow-60-down", "k-panelbar-expand"]);
            $YetaWF.elementAddClasses(icon, ["k-i-arrow-60-down", "k-panelbar-expand"]);
        }
        private kshow(li: HTMLLIElement, tag: HTMLElement): void {
            $YetaWF.setAttribute(tag, "aria-hidden", "false");
            this.expand(tag);

            $YetaWF.elementRemoveClasses(li, ["k-state-active", "k-state-highlight"]);
            $YetaWF.elementAddClasses(li, ["k-state-active", "k-state-highlight"]);
            $YetaWF.setAttribute(li, "aria-expanded", "true");
            $YetaWF.setAttribute(li, "aria-selected", "true");

            let header = $YetaWF.getElement1BySelector(".k-header", [li]);
            $YetaWF.elementRemoveClass(header, "k-state-selected");
            $YetaWF.elementAddClass(header, "k-state-selected");
            let icon = $YetaWF.getElement1BySelector(".k-icon", [li]);
            $YetaWF.elementRemoveClasses(icon, ["k-i-arrow-60-up", "k-panelbar-collapse", "k-i-arrow-60-down", "k-panelbar-expand"]);
            $YetaWF.elementAddClasses(icon, ["k-i-arrow-60-up", "k-panelbar-collapse"]);
        }
        private isShown(tag: HTMLElement): boolean {
            return tag.style.display === "block" || tag.style.display === "";
        }

        // jQuery-ui

        private jhide(header: HTMLElement, tag: HTMLElement): void {
            this.collapse(tag);

            $YetaWF.elementRemoveClasses(header, ["ui-accordion-header-active", "ui-state-active", "ui-accordion-header-collapsed", "ui-corner-all"]);
            $YetaWF.elementAddClasses(header, ["ui-accordion-header-collapsed", "ui-corner-all"]);
            $YetaWF.setAttribute(header, "aria-expanded", "false");
            $YetaWF.setAttribute(header, "aria-selected", "false");
            $YetaWF.setAttribute(header, "tabindex", "-1");

            $YetaWF.elementRemoveClass(tag, "ui-accordion-content-active");
            $YetaWF.setAttribute(tag, "aria-hidden", "true");
        }
        private jshow(header: HTMLElement, tag: HTMLElement): void {
            this.expand(tag);

            $YetaWF.elementRemoveClasses(header, ["ui-accordion-header-active", "ui-state-active", "ui-accordion-header-collapsed", "ui-corner-all"]);
            $YetaWF.elementAddClasses(header, ["ui-accordion-header-active", "ui-state-active"]);
            $YetaWF.setAttribute(header, "aria-expanded", "true");
            $YetaWF.setAttribute(header, "aria-selected", "true");
            $YetaWF.setAttribute(header, "tabindex", "0");

            $YetaWF.elementRemoveClass(tag, "ui-accordion-content-active");
            $YetaWF.elementAddClass(tag, "ui-accordion-content-active");
            $YetaWF.setAttribute(tag, "aria-hidden", "false");
        }

        // helper

        private expand(tag: HTMLElement): void {
            let steps = PanelInfoComponent.MAXTIME / PanelInfoComponent.INCRTIME;
            let incr = window.innerHeight / steps;
            let height = incr;
            tag.style.maxHeight = `${height}px`;
            tag.style.display = "block";
            const timer = setInterval((): void => {
                let rect = tag.getBoundingClientRect();
                height += incr;
                tag.style.maxHeight = `${height}px`;
                let newRect = tag.getBoundingClientRect();
                if (rect.height >= newRect.height) {
                    clearInterval(timer);
                    $YetaWF.sendActivateDivEvent([tag]);
                }
            }, PanelInfoComponent.INCRTIME);

        }
        private collapse(tag: HTMLElement): void {
            let steps = PanelInfoComponent.MAXTIME / PanelInfoComponent.INCRTIME;
            let rect = tag.getBoundingClientRect();
            let incr = rect.height / steps;
            let height = rect.height - incr;
            if (height <= 0)
                height = 0;
            tag.style.maxHeight = `${height}px`;
            if (height > 0) {
                const timer = setInterval((): void => {
                    let rect = tag.getBoundingClientRect();
                    height -= incr;
                    if (height <= 0)
                        height = 0;
                    tag.style.maxHeight = `${height}px`;
                    let newRect = tag.getBoundingClientRect();
                    if (rect.height <= newRect.height || rect.height <= 0) {
                        clearInterval(timer);
                        tag.style.display = "none";
                    }
                }, PanelInfoComponent.INCRTIME);
            }
        }
    }
}