/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Languages.Controllers;

namespace YetaWF.Modules.Languages.Modules {

    public class LocalizeBrowsePackageModuleDataProvider : ModuleDefinitionDataProvider<Guid, LocalizeBrowsePackageModule>, IInstallableModel { }

    [ModuleGuid("{b30d6119-4769-4702-88d8-585ee4ebd4a7}"), PublishedModuleGuid]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class LocalizeBrowsePackageModule : ModuleDefinition {

        public LocalizeBrowsePackageModule() {
            Title = this.__ResStr("modTitle", "Localization Resources");
            Name = this.__ResStr("modName", "Localization Resources");
            Description = this.__ResStr("modSummary", "Displays and manages localization resources for a package. It is used by the YetaWF.Packages package to offer access to localization services. This module allows access to all localizable resource of a package, such as strings used in views and in code. See National Language Support for information about localization.");
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new LocalizeBrowsePackageModuleDataProvider(); }

        [Caption("Edit URL"), Description("The URL to edit a localization resource - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string? EditUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("Localize",
                        this.__ResStr("roleLocalizeC", "Install/Uninstall Packages"), this.__ResStr("roleLocalize", "The role has permission to generate localization resources for packages"),
                        this.__ResStr("userLocalizeC", "Install/Uninstall Packages"), this.__ResStr("userLocalize", "The user has permission to generate localization resources for packages")),
                };
            }
        }

        public ModuleAction? GetAction_Browse(string? url, Package package) {
            if (!package.IsCorePackage && !package.IsCoreAssemblyPackage && !package.IsModulePackage && !package.IsSkinPackage) return null;
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { PackageName = package.Name },
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Localization Resources"),
                MenuText = this.__ResStr("browseText", "Localization Resources"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage localization resources for package {0}", package.Name),
                Legend = this.__ResStr("browseLegend", "Displays and manages localization resources for package {0}", package.Name),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public async Task<ModuleAction?> GetAction_CreateCustomLocalizationAsync() {
            if (YetaWFManager.Deployed) return null; // can't do this on a deployed site
            if (!IsAuthorized("Localize")) return null;
            string packageName = Manager.GetUrlArg<string>("PackageName");
            if (string.IsNullOrWhiteSpace(packageName)) return null;
            Package package = Package.GetPackageFromPackageName(packageName);
            if (!package.IsCorePackage && !package.IsCoreAssemblyPackage && !package.IsModulePackage && !package.IsSkinPackage) return null;
            if (MultiString.ActiveLanguage == MultiString.DefaultLanguage) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(LocalizeBrowsePackageModuleController), nameof(LocalizeBrowsePackageModuleController.CreateCustomLocalization)),
                QueryArgs = new { PackageName = packageName, Language = MultiString.ActiveLanguage },
                Image = await CustomIconAsync("LocalizePackage.png"),
                LinkText = this.__ResStr("creCustLink", "Create Localization Resources (Custom - {0})", MultiString.ActiveLanguage),
                MenuText = this.__ResStr("creCustText", "Create Localization Resources (Custom - {0})", MultiString.ActiveLanguage),
                Tooltip = this.__ResStr("creCustTooltip", "Create custom localization resources for package {0} using language {1} - Saved in folder ./LocalizationCustom/{0}/...", package.Name, MultiString.ActiveLanguage),
                Legend = this.__ResStr("creCustLegend", "Creates custom localization resources for package {0} using language {1} - Saved in folder ./LocalizationCustom/{0}/...", package.Name, MultiString.ActiveLanguage),
                Style = ModuleAction.ActionStyleEnum.Post,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                ConfirmationText = this.__ResStr("creCustConfirm", "Are you sure you want to create custom localization resources for package {0} using language {1} - Custom localization resources are saved in folder ./LocalizationCustom/{0}/...", package.Name, MultiString.ActiveLanguage),
                NeedsModuleContext = true,
            };
        }
        public async Task<ModuleAction?> GetAction_CreateInstalledLocalizationAsync() {
            string? packageName = Manager.GetUrlArg<string>("PackageName");
            if (packageName == null) return null;
            Package package = Package.GetPackageFromPackageName(packageName);
            return await GetAction_CreateInstalledLocalizationWithPackageAsync(package);
        }
        public async Task<ModuleAction?> GetAction_CreateInstalledLocalizationWithPackageAsync(Package package) {
            if (YetaWFManager.Deployed) return null; // can't do this on a deployed site
            if (!IsAuthorized("Localize")) return null;
            if (!package.IsCorePackage && !package.IsCoreAssemblyPackage && !package.IsModulePackage && !package.IsSkinPackage) return null;
            if (MultiString.ActiveLanguage == MultiString.DefaultLanguage) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(LocalizeBrowsePackageModuleController), nameof(LocalizeBrowsePackageModuleController.CreateInstalledLocalization)),
                QueryArgs = new { PackageName = package.Name, Language = MultiString.ActiveLanguage },
                Image = await CustomIconAsync("LocalizePackage.png"),
                LinkText = this.__ResStr("creInstLink", "Create Localization Resources (Installed - {0})", MultiString.ActiveLanguage),
                MenuText = this.__ResStr("creInstText", "Create Localization Resources (Installed - {0})", MultiString.ActiveLanguage),
                Tooltip = this.__ResStr("creInstTooltip", "Create installed localization resources for package {0} using language {1} - Saved in folder ./Localizations/{0}/...", package.Name, MultiString.ActiveLanguage),
                Legend = this.__ResStr("creInstLegend", "Creates installed localization resources for package {0} using language {1} - Saved in folder ./Localizations/{0}/...", package.Name, MultiString.ActiveLanguage),
                Style = ModuleAction.ActionStyleEnum.Post,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                ConfirmationText = this.__ResStr("creInstConfirm", "Are you sure you want to create installed localization resources for package {0} using language {1} - Installed localization resources are saved in folder ./Addons/...?", package.Name, MultiString.ActiveLanguage),
                NeedsModuleContext = true,
            };
        }
        public async Task<ModuleAction?> GetAction_CreateAllInstalledLocalizationsAsync() {
            if (YetaWFManager.Deployed) return null; // can't do this on a deployed site
            if (!IsAuthorized("Localize")) return null;
            if (MultiString.ActiveLanguage == MultiString.DefaultLanguage) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(LocalizeBrowsePackageModuleController), nameof(LocalizeBrowsePackageModuleController.CreateAllInstalledLocalizations)),
                QueryArgs = new { Language = MultiString.ActiveLanguage },
                Image = await CustomIconAsync("LocalizePackage.png"),
                LinkText = this.__ResStr("creAllInstLink", "Create All Localization Resources (Installed - {0})", MultiString.ActiveLanguage),
                MenuText = this.__ResStr("creAllInstText", "Create All Localization Resources (Installed - {0})", MultiString.ActiveLanguage),
                Tooltip = this.__ResStr("creAllInstTooltip", "Create all installed localization resources using language {0} - Saved in folders ./Localizations/{0}/...", MultiString.ActiveLanguage),
                Legend = this.__ResStr("creAllInstLegend", "Creates all installed localization resources using language {0} - Saved in folder ./Localizations/{0}/...", MultiString.ActiveLanguage),
                Style = ModuleAction.ActionStyleEnum.Post,
                Location = ModuleAction.ActionLocationEnum.Explicit,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                ConfirmationText = this.__ResStr("creAllConfirm", "Are you sure you want to create ALL installed localization resources using language {0} - Installed localization resources are saved in folders ./Addons/...?", MultiString.ActiveLanguage),
                NeedsModuleContext = true,
            };
        }
        public async Task<ModuleAction?> GetAction_LocalizePackageDataAsync() {
            string? packageName = Manager.GetUrlArg<string>("PackageName");
            if (packageName == null) return null;
            Package package = Package.GetPackageFromPackageName(packageName);
            return await GetAction_LocalizePackageDataWithPackageAsync(package);
        }
        public async Task<ModuleAction?> GetAction_LocalizePackageDataWithPackageAsync(Package package) {
            if (YetaWFManager.Deployed) return null; // can't do this on a deployed site
            if (!IsAuthorized("Localize")) return null;
            if (!package.IsCorePackage && !package.IsCoreAssemblyPackage && !package.IsModulePackage && !package.IsSkinPackage) return null;
            if (MultiString.ActiveLanguage == MultiString.DefaultLanguage) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(LocalizeBrowsePackageModuleController), nameof(LocalizeBrowsePackageModuleController.LocalizePackageData)),
                QueryArgs = new { PackageName = package.Name, Language = MultiString.ActiveLanguage },
                Image = await CustomIconAsync("LocalizePackage.png"),
                LinkText = this.__ResStr("locDataLink", "Localize Package Data ({0})", MultiString.ActiveLanguage),
                MenuText = this.__ResStr("locDataText", "Localize Package Data ({0})", MultiString.ActiveLanguage),
                Tooltip = this.__ResStr("locDataTooltip", "Localize all package data for package {0} using language {1} - Properties for which localized data has already been defined are not updated", package.Name, MultiString.ActiveLanguage),
                Legend = this.__ResStr("locDataLegend", "Localizes all package data for package {0} using language {1} - Properties for which localized data has already been defined are not updated", package.Name, MultiString.ActiveLanguage),
                Style = ModuleAction.ActionStyleEnum.Post,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                ConfirmationText = this.__ResStr("locDataConfirm", "Are you sure you want to localize all package data for package {0} using language {1}? Properties for which localized data has already been defined are not updated.", package.Name, MultiString.ActiveLanguage),
                NeedsModuleContext = true,
            };
        }
        public async Task<ModuleAction?> GetAction_LocalizeAllPackagesDataAsync() {
            if (YetaWFManager.Deployed) return null; // can't do this on a deployed site
            if (!IsAuthorized("Localize")) return null;
            if (MultiString.ActiveLanguage == MultiString.DefaultLanguage) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(LocalizeBrowsePackageModuleController), nameof(LocalizeBrowsePackageModuleController.LocalizeAllPackagesData)),
                QueryArgs = new { Language = MultiString.ActiveLanguage },
                Image = await CustomIconAsync("LocalizePackage.png"),
                LinkText = this.__ResStr("locAllDataLink", "Localize Data For All Packages ({0})", MultiString.ActiveLanguage),
                MenuText = this.__ResStr("locAllDataText", "Localize Data For All Packages ({0})", MultiString.ActiveLanguage),
                Tooltip = this.__ResStr("locAllDataTooltip", "Localize data for all packages using language {0} - Properties for which localized data has already been defined are not updated", MultiString.ActiveLanguage),
                Legend = this.__ResStr("locAllDataLegend", "Localizes data for all packages using language {0} - Properties for which localized data has already been defined are not updated", MultiString.ActiveLanguage),
                Style = ModuleAction.ActionStyleEnum.Post,
                Location = ModuleAction.ActionLocationEnum.Explicit,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                ConfirmationText = this.__ResStr("locAllDataConfirm", "Are you sure you want to localize all data for all packages using language {0}? Properties for which localized data has already been defined are not updated.", MultiString.ActiveLanguage),
                NeedsModuleContext = true,
            };
        }
    }
}