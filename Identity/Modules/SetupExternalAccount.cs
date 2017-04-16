/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class ExternalAccountModuleDataProvider : ModuleDefinitionDataProvider<Guid, SetupExternalAccountModule>, IInstallableModel { }

    [ModuleGuid("{85E71C49-7C67-4119-AB60-8D3177B15040}")]
    public class SetupExternalAccountModule : ModuleDefinition {

        public SetupExternalAccountModule() {
            Title = this.__ResStr("title", "External Account Setup");
            Name = this.__ResStr("title", "External Account Setup");
            Description = this.__ResStr("modSummary", "External user account setup (used with external login providers only)");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new ExternalAccountModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}