/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using YetaWF.Core.Extensions;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Models;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.Search.DataProvider;

namespace YetaWF.Modules.Search.Scheduler {

    public class Search : IScheduling {

        public int SmallestMixedToken { get; set; }
        public int SmallestUpperCaseToken { get; set; }

        public const string EventSearchPages = "YetaWF.Search: Extract Search Keywords From Pages";

        public void RunItem(SchedulerItemBase evnt) {
            if (evnt.EventName != EventSearchPages)
                throw new Error(this.__ResStr("eventNameErr", "Unknown scheduler event {0}."), evnt.EventName);
            SearchSite(slow: true);
        }

        public SchedulerItemBase[] GetItems() {
            return new SchedulerItemBase[]{
                new SchedulerItemBase {
                    Name=this.__ResStr("eventName", "Extract Search Keywords From Pages"),
                    Description = this.__ResStr("eventDesc", "Extracts search keywords from all pages, used to provide search features for site users"),
                    EventName = EventSearchPages,
                    Enabled = true,
                    EnableOnStartup = true,
                    RunOnce = false,
                    Startup = false,
                    SiteSpecific = true,
                    Frequency = new SchedulerFrequency { TimeUnits = SchedulerFrequency.TimeUnitEnum.Days, Value=1 },
                },
            };
        }

        public Search() { }

        public void SearchSite(bool slow)
        {
            SearchConfigData searchConfig = SearchConfigDataProvider.GetConfig();
            SmallestMixedToken = searchConfig.SmallestMixedToken;
            SmallestUpperCaseToken = searchConfig.SmallestUpperCaseToken;

            DateTime searchStarted = DateTime.UtcNow; // once we have all new keywords, delete all keywords that were added before this date/time
            using (SearchDataProvider searchDP = new SearchDataProvider()) {

                // Search all generated pages (unique modules or classes, like data providers)
                DynamicUrlsImpl dynamicUrls = new DynamicUrlsImpl();
                List<Type> types = dynamicUrls.GetDynamicUrlTypes();
                // search types that generate dynamic urls
                foreach (Type type in types) {
#if DEBUG
//                  if (type.Name != "FileDocumentDisplayModule") continue;//used for debugging
#endif

                    ISearchDynamicUrls iSearch = (ISearchDynamicUrls)Activator.CreateInstance(type);
                    if (iSearch != null) {
                        try {
                            CurrentSearchDP = searchDP;
                            CurrentSearchStarted = searchStarted;
                            iSearch.KeywordsForDynamicUrls(AddSearchTermsForPage);
                            CurrentSearchDP = null;
                            if (slow)
                                Thread.Sleep(500);// delay a bit (slow can only be used by schedule items)
                        } catch (Exception exc) {
                            Logging.AddErrorLog("KeywordsForDynamicUrls failed for {0}", type.FullName, exc);
                        }
                    }
                }

                // search all designed modules that have dynamic urls
                List<DesignedModule> desMods = DesignedModules.LoadDesignedModules();
                foreach (DesignedModule desMod in desMods) {
                    try {
                        ModuleDefinition mod = ModuleDefinition.Load(desMod.ModuleGuid, AllowNone: true);
                        if (mod != null && types.Contains(mod.GetType())) {
                            ISearchDynamicUrls iSearch = (ISearchDynamicUrls)mod;
                            if (iSearch != null) {
                                CurrentSearchDP = searchDP;
                                CurrentSearchStarted = searchStarted;
                                iSearch.KeywordsForDynamicUrls(AddSearchTermsForPage);
                                CurrentSearchDP = null;
                                if (slow)
                                    Thread.Sleep(500);// delay a bit (slow can only be used by schedule items)
                            }
                        }
                    } catch (Exception exc) {
                        Logging.AddErrorLog("KeywordsForDynamicUrls failed for module {0}", desMod.ModuleGuid, exc);
                    }
                }

                // Search all designed pages and extract keywords
                List<Guid> pages = PageDefinition.GetDesignedGuids();
                foreach (Guid pageGuid in pages) {
                    PageDefinition page = PageDefinition.Load(pageGuid);
                    if (page != null) {
                        List<SearchData> searchData = SearchPage(searchDP, page, searchStarted);
                        if (searchData != null && searchData.Count > 0)
                            searchDP.AddItems(searchData, page.CompleteUrl, page.Description, page.Created, page.Updated, searchStarted);
                        if (slow)
                            Thread.Sleep(500);// delay a bit (slow can only be used by schedule items)
                    }
                }

                // Remove old keywords
                searchDP.RemoveOldItems(searchStarted);
            }
        }
        private List<SearchData> SearchPage(SearchDataProvider searchDP, PageDefinition page, DateTime searchStarted) {
            List<SearchData> searchData = new List<SearchData>();
            if (!string.IsNullOrWhiteSpace(page.RedirectToPageUrl)) // only search pages that aren't redirected
                return null;
            if (!page.WantSearch)
                return null;
            bool allowAnonymous = page.IsAuthorized_View_Anonymous();
            bool allowAnyUser = page.IsAuthorized_View_AnyUser();
            if (!allowAnonymous && !allowAnyUser)
                return null;
            if (!searchDP.PageUpdated(page.CompleteUrl, page.Created, page.Updated)) {
                Logging.AddLog("Skipping search keywords for page {0} - page not modified", page.CompleteUrl);
                return null;
            }

            Logging.AddLog("Adding search keywords for page {0}", page.CompleteUrl);

            AddSearchTerms(searchData, page.Title, allowAnonymous, allowAnyUser);
            AddSearchTerms(searchData, page.Description, allowAnonymous, allowAnyUser);
            AddSearchTerms(searchData, page.Keywords, allowAnonymous, allowAnyUser);
            foreach (var m in page.ModuleDefinitions) {
                Guid modGuid = m.ModuleGuid;
                ModuleDefinition mod = null;
                try {
                    mod = ModuleDefinition.Load(m.ModuleGuid);
                } catch(Exception ex) {
                    Logging.AddErrorLog("An error occurred retrieving module {0} in page {1}", m.ModuleGuid, page.Url, ex);
                }
                if (mod != null)
                    SearchModule(searchData, page, mod, allowAnonymous, allowAnyUser);
            }
            return searchData;
        }

