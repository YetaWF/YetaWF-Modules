/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateModuleSelectionModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateModuleSelectionModule>, IInstallableModel { }

    [ModuleGuid("{06D2A133-9E6A-4c02-A2C5-A9963C5A9667}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateModuleSelectionModule : ModuleDefinition {

        public TemplateModuleSelectionModule() {
            Title = this.__ResStr("modTitle", "ModuleSelection Test Template");
            Name = this.__ResStr("modName", "Template Test - ModuleSelection");
            Description = this.__ResStr("modSummary", "ModuleSelection test template");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateModuleSelectionModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}