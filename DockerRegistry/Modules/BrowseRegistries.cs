/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DockerRegistry#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
#if MVC6
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DockerRegistry.Modules {

    public class BrowseRegistriesModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowseRegistriesModule>, IInstallableModel { }

    [ModuleGuid("{133f90c3-203a-4541-9c54-e740ee28c20d}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class BrowseRegistriesModule : ModuleDefinition {

        public BrowseRegistriesModule() {
            Title = this.__ResStr("modTitle", "Docker Registries");
            Name = this.__ResStr("modName", "Docker Registries");
            Description = this.__ResStr("modSummary", "Displays and manages Docker registries");
            DefaultViewName = StandardViews.PropertyListEdit;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BrowseRegistriesModuleDataProvider(); }

        [Category("General"), Caption("Display Url"), Description("The URL to display a registry - If omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string DisplayUrl { get; set; }

        public override async Task<MenuList> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = await base.GetModuleMenuListAsync(renderMode, location);
            return menuList;
        }

        public ModuleAction GetAction_Items(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Registries"),
                MenuText = this.__ResStr("browseText", "Registries"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage registries"),
                Legend = this.__ResStr("browseLegend", "Displays and manages registries"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
    }
}
