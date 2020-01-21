/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Extensions;
using YetaWF.Core.IO;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Search.Controllers;

namespace YetaWF.Modules.Search.DataProvider {
    public class SearchData {

        public const int MaxSearchTerm = 100;

        [Data_PrimaryKey, Data_Identity]
        public int SearchDataId { get; set; }
        [Data_Index]
        public DateTime DateAdded { get; set; }
        [Data_Index, StringLength(MaxSearchTerm)]
        public string SearchTerm { get; set; }
        [Data_Index, StringLength(MultiString.MaxLanguage)]
        public string Language { get; set; }
        public int Count { get; set; }
        public bool AllowAnonymous { get; set; }
        public bool AllowAnyUser { get; set; }

        public int SearchDataUrlId { get; set; }

        /* Obtained from SearchUrlData Join */
        [Data_DontSave]
        public PageDefinition.PageSecurityType PageSecurity { get; set; }
        [Data_DontSave]
        public string PageUrl { get; set; }
        [Data_DontSave]
        public string PageTitle { get; set; }
        [Data_DontSave]
        public DateTime DatePageCreated { get; set; }
        [Data_DontSave]
        public DateTime DatePageUpdated { get; set; }
        [Data_DontSave]
        public string PageSummary { get; set; }

        public SearchData() { }
    }
    public class SearchDataComparer : IEqualityComparer<SearchData> {
        public bool Equals(SearchData x, SearchData y) {
            return x.SearchDataUrlId == y.SearchDataUrlId;
        }
        public int GetHashCode(SearchData obj) {
            return obj.SearchDataUrlId;
        }
    }

    public interface ISearchDataProviderIOMode {
        Task RemoveUnusedUrlsAsync(SearchDataProvider searchDP);
        Task MarkUpdatedAsync(int searchDataUrlId);
    }

    public class SearchDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public SearchDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public SearchDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, SearchData> DataProvider { get { return GetDataProvider(); } }
        private ISearchDataProviderIOMode DataProviderIOMode { get { return GetDataProvider(); } }

        private IDataProvider<int, SearchData> CreateDataProvider() {
            Package package = YetaWF.Modules.Search.Controllers.AreaRegistration.CurrentPackage;
            dynamic dp = MakeDataProvider(package, package.AreaName + "_Data", SiteIdentity: SiteIdentity, Cacheable: true);
            if (dp != null)
                Usable = true;
            return dp;
        }

        private bool Usable { get; set; }

        // API
        // API
        // API

        public static bool IsUsable {
            get {
                using (SearchDataProvider searchDP = new SearchDataProvider()) {
                    return searchDP.Usable;
                }
            }
        }