        List<SearchData> CurrentData;
        bool CurrentAllowAnonymous;
        bool CurrentAllowAnyUser;

        private List<SearchData> SearchModule(List<SearchData> searchData, PageDefinition page, ModuleDefinition mod, bool allowAnonymous, bool allowAnyUser) {
            if (mod.WantSearch) {
                CurrentAllowAnonymous = mod.IsAuthorized_View_Anonymous(allowAnonymous) && allowAnonymous;
                CurrentAllowAnyUser = mod.IsAuthorized_View_AnyUser(allowAnyUser) && allowAnyUser;
                if (!CurrentAllowAnonymous && !CurrentAllowAnyUser)
                    return null;
                AddSearchTerms(searchData, mod.Title, CurrentAllowAnonymous, CurrentAllowAnyUser);
                AddSearchTerms(searchData, mod.Description, CurrentAllowAnonymous, CurrentAllowAnyUser);
                CurrentData = searchData;
                mod.CustomSearch(page, AddTerms);
                CurrentData = null;
            }
            return searchData;
        }

        private void AddTerms(MultiString ms) {
            AddSearchTerms(CurrentData, ms, CurrentAllowAnonymous, CurrentAllowAnyUser);
        }
        private void AddSearchTerms(List<SearchData> searchData, Core.Models.MultiString ms, bool allowAnonymous, bool allowAnyUser) {
            if (ms == null) return;
            foreach (var lang in MultiString.Languages) {
                string culture = lang.Id;
                string val = ms[culture];
                if (!string.IsNullOrEmpty(val))
                    AddSearchTerms(searchData, culture, val, allowAnonymous, allowAnyUser);
            }
        }

