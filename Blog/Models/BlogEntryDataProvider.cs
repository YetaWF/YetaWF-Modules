/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Extensions;
using YetaWF.Core.IO;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Search;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Modules;

namespace YetaWF.Modules.Blog.DataProvider {

    public class BlogEntry {

        public const int MaxTitle = 100;
        public const int MaxAuthor = 100;
        public const int MaxSummary = 500;
        public const int MaxKwds = 100;
        public const int MaxText = 1024*1024;

        [Data_PrimaryKey, Data_Identity]
        public int Identity { get; set; }
        [Data_Index]
        public int CategoryIdentity { get; set; }
        [StringLength(MaxTitle)]
        public MultiString Title { get; set; }
        [StringLength(MaxAuthor)]
        public string Author { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string AuthorUrl { get; set; }
        [StringLength(MaxKwds)]
        public MultiString Keywords { get; set; }

        public Guid UniqueId { get; set; } // a unique Id that never changes - used for image storage

        public bool Published { get; set; }
        public DateTime DatePublished { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }

        public bool OpenForComments { get; set; }

        [StringLength(MaxSummary)]
        public MultiString CompleteSummary { get; set; }
        [StringLength(MaxText)]
        public MultiString CompleteText { get; set; }
        [DontSave]
        public string Summary {
            get {
                return CompleteSummary[MultiString.ActiveLanguage];
            }
            set {
                CompleteSummary[MultiString.ActiveLanguage] = value;
            }
        }
        [DontSave]
        public string Text {
            get {
                return CompleteText[MultiString.ActiveLanguage];
            }
            set {
                CompleteText[MultiString.ActiveLanguage] = value;
            }
        }
        [DontSave]
        public string DisplayableSummary {
            get {
                if (!string.IsNullOrWhiteSpace(Summary)) return Summary;
                string summary = Text;
                if (summary.Length > MaxSummary)
                    summary = summary.Truncate(MaxSummary).Trim() + " ...";
                // remove all <tags> as they may not be properly ended because we truncated the summary
                // This may result in formatting loss, in which case an explicit summary should be provided
                return RemoveHtmlExceptP(summary);
            }
        }
        private string RemoveHtmlExceptP(string summary) {
            Regex re1 = new Regex(@"<\s*(?'tag'[a-zA-Z0-9_]+)[^>]*(?'end'>){0,1}");
            Regex re2 = new Regex(@"</\s*(?'tag'[a-zA-Z0-9_]+)\s*(?'end'>){0,1}");
            pTags = 0;
            summary = re1.Replace(summary, MatchEval);
            int openTags = pTags;
            pTags = 0;
            summary = re2.Replace(summary, MatchEval);
            int closeTags = pTags;
            // we only preserve <p> but we have to make sure that open/closing <p> tags match
            while (openTags > closeTags) {
                summary += ("</p>");
                ++closeTags;
            }
            return summary;
        }
        private int pTags;
        private string MatchEval(Match m) {
            string tag = m.Groups["tag"].Value.Trim();
            string end = m.Groups["end"].Value.Trim();
            if (string.IsNullOrWhiteSpace(end)) return "";// partial, clipped tag
            if (tag == "p") {
                ++pTags;
                return m.Value;
            } else if (tag == "br")
                return m.Value;
            else
                return "";
        }
        [DontSave]
        public string DisplayableSummaryText {
            get {
                string summary;
                if (!string.IsNullOrWhiteSpace(Summary))
                    summary = Summary;
                else {
                    summary = Text;
                    if (summary.Length > MaxSummary)
                        summary = summary.Truncate(MaxSummary).Trim() + " ...";
                }
                // remove all <tags> as they may not be properly ended because we truncated the summary
                // This may result in formatting loss, in which case an explicit summary should be provided
                summary = RemoveHtml(summary);
                summary = YetaWFManager.HtmlDecode(summary);
                return summary;
            }
        }
        private string RemoveHtml(string summary) {
            Regex re1 = new Regex(@"<[^>]*>");
            Regex re2 = new Regex(@"<[^>]*(?'end'>){0,1}");// partial, clipped tags
            summary = re1.Replace(summary, "");
            summary = re2.Replace(summary, "");
            return summary;
        }

        /// <summary>
        /// Returns the number of comments to approve for this blog entry
        /// </summary>
        /// <remarks>
        /// When using file I/O, this is definitely limiting performance. When using more than just a few blog entries, a better I/O method should be used.
        /// </remarks>
        [Data_CalculatedProperty]
        public int CommentsUnapproved { get; set; }

        /// <summary>
        /// Returns the number of comments for this blog entry
        /// </summary>
        /// <remarks>
        /// When using file I/O, this is definitely limiting performance. When using more than just a few blog entries, a better I/O method should be used.
        /// </remarks>
        [Data_CalculatedProperty]
        public int Comments { get; set; }

        public MultiString Category {
            get {
                using (BlogCategoryDataProvider categoryDP = new BlogCategoryDataProvider()) {
                    BlogCategory blogCategory = categoryDP.GetItem(CategoryIdentity);
                    if (blogCategory != null)
                        return blogCategory.Category;
                    return new MultiString();
                }
            }
        }

        public BlogEntry() {
            Title = new MultiString();
            Keywords = new MultiString();
            UniqueId = Guid.NewGuid();
            DatePublished = DateTime.UtcNow;
            DateCreated = DateTime.UtcNow;
            DateUpdated = null;
            OpenForComments = true;
            CompleteSummary = new MultiString();
            CompleteText = new MultiString();
        }
        public static Guid FolderGuid { get { return ModuleDefinition.GetPermanentGuid(typeof(EntryDisplayModule)); } }
    }

