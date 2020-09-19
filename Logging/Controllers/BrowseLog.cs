/* Copyright �2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Logging#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Extensions;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Logging.Modules;
using YetaWF.Core.IO;
using YetaWF.Core.Support.Zip;
using YetaWF.Core.Components;
using YetaWF.Modules.LoggingDataProvider.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Logging.Controllers {

    public class BrowseLogModuleController : ControllerImpl<YetaWF.Modules.Logging.Modules.BrowseLogModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                    DisplayLogModule dispMod = new DisplayLogModule();
                    actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Key), ModuleAction.ActionLocationEnum.GridLinks);

                    return actions;
                }
            }

            public int Key { get; set; }

            [Caption("Date/Time"), Description("The time this record was logged")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime TimeStamp { get; set; }

            [Caption("Category"), Description("The log category")]
            [UIHint("String"), ReadOnly]
            public string Category { get; set; }

            [Caption("Session Id"), Description("The session id used to identify the visitor")]
            [UIHint("String"), ReadOnly]
            public string SessionId { get; set; }

            [Caption("Level"), Description("The error level of this log record")]
            [UIHint("Enum"), ReadOnly]
            public YetaWF.Core.Log.Logging.LevelEnum Level { get; set; }

            [Caption("Info"), Description("The information logged in this record")]
            [UIHint("String"), ReadOnly]
            public string Info { get; set; }

            [Caption("Site Id"), Description("The site which logged this record")]
            [UIHint("IntValue"), ReadOnly]
            public int SiteIdentity { get; set; }

            [Caption("IP Address"), Description("The IP address associated with this log entry")]
            [UIHint("IPAddress"), ReadOnly]
            public string IPAddress { get; set; }
            [Caption("Url"), Description("The requested Url")]
            [UIHint("Url"), ReadOnly]
            public string RequestedUrl { get; set; }
            [UIHint("Url"), ReadOnly]
            [Caption("Referrer"), Description("The referring Url associated with this log entry")]
            public string ReferrerUrl { get; set; }

            [Caption("User"), Description("The user's name/email address (if available)")]
            [UIHint("YetaWF_Identity_UserId"), ReadOnly]
            public int UserId { get; set; }

            [Caption("Module Name"), Description("The module logging this record")]
            [UIHint("String"), ReadOnly]
            public string ModuleName { get; set; }
            [Caption("Class"), Description("The class logging this record")]
            [UIHint("String"), ReadOnly]
            public string Class { get; set; }
            [Caption("Method"), Description("The method logging this record")]
            [UIHint("String"), ReadOnly]
            public string Method { get; set; }
            [Caption("Namespace"), Description("The namespace logging this record")]
            [UIHint("String"), ReadOnly]
            public string Namespace { get; set; }

            private BrowseLogModule Module { get; set; }

            public BrowseItem(BrowseLogModule module, LogRecord data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
                Info = Info.TruncateWithEllipse(80); // make it better displayable
            }
        }

        public class BrowseModel {
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
            public bool LogAvailable { get; set; }
            public bool BrowsingSupported { get; set; }
            public string LoggerName { get; set; }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                PageSizes = new List<int>() { 5, 10, 20, 50 },
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(BrowseLog_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    FlushLog();
                    using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                        DataProviderGetRecords<LogRecord> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem(Module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public async Task<ActionResult> BrowseLog() {
            FlushLog();
            using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                await dataProvider.FlushAsync();// get the latest records
                BrowseModel model = new BrowseModel {
                    LogAvailable = await dataProvider.IsInstalledAsync(),
                    BrowsingSupported = dataProvider.CanBrowse,
                    LoggerName = dataProvider.LoggerName,
                };
                if (dataProvider.CanBrowse)
                    model.GridDef = GetGridModel();
                return View(model);
            }
        }

        private static void FlushLog() {
            YetaWF.Core.Log.Logging.ForceFlush();
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> BrowseLog_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync(GetGridModel(), gridPVData);
        }

        [AllowPost]
        [Permission("RemoveLog")]
        [ExcludeDemoMode]
        public async Task<ActionResult> RemoveAll() {
            FlushLog();
            using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                await dataProvider.RemoveItemsAsync(null);// that means all records
                return Reload(null, PopupText: this.__ResStr("allRemoved", "All log records have been removed"), Reload: ReloadEnum.ModuleParts);
            }
        }

        [Permission("Downloads")]
        public async Task<ActionResult> DownloadLog(long cookieToReturn) {
            FlushLog();
            using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                string filename = dataProvider.GetLogFileName();
                if (!await FileSystem.FileSystemProvider.FileExistsAsync(filename))
                    throw new Error(this.__ResStr("logNotFound", "The log file '{0}' cannot be located", filename));
#if MVC6
                Response.Headers.Remove("Cookie");
                Response.Cookies.Append(Basics.CookieDone, cookieToReturn.ToString(), new Microsoft.AspNetCore.Http.CookieOptions { HttpOnly = false, Path = "/" });
#else
                HttpCookie cookie = new HttpCookie(Basics.CookieDone, cookieToReturn.ToString());
                Response.Cookies.Remove(Basics.CookieDone);
                Response.SetCookie(cookie);
#endif

                string contentType = "application/octet-stream";
#if MVC6
                return new PhysicalFileResult(filename, contentType) { FileDownloadName = "Logfile.txt" };
#else
                FilePathResult result = new FilePathResult(filename, contentType);
                result.FileDownloadName = "Logfile.txt";
                return result;
#endif
            }
        }

        [Permission("Downloads")]
        public async Task<ActionResult> DownloadZippedLog(long cookieToReturn) {
            FlushLog();
            using (LogRecordDataProvider dataProvider = LogRecordDataProvider.GetLogRecordDataProvider()) {
                string filename = dataProvider.GetLogFileName();
                if (!await FileSystem.FileSystemProvider.FileExistsAsync(filename))
                    throw new Error(this.__ResStr("logNotFound", "The log file '{0}' cannot be located", filename));
#if MVC6
#else
                HttpCookie cookie = new HttpCookie(Basics.CookieDone, cookieToReturn.ToString());
                Response.Cookies.Remove(Basics.CookieDone);
                Response.SetCookie(cookie);
#endif
                string zipName = "Logfile.zip";
                YetaWFZipFile zipFile = new YetaWFZipFile {
                    FileName = zipName,
                };
                zipFile.AddFile(filename, "Logfile.txt");
                return new ZippedFileResult(zipFile, cookieToReturn);
            }
        }
    }
}