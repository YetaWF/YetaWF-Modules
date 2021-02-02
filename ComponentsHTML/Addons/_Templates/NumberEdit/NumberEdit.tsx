/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF {
    export interface IConfigs {
        YetaWF_ComponentsHTML: YetaWF_ComponentsHTML.IPackageConfigs;
    }
}
namespace YetaWF_ComponentsHTML {
    export interface IPackageConfigs {
        SVG_fas_caret_up: string;
        SVG_fas_caret_down: string;
        SVG_fas_exclamation_triangle: string;
    }
}

namespace YetaWF_ComponentsHTML {

    export interface NumberSetup {
        Min: number;
        Max: number;
        Step: number;
        Lead: string;
        Trail: string;
        Digits: number;
        Currency: string;
        Locale: string;
    }

    export class NumberEditComponentBase extends YetaWF.ComponentBaseDataImpl {

        public static readonly EVENT: string = "number_changespin";// combines change and spin
        public static readonly EVENTCHANGE: string = "number_change";
        public static readonly EVENTSPIN: string = "number_spin";

        private static readonly INITIALDELAY:number = 300;
        private static readonly STEPDELAY:number = 100;
        private static readonly WARNDELAY:number = 300;

        private Setup: NumberSetup;
        private InputControl: HTMLInputElement;
        private InputHidden: HTMLInputElement;
        private Value: number|null = null;

        private Interval: number = 0;

