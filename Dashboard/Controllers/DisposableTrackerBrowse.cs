/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Dashboard.Modules;

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
            public object DisposableObject { get; set; }
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

        [HttpGet]
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

        [HttpPost]
        [ConditionalAntiForgeryToken]
        public ActionResult DisposableTrackerBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            List<BrowseItem> items = (from k in DisposableTracker.GetDisposableObjects() select new BrowseItem(Module, k.Value)).ToList();
            int total = items.Count;
            items = DataProviderImpl<BrowseItem>.GetRecords(items, skip, take, sort, filters, out total);
            GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
            return GridPartialView(new DataSourceResult {
                Data = items.ToList<object>(),
                Total = total
            });
        }
    }
}
