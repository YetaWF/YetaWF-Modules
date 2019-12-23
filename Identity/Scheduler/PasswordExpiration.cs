/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Scheduler {

    public class PasswordExpiration : IScheduling {

        public const string EventPasswordExpiration = "YetaWF.Identity: Update users' password expiration";

        public async Task RunItemAsync(SchedulerItemBase evnt) {
            if (evnt.EventName != EventPasswordExpiration)
                throw new Error(this.__ResStr("eventNameErr", "Unknown scheduler event {0}."), evnt.EventName);
            await UpdatePasswordExpiration(evnt.Log);
        }

        public SchedulerItemBase[] GetItems() {
            return new SchedulerItemBase[]{
                new SchedulerItemBase {
                    Name=this.__ResStr("eventName", "Update users' password expiration"),
                    Description = this.__ResStr("eventDesc", "Updates users' password expiration"),
                    EventName = EventPasswordExpiration,
                    Enabled = true,
                    EnableOnStartup = true,
                    RunOnce = false,
                    Startup = false,
                    SiteSpecific = true,
                    Frequency = new SchedulerFrequency { TimeUnits = SchedulerFrequency.TimeUnitEnum.Days, Value=1 },
                },
            };
        }

        public PasswordExpiration() { }

        public async Task UpdatePasswordExpiration(List<string> errorList) {

            long ticks = WebConfigHelper.GetValue<long>(YetaWF.Modules.Identity.Controllers.AreaRegistration.CurrentPackage.AreaName, "PasswordRenewal", new TimeSpan(0, 0, 0).Ticks); // 0  = indefinitely
            if (ticks <= 0) return;// nothing to do
            TimeSpan interval = new TimeSpan(ticks); // renewal interval
            DateTime oldestDate = DateTime.UtcNow.Subtract(interval);

            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {

                int offset = 0;
                const int CHUNK = 50;

                for (;;) {

                    List<DataProviderFilterInfo> filters = null;
                    filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(UserDefinition.UserStatus), Operator = "==", Value = UserStatusEnum.Approved });
                    filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(UserDefinition.NeedsNewPassword), Operator = "==", Value = false });
                    DataProviderGetRecords<UserDefinition> usersInfo = await userDP.GetItemsAsync(offset, CHUNK, null, filters);
                    if (usersInfo.Data.Count == 0) return;

                    foreach (UserDefinition user in usersInfo.Data) {
                        bool forceUpdate = false;
                        if (user.LastPasswordChangedDate != null && user.LastPasswordChangedDate < oldestDate) {
                            forceUpdate = true;
                        } else if (user.LastPasswordChangedDate == null && user.Created < oldestDate) {
                            forceUpdate = true;
                        }
                        if (forceUpdate) {
                            user.NeedsNewPassword = true;
                            await userDP.UpdateItemAsync(user);
                            Logging.AddLog($"Updated {user.Id} {user.UserName} - requires password");
                        }
                    }
                    offset += CHUNK;
                    System.Threading.Thread.Sleep(5 * 1000); // wait some 5 seconds
                }
            }
        }
    }
}