/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Support;
using YetaWF.Modules.Search.Addons;

namespace YetaWF.Modules.Search.DataProvider {

    public partial class SearchResultDataProvider {

        public string GetQueryArgsFromKeywords(string searchTerms) {
            List<string> kwds = searchTerms.Split(new char[] { ' ', ',' }).ToList();
            //using (SearchResultDataProvider searchResDP = new SearchResultDataProvider()) {
            string searchKwdOr = GetKeyWordOr();
            string searchKwdAnd = GetKeyWordAnd();
            kwds.RemoveAll(m => { return m == searchKwdOr; });
            kwds.RemoveAll(m => { return m == searchKwdAnd; });
            //}
            kwds = (from k in kwds select k.Trim(new char[] { '(', ')', '*' })).ToList();
            return string.Format("{0}={1}", Info.UrlArg, Utility.HAE(string.Join(",", kwds)));
        }

        internal async Task<SearchResultsInfo> ParseAsync(string searchTerms, int maxResults, string languageId, bool haveUser, List<DataProviderFilterInfo>? extraFilters = null) {
            using (SearchDataProvider searchDP = new SearchDataProvider()) {
                bool haveMore = false;

                extraFilters = DataProviderFilterInfo.Join(extraFilters, new DataProviderFilterInfo { Field = nameof(SearchData.Language), Operator = "==", Value = languageId });
                if (haveUser)
                    extraFilters = DataProviderFilterInfo.Join(extraFilters, new DataProviderFilterInfo { Field = nameof(SearchData.AllowAnyUser), Operator = "==", Value = true });
                else
                    extraFilters = DataProviderFilterInfo.Join(extraFilters, new DataProviderFilterInfo { Field = nameof(SearchData.AllowAnonymous), Operator = "==", Value = true });

                string s = searchTerms;
                BuildNodesInfo urls = await BuildNodesAsync(searchDP, s, languageId, haveUser, extraFilters);

                List<DataProvider.SearchResult> results = (from u in urls.Data group u by u.SearchDataUrlId into g select new SearchResult {
                    Count = g.Sum(x => x.Count),
                    PageUrl = g.Select(m => m.PageUrl).FirstOrDefault(),
                    DateCreated = g.Select(m => m.DatePageCreated).FirstOrDefault(),
                    DateUpdated = g.Select(m => m.DatePageUpdated).FirstOrDefault(),
                    Description = g.Select(m => m.PageTitle).FirstOrDefault(),
                    PageSummary = g.Select(m => m.PageSummary).FirstOrDefault(),
                    PageSecurity = g.Select(m => m.PageSecurity).FirstOrDefault(),
                    CustomData = g.Select(m => m.CustomData).FirstOrDefault(),
                }).OrderByDescending(m => m.Count).Take(maxResults + 1).ToList();

                haveMore = (results.Count >= maxResults);
                return new Search.DataProvider.SearchResultDataProvider.SearchResultsInfo {
                    Data = results,
                    HaveMore = haveMore,
                };
            }
        }

        private int parenLevel = 0;

        private class BuildNodesInfo {
            public List<SearchData> Data { get; set; } = null!;
            public string Search { get; set; } = null!;
        }
        // generate a list of url (ids) based on search terms
        private async Task<BuildNodesInfo> BuildNodesAsync(SearchDataProvider searchDP, string search, string languageId, bool haveUser, List<DataProviderFilterInfo> extraFilters) {

            if (string.IsNullOrEmpty(search))
                return new BuildNodesInfo();

            BuildNodesInfo? list = null;
            for ( ; ; ) {
                search = search.Trim();
                if (search.Length <= 0)
                    break;

                char c = search[0];
                if (c == '(') {
                    parenLevel++;
                    search = search.Remove(0, 1);
                    list = await BuildNodesAsync(searchDP, search, languageId, haveUser, extraFilters);
                    search = list.Search;
                } else if (c == ')') {
                    if (parenLevel <= 0 || list == null)
                        throw new Error(this.__ResStr("invQueryCloseParen", "Invalid query - too many ')'"));
                    parenLevel--;
                    list.Search = search.Remove(0, 1);
                    return list;
                } else {
                    string token;
                    int irp = search.IndexOf(')');
                    int i = search.IndexOf(' ');
                    if (irp >= 0 && (irp < i || i < 0))
                        i = irp;
                    int ilp = search.IndexOf('(');
                    if (ilp >= 0 && (ilp < i || i < 0))
                        i = ilp;
                    if (i < 0) {
                        token = search;
                        search = "";
                    } else {
                        token = search.Substring(0, i);
                        search = search.Remove(0, i);
                    }
                    if (list != null) {
                        if (string.Compare(token, GetKeyWordOr(), true) == 0) {
                            BuildNodesInfo rhsList = await BuildNodesAsync(searchDP, search, languageId, haveUser, extraFilters);
                            search = rhsList.Search;
                            list.Data = list.Data.Union(rhsList.Data, new SearchDataComparer()).ToList();
                        } else if (string.Compare(token, GetKeyWordAnd(), true) == 0) {
                            BuildNodesInfo rhsList = await BuildNodesAsync(searchDP, search, languageId, haveUser, extraFilters);
                            search = rhsList.Search;
                            list.Data = list.Data.Intersect(rhsList.Data, new SearchDataComparer()).ToList();
                        } else {
                            List<DataProviderFilterInfo>? filters = DataProviderFilterInfo.Copy(extraFilters);
                            if (token.EndsWith("*")) {
                                token = token.TrimEnd(new char[] { '*' });
                                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(SearchData.SearchTerm), Operator = "StartsWith", Value = token });
                            } else
                                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(SearchData.SearchTerm), Operator = "==", Value = token });
                            DataProviderGetRecords<SearchData> rhsList = await searchDP.GetItemsWithUrlAsync(0, 0, null, filters);
                            list.Data = list.Data.Intersect(rhsList.Data, new SearchDataComparer()).ToList();
                        }
                    } else {
                        List<DataProviderFilterInfo>? filters = DataProviderFilterInfo.Copy(extraFilters);
                        if (token.EndsWith("*")) {
                            token = token.TrimEnd(new char[] { '*' });
                            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(SearchData.SearchTerm), Operator = "StartsWith", Value = token });
                        } else
                            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(SearchData.SearchTerm), Operator = "==", Value = token });
                        list = new BuildNodesInfo { Search = search };
                        list.Data = (await searchDP.GetItemsWithUrlAsync(0, 0, null, filters)).Data;
                        search = list.Search;
                    }
                }
            }
            if (parenLevel > 0)
                throw new Error(this.__ResStr("invQueryOpenParen", "Invalid query - unmatched '('"));
            list!.Search = search;
            return list;
        }
    }
}
