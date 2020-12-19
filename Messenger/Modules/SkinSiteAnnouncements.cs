/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Messenger.Modules {

    public class SkinSiteAnnouncementsModuleDataProvider : ModuleDefinitionDataProvider<Guid, SkinSiteAnnouncementsModule>, IInstallableModel { }

    [ModuleGuid("{54F6B691-B835-4568-90AA-AA9B308D4272}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SkinSiteAnnouncementsModule : ModuleDefinition {

        public SkinSiteAnnouncementsModule() {
            Title = this.__ResStr("modTitle", "Skin Site Announcements");
            Name = this.__ResStr("modName", "Site Announcements (Skin)");
            Description = this.__ResStr("modSummary", "Skin module used to support site announcements");
            WantFocus = false;
            ShowTitle = false;
            WantSearch = false;
            Invokable = true;
            InvokeInPopup = false;
            InvokeInAjax = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SkinSiteAnnouncementsModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}
