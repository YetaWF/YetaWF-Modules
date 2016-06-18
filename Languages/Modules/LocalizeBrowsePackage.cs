/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Languages#License */

using System;
using System.Web.Mvc;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Views.Shared;
using YetaWF.DataProvider;

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

        public ModuleAction GetAction_Browse(string url, Package package) {
            if (!package.IsCorePackage && !package.IsCoreAssemblyPackage && !package.IsModulePackage) return null;
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                QueryArgs = new { PackageName = package.Name },
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Localization Resources"),
                MenuText = this.__ResStr("browseText", "Localization Resources"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage localization resources for package {0}", package.Name),
                Legend = this.__ResStr("browseLegend", "Displays and manages localization resources for package {0}", package.Name),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks | ModuleAction.ActionLocationEnum.ModuleMenu,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
    }
}