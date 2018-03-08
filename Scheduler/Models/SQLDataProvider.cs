/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL2;

namespace YetaWF.Modules.Scheduler.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SchedulerDataProvider), typeof(SchedulerDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.LogDataProvider), typeof(LogDataProvider));
        }
        class SchedulerDataProvider : SQLSimpleObject<string, SchedulerItemData> {
            public SchedulerDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class LogDataProvider : SQLSimpleObject<int, LogData>, ILogDataProviderIOModeAsync {
            public LogDataProvider(Dictionary<string, object> options) : base(options) { }

            public async Task<bool> AddItemAsync(LogData data) {
                return await AddAsync(data);
            }
            public async Task<DataProviderGetRecords<LogData>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
                return await GetRecordsAsync(skip, take, sort, filters);
            }
            public async Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
                return await RemoveRecordsAsync(filters);
            }
            public bool CanBrowse {
                get { return true; }
            }
            public bool CanImportOrExport {
                get { return true; }
            }
            public string GetLogFileName() {
                return null;
            }
        }
    }
}
