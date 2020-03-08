/* Copyright � 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateTabsModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateTabsModule>, IInstallableModel { }

    [ModuleGuid("{806e260e-608a-4ff1-97cc-58c3f8421ae6}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateTabsModule : ModuleDefinition {

        public TemplateTabsModule() {
            Title = this.__ResStr("modTitle", "Tabs Test Template");
            Name = this.__ResStr("modName", "Template Test - Tabs");
            Description = this.__ResStr("modSummary", "Tabs test template");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateTabsModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
