/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Packages;
using YetaWF.Core.Scheduler;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
using YetaWF.Modules.Scheduler.DataProvider;

namespace YetaWF.Modules.Scheduler.Support {

    public partial class Scheduler : IInitializeApplicationStartup {

        internal static Scheduler Instance = null!;

        public Task InitializeApplicationStartupAsync() {
            Instance = this;
            SchedulerSupport.InstallAsync = InstallItemsAsync;
            SchedulerSupport.UninstallAsync = UninstallItemsAsync;
            SchedulerSupport.RunItemAsync = RunItemAsync;
            SchedulerSupport.SetItemNextRunAsync = SetItemNextRunAsync;

            using (SchedulerDataProvider schedDP = new SchedulerDataProvider()) {
                SchedulerSupport.Enabled = schedDP.GetRunning();
            }
            if (SchedulerSupport.Enabled)
                Initialize();// start task
            return Task.CompletedTask;
        }

        /// <summary>
        /// Initializes scheduling system-wide, called during application startup.
        /// </summary>
        private void Initialize() {
            List<Type> items = SchedulerEvents; // evaluate to cache available scheduler events
            schedulingThread = new Thread(new ThreadStart(Execute));
            schedulingThreadRunning = true;
            schedulingThread.Start();
        }

        /// SCHEDULER ITEMS
        /// SCHEDULER ITEMS
        /// SCHEDULER ITEMS

        /// <summary>
        /// Returns a list of types that implement scheduler events.
        /// </summary>
        public List<Type> SchedulerEvents {
            get {
                if (_schedulerEvents == null)
                    _schedulerEvents = Package.GetClassesInPackages<IScheduling>();
                return _schedulerEvents;
            }
        }
        private static List<Type>? _schedulerEvents;

        // SCHEDULER
        // SCHEDULER
        // SCHEDULER

        /// <summary>
        /// Run a scheduler item.
        /// </summary>
        /// <param name="name"></param>
        private async Task RunItemAsync(string name) {

            using (SchedulerDataProvider schedDP = new SchedulerDataProvider()) {

                using (ILockObject lockObject = await YetaWF.Core.IO.Caching.LockProvider.LockResourceAsync($"{AreaRegistration.CurrentPackage.AreaName}_RunItem_{name}")) {

                    SchedulerItemData? evnt = await schedDP.GetItemAsync(name);
                    if (evnt == null)
                        throw new Error(this.__ResStr("errItemNotFound", "Scheduler item '{0}' does not exist."), name);
                    if (evnt.RunOnce)
                        evnt.Enabled = true;
                    if (!evnt.Enabled)
                        throw new Error(this.__ResStr("errItemDisabled", "Scheduler item '{0}' is currently disabled and cannot be scheduled."), evnt.Name);
                    evnt.Next = DateTime.UtcNow.AddSeconds(-1);
                    evnt.Errors = null;
                    UpdateStatusEnum status = await schedDP.UpdateItemAsync(evnt);

                    await lockObject.UnlockAsync();

                    if (status != UpdateStatusEnum.OK)
                        throw new Error(this.__ResStr("errItemUpdFail", "Scheduler item '{0}' couldn't be updated."), evnt.Name);

                    Dispatch();// run the scheduler now
                }
            }
        }

        /// <summary>
        /// Update next run time for a scheduler item.
        /// </summary>
        /// <param name="name"></param>
        private async Task SetItemNextRunAsync(string name, DateTime? nextRun) {

            using (SchedulerDataProvider schedDP = new SchedulerDataProvider()) {

                using (ILockObject lockObject = await YetaWF.Core.IO.Caching.LockProvider.LockResourceAsync($"{AreaRegistration.CurrentPackage.AreaName}_RunItem_{name}")) {

                    SchedulerItemData? evnt = await schedDP.GetItemAsync(name);
                    if (evnt == null)
                        throw new Error(this.__ResStr("errItemNotFound", "Scheduler item '{0}' does not exist."), name);

                    evnt.Next = nextRun;
                    evnt.Enabled = nextRun != null;
                    if (nextRun != null)
                        evnt.RunOnce = true;
                    evnt.Errors = null;
                    UpdateStatusEnum status = await schedDP.UpdateItemAsync(evnt);

                    await lockObject.UnlockAsync();

                    if (status != UpdateStatusEnum.OK)
                        throw new Error(this.__ResStr("errItemUpdFail", "Scheduler item '{0}' couldn't be updated."), evnt.Name);

                    Dispatch();// run the scheduler now to update next runtime
                }
            }
        }

        /// <summary>
        /// Run the scheduler (wake it from waiting).
        /// </summary>
        /// <remarks>If the scheduler is already running a scheduler item, this call has no adverse effect.</remarks>
        private void Dispatch() {
            if (schedulingThread != null && !schedulingThreadRunning)
                schedulingThread.Interrupt();
        }

