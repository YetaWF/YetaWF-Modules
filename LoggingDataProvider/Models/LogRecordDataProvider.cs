/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/LoggingDataProvider#License */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Extensions;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Localize;
#if MVC6
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Http.Features;
#else
using System.Web;
#endif

namespace YetaWF.Modules.LoggingDataProvider.DataProvider {

    public class LogRecord {

        public const int MaxSessionId = 50;
        public const int MaxMethod = 100;
        public const int MaxNamespace = 100;
        public const int MaxCategory = 50;


        [Data_PrimaryKey, Data_Identity]
        public int Key { get; set; }

        [Data_Index]
        public DateTime TimeStamp { get; set; }
        [StringLength(MaxCategory)]
        public string Category { get; set; }
        [StringLength(MaxSessionId)]
        public string SessionId { get; set; }
        public Core.Log.Logging.LevelEnum Level { get; set; }
        [StringLength(ModuleDefinition.MaxName)]
        public string ModuleName { get; set; }
        [StringLength(ModuleDefinition.MaxCssClass)]
        public string Class { get; set; }
        [StringLength(MaxMethod)]
        public string Method { get; set; }
        [StringLength(MaxNamespace)]
        public string Namespace { get; set; }
        [StringLength(Globals.MaxIP)]
        public string IPAddress { get; set; }
        [StringLength(Globals.MaxUser)]
        public string UserName { get; set; }
        public int UserId { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string RequestedUrl { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string ReferrerUrl { get; set; }

        public int SiteIdentity { get; set; }
        [StringLength(0)]
        public string Info { get; set; }

        public LogRecord() { }
    }

    public abstract class LogRecordDataProvider : IDisposable {

        protected static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(LogRecordDataProvider), name, defaultValue, parms); }
        protected YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        protected LogRecordDataProvider() {
            DisposableTracker.AddObject(this);
        }
        public void Dispose() {
            Dispose(true);
        }
        protected virtual void Dispose(bool disposing) {
            if (disposing)
                DisposableTracker.RemoveObject(this);
        }
        //~DataProviderImpl() { Dispose(false); }

        // API
        // API
        // API

        public static LogRecordDataProvider GetLogRecordDataProvider() {
            if (YetaWF.Core.Log.Logging.DefaultLoggerType == null) throw new Error(__ResStr("noLog", "Logging is not available - See https://yetawf.com/Documentation/YetaWF/Logging"));
            LogRecordDataProvider dp = (LogRecordDataProvider)Activator.CreateInstance(YetaWF.Core.Log.Logging.DefaultLoggerType);
            return dp;
        }

        public virtual Task InitAsync() { return Task.CompletedTask; }
        public virtual Task ClearAsync() { return Task.CompletedTask; }

        public virtual YetaWF.Core.Log.Logging.LevelEnum GetLevel() {
            if (_level == null)
                _level = WebConfigHelper.GetValue<YetaWF.Core.Log.Logging.LevelEnum>("Logging", "MinLevel", YetaWF.Core.Log.Logging.LevelEnum.Trace);
            return (YetaWF.Core.Log.Logging.LevelEnum)_level;
        }
        YetaWF.Core.Log.Logging.LevelEnum? _level;

        public virtual Task FlushAsync() { return Task.CompletedTask; }
        public abstract void SaveMessage(LogRecord record);

