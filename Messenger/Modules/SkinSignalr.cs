/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Messenger.Modules {

    public class SkinSignalrModuleDataProvider : ModuleDefinitionDataProvider<Guid, SkinSignalrModule>, IInstallableModel { }

    [ModuleGuid("{F2C8660A-19DC-4001-B1E1-A02AA07AE39B}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SkinSignalrModule : ModuleDefinition {

        public SkinSignalrModule() {
            Title = this.__ResStr("modTitle", "Skin SignalR");
            Name = this.__ResStr("modName", "SignalR (Skin)");
            Description = this.__ResStr("modSummary", "Skin module used to activate SignalR");
            WantFocus = false;
            ShowTitle = false;
            WantSearch = false;
            Invokable = true;
            InvokeInPopup = false;
            InvokeInAjax = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SkinSignalrModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}
