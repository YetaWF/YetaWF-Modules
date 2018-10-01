/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using System.Threading.Tasks;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YetaWF.Core.DataProvider;
#endif

namespace YetaWF.Modules.Dashboard.Controllers {

    public class HttpModulesBrowseModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.HttpModulesBrowseModule> {

        public class BrowseItem {

            [Caption("Module"), Description("The module name")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }

            public BrowseItem(string name) {
                Name = name;
            }
            public BrowseItem() { }
        }

        public class BrowseModel {
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(HttpModulesBrowse_GridData)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(data, skip, take, sorts, filters);
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
                DirectDataAsync = (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    HttpApplication httpApps = HttpContext.ApplicationInstance;
                    HttpModuleCollection httpModuleCollections = httpApps.Modules;
                    List<BrowseItem> items = (from k in httpModuleCollections.AllKeys select new BrowseItem(k)).ToList();
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
        public ActionResult HttpModulesBrowse() {
            BrowseModel model = new BrowseModel { };
#if MVC6
#else
            model.GridDef = GetGridModel();
#endif
            return View(model);
        }

#if MVC6
#else
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> HttpModulesBrowse_GridData(string data, string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync<BrowseItem>(GetGridModel(), data, fieldPrefix, skip, take, sorts, filters);
        }
#endif
    }
}
