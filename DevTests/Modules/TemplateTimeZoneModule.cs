/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateTimeZoneModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateTimeZoneModule>, IInstallableModel { }

    [ModuleGuid("{e088faf1-ab2b-446f-b2f8-de0ffd7e4125}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateTimeZoneModule : ModuleDefinition {

        public TemplateTimeZoneModule() {
            Title = this.__ResStr("modTitle", "TimeZone Test Component");
            Name = this.__ResStr("modName", "Component Test - TimeZone");
            Description = this.__ResStr("modSummary", "Test module for the TimeZone component. A test page for this module can be found at Tests > Templates > TimeZone (standard YetaWF site).");
            DefaultViewName = StandardViews.EditApply;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateTimeZoneModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
