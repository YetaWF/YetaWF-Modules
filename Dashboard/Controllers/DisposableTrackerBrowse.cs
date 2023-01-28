/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

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
using YetaWF.Modules.Dashboard.Endpoints;
using YetaWF.Modules.Dashboard.Modules;

namespace YetaWF.Modules.Dashboard.Controllers {

    public class DisposableTrackerBrowseModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.DisposableTrackerBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands {
                get {
                    List<ModuleAction> actions = new List<ModuleAction>();
                    return actions;
                }
            }

            [Caption("Type"), Description("The object type of the disposable object that is currently in use")]
            [UIHint("String"), ReadOnly]
            public string DisposableObject { get; set; } = null!;
            [Caption("Time"), Description("The time the object was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }
            [Caption("Callstack"), Description("The callstack at the time the object was created")]
            [UIHint("String"), ReadOnly]
            public string? CallStack { get; set; }

            public DisposableTrackerBrowseModule Module { get; set; }

            public BrowseItem(DisposableTrackerBrowseModule module, TrackedEntry data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
                DisposableObject = data.DisposableObject.GetType().FullName!;
            }
        }
        public class BrowseModel {
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;
        }
        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                InitialPageSize = 20,
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = Utility.UrlFor<DisposableTrackerBrowseModuleEndpoints>(GridSupport.BrowseGridData),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                    DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(data, skip, take, sorts, filters);
                    foreach (BrowseItem r in recs.Data)
                        r.Module = (DisposableTrackerBrowseModule)module;
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
                DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    List<BrowseItem> items = (from k in DisposableTracker.GetDisposableObjects() select new BrowseItem((DisposableTrackerBrowseModule)module, k)).ToList();
                    DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(items, skip, take, sort, filters);
                    DataSourceResult data = new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total
                    };
                    return Task.FromResult(data);
                },
            };
        }

        [AllowGet]
        public ActionResult DisposableTrackerBrowse() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel(Module)
            };
            return View(model);
        }
    }
}
