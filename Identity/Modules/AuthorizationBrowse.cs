/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

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
using YetaWF.Modules.Identity.Endpoints;

namespace YetaWF.Modules.Identity.Modules {

    public class AuthorizationBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, AuthorizationBrowseModule>, IInstallableModel { }

    [ModuleGuid("{d75f9b25-bede-407c-8737-4506982d8e09}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    [ModuleCategory("Configuration")]
    public class AuthorizationBrowseModule : ModuleDefinition {

        public AuthorizationBrowseModule() : base() {
            Title = this.__ResStr("modTitle", "Resources");
            Name = this.__ResStr("modName", "Resources");
            Description = this.__ResStr("modSummary", "Used to display and manage resources used for authorizations. It is accessible using Admin > Identity Settings > Resources (standard YetaWF site).");
        }

        public override IModuleDefinitionIO GetDataProvider() { return new AuthorizationBrowseModuleDataProvider(); }

        [Category("General"), Caption("Add Url"), Description("The Url to add a new resource - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string AddUrl { get; set; }

        [Category("General"), Caption("Display Url"), Description("The Url to display resource information - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string DisplayUrl { get; set; }
        [Category("General"), Caption("Edit Url"), Description("The Url to edit a resource - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemoveResources",
                        this.__ResStr("roleRemItemsC", "Remove Resources"), this.__ResStr("roleRemItems", "The role has permission to remove individual resources"),
                        this.__ResStr("userRemItemsC", "Remove Resources"), this.__ResStr("userRemItems", "The user has permission to remove individual resources")),
                };
            }
        }

        public ModuleAction GetAction_Authorizations(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("authLink", "Resources"),
                MenuText = this.__ResStr("authText", "Resources"),
                Tooltip = this.__ResStr("authTooltip", "Display and manage resources"),
                Legend = this.__ResStr("authLegend", "Displays and manages resources"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public ModuleAction GetAction_Remove(string resourceName) {
            if (!IsAuthorized("RemoveResources")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(AuthorizationBrowseModuleEndpoints), AuthorizationBrowseModuleEndpoints.Remove),
                QueryArgs = new { ResourceName = resourceName },
                Image = "#Remove",
                NeedsModuleContext = true,
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove Resource"),
                MenuText = this.__ResStr("removeMenu", "Remove Resource"),
                Tooltip = this.__ResStr("removeTT", "Removes the resource"),
                Legend = this.__ResStr("removeLegend", "Removes the resource"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove resource \"{0}\"?", resourceName),
            };
        }
    }
}
