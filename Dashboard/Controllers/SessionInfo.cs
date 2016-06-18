/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.Dashboard.Controllers {

    public class SessionInfoModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.SessionInfoModule> {

        public SessionInfoModuleController() { }

        public class BrowseItem {
            [Caption("Key"), Description("The SessionState key")]
            [UIHint("String"), ReadOnly]
            public string Key { get; set; }
            [Caption("Value"), Description("The first 100 bytes of the SessionState value")]
            [UIHint("String"), ReadOnly]
            public string Value { get; set; }
            [Caption("Size"), Description("The size of the value (if available)")]
            [UIHint("FileSize"), ReadOnly]
            public long Size { get; set; }
        }

        public class DisplayModel {

            [Caption("Total Size"), Description("The approximate size of all SessionState items")]
            [UIHint("FileSize"), ReadOnly]
            public long TotalSize { get; set; }

            [Caption("SessionState Items"), Description("The SessionState keys and the values (either the data type or the first 100 bytes of data are shown)")]
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }

            public void SetData(System.Web.SessionState.HttpSessionState session) {
                ObjectSupport.CopyData(session, this);
            }
        }

        [HttpGet]
        public ActionResult SessionInfo() {
            DisplayModel model = new DisplayModel();
            model.SetData(Manager.CurrentSession);
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("SessionInfo_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
                SupportReload = false,
            };
            List<BrowseItem> items = GetAllItems();
            model.TotalSize = items.Sum(m => m.Size);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SessionInfo_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            int total;
            List<BrowseItem> items = DataProviderImpl<BrowseItem>.GetRecords(GetAllItems(), skip, take, sort, filters, out total);
            foreach (BrowseItem item in items)
                item.Value  = item.Value.PadRight(100, ' ').Substring(0, 100).TrimEnd();

            GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
            return GridPartialView(new DataSourceResult {
                Data = items.ToList<object>(),
                Total = total,
            });
        }

        private List<BrowseItem> GetAllItems() {
            System.Web.SessionState.HttpSessionState session = Manager.CurrentSession;
            List<BrowseItem> items = (from string item in session.Keys select new BrowseItem { Key = item, Value = (session[item] ?? "").ToString(), Size = -1 }).ToList();
            foreach (BrowseItem item in items) {
                object o = null;
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