        private Thread? schedulingThread;
        private bool schedulingThreadRunning;
#if DEBUG
        private TimeSpan defaultTimeSpanNoTask = new TimeSpan(1, 0, 0); // default timespan before restart when no task is waiting
        private TimeSpan defaultTimeSpanError = new TimeSpan(0, 0, 30); // default timespan before restart when an error occurred in the scheduling loop
        private TimeSpan defaultStartupTimeSpan = new TimeSpan(0, 0, 30); // default timespan before scheduler start after site startup
#else
        private TimeSpan defaultTimeSpanNoTask = new TimeSpan(1, 0, 0); // default timespan before restart when no task is waiting
        private TimeSpan defaultTimeSpanError = new TimeSpan(0, 0, 30); // default timespan before restart when an erorr occurred in the scheduling loop
        private TimeSpan defaultStartupTimeSpan = new TimeSpan(0, 0, 30); // default timespan before scheduler start after site startup
#endif
        private SchedulerLogging SchedulerLog = null!;

        private void Execute() {

            // get a manager for the scheduler
            YetaWFManager.MakeInitialThreadInstance(null);

            // TODO: Scheduler logging should not start during startup processing. This timer postpones it (but is not a good solution)

            // Because initialization is called during application startup, we'll wait before we
            // check for any scheduler items that may be due (just so app start isn't all too slow).
            try {
                Thread.Sleep(defaultStartupTimeSpan);
            } catch (ThreadInterruptedException) {
                // thread was interrupted because there is work to be done
            }

            SchedulerLog = new SchedulerLogging();
            SchedulerLog.Init();
            SchedulerLog.LimitTo(YetaWFManager.Manager);

            YetaWFManager.Syncify(async () => { // there is no point in running the scheduler async
                await Logging.RegisterLoggingAsync(SchedulerLog);
            });

            Logging.AddTraceLog("Scheduler task started");

            // mark all scheduled items that are supposed to be run at application startup
            try {
                YetaWFManager.Syncify(async () => { // there is no point in running the scheduler async
                    await RunStartupItemsAsync();
                });
            } catch (Exception exc) {
                Logging.AddErrorLog("An error occurred running startup items", exc);
            }

            for (;;) {
                TimeSpan delayTime = defaultTimeSpanNoTask;
                if (SchedulerSupport.Enabled) {
                    try {
                        YetaWFManager.Syncify(async () => { // there is no point in running the scheduler async
                            delayTime = await RunItemsAsync();
                        });
                    } catch (Exception exc) {
                        delayTime = defaultTimeSpanError;
                        Logging.AddErrorLog("An error occurred in the scheduling loop.", exc);
                    }
                    if (delayTime < new TimeSpan(0, 0, 5))// at a few seconds
                        delayTime = new TimeSpan(0, 0, 5);
                    else if (delayTime > new TimeSpan(1, 0, 0, 0)) // max. 1 day
                        delayTime = new TimeSpan(1, 0, 0, 0);
                }
                try {
                    schedulingThreadRunning = false;
                    Logging.AddLog($"Waiting {delayTime}");
                    Thread.Sleep(delayTime);
                } catch (ThreadInterruptedException) {
                    // thread was interrupted because there is work to be done
                } catch (ThreadAbortException) { }
                finally {
                    schedulingThreadRunning = true;
                }
            }

            // This never really ends so we don't need to unregister logging
            //log.Shutdown();
            //Logging.UnregisterLogging(log);
        }

        private async Task RunStartupItemsAsync() {

            Logging.AddTraceLog("Scheduler event - checking startup scheduler items");

            using (SchedulerDataProvider schedDP = new SchedulerDataProvider()) {

                if (!await schedDP.IsInstalledAsync()) return;

                // get all scheduler items
                DataProviderGetRecords<SchedulerItemData> data = await schedDP.GetItemsAsync(0, 0, null, null);
                List<SchedulerItemData> list = data.Data;

                // reset any items that are still marked as running : Next >= DateTime.MaxValue
                foreach (SchedulerItemData item in list) {
                    if (item.Next >= DateTime.MaxValue)
                        item.Next = null;
                }

                if (SchedulerSupport.Enabled) {

                    // enable all startup items
                    foreach (SchedulerItemData item in list) {
                        if (item.EnableOnStartup && !item.Enabled) {
                            item.Enabled = true;
                            item.SetNextRuntime();
                        }
                    }
                    // check all enabled startup items that need to run on startup and set a Next time
                    foreach (SchedulerItemData item in list) {
                        if (item.Startup && item.Enabled) {
                            item.Next = DateTime.UtcNow;
                        }
                    }

                    // check for enabled items that should run eventually but have no Next time
                    foreach (SchedulerItemData item in list) {
                        if (item.Enabled && item.Next == null) {
                            item.SetNextRuntime();
                        }
                    }
                }

                foreach (SchedulerItemData item in list) {
                    UpdateStatusEnum status = await schedDP.UpdateItemAsync(item);
                    if (status != UpdateStatusEnum.OK)
                        throw new Error(this.__ResStr("errUpdateNext", "Failed to update scheduler item {0} during startup to set next time ({1})"), item.Name, status);
                }
            }
        }

