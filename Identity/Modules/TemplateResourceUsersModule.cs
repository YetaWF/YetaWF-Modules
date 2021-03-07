using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

#nullable enable

namespace YetaWF.Modules.Identity.Modules {

    public class TemplateResourceUsersModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateResourceUsersModule>, IInstallableModel { }

    [ModuleGuid("{fc6b0bad-5416-4fb0-b9d1-e5a02359a7b9}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateResourceUsersModule : ModuleDefinition {

        public TemplateResourceUsersModule() {
            Title = this.__ResStr("modTitle", "ResourceUsers Test Template");
            Name = this.__ResStr("modName", "Template Test - ResourceUsers");
            Description = this.__ResStr("modSummary", "ResourceUsers test template");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateResourceUsersModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
