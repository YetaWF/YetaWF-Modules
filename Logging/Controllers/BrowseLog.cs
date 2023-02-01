/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Logging.Endpoints;
using YetaWF.Modules.Logging.Modules;
using YetaWF.Modules.LoggingDataProvider.DataProvider;

namespace YetaWF.Modules.Logging.Controllers {

    public class BrowseLogModuleController : ControllerImpl<YetaWF.Modules.Logging.Modules.BrowseLogModule> {

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
        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                PageSizes = new List<int>() { 5, 10, 20, 50 },
                InitialPageSize = 20,
                RecordType = typeof(BrowseItem),
                AjaxUrl = Utility.UrlFor<BrowseLogModuleEndpoints>(GridSupport.BrowseGridData),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    FlushLog();
                    using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                        DataProviderGetRecords<LogRecord> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem((BrowseLogModule)module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public async Task<ActionResult> BrowseLog() {
            FlushLog();
            using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                await dataProvider.FlushAsync();// get the latest records
                BrowseModel model = new BrowseModel {
                    LogAvailable = await dataProvider.IsInstalledAsync(),
                    BrowsingSupported = dataProvider.CanBrowse,
                    LoggerName = dataProvider.LoggerName,
                };
                if (dataProvider.CanBrowse)
                    model.GridDef = GetGridModel(Module);
                return View(model);
            }
        }

        internal static void FlushLog() {
            YetaWF.Core.Log.Logging.ForceFlush();
        }
    }
}