/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Audit;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
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
        [Data_NewValue]
        public TimeSpan TimeSpan { get; set; }
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
                Next = DateTime.UtcNow.Add(Frequency.TimeSpan);
        }
    }

    public class SchedulerDataProvider : DataProviderImpl, IInstallableModel, IInstallableModel2 {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public SchedulerDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<string, SchedulerItemData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<string, SchedulerItemData> CreateDataProvider() {
            Package package = YetaWF.Modules.Scheduler.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName, Cacheable: true, Parms: new { NoLanguages = true });
        }

        // API
        // API
        // API

        public async Task<SchedulerItemData> GetItemAsync(string key) {
            return await DataProvider.GetAsync(key);
        }
        public async Task<bool> AddItemAsync(SchedulerItemData evnt) {
            evnt.TimeSpan = evnt.Frequency.TimeSpan;
            return await DataProvider.AddAsync(evnt);
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(SchedulerItemData evnt) {
            return await UpdateItemAsync(evnt.Name, evnt);
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(string originalName, SchedulerItemData evnt) {
            evnt.TimeSpan = evnt.Frequency.TimeSpan;
            return await DataProvider.UpdateAsync(originalName, evnt.Name, evnt);
        }
        public async Task<bool> RemoveItemAsync(string key) {
            return await DataProvider.RemoveAsync(key);
        }
        public async Task<DataProviderGetRecords<SchedulerItemData>> GetItemsAsync(List<DataProviderFilterInfo> filters) {
            filters = FixFilters(filters);
            return await DataProvider.GetRecordsAsync(0, 0, null, filters);
        }
        public async Task<DataProviderGetRecords<SchedulerItemData>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            filters = FixFilters(filters);
            sort = FixSort(sort);
            return await DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
        public async Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            filters = FixFilters(filters);
            int result = await DataProvider.RemoveRecordsAsync(filters);
            return result;
        }
        // Replace IsRunning ... with  Next == DateTime.MaxValue
        private List<DataProviderSortInfo> FixSort(List<DataProviderSortInfo> sort) {
            if (sort == null) return null;
            List<DataProviderSortInfo> newSort = new List<DataProviderSortInfo>();
            foreach (DataProviderSortInfo s in sort) {
                if (s.Field == "IsRunning") {
                    newSort.Add(new DataProviderSortInfo { Field = "Next", Order = s.Order });
                } else if (s.Field == "Frequency") {
                    newSort.Add(new DataProviderSortInfo { Field = "TimeSpan", Order = s.Order });
                } else {
                    newSort.Add(s);
                }
            }
            return newSort;
        }

        private List<DataProviderFilterInfo> FixFilters(List<DataProviderFilterInfo> filters) {
            if (filters == null) return filters;
            List<DataProviderFilterInfo> newFilters = new List<DataProviderFilterInfo>();
            DataProviderFilterInfo.NormalizeFilters(typeof(SchedulerItemData), filters);
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

        // APPSETTINGS.JSON

        public async Task SetRunningAsync(bool running) {
            if (running != GetRunning()) {
                WebConfigHelper.SetValue<bool>(AreaRegistration.CurrentPackage.AreaName, "Running", running);
                await WebConfigHelper.SaveAsync();
                await Auditing.AddAuditAsync($"{nameof(SchedulerDataProvider)}.{nameof(SetRunningAsync)}", "Scheduler", Guid.Empty,
                    $"{nameof(SetRunningAsync)}({running})", RequiresRestart: true
                );
            }
        }
        public bool GetRunning() {
            return WebConfigHelper.GetValue<bool>(AreaRegistration.CurrentPackage.AreaName, "Running");
        }

        // IINSTALLABLEMODEL2
        // IINSTALLABLEMODEL2
        // IINSTALLABLEMODEL2

        public async Task<bool> UpgradeModelAsync(List<string> errorList, string lastSeenVersion) {

            // Convert 2.7.0 (and older) data and add TimeSpan
            if (Package.CompareVersion(lastSeenVersion, AreaRegistration.CurrentPackage.Version) < 0 &&
                    Package.CompareVersion(lastSeenVersion, "2.7.0") <= 0) {
                using (SchedulerDataProvider schedDP = new SchedulerDataProvider()) {
                    const int chunk = 100;
                    int skip = 0;
                    for (; ; skip += chunk) {
                        DataProviderGetRecords<SchedulerItemData> list = await schedDP.GetItemsAsync(skip, chunk, null, null);
                        if (list.Data.Count <= 0)
                            break;
                        foreach (SchedulerItemData l in list.Data) {
                            l.TimeSpan = l.Frequency.TimeSpan;
                            await UpdateItemAsync(l.Name, l);
                        }
                    }
                }
            }
            return true;
        }
    }
}
