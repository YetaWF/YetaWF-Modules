/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface UrlEditSetup {
        Type: UrlTypeEnum;
        Url: string;
    }
    enum UrlTypeEnum {//flags
        Local = 1, // Local Url starting with /
        Remote = 2, // Remote Url http:// https:// or /
    }
    export class UrlEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_url";
        public static readonly SELECTOR: string = ".yt_url.t_edit";
        public static readonly EVENTCHANGE: string = "url_change";

        private Setup: UrlEditSetup;
        private inputHidden: HTMLInputElement;
        private selectType: YetaWF_ComponentsHTML.DropDownListEditComponent | null = null;
        private selectPage: YetaWF_ComponentsHTML.DropDownListEditComponent | null = null;
        private inputUrl: HTMLInputElement | null = null;
        private divLocal: HTMLDivElement | null = null;
        private divRemote: HTMLDivElement | null = null;
        private aLink: HTMLAnchorElement;

        constructor(controlId: string, setup: UrlEditSetup) {
            super(controlId, UrlEditComponent.TEMPLATE, UrlEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: UrlEditComponent.EVENTCHANGE,
                GetValue: (control: UrlEditComponent): string | null => {
                    return control.value;
                },
                Enable: (control: UrlEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            });
            this.Setup = setup;

            this.inputHidden = $YetaWF.getElement1BySelector(".t_hidden", [this.Control]) as HTMLInputElement;

            this.selectType = YetaWF.ComponentBaseDataImpl.getControlFromSelectorCond(".yt_urltype", DropDownListEditComponent.SELECTOR, [this.Control]);
            // eslint-disable-next-line no-bitwise
            if (this.Setup.Type & UrlTypeEnum.Local) {
                this.selectPage = YetaWF.ComponentBaseDataImpl.getControlFromSelector<DropDownListEditComponent>(".yt_urldesignedpage", DropDownListEditComponent.SELECTOR, [this.Control]);
                this.divLocal = $YetaWF.getElement1BySelector(".t_local", [this.Control]) as HTMLDivElement;
            }
            // eslint-disable-next-line no-bitwise
            if (this.Setup.Type & UrlTypeEnum.Remote) {
                this.inputUrl = $YetaWF.getElement1BySelector(".yt_urlremotepage", [this.Control]) as HTMLInputElement;
                this.divRemote = $YetaWF.getElement1BySelector(".t_remote", [this.Control]) as HTMLDivElement;
            }
            this.aLink = $YetaWF.getElement1BySelector(".t_link a", [this.Control]) as HTMLAnchorElement;

            this.value = this.Setup.Url;

            if (this.selectType) {
                this.selectType.Control.addEventListener(YetaWF_ComponentsHTML.DropDownListEditComponent.EVENTCHANGE, (evt: Event): void => {
                    this.updateStatus();
                    this.sendEvent();
                });
            }
            if (this.selectPage) {
                this.selectPage.Control.addEventListener(YetaWF_ComponentsHTML.DropDownListEditComponent.EVENTCHANGE, (evt: Event): void => {
                    this.updateStatus();
                    this.sendEvent();
                });
            }
            if (this.inputUrl) {
                $YetaWF.registerMultipleEventHandlers([this.inputUrl], ["input", "change", "click", "keyup", "paste"], null, (ev: Event): boolean => {
                    this.updateStatus();
                    this.sendEvent();
                    return true;
                });
            }
        }

        private updateStatus(): void {
            let tp: UrlTypeEnum;
            if (!this.selectType) {
                if (this.selectPage) {
                    this.inputHidden.value = this.selectPage.value.trim();
                    tp = UrlTypeEnum.Local;
                } else if (this.inputUrl) {
                    this.inputHidden.value = this.inputUrl.value.trim();
                    tp = UrlTypeEnum.Remote;
                } else
                    throw "Can't determine UrlType";
            } else {
                tp = Number(this.selectType.value) as UrlTypeEnum;
                switch (tp) {
                    case UrlTypeEnum.Local:
                        if (this.divLocal)
                            this.divLocal.style.display = "";
                        if (this.divRemote)
                            this.divRemote.style.display = "none";
                        if (this.selectPage)
                            this.inputHidden.value = this.selectPage.value.trim();
                        break;
                    case UrlTypeEnum.Remote:
                        if (this.divLocal)
                            this.divLocal.style.display = "none";
                        if (this.divRemote)
                            this.divRemote.style.display = "";
                        if (this.inputUrl)
                            this.inputHidden.value = this.inputUrl.value.trim();
                        break;
                }
            }
            let url = this.inputHidden.value.trim();
            if (url && url.length > 0) {
                if (tp === UrlTypeEnum.Local) {

                } else {
                    this.aLink.href = url;
                }
                this.aLink.style.display = "";
            } else {
                this.aLink.href = "";
                this.aLink.style.display = "none";
            }
        }
        private sendEvent(): void {
            FormsSupport.validateElement(this.inputHidden);
            let event = document.createEvent("Event");
            event.initEvent(UrlEditComponent.EVENTCHANGE, true, true);
            this.Control.dispatchEvent(event);
        }

        // API

        get value(): string {
            return this.inputHidden.value;
        }
        set value(url: string) {
            if (this.selectType) {
                let sel = Number(this.selectType.value) as UrlTypeEnum;// current selection
                if (this.Setup.Type === UrlTypeEnum.Local + UrlTypeEnum.Remote && this.selectPage) {
                    if (url != null && (url.startsWith("//") || url.startsWith("http"))) {
                        // remote
                        if (this.inputUrl)
                            sel = UrlTypeEnum.Remote;
                    } else {
                        // try local
                        this.selectPage.value = url;
                        if (this.selectPage.value !== url)
                            sel = UrlTypeEnum.Remote;// have to use remote as there was no match in the designed pages
                        else
                            sel = UrlTypeEnum.Local;
                    }
                } else if (this.Setup.Type === UrlTypeEnum.Local && this.selectPage) {
                    sel = UrlTypeEnum.Local;
                } else {
                    sel = UrlTypeEnum.Remote;
                }
                this.selectType.value = sel.toString();

                if (sel === UrlTypeEnum.Local && this.selectPage) {
                    this.selectPage.value = url;
                } else if (sel === UrlTypeEnum.Remote && this.inputUrl) {
                    this.inputUrl.value = url;
                }
            }
            this.inputHidden.value = url;

            this.updateStatus();
        }

        public clear(): void {
            this.value = "";
        }
        public enable(enabled: boolean): void {
            if (this.selectType)
                this.selectType.enable(enabled);
            if (this.selectPage)
                this.selectPage.enable(enabled);
            if (this.inputUrl)
                $YetaWF.elementEnableToggle(this.inputUrl, enabled);
        }
    }
}

