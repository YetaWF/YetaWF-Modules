/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface ColorEditSetup {

    }

    export class ColorEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_color";
        public static readonly SELECTOR: string = ".yt_color.t_edit";
        public static readonly EVENTCHANGE: string = "color_change";

        private Input: HTMLInputElement;
        private Color: HTMLInputElement;
        //private Setup:ColorEditSetup;

        constructor(controlId: string, setup: ColorEditSetup) {
            super(controlId, ColorEditComponent.TEMPLATE, ColorEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: ColorEditComponent.EVENTCHANGE,
                GetValue: (control: ColorEditComponent): string | null => {
                    return control.value;
                },
                Enable: (control: ColorEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                }
            });
            //this.Setup = setup;

            this.Input = $YetaWF.getElement1BySelector("input[type='text']", [this.Control]) as HTMLInputElement;
            this.Color = $YetaWF.getElement1BySelector("input[type='color']", [this.Control]) as HTMLInputElement;

            $YetaWF.registerMultipleEventHandlers([this.Color], ["input", "change"], null, (ev: Event): boolean => {
                this.Input.value = this.Color.value;
                $YetaWF.sendCustomEvent(this.Control, ColorEditComponent.EVENTCHANGE);
                return false;
            });
            $YetaWF.registerMultipleEventHandlers([this.Input], ["input", "change"], null, (ev: Event): boolean => {
                if (this.Input.value.trim().startsWith("#"))
                    this.Color.value = this.Input.value;
                else
                    this.Color.value = "#FFFFFF";
                $YetaWF.sendCustomEvent(this.Control, ColorEditComponent.EVENTCHANGE);
                return false;
            });
        }
        get value(): string {
            return this.Input.value;
        }
        set value(val: string) {
            this.Input.value = val;
            if (val.trim().startsWith("#"))
                this.Color.value = val;
            else
                this.Color.value = "#FFFFFF";
        }
        public clear(): void {
            this.Input.value = "";
            this.Color.value = "";
        }
        public enable(enabled: boolean): void {
            YetaWF_BasicsImpl.elementEnableToggle(this.Input, enabled);
            YetaWF_BasicsImpl.elementEnableToggle(this.Control, enabled);
        }
    }
}

