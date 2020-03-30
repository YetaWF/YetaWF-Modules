/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.GeoLocation;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.Visitors.DataProvider;

namespace YetaWF.Modules.Visitors.Scheduler {

    public class AddVisitorGeoLocation : IScheduling {

        public const string EventAddVisitorGeoLocation = "YetaWF.Visitors: Add Geolocation to Visitor Data";

        public async Task RunItemAsync(SchedulerItemBase evnt) {
            if (evnt.EventName != EventAddVisitorGeoLocation)
                throw new Error(this.__ResStr("eventNameErr", "Unknown scheduler event {0}."), evnt.EventName);
            await AddGeoLocationAsync(evnt.Log);
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

        public async Task AddGeoLocationAsync(List<string> errorList) {
            using (VisitorEntryDataProvider visitorEntryDP = new VisitorEntryDataProvider()) {

                DateTime startTime = DateTime.UtcNow;
                int overall = 0;
                List<DataProviderFilterInfo> filters = null;
                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(VisitorEntry.ContinentCode), Operator = "==", Value = VisitorEntry.Unknown });
                filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(VisitorEntry.AccessDateTime), Operator = "<", Value = startTime });

                GeoLocation geoLocation = new GeoLocation();
                int maxRequest = geoLocation.GetRemainingRequests();
                for (; maxRequest > 0; --maxRequest) {
                    DataProviderGetRecords<VisitorEntry> list = await visitorEntryDP.GetItemsAsync(0, 1, null, filters);
                    if (list.Data.Count == 0) break;
                    VisitorEntry geoData = list.Data.First();
                    GeoLocation.UserInfo userInfo = await geoLocation.GetUserInfoAsync(geoData.IPAddress);
                    if (!string.IsNullOrWhiteSpace(userInfo.ContinentCode)) {
                        geoData.City = userInfo.City;
                        geoData.ContinentCode = userInfo.ContinentCode;
                        geoData.CountryCode = userInfo.CountryCode;
                        geoData.RegionCode = userInfo.RegionCode;
                    } else {
                        geoData.City = "";
                        geoData.ContinentCode = "";
                        geoData.CountryCode = "";
                        geoData.RegionCode = "";
                    }
                    await visitorEntryDP.UpdateSameIPAddressesAsync(geoData);
                    ++overall;
                }
                Logging.AddLog($"Updated {overall} visitor entries");
            }
        }
    }
}