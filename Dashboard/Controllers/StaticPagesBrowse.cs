/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support.StaticPages;
using YetaWF.Modules.Dashboard.Modules;
using System.Threading.Tasks;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Dashboard.Controllers {

    public class StaticPagesBrowseModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.StaticPagesBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };
                    actions.New(Module.GetAction_Remove(LocalUrl), ModuleAction.ActionLocationEnum.GridLinks);
                    return actions;
                }
            }

            [Caption("Local Url"), Description("The local Url of the static page")]
            [UIHint("Url"), ReadOnly]
            public string LocalUrl { get; set; }
            [Caption("Type"), Description("The type of storage used for the static page")]
            [UIHint("Enum"), ReadOnly]
            public StaticPageManager.PageEntryEnum StorageType { get; set; }
            [Caption("Local Files"), Description("The local file(s) containing the contents of the static page")]
            [UIHint("ListOfStrings"), AdditionalMetadata("Delimiter", "<br/>"), ReadOnly]
            public List<string> FileNames { get; set; }

            public StaticPagesBrowseModule Module { get; set; }

            public BrowseItem(StaticPagesBrowseModule module, StaticPageManager.PageEntry data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
                FileNames = new List<string> {
                    data.FileName ?? "-",
                    data.FileNameHttps ?? "-",
                    data.FileNamePopup ?? "-",
                    data.FileNamePopupHttps ?? "-",
                };
            }
            public BrowseItem() { }
        }

        public class BrowseModel {
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                InitialPageSize = 20,
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(StaticPagesBrowse_GridData)),
                SortFilterStaticData = (List<object> data, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) => {
                    DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(data, skip, take, sorts, filters);
                    foreach (BrowseItem r in recs.Data)
                        r.Module = Module;
                    return new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = recs.Total,
                    };
                },
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    List<BrowseItem> items = (from k in await Manager.StaticPageManager.GetSiteStaticPagesAsync() select new BrowseItem(Module, k)).ToList();
                    int total = items.Count;
                    DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(items, skip, take, sort, filters);
                    DataSourceResult data = new DataSourceResult {
                        Data = recs.Data.ToList<object>(),
                        Total = total
                    };
                    return data;
                },
            };
        }

        [AllowGet]
        public ActionResult StaticPagesBrowse() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StaticPagesBrowse_GridData(string data, string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync<BrowseItem>(GetGridModel(), data, fieldPrefix, skip, take, sorts, filters);
        }

        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(string localUrl) {
            await Manager.StaticPageManager.RemovePageAsync(localUrl);
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> RemoveAll() {
            await Manager.StaticPageManager.RemoveAllPagesAsync();
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
    }
}
