/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using Microsoft.AspNetCore.Mvc;
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
using YetaWF.Modules.Dashboard.DataProvider;
using YetaWF.Modules.Dashboard.Endpoints;

namespace YetaWF.Modules.Dashboard.Controllers {

    public class BrowseDataProvidersModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.BrowseDataProvidersModule> {

        public class BrowseItem {

            [Caption("I/O Mode"), Description("The I/O mode supported by this data provider")]
            [UIHint("String"), ReadOnly]
            public string IOModeName { get; set; } = null!;
            [Caption("Type Name"), Description("The type of the supported data provider")]
            [UIHint("String"), ReadOnly]
            public string TypeName { get; set; } = null!;
            [Caption("Implementation Type Name"), Description("The type of the implementation of the supported data provider")]
            [UIHint("String"), ReadOnly]
            public string TypeImplName { get; set; } = null!;

            public BrowseItem(DataProviderInfo data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        public class BrowseModel {
            [Caption(""), Description("")] // empty entries required so property is shown in property list (but with a suppressed label)
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;
        }
        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = Utility.UrlFor<BrowseDataProvidersModuleEndpoints>(GridSupport.BrowseGridData),
                DirectDataAsync = (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    using (DataProviderInfoDataProvider dataProvider = new DataProviderInfoDataProvider()) {
                        DataProviderGetRecords<DataProviderInfo> browseItems = dataProvider.GetItems(skip, take, sort, filters);
                        DataSourceResult data = new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem(s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                        return Task.FromResult(data);
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult BrowseDataProviders() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel(Module)
            };
            return View(model);
        }
    }
}
