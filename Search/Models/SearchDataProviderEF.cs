/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Search/Topic/License */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Dynamic;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.DataProvider.EF;

namespace YetaWF.Modules.Search.DataProvider {

    [Table("YetaWF_Search_SearchData")]
    public class SearchData {

        public SearchData() { }

        public const int MaxSearchTerm = 100;

        public int SearchDataId { get; set; }

        [Index("IX_DateAdded", IsUnique = false), Required]
        public DateTime DateAdded { get; set; }

        [Index("IX_SearchTerm", 1, IsUnique = false), Required]
        public int SiteIdentity { get; set; }
        [Index("IX_SearchTerm", 2, IsUnique = false), StringLength(MaxSearchTerm), Required]
        public string SearchTerm { get; set; }

        [Index("IX_Language", IsUnique = false), StringLength(MultiString.MaxLanguage), Required]
        public string Language { get; set; }

        public int Count { get; set; }
        public bool AllowAnonymous { get; set; }
        public bool AllowAnyUser { get; set; }

        public virtual SearchDataUrl SearchDataUrl { get; set; }
    }

    [Table("YetaWF_Search_SearchDataUrl")]
    public class SearchDataUrl {

        public const int MaxDescription = 100;

        public SearchDataUrl() { }

        public int SearchDataUrlId { get; set; }

        [Index("IX_PageUrl", 1, IsUnique = true), Required]
        public int SiteIdentity { get; set; }
        [StringLength(Globals.MaxUrl)]
        [Index("IX_PageUrl", 2, IsUnique = true), Required]
        public string PageUrl { get; set; }

        [StringLength(MaxDescription)]
        public string Description { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
    }

    public class SearchResult {

        public int Count { get; set; }
        [UIHint("Url")]
        public string PageUrl { get; set; }
        [UIHint("String")]
        public string Description { get; set; }
        [UIHint("DateTime")]
        public DateTime DateCreated { get; set; }
        [UIHint("DateTime")]
        public DateTime DateUpdated { get; set; }

        public SearchResult() { }
    }

    public partial class SearchDataProvider : EFDataProviderImpl<DBContainer, SearchData, int> {

        static object lockObject = new object();

        public SearchData GetItem(int id) {
            return GetItem(id, (key) => (from t in DB.SearchDatas where t.SearchDataId == id select t).FirstOrDefault());
        }
        public bool AddItem(SearchData data) {
            return AddItem(data, (rec) => DB.SearchDatas.Add(rec), () => DB.SaveChanges());
        }
        public bool AddItems(List<SearchData> list, string pageUrl, string pageDescription, DateTime pageCreated, DateTime? pageUpdated, DateTime searchStarted) {
            lock (lockObject) {
                SearchDataUrl url = (from t in DB.SearchDataUrls where t.PageUrl == pageUrl && Manager.CurrentSite.Identity == t.SiteIdentity select t).FirstOrDefault();
                if (url == null)
                    url = new SearchDataUrl() { SiteIdentity = Manager.CurrentSite.Identity, PageUrl = pageUrl };
                if (pageUpdated != null && (DateTime)pageUpdated < pageCreated)
                    pageCreated = (DateTime)pageUpdated;
                url.Description = pageDescription;
                url.DateCreated = pageCreated;
                url.DateUpdated = pageUpdated ?? pageCreated;
                foreach (SearchData data in list) {
                    data.SearchDataUrl = url;
                    data.DateAdded = searchStarted;
                }
                DB.SearchDatas.AddRange(list);
                DB.SaveChanges();
            }
            return true;
        }

