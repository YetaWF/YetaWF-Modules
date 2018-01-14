﻿/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Collections.Generic;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Extensions;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

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

    public class VisitorEntryDataProvider : DataProviderImpl, IInstallableModel {

        public bool Usable { get; private set; }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public VisitorEntryDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public VisitorEntryDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, VisitorEntry> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, VisitorEntry> CreateDataProvider() {
            Package package = YetaWF.Modules.Visitors.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package.AreaName,
                () => { // File
                    // accept so we can run without failure. However, it's only usable with Sql
                    Usable = false;
                    return null;
                },
                (dbo, conn) => {  // SQL
                    Usable = true;
                    return new SQLSimpleObjectDataProvider<int, VisitorEntry>(AreaName, dbo, conn,
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                () => { // External
                    IDataProvider<int, VisitorEntry> dp = MakeExternalDataProvider(new { AreaName = AreaName, CurrentSiteIdentity = SiteIdentity, Cacheable = true });
                    Usable = dp != null;
                    return dp;
                }
            );
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
            return string.Format("{0}_{1}", AreaName, key);
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
            Info info = new Info();
            if (!Usable) return info;
            SQLSimpleObjectDataProvider<int, VisitorEntry> sqlDP = (SQLSimpleObjectDataProvider<int, VisitorEntry>)DataProvider;
            DateTime now = DateTime.Now.Date.ToUniversalTime();
            string startDate = string.Format("{0}", now);
            string endDate = string.Format("{0}", now.AddDays(1));
            info.TodaysAnonymous = sqlDP.Direct_ScalarInt(
                "SELECT Count(DISTINCT SessionId) FROM {TableName} Where " +
                    string.Format("AccessDateTime >= '{0}' AND AccessDateTime < '{1}'", startDate, endDate) +
                    " AND [UserId] = 0" +
                    " AND {__Site}"
            );
            info.TodaysUsers = sqlDP.Direct_ScalarInt(
                "SELECT Count(DISTINCT UserId) FROM {TableName} Where " +
                    string.Format("AccessDateTime >= '{0}' AND AccessDateTime < '{1}'", startDate, endDate) +
                    " AND [UserId] <> 0" +
                    " AND {__Site}"
            );
            startDate = string.Format("{0}", now.AddDays(-1));
            endDate = string.Format("{0}", now);
            info.YesterdaysAnonymous = sqlDP.Direct_ScalarInt(
                "SELECT Count(DISTINCT SessionId) FROM {TableName} Where " +
                    string.Format("AccessDateTime >= '{0}' AND AccessDateTime < '{1}'", startDate, endDate) +
                    " AND [UserId] = 0" +
                    " AND {__Site}"
            );
            info.YesterdaysUsers = sqlDP.Direct_ScalarInt(
                "SELECT Count(DISTINCT UserId) FROM {TableName} Where " +
                    string.Format("AccessDateTime >= '{0}' AND AccessDateTime < '{1}'", startDate, endDate) +
                    " AND [UserId] <> 0" +
                    " AND {__Site}"
            );
            return info;
        }

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
