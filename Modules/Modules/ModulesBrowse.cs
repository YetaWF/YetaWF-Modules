/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Modules#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Modules.Endpoints;

namespace YetaWF.Modules.Modules.Modules {

    public class ModulesBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, ModulesBrowseModule>, IInstallableModel { }

    [ModuleGuid("{276dcecf-f890-4c54-b010-679ee58f0034}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    public class ModulesBrowseModule : ModuleDefinition {

        public ModulesBrowseModule() {
            Title = this.__ResStr("modTitle", "Modules");
            Name = this.__ResStr("modName", "Modules");
            Description = this.__ResStr("modSummary", "Displays and manages modules and implements module removal. It is also used to display, edit and remove modules. It is accessible using Admin > Panel > Modules (standard YetaWF site).");
            DefaultViewName = StandardViews.Browse;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new ModulesBrowseModuleDataProvider(); }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemoveItems",
                        this.__ResStr("roleRemItemsC", "Remove Modules"), this.__ResStr("roleRemItems", "The role has permission to remove individual modules"),
                        this.__ResStr("userRemItemsC", "Remove Modules"), this.__ResStr("userRemItems", "The user has permission to remove individual modules")),
                    new RoleDefinition("SetAuthorization",
                        this.__ResStr("roleSetAuthC", "Set Global Authorization"), this.__ResStr("roleSetAuth", "The role has permission to set global authorization for all modules"),
                        this.__ResStr("userSetAuthC", "Set Global Authorization"), this.__ResStr("userSetAuth", "The user has permission to set global authorization for all modules")),
                    new RoleDefinition("RestoreAuthorization",
                        this.__ResStr("roleRestAuthC", "Restore Default Authorization"), this.__ResStr("roleRestAuth", "The role has permission to restore the default authorizations for ALL modules"),
                        this.__ResStr("userRestAuthC", "Restore Default Authorization"), this.__ResStr("userRestAuth", "The user has permission to restore the default authorizations for ALL modules")),
                };
            }
        }

        public ModuleAction? GetAction_Modules(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("browseLink", "Modules"),
                MenuText = this.__ResStr("browseText", "Modules"),
                Tooltip = this.__ResStr("browseTooltip", "Display and manage modules"),
                Legend = this.__ResStr("browseLegend", "Displays and manages modules"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }
        public ModuleAction? GetAction_ShowModule(Guid modGuid) {
            return new ModuleAction(this) {
                Url = ModuleDefinition.GetModulePermanentUrl(modGuid),
                Image = "#Display",
                Style = ModuleAction.ActionStyleEnum.NewWindow,
                LinkText = this.__ResStr("displayLink", "Show Module"),
                MenuText = this.__ResStr("displayMenu", "Show Module"),
                Tooltip = this.__ResStr("displayTT", "Display the module in a new window"),
                Legend = this.__ResStr("displayLegend", "Displays the module in a new window"),
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                SaveReturnUrl = true,
                DontFollow = true,
            };
        }
        public ModuleAction? GetAction_Remove(Guid moduleGuid) {
            if (!IsAuthorized("RemoveItems")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(ModulesBrowseModuleEndpoints), ModulesBrowseModuleEndpoints.Remove),
                NeedsModuleContext = true,
                QueryArgs = new { ModuleGuid = moduleGuid },
                Image = "#Remove",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove Module"),
                MenuText = this.__ResStr("removeMenu", "Remove Module"),
                Tooltip = this.__ResStr("removeTT", "Remove the module permanently - The module and its data are PERMANENTLY deleted and can no longer be used on any pages"),
                Legend = this.__ResStr("removeLegend", "Removes the module permanently - The module and its data are PERMANENTLY deleted and can no longer be used on any pages"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove this module PERMANENTLY?"),
            };
        }
        public ModuleAction? GetAction_RestoreAllDefaultAuthorization() {
            if (!IsAuthorized("RestoreAuthorization")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(ModulesBrowseModuleEndpoints), ModulesBrowseModuleEndpoints.RestoreAuthorization),
                NeedsModuleContext = true,
                QueryArgs = new { },
                Image = "",
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("restAuthLink", "Restore Default Authorization"),
                MenuText = this.__ResStr("restAuthMenu", "Restore Default Authorization"),
                Tooltip = this.__ResStr("restAuthTT", "DEVELOPMENT FEATURE - Restore the default authorization for all modules"),
                Legend = this.__ResStr("restAuthLegend", "DEVELOPMENT FEATURE - Restores the default authorization for all modules"),
                Category = ModuleAction.ActionCategoryEnum.Significant,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                ConfirmationText = this.__ResStr("restAuthConfirm", "DEVELOPMENT FEATURE - Are you sure you want to restore the default authorizations for ALL modules - This will reset all modules to their \"factory\" authorization?"),
                PleaseWaitText = this.__ResStr("restAuthPlsWait", "Restoring default authorization for all modules..."),
            };
        }
#if DEBUG
        public async Task<ModuleAction?> GetAction_SetSuperuserAsync(Guid guid) {
            if (!IsAuthorized("SetAuthorization")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(ModulesBrowseModuleEndpoints), ModulesBrowseModuleEndpoints.SetSuperuser),
                NeedsModuleContext = true,
                QueryArgs = new { Guid = guid },
                Image = await CustomIconAsync("go.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = "Superuser Authorization",
                MenuText = "Superuser Authorization",
                Tooltip = "Change module to superuser only access",
                Legend = "Changes a module to superuser only access",
                Category = ModuleAction.ActionCategoryEnum.Significant,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            };
        }
        public async Task<ModuleAction?> GetAction_SetAdminAsync(Guid guid) {
            if (!IsAuthorized("SetAuthorization")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(ModulesBrowseModuleEndpoints), ModulesBrowseModuleEndpoints.SetAdmin),
                NeedsModuleContext = true,
                QueryArgs = new { Guid = guid },
                Image = await CustomIconAsync("go.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = "Admin Authorization",
                MenuText = "Admin Authorization",
                Tooltip = "Change module to admin only access",
                Legend = "Changes a module to admin only access",
                Category = ModuleAction.ActionCategoryEnum.Significant,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            };
        }
        public async Task<ModuleAction?> GetAction_SetUserAsync(Guid guid) {
            if (!IsAuthorized("SetAuthorization")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(ModulesBrowseModuleEndpoints), ModulesBrowseModuleEndpoints.SetUser),
                NeedsModuleContext = true,
                QueryArgs = new { Guid = guid },
                Image = await CustomIconAsync("go.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = "User Authorization",
                MenuText = "User Authorization",
                Tooltip = "Change module to user only access",
                Legend = "Changes a module to user only access",
                Category = ModuleAction.ActionCategoryEnum.Significant,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            };
        }
        public async Task<ModuleAction?> GetAction_SetAnonymousAsync(Guid guid) {
            if (!IsAuthorized("SetAuthorization")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(ModulesBrowseModuleEndpoints), ModulesBrowseModuleEndpoints.SetAnonymous),
                NeedsModuleContext = true,
                QueryArgs = new { Guid = guid },
                Image = await CustomIconAsync("go.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = "Anonymous Authorization",
                MenuText = "Anonymous Authorization",
                Tooltip = "Change module to anonymous only access",
                Legend = "Changes a module to anonymous only access",
                Category = ModuleAction.ActionCategoryEnum.Significant,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
            };
        }
#endif
    }
}