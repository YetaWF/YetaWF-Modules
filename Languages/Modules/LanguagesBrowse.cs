/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

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

namespace YetaWF.Modules.Languages.Modules {

    public class LanguagesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, LanguagesBrowseModule>, IInstallableModel { }

    [ModuleGuid("{0ce1d3eb-6f43-44ad-acf0-4590652f9012}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class LanguagesBrowseModule : ModuleDefinition {

        public LanguagesBrowseModule() : base() {
            Title = this.__ResStr("modTitle", "Languages");
            Name = this.__ResStr("modName", "Languages");
            Description = this.__ResStr("modSummary", "Displays and manages languages");
            DefaultViewName = StandardViews.PropertyListEdit;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new LanguagesBrowseModuleDataProvider(); }

        [Category("General"), Caption("Display URL"), Description("The URL to display a language - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string DisplayUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return SuperuserLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_Languages(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Languages"),
                MenuText = this.__ResStr("browseText", "Languages"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage languages"),
                Legend = this.__ResStr("browseLegend", "Displays and manages languages"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
    }
}