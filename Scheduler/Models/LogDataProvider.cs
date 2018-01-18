/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Scheduler.DataProvider
{
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

    public class LogDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        static object lockObject = new object();

        private readonly string LogfileName = "SchedulerLog.txt";
        public string LogFile { get; private set; }// File IO

        public LogDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, LogData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, LogData> CreateDataProvider() {
            Package package = YetaWF.Modules.Scheduler.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Log",
                () => { // File
                    LogFile = Path.Combine(YetaWFManager.DataFolder, Dataset, LogfileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(LogFile));
                    return new FileDataProvider<int, LogData>(
                        Path.Combine(YetaWFManager.DataFolder, Dataset));
                },
                (dbo, conn) => {  // SQL
                    return new YetaWF.DataProvider.SQLIdentityObjectDataProvider<int, object, int, LogData>(Dataset, dbo, conn,
                        Cacheable: true);
                },
                () => { // External
                    return MakeExternalDataProvider(new { Package = Package, Dataset = Dataset, Cacheable = true });
                }
            );
        }

        // API
        // API
        // API

        public LogData GetItem(int logEntry) {
            return DataProvider.Get(logEntry);
        }
        public bool AddItem(LogData data) {
            switch (IOMode) {
                default:
                    throw new InternalError("IOMode undetermined - this means we don't have a valid data provider");
                case WebConfigHelper.IOModeEnum.File:
                    string text = string.Format("{0}-{1}-{2}-{3}-{4}: {5}\n",
                        data.TimeStamp, data.RunId, data.Level, data.Name, data.SiteIdentity, data.Info);
                        text = text.Replace("\n", "\r\n");
                    lock (lockObject) {
                        File.AppendAllText(LogFile, text);
                    }
                    return true;
                case WebConfigHelper.IOModeEnum.Sql:
                    return DataProvider.Add(data);
            }
        }
        public List<LogData> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            if (IOMode == WebConfigHelper.IOModeEnum.File) throw new InternalError("Not supported for File I/O");
            return DataProvider.GetRecords(skip, take, sort, filters, out total);
        }
        public int RemoveItems(List<DataProviderFilterInfo> filters) {
            if (IOMode == WebConfigHelper.IOModeEnum.File) throw new InternalError("Not supported for File I/O");
            return DataProvider.RemoveRecords(filters);
        }
        public bool CanBrowse {
            get {
                return CanImportOrExport;
            }
        }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public bool CanImportOrExport {
            get {
                IDataProvider<int, LogData> providef = DataProvider;// to evaluate IOMode
                if (IOMode == WebConfigHelper.IOModeEnum.Determine)
                    throw new InternalError("unexpected IOMode");
                return base.IOMode == WebConfigHelper.IOModeEnum.Sql;
            }
        }
        public string GetLogFileName() {
            IDataProvider<int, LogData> providef = DataProvider;// to evaluate IOMode
            if (IOMode != WebConfigHelper.IOModeEnum.File)
                throw new InternalError("Not supported for current I/O mode");
            return LogFile;
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
