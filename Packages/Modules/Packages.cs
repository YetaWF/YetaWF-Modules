/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Packages.Endpoints;
using YetaWF.PackageAttributes;

namespace YetaWF.Modules.Packages.Modules;

public class PackagesModuleDataProvider : ModuleDefinitionDataProvider<Guid, PackagesModule>, IInstallableModel { }

[ModuleGuid("{2B4260ED-526D-42d8-9E73-B009A2CBA484}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class PackagesModule : ModuleDefinition {

    public PackagesModule() : base() {
        Title = this.__ResStr("modTitle", "Packages");
        Name = this.__ResStr("modName", "Packages");
        Description = this.__ResStr("modSummary", "Displays and manages installed packages. It is accessible using Admin > Panel > Packages (standard YetaWF site).");
        DefaultViewName = StandardViews.Browse;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new PackagesModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("Imports",
                    this.__ResStr("roleImportsC", "Import/Export Packages"), this.__ResStr("roleImports", "The role has permission to import/export packages"),
                    this.__ResStr("userImportsC", "Import/Export Packages"), this.__ResStr("userImports", "The user has permission to import/export packages")),
                new RoleDefinition("Installs",
                    this.__ResStr("roleInstallsC", "Install/Uninstall Packages"), this.__ResStr("roleInstalls", "The role has permission to install/uninstall packages"),
                    this.__ResStr("userInstallsC", "Install/Uninstall Packages"), this.__ResStr("userInstalls", "The user has permission to install/uninstall packages")),
                new RoleDefinition("Localize",
                    this.__ResStr("roleLocalizeC", "Install/Uninstall Packages"), this.__ResStr("roleLocalize", "The role has permission to generate localization resources for packages"),
                    this.__ResStr("userLocalizeC", "Install/Uninstall Packages"), this.__ResStr("userLocalize", "The user has permission to generate localization resources for packages")),
            };
        }
    }

    public ModuleAction? GetAction_Packages(string? url) {
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            Image = "#Browse",
            LinkText = this.__ResStr("browseLink", "Packages"),
            MenuText = this.__ResStr("browseText", "Packages"),
            Tooltip = this.__ResStr("browseTooltip", "Display and manage installed packages"),
            Legend = this.__ResStr("browseLegend", "Displays and manages installed packages"),
            Style = ModuleAction.ActionStyleEnum.Normal,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            Mode = ModuleAction.ActionModeEnum.Any,
        };
    }
    public async Task<ModuleAction?> GetAction_InfoLinkAsync(string? infoLink) {
        if (string.IsNullOrWhiteSpace(infoLink)) return null;
        return new ModuleAction() {
            Url = infoLink,
            Image = await CustomIconAsync("Info.png"),
            Style = ModuleAction.ActionStyleEnum.NewWindow,
            LinkText = this.__ResStr("infoLink", "Info"),
            MenuText = this.__ResStr("infoMenu", "Info"),
            Tooltip = this.__ResStr("infoTT", "Click to visit the product's website and display product information"),
            Legend = this.__ResStr("infoLegend", "Links to the product's website and display product information"),
        };
    }
    public async Task<ModuleAction?> GetAction_SupportLinkAsync(string? supportLink) {
        if (string.IsNullOrWhiteSpace(supportLink)) return null;
        return new ModuleAction() {
            Url = supportLink,
            Image = await CustomIconAsync("Support.png"),
            Style = ModuleAction.ActionStyleEnum.NewWindow,
            LinkText = this.__ResStr("supportLink", "Support"),
            MenuText = this.__ResStr("supportMenu", "Support"),
            Tooltip = this.__ResStr("supportTT", "Click to visit the product's website and display the product's support page"),
            Legend = this.__ResStr("supportLegend", "Links to the product's website and displays the product's support page"),
        };
    }

    public async Task<ModuleAction?> GetAction_LicenseLinkAsync(string? licenseLink) {
        if (string.IsNullOrWhiteSpace(licenseLink)) return null;
        return new ModuleAction() {
            Url = licenseLink,
            Image = await CustomIconAsync("License.png"),
            Style = ModuleAction.ActionStyleEnum.NewWindow,
            LinkText = this.__ResStr("licenseLink", "License"),
            MenuText = this.__ResStr("licenseMenu", "License"),
            Tooltip = this.__ResStr("licenseTT", "Click to visit the product's website and display the licensing information"),
            Legend = this.__ResStr("licenseLegend", "Links to the product's website and displays the licensing information"),
        };
    }
    public async Task<ModuleAction?> GetAction_ReleaseNoticeLinkAsync(string? releaseNoticeLink) {
        if (string.IsNullOrWhiteSpace(releaseNoticeLink)) return null;
        return new ModuleAction() {
            Url = releaseNoticeLink,
            Image = await CustomIconAsync("Release.png"),
            Style = ModuleAction.ActionStyleEnum.NewWindow,
            LinkText = this.__ResStr("noticeLink", "Release Notice"),
            MenuText = this.__ResStr("noticeMenu", "Release Notice"),
            Tooltip = this.__ResStr("noticeTT", "Click to visit the product's website and display the release notice"),
            Legend = this.__ResStr("noticeLegend", "Links to the product's website and displays the release notice"),
        };
    }
    public async Task<ModuleAction?> GetAction_ExportPackageAsync(Package package) {
#if DEBUG
        return await Task.FromResult<ModuleAction?>(null);
#else
        if (!IsAuthorized("Imports")) return null;
        if (!await package.GetHasSourceAsync()) return null;
        if (!package.IsCorePackage && !package.IsCoreAssemblyPackage && !package.IsDataProviderPackage && !package.IsModulePackage && !package.IsSkinPackage) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(PackagesModuleEndpoints), PackagesModuleEndpoints.ExportPackage),
            QueryArgs = new { PackageName = package.Name },
            Image = await CustomIconAsync("ExportPackage.png"),
            LinkText = this.__ResStr("pLink", "Export Package"),
            MenuText = this.__ResStr("pMenu", "Export Package"),
            Tooltip = this.__ResStr("pTT", "Export the package (binary) by creating an installable ZIP file"),
            Legend = this.__ResStr("pLegend", "Exports the package (binary) by creating an installable ZIP file"),
            CookieAsDoneSignal = true,
            Style = ModuleAction.ActionStyleEnum.Normal,
        };
