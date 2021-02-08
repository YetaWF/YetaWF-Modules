/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

#nullable enable

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateCheckListModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateCheckListModule>, IInstallableModel { }

    [ModuleGuid("{bf1751a7-d7bb-4338-8f66-420edc135b8b}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    //[ModuleCategory("...")]
    public class TemplateCheckListModule : ModuleDefinition {

        public TemplateCheckListModule() {
            Title = this.__ResStr("modTitle", "CheckList Test Template");
            Name = this.__ResStr("modName", "Template Test - CheckList");
            Description = this.__ResStr("modSummary", "CheckList test template");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateCheckListModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
