/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Languages#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Languages.Modules;

namespace YetaWF.Modules.Languages.Controllers {

    public class LocalizeBrowsePackageModuleController : ControllerImpl<YetaWF.Modules.Languages.Modules.LocalizeBrowsePackageModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

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
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
            public class ExtraData {
                public string PackageName { get; set; }
            }
        }

        [HttpGet]
        public ActionResult LocalizeBrowsePackage(string packageName) {
            Package package = Package.GetPackageFromPackageName(packageName);
            BrowseModel model = new BrowseModel { };
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("LocalizeBrowsePackage_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
                ExtraData = new BrowseModel.ExtraData { PackageName = packageName },
            };
            Module.Title = this.__ResStr("modTitle", "Localization Resources - Package {0}", package.Name);
            return View(model);
        }

        public class LocalizeFile {
            public string FileName { get; set; }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LocalizeBrowsePackage_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid, string packageName) {
            Package package = Package.GetPackageFromPackageName(packageName);
            int total;
            List<LocalizeFile> files = (from s in LocalizationSupport.GetFiles(package) select new LocalizeFile { FileName = Path.GetFileName(s) }).ToList();
            files = DataProviderImpl<LocalizeFile>.GetRecords(files, skip, take, sort, filters, out total);
            GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
            return GridPartialView(new DataSourceResult {
                Data = (from s in files select new BrowseItem(Module, packageName, s.FileName)).ToList<object>(),
                Total = total
            });
        }
    }
}