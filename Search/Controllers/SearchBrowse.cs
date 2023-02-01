/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Search.DataProvider;
using YetaWF.Modules.Search.Endpoints;
using YetaWF.Modules.Search.Modules;

namespace YetaWF.Modules.Search.Controllers {

    public class SearchBrowseModuleController : ControllerImpl<YetaWF.Modules.Search.Modules.SearchBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands {
                get {
                    List<ModuleAction> actions = new List<ModuleAction>();

                    SearchEditModule editMod = new SearchEditModule();
                    actions.New(editMod.GetAction_Edit(Module.EditUrl, SearchDataId), ModuleAction.ActionLocationEnum.GridLinks);

                    actions.New(Module.GetAction_Remove(SearchDataId), ModuleAction.ActionLocationEnum.GridLinks);
                    return actions;
                }
            }

            [Caption("Search Keyword"), Description("")]
            [UIHint("String"), ReadOnly]
            public string SearchTerm { get; set; } = null!;

            [Caption("Url"), Description("The page where this keyword was found")]
            [UIHint("Url"), ReadOnly]
            public string PageUrl { get; set; } = null!;

            [Caption("Title"), Description("The page title")]
            [UIHint("String"), ReadOnly]
            public string? PageTitle { get; set; }

            [Caption("Summary"), Description("The page summary")]
            [UIHint("String"), ReadOnly]
            public string? PageSummary { get; set; }

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
            public string Language { get; set; } = null!;

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
            public GridDefinition GridDef { get; set; } = null!;
        }
        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                InitialPageSize = 20,
                RecordType = typeof(BrowseItem),
                AjaxUrl = Utility.UrlFor<SearchBrowseModuleEndpoints>(GridSupport.BrowseGridData),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    using (SearchDataProvider searchDP = new SearchDataProvider()) {
                        DataProviderGetRecords<SearchData> browseItems = await searchDP.GetItemsWithUrlAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem((SearchBrowseModule)module, s)).ToList<object>(),
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
                GridDef = GetGridModel(Module)
            };
            return View(model);
        }
    }
}