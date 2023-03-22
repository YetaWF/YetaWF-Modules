/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Modules.Packages.DataProvider;

namespace YetaWF.Modules.Packages.Endpoints;

public class InitialLogFileEndpoints : YetaWFEndpoints {

    internal const string GetInitialInstallLogRecords = "GetInitialInstallLogRecords";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(InitialLogFileEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(InitialLogFileEndpoints)));

        group.MapPost(GetInitialInstallLogRecords, async (HttpContext context, int offset) => {
            PackagesDataProvider.RetrieveInitialInstallLogInfo info = await PackagesDataProvider.RetrieveInitialInstallLogAsync();
            if (info.Ended) {
                info.Lines.AddRange(new List<string> {
                "*** This site has to be restarted now so the new settings can be activated ***",
                "*** DONE. PLEASE CLOSE YOUR BROWSER AND RESTART YOUR SITE FROM VISUAL STUDIO ***",
                "+++DONE",
            });
            } else {
                info.Lines.RemoveRange(0, Math.Min(offset, info.Lines.Count));
            }
            return Results.Json(info.Lines);
        });
    }
}
