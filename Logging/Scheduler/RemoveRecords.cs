/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

using System;
using System.Collections.Generic;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.Logging.DataProvider;

namespace YetaWF.Modules.Logging.Scheduler {

    public class RemoveOldLogData : IScheduling {

        public const string EventRemoveOldLogData = "YetaWF.Logging: Remove Old Log Data";

        public void RunItem(SchedulerItemBase evnt) {
            if (evnt.EventName != EventRemoveOldLogData)
                throw new Error(this.__ResStr("eventNameErr", "Unknown scheduler event {0}."), evnt.EventName);
            Remove(evnt.Log);
        }

        public SchedulerItemBase[] GetItems() {
            return new SchedulerItemBase[]{
                new SchedulerItemBase {
                    Name=this.__ResStr("eventName", "Remove Old Log Data"),
                    Description = this.__ResStr("eventDesc", "Removes old log data"),
                    EventName = EventRemoveOldLogData,
                    Enabled = true,
                    EnableOnStartup = true,
                    RunOnce = false,
                    Startup = false,
                    SiteSpecific = true,
                    Frequency = new SchedulerFrequency { TimeUnits = SchedulerFrequency.TimeUnitEnum.Weeks, Value=1 },
                },
            };
        }

        public RemoveOldLogData() { }

        public void Remove(List<string> errorList) {
            DateTime oldest = DateTime.UtcNow.AddMonths(-1);
            using (LogRecordDataProvider logDP = LogRecordDataProvider.GetLogRecordDataProvider()) {
                List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "TimeStamp", Operator = "<", Value = oldest });
                int removed = logDP.RemoveItems(filters);
                errorList.Add(string.Format("{0} records removed from log data", removed));
            }
        }
    }
}