/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

// If this javascript snippet is included, that means we're displaying disquslinks.

var DISQUSWIDGETS: any;

namespace YetaWF_Blog {

    class SkinDisqusLinksModule {

        static readonly MODULEGUID: string = "776adfcd-da5f-4926-b29d-4c06353266c0";

        static on: boolean = true;

        /**
         * Initializes the module instance.
         */
        init(): void {

            $YetaWF.registerContentChange((addonGuid: string, on: boolean): void => {
                if (addonGuid === SkinDisqusLinksModule.MODULEGUID) {
                    SkinDisqusLinksModule.on = on;
                }
            });

            $YetaWF.addWhenReady((tag: HTMLElement) : void => {
                if (SkinDisqusLinksModule.on && DISQUSWIDGETS)
                    DISQUSWIDGETS.getCount({ reset: true });
            });
        }
    }

    var disqusLinks: SkinDisqusLinksModule = new SkinDisqusLinksModule();
    disqusLinks.init();
}
