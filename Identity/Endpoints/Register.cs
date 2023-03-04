/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Endpoints;

public class RegisterModuleEndpoints : YetaWFEndpoints {

    internal const string Approve = "Approve";
    internal const string Reject = "Reject";
    internal const string Suspend = "Suspend";

    private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(RegisterModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(RegisterModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(Approve, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string userName) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("ChangeAccounts")) return Results.Unauthorized();
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = await GetUserAsync(userName, dataProvider);
                if (user.UserStatus != UserStatusEnum.Approved) {
                    if (user.UserStatus != UserStatusEnum.NeedApproval)
                        throw new Error(__ResStr("notCantApprove", "User {0} is no longer new and cannot be approved.", userName));
                    user.UserStatus = UserStatusEnum.Approved;
                    UpdateStatusEnum status = await dataProvider.UpdateItemAsync(user);
                    switch (status) {
                        default:
                        case UpdateStatusEnum.NewKeyExists:
                            throw new InternalError("Unexpected status {0}", status);
                        case UpdateStatusEnum.RecordDeleted:
                            throw new Error(__ResStr("approveUserNotFound", "The user account for user {0} no longer exists.", userName));
                        case UpdateStatusEnum.OK:
                            break;
                    }
                }
                Emails emails = new Emails();
                await emails.SendApprovalAsync(user);
                return Reload(ReloadEnum.ModuleParts, PopupText: __ResStr("userApproved", "The user account for user {0} has been marked as approved. An email has been sent to the user.", userName));
            }
        })
        .ExcludeDemoMode();

        group.MapPost(Reject, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string userName) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("ChangeAccounts")) return Results.Unauthorized();
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = await GetUserAsync(userName, dataProvider);
                if (user.UserStatus != UserStatusEnum.Rejected) {
                    user.UserStatus = UserStatusEnum.Rejected;
                    UpdateStatusEnum status = await dataProvider.UpdateItemAsync(user);
                    switch (status) {
                        default:
                        case UpdateStatusEnum.NewKeyExists:
                            throw new InternalError("Unexpected status {0}", status);
                        case UpdateStatusEnum.RecordDeleted:
                            throw new Error(__ResStr("rejectUserNotFound", "The user account for user {0} no longer exists.", userName));
                        case UpdateStatusEnum.OK:
                            break;
                    }
                }
                Emails emails = new Emails();
                await emails.SendRejectedAsync(user);

                return Reload(ReloadEnum.ModuleParts, PopupText: __ResStr("userRejected", "The user account for user {0} has been marked as rejected. An email has been sent to the user.", userName));
            }
        })
            .ExcludeDemoMode();

        group.MapPost(Suspend, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string userName) => {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = await GetUserAsync(userName, dataProvider);
                if (user.UserStatus != UserStatusEnum.Suspended) {
                    user.UserStatus = UserStatusEnum.Suspended;
                    UpdateStatusEnum status = await dataProvider.UpdateItemAsync(user);
                    switch (status) {
                        default:
                        case UpdateStatusEnum.NewKeyExists:
                            throw new InternalError("Unexpected status {0}", status);
                        case UpdateStatusEnum.RecordDeleted:
                            throw new Error(__ResStr("rejectUserNotFound", "The user account for user {0} no longer exists.", userName));
                        case UpdateStatusEnum.OK:
                            break;
                    }
                }
                Emails emails = new Emails();
                await emails.SendSuspendedAsync(user);

                return Reload(ReloadEnum.ModuleParts, PopupText: __ResStr("userSuspended", "The user account for user {0} has been marked as suspended. An email has been sent to the user.", userName));
            }
        })
            .ExcludeDemoMode();
    }

    private static async Task<UserDefinition> GetUserAsync(string userName, UserDefinitionDataProvider dataProvider) {
        if (string.IsNullOrWhiteSpace(userName))
            throw new Error(__ResStr("noItem", "No user name specified"));
        UserDefinition user = await dataProvider.GetItemAsync(userName);
        if (user == null)
            throw new Error(__ResStr("notFoundUser", "User {0} not found.", userName));
        return user;
    }
}
