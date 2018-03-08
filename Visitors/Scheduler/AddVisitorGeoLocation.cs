/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.Visitors.DataProvider;

namespace YetaWF.Modules.Visitors.Scheduler {

    public class AddVisitorGeoLocation : IScheduling {

        public const string EventAddVisitorGeoLocation = "YetaWF.Visitors: Add Geolocation to Visitor Data";

        public Task RunItemAsync(SchedulerItemBase evnt) {
            if (evnt.EventName != EventAddVisitorGeoLocation)
                throw new Error(this.__ResStr("eventNameErr", "Unknown scheduler event {0}."), evnt.EventName);
            AddGeoLocation(evnt.Log);
            return Task.CompletedTask;
        }

        public SchedulerItemBase[] GetItems() {
            return new SchedulerItemBase[]{
                new SchedulerItemBase {
                    Name=this.__ResStr("eventName", "Add Geolocation to Visitor Data"),
                    Description = this.__ResStr("eventDesc", "Adds Geolocation to Visitor Data"),
                    EventName = EventAddVisitorGeoLocation,
                    Enabled = true,
                    EnableOnStartup = true,
                    RunOnce = false,
                    Startup = false,
                    SiteSpecific = true,
                    Frequency = new SchedulerFrequency { TimeUnits = SchedulerFrequency.TimeUnitEnum.Minutes, Value=30 },
                },
            };
        }

        public AddVisitorGeoLocation() { }

        public void AddGeoLocation(List<string> errorList) {
            using (VisitorEntryDataProvider visitorEntryDP = new VisitorEntryDataProvider()) {

                DateTime startTime = DateTime.UtcNow;
                int overall = 0;
                List<DataProviderFilterInfo> filters = null;
                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "ContinentCode", Operator = "==", Value = VisitorEntry.Unknown });
                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "AccessDateTime", Operator = "<", Value = startTime });
                for ( ; ; ) {
                    int total;
                    List<VisitorEntry> list = visitorEntryDP.GetItems(0, 20, null, filters, out total);
                    if (list.Count == 0) break;
                    foreach (VisitorEntry ve in list) {
                        GeoLocation geoLocation = new GeoLocation();
                        GeoLocation.UserInfo userInfo = geoLocation.GetUserInfo(ve.IPAddress);
                        if (!string.IsNullOrWhiteSpace(userInfo.ContinentCode)) {
                            ve.City = userInfo.City;
                            ve.ContinentCode = userInfo.ContinentCode;
                            ve.CountryCode = userInfo.CountryCode;
                            ve.RegionCode = userInfo.RegionCode;
                        } else {
                            ve.City = "";
                            ve.ContinentCode = "";
                            ve.CountryCode = "";
                            ve.RegionCode = "";
                        }
                        UpdateStatusEnum status = visitorEntryDP.UpdateItem(ve);
                        if (status == UpdateStatusEnum.OK || status == UpdateStatusEnum.RecordDeleted) {
                            // all is well
                        } else {
                            throw new InternalError($"Failed to update {nameof(VisitorEntry)} in {nameof(AddGeoLocation)}");
                        }
                    }
                    overall += list.Count;
                }
                Logging.AddLog($"Updated {overall} visitor entries");
            }
        }
    }
}