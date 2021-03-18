/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using System.Threading.Tasks;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

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
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(SessionInfo_GridData)),
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
            model.GridDef = GetGridModel();

            List<BrowseItem> items = GetAllItems();
            model.TotalSize = items.Sum(m => m.Size);
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> SessionInfo_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<BrowseItem>(GetGridModel(), gridPVData);
        }

        private List<BrowseItem> GetAllItems() {
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

        [AllowPost]
        public ActionResult ClearAll() {
            Manager.SessionSettings.ClearAll(true);
            return Reload(null, PopupText: this.__ResStr("clearDone", "Sessions settings have been cleared"));
        }
    }
}
