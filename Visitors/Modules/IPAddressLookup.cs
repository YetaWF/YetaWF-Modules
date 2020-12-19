/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Visitors.Modules {

    public class IPAddressLookupModuleDataProvider : ModuleDefinitionDataProvider<Guid, IPAddressLookupModule>, IInstallableModel { }

    [ModuleGuid("{ad95564e-8eb7-4bcb-be64-dc6f1cd6b55d}"), PublishedModuleGuid]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class IPAddressLookupModule : ModuleDefinition {

        public IPAddressLookupModule() {
            Title = this.__ResStr("modTitle", "IP Address Lookup");
            Name = this.__ResStr("modName", "IP Address Lookup");
            Description = this.__ResStr("modSummary", "Displays information for an IP address. This module is used by the Visitor Activity Module to display host name and geolocation information for a visitor.");
            DefaultViewName = StandardViews.Display;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new IPAddressLookupModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

        public ModuleAction GetAction_DisplayHostName(string url, string ipAddress) {
            if (string.IsNullOrWhiteSpace(ipAddress)) return null;
            return new ModuleAction(this) {
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
        public async Task<ModuleAction> GetAction_DisplayGeoDataAsync(string url, string ipAddress) {
            if (string.IsNullOrWhiteSpace(ipAddress)) return null;
            return new ModuleAction(this) {
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
    }
}