        public UpdateStatusEnum UpdateItem(SearchData data) {
            // this can't change the url
            return UpdateItem(data.SearchDataId, data,
                (key) => (from t in DB.SearchDatas where t.SearchDataId == key select t).FirstOrDefault(),
                () => DB.SaveChanges());
        }
        public bool RemoveItem(int id) {
            lock (lockObject) {
                SearchData data = (from t in DB.SearchDatas where t.SearchDataId == id select t).FirstOrDefault();
                if (data == null)
                    return false;
                DB.SearchDatas.Remove(data);
                RemoveUnusedUrls();
                DB.SaveChanges();
            }
            return true;
        }
        public List<SearchData> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo() { Field = "SearchDataUrl.SiteIdentity", Operator = "==", Value = Manager.CurrentSite.Identity });
            IQueryable<SearchData> list = DB.SearchDatas;
            list = base.GetItems(list, skip, take, sort, filters, out total,
                (recs, order) => recs.OrderBy(order),
                (recs) => recs.OrderBy(s => s.SearchTerm)
            );
            return new List<SearchData>(list.ToList());
        }
        public void RemoveAllItems() { // for current site
            lock (lockObject) {
                int total;
                List<SearchData> list = GetItems(0, 0, null, null, out total);
                DB.SearchDatas.RemoveRange(list);
                RemoveUnusedUrls();
                DB.SaveChanges();
            }
        }
        public void RemoveOldItems(DateTime date) {
            lock (lockObject) {
                int total;
                List<SearchData> list = GetItems(0, 0, null, new List<DataProviderFilterInfo> { new DataProviderFilterInfo { Field = "DateAdded", Operator = "<", Value = date } }, out total);
                DB.SearchDatas.RemoveRange(list);
                DB.SaveChanges();
                RemoveUnusedUrls();
                DB.SaveChanges();
            }
        }
        private void RemoveUnusedUrls() {
            IEnumerable<SearchDataUrl> usedUrls = (from s in DB.SearchDatas select s.SearchDataUrl).Distinct(); // get the list of all USED urls
            IEnumerable<SearchDataUrl> removedUrlIds = DB.SearchDataUrls.Except(usedUrls);
            DB.SearchDataUrls.RemoveRange(removedUrlIds);
            DB.SaveChanges();
        }

        public List<SearchResult> GetSearchResults(string searchTerms, int maxResults, string languageId, bool haveUser, out bool haveMore) {
            haveMore = false;
            List<SearchResult> results = Parse(searchTerms, maxResults, languageId, haveUser, out haveMore);
            return results;
        }
        /// <summary>
        /// Check whether the specified url contents has changed since last time we collected keywords
        /// </summary>
        public bool PageUpdated(string pageUrl, DateTime dateCreated, DateTime? dateUpdated) {
            if (dateCreated == DateTime.MinValue && dateUpdated == DateTime.MinValue)// if no one supplied a date, we don't know
                return true;
            SearchDataUrl url = (from t in DB.SearchDataUrls where t.PageUrl == pageUrl && Manager.CurrentSite.Identity == t.SiteIdentity select t).FirstOrDefault();
            if (url == null)
                return true;
            DateTime dataAge = url.DateCreated;
            if (url.DateUpdated != null && (DateTime)url.DateUpdated < dataAge)
                dataAge = (DateTime)url.DateUpdated;
            DateTime newAge = dateCreated;
            if (dateUpdated != null && (DateTime)dateUpdated < newAge)
                newAge = (DateTime)dateUpdated;
            if (dataAge.AddSeconds(1) < newAge) // all for slight difference in timestamps
                return true;
            // update the dateadded datetime for all search keywords on this page to reflect that we didn't search and just accept them again
            lock (lockObject) {
                DateTime now = DateTime.UtcNow;
                DB.SearchDatas
                    .Where(t => t.SearchDataUrl.SearchDataUrlId == url.SearchDataUrlId && Manager.CurrentSite.Identity == t.SiteIdentity)
                    .ToList()
                    .ForEach(data => data.DateAdded = now);
                DB.SaveChanges();
            }
            return false;
        }

        public void Install() {
            DB.Install();
        }
        public void Uninstall() {
            DB.Uninstall();
        }
    }
}
