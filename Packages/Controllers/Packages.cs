/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using YetaWF.Modules.Packages.Modules;
using YetaWF.PackageAttributes;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Packages.Controllers {

    public class PackagesModuleController : ControllerImpl<YetaWF.Modules.Packages.Modules.PackagesModule> {

        public class PackagesModel {
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }
        public class PackageModel {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                    actions.New(Module.GetAction_InfoLink(Package.InfoLink), ModuleAction.ActionLocationEnum.GridLinks);
                    //actions.New(Module.GetAction_SupportLink(Package.SupportLink), ModuleAction.ActionLocationEnum.GridLinks);
                    //actions.New(Module.GetAction_LicenseLink(Package.LicenseLink), ModuleAction.ActionLocationEnum.GridLinks);
                    //actions.New(Module.GetAction_ReleaseNoticeLink(Package.ReleaseNoticeLink), ModuleAction.ActionLocationEnum.GridLinks);

                    actions.New(Module.GetAction_ExportPackage(Package), ModuleAction.ActionLocationEnum.GridLinks);
                    actions.New(Module.GetAction_ExportPackageWithSource(Package), ModuleAction.ActionLocationEnum.GridLinks);
                    actions.New(Module.GetAction_ExportPackageData(Package), ModuleAction.ActionLocationEnum.GridLinks);
                    actions.New(Module.GetAction_InstallPackageModels(Package), ModuleAction.ActionLocationEnum.GridLinks);
                    actions.New(Module.GetAction_UninstallPackageModels(Package), ModuleAction.ActionLocationEnum.GridLinks);
                    actions.New(Module.GetAction_LocalizePackage(Package), ModuleAction.ActionLocationEnum.GridLinks);
                    actions.New(Module.GetAction_RemovePackage(Package), ModuleAction.ActionLocationEnum.GridLinks);

                    actions.New(ModLocalize.GetModuleActionAsync("Browse", null, Package).Result, ModuleAction.ActionLocationEnum.GridLinks);//$$$$
                    return actions;
                }
            }
            [Caption("Package Name"), Description("The assembly name of the package")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; }
            [Caption("Product Name"), Description("The product name used internally by YetaWF")]
            [UIHint("String"), ReadOnly]
            public string Product { get; set; }
            [Caption("Product Version"), Description("The product's version")]
            [UIHint("String"), ReadOnly]
            public string Version { get; set; }
            [Caption("Product Description"), Description("A brief description of the product")]
            [UIHint("String"), ReadOnly]
            public string Description { get; set; }

            [Caption("Company Name"), Description("The name of the company publishing this product")]
            [UIHint("String"), ReadOnly]
            public string CompanyDisplayName { get; set; }
            [Caption("Domain"), Description("The domain name of the company publishing this product (without .com, .net, etc.)")]
            [UIHint("String"), ReadOnly]
            public string Domain { get; set; }

            [Caption("Package Type"), Description("The package type")]
            [UIHint("Enum"), ReadOnly]
            public PackageTypeEnum PackageType{ get; set; }

            [Caption("Area"), Description("The MVC area for this product's modules, used internally by YetaWF")]
            [UIHint("String"), ReadOnly]
            public string AreaName { get; set; }

            public PackageModel(PackagesModule module, ModuleDefinition modLocalize, Package p) {
                Package = p;
                Module = module;
                ModLocalize = modLocalize;
                ObjectSupport.CopyData(p, this);
            }
            PackagesModule Module;
            Package Package;
            ModuleDefinition ModLocalize; //localization services
        }

        [AllowGet]
        public ActionResult Packages() {
            PackagesModel model = new PackagesModel {};
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("Packages_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(PackageModel),
                SettingsModuleGuid = Module.PermanentGuid,
            };
            return View(model);
        }

        [AllowPost]
        public async Task<ActionResult> Packages_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            ModuleDefinition modLocalize = await ModuleDefinition.LoadAsync(Manager.CurrentSite.PackageLocalizationServices, AllowNone: true);
            if (modLocalize == null)
                throw new InternalError("No localization services available - no module has been defined");
            int total;
            List<Package> packages = Package.GetAvailablePackages(skip, take, sort, filters, out total);
            GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
            return GridPartialView(new DataSourceResult {
                Data = (from p in packages select new PackageModel(Module, modLocalize, p)).ToList<object>(),
                Total = total
            });
        }

        [Permission("Imports")]
        [ExcludeDemoMode]
        public ActionResult ExportPackage(string packageName, long cookieToReturn) {
            Package package = Package.GetPackageFromPackageName(packageName);
            YetaWFZipFile zipFile = package.ExportPackage();
            return new ZippedFileResult(zipFile, cookieToReturn);
        }

        [Permission("Imports")]
        [ExcludeDemoMode]
        public ActionResult ExportPackageWithSource(string packageName, long cookieToReturn) {
            Package package = Package.GetPackageFromPackageName(packageName/*, Utilities: true*/);
            YetaWFZipFile zipFile = package.ExportPackage(SourceCode: true);
            return new ZippedFileResult(zipFile, cookieToReturn);
        }

        [Permission("Imports")]
        public ActionResult ExportPackageData(string packageName, long cookieToReturn) {
            Package package = Package.GetPackageFromPackageName(packageName);
            YetaWFZipFile zipFile = package.ExportData();
            return new ZippedFileResult(zipFile, cookieToReturn);
        }

        [Permission("Installs")]
        [ExcludeDemoMode]
        public async Task<ActionResult> InstallPackageModels(string packageName) {
            Package package = Package.GetPackageFromPackageName(packageName);
            List<string> errorList = new List<string>();
            if (!await package.InstallModelsAsync(errorList)) {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(this.__ResStr("cantInstallModels", "Can't install models for package {0}:(+nl)"), packageName.Split(new char[] { ',' }).First());
                sb.Append(errorList, LeadingNL: true);
                throw new Error(sb.ToString());
            }
            return FormProcessed(null, popupText: this.__ResStr("installed", "Package models successfully installed"), OnClose: OnCloseEnum.Nothing);
        }

        [Permission("Installs")]
        [ExcludeDemoMode]
        public async Task<ActionResult> UninstallPackageModels(string packageName) {
            Package package = Package.GetPackageFromPackageName(packageName);
            List<string> errorList = new List<string>();
            if (!await package.UninstallModelsAsync(errorList)) {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(this.__ResStr("cantUninstallModels", "Can't uninstall models for package {0}:(+nl)"), packageName.Split(new char[] { ',' }).First());
                sb.Append(errorList, LeadingNL: true);
                throw new Error(sb.ToString());
            }
            return FormProcessed(null, popupText: this.__ResStr("removed", "Package models successfully removed"), OnClose: OnCloseEnum.Nothing);
        }

        [Permission("Localize")]
        [ExcludeDemoMode]
        public ActionResult LocalizePackage(string packageName) {
            if (Manager.Deployed)
                throw new InternalError("Can't localize packages on a deployed site");
            Package package = Package.GetPackageFromPackageName(packageName);
            List<string> errorList = new List<string>();
            if (!package.Localize(errorList)) {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(this.__ResStr("cantLocalize", "Can't localize package {0}:(+nl)"), packageName.Split(new char[] { ',' }).First());
                sb.Append(errorList, LeadingNL: true);
                throw new Error(sb.ToString());
            }
            return FormProcessed(null, popupText: this.__ResStr("generated", "Package localization resources successfully generated"), OnClose: OnCloseEnum.Nothing);
        }
        [Permission("Localize")]
        [ExcludeDemoMode]
        public ActionResult LocalizeAllPackages() {
            if (Manager.Deployed)
                throw new InternalError("Can't localize packages on a deployed site");
            List<string> errorList = new List<string>();
            foreach (Package package in Package.GetAvailablePackages()) {
                if (package.IsCorePackage || package.IsModulePackage || package.IsSkinPackage) {
                    if (!package.Localize(errorList)) {
                        ScriptBuilder sb = new ScriptBuilder();
                        sb.Append(this.__ResStr("cantLocalize", "Can't localize package {0}:(+nl)"), package.Name);
                        sb.Append(errorList, LeadingNL: true);
                        throw new Error(sb.ToString());
                    }
                }
            }
            return FormProcessed(null, popupText: this.__ResStr("generatedAll", "Localization resources for all packages have been successfully generated"), OnClose: OnCloseEnum.Nothing);
        }

        [Permission("Installs")]
        [ExcludeDemoMode]
        public async Task<ActionResult> RemovePackage(string packageName) {
            Package package = Package.GetPackageFromPackageName(packageName);
            List<string> errorList = new List<string>();
            if (!await package.RemoveAsync(packageName, errorList)) {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(this.__ResStr("cantRemove", "Can't remove package {0}:(+nl)"), packageName.Split(new char[] { ',' }).First());
                sb.Append(errorList, LeadingNL: true);
                throw new Error(sb.ToString());
            }
            string msg;
            if (package.HasSource)
                msg = this.__ResStr("okRemovedSrc", "Package successfully removed - The site is now restarting...(+nl)(+nl)This package includes source code. The source code, project and project references have to be removed manually in the Visual Studio solution for this YetaWF site.");
            else
                msg = this.__ResStr("okRemoved", "Package successfully removed - The site is now restarting...");
            return Reload(PopupText: msg);
        }
    }
}