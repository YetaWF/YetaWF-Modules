/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

namespace YetaWF_DevTests {

    export class TemplateProgressBarModule extends YetaWF.ModuleBaseNoDataImpl {

        public static readonly SELECTOR: string = ".YetaWF_DevTests_TemplateProgressBar";

        private ProgressBar: YetaWF_ComponentsHTML.ProgressBarComponent;
        private Value: YetaWF_ComponentsHTML.IntValueEditComponent;

        constructor(id: string) {
            super(id, TemplateProgressBarModule.SELECTOR, null);

            this.ProgressBar = YetaWF.ComponentBaseDataImpl.getControlFromSelector(`.t_row.t_bar ${YetaWF_ComponentsHTML.ProgressBarComponent.SELECTOR}`, YetaWF_ComponentsHTML.ProgressBarComponent.SELECTOR, [this.Module]);
            this.Value = YetaWF.ComponentBaseDataImpl.getControlFromSelector("input[name='Value']", YetaWF_ComponentsHTML.IntValueEditComponent.SELECTOR, [this.Module]);

            $YetaWF.registerCustomEventHandler(this.Value, YetaWF_ComponentsHTML.IntValueEditComponent.EVENT, (ev: Event): void => {
                this.ProgressBar.value = this.Value.value;
            });
        }
    }
}

