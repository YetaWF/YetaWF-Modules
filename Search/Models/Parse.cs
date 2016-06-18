/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Search#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Search.DataProvider {

    public partial class SearchResultDataProvider {

        internal List<SearchResult> Parse(string searchTerms, int maxResults, string languageId, bool haveUser, out bool haveMore, List<DataProviderFilterInfo> extraFilters = null) {
            using (SearchDataProvider searchDP = new SearchDataProvider()) {
                haveMore = false;

                string s = searchTerms;
                List<SearchData> urls = BuildNodes(searchDP, ref s, languageId, haveUser, extraFilters);

                List<DataProvider.SearchResult> results = (from u in urls group u by u.SearchDataUrlId into g select new SearchResult {
                    Count = g.Count(),
                    PageUrl = g.Select(m => m.PageUrl).FirstOrDefault(),
                    DateCreated = g.Select(m => m.DatePageCreated).FirstOrDefault(),
                    DateUpdated = g.Select(m => m.DatePageUpdated).FirstOrDefault(),
                     Description = g.Select(m => m.PageDescription).FirstOrDefault(),
                }).OrderByDescending(m => m.Count).Take(maxResults + 1).ToList();

                haveMore = (results.Count >= maxResults);
                return results;
            }
        }

        private int parenLevel = 0;

        // generate a list of url (ids) based on search terms
        private List<SearchData> BuildNodes(SearchDataProvider searchDP, ref string search, string languageId, bool haveUser, List<DataProviderFilterInfo> extraFilters) {
            if (string.IsNullOrEmpty(search))
                return null;

            List<SearchData> list = null;
            for ( ; ; ) {
                search = search.Trim();
                if (search.Length <= 0)
                    break;

                char c = search[0];
                if (c == '(') {
                    parenLevel++;
                    search = search.Remove(0, 1);
                    list = BuildNodes(searchDP, ref search, languageId, haveUser, extraFilters);
                } else if (c == ')') {
                    if (parenLevel <= 0)
                        throw new Error(this.__ResStr("invQueryCloseParen", "Invalid query - too many ')'"));
                    parenLevel--;
                    search = search.Remove(0, 1);
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
                        if (string.Compare(token, "OR", true) == 0) { //TODO: need to localize
                            List<SearchData> rhsList = BuildNodes(searchDP, ref search, languageId, haveUser, extraFilters);
                            list = list.Union(rhsList, new SearchDataComparer()).ToList();
                        } else if (string.Compare(token, "AND", true) == 0) {
                            List<SearchData> rhsList = BuildNodes(searchDP, ref search, languageId, haveUser, extraFilters);
                            list = list.Intersect(rhsList, new SearchDataComparer()).ToList();
                        } else {
                            List<DataProviderFilterInfo> filters = extraFilters;
                            if (token.EndsWith("*")) {
                                token = token.TrimEnd(new char[] { '*' });
                                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "SearchTerm", Operator = "StartsWith", Value = token });
                            } else
                                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "SearchTerm", Operator = "==", Value = token });
                            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "Language", Operator = "==", Value = languageId });
                            if (haveUser)
                                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "AllowAnyUser", Operator = "==", Value = true });
                            else
                                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "AllowAnonymous", Operator = "==", Value = true });
                            int total;
                            List<SearchData> rhsList = searchDP.GetItemsWithUrl(0, 0, null, filters, out total);
                            list = list.Intersect(rhsList, new SearchDataComparer()).ToList();
                        }
                    } else {
                        List<DataProviderFilterInfo> filters = extraFilters;
                        if (token.EndsWith("*")) {
                            token = token.TrimEnd(new char[] { '*' });
                            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "SearchTerm", Operator = "StartsWith", Value = token });
                        } else
                            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "SearchTerm", Operator = "==", Value = token });
                        filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "Language", Operator = "==", Value = languageId });
                        if (!haveUser)
                            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "AllowAnonymous", Operator = "==", Value = true });
                        int total;
                        list = searchDP.GetItemsWithUrl(0, 0, null, filters, out total);
                    }
                }
            }
            if (parenLevel > 0)
                throw new Error(this.__ResStr("invQueryOpenParen", "Invalid query - unmatched '('"));
            return list;
        }
    }
}
