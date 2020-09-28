/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateProgressBarModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateProgressBarModule>, IInstallableModel { }

    [ModuleGuid("{d5fcf9ab-06a7-456b-8d7c-2e0de134b1e8}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateProgressBarModule : ModuleDefinition {

        public TemplateProgressBarModule() {
            Title = this.__ResStr("modTitle", "ProgressBar Test Template");
            Name = this.__ResStr("modName", "Template Test - ProgressBar");
            Description = this.__ResStr("modSummary", "ProgressBar test template");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateProgressBarModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
