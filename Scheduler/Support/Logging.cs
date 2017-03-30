/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using YetaWF.Core.Log;
using YetaWF.Core.Support;
using YetaWF.Modules.Scheduler.DataProvider;

namespace YetaWF.Modules.Scheduler.Support {

    /// <summary>
    /// Logger for scheduling activity.
    /// </summary>
    public class SchedulerLogging : ILogging {

        internal SchedulerLogging() { }

        internal void Init() {
            logDP = new LogDataProvider();
        }
        internal void Shutdown() {
            if (logDP != null)
                logDP.Dispose();
        }
        private LogDataProvider logDP;

        internal void LimitTo(YetaWFManager manager) {
            LimitToManager = manager;
        }
        private YetaWFManager LimitToManager;

        private long CurrentId { get; set; }
        private string CurrentName { get; set; }
        private int CurrentSiteId { get; set; }

        internal void SetCurrent(long currentId, int currentSiteId, string currentName) {
            CurrentId = currentId;
            CurrentSiteId = currentSiteId;
            CurrentName = currentName;
        }
        internal void SetCurrent() {
            CurrentId = 0;
            CurrentName = null;
        }

        // ILOGGING
        // ILOGGING
        // ILOGGING

        public Logging.LevelEnum GetLevel() {
#if DEBUG
            return Logging.LevelEnum.Trace; // Full status information in debug only
#else
            return Logging.LevelEnum.Info;
#endif
        }

        public void Clear() { }
        public void Flush() { }
        public bool IsInstalled() { return true; }

        public void WriteToLogFile(Logging.LevelEnum level, int relStack, string text) {
            if (YetaWFManager.Manager != LimitToManager) return; // this log entry is for another thread
            logDP.AddItem(new DataProvider.LogData {
                TimeStamp = DateTime.UtcNow,
                RunId = CurrentId,
                Name = CurrentName,
                Level = level,
                SiteIdentity = CurrentSiteId,
                Info = text,
            });
        }
    }
}

