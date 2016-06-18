/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/DevTests#License */

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
            Title = this.__ResStr("modTitle", "Url Test Template");
            Name = this.__ResStr("modName", "Template Test - Url");
            Description = this.__ResStr("modSummary", "Url test template");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateTestUrlModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}