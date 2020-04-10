/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

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
using YetaWF.Modules.Logging.Controllers;
using YetaWF.Modules.LoggingDataProvider.DataProvider;
using YetaWF.Core.Components;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Logging.Modules {

    public class BrowseLogModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowseLogModule>, IInstallableModel { }

    [ModuleGuid("{62298abd-1b32-4c04-9477-cba2277f03e6}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class BrowseLogModule : ModuleDefinition {

        public BrowseLogModule() : base() {
            Title = this.__ResStr("modTitle", "Log");
            Name = this.__ResStr("modName", "Log");
            Description = this.__ResStr("modSummary", "Displays and manages log records. The Log module can be accessed using Admin > Dashboard > Logging (standard YetaWF site).");
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BrowseLogModuleDataProvider(); }

        [Category("General"), Caption("Display URL"), Description("The URL to display a log record - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string DisplayUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemoveLog",
                        this.__ResStr("roleRemLogC", "Remove Log"), this.__ResStr("roleRemLog", "The role has permission to clear the log"),
                        this.__ResStr("userRemLogC", "Remove Log"), this.__ResStr("userRemLog", "The user has permission to clear the log")),
                    new RoleDefinition("Downloads",
                        this.__ResStr("roleDownloadC", "Download Log"), this.__ResStr("roleDownload", "The role has permission to download the log file"),
                        this.__ResStr("userDownloadC", "Download Log"), this.__ResStr("userDownload", "The user has permission to download the log file")),
                };
            }
        }

        public override async Task<MenuList> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = await base.GetModuleMenuListAsync(renderMode, location);
            if (location == ModuleAction.ActionLocationEnum.ModuleLinks) {
                using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                    menuList.New(await GetAction_RemoveAllAsync());
                    menuList.New(await GetAction_DownloadZippedLogAsync());
                    menuList.New(await GetAction_DownloadLogAsync());
                }
            }
            return menuList;
        }

        public ModuleAction GetAction_Logging(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Logging"),
                MenuText = this.__ResStr("browseText", "Logging"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage log records"),
                Legend = this.__ResStr("browseLegend", "Displays and manages log records"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public async Task<ModuleAction> GetAction_RemoveAllAsync() {
            if (!IsAuthorized("RemoveLog")) return null;
            using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                if (!await dataProvider.IsInstalledAsync()) return null;
                if (!dataProvider.CanRemove) return null;
            };
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(BrowseLogModuleController), nameof(BrowseLogModuleController.RemoveAll)),
                NeedsModuleContext = true,
                Image = await CustomIconAsync("RemoveAll.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeAllLink", "Remove All"),
                MenuText = this.__ResStr("removeAllMenu", "Remove All"),
                Legend = this.__ResStr("removeAllLegend", "Remove all log record for all sites"),
                Tooltip = this.__ResStr("removeAllTT", "Removes all log records for all sites"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove ALL log records?"),
            };
        }
        public async Task<ModuleAction> GetAction_DownloadLogAsync() {
            if (!IsAuthorized("Downloads")) return null;
            using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                if (!await dataProvider.IsInstalledAsync()) return null;
                if (!dataProvider.CanDownload) return null;
            };
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(BrowseLogModuleController), nameof(BrowseLogModuleController.DownloadLog)),
                NeedsModuleContext = true,
                CookieAsDoneSignal = true,
                Image = await CustomIconAsync("Download.png"),
                LinkText = this.__ResStr("downloadLink", "Download Log"),
                MenuText = this.__ResStr("downloadMenu", "Download Log"),
                Tooltip = this.__ResStr("downloadTT", "Download the log file"),
                Legend = this.__ResStr("downloadLegend", "Downloads the log file"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
            };
        }
        public async Task<ModuleAction> GetAction_DownloadZippedLogAsync() {
            if (!IsAuthorized("Downloads")) return null;
            using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                if (!await dataProvider.IsInstalledAsync()) return null;
                if (!dataProvider.CanDownload) return null;
            };
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(BrowseLogModuleController), nameof(BrowseLogModuleController.DownloadZippedLog)),
                NeedsModuleContext = true,
                CookieAsDoneSignal = true,
                Image = await CustomIconAsync("Download.png"),
                LinkText = this.__ResStr("dlZipLink", "Download Log (Zipped)"),
                MenuText = this.__ResStr("dlZipMenu", "Download Log (Zipped)"),
                Tooltip = this.__ResStr("dlZipTT", "Download the log file as a ZIP file"),
                Legend = this.__ResStr("dlZipLegend", "Downloads the log file as a ZIP file"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
            };
        }
    }
}