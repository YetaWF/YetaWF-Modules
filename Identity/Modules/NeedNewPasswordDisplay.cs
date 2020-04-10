/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Identity.Modules {

    public class NeedNewPasswordDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, NeedNewPasswordDisplayModule>, IInstallableModel { }

    [ModuleGuid("{E6B2C413-EBD6-439c-B69A-586C49BF17E7}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    [ModuleCategory("Login & Registration")]
    public class NeedNewPasswordDisplayModule : ModuleDefinition {

        public NeedNewPasswordDisplayModule() {
            Title = this.__ResStr("modTitle", "New Password Required");
            Name = this.__ResStr("modName", "New Password Required");
            Description = this.__ResStr("modSummary", "Displays a warning that the user must change the login password.");

            Invokable = true;
            ShowTitle = false;
            WantFocus = false;
            WantSearch = false;
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new NeedNewPasswordDisplayModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}
