/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.DataProvider.SQL;

namespace YetaWF.Modules.Scheduler.DataProvider.SQL {

    public class SQLDataProvider : IExternalDataProvider {

        public void Register() {
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.SchedulerDataProvider), typeof(SchedulerDataProvider));
            DataProviderImpl.RegisterExternalDataProvider(SQLBase.ExternalName, typeof(DataProvider.LogDataProvider), typeof(LogDataProvider));
        }
        class SchedulerDataProvider : SQLSimpleObject<string, SchedulerItemData> {
            public SchedulerDataProvider(Dictionary<string, object> options) : base(options) { }
        }
        class LogDataProvider : SQLSimpleObject<int, LogData>, ILogDataProviderIOMode {
            public LogDataProvider(Dictionary<string, object> options) : base(options) { }

            public bool AddItem(LogData data) {
                return Add(data);
            }
            public List<LogData> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
                return GetRecords(skip, take, sort, filters, out total);
            }
            public int RemoveItems(List<DataProviderFilterInfo> filters) {
                return RemoveRecords(filters);
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
