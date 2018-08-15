/* Copyright � 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Dashboard.Modules;
using System.Threading.Tasks;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Dashboard.Controllers {

    public class DisposableTrackerBrowseModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.DisposableTrackerBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };
                    return actions;
                }
            }

            [Caption("Type"), Description("The object type of the disposable object that is currently in use")]
            [UIHint("String"), ReadOnly]
            public string DisposableObject { get; set; }
            [Caption("Time"), Description("The time the object was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }
            [Caption("Callstack"), Description("The callstack at the time the object was created")]
            [UIHint("String"), ReadOnly]
            public string CallStack { get; set; }

            private DisposableTrackerBrowseModule Module { get; set; }

            public BrowseItem(DisposableTrackerBrowseModule module, TrackedEntry data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }

        public class BrowseModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        [AllowGet]
        public ActionResult DisposableTrackerBrowse() {
            BrowseModel model = new BrowseModel { };
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("DisposableTrackerBrowse_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> DisposableTrackerBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            List<BrowseItem> items = (from k in DisposableTracker.GetDisposableObjects() select new BrowseItem(Module, k)).ToList();
            DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(items, skip, take, sort, filters);
            Grid.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
            return await GridPartialViewAsync(new DataSourceResult {
                Data = recs.Data.ToList<object>(),
                Total = recs.Total
            });
        }
    }
}
