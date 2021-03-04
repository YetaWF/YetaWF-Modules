/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF {
    export interface IConfigs {
        YetaWF_ComponentsHTML: YetaWF_ComponentsHTML.IPackageConfigs;
    }
}
namespace YetaWF_ComponentsHTML {
    export interface IPackageConfigs {
        SVG_fas_exclamation_triangle: string;
    }
}

namespace YetaWF_ComponentsHTML {

    export interface MaskedEdit {
        Mask: string;
    }

    export class MaskedEditComponent extends YetaWF.ComponentBaseDataImpl {

        private static readonly WARNDELAY:number = 300;

        private EVENTCHANGE: string;
        private Setup: MaskedEdit;
        private Hidden: HTMLInputElement;
        private Input: HTMLInputElement;

        constructor(controlId: string, setup: MaskedEdit, template: string, selector: string, eventChange: string) {
            super(controlId, template, selector, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: eventChange,
                GetValue: (control: MaskedEditComponent): string | null => {
                    let v = control.Hidden.value;
                    return v;
                },
                Enable: (control: MaskedEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            });
            this.EVENTCHANGE = eventChange;
            this.Setup = setup;

            this.Hidden = $YetaWF.getElement1BySelector("input[type='hidden']", [this.Control]) as HTMLInputElement;
            this.Input = $YetaWF.getElement1BySelector("input[type='text']", [this.Control]) as HTMLInputElement;

            this.value = this.Hidden.value;

            let warn = <div class="t_warn" style="display:none"></div>;
            warn.innerHTML = YConfigs.YetaWF_ComponentsHTML.SVG_fas_exclamation_triangle;
            this.Input.insertAdjacentElement("afterend", warn);

            $YetaWF.registerEventHandler(this.Input, "keypress", null, (ev: KeyboardEvent): boolean => {
                if (!this.isValidKeyPress(ev)) {
                    this.flashError();
                    return false;
                }
                setTimeout((): void => {
                    let caret = this.Input.selectionStart;
                    this.setHidden(this.mergeForHidden(this.Input.value, this.Setup.Mask));
                    this.Input.value = this.mergeForOutput(this.Hidden.value, this.Setup.Mask);
                    this.setCaretForward(caret);
                }, 1);
                return true;
            });
            $YetaWF.registerEventHandler(this.Input, "keydown", null, (ev: KeyboardEvent): boolean => {
                let key = ev.key;
                if (!ev.altKey) {
                    switch (key) {
                        case "Backspace":
                            setTimeout((): void => {
                                let caret = this.Input.selectionStart;
                                this.setHidden(this.mergeForHidden(this.Input.value, this.Setup.Mask));
                                this.Input.value = this.mergeForOutput(this.Hidden.value, this.Setup.Mask);
                                this.setCaretBackward(caret);
                            }, 1);
                            break;
                        case "Delete":
                            setTimeout((): void => {
                                let caret = this.Input.selectionStart;
                                this.setHidden(this.mergeForHidden(this.Input.value, this.Setup.Mask));
                                this.Input.value = this.mergeForOutput(this.Hidden.value, this.Setup.Mask);
                                this.setCaretForward(caret);
                            }, 1);
                            break;
                    }
                }
                return true;
            });
            $YetaWF.registerEventHandler(this.Input, "paste", null, (ev: ClipboardEvent): boolean => {
                setTimeout((): void => {
                    let caret = this.Input.selectionStart;
                    this.setHidden(this.mergeForHidden(this.Input.value, this.Setup.Mask));
                    this.Input.value = this.mergeForOutput(this.Hidden.value, this.Setup.Mask);
                    this.setCaretBackward(caret);
                }, 1);
                return true;
            });
            $YetaWF.registerEventHandler(this.Input, "focusin", null, (ev: FocusEvent): boolean => {
                this.Input.value = this.mergeForOutput(this.Hidden.value, this.Setup.Mask);
                this.Input.select();
                return true;
            });
            $YetaWF.registerEventHandler(this.Input, "focusout", null, (ev: FocusEvent): boolean => {
                this.setHidden(this.mergeForHidden(this.Input.value, this.Setup.Mask));
                if (!this.Hidden.value)
                    this.Input.value = "";
                return true;
            });
        }