        private async Task<TimeSpan> RunItemsAsync() {

            Logging.AddTraceLog("Scheduler event - checking scheduler items");

            using (SchedulerDataProvider schedDP = new SchedulerDataProvider()) {

                DateTime next = DateTime.UtcNow.Add(defaultTimeSpanNoTask);
                if (await schedDP.IsInstalledAsync()) {

                    // run all items that are due now - Enabled == true and Next != null and Next < DateTime.UtcNow
                    List<DataProviderFilterInfo> filters = new List<DataProviderFilterInfo> {
                        new DataProviderFilterInfo {
                            Logic = "&&",
                            Filters = new List<DataProviderFilterInfo> {
                                new DataProviderFilterInfo { Field = nameof(SchedulerItemData.Enabled), Operator = "==", Value = true, },
                                new DataProviderFilterInfo { Field = nameof(SchedulerItemData.Next), Operator = "!=", Value = null, },
                                new DataProviderFilterInfo { Field = nameof(SchedulerItemData.Next), Operator = "<=", Value = DateTime.UtcNow, },
                             },
                        }
                    };
                    DataProviderGetRecords<SchedulerItemData> list = await schedDP.GetItemsAsync(filters);
                    foreach (SchedulerItemData item in list.Data) {
                        await RunItemAsync(schedDP, item);
                    }

                    // Find the next startup time so we know how long to wait
                    List<DataProviderSortInfo>? sorts = null;
                    sorts = DataProviderSortInfo.Join(sorts, new DataProviderSortInfo { Field = nameof(SchedulerItemData.Next), Order = DataProviderSortInfo.SortDirection.Ascending });
                    filters = new List<DataProviderFilterInfo> {
                        new DataProviderFilterInfo {
                            Logic = "&&",
                            Filters = new List<DataProviderFilterInfo> {
                                new DataProviderFilterInfo { Field = nameof(SchedulerItemData.Enabled), Operator = "==", Value = true, },
                                new DataProviderFilterInfo {Field = nameof(SchedulerItemData.Next), Operator = "!=", Value = null, },
                            },
                        }
                    };
                    list = await schedDP.GetItemsAsync(0, 1, sorts, filters);
                    if (list.Data.Count > 0) {
                        SchedulerItemData item = list.Data[0];
                        if (item.Next != null)
                            next = (DateTime)item.Next;
                    }
                }

                TimeSpan diff = next.Subtract(DateTime.UtcNow);
                if (diff < new TimeSpan(0, 0, 0))
                    diff = new TimeSpan(0, 0, 1);
                return diff;
            }
        }

