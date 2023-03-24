/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Messenger.DataProvider;
using YetaWF.Modules.Messenger.Endpoints;
using YetaWF.Modules.Messenger.Views;

namespace YetaWF.Modules.Messenger.Modules;

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
    public string? SendUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
        List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
        SiteAnnouncementModule mod = new SiteAnnouncementModule();
        menuList.New(mod.GetAction_Send(SendUrl), location);
        return menuList;
    }

    public ModuleAction GetAction_Items(string? url) {
        return new ModuleAction() {
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

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands {
            get {
                List<ModuleAction> actions = new List<ModuleAction>();
                return actions;
            }
        }

        [Caption("Date/Time Sent"), Description("The date/time the message was sent to all users")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Sent { get; set; }

        [Caption("Title"), Description("The title of the message sent to all users")]
        [UIHint("String"), ReadOnly]
        public string? Title { get; set; }

        [Caption("Message"), Description("The message that was sent to all users")]
        [UIHint("String"), ReadOnly]
        public string? Message { get; set; }

        public int Key { get; set; }

        private BrowseSiteAnnouncementModule Module { get; set; }

        public BrowseItem(BrowseSiteAnnouncementModule module, SiteAnnouncement data) {
            Module = module;
            ObjectSupport.CopyData(data, this);
        }
    }

    public class BrowseModel {
        [Caption(""), Description("")] // empty entries required so property is shown in property list (but with a suppressed label)
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<BrowseSiteAnnouncementModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                using (SiteAnnouncementDataProvider dataProvider = new SiteAnnouncementDataProvider()) {
                    DataProviderGetRecords<SiteAnnouncement> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(this, s)).ToList<object>(),
                        Total = browseItems.Total
                    };
                }
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (SiteAnnouncementDataProvider sitAnncDP = new SiteAnnouncementDataProvider()) {
            if (!sitAnncDP.Usable)
                return await RenderAsync(new { }, ViewName: SiteAnnouncementsUnavailableView.ViewName);
        }
        BrowseModel model = new BrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}
