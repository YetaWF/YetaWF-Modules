/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
                this.LoadingDiv = <div id="yLoading" style="display:none"><img /></div> as HTMLDivElement;
                ($YetaWF.getElement1BySelector("img", [this.LoadingDiv]) as HTMLImageElement).src = YConfigs.YetaWF_ComponentsHTML.LoaderGif;
                document.body.append(this.LoadingDiv);
            }
        }
        private positionLoading(): void {

            if (!this.LoadingDiv) return;

            let left = this.CursorX + LoadingClass.OFFSETLEFT + window.pageXOffset;
            let top = this.CursorY + LoadingClass.OFFSETTOP + window.pageYOffset;

            this.LoadingDiv.style.top = `${top}px`;
            this.LoadingDiv.style.left = `${left}px`;
        }
    }
}

var LoadingSupport: YetaWF_ComponentsHTML.LoadingClass = new YetaWF_ComponentsHTML.LoadingClass();
