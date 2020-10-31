/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

namespace YetaWF_Panels {

    export class SplitterModule extends YetaWF.ModuleBaseDataImpl {

        public static readonly SELECTOR: string = ".YetaWF_Panels_Splitter";

        constructor(id: string) {
            super(id, SplitterModule.SELECTOR, null);

            $YetaWF.registerEventHandler($YetaWF.getElement1BySelector(".yt_panels_splitterinfo_left .t_area", [this.Module]), "scroll", null, (ev: Event): boolean => {
                $YetaWF.sendContainerScrollEvent(ev.__YetaWFElem);
                return true;
            });
        }
    }
}
