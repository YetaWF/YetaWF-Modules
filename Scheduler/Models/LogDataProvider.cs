/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
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

    public interface ILogDataProviderIOMode {
        bool AddItem(LogData data);
        List<LogData> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total);
        int RemoveItems(List<DataProviderFilterInfo> filters);
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
        private ILogDataProviderIOMode DataProviderIOMode { get { return GetDataProvider(); } }

        private IDataProvider<int, LogData> CreateDataProvider() {
            Package package = YetaWF.Modules.Scheduler.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Log", Cacheable: true);
        }

        // API
        // API
        // API

        public LogData GetItem(int logEntry) {
            return DataProvider.Get(logEntry);
        }
        public bool AddItem(LogData data) {
            return DataProviderIOMode.AddItem(data);
        }
        public List<LogData> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            return DataProviderIOMode.GetItems(skip, take, sort, filters, out total);
        }
        public int RemoveItems(List<DataProviderFilterInfo> filters) {
            return DataProviderIOMode.RemoveItems(filters);
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

        public new bool ExportChunk(int chunk, SerializableList<SerializableFile> fileList, out object obj) {
            // we're not exporting any data
            //if (CanImportOrExport)
            //    return DataProvider.ExportChunk(chunk, fileList, out obj);
            //else {
            obj = null;
            return false;
            //}
        }
        public new void ImportChunk(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            // we're not importing any data
            //if (CanImportOrExport)
            //    DataProvider.ImportChunk(chunk, fileList, obj);
        }
    }
}
