/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Messenger.Modules {

    public class SkinBrowserNotificationsModuleDataProvider : ModuleDefinitionDataProvider<Guid, SkinBrowserNotificationsModule>, IInstallableModel { }

    [ModuleGuid("{9E074871-5C52-43af-8439-D14CB2FDA71D}")]
    [UniqueModule(UniqueModuleStyle.UniqueOnly)]
    public class SkinBrowserNotificationsModule : ModuleDefinition {

        public SkinBrowserNotificationsModule() {
            Title = this.__ResStr("modTitle", "Skin Web Browser Notifications");
            Name = this.__ResStr("modName", "Web Browser Notifications (Skin)");
            Description = this.__ResStr("modSummary", "Skin module used to support web browser notifications");
            WantFocus = false;
            ShowTitle = false;
            WantSearch = false;
            Invokable = true;
            InvokeInPopup = false;
            InvokeInAjax = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SkinBrowserNotificationsModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AnonymousLevel_DefaultAllowedRoles; } }
    }
}