        public void WriteToLogFile(string category, Core.Log.Logging.LevelEnum level, int relStack, string message) {

            if (level == Core.Log.Logging.LevelEnum.Error)
                message += "\n" + GetCallStack(relStack + 1);
            message = message.Truncate(10000); // limit max text

            string moduleName;
            int siteIdentity = 0;

            int userId = 0;
            string userName = "";
            string ipAddress = "";
            string referrer = "";
            string requestedUrl = "";
            string sessionId = "";
            category = category.Truncate(LogRecord.MaxCategory);
            if (YetaWFManager.HaveManager) {
                if (Manager.HaveCurrentSite)
                    siteIdentity = Manager.CurrentSite.Identity;
                userId = Manager.UserId;
                userName = Manager.UserName ?? "";
                userName = userName.Truncate(Globals.MaxUser);
                sessionId = category == YetaWF.Core.Log.Logging.YetaWFEvent ? Manager.CurrentSessionId : "";
            }
            HttpContext httpContext = YetaWFManager.HttpContextAccessor.HttpContext;
            if (httpContext != null) {
                // We don't have a Manager for certain log records (particularly during startup)
                HttpRequest req = httpContext.Request;
                if (req != null) {
                    requestedUrl = req.GetDisplayUrl();
                    IHttpConnectionFeature connectionFeature = httpContext.Features.Get<IHttpConnectionFeature>();
                    if (connectionFeature != null)
                        ipAddress = connectionFeature.RemoteIpAddress.ToString();
                    referrer = req.Headers["Referer"].ToString();
                    requestedUrl = requestedUrl ?? "";
                    requestedUrl = requestedUrl.Truncate(Globals.MaxUrl);
                    referrer = referrer ?? "";
                    referrer = referrer.Truncate(Globals.MaxUrl);
                    ipAddress = ipAddress ?? "";
                    ipAddress = ipAddress.Truncate(Globals.MaxIP);
                }
            }
            MethodBase methBase = null;
            moduleName = null;
#if DEBUG
            GetCallInfo(relStack + 3, out moduleName);// this is really slow
#endif

            SaveMessage(new LogRecord {
                Category = category,
                Level = level,
                Info = message,
                TimeStamp = DateTime.UtcNow,
                SessionId = sessionId,
                ModuleName = moduleName,
                Class = methBase == null ? "" : (methBase.DeclaringType != null) ? methBase.DeclaringType.Name : "",
                Method = methBase == null ? "" : methBase.Name,
                Namespace = methBase == null ? "" : (methBase.DeclaringType != null) ? methBase.DeclaringType.Namespace : "",
                SiteIdentity = siteIdentity,
                UserId = userId,
                UserName = userName,
                IPAddress = ipAddress,
                RequestedUrl = requestedUrl,
                ReferrerUrl = referrer,
            });
        }
        /// <summary>
        /// Defines whether the logging data provider is already logging an event.
        /// </summary>
        protected bool IsProcessing { get; set; }

        public virtual Task<LogRecord> GetItemAsync(int key) {
            throw new NotImplementedException();
        }
        public virtual Task<bool> RemoveItemAsync(int key) {
            throw new NotImplementedException();
        }
        public virtual Task<DataProviderGetRecords<LogRecord>> GetItemsAsync(List<DataProviderFilterInfo> filters) {
            throw new NotImplementedException();
        }
        public virtual Task<DataProviderGetRecords<LogRecord>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            throw new NotImplementedException();
        }
        public virtual Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            throw new NotImplementedException();
        }

        public abstract string LoggerName { get; }
        public abstract bool CanBrowse { get; }
        public abstract bool CanImportOrExport { get; }
        public abstract bool CanRemove { get; }
        public abstract bool CanDownload { get; }
        public virtual string GetLogFileName() { return null; }

        static MethodBase GetCallInfo(int level, out string moduleName) {
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(level + 2);
            MethodBase methodBase = stackFrame.GetMethod();
            moduleName = "(core)";
            for (int lvl = level + 1; lvl < stackTrace.FrameCount; ++lvl) {
                stackFrame = stackTrace.GetFrame(lvl);
                MethodBase mb = stackFrame.GetMethod();
                if (mb.DeclaringType != null) {
                    string name = mb.DeclaringType.FullName;
                    string[] s = name.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                    if (s.Length >= 4) {
                        if (string.Compare(s[1], "Modules", true) == 0) {
                            moduleName = methodBase.Module.Name;
                            if (moduleName.EndsWith(".dll", StringComparison.InvariantCultureIgnoreCase))
                                moduleName = moduleName.Substring(0, moduleName.Length - 4);
                            return methodBase;
                        }
                    }
                }
            }
            return methodBase;
        }

        static string GetCallStack(int level) {
            StringBuilder sb = new StringBuilder();
            StackTrace stackTrace = new StackTrace();
            for (int lvl = level + 2; lvl < stackTrace.FrameCount; ++lvl) {
                StackFrame stackFrame = stackTrace.GetFrame(lvl);
                MethodBase methBase = stackFrame.GetMethod();
                if (methBase.DeclaringType != null) {
                    sb.AppendFormat(" - {0} {1}", methBase.DeclaringType.Namespace, methBase.DeclaringType.Name);
                } else {
                    sb.Append(" - ?");
                }
            }
            return sb.ToString();
        }

        public abstract Task<bool> IsInstalledAsync();
    }
}