        private isValidKeyPress(ev: KeyboardEvent) : boolean {
            let caret = this.Input.selectionStart;
            let mask = this.Setup.Mask;
            if (caret == null || caret < 0) return false;
            let v = ev.key;
            for ( ; ; ) {
                if (caret >= mask.length)
                    return false;
                let m = mask[caret];
                if (!m)
                    return false;
                if (m) {
                    switch (m) {
                        case "N": // 0-9
                            if (v >= "0" && v <= "9")
                                return true;
                            return false;
                        case "-":
                            break;
                        //case ...
                        // TODO: add other options
                        default:
                            throw `Invalid mask character ${m}`;
                    }
                }
                caret++;
            }
        }

        private setCaretForward(pos: number|null): void {
            if (pos == null) {
                this.Input.select();
            } else {
                let found = false;
                --pos;
                if (pos >= 0) {
                    let len = this.Input.value.length;
                    let maskLen = this.Setup.Mask.length;
                    // find the non-separator we just entered
                    while (pos < len && pos < maskLen) {
                        let m = this.Setup.Mask[pos];
                        switch (m) {
                            case "N": // 0-9
                                found = true;
                                break;
                            case "-":
                                break;
                            default:
                                throw `Invalid mask character ${m}`;
                        }
                        if (found)
                            break;
                        ++pos;
                    }
                    ++pos;
                    // skip if we're now on a separator
                    found = false;
                    while (pos < len && pos < maskLen) {
                        let m = this.Setup.Mask[pos];
                        switch (m) {
                            case "N": // 0-9
                                found = true;
                                break;
                            case "-":
                                break;
                            default:
                                throw `Invalid mask character ${m}`;
                        }
                        if (found)
                            break;
                        ++pos;
                    }

                    this.Input.selectionStart = pos;
                    this.Input.selectionEnd = pos;
                } else {
                    this.Input.selectionStart = 0;
                    this.Input.selectionEnd = 0;
                }
            }
        }

        private setCaretBackward(pos: number|null): void {
            if (pos == null) {
                this.Input.select();
            } else {
                this.Input.selectionStart = pos;
                this.Input.selectionEnd = pos;
            }
        }

        private mergeForOutput(value: string, mask: string) : string {
            let out = "";

            for ( ; ; ) {
                let m = mask.substring(0, 1);
                mask = mask.substring(1);
                let v = value.substring(0, 1);
                value = value.substring(1);
                if (!m)
                    break;
                if (m) {
                    switch (m) {
                        case "N": // 0-9
                            if (v >= "0" && v <= "9")
                                out += v;
                            else if (v === "_")
                                out += v;
                            else {
                                out += "_";
                                value = "";
                            }
                            break;
                        case "-":
                            out += m;
                            value = v + value;// push back
                            break;
                        //case ...
                        // TODO: add other options
                        default:
                            throw `Invalid mask character ${m}`;
                    }
                }
                if (!mask)
                    break;
            }
            return out;
        }

        private mergeForHidden(value: string, mask: string) : string {
            let out = "";

            for ( ; ; ) {
                let m = mask.substring(0, 1);
                mask = mask.substring(1);
                let v = value.substring(0, 1);
                value = value.substring(1);
                if (!m)
                    break;
                if (m) {
                    if (v === "_") {
                        v = "";
                        mask = m + mask;// push back
                        continue;
                    } else if (v === "-") {
                        v = "";
                        mask = m + mask;// push back
                        continue;
                    }
                    switch (m) {
                        case "N": // 0-9
                            if (v >= "0" && v <= "9")
                                out += v;
                            else
                                value = v + value;// push back
                            break;
                        case "-":
                            value = v + value;// push back
                            break;
                        //case ...
                        // TODO: add other options
                        default: // assume some other separator
                            throw `Invalid mask character ${m}`;
                    }
                }
                if (!mask)
                    break;
            }
            return out;
        }
        private setHidden(value: string): void {
            if (this.Hidden.value !== value) {
                this.Hidden.value = value;
                this.sendChangeEvent();
            }
        }
        private sendChangeEvent(): void {
            $YetaWF.sendCustomEvent(this.Control, this.EVENTCHANGE);
        }

        private flashError(): void {
            let warn = $YetaWF.getElement1BySelector(".t_warn", [this.Control]);
            warn.style.display = "";
            setTimeout(():void => {
                warn.style.display = "none";
            }, MaskedEditComponent.WARNDELAY);
        }

        // API

        public get value(): string {
            return this.Hidden.value;
        }
        public set value(value: string) {
            this.Hidden.value = value;
            if (this.Hidden.value)
                this.Input.value = this.mergeForOutput(this.Hidden.value, this.Setup.Mask);
            else
                this.Input.value = "";
        }
        public clear(): void {
            this.value = "";
        }
        public enable(enabled: boolean): void {
            $YetaWF.elementEnableToggle(this.Input, enabled);
        }
    }
}

