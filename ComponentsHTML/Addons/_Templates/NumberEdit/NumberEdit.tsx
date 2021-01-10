/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

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
        private static readonly WARNDELAY:number = 100;

        private Setup: NumberSetup;
        private Container: HTMLElement;
        private InputControl: HTMLInputElement;
        private Value: number|null = null;

        private Interval: number = 0;

        constructor(controlId: string, setup: NumberSetup, template: string, selector: string) {
            super(controlId, template, selector, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: NumberEditComponentBase.EVENT,
                GetValue: (control: NumberEditComponentBase): string | null => {
                    let v = control.Value;
                    if (!v) return null;
                    return v.toString();
                },
                Enable: (control: NumberEditComponentBase, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            });
            this.Setup = setup;

            this.InputControl = this.Control as HTMLInputElement;
            this.Container = $YetaWF.elementClosest(this.Control, ".yt_number_container");

            this.internalValue = this.InputControl.value;

            // icons used: fas-exclamation-triangle
            let warn = <div class="t_warn" style="display:none"></div>;
            warn.innerHTML = "<svg aria-hidden='true' focusable='false' role='img' viewBox='0 0 576 512' xmlns='http://www.w3.org/2000/svg'><path fill='currentColor' d='M569.517 440.013C587.975 472.007 564.806 512 527.94 512H48.054c-36.937 0-59.999-40.055-41.577-71.987L246.423 23.985c18.467-32.009 64.72-31.951 83.154 0l239.94 416.028zM288 354c-25.405 0-46 20.595-46 46s20.595 46 46 46 46-20.595 46-46-20.595-46-46-46zm-43.673-165.346l7.418 136c.347 6.364 5.609 11.346 11.982 11.346h48.546c6.373 0 11.635-4.982 11.982-11.346l7.418-136c.375-6.874-5.098-12.654-11.982-12.654h-63.383c-6.884 0-12.356 5.78-11.981 12.654z'></path></svg>";
            // icons used: fas-caret-up, fas-caret-down
            let updown =
                <div class="t_updown">
                    <div class="t_up">
                    </div>
                    <div class="t_down">
                    </div>
                </div>;
            $YetaWF.getElement1BySelector(".t_up", [updown]).innerHTML =  "<svg aria-hidden='true' focusable='false' role='img' viewBox='0 0 320 512' xmlns='http://www.w3.org/2000/svg'><path fill='currentColor' d='M288.662 352H31.338c-17.818 0-26.741-21.543-14.142-34.142l128.662-128.662c7.81-7.81 20.474-7.81 28.284 0l128.662 128.662c12.6 12.599 3.676 34.142-14.142 34.142z'></path></svg>";
            $YetaWF.getElement1BySelector(".t_down", [updown]).innerHTML =  "<svg aria-hidden='true' focusable='false' role='img' viewBox='0 0 320 512'><path fill='currentColor' d='M31.3 192h257.3c17.8 0 26.7 21.5 14.1 34.1L174.1 354.8c-7.8 7.8-20.5 7.8-28.3 0L17.2 226.1C4.6 213.5 13.5 192 31.3 192z'></path></svg>";

            this.Control.insertAdjacentElement("afterend", updown);
            this.Control.insertAdjacentElement("afterend", warn);

            $YetaWF.registerMultipleEventHandlers([this.InputControl], ["change", "input"], null, (ev: Event): boolean => {
                if (!this.isValid(this.InputControl.value)) {
                    this.internalValue = this.Value;
                    this.flashError();
                    return false;
                }
                this.internalValue = this.InputControl.value;
                this.sendChangeEvent();
                return true;
            });

            $YetaWF.registerEventHandler(this.InputControl, "keydown", null, (ev: KeyboardEvent): boolean => {
                let key = ev.key;
                if (key === "ArrowDown" || key === "Down") {
                    this.setNewSpinValue(false);
                    return false;
                } else if (key === "ArrowUp" || key === "Up") {
                    this.setNewSpinValue(true);
                    return false;
                }
                return true;
            });

            $YetaWF.registerEventHandler(this.Container, "mousedown", ".t_up", (ev: Event): boolean => {
                this.InputControl.focus();
                this.setNewSpinValue(true);
                this.startSpin(true);
                return false;
            });
            $YetaWF.registerEventHandler(this.Container, "mousedown", ".t_down", (ev: Event): boolean => {
                this.InputControl.focus();
                this.setNewSpinValue(false);
                this.startSpin(false);
                return false;
            });
            $YetaWF.registerMultipleEventHandlers([this.Container], ["mouseup", "mouseout"], ".t_down,.t_up", (ev: Event): boolean => {
                this.clearSpin();
                return false;
            });
            $YetaWF.registerEventHandler(this.InputControl, "focusin", null, (ev: FocusEvent): boolean => {
                if (this.enabled) {
                    $YetaWF.elementRemoveClass(this.Container, "t_focused");
                    $YetaWF.elementAddClass(this.Container, "t_focused");
                    this.internalValue = this.Value;
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.InputControl, "focusout", null, (ev: FocusEvent): boolean => {
                $YetaWF.elementRemoveClass(this.Container, "t_focused");
                this.clearSpin();
                this.internalValue = this.Value;
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
            // $(this.Control).trigger("change");
            $YetaWF.sendCustomEvent(this.Control, NumberEditComponentBase.EVENT);
            $YetaWF.sendCustomEvent(this.Control, NumberEditComponentBase.EVENTCHANGE);
            FormsSupport.validateElement(this.Control);
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
            this.Value = val;
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
        private set internalValue(val: number|null|string) {
            if (typeof val === "string") {
                val = Number(val);
                if (isNaN(val))
                    val = null;
            }
            if (val != null) {
                this.value = val;
            } else {
                this.InputControl.value = "";
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
            this.internalValue = null;
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

        public flashError(): void {
            let warn = $YetaWF.getElement1BySelector(".t_warn", [this.Container]);
            warn.style.display = "";
            setTimeout(():void => {
                warn.style.display = "none";
            }, NumberEditComponentBase.WARNDELAY);
        }
    }
}

