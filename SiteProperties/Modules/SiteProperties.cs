/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SiteProperties#License */

using System;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.SiteProperties.Modules {

    public class SitePropertiesModuleDataProvider : ModuleDefinitionDataProvider<Guid, SitePropertiesModule>, IInstallableModel { }

    [ModuleGuid("522296A0-B03B-49b7-B849-AB4149466E0D")] // Published Guid
    public class SitePropertiesModule : ModuleDefinition {

        public SitePropertiesModule() {
            Title = this.__ResStr("modTitle", "Site Settings");
            Name = this.__ResStr("modName", "Site Settings");
            Description = this.__ResStr("modSummary", "Changes site settings");
        }
        public override IModuleDefinitionIO GetDataProvider() { return new SitePropertiesModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_EditCurrentSite(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Edit",
                LinkText = this.__ResStr("editSiteLink", "Site Settings"),
                MenuText = this.__ResStr("editSiteText", "Site Settings"),
                Tooltip = this.__ResStr("editSiteTooltip", "Change settings for the current site"),
                Legend = this.__ResStr("editSiteLegend", "Changes settings for the current site"),
                Location = ModuleAction.ActionLocationEnum.MainMenu,
                Mode = ModuleAction.ActionModeEnum.View,
                Style = ModuleAction.ActionStyleEnum.Popup,
                SaveReturnUrl = true,
            };
        }

        public ModuleAction GetAction_EditSite(string url, string domain) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { Domain = domain },
                Image = "#Edit",
                LinkText = this.__ResStr("editLink", "Site Settings"),
                MenuText = this.__ResStr("editText", "Site Settings"),
                Tooltip = this.__ResStr("editTooltip", "Change settings for site \"{0}\"", domain),
                Legend = this.__ResStr("editLegend", "Changes settings for site \"{0}\"", domain),
                Location = ModuleAction.ActionLocationEnum.MainMenu,
                Mode = ModuleAction.ActionModeEnum.View,
                Style = ModuleAction.ActionStyleEnum.Popup,
                SaveReturnUrl = true,
            };
        }

        // Properties used to save initial settings from Templates
        [DontSave]
        public SiteDefinition CurrentSite {
            get {
                return Manager.CurrentSite;
            }
        }
        public void InitComplete() {
            bool restartRequired;
            Manager.CurrentSite.Save(out restartRequired);
            if (restartRequired) throw new InternalError("Adding a new site implementation error - restart required");
        }
    }
}