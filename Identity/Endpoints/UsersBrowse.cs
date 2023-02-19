/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Endpoints {

    public class UsersBrowseModuleEndpoints : YetaWFEndpoints {

        internal const string Remove = "Remove";
        internal const string SendVerificationEmail = "SendVerificationEmail";
        internal const string SendApprovedEmail = "SendApprovedEmail";
        internal const string SendRejectedEmail = "SendRejectedEmail";
        internal const string SendSuspendedEmail = "SendSuspendedEmail";
        internal const string RehashAllPasswords = "RehashAllPasswords";

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(UsersBrowseModuleEndpoints), name, defaultValue, parms); }

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(UsersBrowseModuleEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken();

            group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                ModuleDefinition module = await GetModuleAsync(gridPvData.__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                return await GridSupport.GetGridPartialAsync(context, module, UsersBrowseModuleController.GetGridModel(module), gridPvData);
            });

            group.MapPost(Remove, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string userName) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("RemoveUsers")) return Results.Unauthorized();
                using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                    UserDefinition user = await GetUserAsync(dataProvider, userName);
                    await dataProvider.RemoveItemAsync(userName);
                    return Reload(ReloadEnum.ModuleParts);
                }
            })
                .ExcludeDemoMode();

            group.MapPost(SendVerificationEmail, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string userName) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("SendEmails")) return Results.Unauthorized();
                using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                    UserDefinition user = await GetUserAsync(dataProvider, userName);
                    Emails emails = new Emails();
                    await emails.SendVerificationAsync(user);
                    return Reload(ReloadEnum.ModuleParts, PopupText: __ResStr("verificationSent", "Verification email sent to user {0}.", user.Email));
                }
            })
                .ExcludeDemoMode();

            group.MapPost(SendApprovedEmail, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string userName) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("SendEmails")) return Results.Unauthorized();
                using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                    UserDefinition user = await GetUserAsync(dataProvider, userName);
                    Emails emails = new Emails();
                    await emails.SendApprovalAsync(user);
                    return Reload(ReloadEnum.ModuleParts, PopupText: __ResStr("approvalSent", "Approval email sent to user {0}.", user.Email));
                }
            })
                .ExcludeDemoMode();

            group.MapPost(SendRejectedEmail, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string userName) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("SendEmails")) return Results.Unauthorized();
                using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                    UserDefinition user = await GetUserAsync(dataProvider, userName);
                    Emails emails = new Emails();
                    await emails.SendRejectedAsync(user);
                    return Reload(ReloadEnum.ModuleParts, PopupText: __ResStr("rejectionSent", "Rejection email sent to user {0}.", user.Email));
                }
            })
                .ExcludeDemoMode();

            group.MapPost(SendSuspendedEmail, async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string userName) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("SendEmails")) return Results.Unauthorized();
                using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                    UserDefinition user = await GetUserAsync(dataProvider, userName);
                    Emails emails = new Emails();
                    await emails.SendSuspendedAsync(user);
                    return Reload(ReloadEnum.ModuleParts, PopupText: __ResStr("suspendedSent", "Suspension email sent to user {0}.", user.Email));
                }
            })
                .ExcludeDemoMode();

            group.MapPost(RehashAllPasswords, async (HttpContext context, [FromQuery] Guid __ModuleGuid) => {
                ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
                if (!module.IsAuthorized("SendEmails")) return Results.Unauthorized();
                using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                    await userDP.RehashAllPasswordsAsync();
                    return Reload(ReloadEnum.ModuleParts, PopupText: __ResStr("rehashDone", "All user passwords have been rehashed"));
                }
            })
                .ExcludeDemoMode();
        }

        private static async Task<UserDefinition> GetUserAsync(UserDefinitionDataProvider dataProvider, string userName) {
            if (string.IsNullOrWhiteSpace(userName))
                throw new Error(__ResStr("noItem", "No user name specified"));
            UserDefinition user = await dataProvider.GetItemAsync(userName);
            if (user == null)
                throw new Error(__ResStr("notFoundUser", "User {0} not found.", userName));
            return user;
        }
    }
}
