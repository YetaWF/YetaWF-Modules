/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Modules.Controllers;

namespace YetaWF.Modules.Modules.Endpoints {

    public class ModulesBrowseModuleEndpoints : YetaWFEndpoints {

        internal const string Remove = "Remove";
        internal const string RestoreAuthorization = "RestoreAuthorization";
        internal const string SetSuperuser = "SetSuperuser";
        internal const string SetAdmin = "SetAdmin";
        internal const string SetUser = "SetUser";
        internal const string SetAnonymous = "SetAnonymous";

        private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(ModulesBrowseModuleEndpoints), name, defaultValue, parms); }

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(ModulesBrowseModuleEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken();

            group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                ModuleDefinition module = await GetModuleAsync(gridPvData.__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                return await GridSupport.GetGridPartialAsync(context, module, ModulesBrowseModuleController.GetGridModel(module), gridPvData);
            });

            group.MapPost(RestoreAuthorization, async (HttpContext context, [FromQuery] Guid __ModuleGuid) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("RemoveItems")) return Results.Unauthorized();

                YetaWF.Core.IO.Module.ModuleBrowseInfo info = new YetaWF.Core.IO.Module.ModuleBrowseInfo();
                await YetaWF.Core.IO.Module.GetModulesAsync(info);
                if (info.Modules != null) {
                    foreach (ModuleDefinition genericMod in info.Modules) {
                        ModuleDefinition? mod = await ModuleDefinition.LoadAsync(genericMod.ModuleGuid, AllowNone: true);
                        if (mod != null) {
#if MERGE // enable this to preserve anon and user settings
                        int anonRoleId = Resource.ResourceAccess.GetAnonymousRoleId();
                        int userRoleId = Resource.ResourceAccess.GetUserRoleId();
                        ModuleDefinition.AllowedRole? anonRole = ModuleDefinition.AllowedRole.Find(mod.DefaultAllowedRoles, anonRoleId);
                        ModuleDefinition.AllowedRole? userRole = ModuleDefinition.AllowedRole.Find(mod.DefaultAllowedRoles, userRoleId);
                        if (anonRole == null && userRole == null) {
                            // merge default roles into allowed roles to preserve current anon & user settings
                            anonRole = ModuleDefinition.AllowedRole.Find(mod.AllowedRoles, anonRoleId);
                            userRole = ModuleDefinition.AllowedRole.Find(mod.AllowedRoles, userRoleId);
                            mod.AllowedRoles = new SerializableList<ModuleDefinition.AllowedRole>(mod.DefaultAllowedRoles);
                            if (anonRole != null)
                                mod.AllowedRoles.Add(anonRole);
                            if (userRole != null)
                                mod.AllowedRoles.Add(userRole);
                        } else {
                            mod.AllowedRoles = mod.DefaultAllowedRoles;
                        }
#else
                            mod.AllowedRoles = mod.DefaultAllowedRoles;
#endif
                            //mod.AllowedUsers = // we're not touching this

                            await mod.SaveAsync();
                        }
                    }
                }
                return Reload(ReloadEnum.ModuleParts);
            })
                .ExcludeDemoMode();

            group.MapPost(Remove, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] Guid moduleGuid) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("RemoveItems")) return Results.Unauthorized();
                if (!await YetaWF.Core.IO.Module.RemoveModuleDefinitionAsync(moduleGuid))
                    throw new Error(__ResStr("errRemove", "The module could not be removed - It may already have been deleted"));
                return Reload(ReloadEnum.ModuleParts);
            })
                .ExcludeDemoMode();

#if DEBUG
            group.MapPost(SetSuperuser, async (HttpContext context, [FromQuery] Guid __ModuleGuid, Guid guid) => {
                return await SetAuthorization(__ModuleGuid, guid, (allowedRoles, anonRole, userRole, adminRole) => { 
                });
            })
                .ExcludeDemoMode();

            group.MapPost(SetAdmin, async (HttpContext context, [FromQuery] Guid __ModuleGuid, Guid guid) => {
                return await SetAuthorization(__ModuleGuid, guid, (allowedRoles, anonRole, userRole, adminRole) => {
                    allowedRoles.Add(new ModuleDefinition.AllowedRole { RoleId = adminRole, View = ModuleDefinition.AllowedEnum.Yes });
                });
            })
                .ExcludeDemoMode();

            group.MapPost(SetUser, async (HttpContext context, [FromQuery] Guid __ModuleGuid, Guid guid) => {
                return await SetAuthorization(__ModuleGuid, guid, (allowedRoles, anonRole, userRole, adminRole) => {
                    allowedRoles.Add(new ModuleDefinition.AllowedRole { RoleId = userRole, View = ModuleDefinition.AllowedEnum.Yes });
                });
            })
                .ExcludeDemoMode();

            group.MapPost(SetAnonymous, async (HttpContext context, [FromQuery] Guid __ModuleGuid, Guid guid) => {
                return await SetAuthorization(__ModuleGuid, guid, (allowedRoles, anonRole, userRole, adminRole) => {
                    allowedRoles.Add(new ModuleDefinition.AllowedRole { RoleId = userRole, View = ModuleDefinition.AllowedEnum.Yes });
                    allowedRoles.Add(new ModuleDefinition.AllowedRole { RoleId = anonRole, View = ModuleDefinition.AllowedEnum.Yes });
                });
            })
                .ExcludeDemoMode();
#endif
        }

        private static async Task<IResult> SetAuthorization(Guid __ModuleGuid, Guid guid, Action<SerializableList<ModuleDefinition.AllowedRole>, int, int, int> action) {
            ModuleDefinition mod= await GetModuleAsync(__ModuleGuid);
            if (!mod.IsAuthorized("SetAuthorization")) return Results.Unauthorized();
            ModuleDefinition? module = await ModuleDefinition.LoadAsync(guid, AllowNone: true);
            if (module == null)
                throw new InternalError($"Couldn't load module {guid}");
            int adminRole = Resource.ResourceAccess.GetAdministratorRoleId();
            int userRole = Resource.ResourceAccess.GetUserRoleId();
            int anonRole = Resource.ResourceAccess.GetAnonymousRoleId();
            module.AllowedRoles = new SerializableList<ModuleDefinition.AllowedRole>();
            action(module.AllowedRoles, anonRole, userRole, adminRole);
            await module.SaveAsync();
            return Reload(ReloadEnum.ModuleParts);
        }
    }
}
