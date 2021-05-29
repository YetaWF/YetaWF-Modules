using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateMaskedEditModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateMaskedEditModule>, IInstallableModel { }

    [ModuleGuid("{8494d18a-a8da-40e4-a3d6-05fb75b76d53}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateMaskedEditModule : ModuleDefinition {

        public TemplateMaskedEditModule() {
            Title = this.__ResStr("modTitle", "MaskedEdit Test Template");
            Name = this.__ResStr("modName", "Template Test - MaskedEdit");
            Description = this.__ResStr("modSummary", "MaskedEdit test template");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateMaskedEditModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
