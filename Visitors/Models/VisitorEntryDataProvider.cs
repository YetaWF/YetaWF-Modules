/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Collections.Generic;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Extensions;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Visitors.DataProvider {

    public class VisitorEntry {

        public const int MaxSessionId = 50;
        public const int MaxError = 200;
        public const int MaxUserAgent = 400;
        public const int MaxCity = 30;
        public const int MaxRegionCode = 10;
        public const int MaxCountryCode = 10;
        public const int MaxContinentCode = 10;

        [Data_PrimaryKey, Data_Identity]
        public int Key { get; set; }

        [StringLength(MaxSessionId)]
        public string SessionId { get; set; }
        [Data_Index]
        public DateTime AccessDateTime { get; set; }
        public int UserId { get; set; }
        [StringLength(Globals.MaxIP)]
        public string IPAddress { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string Url { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string Referrer { get; set; }
        [StringLength(MaxUserAgent)]
        public string UserAgent { get; set; }
        [StringLength(MaxError)]
        public string Error { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }
        [StringLength(MaxContinentCode)]
        public string ContinentCode { get; set; }
        [StringLength(MaxCountryCode)]
        public string CountryCode { get; set; }
        [StringLength(MaxRegionCode)]
        public string RegionCode { get; set; }
        [StringLength(MaxCity)]
        public string City { get; set; }

        public VisitorEntry() { }
    }

    public interface VisitorEntryDataProviderIOMode {
        VisitorEntryDataProvider.Info GetStats();
    }

    public class VisitorEntryDataProvider : DataProviderImpl, IInstallableModel, IInitializeApplicationStartup {

        public void InitializeApplicationStartup() {
            ErrorHandling.RegisterCallback(AddVisitEntryError);
            PageLogging.RegisterCallback(AddVisitEntryUrl);
        }

        public bool Usable { get { return DataProvider != null; } }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public VisitorEntryDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public VisitorEntryDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, VisitorEntry> DataProvider { get { return GetDataProvider(); } }
        private VisitorEntryDataProviderIOMode DataProviderIOMode { get { return GetDataProvider(); } }

        private IDataProvider<int, VisitorEntry> CreateDataProvider() {
            Package package = YetaWF.Modules.Visitors.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName, SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public void DoAction(int key, Action action) {
            StringLocks.DoAction(LockKey(key), () => {
                action();
            });
        }
        private string LockKey(int key) {
            return string.Format("{0}_{1}", Dataset, key);
        }
        public VisitorEntry GetItem(int key) {
            if (!Usable) return null;
            return DataProvider.Get(key);
        }
        public bool AddItem(VisitorEntry data) {
            if (!Usable) return false;
            data.Referrer = data.Referrer.Truncate(Globals.MaxUrl);
            data.Url = data.Url.Truncate(Globals.MaxUrl);
            return DataProvider.Add(data);
        }
        public UpdateStatusEnum UpdateItem(VisitorEntry data) {
            if (!Usable) return UpdateStatusEnum.RecordDeleted;
            return UpdateItem(data.Key, data);
        }
        public UpdateStatusEnum UpdateItem(int originalKey, VisitorEntry data) {
            if (!Usable) return UpdateStatusEnum.RecordDeleted;
            return DataProvider.Update(originalKey, data.Key, data);
        }
        public bool RemoveItem(int key) {
            if (!Usable) return false;
            return DataProvider.Remove(key);
        }
        public List<VisitorEntry> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            if (!Usable) {
                total = 0;
                return new List<VisitorEntry>();
            }
            return DataProvider.GetRecords(skip, take, sort, filters, out total);
        }
        public int RemoveItems(List<DataProviderFilterInfo> filters) {
            if (!Usable) return 0;
            return DataProvider.RemoveRecords(filters);
        }
        public class Info {
            public int TodaysAnonymous { get; set; }
            public int TodaysUsers { get; set; }
            public int YesterdaysAnonymous { get; set; }
            public int YesterdaysUsers { get; set; }
        }
        public Info GetStats() {
            if (!Usable) return new VisitorEntryDataProvider.Info();
            return DataProviderIOMode.GetStats();
        }

        // LOGGING CALLBACKS
        // LOGGING CALLBACKS
        // LOGGING CALLBACKS

        public static void AddVisitEntryError(string error) {
            AddVisitEntry(null, error);
        }
        public static void AddVisitEntryUrl(string url, bool full) {
            AddVisitEntry(url);
        }

        private static void AddVisitEntry(string url, string error = null) {

            if (!InCallback) {
                InCallback = true;

                try {

                    if (!YetaWFManager.HaveManager || YetaWFManager.Manager.CurrentSite == null || !YetaWFManager.Manager.HaveCurrentContext) return;
                    YetaWFManager manager = YetaWFManager.Manager;

                    using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {

                        if (!visitorDP.Usable) return;

                        GeoLocation geoLocation = new GeoLocation(manager);
                        GeoLocation.UserInfo userInfo = geoLocation.GetCurrentUserInfo();

                        string userAgent;
                        string sessionId = null;
#if MVC6
                        if (url == null)
                            url = Manager.CurrentRequest.GetDisplayUrl();
                        userAgent = Manager.CurrentRequest.Headers["User-Agent"].ToString();
                        if (Manager.HaveCurrentSession)
                            sessionId = Manager.CurrentContext.Session.Id;
#else
                        if (url == null)
                            url = manager.CurrentRequest.Url.ToString();
                        userAgent = manager.CurrentRequest.UserAgent;
                        if (manager.HaveCurrentSession)
                            sessionId = manager.CurrentContext.Session.SessionID;
#endif
                        string referrer = manager.ReferrerUrl;

                        VisitorEntry visitorEntry = new VisitorEntry {
                            SessionId = sessionId,
                            AccessDateTime = DateTime.UtcNow,
                            UserId = manager.UserId,
                            IPAddress = userInfo.IPAddress.Truncate(Globals.MaxIP),
                            Url = url != null ? url.Truncate(Globals.MaxUrl) : "",
                            Referrer = referrer != null ? referrer.Truncate(Globals.MaxUrl) : "",
                            UserAgent = userAgent != null ? userAgent.Truncate(VisitorEntry.MaxUserAgent) : "",
                            Longitude = userInfo.Longitude,
                            Latitude = userInfo.Latitude,
                            ContinentCode = userInfo.ContinentCode.Truncate(VisitorEntry.MaxContinentCode),
                            CountryCode = userInfo.CountryCode.Truncate(VisitorEntry.MaxCountryCode),
                            RegionCode = userInfo.RegionCode.Truncate(VisitorEntry.MaxRegionCode),
                            City = userInfo.City.Truncate(VisitorEntry.MaxCity),
                            Error = error.Truncate(VisitorEntry.MaxError),
                        };
                        visitorDP.AddItem(visitorEntry);
                    }
                } catch (Exception) {
                } finally {
                    InCallback = false;
                }
            }
        }
        private static bool InCallback = false;

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public new bool IsInstalled() {
            if (DataProvider == null) return false;
            return DataProvider.IsInstalled();
        }
        public new bool InstallModel(List<string> errorList) {
            if (!Usable) return true;
            return DataProvider.InstallModel(errorList);
        }
        public new bool UninstallModel(List<string> errorList) {
            if (!Usable) return true;
            return DataProvider.UninstallModel(errorList);
        }
        public new void AddSiteData() {
            if (!Usable) return;
            DataProvider.AddSiteData();
        }
        public new void RemoveSiteData() {
            if (!Usable) return;
            DataProvider.RemoveSiteData();
        }
        public new bool ExportChunk(int chunk, SerializableList<SerializableFile> fileList, out object obj) {
            // we don't export visitor data
            obj = null;
            return false;
            //if (!Usable) return false;
            //return DataProvider.ExportChunk(chunk, fileList, out obj);
        }
        public new void ImportChunk(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            // we don't import visitor data
            return;
            //if (!Usable) return;
            //DataProvider.ImportChunk(chunk, fileList, obj);
        }
    }
}
