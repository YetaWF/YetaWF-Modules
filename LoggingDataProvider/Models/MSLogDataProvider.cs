/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/LoggingDataProvider#License */

#if MVC6

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Log;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.LoggingDataProvider.DataProvider.MSLog {

    public class LogRecordDataProvider : YetaWF.Modules.LoggingDataProvider.DataProvider.LogRecordDataProvider, IInstallableModel, ILogging {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        private ILogger Logger = null;

        public LogRecordDataProvider() : base() {
            Logger = (ILogger)YetaWFManager.ServiceProvider.GetService(typeof(ILogger<LogRecordDataProvider>));
            //$$$YetaWFLoggerProvider.IgnoredCategory = typeof(LogRecordDataProvider).FullName;
        }

        // API
        // API
        // API

        public override void SaveMessage(LogRecord record) {

            string text = string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}({7})-{8}: {9},{10},{11},{12} - {13}:{14}",
                DateTime.Now/*Local Time*/, record.Category, record.SessionId, record.SiteIdentity, record.IPAddress, record.RequestedUrl, record.UserName, record.UserId, record.ReferrerUrl,
                    record.ModuleName,
                    record.Class,
                    record.Method,
                    record.Namespace,
                    record.Level, record.Info);
            if (Logger != null) {
                Microsoft.Extensions.Logging.LogLevel level;
                switch (record.Level) {
                    case Core.Log.Logging.LevelEnum.Trace:
                        level = Microsoft.Extensions.Logging.LogLevel.Trace; break;
                    default:
                    case Core.Log.Logging.LevelEnum.Info:
                        level = Microsoft.Extensions.Logging.LogLevel.Information; break;
                    case Core.Log.Logging.LevelEnum.Warning:
                        level = Microsoft.Extensions.Logging.LogLevel.Warning; break;
                    case Core.Log.Logging.LevelEnum.Error:
                        level = Microsoft.Extensions.Logging.LogLevel.Error; break;
                }
                Logger.Log(level, text);
            }
        }

        public override string LoggerName { get { return "Console I/O"; } }
        public override bool CanBrowse { get { return false; } }
        public override bool CanImportOrExport { get { return false; } }
        public override bool CanRemove { get { return false; } }
        public override bool CanDownload { get { return false; } }
        /// <summary>
        /// Defines whether the logging data provider is already logging an event.
        /// </summary>
        bool ILogging.IsProcessing { get { return base.IsProcessing; } set { base.IsProcessing = value; } }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public override Task<bool> IsInstalledAsync() {
            if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider)) return Task.FromResult(false);
            return Task.FromResult(true);
        }
        public async Task<bool> InstallModelAsync(List<string> errorList) {
            if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider)) return true;
            await YetaWF.Core.Log.Logging.SetupLoggingAsync();
            return true;
        }

        public Task<bool> UninstallModelAsync(List<string> errorList) {
            if (YetaWF.Core.Log.Logging.DefinedLoggerType != typeof(LogRecordDataProvider)) return Task.FromResult(true);
            YetaWF.Core.Log.Logging.TerminateLogging();
            return Task.FromResult(true);
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
        public Task LocalizeModelAsync(string language, Func<string, bool> isHtml, Func<List<string>, Task<List<string>>> translateStringsAsync, Func<string, Task<string>> translateComplexStringAsync) {
            return Task.CompletedTask;
        }
    }
}

#endif