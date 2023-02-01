/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using Microsoft.AspNetCore.Mvc;
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
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Packages.Endpoints;
using YetaWF.Modules.Packages.Modules;
using YetaWF.PackageAttributes;

namespace YetaWF.Modules.Packages.Controllers {

    public class PackagesModuleController : ControllerImpl<YetaWF.Modules.Packages.Modules.PackagesModule> {

        public class PackageModel {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ModuleActionsGrid"), ReadOnly]
            public List<ModuleAction> Commands { get; set; } = null!;

            public async Task<List<ModuleAction>> __GetCommandsAsync() {
                List<ModuleAction> actions = new List<ModuleAction>();

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
                actions.New(await ModLocalize.GetModuleActionAsync("CreateInstalledLocalizationWithPackage", Package), ModuleAction.ActionLocationEnum.GridLinks);
                
                actions.New(Module.GetAction_RemovePackage(Package), ModuleAction.ActionLocationEnum.GridLinks);

                actions.New(await ModLocalize.GetModuleActionAsync("Browse", null, Package), ModuleAction.ActionLocationEnum.GridLinks);
                actions.New(await ModLocalize.GetModuleActionAsync("LocalizePackageDataWithPackage", Package), ModuleAction.ActionLocationEnum.GridLinks);
                return actions;
            }

            [Caption("Package Name"), Description("The assembly name of the package")]
            [UIHint("String"), ReadOnly]
            public string Name { get; set; } = null!;
            [Caption("Product Name"), Description("The product name used internally by YetaWF")]
            [UIHint("String"), ReadOnly]
            public string Product { get; set; } = null!;
            [Caption("Product Version"), Description("The product's version")]
            [UIHint("String"), ReadOnly]
            public string Version { get; set; } = null!;
            [Caption("Product Description"), Description("A brief description of the product")]
            [UIHint("String"), ReadOnly]
            public string? Description { get; set; }

            [Caption("Company Name"), Description("The name of the company publishing this product")]
            [UIHint("String"), ReadOnly]
            public string CompanyDisplayName { get; set; } = null!;
            [Caption("Domain"), Description("The domain name of the company publishing this product (without .com, .net, etc.)")]
            [UIHint("String"), ReadOnly]
            public string Domain { get; set; } = null!;

            [Caption("Package Type"), Description("The package type")]
            [UIHint("Enum"), ReadOnly]
            public PackageTypeEnum PackageType{ get; set; }

            [Caption("Area"), Description("The MVC area for this product's modules, used internally by YetaWF")]
            [UIHint("String"), ReadOnly]
            public string AreaName { get; set; } = null!;

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
            public GridDefinition GridDef { get; set; } = null!;
        }
        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                RecordType = typeof(PackageModel),
                AjaxUrl = Utility.UrlFor<PackagesModuleEndpoints>(GridSupport.BrowseGridData),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                    ModuleDefinition? modLocalize = await ModuleDefinition.LoadAsync(Manager.CurrentSite.PackageLocalizationServices, AllowNone: true);
                    if (modLocalize == null)
                        throw new InternalError("No localization services available - no module has been defined");
                    DataProviderGetRecords<Package> packages = Package.GetAvailablePackages(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from p in packages.Data select new PackageModel((PackagesModule)module, modLocalize, p)).ToList<object>(),
                        Total = packages.Total
                    };
                },
            };
        }

        [AllowGet]
        public ActionResult Packages() {
            PackagesModel model = new PackagesModel {
                GridDef = GetGridModel(Module)
            };
            return View(model);
        }
    }
}