/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Modules.Search.DataProvider;
using YetaWF.Modules.Search.Modules;
using YetaWF.Core.IO;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

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

            [Caption("Title"), Description("The page title")]
            [UIHint("String"), ReadOnly]
            public string PageTitle { get; set; }

            [Caption("Summary"), Description("The page summary")]
            [UIHint("String"), ReadOnly]
            public string PageSummary { get; set; }

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
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                InitialPageSize = 20,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(SearchBrowse_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    using (SearchDataProvider searchDP = new SearchDataProvider()) {
                        DataProviderGetRecords<SearchData> browseItems = await searchDP.GetItemsWithUrlAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem(Module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult SearchBrowse() {
            if (!SearchDataProvider.IsUsable)
                return View("SearchUnavailable_Browse");
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> SearchBrowse_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync(GetGridModel(), gridPVData);
        }

        [AllowPost]
        [Permission("RemoveItems")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(int searchDataId) {
            using (SearchDataProvider searchDP = new SearchDataProvider()) {
                await searchDP.RemoveItemAsync(searchDataId);
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }

        [AllowPost]
        [Permission("RemoveItems")]
        [ExcludeDemoMode]
        public async Task<ActionResult> RemoveAll() {
            using (SearchDataProvider searchDP = new SearchDataProvider()) {
                using (ILockObject lockObject = await YetaWF.Core.IO.Caching.LockProvider.LockResourceAsync($"{AreaRegistration.CurrentPackage.AreaName}_{nameof(SearchDataProvider)}")) {
                    await searchDP.RemoveItemsAsync(null);/* ALL */
                    await lockObject.UnlockAsync();
                }
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }

        [AllowPost]
        [Permission("CollectKeywords")]
        [ExcludeDemoMode]
        public async Task<ActionResult> CollectKeywords() {
            Scheduler.Search search = new Scheduler.Search();
            await search.SearchSiteAsync(false);
            return Reload(PopupText: this.__ResStr("done", "Site search keywords updated"), Reload: ReloadEnum.ModuleParts);
        }
    }
}