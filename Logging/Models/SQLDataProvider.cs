/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Log;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
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
            YetaWFManager.Syncify(async () => {
                await DataProvider.AddAsync(record);
            });
        }
        public new Task<LogRecord> GetItemAsync(int key) {
            return DataProvider.GetAsync(key);
        }
        public new Task<bool> RemoveItemAsync(int key) {
            return DataProvider.RemoveAsync(key);
        }
        public new async Task<DataProviderGetRecords<LogRecord>> GetItemsAsync(List<DataProviderFilterInfo> filters) {
            return await DataProvider.GetRecordsAsync(0, 0, null, filters);
        }
        public override async Task<DataProviderGetRecords<LogRecord>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            return await DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
        public new async Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            return await DataProvider.RemoveRecordsAsync(filters);
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

        public override async Task<bool> IsInstalledAsync() {
            if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider)) return false;
            return await DataProvider.IsInstalledAsync();
        }
        public async Task<bool> InstallModelAsync(List<string> errorList) {
            if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider)) return true;
            bool success = await DataProvider.InstallModelAsync(errorList);
            if (success)
                await YetaWF.Core.Log.Logging.SetupLoggingAsync();
            return success;
        }
        public async Task<bool> UninstallModelAsync(List<string> errorList) {
            if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider)) return true;
            YetaWF.Core.Log.Logging.TerminateLogging();
            return await DataProvider.UninstallModelAsync(errorList);
        }
        public Task AddSiteDataAsync() { return Task.CompletedTask; }
        public Task RemoveSiteDataAsync() { return Task.CompletedTask; }
        public Task<DataProviderExportChunk> ExportChunkAsync(int chunk, SerializableList<SerializableFile> fileList) {
            // we're not exporting any data
            return Task.FromResult(new DataProviderExportChunk());
        }
        public Task ImportChunkAsync(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            // we're not importing any data
            return Task.CompletedTask;
        }
    }
}