#endif
    }
    public async Task<ModuleAction?> GetAction_ExportPackageWithSourceAsync(Package package) {
#if DEBUG
        return await Task.FromResult<ModuleAction?>(null);
#else
        if (!IsAuthorized("Imports")) return null;
        if (!await package.GetHasSourceAsync()) return null;
        if (!package.IsCorePackage && !package.IsCoreAssemblyPackage && !package.IsDataProviderPackage && !package.IsModulePackage && !package.IsSkinPackage /*&& !package.IsTemplatePackage && !package.IsUtilityPackage*/) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(PackagesModuleEndpoints), PackagesModuleEndpoints.ExportPackageWithSource),
            QueryArgs = new { PackageName = package.Name },
            Image = await CustomIconAsync("ExportPackageWithSource.png"),
            LinkText = this.__ResStr("exportSrcLink", "Export (with source code)"),
            MenuText = this.__ResStr("exportSrcMenu", "Export Package (with source code)"),
            Tooltip = this.__ResStr("exportSrcTT", "Export the package (with source code) by creating an installable ZIP file"),
            Legend = this.__ResStr("exportSrcLegend", "Exports the package (with source code) by creating an installable ZIP file"),
            CookieAsDoneSignal = true,
            Style = ModuleAction.ActionStyleEnum.Normal,
        };
