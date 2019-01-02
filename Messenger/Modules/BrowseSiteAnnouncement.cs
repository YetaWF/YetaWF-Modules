/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;
using YetaWF.Core.Components;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Messenger.Modules {

    public class BrowseSiteAnnouncementModuleDataProvider : ModuleDefinitionDataProvider<Guid, BrowseSiteAnnouncementModule>, IInstallableModel { }

    [ModuleGuid("{7057f2ab-39cd-4db3-ba27-1302a184ebaf}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class BrowseSiteAnnouncementModule : ModuleDefinition {

        public BrowseSiteAnnouncementModule() {
            Title = this.__ResStr("modTitle", "Site Announcements");
            Name = this.__ResStr("modName", "Site Announcements");
            Description = this.__ResStr("modSummary", "Displays and manages site announcements");
            DefaultViewName = StandardViews.PropertyListEdit;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new BrowseSiteAnnouncementModuleDataProvider(); }

        [Category("General"), Caption("Send Message Url"), Description("The Url to send a new site announcement - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string SendUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public override async Task<MenuList> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = await base.GetModuleMenuListAsync(renderMode, location);
            SiteAnnouncementModule mod = new SiteAnnouncementModule();
            menuList.New(mod.GetAction_Send(SendUrl), location);
            return menuList;
        }

        public ModuleAction GetAction_Items(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Site Announcements"),
                MenuText = this.__ResStr("browseText", "Site Announcements"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage site announcements"),
                Legend = this.__ResStr("browseLegend", "Displays and manages site announcements"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
    }
}
