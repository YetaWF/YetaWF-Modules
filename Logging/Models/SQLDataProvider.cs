/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Log;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.Logging.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {
        public void Register() {
            // registration not used - log provider defined in appsettings
        }
    }

    public class LogRecordDataProvider : YetaWF.Modules.Logging.DataProvider.LogRecordDataProvider, IInstallableModel, ILogging {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public LogRecordDataProvider() : base() { }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (_dataProvider != null)
                    _dataProvider.Dispose();
                _dataProvider = null;
            }
            base.Dispose(disposing);
        }

        private IDataProvider<int, LogRecord> DataProvider {
            get {
                if (_dataProvider == null)
                    _dataProvider = CreateDataProvider();
                return (IDataProvider<int, LogRecord>) _dataProvider;
            }
        }
        private SQLSimpleObject<int, LogRecord> _dataProvider;

        private SQLSimpleObject<int, LogRecord> CreateDataProvider() {
            // can't use CurrentPackage as RegisterAllAreas has not yet been called 
            Package package = Package.GetPackageFromAssembly(GetType().Assembly);

            Dictionary<string, object> options = new Dictionary<string, object>() {
                { "Package", package },
                { "Dataset", package.AreaName },
                { "Logging", false },
                { "NoLanguages", true },
            };
            return new SQLSimpleObject<int, LogRecord>(options);
        }

        // API
        // API
        // API

        public override void Clear() { }
        public override void Flush() { }

        public override void SaveMessage(LogRecord record) {
            DataProvider.Add(record);
        }
        public override LogRecord GetItem(int key) {
            return DataProvider.Get(key);
        }
        public override bool RemoveItem(int key) {
            return DataProvider.Remove(key);
        }
        public override List<LogRecord> GetItems(List<DataProviderFilterInfo> filters) {
            int total;
            return DataProvider.GetRecords(0, 0, null, filters, out total);
        }
        public override List<LogRecord> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            return DataProvider.GetRecords(skip, take, sort, filters, out total);
        }
        public override int RemoveItems(List<DataProviderFilterInfo> filters) {
            return DataProvider.RemoveRecords(filters);
        }
        public override bool CanBrowse {
            get {
                return true;
            }
        }
        public override bool CanImportOrExport {
            get {
                return true;
            }
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public override bool IsInstalled() {
            return DataProvider.IsInstalled();
        }
        public bool InstallModel(List<string> errorList) {
            bool success = DataProvider.InstallModel(errorList);
            if (success)
                YetaWF.Core.Log.Logging.SetupLogging();
            return success;
        }
        public bool UninstallModel(List<string> errorList) {
            YetaWF.Core.Log.Logging.TerminateLogging();
            return DataProvider.UninstallModel(errorList);
        }
        public void AddSiteData() { }
        public void RemoveSiteData() { }
        public bool ExportChunk(int chunk, SerializableList<SerializableFile> fileList, out object obj) {
            // we're not exporting any data
            obj = null;
            return false;
        }
        public void ImportChunk(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            // we're not importing any data
        }
    }
}
