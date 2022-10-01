/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

namespace YetaWF_BootstrapCarousel {

    interface CarouselSetup {
        Interval: number;
        Wrap: boolean;
        Keyboard: boolean;
        Pause: boolean;
        ImageCount: number;
    }

    export class CarouselComponent extends YetaWF.ComponentBaseDataImpl {
        public static readonly TEMPLATE: string = "yt_bootstrapcarousel_slideshow";
        public static readonly SELECTOR: string = ".yt_bootstrapcarousel_slideshow.t_display";
        public static readonly SCROLLTIME: number = 300;
        public static readonly STEPS: number = 20;

        private Setup: CarouselSetup;
        private List: HTMLElement;
        private Images: HTMLElement[];
        private currentImage: number = 0;
        private ScrollInterval: number = 0;

        constructor(controlId: string, setup: CarouselSetup) {
            super(controlId, CarouselComponent.TEMPLATE, CarouselComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: null,
                Enable: null,
            }, false, (tag: HTMLElement, control: CarouselComponent): void => {
                control.stopAutoScroll();
            });
            this.Setup = setup;
            this.List = $YetaWF.getElement1BySelector(".t_inner", [this.Control]);
            this.Images = $YetaWF.getElementsBySelector(".t_item", [this.List]);

            this.startAutoScroll();

            $YetaWF.registerEventHandler(this.Control, "click", ".t_prev", (ev: MouseEvent): boolean => {
                this.scroll(false);
                return false;
            });
            $YetaWF.registerEventHandler(this.Control, "click", ".t_next", (ev: MouseEvent): boolean => {
                this.scroll(true);
                return false;
            });
            if (this.Setup.Keyboard) {
                $YetaWF.registerEventHandler(this.Control, "keydown", null, (ev: KeyboardEvent): boolean => {
                    let key = ev.key;
                    if (key === "ArrowRight" || key === "Right") {
                        this.scroll(true);
                        return false;
                    } else if (key === "ArrowLeft" || key === "Left") {
                        this.scroll(false);
                        return false;
                    } else if (key === "Home") {
                        this.setImage(0);
                        return false;
                    } else if (key === "End") {
                        this.setImage(999999);
                        return false;
                    }
                    return true;
                });
            }
            if (this.Setup.Pause) {
                $YetaWF.registerEventHandler(this.Control, "mouseenter", null, (ev: MouseEvent): boolean => {
                    this.stopAutoScroll();
                    return true;
                });
                $YetaWF.registerEventHandler(this.Control, "mouseleave", null, (ev: MouseEvent): boolean => {
                    this.startAutoScroll();
                    return true;
                });
            }
            $YetaWF.registerEventHandler(this.Control, "mousedown", ".t_indicators li", (ev: MouseEvent): boolean => {
                let li = ev.__YetaWFElem;
                let inds = $YetaWF.getElementsBySelector(".t_indicators li", [this.Control]);
                let index = inds.indexOf(li);
                this.setImage(index);
                return true;
            });
        }

        private startAutoScroll(): void {
            if (this.Setup.Interval) {
                this.stopAutoScroll();
                this.ScrollInterval = setInterval((): void => {
                    if (this.ScrollInterval) {
                        this.scroll(true);
                    }
                }, this.Setup.Interval);
            }
        }
        private stopAutoScroll(): void {
            if (this.ScrollInterval) {
                clearInterval(this.ScrollInterval);
                this.ScrollInterval = 0;
            }
        }
        private updateIndicators(): void {
            let inds = $YetaWF.getElementsBySelector(".t_indicators li");
            for (let ind of inds) {
                $YetaWF.elementRemoveClass(ind, "t_active");
            }
            $YetaWF.elementAddClass(inds[this.currentImage], "t_active");
        }

        private startScroll(offset: number): void {

            this.stopAutoScroll();

            let incr = (offset - this.List.scrollLeft) / CarouselComponent.STEPS;
            if (incr === 0) {
                this.List.scrollLeft = offset;
                this.startAutoScroll();
                return;
            }
            let interval = setInterval((): void => {
                let newOffs = this.List.scrollLeft + incr;
                if (incr > 0) {
                    if (newOffs < offset) {
                        this.List.scrollLeft = newOffs;
                        return;
                    }
                } else {
                    if (newOffs > offset) {
                        this.List.scrollLeft = newOffs;
                        return;
                    }
                }
                this.List.scrollLeft = offset;
                clearInterval(interval);
                this.startAutoScroll();

            }, CarouselComponent.SCROLLTIME / CarouselComponent.STEPS + 1);
        }

        // API

        public scroll(next: boolean): void {
            if (next)
                this.setImage(this.currentImage + 1);
            else
                this.setImage(this.currentImage - 1);
        }

        public setImage(index: number): void {
            let nextIndex: number = index;
            if (nextIndex >= this.Setup.ImageCount)
                nextIndex = 0;
            else if (nextIndex < 0)
                nextIndex = this.Setup.ImageCount-1;

            let offset = this.Images[nextIndex].offsetLeft;

            this.startScroll(offset);

            this.currentImage = nextIndex;
            this.updateIndicators();
        }
    }
}

