/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.DevTests.Modules {

    public class TestStepsModuleDataProvider : ModuleDefinitionDataProvider<Guid, TestStepsModule>, IInstallableModel { }

    [ModuleGuid("{7a151158-7afd-48ad-95f5-011034760111}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class TestStepsModule : ModuleDefinition {

        public TestStepsModule() {
            Title = this.__ResStr("modTitle", "Steps Test");
            Name = this.__ResStr("modName", "Test - Steps");
            Description = this.__ResStr("modSummary", "Test module for the Display Steps Module (YetaWF.Panels). A test page for this module can be found at Tests > Modules > Steps (standard YetaWF site).");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new TestStepsModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    }
}
