/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Views.Shared;
using YetaWF.DataProvider;
#if MVC6
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Visitors.Modules {

    public class VisitorsModuleDataProvider : ModuleDefinitionDataProvider<Guid, VisitorsModule>, IInstallableModel { }

    [ModuleGuid("{d0a9aee6-e93f-4c39-afde-3a63cf5b3df7}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class VisitorsModule : ModuleDefinition {

        public VisitorsModule() {
            Title = this.__ResStr("modTitle", "Visitor Activity");
            Name = this.__ResStr("modName", "Visitor Activity");
            Description = this.__ResStr("modSummary", "Displays visitor activity");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new VisitorsModuleDataProvider(); }

        [Category("General"), Caption("Display URL"), Description("The URL to display a visitor entry - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string DisplayUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Items(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Visitor Activity"),
                MenuText = this.__ResStr("browseText", "Visitor Activity"),
                Tooltip = this.__ResStr("browseTooltip", "Display visitor activity"),
                Legend = this.__ResStr("browseLegend", "Displays visitor activity"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
    }
}