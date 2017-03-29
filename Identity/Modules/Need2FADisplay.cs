/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class Need2FADisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, Need2FADisplayModule>, IInstallableModel { }

    [ModuleGuid("{661bb0dd-769f-4850-bd6f-3d1c563e84b2}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class Need2FADisplayModule : ModuleDefinition {

        public Need2FADisplayModule() {
            Title = this.__ResStr("modTitle", "Two-Step Authentication Setup Required");
            Name = this.__ResStr("modName", "Two-Step Authentication Setup Required");
            Description = this.__ResStr("modSummary", "Displays a warning that the user must complete two-step authentication setup");

            Invokable = true;
            ShowTitle = false;
            WantFocus = false;
            WantSearch = false;
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new Need2FADisplayModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}
