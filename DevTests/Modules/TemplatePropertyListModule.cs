/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplatePropertyListModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplatePropertyListModule>, IInstallableModel { }

    [ModuleGuid("{e356901b-fd5f-49d9-a438-af7f6c491c9e}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplatePropertyListModule : ModuleDefinition {

        public TemplatePropertyListModule() {
            Title = this.__ResStr("modTitle", "PropertyList Test Template");
            Name = this.__ResStr("modName", "Template Test - PropertyList");
            Description = this.__ResStr("modSummary", "PropertyList test template");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplatePropertyListModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
