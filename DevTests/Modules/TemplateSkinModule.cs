/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateSkinModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateSkinModule>, IInstallableModel { }

    [ModuleGuid("{b49b3d02-29d4-4466-9ebd-20209f0ba7de}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateSkinModule : ModuleDefinition {

        public TemplateSkinModule() {
            Title = this.__ResStr("modTitle", "Skin Selection Test Component");
            Name = this.__ResStr("modName", "Component Test - Skin Selection");
            Description = this.__ResStr("modSummary", "Skin selection test component");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateSkinModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
