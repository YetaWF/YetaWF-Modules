/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TemplateDateTimeModuleDataProvider : ModuleDefinitionDataProvider<Guid, TemplateDateTimeModule>, IInstallableModel { }

    [ModuleGuid("{2c538164-9151-46b1-8118-c93bbcedd23f}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TemplateDateTimeModule : ModuleDefinition {

        public TemplateDateTimeModule() {
            Title = this.__ResStr("modTitle", "DateTime Test Template");
            Name = this.__ResStr("modName", "Template Test - DateTime");
            Description = this.__ResStr("modSummary", "DateTime test template");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TemplateDateTimeModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}