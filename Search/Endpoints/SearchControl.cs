/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.Search.Endpoints;

public class SearchControlModuleEndpoints : YetaWFEndpoints {

    internal static string Switch = "Switch";

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(SearchControlModuleEndpoints)));

        group.MapPost(Switch, async (HttpContext context, [FromQuery] Guid __ModuleGuid, bool value) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            Manager.SessionSettings.SiteSettings.SetValue<bool>("YetaWF_SearchControl_Highlight", value);
            Manager.SessionSettings.SiteSettings.Save();
            return Results.Json("", null, "application/json");
        });
    }
}
