"use strict";
/* Copyright � 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DockerRegistry#License */
var YetaWF_DockerRegistry;
(function (YetaWF_DockerRegistry) {
    var BrowseRegistriesModule = /** @class */ (function () {
        function BrowseRegistriesModule(modId) {
            var _this = this;
            this.Module = $YetaWF.getElementById(modId);
            this.RegistryId = YetaWF.ComponentBaseDataImpl.getControlFromSelector("[name='RegistryId']", YetaWF_ComponentsHTML.DropDownListEditComponent.SELECTOR, [this.Module]);
            $YetaWF.registerCustomEventHandler(this.RegistryId, "dropdownlist_change", function (ev) {
                if (_this.RegistryId.value) {
                    var uri = $YetaWF.parseUrl(window.location.href);
                    uri.removeSearch("RegistryId");
                    uri.addSearch("RegistryId", _this.RegistryId.value);
                    $YetaWF.ContentHandling.setNewUri(uri);
                }
            });
        }
        return BrowseRegistriesModule;
    }());
    YetaWF_DockerRegistry.BrowseRegistriesModule = BrowseRegistriesModule;
})(YetaWF_DockerRegistry || (YetaWF_DockerRegistry = {}));

//# sourceMappingURL=BrowseRegistries.js.map
