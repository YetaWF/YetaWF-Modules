/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.PostgreSQL;

namespace YetaWF.Modules.Scheduler.DataProvider.PostgreSQL {

    public class PostgreSQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SchedulerDataProvider), typeof(SchedulerDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.LogDataProvider), typeof(LogDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SchedulerConfigDataProvider), typeof(SchedulerConfigDataProvider));
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
        class SchedulerConfigDataProvider : SQLSimpleObject<int, SchedulerConfigData> {
            public SchedulerConfigDataProvider(Dictionary<string, object> options) : base(options) { }
        }
    }
}
