/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

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
        public string PageUrl { get; set; }
        [Data_DontSave]
        public string PageDescription { get; set; }
        [Data_DontSave]
        public DateTime DatePageCreated { get; set; }
        [Data_DontSave]
        public DateTime DatePageUpdated { get; set; }

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

    public class SearchDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public SearchDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(DataProvider); }
        public SearchDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(DataProvider); }

        private IDataProviderIdentity<int, object, int, SearchData> DataProvider {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName + "_Data")) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            // accept so we can run without failure. However, it's only usable with Sql
                            Usable = false;
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLIdentityObjectDataProvider<int, object, int, SearchData>(AreaName, SQLDbo, SQLConn,
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            Usable = true;
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProviderIdentity<int, object, int, SearchData> _dataProvider { get; set; }

        private bool Usable { get; set; }

        public static bool IsUsable {
            get {
                using (SearchDataProvider searchDP = new SearchDataProvider()) {
                    return searchDP.Usable;
                }
            }
        }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        // Locking is only used when collecting keywords
        public void DoAction(Action action) {
            StringLocks.DoAction(LockKey(), () => {
                action();
            });
        }
        private string LockKey() {
            return GetTableName();
        }
        internal SearchData GetItemWithUrl(int searchDataId) {
            if (!IsUsable) return null;
            using (SearchDataUrlDataProvider searchUrlDP = new SearchDataUrlDataProvider()) {
                List<DataProviderFilterInfo> filters = null;
                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "SearchDataId", Operator = "==", Value = searchDataId });
                List<JoinData> joins = new List<JoinData> {
                    new JoinData {MainDP = this, JoinDP= searchUrlDP, MainColumn = "SearchDataUrlId", JoinColumn = "SearchDataUrlId" },
                };
                return DataProvider.GetOneRecord(filters, Joins: joins);
            }
        }

        public bool AddItem(SearchData data) {
            if (!IsUsable) return false;
            return DataProvider.Add(data);
        }
        public bool AddItems(List<SearchData> list, string pageUrl, string pageDescription, DateTime pageCreated, DateTime? pageUpdated, DateTime searchStarted) {
            if (!IsUsable) return false;
            bool status = false;
            DoAction(() => {
                if (pageUpdated != null && (DateTime)pageUpdated < pageCreated)
                    pageCreated = (DateTime)pageUpdated;
                using (SearchDataUrlDataProvider searchUrlDP = new SearchDataUrlDataProvider()) {
                    SearchDataUrl searchUrl = searchUrlDP.GetItemByUrl(pageUrl);
                    if (searchUrl == null) {
                        searchUrl = new SearchDataUrl {
                            DatePageCreated = pageCreated,
                            DatePageUpdated = pageUpdated,
                            PageDescription = pageDescription,
                            PageUrl = pageUrl,
                        };
                        if (!searchUrlDP.AddItem(searchUrl))
                            throw new InternalError("Unexpected error adding SearchDataUrl for url {0}", pageUrl);
                    } else {
                        searchUrl.PageDescription = pageDescription;
                        searchUrl.DatePageCreated = pageCreated;
                        searchUrl.DatePageUpdated = pageUpdated ?? pageCreated;
                        UpdateStatusEnum updStatus = searchUrlDP.UpdateItem(searchUrl);
                        if (updStatus != UpdateStatusEnum.OK)
                            throw new InternalError("Unexpected error updating SearchDataUrl for url {0} - {1}", pageUrl, updStatus);
                    }
                    foreach (SearchData data in list) {
                        data.SearchDataUrlId = searchUrl.SearchDataUrlId;
                        data.DateAdded = searchStarted;
                        AddItem(data);
                    }
                }
                status = true;
            });
            return status;
        }
        public UpdateStatusEnum UpdateItem(SearchData data) {
            if (!IsUsable) return UpdateStatusEnum.RecordDeleted;
            return DataProvider.Update(data.SearchDataId, null, data.SearchDataId, null, data);
        }
        public bool RemoveItem(int id) {
            if (!IsUsable) return false;
            return DataProvider.RemoveByIdentity(id);
        }
        public List<SearchData> GetItemsWithUrl(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            if (!IsUsable) {
                total = 0;
                return new List<SearchData>();
            }
            using (SearchDataUrlDataProvider searchUrlDP = new SearchDataUrlDataProvider()) {
                List<JoinData> joins = new List<JoinData> {
                    new JoinData {MainDP = this, JoinDP= searchUrlDP, MainColumn = "SearchDataUrlId", JoinColumn = "SearchDataUrlId" },
                };
                return DataProvider.GetRecords(skip, take, sort, filters, out total, Joins: joins).ToList();
            }
        }
        public int RemoveItems(List<DataProviderFilterInfo> filters) {
            if (!IsUsable) return 0;
            int count = 0;
            DoAction(() => {
                count = DataProvider.RemoveRecords(filters);
                if (filters == null) {
                    using (SearchDataUrlDataProvider searchUrlDP = new SearchDataUrlDataProvider()) {
                        searchUrlDP.RemoveItems(null);
                    }
                } else {
                    RemoveUnusedUrls();
                }
            });
            return count;
        }

        public void RemoveOldItems(DateTime searchStarted) {
            if (!IsUsable) return;
            DoAction(() => {
                List<DataProviderFilterInfo> filters = null;
                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "DateAdded", Operator = "<", Value = searchStarted });
                RemoveItems(filters);
                RemoveUnusedUrls();
            });
        }

        private void RemoveUnusedUrls() {
            using (SearchDataUrlDataProvider searchUrlDP = new SearchDataUrlDataProvider()) {
                DoAction(() => {
                    SQLSimpleObjectDataProvider<int, SearchData> sqlDP = (SQLSimpleObjectDataProvider<int, SearchData>)DataProvider;
                    string sql = @"
                        DELETE {UrlTableName}
                          FROM {UrlTableName}
                          LEFT JOIN {TableName} ON {UrlTableName}.[SearchDataUrlId] = {TableName}.[SearchDataUrlId]
                          WHERE {TableName}.[SearchDataUrlId] IS NULL";
                    sql = sql.Replace("{UrlTableName}", SQLDataProviderImpl.WrapBrackets(searchUrlDP.GetTableName()));
                    sqlDP.Direct_Query(GetTableName(), sql);
                });
            }
        }

        /// <summary>
        /// Check whether the specified url contents have changed since last time we collected keywords
        /// </summary>
        public bool PageUpdated(string pageUrl, DateTime dateCreated, DateTime? dateUpdated) {
            if (dateCreated == DateTime.MinValue && dateUpdated == DateTime.MinValue)// if no one supplied a date, we don't know
                return true;
            using (SearchDataUrlDataProvider searchUrlDP = new SearchDataUrlDataProvider()) {
                SearchDataUrl searchUrl = searchUrlDP.GetItemByUrl(pageUrl);
                if (searchUrl == null)
                    return true;
                DateTime dataAge = searchUrl.DatePageCreated;
                if (searchUrl.DatePageUpdated != null && (DateTime)searchUrl.DatePageUpdated < dataAge)
                    dataAge = (DateTime)searchUrl.DatePageUpdated;
                DateTime newAge = dateCreated;
                if (dateUpdated != null && (DateTime)dateUpdated < newAge)
                    newAge = (DateTime)dateUpdated;
                if (dataAge.AddSeconds(1) < newAge) // all for slight difference in timestamps
                    return true;
                // update the dateadded datetimes for all search keywords on this page to reflect that we didn't search and just accept them again
                DoAction(() => {
                    SQLSimpleObjectDataProvider<int, SearchData> sqlDP = (SQLSimpleObjectDataProvider<int, SearchData>)DataProvider;
                    string sql = @"UPDATE {TableName} Set DateAdded = GETUTCDATE() WHERE {__Site} AND [SearchDataUrlId] = {UrlId}";
                    sql = sql.Replace("{UrlId}", searchUrl.SearchDataUrlId.ToString());
                    sqlDP.Direct_Query(GetTableName(), sql);
                });
            }
            return false;
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public bool IsInstalled() {
            return DataProvider.IsInstalled();
        }
        public bool InstallModel(List<string> errorList) {
            if (!IsUsable) return true;
            return DataProvider.InstallModel(errorList);
        }
        public bool UninstallModel(List<string> errorList) {
            if (!IsUsable) return true;
            return DataProvider.UninstallModel(errorList);
        }
        public void AddSiteData() {
            if (!IsUsable) return;
            DataProvider.AddSiteData();
        }
        public void RemoveSiteData() {
            if (!IsUsable) return;
            DataProvider.RemoveSiteData();
        }
        public bool ExportChunk(int chunk, SerializableList<SerializableFile> fileList, out object obj) {
            // we don't export search data
            obj = null;
            return false;
            //if (!IsUsable) return false;
            //return DataProvider.ExportChunk(chunk, fileList, out obj);
        }
        public void ImportChunk(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            // we don't import search data
            return;
            //if (!IsUsable) return;
            //DataProvider.ImportChunk(chunk, fileList, obj);
        }
    }
}
