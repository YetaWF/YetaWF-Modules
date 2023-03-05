/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Net;
using System.Threading.Tasks;
using YetaWF.Core.GeoLocation;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Visitors.Modules;

public class IPAddressLookupModuleDataProvider : ModuleDefinitionDataProvider<Guid, IPAddressLookupModule>, IInstallableModel { }

[ModuleGuid("{ad95564e-8eb7-4bcb-be64-dc6f1cd6b55d}"), PublishedModuleGuid]
[UniqueModule(UniqueModuleStyle.NonUnique)]
public class IPAddressLookupModule : ModuleDefinition2 {

    public IPAddressLookupModule() {
        Title = this.__ResStr("modTitle", "IP Address Lookup");
        Name = this.__ResStr("modName", "IP Address Lookup");
        Description = this.__ResStr("modSummary", "Displays information for an IP address. This module is used by the Visitor Activity Module to display host name and geolocation information for a visitor.");
        DefaultViewName = StandardViews.Display;
    }

    public override IModuleDefinitionIO GetDataProvider() { return new IPAddressLookupModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

    public ModuleAction? GetAction_DisplayHostName(string? url, string ipAddress) {
        if (string.IsNullOrWhiteSpace(ipAddress)) return null;
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { IPAddress = ipAddress, GeoData = false },
            Image = "#Display",
            LinkText = this.__ResStr("displayHostLink", "IP Address Host Name"),
            MenuText = this.__ResStr("displayHostText", "IP Address Host Name"),
            Tooltip = this.__ResStr("displayHostTooltip", "Display the IP Address Host Name"),
            Legend = this.__ResStr("displayHostLegend", "Displays the IP Address Host Name"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }
    public async Task<ModuleAction?> GetAction_DisplayGeoDataAsync(string? url, string ipAddress) {
        if (string.IsNullOrWhiteSpace(ipAddress)) return null;
        return new ModuleAction() {
            Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
            QueryArgs = new { IPAddress = ipAddress, GeoData = true },
            Image = await CustomIconAsync("GeoData.png"),
            LinkText = this.__ResStr("displayGeoLink", "IP Address Geo Data"),
            MenuText = this.__ResStr("displayGeoText", "IP Address Geo Data"),
            Tooltip = this.__ResStr("displayGeoTooltip", "Display IP Address Geo Location"),
            Legend = this.__ResStr("displayGeoLegend", "Displays IP Address Geo Location"),
            Style = ModuleAction.ActionStyleEnum.Popup,
            Category = ModuleAction.ActionCategoryEnum.Read,
            Mode = ModuleAction.ActionModeEnum.Any,
            Location = ModuleAction.ActionLocationEnum.NoAuto,
            SaveReturnUrl = true,
        };
    }

    public class Model {

        [Caption("IP Address"), Description("The IP address")]
        [UIHint("String"), ReadOnly]
        public string IPAddress { get; set; } = null!;

        [Caption("Host Name"), Description("The host name")]
        [UIHint("String"), ReadOnly]
        public string? HostName { get; set; }

        [Caption("Latitude"), Description("The latitude where the IP address is located")]
        [UIHint("FloatValue"), AdditionalMetadata("EmptyIf0", true), SuppressEmpty, ReadOnly]
        public float Latitude { get; set; }
        [Caption("Longitude"), Description("The longitude where the IP address is located")]
        [UIHint("FloatValue"), AdditionalMetadata("EmptyIf0", true), SuppressEmpty, ReadOnly]
        public float Longitude { get; set; }
        [Caption("Region"), Description("The region where the IP address is located")]
        [UIHint("String"), SuppressEmpty, ReadOnly]
        public string? RegionName { get; set; }
        [Caption("City"), Description("The city where the IP address is located")]
        [UIHint("String"), SuppressEmpty, ReadOnly]
        public string? City { get; set; }
        [Caption("Country"), Description("The country where the IP address is located")]
        [UIHint("String"), SuppressEmpty, ReadOnly]
        public string? CountryName { get; set; }
        [Caption("Continent"), Description("The continent where the IP address is located")]
        [UIHint("String"), SuppressEmpty, ReadOnly]
        public string? ContinentCode { get; set; }
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        if (!bool.TryParse(Manager.RequestQueryString["GeoData"], out bool geoData)) geoData = false;
        string ipAddress = Manager.RequestQueryString["IpAddress"] ?? throw new InternalError("IP address not specified");

        Model model = new Model();
        model.IPAddress = ipAddress;

        if (geoData) {
            GeoLocation geoLocation = new GeoLocation();
            if (geoLocation.GetRemainingRequests() <= 0)
                throw new Error(this.__ResStr("limitExceeded", "The current limit of geolocation requests per minute has been exceeded"));
            GeoLocation.UserInfo info = await geoLocation.GetUserInfoAsync(ipAddress);
            ObjectSupport.CopyData(info, model);
        }
        try {
            IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);
            model.HostName = hostEntry.HostName;
        } catch { }

        return await RenderAsync(model);
    }
}