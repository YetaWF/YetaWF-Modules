/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.PageEdit.Endpoints;

public class PageEditModuleEndpoints : YetaWFEndpoints {

    internal const string RemovePage = "RemovePage";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(PageEditModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(PageEditModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(RemovePage, async (HttpContext context, Guid __ModuleGuid, Guid pageGuid) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            PageDefinition? page = page = await PageDefinition.LoadAsync(pageGuid);
            if (page == null)
                throw new InternalError("Page {0} does not exist", pageGuid);
            if (!page.IsAuthorized_Remove())
                return Results.Unauthorized();
            await PageDefinition.RemovePageDefinitionAsync(pageGuid);
            return Redirect(Manager.CurrentSite.HomePageUrl);
        })
            .ExcludeDemoMode();
    }
}
