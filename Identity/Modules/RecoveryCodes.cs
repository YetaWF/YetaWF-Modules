/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class RecoveryCodesModuleDataProvider : ModuleDefinitionDataProvider<Guid, RecoveryCodesModule>, IInstallableModel { }

    [ModuleGuid("{e26230b2-a603-4a54-97ca-1e1b0b400d19}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class RecoveryCodesModule : ModuleDefinition {

        public RecoveryCodesModule() {
            Title = this.__ResStr("modTitle", "Recovery Codes");
            Name = this.__ResStr("modName", "Recovery Codes");
            Description = this.__ResStr("modSummary", "Manages Two Step Authentication Recovery Codes");
            DefaultViewName = StandardViews.Edit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new RecoveryCodesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return UserLevel_DefaultAllowedRoles; } }
    }
}
