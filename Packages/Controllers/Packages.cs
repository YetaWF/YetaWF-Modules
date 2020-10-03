/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Packages.Modules;
using YetaWF.PackageAttributes;
using YetaWF.Core.Support.Zip;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Packages.Controllers {

    public class PackagesModuleController : ControllerImpl<YetaWF.Modules.Packages.Modules.PackagesModule> {

        public class PackageModel {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands { get; set; }

            public async Task<MenuList> __GetCommandsAsync() {
                MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                actions.New(await Module.GetAction_InfoLinkAsync(Package.InfoLink), ModuleAction.ActionLocationEnum.GridLinks);
                //actions.New(Module.GetAction_SupportLink(Package.SupportLink), ModuleAction.ActionLocationEnum.GridLinks);
                //actions.New(Module.GetAction_LicenseLink(Package.LicenseLink), ModuleAction.ActionLocationEnum.GridLinks);
                //actions.New(Module.GetAction_ReleaseNoticeLink(Package.ReleaseNoticeLink), ModuleAction.ActionLocationEnum.GridLinks);

                actions.New(await Module.GetAction_ExportPackageAsync(Package), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(await Module.GetAction_ExportPackageWithSourceAsync(Package), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(await Module.GetAction_ExportPackageDataAsync(Package), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(await Module.GetAction_InstallPackageModelsAsync(Package), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(await Module.GetAction_UninstallPackageModelsAsync(Package), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(await Module.GetAction_LocalizePackageAsync(Package), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(Module.GetAction_RemovePackage(Package), ModuleAction.ActionLocationEnum.GridLinks);

                actions.New(await ModLocalize.GetModuleActionAsync("Browse", null, Package), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(await ModLocalize.GetModuleActionAsync("LocalizePackageData", Package), ModuleAction.ActionLocationEnum.GridLinks);
                return actions;
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
        public class PackagesModel {
            [UIHint("Grid"), ReadOnly]
            public GridDefinition GridDef { get; set; }
        }
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(PackageModel),
                AjaxUrl = GetActionUrl(nameof(Packages_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    ModuleDefinition modLocalize = await ModuleDefinition.LoadAsync(Manager.CurrentSite.PackageLocalizationServices, AllowNone: true);
                    if (modLocalize == null)
                        throw new InternalError("No localization services available - no module has been defined");
                    DataProviderGetRecords<Package> packages = Package.GetAvailablePackages(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from p in packages.Data select new PackageModel(Module, modLocalize, p)).ToList<object>(),
                        Total = packages.Total
                    };
                },
            };
        }

        [AllowGet]
        public ActionResult Packages() {
            PackagesModel model = new PackagesModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> Packages_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync(GetGridModel(), gridPVData);
        }

        [Permission("Imports")]
        [ExcludeDemoMode]
        public async Task<ActionResult> ExportPackage(string packageName, long cookieToReturn) {
            Package package = Package.GetPackageFromPackageName(packageName);
            YetaWFZipFile zipFile = await package.ExportPackageAsync();
            return new ZippedFileResult(zipFile, cookieToReturn);
        }

        [Permission("Imports")]
        [ExcludeDemoMode]
        public async Task<ActionResult> ExportPackageWithSource(string packageName, long cookieToReturn) {
            Package package = Package.GetPackageFromPackageName(packageName/*, Utilities: true*/);
            YetaWFZipFile zipFile = await package.ExportPackageAsync(SourceCode: true);
            return new ZippedFileResult(zipFile, cookieToReturn);
        }

        [Permission("Imports")]
        public async Task<ActionResult> ExportPackageData(string packageName, long cookieToReturn) {
            Package package = Package.GetPackageFromPackageName(packageName);
            YetaWFZipFile zipFile = await package.ExportDataAsync();
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
            return FormProcessed(null, popupText: this.__ResStr("installed", "Package models successfully installed"), OnClose: OnCloseEnum.Nothing, ForcePopup: true);
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
        public async Task<ActionResult> LocalizePackage(string packageName) {
            if (YetaWFManager.Deployed)
                throw new InternalError("Can't localize packages on a deployed site");
            Package package = Package.GetPackageFromPackageName(packageName);
            List<string> errorList = new List<string>();
            if (!await package.LocalizeAsync(errorList)) {
                ScriptBuilder sb = new ScriptBuilder();
                sb.Append(this.__ResStr("cantLocalize", "Can't localize package {0}:(+nl)"), packageName.Split(new char[] { ',' }).First());
                sb.Append(errorList, LeadingNL: true);
                throw new Error(sb.ToString());
            }
            return FormProcessed(null, popupText: this.__ResStr("generated", "Package localization resources successfully generated"), OnClose: OnCloseEnum.Nothing, ForcePopup: true);
        }
        [Permission("Localize")]
        [ExcludeDemoMode]
        public async Task<ActionResult> LocalizeAllPackages() {
            if (YetaWFManager.Deployed)
                throw new InternalError("Can't localize packages on a deployed site");
            List<string> errorList = new List<string>();
            foreach (Package package in Package.GetAvailablePackages()) {
                if (package.IsCorePackage || package.IsModulePackage || package.IsSkinPackage) {
                    if (!await package.LocalizeAsync(errorList)) {
                        ScriptBuilder sb = new ScriptBuilder();
                        sb.Append(this.__ResStr("cantLocalize", "Can't localize package {0}:(+nl)"), package.Name);
                        sb.Append(errorList, LeadingNL: true);
                        throw new Error(sb.ToString());
                    }
                }
            }
            return FormProcessed(null, popupText: this.__ResStr("generatedAll", "Localization resources for all packages have been successfully generated"), OnClose: OnCloseEnum.Nothing, ForcePopup: true);
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
            if (await package.GetHasSourceAsync())
                msg = this.__ResStr("okRemovedSrc", "Package successfully removed - These settings won't take effect until the site is restarted(+nl)(+nl)This package includes source code. The source code, project and project references have to be removed manually in the Visual Studio solution for this YetaWF site.");
            else
                msg = this.__ResStr("okRemoved", "Package successfully removed - These settings won't take effect until the site is restarted");
            return Reload(PopupText: msg);
        }
    }
}