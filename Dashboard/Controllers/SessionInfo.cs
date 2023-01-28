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

namespace YetaWF.Modules.Dashboard.Controllers {

    public class SessionInfoModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.SessionInfoModule> {

        public SessionInfoModuleController() { }

        public class BrowseItem {
            [Caption("Key"), Description("The SessionState key")]
            [UIHint("String"), ReadOnly]
            public string Key { get; set; } = null!;
            [Caption("Value"), Description("The first 100 bytes of the SessionState value")]
            [UIHint("String"), ReadOnly]
            public string Value { get; set; } = null!;
            [Caption("Size"), Description("The size of the value (if available)")]
            [UIHint("FileFolderSize"), ReadOnly]
            public long Size { get; set; }
        }

        public class DisplayModel {

            [Caption("Total Size"), Description("The approximate size of all SessionState items")]
            [UIHint("FileFolderSize"), ReadOnly]
            public long TotalSize { get; set; }

            [Caption("SessionState Items"), Description("The SessionState keys and the values (either the data type or the first 100 bytes of data are shown)")]
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;

            public void SetData(SessionState session) { }
        }
        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = Utility.UrlFor<SessionInfoModuleEndpoints>(GridSupport.BrowseGridData),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo>? sorts, List<DataProviderFilterInfo>? filters) => {
                    DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
                DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    DataProviderGetRecords<BrowseItem> items = DataProviderImpl<BrowseItem>.GetRecords(GetAllItems(), skip, take, sort, filters);
                    foreach (BrowseItem item in items.Data)
                        item.Value = item.Value.PadRight(100, ' ').Substring(0, 100).TrimEnd();

                    DataSourceResult data = new DataSourceResult {
                        Data = items.Data.ToList<object>(),
                        Total = items.Total,
                    };
                    return Task.FromResult(data);
                },
            };
        }

        [AllowGet]
        public ActionResult SessionInfo() {
            DisplayModel model = new DisplayModel();
            model.SetData(Manager.CurrentSession);
            model.GridDef = GetGridModel(Module);

            List<BrowseItem> items = GetAllItems();
            model.TotalSize = items.Sum(m => m.Size);
            return View(model);
        }

        private static List<BrowseItem> GetAllItems() {
            SessionState session = Manager.CurrentSession;
            List<BrowseItem> items = (from string item in session.Keys select new BrowseItem { Key = item, Value = (session[item] ?? string.Empty).ToString()!, Size = -1 }).ToList();
            foreach (BrowseItem item in items) {
                object? o = null;
                try {
                    o = session[item.Key];
                } catch (Exception) { }
                if (o != null) {
                    if (o as byte[] != null)
                        item.Size = ((byte[])o).Length;
                    else if (o as string != null)
                        item.Size = item.Value.Length;
                    // add more types as needed
                }
            }
            return items;
        }
    }
}
