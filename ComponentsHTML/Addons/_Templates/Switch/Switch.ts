/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    export interface IPackageLocs {
        CopyToClip: string;
    }

    export class SwitchComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_switch";
        public static readonly SELECTOR: string = ".yt_switch";
        public static readonly EVENTCHANGE: string = "switch_change";

        private Input: HTMLInputElement;
        private Label: HTMLLabelElement;

        constructor(controlId: string) {
            super(controlId, SwitchComponent.TEMPLATE, SwitchComponent.SELECTOR, {
                ControlType: YetaWF_ComponentsHTML.ControlTypeEnum.Template,
                ChangeEvent: SwitchComponent.EVENTCHANGE,
                GetValue: (control: SwitchComponent): string | null => {
                    return control.Input.checked ? "true" : "false";
                },
                Enable: (control: SwitchComponent, enable: boolean, clearOnDisable: boolean): void => {
                    YetaWF_BasicsImpl.elementEnableToggle(control.Input, enable);
                    if (clearOnDisable)
                        control.Input.checked = false;
                },
            });

            this.Input = $YetaWF.getElement1BySelector("input", [this.Control]) as HTMLInputElement;
            this.Label = $YetaWF.getElement1BySelector("label", [this.Control]) as HTMLLabelElement;
            $YetaWF.registerEventHandler(this.Label, "keypress", null, (ev: KeyboardEvent): boolean => {
                if (this.Input.disabled) return true;
                if (ev.key.length !== 1) return true;// special key, like Enter
                if (ev.key === " ") {
                    this.Input.checked = !this.Input.checked;
                    this.sendChangeEvent();
                    return false;
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Input, "change", null, (ev: Event): boolean => {
                this.sendChangeEvent();
                return true;
            });
        }
        private sendChangeEvent(): void {
            var event = document.createEvent("Event");
            event.initEvent(SwitchComponent.EVENTCHANGE, false, true);
            this.Control.dispatchEvent(event);
        }
    }
}