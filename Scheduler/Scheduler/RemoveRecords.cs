/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.Scheduler.DataProvider;

namespace YetaWF.Modules.Scheduler.Scheduler {

    public class RemoveOldLogData : IScheduling {

        public const string EventRemoveOldLogData = "YetaWF.Scheduler: Remove Old Log Data";

        public async Task RunItemAsync(SchedulerItemBase evnt) {
            if (evnt.EventName != EventRemoveOldLogData)
                throw new Error(this.__ResStr("eventNameErr", "Unknown scheduler event {0}."), evnt.EventName);
            await RemoveAsync(evnt.Log);
        }

        public SchedulerItemBase[] GetItems() {
            return new SchedulerItemBase[]{
                new SchedulerItemBase {
                    Name=this.__ResStr("eventName", "Remove Old Scheduler Log Data"),
                    Description = this.__ResStr("eventDesc", "Removes old scheduler log data"),
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

        public async Task RemoveAsync(List<string> errorList) {
            SchedulerConfigData config = await SchedulerConfigDataProvider.GetConfigAsync();
            DateTime oldest = DateTime.UtcNow.AddDays(-config.Days);
            using (LogDataProvider logDP = new LogDataProvider()) {
                List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = nameof(LogData.TimeStamp), Operator = "<", Value = oldest });
                int removed = await logDP.RemoveItemsAsync(filters);
                errorList.Add(string.Format("{0} records removed from scheduler log data", removed));
            }
        }
    }
}