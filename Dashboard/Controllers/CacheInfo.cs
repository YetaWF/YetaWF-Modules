/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Views.Shared;
#endif

namespace YetaWF.Modules.Dashboard.Controllers {

    public class CacheInfoModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.CacheInfoModule> {

        public CacheInfoModuleController() { }

        public class BrowseItem {
            [Caption("Key"), Description("The cache key")]
            [UIHint("String"), ReadOnly]
            public string Key { get; set; }
            [Caption("Value"), Description("The first 100 bytes of the cache value")]
            [UIHint("String"), ReadOnly]
            public string Value { get; set; }
            [Caption("Size"), Description("The size of the value (if available)")]
            [UIHint("FileSize"), ReadOnly]
            public long Size { get; set; }
        }

        public class DisplayModel {

            [Caption("Memory Limit"), Description("The percentage of physical memory available to the application")]
            [UIHint("LongValue"), ReadOnly]
            public long EffectivePercentagePhysicalMemoryLimit { get; set; }

            [Caption("Bytes Limit"), Description("The number of bytes available for the cache")]
            [UIHint("FileSize"), ReadOnly]
            public long EffectivePrivateBytesLimit { get; set; }

            [Caption("Total Size"), Description("The approximate size of all cached items")]
            [UIHint("FileSize"), ReadOnly]
            public long TotalSize { get; set; }

            [Caption("Cached Items"), Description("The cache keys and the values (either the data type or the first 100 bytes of data are shown)")]
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
#if MVC6
#else
            public void SetData(System.Web.Caching.Cache data) {
                ObjectSupport.CopyData(data, this);
            }
#endif
        }

        [HttpGet]
        public ActionResult CacheInfo() {
            DisplayModel model = new DisplayModel();
#if MVC6
#else
            model.SetData(System.Web.HttpRuntime.Cache);
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("CacheInfo_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
                SupportReload = false,
            };
            List<BrowseItem> items = GetAllItems();
            model.TotalSize = items.Sum(m => m.Size);
#endif
            return View(model);
        }


#if MVC6
#else
        [HttpPost]
        [ConditionalAntiForgeryToken]
        public ActionResult CacheInfo_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
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
            System.Web.Caching.Cache cache = System.Web.HttpRuntime.Cache;
            List<BrowseItem> items = (from DictionaryEntry item in cache select new BrowseItem { Key = item.Key.ToString(), Value = (item.Value ?? "").ToString(), Size = -1 }).ToList();
            foreach (BrowseItem item in items) {
                object o = null;
                try {
                    o = cache[item.Key];
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
#endif
    }
}