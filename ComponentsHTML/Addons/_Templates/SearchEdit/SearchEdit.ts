/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface SearchEditSetup {
        AutoClickDelay: number;
    }

    export class SearchEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_search";
        public static readonly SELECTOR: string = ".yt_search.t_edit";

        public static readonly EVENTCHANGE: string = "search_change";
        public static readonly EVENTCLICK: string = "search_click";

        private Setup: SearchEditSetup;
        private Container: HTMLElement;
        private InputControl: HTMLInputElement;

        private AutoTimeout: number = 0;

        constructor(controlId: string, setup: SearchEditSetup) {
            super(controlId, SearchEditComponent.TEMPLATE, SearchEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: SearchEditComponent.EVENTCHANGE,
                GetValue: (control: SearchEditComponent): string | null => {
                    return control.value;
                },
                Enable: (control: SearchEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            }, false, (tag: HTMLElement, control: SearchEditComponent): void =>{
                control.stopAutoClick();
            });
            this.Setup = setup;

            this.InputControl = this.Control as HTMLInputElement;
            this.Container = $YetaWF.elementClosest(this.Control, ".yt_search_container");

            $YetaWF.registerMultipleEventHandlers([this.InputControl], ["change", "input"], null, (ev: Event): boolean => {
                this.sendChangeEvent();
                this.startAutoClick();
                return true;
            });

            $YetaWF.registerEventHandler(this.InputControl, "keydown", null, (ev: KeyboardEvent): boolean => {
                let key = ev.key;
                switch(key) {
                    case "Enter":
                        this.sendClickEvent();
                        return false;
                }
                return true;
            });

            $YetaWF.registerEventHandler(this.Container, "click", ".t_search", (ev: Event): boolean => {
                this.InputControl.focus();
                this.sendClickEvent();
                return false;
            });
            $YetaWF.registerEventHandler(this.InputControl, "focusin", null, (ev: FocusEvent): boolean => {
                if (this.enabled) {
                    $YetaWF.elementRemoveClass(this.Container, "t_focused");
                    $YetaWF.elementAddClass(this.Container, "t_focused");
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.InputControl, "focusout", null, (ev: FocusEvent): boolean => {
                $YetaWF.elementRemoveClass(this.Container, "t_focused");
                return true;
            });
        }

        private startAutoClick() : void {
            this.stopAutoClick();
            if (this.Setup.AutoClickDelay) {
                this.AutoTimeout = setTimeout((): void => {
                    this.sendClickEvent();
                }, this.Setup.AutoClickDelay);
            }
        }
        private stopAutoClick() : void {
            if (this.AutoTimeout) {
                clearInterval(this.AutoTimeout);
                this.AutoTimeout = 0;
            }
        }

        // events

        private sendChangeEvent(): void {
            $YetaWF.sendCustomEvent(this.Control, SearchEditComponent.EVENTCHANGE);
            FormsSupport.validateElement(this.Control);
        }
        private sendClickEvent(): void {
            $YetaWF.sendCustomEvent(this.Control, SearchEditComponent.EVENTCLICK);
        }

        // API

        get value(): string {
            return this.InputControl.value;
        }
        set value(val: string) {
            this.InputControl.value = val;
        }
        public clear(): void {
            this.InputControl.value = "";
        }
        public enable(enabled: boolean): void {
            $YetaWF.elementEnableToggle(this.InputControl, enabled);
            $YetaWF.elementRemoveClass(this.Container, "t_disabled");
            if (!enabled)
                $YetaWF.elementAddClass(this.Container, "t_disabled");
        }
        public get enabled(): boolean {
            return !$YetaWF.elementHasClass(this.Container, "t_disabled");
        }
        public get focused(): boolean {
            return $YetaWF.elementHasClass(this.Container, "t_focused");
        }
    }
}