        private async Task RunItemAsync(SchedulerDataProvider schedDP, SchedulerItemData item) {

            long logId = DateTime.UtcNow.Ticks;
            SchedulerLog.SetCurrent(logId, 0, item.Name);

            item.IsRunning = true;
            item.RunTime = new TimeSpan();
            item.Last = DateTime.UtcNow;

            try {
                await schedDP.UpdateItemAsync(item);
            } catch (Exception exc) {
                Logging.AddErrorLog("Updating scheduler item {0} failed.", item.Name, exc);
            }

            StringBuilder errors = new StringBuilder();
            DateTime? nextRun = null;// called event handlers can return a next run time

            try {
                item.Errors = null;

                DateTime now = DateTime.UtcNow;
                {
                    string m = $"Scheduler event - running scheduler item '{item.Name}'.";
                    Logging.AddLog(m);
                    errors.AppendLine(m);
                }

                Type? tp = null;
                try {
                    Assembly asm = Assemblies.Load(item.Event.ImplementingAssembly)!;
                    tp = asm.GetType(item.Event.ImplementingType)!;
                } catch (Exception exc) {
                    throw new InternalError("Scheduler item '{0}' could not be loaded (Type={1}, Assembly={2}) - {3}", item.Name, item.Event.ImplementingType, item.Event.ImplementingAssembly, ErrorHandling.FormatExceptionMessage(exc));
                }

                if (item.SiteSpecific) {
                    DataProviderGetRecords<SiteDefinition> info = await SiteDefinition.GetSitesAsync(0, 0, null, null);
                    foreach (SiteDefinition site in info.Data) {

                        IScheduling? schedEvt = null;
                        try {
                            schedEvt = (IScheduling)Activator.CreateInstance(tp)!;
                        } catch (Exception exc) {
                            string m = $"Scheduler item '{item.Name}' could not be instantiated (Type={item.Event.ImplementingType}, Assembly={item.Event.ImplementingAssembly}) - {ErrorHandling.FormatExceptionMessage(exc)}";
                            Logging.AddLog(m);
                            errors.AppendLine(m);
                        }

                        if (schedEvt != null) {

                            YetaWFManager.MakeThreadInstance(site, null, true);// set up a manager for the site

                            SchedulerLog.LimitTo(YetaWFManager.Manager);
                            SchedulerLog.SetCurrent(logId, site.Identity, item.Name);

                            SchedulerItemBase itemBase = new SchedulerItemBase { Name = item.Name, Description = item.Description, EventName = item.Event.Name, Enabled = true, Frequency = item.Frequency, Startup = item.Startup, SiteSpecific = true };
                            try {
                                await schedEvt.RunItemAsync(itemBase);
                            } catch (Exception exc) {
                                string m = $"An error occurred in scheduler item '{site.Identity}: {item.Name}' - {ErrorHandling.FormatExceptionMessage(exc)}";
                                Logging.AddLog(m);
                                errors.AppendLine(m);
                            }

                            foreach (var s in itemBase.Log) {
                                string m = $"{site.Identity}: {s}";
                                Logging.AddLog(m);
                                errors.AppendLine(m);
                            }
                            if (itemBase.NextRun != null && (nextRun == null || (DateTime)itemBase.NextRun < nextRun))
                                nextRun = itemBase.NextRun;

                            YetaWFManager.RemoveThreadInstance();

                            YetaWFManager.MakeThreadInstance(null, null, true);// restore scheduler's manager
                            SchedulerLog.LimitTo(YetaWFManager.Manager);
                            SchedulerLog.SetCurrent();
                        }
                    }
                } else {

                    IScheduling? schedEvt = null;
                    try {
                        schedEvt = (IScheduling)Activator.CreateInstance(tp)!;
                    } catch (Exception exc) {
                        string m = $"Scheduler item '{item.Name}' could not be instantiated (Type={item.Event.ImplementingType}, Assembly={item.Event.ImplementingAssembly}) - {ErrorHandling.FormatExceptionMessage(exc)}";
                        Logging.AddLog(m);
                        errors.AppendLine(m);
                    }

                    if (schedEvt != null) {
                        SchedulerItemBase itemBase = new SchedulerItemBase { Name = item.Name, Description = item.Description, EventName = item.Event.Name, Enabled = true, Frequency = item.Frequency, Startup = item.Startup, SiteSpecific = false };
                        try {
                            await schedEvt.RunItemAsync(itemBase);
                        } catch (Exception exc) {
                            string m = $"An error occurred in scheduler item '{item.Name}' - {ErrorHandling.FormatExceptionMessage(exc)}";
                            Logging.AddLog(m);
                            errors.AppendLine(m);
                        }
                        foreach (var s in itemBase.Log) {
                            Logging.AddLog(s);
                            errors.AppendLine(s);
                        }

                        if (itemBase.NextRun != null && (nextRun == null || (DateTime)itemBase.NextRun < nextRun))
                            nextRun = itemBase.NextRun;
                    }
                }

                TimeSpan diff = DateTime.UtcNow - now;
                item.RunTime = diff;
                {
                    string m = $"Elapsed time for scheduler item '{item.Name}' was {diff} (hh:mm:ss.ms).";
                    Logging.AddLog(m);
                    errors.AppendLine(m);
                }

            } catch (Exception exc) {
                string m = $"Scheduler item {item.Name} failed - {ErrorHandling.FormatExceptionMessage(exc)}";
                Logging.AddErrorLog(m);
                errors.AppendLine(m);
            }

            if (item.RunOnce)
                item.Enabled = false;

            item.IsRunning = false;

            item.SetNextRuntime();
            if (nextRun != null) {
                Logging.AddLog($"Next run at {nextRun} (UTC)");
                item.Next = nextRun;
                item.Enabled = true;
            }
            item.Errors = errors.ToString();

            try {
                await schedDP.UpdateItemAsync(item);
            } catch (Exception exc) {
                string m = $"Updating scheduler item {item.Name} failed - {ErrorHandling.FormatExceptionMessage(exc)}";
                Logging.AddErrorLog(m);
                errors.AppendLine(m);
            }

            SchedulerLog.SetCurrent();
        }
    }
}