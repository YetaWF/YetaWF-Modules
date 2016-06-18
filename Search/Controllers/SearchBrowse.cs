/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Search.DataProvider;
using YetaWF.Modules.Search.Modules;

namespace YetaWF.Modules.Search.Controllers {

    public class SearchBrowseModuleController : ControllerImpl<YetaWF.Modules.Search.Modules.SearchBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                    SearchEditModule editMod = new SearchEditModule();
                    actions.New(editMod.GetAction_Edit(Module.EditUrl, SearchDataId), ModuleAction.ActionLocationEnum.GridLinks);

                    actions.New(Module.GetAction_Remove(SearchDataId), ModuleAction.ActionLocationEnum.GridLinks);
                    return actions;
                }
            }

            [Caption("Search Keyword"), Description("")]
            [UIHint("String"), ReadOnly]
            public string SearchTerm { get; set; }

            [Caption("Url"), Description("The page where this keyword was found")]
            [UIHint("Url"), ReadOnly]
            public string PageUrl { get; set; }

            [Caption("Description"), Description("The page description")]
            [UIHint("String"), ReadOnly]
            public string PageDescription { get; set; }

            [Caption("Created"), Description("The date/time the page was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime DatePageCreated { get; set; }
            [Caption("Updated"), Description("The date/time the page was last updated")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime DatePageUpdated { get; set; }

            [Caption("Added"), Description("The date/time this keyword was added")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime DateAdded { get; set; }

            [Caption("Language"), Description("The page language where this keyword was found")]
            [UIHint("String"), ReadOnly]
            public string Language { get; set; }

            [Caption("Count"), Description("The number of times this keyword was found on the page")]
            [UIHint("IntValue"), ReadOnly]
            public int Count { get; set; }

            [Caption("Anonymous Users"), Description("Whether anonymous users can view the page")]
            [UIHint("Boolean"), ReadOnly]
            public bool AllowAnonymous { get; set; }

            [Caption("Any Users"), Description("Whether any logged on user can view the page")]
            [UIHint("Boolean"), ReadOnly]
            public bool AllowAnyUser { get; set; }

            [Caption("Id"), Description("The internal id this keyword")]
            [UIHint("IntValue"), ReadOnly]
            public int SearchDataId { get; set; }

            private SearchBrowseModule Module { get; set; }

            public BrowseItem(SearchBrowseModule module, SearchData data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }

        public class BrowseModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        [HttpGet]
        public ActionResult SearchBrowse() {
            BrowseModel model = new BrowseModel { };
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("SearchBrowse_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            using (SearchDataProvider searchDP = new SearchDataProvider()) {
                int total;
                List<SearchData> browseItems = searchDP.GetItemsWithUrl(skip, take, sort, filters, out total);
                GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return GridPartialView(new DataSourceResult {
                    Data = (from s in browseItems select new BrowseItem(Module, s)).ToList<object>(),
                    Total = total
                });
            }
        }

        [HttpPost]
        [Permission("RemoveItems")]
        public ActionResult Remove(int searchDataId) {
            using (SearchDataProvider searchDP = new SearchDataProvider()) {
                searchDP.RemoveItem(searchDataId);
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }

        [HttpPost]
        [Permission("RemoveItems")]
        public ActionResult RemoveAll() {
            using (SearchDataProvider searchDP = new SearchDataProvider()) {
                searchDP.RemoveItems(null);/* ALL */
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }

        [HttpPost]
        [Permission("CollectKeywords")]
        public ActionResult CollectKeywords() {
            Scheduler.Search search = new Scheduler.Search();
            search.SearchSite(false);
            return Reload(PopupText: this.__ResStr("done", "Site search keywords updated"), Reload: ReloadEnum.ModuleParts);
        }
    }
}