/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.ImageRepository.Modules {

    public class CKEBrowseImagesModuleDataProvider : ModuleDefinitionDataProvider<Guid, CKEBrowseImagesModule>, IInstallableModel { }

    [ModuleGuid("{079f499e-0ce2-4da0-a7e2-e036bc9c98ee}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class CKEBrowseImagesModule : ModuleDefinition {

        public CKEBrowseImagesModule() {
            Title = this.__ResStr("modTitle", "Select Image");
            Name = this.__ResStr("modName", "Select Image (CKEditor)");
            Description = this.__ResStr("modSummary", "CKEditor interface to select or upload an image");
            WantSearch = false;
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CKEBrowseImagesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}