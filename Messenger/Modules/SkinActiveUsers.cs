/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Messenger.Modules {

    public class SkinActiveUsersModuleDataProvider : ModuleDefinitionDataProvider<Guid, SkinActiveUsersModule>, IInstallableModel { }

    [ModuleGuid("{E3542667-1884-4bac-BC58-F3CA458199F3}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SkinActiveUsersModule : ModuleDefinition {

        public SkinActiveUsersModule() {
            Title = this.__ResStr("modTitle", "Skin Active Users");
            Name = this.__ResStr("modName", "Active Users (Skin)");
            Description = this.__ResStr("modSummary", "Skin module used to keep a list of active users");
            WantFocus = false;
            ShowTitle = false;
            WantSearch = false;
            Invokable = true;
            InvokeInPopup = false;
            InvokeInAjax = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SkinActiveUsersModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}
