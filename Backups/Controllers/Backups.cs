/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Backups#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Backups.DataProvider;
using YetaWF.Modules.Backups.Modules;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Backups.Controllers {

    public class BackupsModuleController : ControllerImpl<YetaWF.Modules.Backups.Modules.BackupsModule> {

        public class BackupModel {

            [Caption("Actions"), Description("All available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };
                    actions.New(Module.GetAction_DownloadLink(FileName), ModuleAction.ActionLocationEnum.GridLinks);
                    actions.New(Module.GetAction_RemoveLink(FileName), ModuleAction.ActionLocationEnum.GridLinks);
                    return actions;
                }
            }

            [Caption("File Name"), Description("The site backup file name (located in the Backups folder)")]
            [UIHint("String"), ReadOnly]
            public string FileName { get; set; }

            [Caption("Created"), Description("The date/time the site backup was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }

            [Caption("Size"), Description("The file size of the site backup")]
            [UIHint("FileSize"), ReadOnly]
            public long Size { get; set; }

            [Caption("Full File Name"), Description("The site backup file name")]
            [UIHint("String"), ReadOnly]
            public string FullFileName { get; set; }

            public BackupsModule Module { get; private set; }

            public BackupModel(BackupsModule module, BackupEntry backup) {
                // the filename has all the info to make a BackupModel
                Module = module;
                ObjectSupport.CopyData(backup, this);
            }
        }

        public class BackupsModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        [HttpGet]
        public ActionResult Backups() {
            BackupsModel model = new BackupsModel {};
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("Backups_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BackupModel),
                SettingsModuleGuid = Module.PermanentGuid,
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult Backups_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            using (BackupsDataProvider dataProvider = new BackupsDataProvider()) {
                int total;
                List<BackupEntry> backups = dataProvider.GetBackups(skip, take, sort, filters, out total);
                GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return GridPartialView(
                    new DataSourceResult {
                        Data = (from b in backups select new BackupModel(Module, b)).ToList<object>(),
                        Total = total
                    }
                );
            }
        }

        [HttpPost]
        [Permission("Backups")]
        [ExcludeDemoMode]
        public ActionResult PerformSiteBackup() {
            List<string> errorList = new List<string>();
            SiteBackup siteBackup = new SiteBackup();
            if (!siteBackup.Create(errorList, ForDistribution: true)) {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(this.__ResStr("cantBackup", "Can't create a site backup for site {0}:(+nl)"), Manager.CurrentSite.SiteDomain);
                sb.Append(errorList, LeadingNL: true);
                throw new Error(sb.ToString());
            }
            return Reload(null, this.__ResStr("backupCreated", "The site backup has been successfully created"), Reload: ReloadEnum.ModuleParts);
        }

        [HttpPost]
        [Permission("Backups")]
        [ExcludeDemoMode]
        public ActionResult MakeSiteTemplateData() {
            if (Manager.Deployed)
                throw new InternalError("Can't make site template data on a deployed site");
            SiteTemplateData siteTemplateData = new SiteTemplateData();
            siteTemplateData.MakeSiteTemplateData();
            return Reload(null, this.__ResStr("templatesCreated", "The template for the current site has been successfully created in the \\SiteTemplates\\Data folder"), Reload: ReloadEnum.ModuleParts);
        }

        [Permission("Downloads")]
        public ActionResult Download(string filename, long cookieToReturn) {
            if (string.IsNullOrWhiteSpace(filename))
                throw new Error("No backup specified");
            filename = Path.ChangeExtension(filename, "zip");
            string path = Path.Combine(Manager.SiteFolder, SiteBackup.BackupFolder, filename);
            if (!System.IO.File.Exists(path))
                throw new Error(this.__ResStr("backupNotFound", "The backup '{0}' cannot be located.", filename));
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
            return new PhysicalFileResult(path, contentType) { FileDownloadName = filename };
#else
            FilePathResult result = new FilePathResult(path, contentType);
            result.FileDownloadName = filename;
            return result;
#endif
        }

        [HttpPost]
        [Permission("Backups")]
        [ExcludeDemoMode]
        public ActionResult Remove(string filename) {
            if (string.IsNullOrWhiteSpace(filename))
                throw new Error("No backup specified");
            SiteBackup siteBackup = new SiteBackup();
            siteBackup.Remove(filename);
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
    }
}