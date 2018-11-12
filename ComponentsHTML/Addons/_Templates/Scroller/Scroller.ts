/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    //interface Setup {
    //}

    export class ScrollerComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly SELECTOR: string = ".yt_scroller.t_display";
        private Panel: number = 0;
        private Panels: number;
        private ElemLeft: HTMLElement;
        private ElemRight: HTMLElement;
        private DivItems: HTMLDivElement;

        constructor(controlId: string/*, setup: Setup*/) {
            super(controlId);
            //this.Setup = setup;

            this.ElemLeft = $YetaWF.getElement1BySelector(".t_left", [this.Control]);
            this.ElemRight = $YetaWF.getElement1BySelector(".t_right", [this.Control]);
            this.Panels = $YetaWF.getElementsBySelector(".t_item", [this.Control]).length;
            this.DivItems = $YetaWF.getElement1BySelector(".t_items", [this.Control]) as HTMLDivElement;

            this.DivItems.style.left = "0px";

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

            //$('.t_items', this.Control).animate({
            //    left: -offs,
            //}, 250, function () { });
        }
    }

    $YetaWF.addWhenReady((tag: HTMLElement): void => {
        var scrollers = $YetaWF.getElementsBySelector(ScrollerComponent.SELECTOR);
        for (let scroller of scrollers) {
            var scr = ScrollerComponent.getControlFromTag<ScrollerComponent>(scroller, ScrollerComponent.SELECTOR);
            scr.updateButtons();
        }
    });

    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        ScrollerComponent.clearDiv(tag, ScrollerComponent.SELECTOR);
    });
}
