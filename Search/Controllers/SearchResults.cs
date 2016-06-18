/* Copyright � 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Search#License */

using System.Collections.Generic;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Search.DataProvider;

namespace YetaWF.Modules.Search.Controllers {

    public class SearchResultsModuleController : ControllerImpl<YetaWF.Modules.Search.Modules.SearchResultsModule> {

        public SearchResultsModuleController() { }

        public class Model {

            [UIHint("Hidden")]
            public string SearchTerms { get; set; }
            public bool MoreResults { get; set; }
            public int MaxResults { get; set; }
            public List<SearchResult> SearchResults { get; set; }

        }

        [HttpGet]
        public ActionResult SearchResults(string searchTerms) {
            if (string.IsNullOrWhiteSpace(searchTerms)) return new EmptyResult();

            SearchConfigData config = SearchConfigDataProvider.GetConfig();
            using (SearchResultDataProvider searchResDP = new SearchResultDataProvider()) {

                bool haveMore;
                List<SearchResult> list = searchResDP.GetSearchResults(searchTerms, config.MaxResults, MultiString.ActiveLanguage, Manager.HaveUser, out haveMore);
                Model model = new Model() {
                    SearchTerms = searchTerms,
                    SearchResults = list,
                    MoreResults = haveMore,
                    MaxResults = config.MaxResults,
                };
                return View(model);
            }
        }
    }
}