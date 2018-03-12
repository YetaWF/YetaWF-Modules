/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Dashboard.DataProvider;
using System.Threading.Tasks;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Dashboard.Controllers {

    public class BrowseDataProvidersModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.BrowseDataProvidersModule> {

        public class BrowseItem {

            [Caption("I/O Mode"), Description("The I/O mode supported by this data provider")]
            [UIHint("String"), ReadOnly]
            public string IOModeName { get; set; }
            [Caption("Type Name"), Description("The type of the supported data provider")]
            [UIHint("String"), ReadOnly]
            public string TypeName { get; set; }
            [Caption("Implementation Type Name"), Description("The type of the implementation of the supported data provider")]
            [UIHint("String"), ReadOnly]
            public string TypeImplName { get; set; }

            public BrowseItem(DataProviderInfo data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        public class BrowseModel {
            [Caption(""), Description("")] // empty entries required so property is shown in property list (but with a suppressed label)
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
        }

        [AllowGet]
        public ActionResult BrowseDataProviders() {
            BrowseModel model = new BrowseModel { };
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("BrowseDataProviders_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> BrowseDataProviders_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            using (DataProviderInfoDataProvider dataProvider = new DataProviderInfoDataProvider()) {
                DataProviderGetRecords<DataProviderInfo> browseItems = dataProvider.GetItems(skip, take, sort, filters);
                GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return await GridPartialViewAsync(new DataSourceResult {
                    Data = (from s in browseItems.Data select new BrowseItem(s)).ToList<object>(),
                    Total = browseItems.Total
                });
            }
        }
    }
}
