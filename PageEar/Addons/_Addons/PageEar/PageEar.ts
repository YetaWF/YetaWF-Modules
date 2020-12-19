/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEar#License */

namespace YetaWF_PageEar {

    interface Setup {
        AdImage: string;
        PeelImage: string;
        ClickURL: string;
        SmallSize: number;
        LargeSize: number;
        AutoAnimate: boolean;
        Debug: boolean;
    }

    export class PageEarModule extends YetaWF.ModuleBaseDataImpl {

        public static readonly SELECTOR: string = ".YetaWF_PageEar_PageEar";

        private Setup: Setup;

        constructor(id: string, setup: Setup) {
            super(id, PageEarModule.SELECTOR, null, (tag: HTMLElement, module: PageEarModule) : void => {
                // when the page is removed, we need to clean up
                $("#peelback").remove();
            });
            this.Setup = setup;

            let $$: any = $;
            $$("body").peelback({
                adImage: this.Setup.AdImage,
                peelImage: this.Setup.PeelImage,
                clickURL: this.Setup.ClickURL,
                smallSize: this.Setup.SmallSize,
                bigSize: this.Setup.LargeSize,
                autoAnimate: this.Setup.AutoAnimate,
                //gaTrack: true, //RFFU
                //gaLabel: '#1 Stegosaurus',
                debug: false
            });
        }
    }
}

