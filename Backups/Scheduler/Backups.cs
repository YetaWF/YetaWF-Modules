/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Backups#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Support;
using YetaWF.Modules.Backups.DataProvider;

namespace YetaWF.Modules.Backups.Scheduler {

    public class Backups : IScheduling {

        public const string EventCreateBackup = "YetaWF.Backups: Create Daily Backup";
        public const string EventRemoveOldBackups = "YetaWF.Backups: Remove Old Backups";

        public async Task RunItemAsync(SchedulerItemBase evnt) {
            if (evnt.EventName == EventCreateBackup) {
                SiteBackup siteBackup = new SiteBackup();
                List<string> errorList = new List<string>();
                ConfigData config = await ConfigDataProvider.GetConfigAsync();
                await siteBackup.CreateAsync(errorList, DataOnly: config.DataOnly);
            } else if (evnt.EventName == EventRemoveOldBackups) {
                SiteBackup siteBackup = new SiteBackup();
                List<string> errorList = new List<string>();
                ConfigData config = await ConfigDataProvider.GetConfigAsync();
                await siteBackup.RemoveOldBackupsAsync(errorList, config.Days);
            } else
                throw new Error(this.__ResStr("eventNameErr", "Unknown scheduler event {0}"), evnt.EventName);
        }

        public SchedulerItemBase[] GetItems() {
            return new SchedulerItemBase[] {
                new SchedulerItemBase {
                    Name=this.__ResStr("eventNameDaily", "Create Daily Site Backup"),
                    Description = this.__ResStr("eventDescDaily", "Creates a site backup and stores it in the site's Backup folder"),
                    EventName = EventCreateBackup,
                    Enabled = true,
                    EnableOnStartup = true,
                    RunOnce=false,
                    Startup = false,
                    SiteSpecific = true,
                    Frequency = new SchedulerFrequency { TimeUnits = SchedulerFrequency.TimeUnitEnum.Days, Value=1 },
                },
                new SchedulerItemBase {
                    Name=this.__ResStr("eventNameRemove", "Remove Old Site Backup"),
                    Description = this.__ResStr("eventDescRemove", "Removes site backups that are too old (defined using backup settings)"),
                    EventName = EventRemoveOldBackups,
                    Enabled = true,
                    EnableOnStartup = true,
                    RunOnce=false,
                    Startup = false,
                    SiteSpecific = true,
                    Frequency = new SchedulerFrequency { TimeUnits = SchedulerFrequency.TimeUnitEnum.Days, Value=1 },
                },
            };
        }
    }
}