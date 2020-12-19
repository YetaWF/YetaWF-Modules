/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateMultiStringModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateMultiStringModule>, IInstallableModel { }

    [ModuleGuid("{4485D52E-A8E3-40fa-9251-B6F7A34CEDA1}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateMultiStringModule : ModuleDefinition {

        public TemplateMultiStringModule() {
            Title = this.__ResStr("modTitle", "MultiString Test Component");
            Name = this.__ResStr("modName", "Component Test - MultiString");
            Description = this.__ResStr("modSummary", "Test module for the MultiString component. A test page for this module can be found at Tests > Templates > MultiString (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateMultiStringModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
