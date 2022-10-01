/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateReferencedModulesModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateReferencedModulesModule>, IInstallableModel { }

    [ModuleGuid("{8c9deda8-82e5-44fa-9aba-d1625b3c277d}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateReferencedModulesModule : ModuleDefinition {

        public TemplateReferencedModulesModule() {
            Title = this.__ResStr("modTitle", "ReferencedModules Test Component");
            Name = this.__ResStr("modName", "Component Test - ReferencedModules");
            Description = this.__ResStr("modSummary", "Test module for the ReferencedModules component. A test page for this module can be found at Tests > Templates > ReferencedModules (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateReferencedModulesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
