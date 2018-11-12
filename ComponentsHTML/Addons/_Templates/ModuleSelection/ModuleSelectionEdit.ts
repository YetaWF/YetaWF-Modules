/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

namespace YetaWF_ComponentsHTML {

    interface Setup {
        AjaxUrl: string;
        AjaxUrlComplete: string;
    }

    export class ModuleSelectionComponent extends YetaWF.ComponentBaseDataImpl {

        public static SELECTOR: string = ".yt_moduleselection.t_edit";

        private Setup: Setup;
        private SelectPackage: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private SelectModule: YetaWF_ComponentsHTML.DropDownListEditComponent;
        private DivDescription: HTMLDivElement;
        private DivLink: HTMLDivElement;
        private ALink: HTMLAnchorElement;

        constructor(controlId: string, setup: Setup) {
            super(controlId);
            this.Setup = setup;

            this.SelectPackage = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector(".t_packages select", [this.Control]);
            this.SelectModule = YetaWF_ComponentsHTML.DropDownListEditComponent.getControlFromSelector(".t_select select", [this.Control]);
            this.DivDescription = $YetaWF.getElement1BySelector(".t_description", [this.Control]) as HTMLDivElement;
            this.DivLink = $YetaWF.getElement1BySelector(".t_link", [this.Control]) as HTMLDivElement;
            this.ALink = $YetaWF.getElement1BySelector("a", [this.DivLink ]) as HTMLAnchorElement;

            this.showDescription();

            this.SelectPackage.Control.addEventListener("dropdownlist_change", (evt: Event): void => {
                var data = { AreaName: this.SelectPackage.value };
                this.SelectModule.ajaxUpdate(data, this.Setup.AjaxUrl);
            });
            this.SelectModule.Control.addEventListener("dropdownlist_change", (evt: Event): void => {
                this.showDescription();
            });
        }

        private showDescription(): void {
            var modGuid = this.SelectModule.value;
            if (this.hasValue) {
                this.ALink.href = "/!Mod/" + modGuid; // Globals.ModuleUrl
                this.ALink.style.display = "inline-block";
                this.DivDescription.textContent = this.getDescriptionText();
                this.DivDescription.style.display = "block";
            } else {
                this.ALink.style.display = "none";
                this.DivDescription.style.display = "none";
                this.DivDescription.textContent = "";
            }
        }
        private getDescriptionText(): string | null {
            return this.SelectModule.getToolTip(this.SelectModule.selectedIndex);
        }

        // API

        get hasValue(): boolean {
            var modGuid = this.SelectModule.value;
            return (modGuid !== undefined && modGuid !== null && modGuid.length > 0 && modGuid !== "00000000-0000-0000-0000-000000000000");
        }
        get value(): string {
            return this.SelectModule.value;
        }
        public enable(enabled: boolean): void {
            this.SelectPackage.enable(enabled);
            this.SelectModule.enable(enabled);
            if (enabled && this.hasValue) {
                this.ALink.style.display = "inline-block";
                this.DivDescription.style.display = "block";
            } else {
                this.ALink.style.display = "none";
                this.DivDescription.style.display = "none";
            }
        }
        public clear(): void {
            this.SelectPackage.value = "";
            this.SelectModule.value = "";
            this.ALink.style.display = "none";
            this.DivDescription.style.display = "none";
            this.DivDescription.textContent = "";
        }
        public hasChanged(data: string): boolean {
            if (!this.hasValue) return false;
            var modGuid = this.SelectModule.value;
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
                        this.SelectPackage.value = data.extra;
                        this.SelectModule.value = modGuid;
                        this.showDescription();
                        FormsSupport.validateElement(this.SelectModule.Control);
                    },
                    (result: string): void => {
                        this.clear();
                        FormsSupport.validateElement(this.SelectModule.Control);
                    }
                );
            } else {
                this.clear();
            }
        }
    }
    // A <div> is being emptied. Destroy all controls the <div> may contain.
    $YetaWF.registerClearDiv((tag: HTMLElement): void => {
        ModuleSelectionComponent.clearDiv<ModuleSelectionComponent>(tag, ModuleSelectionComponent.SELECTOR);
    });
}


