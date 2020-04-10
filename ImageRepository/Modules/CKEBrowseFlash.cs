/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.ImageRepository.Modules {

    public class CKEBrowseFlashModuleDataProvider : ModuleDefinitionDataProvider<Guid, CKEBrowseFlashModule>, IInstallableModel { }

    [ModuleGuid("{253BBD32-7377-4e42-9AB6-49D5F434309A}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class CKEBrowseFlashModule : ModuleDefinition {

        public CKEBrowseFlashModule() {
            Title = this.__ResStr("modTitle", "Select Flash Image");
            Name = this.__ResStr("modName", "Select Flash Image (CKEditor)");
            Description = this.__ResStr("modSummary", "Displays a form to upload and select a Flash image.");
            WantSearch = false;
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CKEBrowseFlashModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}