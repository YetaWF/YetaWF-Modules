/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface Setup {
        AjaxUrl: string;
        AjaxUrlComplete: string;
    }

    export class ModuleSelectionEditComponent extends YetaWF.ComponentBaseDataImpl {

        public static readonly TEMPLATE: string = "yt_moduleselection";
        public static SELECTOR: string = ".yt_moduleselection.t_edit";

        private Setup: Setup;
        private Hidden: HTMLInputElement;
        private SelectPackage: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private SelectModule: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private DivDescription: HTMLDivElement;
        private DivEditSettings: HTMLDivElement | null;
        private DivLink: HTMLDivElement;
        private ALink: HTMLAnchorElement;

        constructor(controlId: string, setup: Setup) {
            super(controlId, ModuleSelectionEditComponent.TEMPLATE, ModuleSelectionEditComponent.SELECTOR, {
                ControlType: ControlTypeEnum.Template,
                ChangeEvent: null,
                GetValue: (control: ModuleSelectionEditComponent): string | null => {
                    return control.value;
                },
                Enable: (control: ModuleSelectionEditComponent, enable: boolean, clearOnDisable: boolean): void => {
                    control.enable(enable);
                    if (!enable && clearOnDisable)
                        control.clear();
                },
            });

            this.Setup = setup;

            this.Hidden = $YetaWF.getElement1BySelector("input[type='hidden']", [this.Control]) as HTMLInputElement;
            this.SelectPackage = YetaWF.ComponentBaseDataImpl.getControlFromSelector(".t_packages select", DropDownListEditComponent.SELECTOR, [this.Control]);
            this.SelectModule = YetaWF.ComponentBaseDataImpl.getControlFromSelector(".t_select select", DropDownListEditComponent.SELECTOR, [this.Control]);
            this.DivDescription = $YetaWF.getElement1BySelector(".t_description", [this.Control]) as HTMLDivElement;
            this.DivEditSettings = $YetaWF.getElement1BySelectorCond(".t_editsettings", [this.Control]) as HTMLDivElement;
            this.DivLink = $YetaWF.getElement1BySelector(".t_link", [this.Control]) as HTMLDivElement;
            this.ALink = $YetaWF.getElement1BySelector("a", [this.DivLink ]) as HTMLAnchorElement;

            this.showDescription();

            this.SelectPackage.Control.addEventListener(DropDownListEditComponent.EVENTCHANGE, (evt: Event): void => {
                let data = { AreaName: this.SelectPackage.value };
                this.SelectModule.ajaxUpdate(data, this.Setup.AjaxUrl);
            });
            this.SelectModule.Control.addEventListener(DropDownListEditComponent.EVENTCHANGE, (evt: Event): void => {
                let modGuid = this.SelectModule.value;
                this.Hidden.value = modGuid;
                this.showDescription();
                FormsSupport.validateElement(this.Hidden);
            });
        }

        private showDescription(): void {
            let modGuid = this.Hidden.value;
            if (this.hasValue) {
                this.ALink.href = "/!Mod/" + modGuid; // Globals.ModuleUrl
                this.ALink.style.display = "inline-block";
                this.DivDescription.textContent = this.getDescriptionText();
                this.DivDescription.style.display = "block";
                if (this.DivEditSettings) {
                    this.updateEditSettings(modGuid);
                    this.DivEditSettings.style.display = "block";
                }
            } else {
                this.ALink.style.display = "none";
                this.DivDescription.style.display = "none";
                this.DivDescription.textContent = "";
                if (this.DivEditSettings)
                    this.DivEditSettings.style.display = "none";
            }
        }
        private getDescriptionText(): string | null {
            return this.SelectModule.getToolTip(this.SelectModule.selectedIndex);
        }
        private updateEditSettings(modGuid: string): void {
            if (this.DivEditSettings) {
                let anchor = $YetaWF.getElement1BySelector("a", [this.DivEditSettings]) as HTMLAnchorElement;
                let uri = new YetaWF.Url();
                uri.parse(anchor.href);
                uri.removeSearch("ModuleGuid");
                uri.addSearch("ModuleGuid", modGuid);
                anchor.href = uri.toUrl();
            }
        }

        // API

        get hasValue(): boolean {
            var modGuid = this.Hidden.value;
            return (modGuid !== undefined && modGuid !== null && modGuid.length > 0 && modGuid !== "00000000-0000-0000-0000-000000000000");
        }
        get value(): string {
            return this.Hidden.value;
        }
        public enable(enabled: boolean): void {
            $YetaWF.elementEnableToggle(this.Hidden, enabled);
            this.SelectPackage.enable(enabled);
            this.SelectModule.enable(enabled);
            if (enabled && this.hasValue) {
                this.ALink.style.display = "inline-block";
                this.DivDescription.style.display = "block";
                if (this.DivEditSettings)
                    this.DivEditSettings.style.display = "block";
            } else {
                this.ALink.style.display = "none";
                this.DivDescription.style.display = "none";
                if (this.DivEditSettings)
                    this.DivEditSettings.style.display = "none";
            }
        }
        public clear(): void {
            this.Hidden.value = "";
            this.SelectPackage.value = "";
            this.SelectModule.value = "";
            this.showDescription();
        }
        public hasChanged(data: string): boolean {
            if (!this.hasValue) return false;
            var modGuid = this.Hidden.value;
            return modGuid !== data;
        }
        /**
         * Load object with data. Selects the correct package in the dropdownlist and selects the module (the package is detected using ajax).
         */
        public updateComplete(modGuid: string): void {

            if (modGuid !== undefined && modGuid !== null && modGuid.length > 0 && modGuid !== "00000000-0000-0000-0000-000000000000") {

                var data = { "modGuid": modGuid };
                this.SelectModule.ajaxUpdate(data, this.Setup.AjaxUrlComplete,
                    (data: any): void => {
                        this.Hidden.value = modGuid;
                        this.SelectPackage.value = data.extra;
                        this.SelectModule.value = modGuid;
                        this.showDescription();
                        FormsSupport.validateElement(this.Hidden);
                    },
                    (result: string): void => {
                        this.clear();
                        FormsSupport.validateElement(this.Hidden);
                    }
                );
            } else {
                this.clear();
            }
        }
    }
}