        private readonly Regex reTags = new Regex(@"<[^>]*?>", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex reWords = new Regex(@"&[a-zA-Z]+?;", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex preRe = new Regex(@"\S+", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex reChars = new Regex(@"&#(?'num'[0-9]+?);", RegexOptions.Compiled | RegexOptions.Singleline);
        private readonly Regex reHex = new Regex(@"&#x(?'num'[0-9]+?);", RegexOptions.Compiled | RegexOptions.Singleline);

        private void AddSearchTerms(List<SearchData> searchData, string culture, string value, bool allowAnonymous, bool allowAnyUser) {

            YetaWFManager manager = YetaWFManager.Manager;
            int siteIdentity = manager.CurrentSite.Identity;

            // remove html tags
            value = reTags.Replace(value, " ");
            value = reWords.Replace(value, " ");
            value = reChars.Replace(value, new MatchEvaluator(substDecimal));
            value = reHex.Replace(value, new MatchEvaluator(substHexadecimal));

            Match m = preRe.Match(value);
            while (m.Success) {
                string token = m.Value.Trim().ToLower();
                // remove non-word characters from start and end of token
                for (int len = token.Length ; len > 0 ; --len) {
                    char c = token[0];
                    if (char.IsLetterOrDigit(c))
                        break;
                    token = token.Substring(1);
                }
                for (int len = token.Length ; len > 0 ; --len) {
                    char c = token[len - 1];
                    if (char.IsLetterOrDigit(c))
                        break;
                    token = token.Truncate(len - 1);
                }
                if (token.Length > 0 && token.Length < SearchData.MaxSearchTerm && (token.Length >= SmallestMixedToken || (token.Length >= SmallestUpperCaseToken && token.ToUpper() == token))) {
                    SearchData data = (from cd in searchData where cd.SearchTerm == token &&
                                        cd.Language == culture && cd.AllowAnyUser == allowAnyUser && cd.AllowAnonymous == allowAnonymous select cd).FirstOrDefault();
                    if (data == null) {
                        data = new SearchData() {
                            Language = culture,
                            SearchTerm = token,
                            AllowAnyUser = allowAnyUser,
                            AllowAnonymous = allowAnonymous,
                        };
                        searchData.Add(data);
                    }
                    data.Count = data.Count + 1;
                }
                m = m.NextMatch();
            }
        }
        private string substDecimal(Match m) {
            try {
                return Convert.ToChar(Convert.ToInt64(m.Groups["num"].Value)).ToString();
            } catch (Exception) { }
            return " ";
        }
        private string substHexadecimal(Match m) {
            try {
                return Convert.ToChar(Convert.ToInt64(m.Groups["num"].Value, 16)).ToString();
            } catch (Exception) { }
            return " ";
        }

        private SearchDataProvider CurrentSearchDP;
        private DateTime CurrentSearchStarted;

        public void AddSearchTermsForPage(Core.Models.MultiString ms, PageDefinition page, string url, string title, DateTime dateCreated, DateTime? dateUpdated) {
            List<SearchData> searchData = new List<SearchData>();
            bool allowAnonymous = page.IsAuthorized_View_Anonymous();
            bool allowAnyUser = page.IsAuthorized_View_AnyUser();
            if (!allowAnonymous && !allowAnyUser)
                return;

            if (CurrentSearchDP.PageUpdated(url, dateCreated, dateUpdated)) {
                Logging.AddLog("Adding search keywords for page {0}", url);
                AddSearchTerms(searchData, ms, allowAnonymous, allowAnyUser);
                if (searchData != null && searchData.Count > 0)
                    CurrentSearchDP.AddItems(searchData, url, title, dateCreated, dateUpdated, CurrentSearchStarted);
            } else
                Logging.AddLog("Not adding search keywords for page {0} - no change", url);
        }
    }
}