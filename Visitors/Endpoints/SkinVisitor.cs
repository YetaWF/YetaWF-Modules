/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Packages;
using YetaWF.Modules.Visitors.DataProvider;

namespace YetaWF.Modules.Visitors.Endpoints {

    public class SkinVisitorModuleEndpoints : YetaWFEndpoints {

        internal const string TrackClick = "TrackClick";

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(SkinVisitorModuleEndpoints)))
                .AntiForgeryToken(); // always force antiforgery to avoid cross-site attacks exploiting flooding click tracking

            group.MapPost(TrackClick, (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromQuery] string url) => {
                VisitorEntryDataProvider.AddVisitEntryUrlAsync(url, true); // no await, as in fire and forget
                return Results.Json("", null, "application/json");
            });
        }
    }
}
