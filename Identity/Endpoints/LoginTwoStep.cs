/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Support.TwoStepAuthorization;

namespace YetaWF.Modules.Identity.Endpoints;

public class LoginTwoStepEndpoints : YetaWFEndpoints {

    private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(LoginTwoStepEndpoints), name, defaultValue, parms); }

    internal const string IDENTITY_TWOSTEP_USERID = "YetaWF_Identity-Login-TwoStep";
    internal const string IDENTITY_TWOSTEP_NEXTURL = "YetaWF_Identity-Login-NextUrl";//$$$ needed?

    public const string Login = "Login";

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(LoginTwoStepEndpoints)));

        group.MapGet(Login, async (HttpContext context) => {
            // verify that the user already entered the name/password correctly
            int userId = Manager.SessionSettings.SiteSettings.GetValue<int>(IDENTITY_TWOSTEP_USERID, 0);
            string returnUrl = Manager.SessionSettings.SiteSettings.GetValue<string>(IDENTITY_TWOSTEP_NEXTURL);

            await Resource.ResourceAccess.LoginAsAsync(userId);
            await Resource.ResourceAccess.ClearTwoStepUserAsync();
            if (userId == 0) return Redirect(Manager.CurrentSite.HomePageUrl);

            // verify that the TwoStepAuthorization provider just verified this user
            TwoStepAuth twoStep = new TwoStepAuth();
            if (!await twoStep.VerifyTwoStepAutheticationDoneAsync(userId))
                return Redirect(Manager.CurrentSite.HomePageUrl);
            await twoStep.ClearTwoStepAutheticationAsync(userId);

            await Resource.ResourceAccess.LoginAsAsync(userId);

            returnUrl = QueryHelper.AddRando(returnUrl); // to defeat client-side caching
            return Redirect(returnUrl);
        });
    }
}
