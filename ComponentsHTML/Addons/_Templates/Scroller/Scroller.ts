/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    //interface Setup {
    //}

    export class ScrollerComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_scroller";
        public static readonly SELECTOR: string = ".yt_scroller.t_display";

        private Panel: number = 0;
        private Panels: number;
        private ElemLeft: HTMLElement;
        private ElemRight: HTMLElement;
        private DivItems: HTMLDivElement;

        constructor(controlId: string/*, setup: Setup*/) {
            super(controlId, ScrollerComponent.TEMPLATE, ScrollerComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            });
            //this.Setup = setup;

            this.ElemLeft = $YetaWF.getElement1BySelector(".t_left", [this.Control]);
            this.ElemRight = $YetaWF.getElement1BySelector(".t_right", [this.Control]);
            this.Panels = $YetaWF.getElementsBySelector(".t_item", [this.Control]).length;
            this.DivItems = $YetaWF.getElement1BySelector(".t_items", [this.Control]) as HTMLDivElement;

            this.DivItems.style.left = "0px";

            this.updateButtons();

            $YetaWF.registerEventHandler(this.ElemLeft, "click", null, (ev: MouseEvent): boolean => {
                this.scroll(-1);
                return false;
            });
            $YetaWF.registerEventHandler(this.ElemRight, "click", null, (ev: MouseEvent): boolean => {
                this.scroll(1);
                return false;
            });
        }

        // API

        public updateButtons(): void {

            this.ElemLeft.style.backgroundPosition = this.Panel === 0 ? "0px 0px" : "0px -48px";
            $YetaWF.elementEnableToggle(this.ElemLeft, this.Panel !== 0);

            var controlRect = this.Control.getBoundingClientRect();
            var width = controlRect.width;

            var itemRect = $YetaWF.getElement1BySelector(".t_item", [this.Control]).getBoundingClientRect();
            var itemwidth = itemRect.width;

            var skip = Math.floor(width / itemwidth);
            this.ElemRight.style.backgroundPosition = this.Panel + skip < this.Panels ? "-48px -48px" : "-48px 0px";
            $YetaWF.elementEnableToggle(this.ElemRight, this.Panel + skip < this.Panels);
        }
        public scroll(direction: number): void {

            var controlRect = this.Control.getBoundingClientRect();
            var width = controlRect.width;

            var itemRect = $YetaWF.getElement1BySelector(".t_item", [this.Control]).getBoundingClientRect();
            var itemwidth = itemRect.width;

            var skip = Math.floor(width / itemwidth);
            if (skip < 1) skip = 1;
            this.Panel = this.Panel + skip * direction;
            //if (this.Panel >= this.Panels - skip) this.Panel %= this.Panels;
            //if (this.Panel < 0) this.Panel = this.Panels + this.Panel;
            if (this.Panel >= this.Panels) this.Panel = this.Panels - skip;
            if (this.Panel < 0) this.Panel = 0;

            this.updateButtons();

            var offs = this.Panel * itemwidth;
            this.DivItems.style.transition = "all 250ms";
            this.DivItems.style.left = `${-offs}px`;
        }
    }
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERRESIZE, null, (ev: CustomEvent<YetaWF.DetailsEventContainerResize>): boolean => {
        let scrolls = YetaWF.ComponentBaseDataImpl.getControls<ScrollerComponent>(ScrollerComponent.SELECTOR, [ev.detail.container]);
        for (let scroll of scrolls) {
            scroll.updateButtons();
        }
        return true;
    });
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.BasicsServices.EVENTCONTAINERSCROLL, null, (ev: CustomEvent<YetaWF.DetailsEventContainerScroll>): boolean => {
        let scrolls = YetaWF.ComponentBaseDataImpl.getControls<ScrollerComponent>(ScrollerComponent.SELECTOR, [ev.detail.container]);
        for (let scroll of scrolls) {
            scroll.updateButtons();
        }
        return true;
    });
}
