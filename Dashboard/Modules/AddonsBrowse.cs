/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using System;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.DataProvider;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Dashboard.Modules {

    public class AddonsBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, AddonsBrowseModule>, IInstallableModel { }

    [ModuleGuid("{2d30fa39-622d-45eb-b52c-530699b2c9ae}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class AddonsBrowseModule : ModuleDefinition {

        public AddonsBrowseModule() {
            Title = this.__ResStr("modTitle", "AddOn Info");
            Name = this.__ResStr("modName", "AddOn Info");
            Description = this.__ResStr("modSummary", "Displays information about all Javascript AddOns, Css AddOns that are installed in the current YetaWF instance");
            DefaultViewName = StandardViews.PropertyListDisplay;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new AddonsBrowseModuleDataProvider(); }

        [Category("General"), Caption("Display Url"), Description("The Url to display a AddOn info - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string DisplayUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Items(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "AddOn Info"),
                MenuText = this.__ResStr("browseText", "AddOn Info"),
                Tooltip = this.__ResStr("browseTooltip", "Display information about all Javascript AddOns, Css AddOns that are installed in the current YetaWF instance"),
                Legend = this.__ResStr("browseLegend", "Displays information about all Javascript AddOns, Css AddOns that are installed in the current YetaWF instance"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
    }
}