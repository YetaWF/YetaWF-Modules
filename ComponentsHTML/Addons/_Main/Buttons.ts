/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

/* initialize buttons (bootstrap and/or jquery-ui) */

namespace YetaWF_ComponentsHTML {

    export class Buttons {

        public static initButtons(tag: HTMLElement): void {
            // Diagnostics, find old bootstrap and kendo buttons, which shouldn't be used any longer.
            if (YConfigs.Basics.DEBUGBUILD) {
                let invalids = $YetaWF.getElementsBySelector(".y_bootstrap");
                if (invalids.length)
                    throw "Elements with .y_bootstrap found - no longer supported";
                invalids = $YetaWF.getElementsBySelector(".btn,.btn-primary");
                if (invalids.length)
                    throw "Elements with .btn,.btn-primary found - no longer supported";
                invalids = $YetaWF.getElementsBySelector(".y_jqueryui");
                if (invalids.length)
                    throw "Elements with .y_jqueryui found - no longer supported";
                invalids = $YetaWF.getElementsBySelector(".ui-button");
                if (invalids.length)
                    throw "Elements with .ui-button found - no longer supported";
            }
        }
    }
    $YetaWF.registerCustomEventHandlerDocument(YetaWF.Content.EVENTNAVPAGELOADED, null, (ev: CustomEvent<YetaWF.DetailsEventNavPageLoaded>): boolean => {
        for (let container of ev.detail.containers)
            YetaWF_ComponentsHTML.Buttons.initButtons(container);
        return true;
    });
}


