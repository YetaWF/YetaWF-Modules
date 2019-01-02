/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Search.DataProvider {

    public class SearchResult {

        public int Count { get; set; }
        [UIHint("Url")]
        public string PageUrl { get; set; }
        [UIHint("String")]
        public string Description { get; set; }
        [UIHint("DateTime")]
        public DateTime DateCreated { get; set; }
        [UIHint("DateTime")]
        public DateTime? DateUpdated { get; set; }
        [UIHint("String")]
        public string PageSummary { get; set; }
        public PageDefinition.PageSecurityType PageSecurity { get; set; }

        public SearchResult() { }
    }

    public partial class SearchResultDataProvider : DataProviderImpl {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public SearchResultDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public SearchResultDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, SearchResult> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, SearchResult> CreateDataProvider() {
            if (SearchDataProvider.IsUsable) {
                Package package = YetaWF.Modules.Search.Controllers.AreaRegistration.CurrentPackage;
                return MakeDataProvider(package, package.AreaName + "_Urls", SiteIdentity: SiteIdentity, Cacheable: true);
            } else {
                return null;
            }
        }

        // API
        // API
        // API

        public string GetKeyWordOr() {
            return this.__ResStr("keyWordOR", "OR");
        }
        public string GetKeyWordAnd() {
            return this.__ResStr("keyWordAnd", "AND");
        }
        public class SearchResultsInfo {
            public bool HaveMore{ get; set; }
            public List<SearchResult> Data { get; set; }
        }

        public async Task<SearchResultsInfo> GetSearchResultsAsync(string searchTerms, int maxResults, string languageId, bool haveUser, List<DataProviderFilterInfo> Filters = null) {
            if (!SearchDataProvider.IsUsable) return new SearchResultsInfo();
            SearchResultsInfo info = await ParseAsync(searchTerms, maxResults, languageId, haveUser, Filters);
            return info;
        }
    }
}
