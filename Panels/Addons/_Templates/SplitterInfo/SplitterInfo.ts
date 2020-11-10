/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

namespace YetaWF_Panels {

    interface Setup {
        Height: number;     // 0 (auto-fill), or pixels
        MinWidth: number;   // pixels
        Width: number;      // percentage
    }

    export class SplitterInfoComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_panels_splitterinfo";
        public static readonly SELECTOR: string = ".yt_panels_splitterinfo.t_display";
        public static TEMPLATENAME: string = "YetaWF_Panels_SplitterInfo";

        private readonly SMALLSCREEN: number = 900;

        private Setup: Setup;
        private Left: HTMLElement;
        private Right: HTMLElement;
        private Collapse: HTMLElement;
        private CollapseText: HTMLElement;
        private Expand: HTMLElement;
        private Resize: HTMLElement;
        private static ResizeSplitter: SplitterInfoComponent | null = null;

        constructor(controlId: string, setup: Setup) {
            super(controlId, SplitterInfoComponent.TEMPLATE, SplitterInfoComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });
            this.Setup = setup;

            this.Left = $YetaWF.getElement1BySelector(".yt_panels_splitterinfo_left", [this.Control]);
            this.Right = $YetaWF.getElement1BySelector(".yt_panels_splitterinfo_right", [this.Control]);
            this.Collapse = $YetaWF.getElement1BySelector(".yt_panels_splitterinfo_coll", [this.Control]);
            this.CollapseText = $YetaWF.getElement1BySelector(".yt_panels_splitterinfo_colldesc", [this.Control]);
            this.Expand = $YetaWF.getElement1BySelector(".yt_panels_splitterinfo_exp", [this.Control]);
            this.Resize = $YetaWF.getElement1BySelector(".yt_panels_splitterinfo_resize", [this.Control]);

            this.resized();

            // expand/collapse
            $YetaWF.registerMultipleEventHandlers([this.Collapse, this.CollapseText, this.Expand], ["click"], null, (ev: Event): boolean => {
                this.toggleExpandCollapse();
                return false;
            });
            $YetaWF.registerEventHandler(this.Resize, "mousedown", null, (ev: MouseEvent): boolean => {
                document.body.style.cursor = "col-resize";
                SplitterInfoComponent.ResizeSplitter = this;
                window.addEventListener("mousemove", SplitterInfoComponent.resizeWidth, false);
                window.addEventListener("mouseup", SplitterInfoComponent.resizeWidthDone, false);
                return false;
            });

        }

        public resized(): void { /* main window resized, reposition */
            if (this.Setup.Height === 0) {
                if ($YetaWF.isPrinting) {
                    this.Control.style.height = "";
                } else {
                    // Resize in height so we fill the remaining page height
                    // While this is possible in css also, it can't be done without knowing the structure of the page, which we can't assume in this component
                    // so we just do it at load time (and when the window is resized).

                    let winHeight = window.innerHeight;
                    let winWidth = window.innerWidth;

                    if (winWidth >= this.SMALLSCREEN) {

                        window.scrollTo(0,0);

                        let ctrlRect = this.Control.getBoundingClientRect();

                        let ctrlHeight = winHeight - ctrlRect.top;
                        if (ctrlHeight < 0) ctrlHeight = 0;
                        this.Control.style.height = `${ctrlHeight}px`;
                        this.Left.style.flexBasis = `${this.Setup.Width}%`;

                    } else {
                        this.Control.style.height = "100%";

                        if ($YetaWF.elementHasClass(this.Control, "t_expanded"))
                            this.Left.style.flexBasis = "100%";

                    }
                }
            }
        }

        public toggleExpandCollapse(): void {
            if ($YetaWF.elementHasClass(this.Control, "t_expanded")) {
                $YetaWF.elementRemoveClass(this.Control, "t_expanded");
            } else {
                $YetaWF.elementAddClass(this.Control, "t_expanded");
            }
            $YetaWF.sendContainerResizeEvent(this.Left);
            $YetaWF.sendContainerResizeEvent(this.Right);
        }

        public collapseSmallScreen(): void {
            let winWidth = window.innerWidth;
            if (winWidth <= this.SMALLSCREEN) {
                $YetaWF.elementRemoveClass(this.Control, "t_expanded");
                $YetaWF.sendContainerResizeEvent(this.Left);
                $YetaWF.sendContainerResizeEvent(this.Right);
            }
        }

        private static resizeWidth(ev: MouseEvent): boolean {
            let ctrl = SplitterInfoComponent.ResizeSplitter;
            if (!ctrl) return false;
            let rect = ctrl.Left.getBoundingClientRect();
            let newActualWidth = ev.clientX - rect.left;
            if (newActualWidth < ctrl.Setup.MinWidth)
                newActualWidth = ctrl.Setup.MinWidth;

            ctrl.Left.style.flexBasis = `${newActualWidth}px`;
            return false;
        }
        private static resizeWidthDone(ev: MouseEvent): boolean {
            let ctrl = SplitterInfoComponent.ResizeSplitter;
            if (!ctrl) return false;
            document.body.style.cursor = "default";
            window.removeEventListener("mousemove", SplitterInfoComponent.resizeWidth, false);
            window.removeEventListener("mouseup", SplitterInfoComponent.resizeWidthDone, false);
            $YetaWF.sendContainerResizeEvent(ctrl.Left);
            $YetaWF.sendContainerResizeEvent(ctrl.Right);
            return false;
        }
    }

    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: CustomEvent<YetaWF.DetailsEventContainerResize>): boolean => {
        let ctrlDivs = $YetaWF.getElementsBySelector(SplitterInfoComponent.SELECTOR);
        for (let ctrlDiv of ctrlDivs) {
            if ($YetaWF.elementHas(ev.detail.container, ctrlDiv)) {
                let ctrl = SplitterInfoComponent.getControlFromTag<SplitterInfoComponent>(ctrlDiv, SplitterInfoComponent.SELECTOR);
                ctrl.resized();
            }
        }
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, (ev: CustomEvent<YetaWF.DetailsEventContainerScroll>): boolean => {
        let ctrlDivs = $YetaWF.getElementsBySelector(SplitterInfoComponent.SELECTOR);
        for (let ctrlDiv of ctrlDivs) {
            if ($YetaWF.elementHas(ev.detail.container, ctrlDiv)) {
                let ctrl = SplitterInfoComponent.getControlFromTag<SplitterInfoComponent>(ctrlDiv, SplitterInfoComponent.SELECTOR);
                ctrl.resized();
            }
        }
        return true;
    });
}