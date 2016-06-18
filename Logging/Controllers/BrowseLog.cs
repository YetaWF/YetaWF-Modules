/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Logging#License */

using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Extensions;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Logging.DataProvider;
using YetaWF.Modules.Logging.Modules;
using static YetaWF.Core.Log.Logging;

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

            [Caption("Level"), Description("The error level of this log record")]
            [UIHint("Enum"), ReadOnly]
            public LevelEnum Level { get; set; }

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
                Info = Info.Truncate(40); // make it better displayable
            }
        }

        public class BrowseModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
            public bool LogAvailable { get; set; }
            public bool BrowsingSupported { get; set; }
        }

        [HttpGet]
        public ActionResult BrowseLog() {
            FlushLog();
            using (LogRecordDataProvider dataProvider = new LogRecordDataProvider()) {
                BrowseModel model = new BrowseModel {
                    LogAvailable = dataProvider.IsInstalled(),
                    BrowsingSupported = dataProvider.CanBrowse,
                };
                if (dataProvider.CanBrowse) {
                    model.GridDef = new GridDefinition {
                        AjaxUrl = GetActionUrl("BrowseLog_GridData"),
                        ModuleGuid = Module.ModuleGuid,
                        RecordType = typeof(BrowseItem),
                        SettingsModuleGuid = Module.PermanentGuid,
                        PageSizes = new List<int>() { 5, 10, 20, 50 },

                };
                }
                return View(model);
            }
        }

        private static void FlushLog() {
            if (YetaWF.Core.Log.Logging.ForceFlush != null)
                YetaWF.Core.Log.Logging.ForceFlush();
        }

        [HttpPost]
        public ActionResult BrowseLog_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            FlushLog();
            GridHelper.UpdateAlternateSortColumn(sort, filters, "UserId", "UserName");
            using (LogRecordDataProvider dataProvider = new LogRecordDataProvider()) {
                int total;
                List<LogRecord> browseItems = dataProvider.GetItems(skip, take, sort, filters, out total);
                GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return GridPartialView(new DataSourceResult {
                    Data = (from s in browseItems select new BrowseItem(Module, s)).ToList<object>(),
                    Total = total
                });
            }
        }

        [HttpPost]
        [Permission("RemoveLog")]
        public ActionResult RemoveAll() {
            FlushLog();
            using (LogRecordDataProvider dataProvider = new LogRecordDataProvider()) {
                dataProvider.RemoveItems(null);// that means all records
                return Reload(null, PopupText: this.__ResStr("allRemoved", "All log records have been removed"), Reload: ReloadEnum.ModuleParts);
            }
        }

        [Permission("Downloads")]
        public ActionResult DownloadLog(long cookieToReturn) {
            FlushLog();
            using (LogRecordDataProvider dataProvider = new LogRecordDataProvider()) {
                string filename = dataProvider.GetLogFileName();
                if (!System.IO.File.Exists(filename))
                    throw new Error(this.__ResStr("logNotFound", "The log file '{0}' cannot be located", filename));

                HttpCookie cookie = new HttpCookie(Basics.CookieDone, cookieToReturn.ToString());
                Response.Cookies.Remove(Basics.CookieDone);
                Response.SetCookie(cookie);

                string contentType = "application/octet-stream";
                FilePathResult result = new FilePathResult(filename, contentType);
                result.FileDownloadName = "Logfile.txt";
                return result;
            }
        }

        [Permission("Downloads")]
        public ActionResult DownloadZippedLog(long cookieToReturn) {
            FlushLog();
            using (LogRecordDataProvider dataProvider = new LogRecordDataProvider()) {
                string filename = dataProvider.GetLogFileName();
                if (!System.IO.File.Exists(filename))
                    throw new Error(this.__ResStr("logNotFound", "The log file '{0}' cannot be located", filename));

                HttpCookie cookie = new HttpCookie(Basics.CookieDone, cookieToReturn.ToString());
                Response.Cookies.Remove(Basics.CookieDone);
                Response.SetCookie(cookie);

                string zipName = "Logfile.zip";
                YetaWFZipFile zipFile = new YetaWFZipFile {
                    FileName = zipName,
                    Zip = new ZipFile(zipName),
                };
                ZipEntry ze = zipFile.Zip.AddFile(filename);
                ze.FileName = "Logfile.txt";
                return new ZippedFileResult(zipFile, cookieToReturn);
            }
        }
    }
}