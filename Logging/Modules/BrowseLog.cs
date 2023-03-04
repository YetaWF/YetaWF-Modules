/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Logging.Endpoints;
using YetaWF.Modules.LoggingDataProvider.DataProvider;

namespace YetaWF.Modules.Logging.Modules;

public class BrowseLogModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowseLogModule>, IInstallableModel { }

[ModuleGuid("{62298abd-1b32-4c04-9477-cba2277f03e6}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class BrowseLogModule : ModuleDefinition2 {

    public BrowseLogModule() : base() {
        Title = this.__ResStr("modTitle", "Log");
        Name = this.__ResStr("modName", "Log");
        Description = this.__ResStr("modSummary", "Displays and manages log records. The Log module can be accessed using Admin > Dashboard > Logging (standard YetaWF site).");
        UsePartialFormCss = false;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new BrowseLogModuleDataProvider(); }

    [Category("General"), Caption("Display Url"), Description("The Url to display a log record - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? DisplayUrl { get; set; }

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

    public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
        List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
        if (location == ModuleAction.ActionLocationEnum.ModuleLinks) {
            using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                menuList.New(await GetAction_RemoveAllAsync());
                menuList.New(await GetAction_DownloadZippedLogAsync());
                menuList.New(await GetAction_DownloadLogAsync());
            }
        }
        return menuList;
    }

    public ModuleAction? GetAction_Logging(string url) {
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
    public async Task<ModuleAction?> GetAction_RemoveAllAsync() {
        if (!IsAuthorized("RemoveLog")) return null;
        using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
            if (!await dataProvider.IsInstalledAsync()) return null;
            if (!dataProvider.CanRemove) return null;
        };
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(BrowseLogModuleEndpoints), BrowseLogModuleEndpoints.RemoveAll),
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
    public async Task<ModuleAction?> GetAction_DownloadLogAsync() {
        if (!IsAuthorized("Downloads")) return null;
        using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
            if (!await dataProvider.IsInstalledAsync()) return null;
            if (!dataProvider.CanDownload) return null;
        };
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(BrowseLogModuleEndpoints), BrowseLogModuleEndpoints.DownloadLog),
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
    public async Task<ModuleAction?> GetAction_DownloadZippedLogAsync() {
        if (!IsAuthorized("Downloads")) return null;
        using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
            if (!await dataProvider.IsInstalledAsync()) return null;
            if (!dataProvider.CanDownload) return null;
        };
        return new ModuleAction(this) {
            Url = Utility.UrlFor(typeof(BrowseLogModuleEndpoints), BrowseLogModuleEndpoints.DownloadZippedLog),
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

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands {
            get {
                List<ModuleAction> actions = new List<ModuleAction>();

                DisplayLogModule dispMod = new DisplayLogModule();
                actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Key), ModuleAction.ActionLocationEnum.GridLinks);

                return actions;
            }
        }

        public int Key { get; set; }

        [Caption("Date/Time"), Description("The time this record was logged")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime TimeStamp { get; set; }

        [Caption("Category"), Description("The log category")]
        [UIHint("String"), ReadOnly]
        public string Category { get; set; } = null!;

        [Caption("Session Id"), Description("The session id used to identify the visitor")]
        [UIHint("String"), ReadOnly]
        public string SessionId { get; set; } = null!;

        [Caption("Level"), Description("The error level of this log record")]
        [UIHint("Enum"), ReadOnly]
        public YetaWF.Core.Log.Logging.LevelEnum Level { get; set; }

        [Caption("Info"), Description("The information logged in this record")]
        [UIHint("String"), ReadOnly]
        public string Info { get; set; } = null!;

        [Caption("Site Id"), Description("The site which logged this record")]
        [UIHint("IntValue"), ReadOnly]
        public int SiteIdentity { get; set; }

        [Caption("IP Address"), Description("The IP address associated with this log entry")]
        [UIHint("IPAddress"), ReadOnly]
        public string IPAddress { get; set; } = null!;
        [Caption("Url"), Description("The requested Url")]
        [UIHint("Url"), ReadOnly]
        public string RequestedUrl { get; set; } = null!;
        [UIHint("Url"), ReadOnly]
        [Caption("Referrer"), Description("The referring Url associated with this log entry")]
        public string ReferrerUrl { get; set; } = null!;

        [Caption("User"), Description("The user's name/email address (if available)")]
        [UIHint("YetaWF_Identity_UserId"), ReadOnly]
        public int UserId { get; set; }

        [Caption("Module Name"), Description("The module logging this record")]
        [UIHint("String"), ReadOnly]
        public string ModuleName { get; set; } = null!;
        [Caption("Class"), Description("The class logging this record")]
        [UIHint("String"), ReadOnly]
        public string Class { get; set; } = null!;
        [Caption("Method"), Description("The method logging this record")]
        [UIHint("String"), ReadOnly]
        public string Method { get; set; } = null!;
        [Caption("Namespace"), Description("The namespace logging this record")]
        [UIHint("String"), ReadOnly]
        public string Namespace { get; set; } = null!;

        private BrowseLogModule Module { get; set; }

        public BrowseItem(BrowseLogModule module, LogRecord data) {
            Module = module;
            ObjectSupport.CopyData(data, this);
        }
    }

    public class BrowseModel {
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;
        public bool LogAvailable { get; set; }
        public bool BrowsingSupported { get; set; }
        public string LoggerName { get; set; } = null!;
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            PageSizes = new List<int>() { 5, 10, 20, 50 },
            InitialPageSize = 20,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<BrowseLogModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                FlushLog();
                using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                    DataProviderGetRecords<LogRecord> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(this, s)).ToList<object>(),
                        Total = browseItems.Total
                    };
                }
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        FlushLog();
        using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
            await dataProvider.FlushAsync();// get the latest records
            BrowseModel model = new BrowseModel {
                LogAvailable = await dataProvider.IsInstalledAsync(),
                BrowsingSupported = dataProvider.CanBrowse,
                LoggerName = dataProvider.LoggerName,
            };
            if (dataProvider.CanBrowse)
                model.GridDef = GetGridModel();
            return await RenderAsync(model);
        }
    }

    internal static void FlushLog() {
        YetaWF.Core.Log.Logging.ForceFlush();
    }
}