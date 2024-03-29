﻿/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Backups#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Backups.DataProvider;
using YetaWF.Modules.Backups.Modules;
using YetaWF.Core.IO;
using YetaWF.Core.Components;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.Backups.Controllers {

    public class BackupsModuleController : ControllerImpl<YetaWF.Modules.Backups.Modules.BackupsModule> {

        public class BackupModel {

            [Caption("Actions"), Description("All available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands { get; set; } = null!;

            public async Task<List<ModuleAction>> __GetCommandsAsync() {
                List<ModuleAction> actions = new List<ModuleAction>();
                actions.New(await Module.GetAction_DownloadLinkAsync(FileName), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(Module.GetAction_RemoveLink(FileName), ModuleAction.ActionLocationEnum.GridLinks);
                return actions;
            }

            [Caption("File Name"), Description("The site backup file name (located in the Backups folder)")]
            [UIHint("String"), ReadOnly]
            public string FileName { get; set; } = null!;

            [Caption("Created"), Description("The date/time the site backup was created")]
            [UIHint("DateTime"), ReadOnly]
            public DateTime Created { get; set; }

            [Caption("Size"), Description("The file size of the site backup")]
            [UIHint("FileFolderSize"), ReadOnly]
            public long Size { get; set; }

            [Caption("Full File Name"), Description("The site backup file name")]
            [UIHint("String"), ReadOnly]
            public string FullFileName { get; set; } = null!;

            public BackupsModule Module { get; private set; }

            public BackupModel(BackupsModule module, BackupEntry backup) {
                // the filename has all the info to make a BackupModel
                Module = module;
                ObjectSupport.CopyData(backup, this);
            }
        }

        public class BackupsModel {
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;
        }

        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BackupModel),
                AjaxUrl = GetActionUrl(nameof(Backups_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    using (BackupsDataProvider dataProvider = new BackupsDataProvider()) {
                        DataProviderGetRecords<BackupEntry> backups = await dataProvider.GetBackupsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from b in backups.Data select new BackupModel(Module, b)).ToList<object>(),
                            Total = backups.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult Backups() {
            BackupsModel model = new BackupsModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        public async Task<ActionResult> Backups_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync(GetGridModel(), gridPVData);
        }

        [AllowPost]
        [Permission("Backups")]
        [ExcludeDemoMode]
        public async Task<ActionResult> PerformSiteBackup() {
            List<string> errorList = new List<string>();
            SiteBackup siteBackup = new SiteBackup();
            if (!await siteBackup.CreateAsync(errorList, ForDistribution: true)) {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(this.__ResStr("cantBackup", "Can't create a site backup for site {0}:(+nl)"), Manager.CurrentSite.SiteDomain);
                sb.Append(errorList, LeadingNL: true);
                throw new Error(sb.ToString());
            }
            return Reload(null, PopupText: this.__ResStr("backupCreated", "The site backup has been successfully created"), Reload: ReloadEnum.ModuleParts);
        }

        [AllowPost]
        [Permission("Backups")]
        [ExcludeDemoMode]
        public async Task<ActionResult> MakeSiteTemplateData() {
            if (YetaWFManager.Deployed)
                throw new InternalError("Can't make site template data on a deployed site");
            SiteTemplateData siteTemplateData = new SiteTemplateData();
            await siteTemplateData.MakeSiteTemplateDataAsync();
            return Reload(null, PopupText: this.__ResStr("templatesCreated", "The templates for the current site have been successfully created in the \\SiteTemplates\\Data folder"), Reload: ReloadEnum.ModuleParts);
        }

        [Permission("Downloads")]
        public async Task<ActionResult> Download(string filename, long cookieToReturn) {
            if (string.IsNullOrWhiteSpace(filename))
                throw new Error("No backup specified");
            filename = Path.ChangeExtension(filename, "zip");
            string path = Path.Combine(Manager.SiteFolder, SiteBackup.BackupFolder, filename);
            if (!await FileSystem.FileSystemProvider.FileExistsAsync(path))
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

        [AllowPost]
        [Permission("Backups")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(string filename) {
            if (string.IsNullOrWhiteSpace(filename))
                throw new Error("No backup specified");
            SiteBackup siteBackup = new SiteBackup();
            await siteBackup.RemoveAsync(filename);
            return Reload(null, Reload: ReloadEnum.ModuleParts);
        }
    }
}