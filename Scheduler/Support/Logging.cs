/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Log;
using YetaWF.Core.Support;
using YetaWF.Modules.Scheduler.DataProvider;

namespace YetaWF.Modules.Scheduler.Support {

    /// <summary>
    /// Logger for scheduling activity.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",Justification = "Explicit Init/Shutdown methods must be called")]
    public class SchedulerLogging : ILogging {

        internal SchedulerLogging() { }

        internal void Init() {
            logDP = new LogDataProvider();
        }
        internal void Shutdown() {
            if (logDP != null)
                logDP.Dispose();
        }
        private LogDataProvider logDP = null!;

        internal void LimitTo(YetaWFManager manager) {
            LimitToManager = manager;
        }
        private YetaWFManager? LimitToManager;

        private long CurrentId { get; set; }
        private string? CurrentName { get; set; }
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
        public Task InitAsync() { return Task.CompletedTask; }
        public Task ClearAsync() { return Task.CompletedTask; }
        public Task FlushAsync() { return Task.CompletedTask; }
        public Task<bool> IsInstalledAsync() { return Task.FromResult(true); }

        public void WriteToLogFile(string category, Logging.LevelEnum level, int relStack, string text) {
            if (!YetaWFManager.HaveManager) return;
            if (YetaWFManager.Manager != LimitToManager) return; // this log entry is for another thread
            YetaWFManager.Syncify(async () => { // Logging is sync (because most log providers like NLog use async tasks), not worth the trouble to make this async. Scheduler is separate thread anyway.
                await logDP.AddItemAsync(new DataProvider.LogData {
                    TimeStamp = DateTime.UtcNow,
                    RunId = CurrentId,
                    Name = CurrentName,
                    Level = level,
                    SiteIdentity = CurrentSiteId,
                    Info = text,
                });
            });
        }
        /// <summary>
        /// Defines whether the logging data provider is already logging an event.
        /// </summary>
        bool ILogging.IsProcessing { get; set; }
    }
}

