/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF {
    export interface IConfigs {
        YetaWF_ComponentsHTML: YetaWF_ComponentsHTML.IPackageConfigs;
    }
}
namespace YetaWF_ComponentsHTML {
    export interface IPackageConfigs {
        LoaderGif: string;
    }
}

namespace YetaWF_ComponentsHTML {

    // Loading indicator

    export class LoadingClass {

        private static readonly OFFSETTOP: number = 13;
        private static readonly OFFSETLEFT: number = 0;

        private LoadingDiv: HTMLDivElement | null = null;

        private On: boolean = false;
        private CursorX: number = 0;
        private CursorY: number = 0;
        private Width : number = 0;
        private Height : number = 0;

        constructor() {

            $YetaWF.registerEventHandlerBody("mousemove", null, (ev: MouseEvent): boolean => {
                this.CursorX = ev.clientX;
                this.CursorY = ev.clientY;
                if (this.On)
                    this.positionLoading();
                return true;
            });
        }

        public show(): void {
            if (this.On) return;
            this.createLoading();
            this.positionLoading();
            if (this.LoadingDiv)
                this.LoadingDiv.style.display = "";
            this.On = true;
        }

        public hide(): void {
            if (this.LoadingDiv)
                this.LoadingDiv.style.display = "none";
            this.On = false;
        }

        private createLoading(): void {
            if (!this.LoadingDiv) {
                this.LoadingDiv = <div id="yLoading"><img /></div> as HTMLDivElement;
                ($YetaWF.getElement1BySelector("img", [this.LoadingDiv]) as HTMLImageElement).src = YConfigs.YetaWF_ComponentsHTML.LoaderGif;
                document.body.appendChild(this.LoadingDiv);

                let rect = this.LoadingDiv.getBoundingClientRect();
                this.LoadingDiv.style.display = "none";

                this.Width = rect.width;
                this.Height = rect.height;
            }
        }
        private positionLoading(): void {

            if (!this.LoadingDiv) return;

            let htmlDiv = document.querySelector("html")!;

            let x = this.CursorX + LoadingClass.OFFSETLEFT;
            let y = this.CursorY + LoadingClass.OFFSETTOP;
            if (x + this.Width > htmlDiv.clientWidth) x = htmlDiv.clientWidth - this.Width;
            if (x < 0) x = 0;
            if (y + this.Height > htmlDiv.clientHeight) y = htmlDiv.clientHeight - this.Height;
            if (y < 0) y = 0;

            let left = x + window.pageXOffset;
            let top = y + window.pageYOffset;

            this.LoadingDiv.style.top = `${top}px`;
            this.LoadingDiv.style.left = `${left}px`;
        }
    }
}

var LoadingSupport: YetaWF_ComponentsHTML.LoadingClass = new YetaWF_ComponentsHTML.LoadingClass();
