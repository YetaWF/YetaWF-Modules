/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using YetaWF.Core.Extensions;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Models;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Search;
using YetaWF.Core.Support;
using YetaWF.Modules.Search.DataProvider;

namespace YetaWF.Modules.Search.Scheduler {

    public class Search : IScheduling {

        public const string EventSearchPages = "YetaWF.Search: Extract Search Keywords From Pages";

        public async Task RunItemAsync(SchedulerItemBase evnt) {
            if (evnt.EventName != EventSearchPages)
                throw new Error(this.__ResStr("eventNameErr", "Unknown scheduler event {0}."), evnt.EventName);
            await SearchSiteAsync(slow: true);
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

        public async Task SearchSiteAsync(bool slow) {
            SearchConfigData searchConfig = await SearchConfigDataProvider.GetConfigAsync();
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
                    ISearchDynamicUrls iSearch = Activator.CreateInstance(type) as ISearchDynamicUrls;
                    if (iSearch != null) {
                        try {
                            SearchWords searchWords = new SearchWords(searchConfig, searchDP, searchStarted);
                            iSearch.KeywordsForDynamicUrls(searchWords);
                            if (slow)
                                Thread.Sleep(500);// delay a bit (slow can only be used by scheduler items)
                        } catch (Exception exc) {
                            Logging.AddErrorLog("KeywordsForDynamicUrls failed for {0}", type.FullName, exc);
                        }
                    }
                }

                // search all designed modules that have dynamic urls
                List<DesignedModule> desMods = await DesignedModules.LoadDesignedModulesAsync();
                foreach (DesignedModule desMod in desMods) {
                    try {
                        ModuleDefinition mod = await ModuleDefinition.LoadAsync(desMod.ModuleGuid, AllowNone: true);
                        if (mod != null && types.Contains(mod.GetType()) && mod.WantSearch) {
                            ISearchDynamicUrls iSearch = mod as ISearchDynamicUrls;
                            if (iSearch != null) {
                                SearchWords searchWords = new SearchWords(searchConfig, searchDP, searchStarted);
                                iSearch.KeywordsForDynamicUrls(searchWords);
                                if (slow)
                                    Thread.Sleep(500);// delay a bit (slow can only be used by scheduler items)
                            }
                        }
                    } catch (Exception exc) {
                        Logging.AddErrorLog("KeywordsForDynamicUrls failed for module {0}", desMod.ModuleGuid, exc);
                    }
                }

                // Search all designed pages and extract keywords
                List<Guid> pages = PageDefinition.GetDesignedGuids();
                foreach (Guid pageGuid in pages) {
                    PageDefinition page = await PageDefinition.LoadAsync(pageGuid);
                    if (page != null) {
                        SearchWords searchWords = new SearchWords(searchConfig, searchDP, searchStarted);
                        await SearchPageAsync(searchWords, page);
                        if (slow)
                            Thread.Sleep(500);// delay a bit (slow can only be used by scheduler items)
                    }
                }

                // Remove old keywords
                await searchDP.RemoveOldItemsAsync(searchStarted);
            }
        }

        private async Task SearchPageAsync(SearchWords searchWords, PageDefinition page) {
            if (!searchWords.WantPage(page)) return;
            if (await searchWords.SetUrlAsync(page.Url, page.PageSecurity, page.Title, page.Description, page.Created, page.Updated, page.IsAuthorized_View_Anonymous(), page.IsAuthorized_View_AnyUser())) {
                searchWords.AddKeywords(page.Keywords);
                foreach (var m in page.ModuleDefinitions) {
                    Guid modGuid = m.ModuleGuid;
                    ModuleDefinition mod = null;
                    try {
                        mod = await ModuleDefinition.LoadAsync(m.ModuleGuid);
                    } catch (Exception ex) {
                        Logging.AddErrorLog("An error occurred retrieving module {0} in page {1}", m.ModuleGuid, page.Url, ex);
                    }
                    if (mod != null)
                        SearchModule(searchWords, mod);
                }
                await searchWords.SaveAsync();
            }
        }
        private void SearchModule(SearchWords searchWords, ModuleDefinition mod) {
            if (mod.WantSearch) {
                if (searchWords.SetModule(mod.IsAuthorized_View_Anonymous(), mod.IsAuthorized_View_AnyUser())) {
                    if (mod.ShowTitle)
                        searchWords.AddTitle(mod.Title);
                    searchWords.AddContent(mod.Description);
                    mod.CustomSearch(searchWords);
                    searchWords.ClearModule();
                }
            }
        }

