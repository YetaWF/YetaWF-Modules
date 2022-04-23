/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateEmailModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateEmailModule>, IInstallableModel { }

    [ModuleGuid("{d7c549fe-09f2-494f-8ac7-db332d579589}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateEmailModule : ModuleDefinition {

        public TemplateEmailModule() {
            Title = this.__ResStr("modTitle", "Email Test Component");
            Name = this.__ResStr("modName", "Component Test - Email");
            Description = this.__ResStr("modSummary", "Test module for the Email component. A test page for this module can be found at Tests > Templates > Email (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateEmailModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
