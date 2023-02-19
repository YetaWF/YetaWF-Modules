/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Modules.Feedback.DataProvider;
using YetaWF.Modules.Feedback.Modules;

namespace YetaWF.Modules.Feedback.Endpoints;

public class FeedbackBrowseModuleEndpoints : YetaWFEndpoints {

    internal const string Remove = "Remove";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(FeedbackBrowseModuleEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(FeedbackBrowseModuleEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
            FeedbackBrowseModule module = await GetModuleAsync<FeedbackBrowseModule>(gridPvData.__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            return await GridSupport.GetGridPartialAsync(context, module, module.GetGridModel(), gridPvData);
        });

        group.MapPost($"{Remove}/{{id}}", async (HttpContext context, [FromQuery] Guid __ModuleGuid, [FromRoute] int id) => {
            ModuleDefinition module = await GetModuleAsync(__ModuleGuid);
            if (!module.IsAuthorized("RemoveFeedback")) return Results.Unauthorized();
            using (FeedbackDataProvider dataProvider = new FeedbackDataProvider()) {
                await dataProvider.RemoveItemAsync(id);
                return Reload(ReloadEnum.ModuleParts);
            }
        })
            .ExcludeDemoMode();
    }
}