        // ISearchWords
        // ISearchWords
        // ISearchWords

        public class SearchWords : ISearchWords {

            private const int TITLE_WEIGHT = 200;
            private const int DESCRIPTION_WEIGHT = 200;
            private const int KEYWORDS_WEIGHT = 200;
            private const int CONTENT_WEIGHT = 1;

            int SmallestMixedToken;
            int SmallestUpperCaseToken;

            SearchDataProvider CurrentSearchDP;
            DateTime CurrentSearchStarted;
            string CurrentUrl;
            PageDefinition.PageSecurityType CurrentPageSecurity;
            bool CurrentAllowAnonymous;
            bool CurrentAllowAnyUser;
            bool SavedCurrentAllowAnonymous, SavedCurrentAllowAnyUser;
            MultiString CurrentTitle;
            MultiString CurrentSummary;
            DateTime CurrentDateCreated;
            DateTime? CurrentDateUpdated;
            List<SearchData> CurrentSearchData;

            public SearchWords(SearchConfigData searchConfig, SearchDataProvider searchDP, DateTime searchStarted) {
                CurrentSearchDP = searchDP;
                CurrentSearchStarted = searchStarted;

                SmallestMixedToken = searchConfig.SmallestMixedToken;
                SmallestUpperCaseToken = searchConfig.SmallestUpperCaseToken;
            }

            public void AddContent(MultiString content) { AddItems(content, CONTENT_WEIGHT); }
            public void AddTitle(MultiString content) { AddItems(content, TITLE_WEIGHT); }
            public void AddKeywords(MultiString content) { AddItems(content, KEYWORDS_WEIGHT); }
            public bool WantPage(PageDefinition page) {
                if (!page.WantSearch) {
                    Logging.AddLog("No search keywords wanted for page {0}", page.Url);
                    return false;
                }
                if (!string.IsNullOrWhiteSpace(page.RedirectToPageUrl)) { // only search pages that aren't redirected
                    Logging.AddLog("No search keywords for page {0} as it is redirected", page.Url);
                    return false;
                }
                if (!page.IsAuthorized_View_Anonymous() && !page.IsAuthorized_View_AnyUser()) {
                    Logging.AddLog("No search keywords for page {0} - neither Anonymous nor User role has access to the page", page.Url);
                    return false;
                }
                return true;
            }
            public async Task<bool> SetUrlAsync(string url, PageDefinition.PageSecurityType pageSecurity, MultiString title, MultiString summary, DateTime dateCreated, DateTime? dateUpdated, bool allowAnonymous, bool allowUser) {
                YetaWFManager manager = YetaWFManager.Manager;
                if (CurrentUrl != null) throw new InternalError("Already have an active Url - {0} {1} called", nameof(SetUrlAsync), url);
                if (!url.StartsWith("/"))
                    throw new InternalError("Urls for search terms must be local and start with \"/\"");
                CurrentPageSecurity = pageSecurity;
                CurrentAllowAnonymous = allowAnonymous;
                CurrentAllowAnyUser = allowUser;
                CurrentTitle = title;
                CurrentSummary = summary;
                CurrentDateCreated = dateCreated;
                CurrentDateUpdated = dateUpdated;
                CurrentSearchData = new List<SearchData>();
                if (!CurrentAllowAnonymous && !CurrentAllowAnyUser) {
                    Logging.AddLog("No search keywords for Url {0} - neither Anonymous nor User role has access to the page", url);
                    return false;
                }
                if (!await CurrentSearchDP.PageUpdated(url, CurrentDateCreated, CurrentDateUpdated)) {
                    Logging.AddLog("Page {0} not evaluated as it has not changed", url);
                    return false;
                }
                CurrentUrl = url;
                Logging.AddLog("Adding search keywords for page {0}", CurrentUrl);
                AddTitle(CurrentTitle);
                AddContent(CurrentSummary);
                return true;
            }
            /// <summary>
            /// Add all string properties of an object as search terms.
            /// </summary>
            public void AddObjectContents(object searchObject) {
                Type tp = searchObject.GetType();
                foreach (var propData in ObjectSupport.GetPropertyData(tp)) {
                    if (propData.PropInfo.CanRead && propData.PropInfo.CanWrite) {
                        if (propData.PropInfo.PropertyType == typeof(string)) {
                            string s = (string)propData.PropInfo.GetValue(searchObject, null);
                            AddContent(s);
                        } else if (propData.PropInfo.PropertyType == typeof(MultiString)) {
                            MultiString ms = (MultiString)propData.PropInfo.GetValue(searchObject, null);
                            AddContent(ms);
                        }
                    }
                }
            }
            public async Task SaveAsync() {
                VerifyPage();
                await CurrentSearchDP.AddItemsAsync(CurrentSearchData, CurrentUrl, CurrentPageSecurity, CurrentTitle, CurrentSummary, CurrentDateCreated, CurrentDateUpdated, CurrentSearchStarted);
                Reset(CurrentSearchDP, CurrentSearchStarted);
            }
            internal bool SetModule(bool allowAnonymous, bool allowUser) {
                if (!(CurrentAllowAnonymous && allowAnonymous) && !(CurrentAllowAnyUser && allowUser))
                    return false;
                SavedCurrentAllowAnonymous = CurrentAllowAnonymous;
                SavedCurrentAllowAnyUser = CurrentAllowAnyUser;
                CurrentAllowAnonymous = allowAnonymous;
                CurrentAllowAnyUser = allowUser;
                return true;
            }
            internal void ClearModule() {
                CurrentAllowAnonymous = SavedCurrentAllowAnonymous;
                CurrentAllowAnyUser = SavedCurrentAllowAnyUser;
            }

