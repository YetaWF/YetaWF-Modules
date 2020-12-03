/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

namespace YetaWF_Panels {

    export class SplitterModule extends YetaWF.ModuleBaseDataImpl {

        public static readonly SELECTOR: string = ".YetaWF_Panels_Splitter";

        constructor(id: string) {
            super(id, SplitterModule.SELECTOR, null);

            $YetaWF.registerEventHandler(this.LeftArea, "scroll", null, (ev: Event): boolean => {
                $YetaWF.sendContainerScrollEvent(this.LeftArea);
                return true;
            });
            $YetaWF.registerEventHandler(this.RightArea, "scroll", null, (ev: Event): boolean => {
                $YetaWF.sendContainerScrollEvent(this.RightArea);
                return true;
            });
        }

        public static GetSplitterFromTag(tag: HTMLElement) : YetaWF_Panels.SplitterInfoComponent {
            let mod = YetaWF_Panels.SplitterModule.GetSplitterModuleFromTagCond(tag);
            if (!mod)
                throw "The Splitter module cannot be found";

            let splitter = YetaWF_Panels.SplitterInfoComponent.getControlFromSelector<YetaWF_Panels.SplitterInfoComponent>(YetaWF_Panels.SplitterInfoComponent.SELECTOR, YetaWF_Panels.SplitterInfoComponent.SELECTOR, [mod.Module]);
            if (!splitter)
                throw "The Splitter module cannot be found";

            return splitter;
        }

        public static GetSplitterModuleFromTagCond(tag: HTMLElement) : YetaWF_Panels.SplitterModule | null {
            let modDiv = $YetaWF.elementClosestCond(tag, SplitterModule.SELECTOR);
            if (!modDiv)
                return null;

            let mod = YetaWF_Panels.SplitterModule.getModuleFromTag<SplitterModule>(modDiv);
            if (!mod)
                throw "The Splitter module cannot be found";

            return mod;
        }

        public static GetSplitterModuleFromTag(tag: HTMLElement) : YetaWF_Panels.SplitterModule {
            let mod = SplitterModule.GetSplitterModuleFromTagCond(tag);
            if (!mod)
                throw "The Splitter module cannot be found";
            return mod;
        }

        public get LeftArea(): HTMLElement {
            return $YetaWF.getElement1BySelector(".yt_panels_splitterinfo_left .t_area", [this.Module]);
        }
        public get RightArea(): HTMLElement {
            return $YetaWF.getElement1BySelector(".yt_panels_splitterinfo_right .t_area", [this.Module]);
        }
    }
}
