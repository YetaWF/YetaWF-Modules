/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Sites#License */

using System;
using System.Web.Mvc;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Site;
using YetaWF.Core.Views.Shared;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Sites.Modules {

    public class SitesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, SitesBrowseModule>, IInstallableModel { }

    [ModuleGuid("{904ad843-1e42-4995-a600-a04b1233abfa}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class SitesBrowseModule : ModuleDefinition {

        public SitesBrowseModule() {
            Title = this.__ResStr("modTitle", "Sites");
            Name = this.__ResStr("modName", "Sites");
            Description = this.__ResStr("modSummary", "Displays and manages sites");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new SitesBrowseModuleDataProvider(); }

        [Category("General"), Caption("Add URL"), Description("The URL to add a new web site - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string AddUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return SuperuserLevel_DefaultAllowedRoles; } }

        public override MenuList GetModuleMenuList(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = base.GetModuleMenuList(renderMode, location);
            SiteAddModule mod = new SiteAddModule();
            menuList.New(mod.GetAction_Add(AddUrl), location);
            return menuList;
        }

        public ModuleAction GetAction_Sites(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Sites"),
                MenuText = this.__ResStr("browseText", "Sites"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage sites"),
                Legend = this.__ResStr("browseLegend", "Displays and manages sites"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public ModuleAction GetAction_SiteDisplay(SiteDefinition site) {
            return new ModuleAction(this) {
                Url = site.MakeUrl(RealDomain: site.SiteDomain),
                Image = "#Display",
                LinkText = this.__ResStr("dispLink", "Display Site \"{0}\"", site.SiteDomain),
                MenuText = this.__ResStr("dispText", "Display Site \"{0}\"", site.SiteDomain),
                Tooltip = this.__ResStr("dispTooltip", "Display the site \"{0}\"", site.SiteDomain),
                Legend = this.__ResStr("dispLegend", "Displays the site \"{0}\"", site.SiteDomain),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
    }
}