/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Log;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider.SQL;
using YetaWF.Modules.Logging.Controllers;

namespace YetaWF.Modules.Logging.DataProvider.NLogProvider {

    public class NLogDataProvider : IExternalDataProvider {

        public static NLog.Logger Logger;

        public const string NLogSettingsFile = "NLog.config";
        private const string NLogMessageFormat = "NLogMessageFormat";
        private const string NLogMessageEvent = "NLogMessageEvent";

        public static string MessageFormat { get; private set; }
        public static bool MessageEvent { get; private set; }

        public void Register() {

            // Get config file
            string rootFolder;
#if MVC6
            rootFolder = YetaWFManager.RootFolderWebProject;
#else
            rootFolder = YetaWFManager.RootFolder;
#endif
            string configFile = Path.Combine(rootFolder, Globals.DataFolder, NLogSettingsFile);
            bool useNlog = YetaWFManager.Syncify<bool>(async () => { // registration is sync by definition (this runs once during startup only)
                return await FileSystem.FileSystemProvider.FileExistsAsync(configFile);
            });
            if (!useNlog)
                return;

            // register custom target (write to Sql table)
            Target.Register<YetaWFDBTarget>("YetaWFDB");

            LogManager.Configuration = new XmlLoggingConfiguration(configFile);

            MessageFormat = WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, NLogMessageFormat);
            MessageFormat = MessageFormat?.ToLower();
            MessageEvent = WebConfigHelper.GetValue<bool>(AreaRegistration.CurrentPackage.AreaName, NLogMessageEvent);

            // get logger
            Logger = NLog.LogManager.GetLogger("YetaWF");
        }
    }

    // NLog Custom Target writing to SQL table
    [Target("YetaWFDB")]
    public sealed class YetaWFDBTarget : TargetWithLayout {

        public YetaWFDBTarget() { }

        protected override void Write(LogEventInfo logEvent) {
            object o;
            if (!logEvent.Properties.TryGetValue("record", out o))
                throw new InternalError("When using the YetaWFDB target for NLog, the NLogMessageEvent property (appsettings.json) must be set to true");
            LogRecord data = (LogRecord)o;
            DataProvider.AddAsync(data).Wait(); // sync is OK as we're saving on a separate thread with async NLog
        }
        private IDataProvider<int, LogRecord> DataProvider {
            get {
                if (_dataProvider == null)
                    _dataProvider = CreateDataProvider();
                return (IDataProvider<int, LogRecord>)_dataProvider;
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

        public override Task ClearAsync() { return Task.CompletedTask; }
        public override Task FlushAsync() {
            NLog.LogManager.Flush();
            return Task.CompletedTask;
        }

        public override void SaveMessage(LogRecord record) {

            if (NLogDataProvider.Logger == null) return;// can be null before external data providers are registered

            NLog.LogLevel level = NLog.LogLevel.Off;
            switch (record.Level) {
                case Core.Log.Logging.LevelEnum.Trace: level = NLog.LogLevel.Trace; break;
                case Core.Log.Logging.LevelEnum.Info: level = NLog.LogLevel.Info; break;
                case Core.Log.Logging.LevelEnum.Warning: level = NLog.LogLevel.Warn; break;
                case Core.Log.Logging.LevelEnum.Error: level = NLog.LogLevel.Error; break;
            }
            if (level == NLog.LogLevel.Off) return;

            string message;
            if (NLogDataProvider.MessageFormat == "json") {
                message = Utility.JsonSerialize(record);
            } else {
                message = $"{Enc(record.Info)};{Enc(record.Category)};{Enc(record.RequestedUrl)};{record.ReferrerUrl};{record.IPAddress};{Enc(record.UserName)};{record.UserId};{record.SessionId}" +
                                    $";{record.ModuleName};{record.Class};{record.Method}" +
                                    $";{record.Namespace};{record.SiteIdentity}";
            }
            if (NLogDataProvider.MessageEvent) {
                NLog.LogEventInfo ev = new NLog.LogEventInfo(level, "YetaWF", message);
                ev.Properties["record"] = record;
                NLogDataProvider.Logger.Log(ev);
            } else {
                NLogDataProvider.Logger.Log(level, message);
            }
        }
        private object Enc(string s) {
            return s.Replace(";", ";;");
        }
        public override Task<LogRecord> GetItemAsync(int key) {
            return DataProvider.GetAsync(key);
        }
        public override Task<bool> RemoveItemAsync(int key) {
            return DataProvider.RemoveAsync(key);
        }
        public override async Task<DataProviderGetRecords<LogRecord>> GetItemsAsync(List<DataProviderFilterInfo> filters) {
            return await DataProvider.GetRecordsAsync(0, 0, null, filters);
        }
        public override async Task<DataProviderGetRecords<LogRecord>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            return await DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
        public override async Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            return await DataProvider.RemoveRecordsAsync(filters);
        }
        public Task LocalizeModelAsync(string language, Func<string, bool> isHtml, Func<List<string>, Task<List<string>>> translateStringsAsync, Func<string, Task<string>> translateComplexStringAsync) {
            return Task.CompletedTask;
        }

        public override string LoggerName { get { return $"NLog (synchronous and asynchronous I/O configurable using {NLogDataProvider.NLogSettingsFile})"; } }
        public override bool CanBrowse { get { return true; } }
        public override bool CanImportOrExport { get { return false; } }
        public override bool CanRemove { get { return true; } }
        public override bool CanDownload { get { return false; } }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public override async Task<bool> IsInstalledAsync() {
            if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider)) return false;
            return await DataProvider.IsInstalledAsync();
        }
        public async Task<bool> InstallModelAsync(List<string> errorList) {
            if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider)) return true;
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Installing new models is not possible when distributed caching is enabled");
            bool success = await DataProvider.InstallModelAsync(errorList);
            if (success)
                await YetaWF.Core.Log.Logging.SetupLoggingAsync();
            return success;
        }
        public async Task<bool> UninstallModelAsync(List<string> errorList) {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Uninstalling models is not possible when distributed caching is enabled");
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
