/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/LoggingDataProvider#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Log;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.LoggingDataProvider.DataProvider.SQL;

public class SQLDataProvider : IExternalDataProvider {
    public void Register() {
        DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.LogRecordDataProvider), typeof(LogRecordDataProvider));
    }
}

public class LogRecordDataProvider : YetaWF.Modules.LoggingDataProvider.DataProvider.LogRecordDataProvider, IInstallableModel, ILogging {

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
    private SQLSimpleObject<int, LogRecord>? _dataProvider;

    private SQLSimpleObject<int, LogRecord> CreateDataProvider() {
        // can't use CurrentPackage as RegisterAllAreas has not yet been called
        Package package = Package.GetPackageFromAssembly(GetType().Assembly);

        Dictionary<string, object> options = new Dictionary<string, object>() {
            { "Package", package },
            { "Dataset", "YetaWF_Logging" },
            { "Logging", false },
            { "NoLanguages", true },
        };
        return new SQLSimpleObject<int, LogRecord>(options);
    }

    // API
    // API
    // API

    public override Task ClearAsync() { return Task.CompletedTask; }
    public override Task FlushAsync() { return Task.CompletedTask; }

    public override void SaveMessage(LogRecord record) {
        YetaWFManager.Syncify(async () => { // Logging is advertised as Sync - use NLog instead
            await DataProvider.AddAsync(record);
        });
    }
    /// <summary>
    /// Defines whether the logging data provider is already logging an event.
    /// </summary>
    bool ILogging.IsProcessing { get { return base.IsProcessing; } set { base.IsProcessing = value; } }

    public override Task<LogRecord?> GetItemAsync(int key) {
        return DataProvider.GetAsync(key);
    }
    public override Task<bool> RemoveItemAsync(int key) {
        return DataProvider.RemoveAsync(key);
    }
    public override async Task<DataProviderGetRecords<LogRecord>> GetItemsAsync(List<DataProviderFilterInfo> filters) {
        return await DataProvider.GetRecordsAsync(0, 0, null, filters);
    }
    public override async Task<DataProviderGetRecords<LogRecord>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) {
        return await DataProvider.GetRecordsAsync(skip, take, sort, filters);
    }
    public override async Task<int> RemoveItemsAsync(List<DataProviderFilterInfo>? filters) {
        return await DataProvider.RemoveRecordsAsync(filters);
    }

    public override string LoggerName { get { return "SQL Table (synchronous I/O)"; } }
    public override bool CanBrowse { get { return true; } }
    public override bool CanImportOrExport { get { return true; } }
    public override bool CanRemove { get { return true; } }
    public override bool CanDownload { get { return false; } }

    // IINSTALLABLEMODEL
    // IINSTALLABLEMODEL
    // IINSTALLABLEMODEL

    public override async Task<bool> IsInstalledAsync() {
        if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider) && YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(NLogProvider.LogRecordDataProvider)) return false;
        return await DataProvider.IsInstalledAsync();
    }
    public async Task<bool> InstallModelAsync(List<string> errorList) {
        if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider) && YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(NLogProvider.LogRecordDataProvider)) return true;
        if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Installing new models is not possible when distributed caching is enabled");
        bool success = await DataProvider.InstallModelAsync(errorList);
        if (success)
            await YetaWF.Core.Log.Logging.SetupLoggingAsync();
        return success;
    }
    public async Task<bool> UninstallModelAsync(List<string> errorList) {
        if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider) && YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(NLogProvider.LogRecordDataProvider)) return true;
        if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Uninstalling models is not possible when distributed caching is enabled");
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
    public Task LocalizeModelAsync(string language, Func<string, bool> isHtml, Func<List<string>, Task<List<string>>> translateStringsAsync, Func<string, Task<string>> translateComplexStringAsync) {
        return Task.CompletedTask;
    }
}
