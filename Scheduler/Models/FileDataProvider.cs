/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Scheduler.DataProvider.File {

    public class FileDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.SchedulerDataProvider), typeof(SchedulerDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.LogDataProvider), typeof(LogDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(FileDataProviderBase.ExternalName, typeof(DataProvider.SchedulerConfigDataProvider), typeof(SchedulerConfigDataProvider));
        }
        class SchedulerDataProvider : FileDataProvider<string, SchedulerItemData> {
            public SchedulerDataProvider(Dictionary<string, object> options) : base(options) { }
            public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset); }
        }
        class LogDataProvider : FileDataProvider<int, LogData>, ILogDataProviderIOModeAsync {

            private readonly string LogfileName = "SchedulerLog.txt";
            public string LogFile { get; private set; }// File IO

            public LogDataProvider(Dictionary<string, object> options) : base(options) {
                LogFile = Path.Combine(BaseFolder, LogfileName);
                YetaWFManager.Syncify(async () => // Log is sync by definition
                    await FileSystem.FileSystemProvider.CreateDirectoryAsync(Path.GetDirectoryName(LogFile)!)
                );
            }
            public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset); }

            public async Task<bool> AddItemAsync(LogData data) {
                string text = string.Format("{0}-{1}-{2}-{3}-{4}: {5}\n",
                    data.TimeStamp, data.RunId, data.Level, data.Name, data.SiteIdentity, data.Info);
                text = text.Replace("\n", "\r\n");
                await FileSystem.FileSystemProvider.AppendAllTextAsync(LogFile, text);
                return true;
            }
            public Task<DataProviderGetRecords<LogData>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) {
                throw new NotImplementedException();
            }
            public Task<int> RemoveItemsAsync(List<DataProviderFilterInfo>? filters) {
                throw new NotImplementedException();
            }
            public bool CanBrowse {
                get { return false; }
            }
            public bool CanImportOrExport {
                get { return false; }
            }
            public string GetLogFileName() {
                return LogFile;
            }
        }
        class SchedulerConfigDataProvider : FileDataProvider<int, SchedulerConfigData> {
            public SchedulerConfigDataProvider(Dictionary<string, object> options) : base(options) { }
            public override string GetBaseFolder() { return Path.Combine(YetaWFManager.DataFolder, Dataset); }
        }
    }
}
