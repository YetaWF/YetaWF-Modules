/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

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
using YetaWF.Modules.Visitors.DataProvider;
using YetaWF.Modules.Visitors.Endpoints;

namespace YetaWF.Modules.Visitors.Modules;

public class VisitorsModuleDataProvider : ModuleDefinitionDataProvider<Guid, VisitorsModule>, IInstallableModel { }

[ModuleGuid("{d0a9aee6-e93f-4c39-afde-3a63cf5b3df7}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class VisitorsModule : ModuleDefinition2 {

    public VisitorsModule() {
        Title = this.__ResStr("modTitle", "Visitor Activity");
        Name = this.__ResStr("modName", "Visitor Activity");
        Description = this.__ResStr("modSummary", "Displays visitor activity. It is accessible using Admin > Dashboard > Visitor Activity (standard YetaWF site). Displaying a record's detailed information is available in the Visitor Activity grid shown by the Visitors Module. Records cannot be removed.");
        DefaultViewName = StandardViews.Browse;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new VisitorsModuleDataProvider(); }

    [Category("General"), Caption("Display URL"), Description("The URL to display a visitor entry - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string? DisplayUrl { get; set; }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
    public override List<RoleDefinition> ExtraRoles {
        get {
            return new List<RoleDefinition>() {
                new RoleDefinition("UpdateGeoLocation",
                    this.__ResStr("roleUpdateGeoLocationC", "Update Geolocation Data"), this.__ResStr("roleUpdateGeoLocations", "The role has permission to update Geolocation data"),
                    this.__ResStr("userUpdateGeoLocationC", "Update Geolocation Data"), this.__ResStr("userUpdateGeoLocations", "The user has permission to update Geolocation data")),
            };
        }
    }

    public ModuleAction? GetAction_Items(string? url) {
        return new ModuleAction() {
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
    public ModuleAction? GetAction_UpdateGeoLocation() {
        if (!IsAuthorized("UpdateGeoLocation")) return null;
        using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
            if (!visitorDP.Usable) return null;
        }
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(VisitorsModuleEndpoints), VisitorsModuleEndpoints.UpdateGeoLocation),
            QueryArgs = new { },
            Image = "#Add",
            Style = ModuleAction.ActionStyleEnum.Post,
            LinkText = this.__ResStr("updgAuthLink", "GeoLocation Data"),
            MenuText = this.__ResStr("updgAuthMenu", "GeoLocation Data"),
            Tooltip = this.__ResStr("updgAuthTT", "Update GeoLocation Data"),
            Legend = this.__ResStr("updgAuthLegend", "Updates GeoLocation Data"),
            Category = ModuleAction.ActionCategoryEnum.Significant,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            ConfirmationText = this.__ResStr("updgUpdateGeoLocation", "Are you sure you want to update GeoLocation data?"),
            PleaseWaitText = this.__ResStr("confUpdateGeoLocation", "Updating GeoLocation data..."),
        };
    }

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands {
            get {
                List<ModuleAction> actions = new List<ModuleAction>();

                VisitorDisplayModule dispMod = new VisitorDisplayModule();
                actions.New(dispMod.GetAction_Display(Module.DisplayUrl, Key), ModuleAction.ActionLocationEnum.GridLinks);

                return actions;
            }
        }

        [UIHint("Hidden")]
        public int Key { get; set; }

        [Caption("Session Id"), Description("The session id used to identify the visitor")]
        [UIHint("String"), ReadOnly]
        public string? SessionId { get; set; }

        [Caption("Accessed"), Description("The date and time the visitor visited the site")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime AccessDateTime { get; set; }

        [Caption("User"), Description("The user's email address (if available)")]
        [UIHint("YetaWF_Identity_UserId"), ReadOnly]
        public int UserId { get; set; }

        [Caption("IP Address"), Description("The IP address of the site visitor")]
        [UIHint("IPAddress"), ReadOnly]
        public string? IPAddress { get; set; }

        [Caption("Continent"), Description("The continent where the visitor is located, based on IP address (if available)")]
        [UIHint("String"), ReadOnly]
        public string? ContinentCode { get; set; }
        [Caption("Country"), Description("The country where the visitor is located, based on IP address (if available)")]
        [UIHint("String"), ReadOnly]
        public string? CountryCode { get; set; }
        [Caption("Region"), Description("The region where the visitor is located, based on IP address (if available)")]
        [UIHint("String"), ReadOnly]
        public string? RegionCode { get; set; }
        [Caption("City"), Description("The city where the visitor is located, based on IP address (if available)")]
        [UIHint("String"), ReadOnly]
        public string? City { get; set; }

        [Caption("Url"), Description("The Url accessed by the site visitor")]
        [UIHint("Url"), ReadOnly]
        public string? Url { get; set; }
        [Caption("Referrer"), Description("The Url where the site visitor came from")]
        [UIHint("Url"), ReadOnly]
        public string? Referrer { get; set; }
        [Caption("User Agent"), Description("The web browser's user agent")]
        [UIHint("String"), ReadOnly]
        public string? UserAgent { get; set; }
        [Caption("Error"), Description("Shows any error that may have occurred")]
        [UIHint("String"), ReadOnly]
        public string? Error { get; set; }

        private VisitorsModule Module { get; set; }

        public BrowseItem(VisitorsModule module, VisitorEntry data) {
            Module = module;
            ObjectSupport.CopyData(data, this);
        }
    }

    public class BrowseModel {
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; } = null!;
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            InitialPageSize = 20,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<VisitorsModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) => {
                using (VisitorEntryDataProvider dataProvider = new VisitorEntryDataProvider()) {
                    DataProviderGetRecords<VisitorEntry> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                    return new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(this, s)).ToList<object>(),
                        Total = browseItems.Total
                    };
                }
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
            if (!visitorDP.Usable)
                throw new Error(this.__ResStr("noInfo", "Visitor information is not available - See https://yetawf.com/Documentation/YetaWF/Visitors"));
        }
        BrowseModel model = new BrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}