        constructor(controlId: string, setup: NumberSetup, template: string, selector: string) {
            super(controlId, template, selector, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: NumberEditComponentBase.EVENT,
                GetValue: (control: NumberEditComponentBase): string | null => {
                    let v = control.InputHidden.value;
                    return v;
                },
                Enable: (control: NumberEditComponentBase, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            });
            this.Setup = setup;

            this.InputControl = $YetaWF.getElement1BySelector("input[type='text']", [this.Control]) as HTMLInputElement;
            this.InputHidden = $YetaWF.getElement1BySelector("input[type='hidden']", [this.Control]) as HTMLInputElement;

            this.setInternalValue(this.InputControl.value);

            // icons used: fas-exclamation-triangle
            let warn = <div class="t_warn" style="display:none"></div>;
            warn.innerHTML = YConfigs.YetaWF_ComponentsHTML.SVG_fas_exclamation_triangle;
            // icons used: fas-caret-up, fas-caret-down
            let updown =
                <div class="t_updown">
                    <div class="t_up">
                    </div>
                    <div class="t_down">
                    </div>
                </div>;
            $YetaWF.getElement1BySelector(".t_up", [updown]).innerHTML =  YConfigs.YetaWF_ComponentsHTML.SVG_fas_caret_up;
            $YetaWF.getElement1BySelector(".t_down", [updown]).innerHTML =  YConfigs.YetaWF_ComponentsHTML.SVG_fas_caret_down;

            this.InputControl.insertAdjacentElement("afterend", updown);
            this.InputControl.insertAdjacentElement("afterend", warn);

            $YetaWF.registerMultipleEventHandlers([this.InputControl], ["change", "input"], null, (ev: Event): boolean => {
                if (!this.isValid(this.InputControl.value)) {
                    this.setInternalValue(this.Value);
                    this.flashError();
                    return false;
                }
                this.setInternalValue(this.InputControl.value, false);
                this.sendChangeEvent();
                return true;
            });

            $YetaWF.registerEventHandler(this.InputControl, "keydown", null, (ev: KeyboardEvent): boolean => {
                let key = ev.key;
                switch(key) {
                    case "ArrowDown":
                    case "Down":
                        this.setNewSpinValue(false);
                        return false;
                    case "ArrowUp":
                    case "Up":
                        this.setNewSpinValue(true);
                        return false;
                }
                return true;
            });
            // deprecated, understood. Switch to beforeinput, but oh no IE11.
            $YetaWF.registerEventHandler(this.InputControl, "keypress", null, (ev: KeyboardEvent): boolean => {
                let key = ev.key;
                switch(key) {
                    case "0": case "1": case "2": case "3": case "4":
                    case "5": case "6": case "7": case "8": case "9":
                        break;
                    case ".":
                        if (!this.Setup.Digits) {
                            this.flashError();
                            return false;
                        }
                        break;
                    default:
                        this.flashError();
                        return false;
                }
                return true;
            });

            $YetaWF.registerEventHandler(this.Control, "mousedown", ".t_up", (ev: Event): boolean => {
                this.InputControl.focus();
                this.setNewSpinValue(true);
                this.startSpin(true);
                return false;
            });
            $YetaWF.registerEventHandler(this.Control, "mousedown", ".t_down", (ev: Event): boolean => {
                this.InputControl.focus();
                this.setNewSpinValue(false);
                this.startSpin(false);
                return false;
            });
            $YetaWF.registerMultipleEventHandlers([this.Control], ["mouseup", "mouseout"], ".t_down,.t_up", (ev: Event): boolean => {
                this.clearSpin();
                return false;
            });
            $YetaWF.registerEventHandler(this.InputControl, "focusin", null, (ev: FocusEvent): boolean => {
                if (this.enabled) {
                    $YetaWF.elementRemoveClass(this.Control, "t_focused");
                    $YetaWF.elementAddClass(this.Control, "t_focused");
                    this.setInternalValue(this.Value);
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.InputControl, "focusout", null, (ev: FocusEvent): boolean => {
                $YetaWF.elementRemoveClass(this.Control, "t_focused");
                this.clearSpin();
                this.setInternalValue(this.Value);
                return true;
            });
        }

        // Spin

        private clearSpin(): void {
            if (this.Interval)
                clearInterval(this.Interval);
            this.Interval = 0;
        }
        private startSpin(up:boolean, timeout?:number): void {
            this.clearSpin();
            this.Interval = setInterval((): void => {
                this.setNewSpinValue(up);
                this.clearSpin();
                this.startSpin(up, NumberEditComponentBase.STEPDELAY);
            }, timeout === undefined ? NumberEditComponentBase.INITIALDELAY : timeout);
        }
        private setNewSpinValue(up:boolean):void {
            let value: number|null;
            if (this.Value !== null) {
                value = this.value;
                value += up ? this.Setup.Step : -this.Setup.Step;
                value = Math.min(this.Setup.Max, value);
                value = Math.max(this.Setup.Min, value);
                value = Number(value.toFixed(this.Setup.Digits));
            } else {
                value = this.Setup.Min;
            }
            if (value !== this.Value) {
                this.value = value;
                this.sendSpinEvent();
            } else {
                this.clearSpin();
            }
        }

        // events

        private sendChangeEvent(): void {
            $YetaWF.sendCustomEvent(this.Control, NumberEditComponentBase.EVENT);
            $YetaWF.sendCustomEvent(this.Control, NumberEditComponentBase.EVENTCHANGE);
            FormsSupport.validateElement(this.InputHidden);
        }
        private sendSpinEvent(): void {
            $YetaWF.sendCustomEvent(this.Control, NumberEditComponentBase.EVENT);
            $YetaWF.sendCustomEvent(this.Control, NumberEditComponentBase.EVENTSPIN);
        }

        // API

        get value(): number {
            return this.Value ?? 0;
        }
        set value(val: number) {
            val = Number(val.toFixed(this.Setup.Digits));
            this.Value = val;
            this.InputHidden.value = val.toString();
            if (this.focused)
                this.InputControl.value = val.toString();
            else {
                if (this.Setup.Currency && this.Setup.Locale) {
                    let v  = val.toLocaleString(this.Setup.Locale, { style: "currency", currency: this.Setup.Currency });
                    // special case for $US
                    if (v.startsWith("$"))
                        v = `$ ${v.substring(1)}`;
                    this.InputControl.value = v;
                } else {
                    let l = this.Setup.Lead ? `${this.Setup.Lead} ` : "";
                    let t = this.Setup.Trail ? ` ${this.Setup.Trail}` : "";
                    this.InputControl.value = `${l}${val.toLocaleString(this.Setup.Locale, { style: "decimal", minimumFractionDigits: this.Setup.Digits, maximumFractionDigits: this.Setup.Digits })}${t}`;
                }
            }
        }
        private setInternalValue(val: number|null|string, updateIfValid?: boolean): void {
            if (typeof val === "string") {
                if (val) {
                    val = Number(val);
                    if (isNaN(val))
                        val = null;
                } else {
                    val = null;
                }
            }
            if (val != null) {
                if (updateIfValid === undefined || updateIfValid === true) {
                    this.value = val;
                } else {
                    this.Value = val;
                    this.InputHidden.value = val.toString();
                }
            } else {
                this.InputControl.value = "";
                this.InputHidden.value = "";
                this.Value = val;
            }
        }
        private isValid(text: string): boolean {
            let val = Number(text);
            if (isNaN(val))
                return false;
            if (val > this.Setup.Max || val < this.Setup.Min)
                return false;
            return true;
        }
        get valueText(): string  {
            return this.Value != null ? this.value.toString() : "";
        }
        public clear(): void {
            this.setInternalValue(null);
        }
        public enable(enabled: boolean): void {
            $YetaWF.elementEnableToggle(this.InputControl, enabled);
            $YetaWF.elementEnableToggle(this.InputHidden, enabled);
            $YetaWF.elementRemoveClass(this.Control, "t_disabled");
            if (!enabled)
                $YetaWF.elementAddClass(this.Control, "t_disabled");
        }
        public get enabled(): boolean {
            return !$YetaWF.elementHasClass(this.Control, "t_disabled");
        }
        public get focused(): boolean {
            return $YetaWF.elementHasClass(this.Control, "t_focused");
        }

        public flashError(): void {
            let warn = $YetaWF.getElement1BySelector(".t_warn", [this.Control]);
            warn.style.display = "";
            setTimeout(():void => {
                warn.style.display = "none";
            }, NumberEditComponentBase.WARNDELAY);
        }
    }
}

