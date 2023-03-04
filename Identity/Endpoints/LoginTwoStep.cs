/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Support.TwoStepAuthorization;

namespace YetaWF.Modules.Identity.Endpoints;

public class LoginTwoStepEndpoints : YetaWFEndpoints {

    private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(LoginTwoStepEndpoints), name, defaultValue, parms); }

    public const string IDENTITY_TWOSTEP_USERID = "YetaWF_Identity-Login-TwoStep";
    public const string IDENTITY_TWOSTEP_NEXTURL = "YetaWF_Identity-Login-NextUrl";
    public const string IDENTITY_TWOSTEP_CLOSEONLOGIN = "YetaWF_Identity-Login-CloseOnLogin";

    public const string Login = "Login";

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(LoginTwoStepEndpoints)));

        group.MapGet(Login, async (HttpContext context) => {
            // verify that the user already entered the name/password correctly
            int userId = Manager.SessionSettings.SiteSettings.GetValue<int>(IDENTITY_TWOSTEP_USERID, 0);
            bool closeOnLogin = Manager.SessionSettings.SiteSettings.GetValue<bool>(IDENTITY_TWOSTEP_CLOSEONLOGIN, false);
            string returnUrl = Manager.SessionSettings.SiteSettings.GetValue<string>(IDENTITY_TWOSTEP_NEXTURL);

            Manager.SessionSettings.SiteSettings.ClearValue(IDENTITY_TWOSTEP_USERID);
            Manager.SessionSettings.SiteSettings.ClearValue(IDENTITY_TWOSTEP_NEXTURL);
            Manager.SessionSettings.SiteSettings.ClearValue(IDENTITY_TWOSTEP_CLOSEONLOGIN);
            Manager.SessionSettings.SiteSettings.Save();
            if (userId == 0) return Results.Redirect(Manager.CurrentSite.HomePageUrl);

            // verify that the TwoStepAuthorization provider just verified this user
            TwoStepAuth twoStep = new TwoStepAuth();
            if (!await twoStep.VerifyTwoStepAutheticationDoneAsync(userId))
                return Results.Redirect(Manager.CurrentSite.HomePageUrl);
            await twoStep.ClearTwoStepAutheticationAsync(userId);

            await Resource.ResourceAccess.LoginAsAsync(userId);

            if (closeOnLogin)
                throw new NotImplementedException();
                //$$$ return View("YetaWF_Identity_TwoStepDone");
            else {
                returnUrl = QueryHelper.AddRando(returnUrl); // to defeat client-side caching
                return Results.Redirect(returnUrl);
            }
        });
    }
}
