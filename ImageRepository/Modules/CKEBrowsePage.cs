/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ImageRepository#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.ImageRepository.Modules {

    public class CKEBrowsePageModuleDataProvider : ModuleDefinitionDataProvider<Guid, CKEBrowsePageModule>, IInstallableModel { }

    [ModuleGuid("{3645C411-EF04-4fd3-8B87-9E37B803C4B5}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class CKEBrowsePageModule : ModuleDefinition {

        public CKEBrowsePageModule() {
            Title = this.__ResStr("modTitle", "Select Page");
            Name = this.__ResStr("modName", "Select Page (CKEditor)");
            Description = this.__ResStr("modSummary", "Displays a form to select a local page URL.");
            WantSearch = false;
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CKEBrowsePageModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}