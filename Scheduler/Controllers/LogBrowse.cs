/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

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
using YetaWF.Modules.Scheduler.DataProvider;
using YetaWF.Modules.Scheduler.Endpoints;
using YetaWF.Modules.Scheduler.Modules;

namespace YetaWF.Modules.Scheduler.Controllers {

    public class LogBrowseModuleController : ControllerImpl<YetaWF.Modules.Scheduler.Modules.LogBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands {
                get {
                    List<ModuleAction> actions = new List<ModuleAction>();

                    LogDisplayModule dispMod = new LogDisplayModule();
                    actions.New(dispMod.GetAction_Display(Module.DisplayUrl, LogEntry), ModuleAction.ActionLocationEnum.GridLinks);

                    return actions;
                }
            }

            public int LogEntry { get; set; }

            [Caption("Created"), Description("The date/time this log record was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime TimeStamp { get; set; }

            [Caption("Id"), Description("The id of the scheduler item run")]
            [UIHint("LongValue"), ReadOnly]
            public long RunId { get; set; }

            [Caption("Name"), Description("The name of the running scheduler item")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; } = null!;

            [Caption("Level"), Description("The message level")]
            [UIHint("Enum"), ReadOnly]
            public Core.Log.Logging.LevelEnum Level { get; set; }

            [Caption("Site Id"), Description("The site which was affected by the scheduler item")]
            [UIHint("SiteId"), ReadOnly]
            public int SiteIdentity { get; set; }

            [Caption("Message"), Description("The message")]
            [UIHint("String"), ReadOnly]
            public string? Info { get; set; }

            private LogBrowseModule Module { get; set; }

            public BrowseItem(LogBrowseModule module, LogData data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }

        public class BrowseModel {
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;
            public bool LogAvailable { get; set; }
            public bool BrowsingSupported { get; set; }
        }
        
        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                SizeStyle = GridDefinition.SizeStyleEnum.SizeToFit,
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                InitialPageSize = 20,
                AjaxUrl = Utility.UrlFor<LogBrowseModuleEndpoints>(GridSupport.BrowseGridData),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    using (LogDataProvider logDP = new LogDataProvider()) {
                        DataProviderGetRecords<LogData> browseItems = await logDP.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem((LogBrowseModule)module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public async Task<ActionResult> LogBrowse() {
            using (LogDataProvider logDP = new LogDataProvider()) {
                BrowseModel model = new BrowseModel {
                    LogAvailable = await logDP.IsInstalledAsync(),
                    BrowsingSupported = logDP.CanBrowse,
                };
                if (logDP.CanBrowse)
                    model.GridDef = GetGridModel(Module);
                return View(model);
            }
        }
    }
}
