/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;

namespace YetaWF.Modules.Scheduler.DataProvider {

    public class LogData {

        [Data_PrimaryKey, Data_Identity]
        public int LogEntry { get; set; }

        [Data_Index]
        public DateTime TimeStamp { get; set; }
        [Data_Index]
        public long RunId { get; set; }
        [StringLength(SchedulerItemData.MaxName)]
        public string Name { get; set; }

        public Core.Log.Logging.LevelEnum Level { get; set; }
        public int SiteIdentity { get; set; }

        [StringLength(0)]
        public string Info { get; set; }

        public LogData() { }
    }

    public interface ILogDataProviderIOModeAsync {
        Task<bool> AddItemAsync(LogData data);
        Task<DataProviderGetRecords<LogData>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters);
        Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters);
        bool CanBrowse { get; }
        bool CanImportOrExport { get; }
        string GetLogFileName();
    }

    public class LogDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public LogDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, LogData> DataProvider { get { return GetDataProvider(); } }
        private ILogDataProviderIOModeAsync DataProviderIOMode { get { return GetDataProvider(); } }

        private IDataProvider<int, LogData> CreateDataProvider() {
            Package package = YetaWF.Modules.Scheduler.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Log", Cacheable: true);
        }

        // API
        // API
        // API

        public async Task<LogData> GetItemAsync(int logEntry) {
            return await DataProvider.GetAsync(logEntry);
        }
        public async Task<bool> AddItemAsync(LogData data) {
            return await DataProviderIOMode.AddItemAsync(data);
        }
        public async Task<DataProviderGetRecords<LogData>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            return await DataProviderIOMode.GetItemsAsync(skip, take, sort, filters);
        }
        public async Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            return await DataProviderIOMode.RemoveItemsAsync(filters);
        }
        public bool CanBrowse {
            get {
                return DataProviderIOMode.CanBrowse;
            }
        }
        public bool CanImportOrExport {
            get {
                return DataProviderIOMode.CanImportOrExport;
            }
        }
        public string GetLogFileName() {
            return DataProviderIOMode.GetLogFileName();
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public new Task<DataProviderExportChunk> ExportChunkAsync(int chunk, SerializableList<SerializableFile> fileList) {
            // we're not exporting any data
            //if (CanImportOrExport)
            //    return DataProvider.ExportChunk(chunk, fileList, out obj);
            //else {
            return Task.FromResult(new DataProviderExportChunk {
                ObjectList = null,
                More = false,
            });
            //}
        }
        public new Task ImportChunkAsync(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            // we're not importing any data
            //if (CanImportOrExport)
            //    DataProvider.ImportChunk(chunk, fileList, obj);
            return Task.CompletedTask;
        }
    }
}
