/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support.StaticPages;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Dashboard.Modules;
using YetaWF.Core.Support;
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

        [HttpGet]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StaticPagesBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            List<BrowseItem> items = (from k in Manager.StaticPageManager.GetSiteStaticPages() select new BrowseItem(Module, k)).ToList();
            int total = items.Count;
            items = DataProviderImpl<BrowseItem>.GetRecords(items, skip, take, sort, filters, out total);
            GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
            return GridPartialView(new DataSourceResult {
                Data = items.ToList<object>(),
                Total = total
            });
        }

        [HttpPost]
        [ExcludeDemoMode]
        public ActionResult Remove(string localUrl) {
            Manager.StaticPageManager.RemovePage(localUrl);
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
        [HttpPost]
        [ExcludeDemoMode]
        public ActionResult RemoveAll() {
            Manager.StaticPageManager.RemoveAllPages();
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
    }
}
