/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Languages.Endpoints;
using YetaWF.Modules.Languages.Modules;

namespace YetaWF.Modules.Languages.Controllers {

    public class LocalizeBrowsePackageModuleController : ControllerImpl<YetaWF.Modules.Languages.Modules.LocalizeBrowsePackageModule> {

        public class LocalizeFile {
            public string FileName { get; set; } = null!;
        }

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands {
                get {
                    List<ModuleAction> actions = new List<ModuleAction>();

                    LocalizeEditFileModule editMod = new LocalizeEditFileModule();
                    actions.New(editMod.GetAction_Edit(Module.EditUrl, PackageName, FileName), ModuleAction.ActionLocationEnum.GridLinks);

                    return actions;
                }
            }

            [Caption("File Name"), Description("The name of the localization resource")]
            [UIHint("String"), ReadOnly]
            public string FileName { get; set; }

            public string PackageName { get; set; }

            private LocalizeBrowsePackageModule Module { get; set; }

            public BrowseItem(LocalizeBrowsePackageModule module, string packageName, string file) {
                Module = module;
                FileName = file;
                PackageName = packageName;
            }
        }

        public class BrowseModel {
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; } = null!;

            public class ExtraData {
                public string PackageName { get; set; } = null!;
            }
        }
        internal static GridDefinition GetGridModel(ModuleDefinition module, Package package) {
            return new GridDefinition {
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = Utility.UrlFor<LocalizeBrowsePackageModuleEndpoints>(GridSupport.BrowseGridData),
                ExtraData = new BrowseModel.ExtraData { PackageName = package.Name },
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    List<LocalizeFile> files = (from s in await Localization.GetFilesAsync(package, MultiString.DefaultLanguage, false) select new LocalizeFile { FileName = Path.GetFileName(s) }).ToList();
                    DataProviderGetRecords<LocalizeFile> recs = DataProviderImpl<LocalizeFile>.GetRecords(files, skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in recs.Data select new BrowseItem((LocalizeBrowsePackageModule)module, package.Name, s.FileName)).ToList<object>(),
                        Total = recs.Total
                    };
                },
            };
        }

        [AllowGet]
        public ActionResult LocalizeBrowsePackage(string packageName) {
            Package package = Package.GetPackageFromPackageName(packageName);
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel(Module, package)
            };
            Module.Title = this.__ResStr("modTitle", "Localization Resources - Package {0}", package.Name);
            return View(model);
        }
    }
}