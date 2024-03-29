/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Scheduler.Controllers;
using YetaWF.Modules.Scheduler.DataProvider;

namespace YetaWF.Modules.Scheduler.Modules {

    public class LogBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, LogBrowseModule>, IInstallableModel { }

    [ModuleGuid("{5ababac3-7319-40ba-b73c-54b3946489bb}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class LogBrowseModule : ModuleDefinition {

        public LogBrowseModule() {
            Title = this.__ResStr("modTitle", "Scheduler Log");
            Name = this.__ResStr("modName", "Scheduler Log");
            Description = this.__ResStr("modSummary", "Displays and manages the scheduler log.");
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new LogBrowseModuleDataProvider(); }

        [Category("General"), Caption("Display Url"), Description("The Url to display a log entry - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string? DisplayUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemoveLog",
                        this.__ResStr("roleRemLogC", "Remove Log"), this.__ResStr("roleRemLog", "The role has permission to clear the scheduler log"),
                        this.__ResStr("userRemLogC", "Remove Log"), this.__ResStr("userRemLog", "The user has permission to clear the scheduler log")),
                    new RoleDefinition("Downloads",
                        this.__ResStr("roleDownloadC", "Download Log"), this.__ResStr("roleDownload", "The role has permission to download the scheduler log file"),
                        this.__ResStr("userDownloadC", "Download Log"), this.__ResStr("userDownload", "The user has permission to download the scheduler log file")),
                };
            }
        }

        public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
            if (location == ModuleAction.ActionLocationEnum.ModuleLinks) {
                using (LogDataProvider logDP = new LogDataProvider()) {
                    if (logDP.CanBrowse)
                        menuList.New(await GetAction_RemoveAllAsync());
                    else {
                        menuList.New(await GetAction_DownloadZippedLogAsync());
                        menuList.New(await GetAction_DownloadLogAsync());
                    }
                }
            }
            return menuList;
        }

        public ModuleAction? GetAction_Items(string? url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Log"),
                MenuText = this.__ResStr("browseText", "Log"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage the scheduler log"),
                Legend = this.__ResStr("browseLegend", "Displays and manages the scheduler log"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public async Task<ModuleAction?> GetAction_RemoveAllAsync() {
            if (!IsAuthorized("RemoveLog")) return null;
            using (LogDataProvider logDP = new LogDataProvider()) {
                if (!await logDP.IsInstalledAsync()) return null;
            };
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(LogBrowseModuleController), "RemoveAll"),
                NeedsModuleContext = true,
                Image = await CustomIconAsync("RemoveAll.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeAllLink", "Remove All"),
                MenuText = this.__ResStr("removeAllMenu", "Remove All"),
                Tooltip = this.__ResStr("removeAllTT", "Remove all scheduler log records"),
                Legend = this.__ResStr("removeAllLegend", "Removes all scheduler log records"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove ALL scheduler log records?"),
            };
        }
        public async Task<ModuleAction?> GetAction_DownloadLogAsync() {
            if (!IsAuthorized("Downloads")) return null;
            using (LogDataProvider logDP = new LogDataProvider()) {
                if (!await logDP.IsInstalledAsync()) return null;
            };
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(LogBrowseModuleController), "DownloadLog"),
                NeedsModuleContext = true,
                CookieAsDoneSignal = true,
                Image = await CustomIconAsync("Download.png"),
                LinkText = this.__ResStr("downloadLink", "Download Log"),
                MenuText = this.__ResStr("downloadMenu", "Download Log"),
                Tooltip = this.__ResStr("downloadTT", "Download the scheduler log file"),
                Legend = this.__ResStr("downloadLegend", "Downloads the scheduler log file"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
            };
        }
        public async Task<ModuleAction?> GetAction_DownloadZippedLogAsync() {
            if (!IsAuthorized("Downloads")) return null;
            using (LogDataProvider logDP = new LogDataProvider()) {
                if (!await logDP.IsInstalledAsync()) return null;
            };
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(LogBrowseModuleController), "DownloadZippedLog"),
                NeedsModuleContext = true,
                CookieAsDoneSignal = true,
                Image = await CustomIconAsync("Download.png"),
                LinkText = this.__ResStr("dlZipLink", "Download Log (Zipped)"),
                MenuText = this.__ResStr("dlZipMenu", "Download Log (Zipped)"),
                Tooltip = this.__ResStr("dlZipTT", "Download the scheduler log file as a ZIP file"),
                Legend = this.__ResStr("dlZipLegend", "Downloads the scheduler log file as a ZIP file"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
            };
        }
    }
}
