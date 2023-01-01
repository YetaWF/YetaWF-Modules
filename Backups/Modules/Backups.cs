/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Backups#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Backups.Controllers;

namespace YetaWF.Modules.Backups.Modules {

    public class BackupsModuleDataProvider : ModuleDefinitionDataProvider<Guid, BackupsModule>, IInstallableModel { }

    [ModuleGuid("{C819DFC0-2A7D-4263-9388-0AEE779001B2}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class BackupsModule : ModuleDefinition {

        public BackupsModule() : base() {
            Title = this.__ResStr("modTitle", "Site Backups");
            Name = this.__ResStr("modName", "Backups");
            Description = this.__ResStr("modSummary", "Manages site backups.");
            DefaultViewName = StandardViews.Browse;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BackupsModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("Backups",
                        this.__ResStr("roleBackupsC", "Manage Backups"), this.__ResStr("roleBackups", "The role has permission to create and remove backups"),
                        this.__ResStr("userBackupsC", "Manage Backups"), this.__ResStr("userBackups", "The user has permission to create and remove backups")),
                    new RoleDefinition("Downloads",
                        this.__ResStr("roleDownloadsC", "Manage Backups"), this.__ResStr("roleDownloads", "The role has permission to download backups"),
                        this.__ResStr("userDownloadsC", "Manage Backups"), this.__ResStr("userDownloads", "The user has permission to download backups")),
                };
            }
        }

        public ModuleAction? GetAction_Backups(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Backups"),
                MenuText = this.__ResStr("browseText", "Backups"),
                Tooltip = this.__ResStr("browseTooltip", "Manage site backups"),
                Legend = this.__ResStr("browseLegend", "Manages site backups"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public async Task<ModuleAction?> GetAction_PerformSiteBackupAsync() {
            if (!IsAuthorized("Backups")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(BackupsModuleController), nameof(BackupsModuleController.PerformSiteBackup)),
                NeedsModuleContext = true,
                Image = await CustomIconAsync("SiteBackup.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("sbackupLink", "Site Backup"),
                MenuText = this.__ResStr("sbackupMenu", "Site Backup"),
                Tooltip = this.__ResStr("sbackupTT", "Create a site backup in the backup folder"),
                Legend = this.__ResStr("sbackupLegend", "Creates a site backup in the backup folder"),
                Category = ModuleAction.ActionCategoryEnum.Significant,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                ConfirmationText = this.__ResStr("sbackupConfirm", "Are you sure you want to create a site backup?"),
                PleaseWaitText = this.__ResStr("sbackupPleaseWait", "Your site backup is in progress..."),
            };
        }

        // This action is only useful if you're the YetaWF Publisher
        public async Task<ModuleAction?> GetAction_MakeSiteTemplateDataAsync() {
            if (YetaWFManager.Deployed) return null; //Can't make site template data on a deployed site
            if (!IsAuthorized("Backups")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(BackupsModuleController), nameof(BackupsModuleController.MakeSiteTemplateData)),
                NeedsModuleContext = true,
                Image = await CustomIconAsync("SiteBackup.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("makeTemplateLink", "Make Site Template Data"),
                MenuText = this.__ResStr("makeTemplateMenu", "Make Site Template Data"),
                Tooltip = this.__ResStr("makeTemplateTT", "Create site templates for installation of a new YetaWF site using the current site"),
                Legend = this.__ResStr("makeTemplateLegend", "Creates site templates for installation of a new YetaWF site using the current site"),
                Category = ModuleAction.ActionCategoryEnum.Significant,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                ConfirmationText = this.__ResStr("makeTemplateConfirm", "Are you sure you want to make site template data?"),
            };
        }

        public async Task<ModuleAction?> GetAction_DownloadLinkAsync(string filename) {
            if (!IsAuthorized("Downloads")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(BackupsModuleController), nameof(BackupsModuleController.Download)),
                QueryArgs = new { FileName = filename },
                Image = await CustomIconAsync("Download.png"),
                NeedsModuleContext = true,
                CookieAsDoneSignal = true,
                LinkText = this.__ResStr("dnldLink", "Download Backup"),
                MenuText = this.__ResStr("dnldMenu", "Download Backup"),
                Tooltip = this.__ResStr("dnldTT", "Download the site backup (zip file)"),
                Legend = this.__ResStr("dnldLegend", "Downloads the site backup (zip file)"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
            };
        }
        public ModuleAction? GetAction_RemoveLink(string filename) {
            if (!IsAuthorized("Backups")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(BackupsModuleController), "Remove"),
                QueryArgs = new { FileName = filename },
                NeedsModuleContext = true,
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove Backup"),
                MenuText = this.__ResStr("removeMenu", "Remove Backup"),
                Tooltip = this.__ResStr("removeTT", "Remove the site backup (zip file)"),
                Legend = this.__ResStr("removeLegend", "Removes the site backup (zip file)"),
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove \"{0}\"?", filename),
            };
        }
    }
}