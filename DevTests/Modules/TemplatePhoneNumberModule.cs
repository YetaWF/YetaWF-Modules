using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplatePhoneNumberModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplatePhoneNumberModule>, IInstallableModel { }

    [ModuleGuid("{16d600a0-3519-4cb3-a929-665b23a4347f}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplatePhoneNumberModule : ModuleDefinition {

        public TemplatePhoneNumberModule() {
            Title = this.__ResStr("modTitle", "PhoneNumber Test Template");
            Name = this.__ResStr("modName", "Template Test - PhoneNumber");
            Description = this.__ResStr("modSummary", "PhoneNumber test template");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplatePhoneNumberModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
