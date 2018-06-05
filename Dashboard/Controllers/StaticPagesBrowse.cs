/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

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

            private StaticPagesBrowseModule Module { get; set; }

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
        }

        public class BrowseModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        [AllowGet]
        public ActionResult StaticPagesBrowse() {
            BrowseModel model = new BrowseModel { };
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("StaticPagesBrowse_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
            };
            return View(model);
        }

        [AllowPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> StaticPagesBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            List<BrowseItem> items = (from k in await Manager.StaticPageManager.GetSiteStaticPagesAsync() select new BrowseItem(Module, k)).ToList();
            int total = items.Count;
            DataProviderGetRecords<BrowseItem> recs = DataProviderImpl<BrowseItem>.GetRecords(items, skip, take, sort, filters);
            Grid.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
            return await GridPartialViewAsync(new DataSourceResult {
                Data = recs.Data.ToList<object>(),
                Total = total
            });
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
