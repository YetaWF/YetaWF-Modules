/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using System;
using System.Collections.Generic;
using YetaWF.Core;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Visitors.Controllers;
using YetaWF.Modules.Visitors.DataProvider;
#if MVC6
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
        public ModuleAction? GetAction_UpdateGeoLocation() {
            if (!IsAuthorized("UpdateGeoLocation")) return null;
            using (VisitorEntryDataProvider visitorDP = new VisitorEntryDataProvider()) {
                if (!visitorDP.Usable) return null;
            }
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(VisitorsModuleController), nameof(VisitorsModuleController.UpdateGeoLocation)),
                NeedsModuleContext = true,
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
    }
}