    public class BlogEntryDataProvider : DataProviderImpl, IInstallableModel, ISearchDynamicUrls, ISiteMapDynamicUrls {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public BlogEntryDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, BlogEntry> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, BlogEntry> CreateDataProvider() {
            Package package = YetaWF.Modules.Blog.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Entries", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public BlogEntry GetItem(int blogEntry) {
            BlogEntry data = DataProvider.Get(blogEntry);
            if (data == null) return null;
            data.Identity = blogEntry;
            return data;
        }
        public bool AddItem(BlogEntry data) {
            return DataProvider.Add(data);
        }
        public UpdateStatusEnum UpdateItem(BlogEntry data) {
            data.DateUpdated = DateTime.UtcNow;
            return DataProvider.Update(data.Identity, data.Identity, data);
        }
        public bool RemoveItem(int blogEntry) {
            bool status = DataProvider.Remove(blogEntry);
            if (!status) return false;
            // remove comments for this entry
            using (BlogCommentDataProvider commentDP = new BlogCommentDataProvider(blogEntry)) {
                commentDP.RemoveAllComments();
            }
            return true;
        }
        public List<BlogEntry> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            return DataProvider.GetRecords(skip, take, sort, filters, out total);
        }
        public bool RemoveEntries(int categoryIdentity) {
            // TODO: This could be optimized for SQL using joins
            // remove all entries for this category
            List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "CategoryIdentity", Operator = "==", Value = categoryIdentity });
            int total;
            List<BlogEntry> entries = GetItems(0, 0, null, filters, out total);
            foreach (BlogEntry entry in entries) {
                // remove all comments
                using (BlogCommentDataProvider commentDP = new BlogCommentDataProvider(entry.Identity)) {
                    commentDP.RemoveAllComments();
                }
            }
            return true;
        }
    
        // ISEARCHDYNAMICURLS
        // ISEARCHDYNAMICURLS
        // ISEARCHDYNAMICURLS

        public void KeywordsForDynamicUrls(ISearchWords searchWords) {

            using (this) {
                BlogConfigData config = BlogConfigDataProvider.GetConfig();
                int total;
                List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "Published", Operator = "==", Value = true });
                List<BlogEntry> entries = GetItems(0, 0, null, filters, out total);
                foreach (BlogEntry entry in entries) {

                    string url = BlogConfigData.GetEntryCanonicalName(entry.Identity);

                    PageDefinition page = PageDefinition.LoadFromUrl(url);
                    if (page == null) return; // there is no such root page
                    if (!searchWords.WantPage(page)) return;

                    if (searchWords.SetUrl(url, page.PageSecurity, entry.Title, entry.DisplayableSummaryText, entry.DateCreated, entry.DateUpdated, page.IsAuthorized_View_Anonymous(), page.IsAuthorized_View_AnyUser())) {
                        searchWords.AddObjectContents(entry);
                        using (BlogCommentDataProvider commentDP = new BlogCommentDataProvider(entry.Identity)) {
                            int totalComments;
                            List<BlogComment> comments = commentDP.GetItems(0, 0, null, null, out totalComments);
                            foreach (BlogComment comment in comments) {
                                searchWords.AddObjectContents(comment);
                            }
                        }
                        searchWords.Save();
                    }
                }
            }
        }

        // ISITEMAPDYNAMICURLS
        // ISITEMAPDYNAMICURLS
        // ISITEMAPDYNAMICURLS

        public void FindDynamicUrls(Action<PageDefinition, string, DateTime?, PageDefinition.SiteMapPriorityEnum, PageDefinition.ChangeFrequencyEnum, object> addDynamicUrl, Func<PageDefinition, bool> validForSiteMap) {
            using (this) {
                int total;
                List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "Published", Operator = "==", Value = true });
                List<BlogEntry> entries = GetItems(0, 0, null, filters, out total);
                foreach (BlogEntry entry in entries) {
                    string url = BlogConfigData.GetEntryCanonicalName(entry.Identity);
                    PageDefinition page = PageDefinition.LoadFromUrl(url);
                    if (page == null) return; // there is no such root page
                    addDynamicUrl(page, url, entry.DateUpdated, page.SiteMapPriority, page.ChangeFrequency, entry);
                }
            }
        }
    }
}
