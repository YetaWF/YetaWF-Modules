/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Linq;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Addons;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Endpoints;

public class LoginDirectEndpoints : YetaWFEndpoints {

    private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(LoginDirectEndpoints), name, defaultValue, parms); }

    public const string LoginAs = "LoginAs";
    public const string LoginDirectDemoUser = "LoginDirectDemoUser";
    public const string Logoff = "Logoff";
    public const string LogoffDirect = "LogoffDirect";

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(LoginDirectEndpoints)));

        group.MapGet(LoginAs, async (HttpContext context, int userId) => {
            Package package = AreaRegistration.CurrentPackage;
            await Resource.ResourceAccess.LoginAsAsync(userId);
            string url = Manager.CurrentSite.HomePageUrl;
            url = QueryHelper.AddRando(url); // to defeat client-side caching
            return Redirect(url);
        })
            .ResourceAuthorize(Info.Resource_AllowUserLogon);

        group.MapPost(LoginDirectDemoUser, async (HttpContext context, string name, string url) => {
            url = QueryHelper.AddRando(url ?? Manager.CurrentSite.HomePageUrl); // to defeat client-side caching

            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemAsync(name);
                if (user == null || !user.RolesList.Contains(new Role { RoleId = Resource.ResourceAccess.GetUserDemoRoleId() }, new RoleComparer())) {
                    Manager.CurrentResponse.StatusCode = 401;
                } else {
                    await Resource.ResourceAccess.LoginAsAsync(user.UserId);
                }
                return Redirect(url);
            }
        });

        group.MapGet(Logoff, async (HttpContext context, string nextUrl) => {
            Manager.SetSuperUserRole(false);// explicit logoff clears superuser state
            await LoginModule.UserLogoffAsync();
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            string url = nextUrl;
            if (string.IsNullOrWhiteSpace(url))
                url = config.LoggedOffUrl;
            if (string.IsNullOrWhiteSpace(url))
                url = Manager.CurrentSite.HomePageUrl;
            url = QueryHelper.AddRando(url); // to defeat client-side caching
            return Redirect(url);
        });

        group.MapPost(LogoffDirect, async (HttpContext context, string nextUrl) => {
            Manager.SetSuperUserRole(false);// explicit logoff clears superuser state
            await LoginModule.UserLogoffAsync();
            return Results.Ok();
        });
    }
}
