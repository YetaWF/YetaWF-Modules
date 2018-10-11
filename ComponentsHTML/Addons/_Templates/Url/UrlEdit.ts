/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface UrlEditSetup {
        Type: UrlTypeEnum;
        Url: string;
    }
    enum UrlTypeEnum {//flags
        Local = 1, // Local Url starting with /
        Remote = 2, // Remote Url http:// https:// or /
    }
    export class UrlEditComponent extends YetaWF.ComponentBase<HTMLDivElement> {

        public static readonly SELECTOR: string = ".yt_url.t_edit";

        private Setup: UrlEditSetup;
        private inputHidden: HTMLInputElement;
        private selectType: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private selectPage: YetaWF_ComponentsHTML.DropDownListEditComponent | null = null;
        private inputUrl: HTMLInputElement | null = null;
        private divLocal: HTMLDivElement | null = null;
        private divRemote: HTMLDivElement | null = null;
        private aLink: HTMLAnchorElement;

        constructor(controlId: string, setup: UrlEditSetup) {
            super(controlId);
            this.Setup = setup;

            $YetaWF.addObjectDataById(controlId, this);

            this.inputHidden = $YetaWF.getElement1BySelector(".t_hidden", [this.Control]) as HTMLInputElement;
            this.selectType = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector("select.yt_urltype", [this.Control]);
            // tslint:disable-next-line:no-bitwise
            if (this.Setup.Type & UrlTypeEnum.Local) {
                this.selectPage = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector("select.yt_urldesignedpage", [this.Control]);
                this.divLocal = $YetaWF.getElement1BySelector(".t_local", [this.Control]) as HTMLDivElement;
            }
            // tslint:disable-next-line:no-bitwise
            if (this.Setup.Type & UrlTypeEnum.Remote) {
                this.inputUrl = $YetaWF.getElement1BySelector(".yt_urlremotepage", [this.Control]) as HTMLInputElement;
                this.divRemote = $YetaWF.getElement1BySelector(".t_remote", [this.Control]) as HTMLDivElement;
            }
            this.aLink = $YetaWF.getElement1BySelector(".t_link a", [this.Control]) as HTMLAnchorElement;

            this.value = this.Setup.Url;

            if (!this.inputUrl || !this.selectPage)
                this.selectType.enable(false);

            this.selectType.Control.addEventListener("dropdownlist_change", (evt: Event): void => {
                this.updateStatus();
            });
            if (this.selectPage) {
                this.selectPage.Control.addEventListener("dropdownlist_change", (evt: Event): void => {
                    this.updateStatus();
                });
            }
            if (this.inputUrl) {
                $YetaWF.registerMultipleEventHandlers(this.inputUrl, ["input", "change", "click", "keyup", "paste"], null, (ev: Event): boolean => { this.updateStatus(); return true; });
            }
        }

        private updateStatus(): void {
            var tp = Number(this.selectType.value) as UrlTypeEnum;
            var oldValue = this.inputHidden.value;
            switch (tp) {
                case UrlTypeEnum.Local:
                    if (this.divLocal)
                        this.divLocal.style.display = "";
                    if (this.divRemote)
                        this.divRemote.style.display = "none";
                    if (this.selectPage)
                        this.inputHidden.value = this.selectPage.Control.value.trim();
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

            var url = this.inputHidden.value.trim();
            if (url && url.length > 0) {
                if (tp === UrlTypeEnum.Local) {
                    var uri = $YetaWF.parseUrl(url);
                    uri.removeSearch(YConfigs.Basics.Link_NoEditMode);
                    uri.addSearch(YConfigs.Basics.Link_NoEditMode, "y");
                    this.aLink.href = uri.toUrl();
                } else {
                    this.aLink.href = url;
                }
                this.aLink.style.display = "";
            } else {
                this.aLink.href = "";
                this.aLink.style.display = "none";
            }

            if (oldValue !== url) {
                var event = document.createEvent("Event");
                event.initEvent("url_change", true, true);
                this.Control.dispatchEvent(event);
            }
        }

        // API

        get value(): string {
            return this.inputHidden.value;
        }
        set value(url: string) {
            var sel = Number(this.selectType.value) as UrlTypeEnum;// current selection
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
            this.inputHidden.value = url;
            this.selectType.value = sel.toString();

            if (sel === UrlTypeEnum.Local && this.selectPage) {
                this.selectPage.value = url;
            } else if (sel === UrlTypeEnum.Remote && this.inputUrl) {
                this.inputUrl.value = url;
            }
            this.updateStatus();
        }

        public clear(): void {
            this.value = "";
        }
        public enable(enabled: boolean): void {
            this.selectType.enable(enabled);
            if (this.selectPage)
                this.selectPage.enable(enabled);
            if (this.inputUrl)
                $YetaWF.elementEnableToggle(this.inputUrl, enabled);
        }

        public static getControlFromTag(elem: HTMLElement): UrlEditComponent { return super.getControlBaseFromTag<UrlEditComponent>(elem, UrlEditComponent.SELECTOR); }
        public static getControlFromSelector(selector: string | null, tags: HTMLElement[]): UrlEditComponent { return super.getControlBaseFromSelector<UrlEditComponent>(selector || UrlEditComponent.SELECTOR, UrlEditComponent.SELECTOR, tags); }
        public static getControlById(id: string): UrlEditComponent { return super.getControlBaseById<UrlEditComponent>(id, UrlEditComponent.SELECTOR); }

        public destroy(): void {
            $YetaWF.removeObjectDataById(this.Control.id);
        }
    }

    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        YetaWF.ComponentBase.clearDiv<UrlEditComponent>(tag, UrlEditComponent.SELECTOR, (control: UrlEditComponent): void => {
            control.destroy();
        });
    });
}

