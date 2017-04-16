/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/BootstrapCarousel#License */

using System;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Views.Shared;
using YetaWF.DataProvider;
using YetaWF.Modules.BootstrapCarousel.Models;
#if MVC6
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.BootstrapCarousel.Modules {

    public class CarouselDisplayModuleDataProvider : ModuleDefinitionDataProvider<Guid, CarouselDisplayModule>, IInstallableModel { }

    [ModuleGuid("{d2ddaf6d-dce0-4250-95b2-48a769e04c74}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class CarouselDisplayModule : ModuleDefinition {

        public CarouselDisplayModule() {
            Title = this.__ResStr("modTitle", "Bootstrap Carousel");
            Name = this.__ResStr("modName", "Bootstrap Carousel");
            Description = this.__ResStr("modSummary", "Displays a Bootstrap carousel (used on Bootstrap skins)");
            SlideShow = new Models.CarouselInfo();
            WantFocus = false;
            WantSearch = false;
            ShowTitle = false;
            Print = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new CarouselDisplayModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public override MenuList GetModuleMenuList(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = base.GetModuleMenuList(renderMode, location);
            CarouselEditModule mod = new CarouselEditModule();
            menuList.New(mod.GetAction_Edit(EditUrl, ModuleGuid), location);
            return menuList;
        }

        [Category("General"), Caption("Edit Url"), Description("The Url used to edit this Bootstrap Carousel - If omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }

        [Category("General"), Caption("Carousel"), Description("Defines the Bootstrap Carousel settings")]
        [UIHint("YetaWF_BootstrapCarousel_SlideShow"), Trim]
        public CarouselInfo SlideShow { get; set; }

        public override void ModuleSaving() {
            SlideShow.Saving("SlideShow", ModuleGuid); // update internal information (images)
        }

        public ModuleAction GetAction_Display(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Display",
                LinkText = this.__ResStr("displayLink", "Bootstrap Carousel"),
                MenuText = this.__ResStr("displayText", "Bootstrap Carousel"),
                Tooltip = this.__ResStr("displayTooltip", "Display the Bootstrap carousel"),
                Legend = this.__ResStr("displayLegend", "Displays a Bootstrap carousel"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
            };
        }
    }
}