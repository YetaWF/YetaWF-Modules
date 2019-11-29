/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DockerRegistry#License */

namespace YetaWF_DockerRegistry {

    export class BrowseRegistriesModule {

        private Module: HTMLDivElement;
        private RegistryId: YetaWF_ComponentsHTML.DropDownListEditComponent;

        constructor(modId: string) {

            this.Module = $YetaWF.getElementById(modId) as HTMLDivElement;

            this.RegistryId = YetaWF.ComponentBaseDataImpl.getControlFromSelector("[name='RegistryId']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Module]);

            $YetaWF.registerCustomEventHandler(this.RegistryId, "dropdownlist_change", (ev: Event): void => {
                if (this.RegistryId.value) {
                    var uri = $YetaWF.parseUrl(window.location.href);
                    uri.removeSearch("RegistryId");
                    uri.addSearch("RegistryId", this.RegistryId.value);
                    $YetaWF.ContentHandling.setNewUri(uri);
                }
            });
        }
    }
}