            private void Reset(SearchDataProvider currentSearchDP, DateTime currentSearchStarted) {
                CurrentSearchDP = currentSearchDP;
                CurrentSearchStarted = currentSearchStarted;
                CurrentUrl = null;
                CurrentPageSecurity = PageDefinition.PageSecurityType.Any;
                CurrentAllowAnonymous = false;
                CurrentAllowAnyUser = false;
                CurrentDateCreated = DateTime.MinValue;
                CurrentDateUpdated = null;
                CurrentSearchData = new List<SearchData>();
            }
            private void AddItems(MultiString content, int weight) {
                VerifyPage();
                AddSearchTerms(content, CurrentAllowAnonymous, CurrentAllowAnyUser, weight);
            }
            private void VerifyPage() {
                if (CurrentUrl == null) throw new InternalError("No active page");
            }
            private void AddSearchTerms(MultiString ms, bool allowAnonymous, bool allowAnyUser, int weight) {
                if (ms == null) return;
                foreach (var lang in MultiString.Languages) {
                    string culture = lang.Id;
                    string val = ms[culture];
                    if (!string.IsNullOrEmpty(val))
                        AddSearchTerms(culture, val, allowAnonymous, allowAnyUser, weight);
                }
            }

            private readonly Regex reTags = new Regex(@"<[^>]*?>", RegexOptions.Compiled | RegexOptions.Singleline);
            private readonly Regex reWords = new Regex(@"&[a-zA-Z]+?;", RegexOptions.Compiled | RegexOptions.Singleline);
            private readonly Regex preRe = new Regex(@"\S+", RegexOptions.Compiled | RegexOptions.Singleline);
            private readonly Regex reChars = new Regex(@"&#(?'num'[0-9]+?);", RegexOptions.Compiled | RegexOptions.Singleline);
            private readonly Regex reHex = new Regex(@"&#x(?'num'[0-9]+?);", RegexOptions.Compiled | RegexOptions.Singleline);

            private void AddSearchTerms(string culture, string value, bool allowAnonymous, bool allowAnyUser, int weight) {

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
                    for (int len = token.Length; len > 0; --len) {
                        char c = token[0];
                        if (char.IsLetterOrDigit(c))
                            break;
                        token = token.Substring(1);
                    }
                    for (int len = token.Length; len > 0; --len) {
                        char c = token[len - 1];
                        if (char.IsLetterOrDigit(c))
                            break;
                        token = token.Truncate(len - 1);
                    }
                    if (token.Length > 0 && token.Length < SearchData.MaxSearchTerm && (token.Length >= SmallestMixedToken || (token.Length >= SmallestUpperCaseToken && token.ToUpper() == token))) {
                        SearchData data = (from cd in CurrentSearchData
                                           where cd.SearchTerm == token &&
                                               cd.Language == culture && cd.AllowAnyUser == allowAnyUser && cd.AllowAnonymous == allowAnonymous
                                           select cd).FirstOrDefault();
                        if (data == null) {
                            data = new SearchData() {
                                Language = culture,
                                SearchTerm = token,
                                AllowAnyUser = allowAnyUser,
                                AllowAnonymous = allowAnonymous,
                            };
                            CurrentSearchData.Add(data);
                        }
                        data.Count = data.Count + weight;
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
        }
    }
}