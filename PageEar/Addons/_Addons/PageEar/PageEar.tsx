/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEar#License */
/* eslint-disable @typescript-eslint/indent */
/*!
 * jQuery Peelback ported to TypeScript - Original by Rob Flaherty
 */

namespace YetaWF_PageEar {

    interface Setup {
        AdImage: string|null;
        PeelImage: string|null;
        ClickURL: string|null;
        SmallSize: number;
        LargeSize: number;
        AutoAnimate: boolean;
    }

    export class PageEarModule extends YetaWF.ModuleBaseDataImpl {

        public static readonly SELECTOR: string = ".YetaWF_PageEar_PageEar";

        private static readonly TIME:number = 400; // time to grow/shrink
        private static readonly STEPTIME:number = 15; // time for 1 step

        private Setup: Setup;

        private PeelDiv: HTMLDivElement;
        private PeelImage: HTMLImageElement;
        private PeelMask: HTMLDivElement;
        private Interval: number = 0;
        private Increase: boolean = false;
        private Increment: number;

        constructor(id: string, setup: Setup) {
            super(id, PageEarModule.SELECTOR, null, (tag: HTMLElement, module: PageEarModule) : void => {
                // when the page is removed, we need to clean up
                module.PeelDiv.remove();
            });
            this.Setup = setup;

            if (!this.Setup.AdImage) throw "Ad image missing";
            if (!this.Setup.PeelImage) throw "Peel effect image missing";
            if (!this.Setup.ClickURL) throw "Click URL missing";

            this.PeelDiv =  <div id="YetaWF_PageEar_Peelback">
                                <a href={this.Setup.ClickURL} class="yNoToolTip" target="_blank" rel="noopener noreferrer">
                                    <img class="t_img" src={this.Setup.PeelImage} alt="" border="0" />
                                </a>
                                <div class="t_mask"></div>
                            </div> as HTMLDivElement;

            this.PeelImage = $YetaWF.getElement1BySelector(".t_img", [this.PeelDiv]) as HTMLImageElement;

            this.PeelMask = $YetaWF.getElement1BySelector(".t_mask", [this.PeelDiv]) as HTMLDivElement;
            this.PeelMask.style.backgroundImage = `url('${this.Setup.AdImage}')`;

            document.body.prepend(this.PeelDiv);

            this.PeelImage.style.width = `${this.Setup.SmallSize}px`;
            this.PeelImage.style.height = `${this.Setup.SmallSize}px`;
            this.PeelMask.style.width = `${this.Setup.SmallSize}px`;
            this.PeelMask.style.height = `${this.Setup.SmallSize}px`;

            this.Increment = (this.Setup.LargeSize - this.Setup.SmallSize) / (PageEarModule.TIME / PageEarModule.STEPTIME)

            $YetaWF.registerEventHandler(this.PeelDiv, "mouseover", null, (ev:MouseEvent): boolean => {
                this.maximize();
                return true;
            });
            $YetaWF.registerEventHandler(this.PeelDiv, "mouseout", null, (ev:MouseEvent): boolean =>{
                this.clearInterval();
                this.minimize();
                return true;
            });
        }

        private clearInterval(): void {
            if (this.Interval)
                clearInterval(this.Interval);
            this.Interval = 0;
        }

        private maximize(): void {
            if (!this.Increase) {
                this.clearInterval();
                this.Increase = true;
                this.Interval = setInterval((): void => { this.DoSteps(); }, PageEarModule.STEPTIME);
            }
        }
        private minimize(): void {
            if (this.Increase) {
                this.clearInterval();
                this.Increase = false;
                this.Interval = setInterval((): void => { this.DoSteps(); }, PageEarModule.STEPTIME);
            }
        }

        private DoSteps(): void {
            let done = false;
            let width: number;
            if (this.Setup.AutoAnimate) {
                if (!this.Interval) return;
                width = Number.parseFloat(this.PeelImage.style.width);
                if (this.Increase) {
                    width += this.Increment;
                    if (width >= this.Setup.LargeSize) {
                        width = this.Setup.LargeSize;
                        done = true;
                    }
                } else {
                    width -= this.Increment;
                    if (width <= this.Setup.SmallSize) {
                        width = this.Setup.SmallSize;
                        done = true;
                    }
                }
            } else {
                if (this.Increase)
                    width = this.Setup.LargeSize;
                else
                    width = this.Setup.SmallSize;
                done = true;
            }
            this.PeelImage.style.width = `${width}px`;
            this.PeelImage.style.height = `${width}px`;
            this.PeelMask.style.width = `${width}px`;
            this.PeelMask.style.height = `${width}px`;

            if (done)
                this.clearInterval();
        }
    }
}

