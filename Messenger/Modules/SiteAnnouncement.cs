/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Messenger.Modules
{
    public class SiteAnnouncementModuleDataProvider : ModuleDefinitionDataProvider<Guid, SiteAnnouncementModule>, IInstallableModel { }

    [ModuleGuid("{bace50b3-7508-4df9-9e90-62cfd2a7a1a1}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class SiteAnnouncementModule : ModuleDefinition {

        public SiteAnnouncementModule() {
            Title = this.__ResStr("modTitle", "New Site Announcement");
            Name = this.__ResStr("modName", "Site Announcement");
            Description = this.__ResStr("modSummary", "Sends a new site announcement to all users");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SiteAnnouncementModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Send(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Add",
                LinkText = this.__ResStr("addLink", "Send Announcement"),
                MenuText = this.__ResStr("addText", "Send Announcement"),
                Tooltip = this.__ResStr("addTooltip", "Send a new site announcement to all users"),
                Legend = this.__ResStr("addLegend", "Sends a new site announcement to all users"),
                Style = ModuleAction.ActionStyleEnum.Popup,
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                SaveReturnUrl = true,
            };
        }
    }
}

