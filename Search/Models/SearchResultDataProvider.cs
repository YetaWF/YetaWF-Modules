﻿/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Collections.Generic;
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
                return MakeDataProvider2(package, package.AreaName + "_Urls", SiteIdentity: SiteIdentity, Cacheable: true);
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
        public List<SearchResult> GetSearchResults(string searchTerms, int maxResults, string languageId, bool haveUser, out bool haveMore, List<DataProviderFilterInfo> Filters = null) {
            haveMore = false;
            if (!SearchDataProvider.IsUsable) return new List<SearchResult>();
            List<SearchResult> results = Parse(searchTerms, maxResults, languageId, haveUser, out haveMore, Filters);
            return results;
        }
    }
}
