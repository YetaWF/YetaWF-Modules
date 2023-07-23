/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

namespace YetaWF_Panels {

    export enum StyleEnum {
        Tabs = 0,
        Accordion = 1,
    }
    interface Setup {
        Style: StyleEnum;
        ActiveIndex: number;
    }

    export class PanelInfoComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_panels_panelinfo";
        public static readonly SELECTOR: string = ".yt_panels_panelinfo.t_display";
        public static TEMPLATENAME: string = "YetaWF_Panels_PanelInfo";

        private Setup: Setup;

        constructor(controlId: string, setup: Setup) {
            super(controlId, PanelInfoComponent.TEMPLATE, PanelInfoComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });
            this.Setup = setup;

            if (this.Setup.Style === StyleEnum.Accordion) {
                $YetaWF.registerEventHandler(this.Control, "click", "h2.t_accheader", (ev: MouseEvent): boolean => {
                    let hActive = ev.__YetaWFElem as HTMLElement;
                    let contentActive = hActive.nextElementSibling as HTMLElement;
                    let activeShown = this.isShown(contentActive);
                    let headers = $YetaWF.getElementsBySelector("h2.t_accheader", [this.Control]) as HTMLLIElement[];
                    for (let header of headers)
                        this.hide(header, header.nextElementSibling as HTMLElement);
                    if (!activeShown)
                        this.show(hActive, hActive.nextElementSibling as HTMLElement);
                    return false;
                });
            }
        }

        private hide(header: HTMLElement, tag: HTMLElement): void {
            $YetaWF.elementRemoveClasses(header, ["t_active", "t_collapsed"]);
            $YetaWF.elementAddClass(header, "t_collapsed");
            $YetaWF.setAttribute(header, "aria-expanded", "false");
            $YetaWF.setAttribute(header, "aria-selected", "false");
            $YetaWF.setAttribute(header, "tabindex", "-1");

            $YetaWF.elementRemoveClass(tag, "t_active");
            $YetaWF.setAttribute(tag, "aria-hidden", "true");

            $YetaWF.animateHeight(tag, false, (): void => {
                tag.style.display = "none";
            });
        }
        private show(header: HTMLElement, tag: HTMLElement): void {
            $YetaWF.elementRemoveClasses(header, ["t_active", "t_collapsed"]);
            $YetaWF.elementAddClass(header, "t_active");
            $YetaWF.setAttribute(header, "aria-expanded", "true");
            $YetaWF.setAttribute(header, "aria-selected", "true");
            $YetaWF.setAttribute(header, "tabindex", "0");

            $YetaWF.elementRemoveClass(tag, "t_active");
            $YetaWF.elementAddClass(tag, "t_active");
            $YetaWF.setAttribute(tag, "aria-hidden", "false");

            $YetaWF.animateHeight(tag, true);
        }

        private isShown(tag: HTMLElement): boolean {
            return tag.style.display === "block" || tag.style.display === "";
        }
    }
}