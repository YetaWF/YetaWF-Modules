/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Search#License */

using System.Collections.Specialized;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Search.DataProvider;

namespace YetaWF.Modules.Search.Controllers {

    public class SearchInputModuleController : ControllerImpl<YetaWF.Modules.Search.Modules.SearchInputModule> {

        public SearchInputModuleController() { }

        [Trim]
        public class Model {

            [TextAbove("Enter the search term(s) - Use AND or OR to create search queries. When multiple terms (without AND or OR) are used, all terms must be present in a page to be listed in the search results. Use * at the end of a search term to search for all occurrences that start with the specified term (wildcard searching).")]
            [Caption("Search Terms"), Description("Enter the search term(s) - Use AND or OR to create search queries - When multiple terms (without AND or OR) are used, all terms must be present in a page to be listed in the search results - Use * at the end of a search term to search for all occurrences that start with the specified term (wildcard searching)")]
            [UIHint("Text40"), Trim, StringLength(SearchData.MaxSearchTerm), Required]
            public string SearchTerms { get; set; }

            public Model() { }
        }

        [HttpGet]
        public ActionResult SearchInput(string searchTerms) {
            SearchConfigData config = SearchConfigDataProvider.GetConfig();
            if (!Manager.EditMode && string.IsNullOrWhiteSpace(config.ResultsUrl)) // if no search result url is available, don't show the module
                return new EmptyResult();
            Model model = new Model { SearchTerms = searchTerms };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchInput_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            SearchConfigData config = SearchConfigDataProvider.GetConfig();
            NameValueCollection qs = System.Web.HttpUtility.ParseQueryString(string.Empty);
            qs["SearchTerms"] = model.SearchTerms;
            string url = config.ResultsUrl + "?" + qs.ToString();
            return FormProcessed(model, NextPage: url);
        }
    }
}