#endif
    }
    public async Task<ModuleAction?> GetAction_ExportPackageDataAsync(Package package) {
        if (!package.IsModulePackage) return null;
        if (!IsAuthorized("Imports")) return null;
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(PackagesModuleEndpoints), PackagesModuleEndpoints.ExportPackageData),
            QueryArgs = new { PackageName = package.Name },
            Image = await CustomIconAsync("ExportPackageData.png"),
            LinkText = this.__ResStr("exportLink", "Export Data"),
            MenuText = this.__ResStr("exportMenu", "Export Data"),
            Tooltip = this.__ResStr("exportTT", "Export the package data (data only) by creating a ZIP file"),
            Legend = this.__ResStr("exportLegend", "Exports the package data (data only) by creating a ZIP file"),
            CookieAsDoneSignal = true,
            Style = ModuleAction.ActionStyleEnum.Normal,
        };
    }
    public async Task<ModuleAction?> GetAction_InstallPackageModelsAsync(Package package) {
        if (!package.IsModulePackage && !package.IsCorePackage && !package.IsDataProviderPackage) return null;
        if (!IsAuthorized("Installs")) return null;
        return new ModuleAction() {
            Style = ModuleAction.ActionStyleEnum.Post,
            Url = Utility.UrlFor(typeof(PackagesModuleEndpoints), PackagesModuleEndpoints.InstallPackageModels),
            QueryArgs = new { PackageName = package.Name },
            Image = await CustomIconAsync("InstallPackageModels.png"),
            LinkText = this.__ResStr("installLink", "Install Models"),
            MenuText = this.__ResStr("installMenu", "Install Models"),
            Tooltip = this.__ResStr("installTT", "Install all data (files, SQL tables, etc.) that are needed by this package"),
            Legend = this.__ResStr("installLegend", "Installs all data (files, SQL tables, etc.) that are needed by this package"),
            Category = ModuleAction.ActionCategoryEnum.Update,
        };
    }
    public async Task<ModuleAction?> GetAction_UninstallPackageModelsAsync(Package package) {
        if (!package.IsModulePackage) return null;
        if (!IsAuthorized("Installs")) return null;
        return new ModuleAction() {
            Style = ModuleAction.ActionStyleEnum.Post,
            Url = Utility.UrlFor(typeof(PackagesModuleEndpoints), PackagesModuleEndpoints.UninstallPackageModels),
            QueryArgs = new { PackageName = package.Name },
            Image = await CustomIconAsync("UninstallPackageModels.png"),
            LinkText = this.__ResStr("uninstallLink", "Uninstall Models"),
            MenuText = this.__ResStr("uninstallMenu", "Uninstall Models"),
            Legend = this.__ResStr("uninstallLegend", "Removes all data (files, SQL tables, etc.) that are used by this package"),
            Tooltip = this.__ResStr("uninstallTT", "Removes all data (files, SQL tables, etc.) that are used by this package"),
            Category = ModuleAction.ActionCategoryEnum.Update,
        };
    }
    public async Task<ModuleAction?> GetAction_LocalizePackageAsync(Package package) {
        if (YetaWFManager.Deployed) return null; // can't do this on a deployed site
        if (!package.IsModulePackage && !package.IsCorePackage && !package.IsSkinPackage) return null;
        if (!IsAuthorized("Localize")) return null;
        return new ModuleAction() {
            Style = ModuleAction.ActionStyleEnum.Post,
            Url = Utility.UrlFor(typeof(PackagesModuleEndpoints), PackagesModuleEndpoints.LocalizePackage),
            QueryArgs = new { PackageName = package.Name },
            Image = await CustomIconAsync("LocalizePackage.png"),
            LinkText = this.__ResStr("localizeLink", "Localize Package"),
            MenuText = this.__ResStr("localizeMenu", "Localize Package"),
            Tooltip = this.__ResStr("localizeTT", "Generate all default localization resources ({0}) used by this package", MultiString.DefaultLanguage),
            Legend = this.__ResStr("localizeLegend", "Generates all default localization resources ({0}) used by this package", MultiString.DefaultLanguage),
            Category = ModuleAction.ActionCategoryEnum.Update,
        };
    }
    public async Task<ModuleAction?> GetAction_LocalizeAllPackagesAsync() {
        if (YetaWFManager.Deployed) return null; // can't do this on a deployed site
        if (!IsAuthorized("Localize")) return null;
        return new ModuleAction() {
            Style = ModuleAction.ActionStyleEnum.Post,
            Url = Utility.UrlFor(typeof(PackagesModuleEndpoints), PackagesModuleEndpoints.LocalizeAllPackages),
            Image = await CustomIconAsync("LocalizeAllPackages.png"),
            LinkText = this.__ResStr("localizeAllLink", "Localize All Packages ({0})", MultiString.DefaultLanguage),
            MenuText = this.__ResStr("localizeAllMenu", "Localize All Packages ({0})", MultiString.DefaultLanguage),
            Tooltip = this.__ResStr("localizeAllTT", "Generate all default localization resources ({0}) used by all packages", MultiString.DefaultLanguage),
            Legend = this.__ResStr("localizeAllLegend", "Generates all default localization resources ({0}) used by all packages", MultiString.DefaultLanguage),
            ConfirmationText = this.__ResStr("localizeAllConfirm", "Are you sure you want to generate the localization resources for all packages?"),
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            Category = ModuleAction.ActionCategoryEnum.Update,
            PleaseWaitText = this.__ResStr("localizePlsWait", "Updating default localization resources ({0})...", MultiString.DefaultLanguage),
        };
    }
    public async Task<ModuleAction?> GetAction_LocalizeAllPackagesDataAsync() {
        ModuleDefinition? modLocalize = await ModuleDefinition.LoadAsync(Manager.CurrentSite.PackageLocalizationServices, AllowNone: true);
        if (modLocalize == null) return null;
        ModuleAction? action = await modLocalize.GetModuleActionAsync("LocalizeAllPackagesData");
        if (action == null) return null;
        action.Location = ModuleAction.ActionLocationEnum.ModuleLinks;
        return action;
    }
    public ModuleAction? GetAction_RemovePackage(Package package) {
        if (!package.IsDataProviderPackage && !package.IsModulePackage && !package.IsSkinPackage) return null;
        if (!IsAuthorized("Installs")) return null;
        return new ModuleAction() {
            Style = ModuleAction.ActionStyleEnum.Post,
            Url = Utility.UrlFor(typeof(PackagesModuleEndpoints), PackagesModuleEndpoints.RemovePackage),
            QueryArgs = new { PackageName = package.Name },
            Image = "#Remove",
            LinkText = this.__ResStr("removeLink", "Remove Package"),
            MenuText = this.__ResStr("removeMenu", "Remove Package"),
            Tooltip = this.__ResStr("removeTT", "Remove the package"),
            Legend = this.__ResStr("removeLegend", "Removes the package"),
            ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this package?"),
            Category = ModuleAction.ActionCategoryEnum.Significant,
        };
    }
    public async Task<ModuleAction?> GetAction_CreateAllInstalledLocalizationsAsync() {
        if (YetaWFManager.Deployed) return null; // can't do this on a deployed site
        ModuleDefinition? modLocalize = await ModuleDefinition.LoadAsync(Manager.CurrentSite.PackageLocalizationServices, AllowNone: true);
        if (modLocalize == null)
            throw new InternalError("No localization services available - no module has been defined");
        ModuleAction? action = await modLocalize.GetModuleActionAsync("CreateAllInstalledLocalizations");
        if (action == null) return null;
        action.Location = ModuleAction.ActionLocationEnum.ModuleLinks;
        return action;
    }

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
        public PackageTypeEnum PackageType { get; set; }

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
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(PackageModel),
            AjaxUrl = Utility.UrlFor<PackagesModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                ModuleDefinition? modLocalize = await ModuleDefinition.LoadAsync(Manager.CurrentSite.PackageLocalizationServices, AllowNone: true);
                if (modLocalize == null)
                    throw new InternalError("No localization services available - no module has been defined");
                DataProviderGetRecords<Package> packages = Package.GetAvailablePackages(skip, take, sort, filters);
                return new DataSourceResult {
                    Data = (from p in packages.Data select new PackageModel(this, modLocalize, p)).ToList<object>(),
                    Total = packages.Total
                };
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        PackagesModel model = new PackagesModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}