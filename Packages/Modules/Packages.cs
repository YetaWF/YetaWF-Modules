/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Packages#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Packages.Controllers;

namespace YetaWF.Modules.Packages.Modules {

    public class PackagesModuleDataProvider : ModuleDefinitionDataProvider<Guid, PackagesModule>, IInstallableModel { }

    [ModuleGuid("{2B4260ED-526D-42d8-9E73-B009A2CBA484}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class PackagesModule : ModuleDefinition {

        public PackagesModule() : base() {
            Title = this.__ResStr("modTitle", "Packages");
            Name = this.__ResStr("modName", "Packages");
            Description = this.__ResStr("modSummary", "Displays and manages installed packages");
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

        public ModuleAction GetAction_Packages(string url) {
            return new ModuleAction(this) {
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
        public async Task<ModuleAction> GetAction_InfoLinkAsync(string infoLink) {
            if (string.IsNullOrWhiteSpace(infoLink)) return null;
            return new ModuleAction(this) {
                Url = infoLink,
                Image = await CustomIconAsync("Info.png"),
                Style = ModuleAction.ActionStyleEnum.NewWindow,
                LinkText = this.__ResStr("infoLink", "Info"),
                MenuText = this.__ResStr("infoMenu", "Info"),
                Tooltip = this.__ResStr("infoTT", "Click to visit the product's website and display product information"),
                Legend = this.__ResStr("infoLegend", "Links to the product's website and display product information"),
            };
        }
        public async Task<ModuleAction> GetAction_SupportLinkAsync(string supportLink) {
            if (string.IsNullOrWhiteSpace(supportLink)) return null;
            return new ModuleAction(this) {
                Url = supportLink,
                Image = await CustomIconAsync("Support.png"),
                Style = ModuleAction.ActionStyleEnum.NewWindow,
                LinkText = this.__ResStr("supportLink", "Support"),
                MenuText = this.__ResStr("supportMenu", "Support"),
                Tooltip = this.__ResStr("supportTT", "Click to visit the product's website and display the product's support page"),
                Legend = this.__ResStr("supportLegend", "Links to the product's website and displays the product's support page"),
            };
        }

        public async Task<ModuleAction> GetAction_LicenseLinkAsync(string licenseLink) {
            if (string.IsNullOrWhiteSpace(licenseLink)) return null;
            return new ModuleAction(this) {
                Url = licenseLink,
                Image = await CustomIconAsync("License.png"),
                Style = ModuleAction.ActionStyleEnum.NewWindow,
                LinkText = this.__ResStr("licenseLink", "License"),
                MenuText = this.__ResStr("licenseMenu", "License"),
                Tooltip = this.__ResStr("licenseTT", "Click to visit the product's website and display the licensing information"),
                Legend = this.__ResStr("licenseLegend", "Links to the product's website and displays the licensing information"),
            };
        }
        public async Task<ModuleAction> GetAction_ReleaseNoticeLinkAsync(string releaseNoticeLink) {
            if (string.IsNullOrWhiteSpace(releaseNoticeLink)) return null;
            return new ModuleAction(this) {
                Url = releaseNoticeLink,
                Image = await CustomIconAsync("Release.png"),
                Style = ModuleAction.ActionStyleEnum.NewWindow,
                LinkText = this.__ResStr("noticeLink", "Release Notice"),
                MenuText = this.__ResStr("noticeMenu", "Release Notice"),
                Tooltip = this.__ResStr("noticeTT", "Click to visit the product's website and display the release notice"),
                Legend = this.__ResStr("noticeLegend", "Links to the product's website and displays the release notice"),
            };
        }
        public async Task<ModuleAction> GetAction_ExportPackageAsync(Package package) {
#if DEBUG
            return await Task.FromResult<ModuleAction>(null);
#else
            if (!IsAuthorized("Imports")) return null;
            if (!await package.GetHasSourceAsync()) return null;
            if (!package.IsCorePackage && !package.IsCoreAssemblyPackage && !package.IsDataProviderPackage && !package.IsModulePackage && !package.IsSkinPackage) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(PackagesModuleController), nameof(PackagesModuleController.ExportPackage)),
                QueryArgs = new { PackageName = package.Name },
                NeedsModuleContext = true,
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
        public async Task<ModuleAction> GetAction_ExportPackageWithSourceAsync(Package package) {
#if DEBUG
            return await Task.FromResult<ModuleAction>(null);
#else
            if (!IsAuthorized("Imports")) return null;
            if (!await package.GetHasSourceAsync()) return null;
            if (!package.IsCorePackage && !package.IsCoreAssemblyPackage && !package.IsDataProviderPackage && !package.IsModulePackage && !package.IsSkinPackage /*&& !package.IsTemplatePackage && !package.IsUtilityPackage*/) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(PackagesModuleController), nameof(PackagesModuleController.ExportPackageWithSource)),
                QueryArgs = new { PackageName = package.Name },
                NeedsModuleContext = true,
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
        public async Task<ModuleAction> GetAction_ExportPackageDataAsync(Package package) {
            if (!package.IsModulePackage) return null;
            if (!IsAuthorized("Imports")) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(PackagesModuleController), nameof(PackagesModuleController.ExportPackageData)),
                QueryArgs = new { PackageName = package.Name },
                NeedsModuleContext = true,
                Image = await CustomIconAsync("ExportPackageData.png"),
                LinkText = this.__ResStr("exportLink", "Export Data"),
                MenuText = this.__ResStr("exportMenu", "Export Data"),
                Tooltip = this.__ResStr("exportTT", "Export the package data (data only) by creating a ZIP file"),
                Legend = this.__ResStr("exportLegend", "Exports the package data (data only) by creating a ZIP file"),
                CookieAsDoneSignal = true,
                Style = ModuleAction.ActionStyleEnum.Normal,
            };
        }
        public async Task<ModuleAction> GetAction_InstallPackageModelsAsync(Package package) {
            if (!package.IsModulePackage && !package.IsCorePackage) return null;
            if (!IsAuthorized("Installs")) return null;
            return new ModuleAction(this) {
                Style = ModuleAction.ActionStyleEnum.Post,
                Url = YetaWFManager.UrlFor(typeof(PackagesModuleController), nameof(PackagesModuleController.InstallPackageModels)),
                QueryArgs = new { PackageName = package.Name },
                NeedsModuleContext = true,
                Image = await CustomIconAsync("InstallPackageModels.png"),
                LinkText = this.__ResStr("installLink", "Install Models"),
                MenuText = this.__ResStr("installMenu", "Install Models"),
                Tooltip = this.__ResStr("installTT", "Install all data (files, SQL tables, etc.) that are needed by this package"),
                Legend = this.__ResStr("installLegend", "Installs all data (files, SQL tables, etc.) that are needed by this package"),
                Category = ModuleAction.ActionCategoryEnum.Update,
            };
        }
        public async Task<ModuleAction> GetAction_UninstallPackageModelsAsync(Package package) {
            if (!package.IsModulePackage) return null;
            if (!IsAuthorized("Installs")) return null;
            return new ModuleAction(this) {
                Style = ModuleAction.ActionStyleEnum.Post,
                Url = YetaWFManager.UrlFor(typeof(PackagesModuleController), nameof(PackagesModuleController.UninstallPackageModels)),
                QueryArgs = new { PackageName = package.Name },
                NeedsModuleContext = true,
                Image = await CustomIconAsync("UninstallPackageModels.png"),
                LinkText = this.__ResStr("uninstallLink", "Uninstall Models"),
                MenuText = this.__ResStr("uninstallMenu", "Uninstall Models"),
                Legend = this.__ResStr("uninstallLegend", "Removes all data (files, SQL tables, etc.) that are used by this package"),
                Tooltip = this.__ResStr("uninstallTT", "Removes all data (files, SQL tables, etc.) that are used by this package"),
                Category = ModuleAction.ActionCategoryEnum.Update,
            };
        }
        public async Task<ModuleAction> GetAction_LocalizePackageAsync(Package package) {
            if (Manager.Deployed) return null; // can't do this on a deployed site
            if (!package.IsModulePackage && !package.IsCorePackage && !package.IsSkinPackage) return null;
            if (!IsAuthorized("Localize")) return null;
            return new ModuleAction(this) {
                Style = ModuleAction.ActionStyleEnum.Post,
                Url = YetaWFManager.UrlFor(typeof(PackagesModuleController), nameof(PackagesModuleController.LocalizePackage)),
                QueryArgs = new { PackageName = package.Name },
                NeedsModuleContext = true,
                Image = await CustomIconAsync("LocalizePackage.png"),
                LinkText = this.__ResStr("localizeLink", "Localize Package"),
                MenuText = this.__ResStr("localizeMenu", "Localize Package"),
                Tooltip = this.__ResStr("localizeTT", "Generate all default localization resources ({0}) used by this package", MultiString.DefaultLanguage),
                Legend = this.__ResStr("localizeLegend", "Generates all default localization resources ({0}) used by this package", MultiString.DefaultLanguage),
                Category = ModuleAction.ActionCategoryEnum.Update,
            };
        }
        public async Task<ModuleAction> GetAction_LocalizeAllPackagesAsync() {
            if (Manager.Deployed) return null; // can't do this on a deployed site
            if (!IsAuthorized("Localize")) return null;
            return new ModuleAction(this) {
                Style = ModuleAction.ActionStyleEnum.Post,
                Url = YetaWFManager.UrlFor(typeof(PackagesModuleController), nameof(PackagesModuleController.LocalizeAllPackages)),
                NeedsModuleContext = true,
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
        public ModuleAction GetAction_RemovePackage(Package package) {
            if (!package.IsDataProviderPackage && !package.IsModulePackage && !package.IsSkinPackage) return null;
            if (!IsAuthorized("Installs")) return null;
            return new ModuleAction(this) {
                Style = ModuleAction.ActionStyleEnum.Post,
                Url = YetaWFManager.UrlFor(typeof(PackagesModuleController), "RemovePackage"),
                QueryArgs = new { PackageName = package.Name },
                NeedsModuleContext = true,
                Image = "#Remove",
                LinkText = this.__ResStr("removeLink", "Remove Package"),
                MenuText = this.__ResStr("removeMenu", "Remove Package"),
                Tooltip = this.__ResStr("removeTT", "Remove the package"),
                Legend = this.__ResStr("removeLegend", "Removes the package"),
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this package?"),
                Category = ModuleAction.ActionCategoryEnum.Significant,
            };
        }
    }
}