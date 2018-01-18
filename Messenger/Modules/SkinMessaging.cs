/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Messenger.Modules {

    public class SkinMessagingModuleDataProvider : ModuleDefinitionDataProvider<Guid, SkinMessagingModule>, IInstallableModel { }

    [ModuleGuid("{0B230757-67E5-43F1-9423-3B092EC78519}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SkinMessagingModule : ModuleDefinition {

        public SkinMessagingModule() {
            Title = this.__ResStr("modTitle", "Skin Messaging");
            Name = this.__ResStr("modName", "Messaging (Skin)");
            Description = this.__ResStr("modSummary", "Skin module used to support intra-user messaging");
            WantFocus = false;
            ShowTitle = false;
            WantSearch = false;
            Invokable = true;
            InvokeInPopup = false;
            InvokeInAjax = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SkinMessagingModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}
