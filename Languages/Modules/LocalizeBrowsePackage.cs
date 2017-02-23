/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Languages#License */

using System;
using System.Collections.Generic;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.DataProvider;
using YetaWF.Modules.Languages.Controllers;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Languages.Modules {

    public class LocalizeBrowsePackageModuleDataProvider : ModuleDefinitionDataProvider<Guid, LocalizeBrowsePackageModule>, IInstallableModel { }

    [ModuleGuid("{b30d6119-4769-4702-88d8-585ee4ebd4a7}")] // Published Guid
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class LocalizeBrowsePackageModule : ModuleDefinition {

        public LocalizeBrowsePackageModule() {
            Title = this.__ResStr("modTitle", "Localization Resources");
            Name = this.__ResStr("modName", "Localization Resources");
            Description = this.__ResStr("modSummary", "Displays and manages localization resources for a package");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new LocalizeBrowsePackageModuleDataProvider(); }

        [Caption("Edit URL"), Description("The URL to edit a localization resource - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }

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

        public ModuleAction GetAction_Browse(string url, Package package) {
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
        public ModuleAction GetAction_CreateCustomLocalization() {
            if (Manager.Deployed) return null; // can't do this on a deployed site
            if (!IsAuthorized("Localize")) return null;
            string packageName = Manager.GetUrlArg<string>("PackageName");
            if (string.IsNullOrWhiteSpace(packageName)) return null;
            Package package = Package.GetPackageFromPackageName(packageName);
            if (!package.IsCorePackage && !package.IsCoreAssemblyPackage && !package.IsModulePackage && !package.IsSkinPackage) return null;
            if (MultiString.ActiveLanguage == MultiString.DefaultLanguage) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(LocalizeBrowsePackageModuleController), "CreateCustomLocalization"),
                QueryArgs = new { PackageName = packageName, Language = MultiString.ActiveLanguage },
                Image = "LocalizePackage.png",
                LinkText = this.__ResStr("creCustLink", "Create Localization Resources (Custom - {0})", MultiString.ActiveLanguage),
                MenuText = this.__ResStr("creCustText", "Create Localization Resources (Custom - {0})", MultiString.ActiveLanguage),
                Tooltip = this.__ResStr("creCustTooltip", "Create a custom localization resources for package {0} using language {1} - Saved in folder ./AddonsCustom/...", package.Name, MultiString.ActiveLanguage),
                Legend = this.__ResStr("creCustLegend", "Creates a custom localization resources for package {0} using language {1} - Saved in folder ./AddonsCustom/...", package.Name, MultiString.ActiveLanguage),
                Style = ModuleAction.ActionStyleEnum.Post,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to create custom localization resources for package {0} using language {1} - Custom localization resources are saved in ./AddonsCustom/...?", package.Name, MultiString.ActiveLanguage),
                NeedsModuleContext = true,
            };
        }
        public ModuleAction GetAction_CreateInstalledLocalization() {
            if (Manager.Deployed) return null; // can't do this on a deployed site
            if (!IsAuthorized("Localize")) return null;
            string packageName = Manager.GetUrlArg<string>("PackageName");
            if (string.IsNullOrWhiteSpace(packageName)) return null;
            Package package = Package.GetPackageFromPackageName(packageName);
            if (!package.IsCorePackage && !package.IsCoreAssemblyPackage && !package.IsModulePackage && !package.IsSkinPackage) return null;
            if (MultiString.ActiveLanguage == MultiString.DefaultLanguage) return null;
            return new ModuleAction(this) {
                Url = YetaWFManager.UrlFor(typeof(LocalizeBrowsePackageModuleController), "CreateInstalledLocalization"),
                QueryArgs = new { PackageName = packageName, Language = MultiString.ActiveLanguage },
                Image = "LocalizePackage.png",
                LinkText = this.__ResStr("creInstLink", "Create Localization Resources (Installed - {0})", MultiString.ActiveLanguage),
                MenuText = this.__ResStr("creInstText", "Create Localization Resources (Installed - {0})", MultiString.ActiveLanguage),
                Tooltip = this.__ResStr("creInstTooltip", "Create an installed localization resources for package {0} using language {1} - Saved in folder ./Addons/...", package.Name, MultiString.ActiveLanguage),
                Legend = this.__ResStr("creInstLegend", "Creates an installed localization resources for package {0} using language {1} - Saved in folder ./Addons/...", package.Name, MultiString.ActiveLanguage),
                Style = ModuleAction.ActionStyleEnum.Post,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to create custom localization resources for package {0} using language {1} - Custom localization resources are saved in ./AddonsCustom/...?", package.Name, MultiString.ActiveLanguage),
                NeedsModuleContext = true,
            };
        }
    }
}