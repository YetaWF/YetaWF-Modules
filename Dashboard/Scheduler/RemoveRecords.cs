/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.Dashboard.DataProvider;

namespace YetaWF.Modules.Dashboard.Scheduler {

    public class RemoveOldAuditData : IScheduling {

        public const string EventRemoveAuditData = "YetaWF.Dashboard: Remove Old Audit Data";

        public async Task RunItemAsync(SchedulerItemBase evnt) {
            if (evnt.EventName != EventRemoveAuditData)
                throw new Error(this.__ResStr("eventNameErr", "Unknown scheduler event {0}."), evnt.EventName);
            await RemoveAsync(evnt.Log);
        }

        public SchedulerItemBase[] GetItems() {
            return new SchedulerItemBase[]{
                new SchedulerItemBase {
                    Name=this.__ResStr("eventName", "Remove Old Audit Data"),
                    Description = this.__ResStr("eventDesc", "Removes old audit data"),
                    EventName = EventRemoveAuditData,
                    Enabled = false,
                    EnableOnStartup = false,
                    RunOnce = false,
                    Startup = false,
                    SiteSpecific = true,
                    Frequency = new SchedulerFrequency { TimeUnits = SchedulerFrequency.TimeUnitEnum.Days, Value=1 },
                },
            };
        }

        public RemoveOldAuditData() { }

        public async Task RemoveAsync(List<string> errorList) {

            AuditConfigData config = await AuditConfigDataProvider.GetConfigAsync();
            DateTime oldest = DateTime.UtcNow.AddDays(-config.Days);
            using (AuditInfoDataProvider auditDP = new AuditInfoDataProvider()) {
                List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = nameof(AuditInfo.Created), Operator = "<", Value = oldest });
                int removed = await auditDP.RemoveItemsAsync(filters);
                errorList.Add(string.Format("{0} records removed from audit data", removed));
            }
        }
    }
}
