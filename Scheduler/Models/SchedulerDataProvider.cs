/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Scheduler.Controllers;

namespace YetaWF.Modules.Scheduler.DataProvider {
    public class SchedulerItemData {

        public const int MaxName = 50;
        public const int MaxDescription = 1000;
        public const int MaxErrors = 40000;

        [Data_PrimaryKey, StringLength(MaxName)]
        public string Name { get; set; }
        [StringLength(MaxDescription)]
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public bool EnableOnStartup { get; set; }
        public bool RunOnce { get; set; }
        public bool Startup { get; set; }
        public bool SiteSpecific { get; set; }
        public SchedulerFrequency Frequency { get; set; }
        public SchedulerEvent Event { get; set; }

        public DateTime? Last { get; set; }
        public DateTime? Next { get; set; }
        public TimeSpan RunTime { get; set; }
        [StringLength(MaxErrors)]
        public string Errors { get; set; }

        public SchedulerItemData() {
            Frequency = new SchedulerFrequency();
            Event = new SchedulerEvent();
            Next = null;
        }

        [DontSave]
        public bool IsRunning {
            get {
                return Next == DateTime.MaxValue;
            }
            set {
                if (value)
                    Next = DateTime.MaxValue;
                else
                    SetNextRuntime();
            }
        }
        public void SetNextRuntime() {
            Next = null;
            if (Enabled)
                Next = DateTime.UtcNow.Add(Frequency.GetTimeSpan());
        }
    }

    public class SchedulerDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public SchedulerDataProvider() : base(0) { SetDataProvider(DataProvider); }

        private IDataProvider<string, SchedulerItemData> DataProvider {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName)) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            _dataProvider = new YetaWF.DataProvider.FileDataProvider<string, SchedulerItemData>(
                                Path.Combine(YetaWFManager.DataFolder, AreaName),
                                Cacheable: true);
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLSimpleObjectDataProvider<string, SchedulerItemData>(AreaName, SQLDbo, SQLConn,
                                NoLanguages: true,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<string, SchedulerItemData> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public SchedulerItemData GetItem(string key) {
            return DataProvider.Get(key);
        }
        public bool AddItem(SchedulerItemData evnt) {
            return DataProvider.Add(evnt);
        }
        public UpdateStatusEnum UpdateItem(SchedulerItemData evnt) {
            return UpdateItem(evnt.Name, evnt);
        }
        public UpdateStatusEnum UpdateItem(string originalName, SchedulerItemData evnt) {
            return DataProvider.Update(originalName, evnt.Name, evnt);
        }
        public bool RemoveItem(string key) {
            return DataProvider.Remove(key);
        }

        public List<SchedulerItemData> GetItems(List<DataProviderFilterInfo> filters) {
            int total;
            filters = FixFilters(filters);
            return DataProvider.GetRecords(0, 0, null, filters, out total);
        }
        public List<SchedulerItemData> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            filters = FixFilters(filters);
            sort = FixSort(sort);
            return DataProvider.GetRecords(skip, take, sort, filters, out total);
        }
        public int RemoveItems(List<DataProviderFilterInfo> filters) {
            filters = FixFilters(filters);
            return DataProvider.RemoveRecords(filters);
        }
        // Replace IsRunning ... with  Next == DateTime.MaxValue
        private List<DataProviderSortInfo> FixSort(List<DataProviderSortInfo> sort) {
            if (sort == null) return null;
            List<DataProviderSortInfo> newSort = new List<DataProviderSortInfo>();
            foreach (DataProviderSortInfo s in sort) {
                if (s.Field == "IsRunning") {
                    if (s.Order == DataProviderSortInfo.SortDirection.Ascending) {
                        newSort.Add(new DataProviderSortInfo { Field = "Next", Order = DataProviderSortInfo.SortDirection.Ascending });
                    } else {
                        newSort.Add(new DataProviderSortInfo { Field = "Next", Order = DataProviderSortInfo.SortDirection.Descending });
                    }
                } else {
                    newSort.Add(s);
                }
            }
            return newSort;
        }

        // Replace IsRunning ... with  Next == DateTime.MaxValue
        private List<DataProviderFilterInfo> FixFilters(List<DataProviderFilterInfo> filters) {
            if (filters == null) return filters;
            List<DataProviderFilterInfo> newFilters = new List<DataProviderFilterInfo>();
            GridHelper.NormalizeFilters(typeof(SchedulerItemData), filters);
            foreach (DataProviderFilterInfo f in filters) {
                if (f.Field == "IsRunning") {
                    bool val;
                    if (f.Value.GetType() == typeof(bool))
                        val = (bool)f.Value;
                    else
                        throw new InternalError("Unexpected value type in filter for IsRunning");
                    if (f.Operator == "==") {
                        // nothing
                    } else if (f.Operator == "!=")
                        val = !val;
                    else
                        throw new InternalError("Unexpected operator in filter for IsRunning");
                    newFilters.Add(new DataProviderFilterInfo { Field = "Next", Operator = val ? ">=" : "<", Value = DateTime.MaxValue });
                } else if (f.Filters != null) {
                    f.Filters = FixFilters(f.Filters);
                    newFilters.Add(f);
                } else {
                    newFilters.Add(f);
                }
            }
            return newFilters;
        }

        // WEB.CONFIG/APPSETTINGS.JSON

        public void SetRunning(bool running) {
            if (running != GetRunning()) {
                WebConfigHelper.SetValue<bool>(AreaRegistration.CurrentPackage.AreaName, "Running", running);
                WebConfigHelper.Save();
            }
        }
        public bool GetRunning() {
            return WebConfigHelper.GetValue<bool>(AreaRegistration.CurrentPackage.AreaName, "Running");
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public bool IsInstalled() {
            return DataProvider.IsInstalled();
        }
        public bool InstallModel(List<string> errorList) {
            return DataProvider.InstallModel(errorList);
        }
        public bool UninstallModel(List<string> errorList) {
            return DataProvider.UninstallModel(errorList);
        }
        public void AddSiteData() {
            DataProvider.AddSiteData();
        }
        public void RemoveSiteData() {
            DataProvider.RemoveSiteData();
        }
        public bool ExportChunk(int chunk, SerializableList<SerializableFile> fileList, out object obj) {
            return DataProvider.ExportChunk(chunk, fileList, out obj);
        }
        public void ImportChunk(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            DataProvider.ImportChunk(chunk, fileList, obj);
        }
    }
}
