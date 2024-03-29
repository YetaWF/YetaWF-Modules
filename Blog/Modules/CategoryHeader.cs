/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Blog.Modules {

    public class CategoryHeaderModuleDataProvider : ModuleDefinitionDataProvider<Guid, CategoryHeaderModule>, IInstallableModel { }

    [ModuleGuid("{7c3d3c99-78a0-4661-bbc7-77c71978463c}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    [ModuleCategory("Categories")]
    public class CategoryHeaderModule : ModuleDefinition {

        public CategoryHeaderModule() {
            Title = this.__ResStr("modTitle", "Blog Category Header");
            Name = this.__ResStr("modName", "Blog Category Header");
            Description = this.__ResStr("modSummary", "Displays a blog category header. Add this to a page with a Blog Module or an Entry Display Module to show the category information for the current blog category.");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CategoryHeaderModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}