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

        public static GetSplitterFromTag(tag: HTMLElement) : YetaWF_Panels.SplitterInfoComponent {
            let mod = $YetaWF.elementClosestCond(tag, SplitterModule.SELECTOR);
            if (!mod)
                throw "The Splitter module cannot be found";

            let splitter = YetaWF_Panels.SplitterInfoComponent.getControlFromSelector<YetaWF_Panels.SplitterInfoComponent>(YetaWF_Panels.SplitterInfoComponent.SELECTOR, YetaWF_Panels.SplitterInfoComponent.SELECTOR, [mod]);
            if (!splitter)
                throw "The Splitter module cannot be found";

            return splitter;
        }
    }
}
