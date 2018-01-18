/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/KeepAlive#License */

using System;
using System.Net;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.KeepAlive.DataProvider;

namespace YetaWF.Modules.KeepAlive.Scheduler {

    public class KeepAlive : IScheduling {

        public const string EventKeepAlive = "YetaWF.KeepAlive: Keep Site Alive";

        public void RunItem(SchedulerItemBase evnt) {
            if (evnt.EventName != EventKeepAlive)
                throw new Error(this.__ResStr("eventNameErr", "Unknown scheduler event {0}."), evnt.EventName);
            RunKeepAlive(slow: true);
        }

        public SchedulerItemBase[] GetItems() {
            return new SchedulerItemBase[]{
                new SchedulerItemBase {
                    Name=this.__ResStr("eventName", "Keep Site Alive"),
                    Description = this.__ResStr("eventDesc", "Accesses a page at a defined interval to keep the site alive (used with shared hosting)"),
                    EventName = EventKeepAlive,
                    Enabled = true,
                    EnableOnStartup = true,
                    RunOnce = false,
                    Startup = false,
                    SiteSpecific = true,
                    Frequency = new SchedulerFrequency { TimeUnits = SchedulerFrequency.TimeUnitEnum.Minutes, Value=1 },
                },
            };
        }

        public KeepAlive() { }

        private static DateTime LastRun = DateTime.MinValue;

        public void RunKeepAlive(bool slow)
        {
            KeepAliveConfigData config = KeepAliveConfigDataProvider.GetConfig();
            if (slow) {
                if (config.Interval == 0) return;
                if (DateTime.UtcNow.Subtract(new TimeSpan(0, config.Interval, 0)) <= LastRun) return;
            }
            if (string.IsNullOrWhiteSpace(config.Url)) throw new InternalError("The KeepAlive Url has not yet been defined in the Keep Alive Settings");

            // retrieve defined page
            WebClient client = new WebClient();
            Logging.AddLog("KeepAlive retrieving page {0}", config.Url);
            string s = client.DownloadString(config.Url);
            Logging.AddLog("KeepAlive page retrieved");

            LastRun = DateTime.UtcNow;
        }
    }
}