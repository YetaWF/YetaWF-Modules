/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;
using YetaWF.Modules.Identity.Modules;
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Endpoints;

public class LoginModuleEndpoints : YetaWFEndpoints {

    private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(LoginModuleEndpoints), name, defaultValue, parms); }

    public const string LoginDirectGet = "LoginDirectGet";
    public const string ResendVerificationEmail = "ResendVerificationEmail";

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(LoginModuleEndpoints)));

        group.MapGet(LoginDirectGet, async (HttpContext context, string name, string password, Guid security) => {
            Package package = AreaRegistration.CurrentPackage;
            Guid batchKey = WebConfigHelper.GetValue<Guid>(package.AreaName, "BatchKey");
            if (batchKey != security)
                return Results.Unauthorized();

            CompleteLoginModel model = new CompleteLoginModel {
                UserName = name,
                Password = password
            };

            await CompleteLoginAsync(model);
            if (!model.Success)
                return Results.Unauthorized();
            return Results.Ok();
        })
            .ExcludeDemoMode();

        group.MapGet(ResendVerificationEmail, async (HttpContext context, string userName) => {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                if (string.IsNullOrWhiteSpace(userName))
                    throw new Error(__ResStr("noItem", "Unable to send verification email (Reason Code 1)"));
                UserDefinition user = await dataProvider.GetItemAsync(userName);
                if (user == null)
                    throw new Error(__ResStr("notFoundUser", "Unable to send verification email (Reason Code 2)"));
                if (user.UserStatus != UserStatusEnum.NeedValidation)
                    throw new Error(__ResStr("noNeed", "Unable to send verification email (Reason Code 3)"));
                Emails emails = new Emails();
                await emails.SendVerificationAsync(user);
                return Reload(ReloadEnum.ModuleParts, PopupText: __ResStr("verificationSent", "Verification email sent."));
            }
        })
            .ExcludeDemoMode();

    }

    public class CompleteLoginModel {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Success { get; set; }
    }

    private static async Task CompleteLoginAsync(CompleteLoginModel model) {

        model.Success = false;

        // make sure it's a valid user
        UserDefinition user = await Managers.GetUserManager().FindByNameAsync(model.UserName);
        if (user == null) {
            Logging.AddErrorLog("User login failed: {0} - no such user", model.UserName);
            return;
        }
        using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
            if (await logInfoDP.IsExternalUserAsync(user.UserId))
                throw new Error(__ResStr("extUser", "This account can only be accessed using an external login provider"));
        }

        UserDefinition foundUser = user;
        user = null;

        // Handle random super user password
        if (foundUser.UserId == SuperuserDefinitionDataProvider.SuperUserId && SuperuserDefinitionDataProvider.SuperuserAvailable && SuperuserDefinitionDataProvider.SuperUserPasswordRandom &&
                model.UserName == SuperuserDefinitionDataProvider.SuperUserName && model.Password == SuperuserDefinitionDataProvider.SuperUserPassword) {
            user = foundUser;
        }

        if (user == null) {
            user = await Managers.GetUserManager().FindByNameAsync(model.UserName);
            if (string.IsNullOrWhiteSpace(model.Password) || !await Managers.GetUserManager().CheckPasswordAsync(user, model.Password))
                user = null;
        }
        if (user == null) {
            Logging.AddErrorLog("User login failed: {0}, {1}", model.UserName, model.Password);
            return;
        }

        if (user.UserStatus != UserStatusEnum.Approved)
            throw new Error(__ResStr("notApproved", "Your account is not an approved account."));

        await LoginModule.UserLoginAsync(user, false);
        model.Success = true;
    }
}
