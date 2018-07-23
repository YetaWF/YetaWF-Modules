/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Scheduler.DataProvider;
using YetaWF.Modules.Scheduler.Modules;
using YetaWF.Core.IO;
using YetaWF.Core.Support.Zip;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Scheduler.Controllers {

    public class LogBrowseModuleController : ControllerImpl<YetaWF.Modules.Scheduler.Modules.LogBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                    LogDisplayModule dispMod = new LogDisplayModule();
                    actions.New(dispMod.GetAction_Display(Module.DisplayUrl, LogEntry), ModuleAction.ActionLocationEnum.GridLinks);

                    return actions;
                }
            }

            public int LogEntry { get; set; }

            [Caption("Created"), Description("The date/time this log record was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime TimeStamp { get; set; }

            [Caption("Id"), Description("The id of the scheduler item run")]
            [UIHint("LongValue"), ReadOnly]
            public long RunId { get; set; }

            [Caption("Name"), Description("The name of the running scheduler item")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }

            [Caption("Level"), Description("The message level")]
            [UIHint("Enum"), ReadOnly]
            public Core.Log.Logging.LevelEnum Level { get; set; }

            [Caption("Site Id"), Description("The site which was affected by the scheduler item")]
            [UIHint("SiteId"), ReadOnly]
            public int SiteIdentity { get; set; }

            [Caption("Message"), Description("The message")]
            [UIHint("String"), ReadOnly]
            public string Info { get; set; }

            private LogBrowseModule Module { get; set; }

            public BrowseItem(LogBrowseModule module, LogData data) {
                Module = module;
                ObjectSupport.CopyData(data, this);
            }
        }

        public class BrowseModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
            public bool LogAvailable { get; set; }
            public bool BrowsingSupported { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> LogBrowse() {
            using (LogDataProvider logDP = new LogDataProvider()) {
                BrowseModel model = new BrowseModel {
                    LogAvailable = await logDP.IsInstalledAsync(),
                    BrowsingSupported = logDP.CanBrowse,
                };
                if (logDP.CanBrowse) {
                    model.GridDef = new GridDefinition {
                        AjaxUrl = GetActionUrl("LogBrowse_GridData"),
                        ModuleGuid = Module.ModuleGuid,
                        RecordType = typeof(BrowseItem),
                        SettingsModuleGuid = Module.PermanentGuid,
                        InitialPageSize = 20,
                    };
                }
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> LogBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            using (LogDataProvider logDP = new LogDataProvider()) {
                DataProviderGetRecords<LogData> browseItems = await logDP.GetItemsAsync(skip, take, sort, filters);
                Grid.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return await GridPartialViewAsync(new DataSourceResult {
                    Data = (from s in browseItems.Data select new BrowseItem(Module, s)).ToList<object>(),
                    Total = browseItems.Total
                });
            }
        }

        [AllowPost]
        [Permission("RemoveLog")]
        public async Task<ActionResult> RemoveAll() {
            using (LogDataProvider logDP = new LogDataProvider()) {
                await logDP.RemoveItemsAsync(null);
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }

        [Permission("Downloads")]
        public async Task<ActionResult> DownloadLog(long cookieToReturn) {
            using (LogDataProvider logDP = new LogDataProvider()) {
                string filename = logDP.GetLogFileName();
                if (!await FileSystem.FileSystemProvider.FileExistsAsync(filename))
                    throw new Error(this.__ResStr("logNotFound", "The scheduler log file '{0}' cannot be located", filename));
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
                result.FileDownloadName = "SchedulerLog.txt";
                return result;
#endif
            }
        }

        [Permission("Downloads")]
        public async Task<ActionResult> DownloadZippedLog(long cookieToReturn) {
            using (LogDataProvider dataProvider = new LogDataProvider()) {
                string filename = dataProvider.GetLogFileName();
                if (!await FileSystem.FileSystemProvider.FileExistsAsync(filename))
                    throw new Error(this.__ResStr("logNotFound", "The scheduler log file '{0}' cannot be located", filename));
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
                zipFile.AddFile(filename, "SchedulerLog.txt");
                return new ZippedFileResult(zipFile, cookieToReturn);
            }
        }
    }
}
