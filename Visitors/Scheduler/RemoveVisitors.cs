/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.Visitors.DataProvider;

namespace YetaWF.Modules.Visitors.Scheduler {

    public class RemoveOldVisitorData : IScheduling {

       public const string EventRemoveOldVisitorData = "YetaWF.Visitors: Remove Old Visitor Data";

        public async Task RunItemAsync(SchedulerItemBase evnt) {
            if (evnt.EventName != EventRemoveOldVisitorData)
                throw new Error(this.__ResStr("eventNameErr", "Unknown scheduler event {0}."), evnt.EventName);
            await RemoveAsync(evnt.Log);
        }

        public SchedulerItemBase[] GetItems() {
            return new SchedulerItemBase[]{
                new SchedulerItemBase {
                    Name=this.__ResStr("eventName", "Remove Old Visitor Data"),
                    Description = this.__ResStr("eventDesc", "Removes old visitor data"),
                    EventName = EventRemoveOldVisitorData,
                    Enabled = true,
                    EnableOnStartup = true,
                    RunOnce = false,
                    Startup = false,
                    SiteSpecific = true,
                    Frequency = new SchedulerFrequency { TimeUnits = SchedulerFrequency.TimeUnitEnum.Weeks, Value=1 },
                },
            };
        }

        public RemoveOldVisitorData() { }

        public async Task RemoveAsync(List<string> errorList) {
            VisitorsConfigData config = await VisitorsConfigDataProvider.GetConfigAsync();
            DateTime oldest = DateTime.UtcNow.AddDays(-config.Days);
            using (VisitorEntryDataProvider visitorEntryDP = new VisitorEntryDataProvider()) {
                List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = nameof(VisitorEntry.AccessDateTime), Operator = "<", Value = oldest });
                int removed = await visitorEntryDP.RemoveItemsAsync(filters);
                errorList.Add(string.Format("{0} records removed from visitor data", removed));
            }
        }
    }
}