        internal async Task<SearchData> GetItemWithUrlAsync(int searchDataId) {
            if (!IsUsable) return null;
            using (SearchDataUrlDataProvider searchUrlDP = new SearchDataUrlDataProvider()) {
                List<DataProviderFilterInfo> filters = null;
                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "SearchDataId", Operator = "==", Value = searchDataId });
                List<JoinData> joins = new List<JoinData> {
                    new JoinData {MainDP = this, JoinDP= searchUrlDP, MainColumn = "SearchDataUrlId", JoinColumn = "SearchDataUrlId" },
                };
                return await DataProvider.GetOneRecordAsync(filters, Joins: joins);
            }
        }

        public async Task<bool> AddItemAsync(SearchData data) {
            if (!IsUsable) return false;
            return await DataProvider.AddAsync(data);
        }
        public async Task<bool> AddItemsAsync(List<SearchData> list, string pageUrl, PageDefinition.PageSecurityType pageSecurity, string pageDescription, string pageSummary, DateTime pageCreated, DateTime? pageUpdated, DateTime searchStarted) {
            if (!IsUsable) return false;
            bool status = false;
            using (ILockObject lockObject = await YetaWF.Core.IO.Caching.LockProvider.LockResourceAsync($"{AreaRegistration.CurrentPackage.AreaName}_{nameof(SearchDataProvider)}")) {
                if (pageUpdated != null && (DateTime)pageUpdated < pageCreated)
                    pageCreated = (DateTime)pageUpdated;
                using (SearchDataUrlDataProvider searchUrlDP = new SearchDataUrlDataProvider()) {
                    SearchDataUrl searchUrl = await searchUrlDP.GetItemByUrlAsync(pageUrl);
                    if (searchUrl == null) {
                        searchUrl = new SearchDataUrl {
                            DatePageCreated = pageCreated,
                            DatePageUpdated = pageUpdated,
                            PageTitle = pageDescription.Truncate(SearchDataUrl.MaxTitle),
                            PageUrl = pageUrl,
                            PageSecurity = pageSecurity,
                            PageSummary = pageSummary.Truncate(SearchDataUrl.MaxSummary),
                        };
                        if (!await searchUrlDP.AddItemAsync(searchUrl))
                            throw new InternalError("Unexpected error adding SearchDataUrl for url {0}", pageUrl);
                    } else {
                        searchUrl.PageTitle = pageDescription.Truncate(SearchDataUrl.MaxTitle);
                        searchUrl.PageSummary = pageSummary.Truncate(SearchDataUrl.MaxSummary);
                        searchUrl.DatePageCreated = pageCreated;
                        searchUrl.DatePageUpdated = pageUpdated ?? pageCreated;
                        searchUrl.PageSecurity = pageSecurity;
                        UpdateStatusEnum updStatus = await searchUrlDP.UpdateItemAsync(searchUrl);
                        if (updStatus != UpdateStatusEnum.OK)
                            throw new InternalError("Unexpected error updating SearchDataUrl for url {0} - {1}", pageUrl, updStatus);
                    }
                    foreach (SearchData data in list) {
                        data.SearchDataUrlId = searchUrl.SearchDataUrlId;
                        data.DateAdded = searchStarted;
                        await AddItemAsync(data);
                    }
                }
                status = true;
                await lockObject.UnlockAsync();
            }
            return status;
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(SearchData data) {
            if (!IsUsable) return UpdateStatusEnum.RecordDeleted;
            return await DataProvider.UpdateAsync(data.SearchDataId, data.SearchDataId, data);
        }
        public async Task<bool> RemoveItemAsync(int id) {
            if (!IsUsable) return false;
            return await DataProvider.RemoveAsync(id);
        }
        public async Task<DataProviderGetRecords<SearchData>> GetItemsWithUrlAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            if (!IsUsable)
                return new DataProviderGetRecords<SearchData>();
            using (SearchDataUrlDataProvider searchUrlDP = new SearchDataUrlDataProvider()) {
                List<JoinData> joins = new List<JoinData> {
                    new JoinData {MainDP = this, JoinDP = searchUrlDP, MainColumn = nameof(SearchData.SearchDataUrlId), JoinColumn = nameof(SearchDataUrl.SearchDataUrlId) },
                };
                return await DataProvider.GetRecordsAsync(skip, take, sort, filters, Joins: joins);
            }
        }
        public async Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            if (!IsUsable) return 0;
            int count = 0;
            count = await DataProvider.RemoveRecordsAsync(filters);
            if (filters == null) {
                using (SearchDataUrlDataProvider searchUrlDP = new SearchDataUrlDataProvider()) {
                    await searchUrlDP.RemoveItemsAsync(null);
                }
            } else {
                await RemoveUnusedUrlsAsync();
            }
            return count;
        }

        public async Task RemoveOldItemsAsync(DateTime searchStarted) {
            if (!IsUsable) return;
            using (ILockObject lockObject = await YetaWF.Core.IO.Caching.LockProvider.LockResourceAsync($"{AreaRegistration.CurrentPackage.AreaName}_{nameof(SearchDataProvider)}")) {
                List<DataProviderFilterInfo> filters = null;
                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "DateAdded", Operator = "<", Value = searchStarted });
                await RemoveItemsAsync(filters);
                await RemoveUnusedUrlsAsync();
                await lockObject.UnlockAsync();
            }
        }
        private async Task RemoveUnusedUrlsAsync() {
            await DataProviderIOMode.RemoveUnusedUrlsAsync(this);
        }

        /// <summary>
        /// Check whether the specified url contents have changed since last time we collected keywords
        /// </summary>
        public async Task<bool> PageUpdated(string pageUrl, DateTime dateCreated, DateTime? dateUpdated) {
            if (dateCreated == DateTime.MinValue && (dateUpdated == null || dateUpdated == DateTime.MinValue))// if no one supplied a date, we don't know
                return true;
            using (SearchDataUrlDataProvider searchUrlDP = new SearchDataUrlDataProvider()) {
                SearchDataUrl searchUrl = await searchUrlDP.GetItemByUrlAsync(pageUrl);
                if (searchUrl == null)
                    return true;
                DateTime dataAge = searchUrl.DatePageCreated;
                if (searchUrl.DatePageUpdated != null && (DateTime)searchUrl.DatePageUpdated < dataAge)
                    dataAge = (DateTime)searchUrl.DatePageUpdated;
                DateTime newAge = dateCreated;
                if (dateUpdated != null && (DateTime)dateUpdated > newAge)
                    newAge = (DateTime)dateUpdated;
                if (dataAge.AddSeconds(1) < newAge) // allow for slight difference in timestamps
                    return true;
                // update the dateadded datetimes for all search terms on this page to reflect that we didn't search and just accept them again
                await MarkUpdatedAsync(searchUrl.SearchDataUrlId);
            }
            return false;
        }
        private async Task MarkUpdatedAsync(int searchDataUrlId) {
            using (ILockObject lockObject = await YetaWF.Core.IO.Caching.LockProvider.LockResourceAsync($"{AreaRegistration.CurrentPackage.AreaName}_{nameof(SearchDataProvider)}")) {
                await DataProviderIOMode.MarkUpdatedAsync(searchDataUrlId);
                await lockObject.UnlockAsync();
            }
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public new async Task<bool> IsInstalledAsync() {
            if (DataProvider == null) return false;
            return await DataProvider.IsInstalledAsync();
        }
        public new async Task<bool> InstallModelAsync(List<string> errorList) {
            if (!IsUsable) return true;
            return await DataProvider.InstallModelAsync(errorList);
        }
        public new async Task<bool> UninstallModelAsync(List<string> errorList) {
            if (!IsUsable) return true;
            return await DataProvider.UninstallModelAsync(errorList);
        }
        public new async Task AddSiteDataAsync() {
            if (!IsUsable) return;
            await DataProvider.AddSiteDataAsync();
        }
        public new async Task RemoveSiteDataAsync() {
            if (!IsUsable) return;
            await DataProvider.RemoveSiteDataAsync();
        }
        public new Task<DataProviderExportChunk> ExportChunkAsync(int chunk, SerializableList<SerializableFile> fileList) {
            // we don't export search data
            return Task.FromResult(new DataProviderExportChunk());
            //if (!IsUsable) return false;
            //return DataProvider.ExportChunk(chunk, fileList, out obj);
        }
        public new Task ImportChunkAsync(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            // we don't import search data
            return Task.CompletedTask;
            //if (!IsUsable) return;
            //DataProvider.ImportChunk(chunk, fileList, obj);
        }
    }
}
