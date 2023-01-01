/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Search.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Search.Controllers {

    public class SearchResultsModuleController : ControllerImpl<YetaWF.Modules.Search.Modules.SearchResultsModule> {

        public SearchResultsModuleController() { }

        public class Model {

            [UIHint("Hidden")]
            public string SearchTerms { get; set; } = null!;
            public bool MoreResults { get; set; }
            public int MaxResults { get; set; }
            public List<SearchResult> SearchResults { get; set; } = null!;
            public bool ShowUrl { get; set; }
            public bool ShowSummary { get; set; }
            public bool NewWindow { get; set; }
            public string QSArgs { get; set; } = null!;
        }

        [AllowGet]
        public async Task<ActionResult> SearchResults(string searchTerms) {
            if (string.IsNullOrWhiteSpace(searchTerms)) return new EmptyResult();

            if (!SearchDataProvider.IsUsable)
                return View("SearchUnavailable_Results");

            using (SearchResultDataProvider searchResDP = new SearchResultDataProvider()) {
                SearchResultDataProvider.SearchResultsInfo list = await searchResDP.GetSearchResultsAsync(searchTerms, Module.MaxResults, MultiString.ActiveLanguage, Manager.HaveUser);
                Model model = new Model() {
                    SearchTerms = searchTerms,
                    SearchResults = list.Data,
                    MoreResults = list.HaveMore,
                    NewWindow = Module.NewWindow,
                    MaxResults = Module.MaxResults,
                    ShowUrl = Module.ShowUrl,
                    ShowSummary = Module.ShowSummary,
                    QSArgs = searchResDP.GetQueryArgsFromKeywords(searchTerms),
                };
                return View(model);
            }
        }
    }
}