/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Modules.Dashboard.Modules;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Views.Shared;
#endif

namespace YetaWF.Modules.Dashboard.Controllers {

    public class HttpModulesBrowseModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.HttpModulesBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };
                    return actions;
                }
            }
            [Caption("Module"), Description("The module name")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }

            private HttpModulesBrowseModule Module { get; set; }

            public BrowseItem(HttpModulesBrowseModule module, string name) {
                Module = module;
                Name = name;
            }
        }

        public class BrowseModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }
        [HttpGet]
        public ActionResult HttpModulesBrowse() {
            BrowseModel model = new BrowseModel { };
#if MVC6
#else
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("HttpModulesBrowse_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
            };
#endif
            return View(model);
        }

#if MVC6
#else
        [HttpPost]
        [ConditionalAntiForgeryToken]
        public ActionResult HttpModulesBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            HttpApplication httpApps = HttpContext.ApplicationInstance;
            HttpModuleCollection httpModuleCollections = httpApps.Modules;
            List<BrowseItem> items = (from k in httpModuleCollections.AllKeys select new BrowseItem(Module, k)).ToList();
            int total = items.Count;
            items = DataProviderImpl<BrowseItem>.GetRecords(items, skip, take, sort, filters, out total);
            GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
            return GridPartialView(new DataSourceResult {
                Data = items.ToList<object>(),
                Total = total
            });
        }
#endif
    }
}
