/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Log;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Logging.DataProvider.File {

    public class FileDataProvider : IExternalDataProvider {
        public void Register() {
            // registration not used - log provider defined in appsettings
        }
    }

    public class LogRecordDataProvider : YetaWF.Modules.Logging.DataProvider.LogRecordDataProvider, IInstallableModel, ILogging {

        static protected AsyncLock lockObject = new AsyncLock();

        private readonly string LogfileName = "Logfile.txt";
        string LogFile;

        private const int MAXRECORDS = 1000;// cache # of records
        List<string> LogCache { get; set; }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public LogRecordDataProvider() : base() {
            // can't use CurrentPackage as RegisterAllAreas has not yet been called 
            Package package = Package.GetPackageFromAssembly(GetType().Assembly);
            LogFile = Path.Combine(YetaWFManager.DataFolder, package.AreaName, LogfileName);
        }

        // API
        // API
        // API

        public override Task InitAsync() { return Task.CompletedTask; }

        public override async Task ClearAsync() {
            using (lockObject.LockAsync()) {
                try {
                    await FileSystem.FileSystemProvider.DeleteFileAsync(LogFile);
                } catch (Exception) { }
                await FileSystem.FileSystemProvider.CreateDirectoryAsync(Path.GetDirectoryName(LogFile));
                LogCache = new List<string>();
            }
        }

        public override async Task FlushAsync() {
            using (lockObject.LockAsync()) {
                if (LogCache != null)
                    await FileSystem.FileSystemProvider.AppendAllLinesAsync(LogFile, LogCache);
                LogCache = new List<string>();
            }
        }

        public override void SaveMessage(LogRecord record) {

            string text = string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}({7})-{8}: {9},{10},{11},{12} - {13}:{14}",
                DateTime.Now/*Local Time*/, record.Category, record.SessionId, record.SiteIdentity, record.IPAddress, record.RequestedUrl, record.UserName, record.UserId, record.ReferrerUrl,
                    record.ModuleName,
                    record.Class,
                    record.Method,
                    record.Namespace,
                    record.Level, record.Info);
            text = text.Replace("\n", "\r\n");

            using (lockObject.Lock()) {
                YetaWFManager.Syncify(async () => { // logging is sync by default
                    LogCache.Add(text);
                    if (LogCache.Count >= MAXRECORDS)
                        await FlushAsync();
                });
            }
        }

        public override Task<LogRecord> GetItemAsync(int key) {
            throw new NotImplementedException();
        }
        public override Task<bool> RemoveItemAsync(int key) {
            throw new NotImplementedException();
        }

        public override Task<DataProviderGetRecords<LogRecord>> GetItemsAsync(List<DataProviderFilterInfo> filters) {
            throw new NotImplementedException();
        }
        public override Task<DataProviderGetRecords<LogRecord>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            throw new NotImplementedException();
        }
        public override Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            throw new NotImplementedException();
        }

        public override string LoggerName { get { return "Flat File (synchronous I/O)"; } }
        public override bool CanBrowse { get { return false; } }
        public override bool CanImportOrExport { get { return false; } }
        public override bool CanRemove { get { return true; } }
        public override bool CanDownload { get { return true; } }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public override Task<bool> IsInstalledAsync() {
            if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider)) return Task.FromResult(false);
            return Task.FromResult(_isInstalled != null);
        }
        public async Task<bool> InstallModelAsync(List<string> errorList) {
            if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider)) return true;
            if (YetaWF.Core.IO.Caching.MultiInstance) throw new InternalError("Installing new models is not possible when distributed caching is enabled");
            if (_isInstalled == null)
                _isInstalled = await FileSystem.FileSystemProvider.DirectoryExistsAsync(Path.GetDirectoryName(LogFile));
            await YetaWF.Core.Log.Logging.SetupLoggingAsync();
            _isInstalled = true;
            return true;
        }
        private bool? _isInstalled;

        public async Task<bool> UninstallModelAsync(List<string> errorList) {
            if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider)) return true;
            if (YetaWF.Core.IO.Caching.MultiInstance) throw new InternalError("Adding site data is not possible when distributed caching is enabled");
            YetaWF.Core.Log.Logging.TerminateLogging();
            try {
                await FileSystem.FileSystemProvider.DeleteFileAsync(LogFile);
            } catch (Exception) { }
            _isInstalled = false;
            return true;
        }
        public Task AddSiteDataAsync() { return Task.CompletedTask;  }
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

