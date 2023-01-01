/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateTestUrlModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateTestUrlModule>, IInstallableModel { }

    [ModuleGuid("{fbcc7d7a-5090-48a7-9958-6eff8b8c6d7e}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateTestUrlModule : ModuleDefinition {

        public TemplateTestUrlModule() {
            Title = this.__ResStr("modTitle", "Url Test Component");
            Name = this.__ResStr("modName", "Component Test - Url");
            Description = this.__ResStr("modSummary", "Test module for the Url component. A test page for this module can be found at Tests > Templates > Url (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateTestUrlModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
