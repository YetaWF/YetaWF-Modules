/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Backups#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Backups.DataProvider;
using YetaWF.Modules.Backups.Endpoints;
using YetaWF.Modules.Backups.Modules;

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

        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                RecordType = typeof(BackupModel),
                AjaxUrl = Utility.UrlFor(typeof(BackupsModuleEndpoints), nameof(GridSupport.BrowseGridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    using (BackupsDataProvider dataProvider = new BackupsDataProvider()) {
                        DataProviderGetRecords<BackupEntry> backups = await dataProvider.GetBackupsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from b in backups.Data select new BackupModel((BackupsModule)module, b)).ToList<object>(),
                            Total = backups.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult Backups() {
            BackupsModel model = new BackupsModel {
                GridDef = GetGridModel(Module)
            };
            return View(model);
        }
    }
}