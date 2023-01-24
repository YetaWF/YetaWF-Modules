/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Packages;

namespace YetaWF.Modules.Basics.Endpoints {

    public class AlertDisplayModuleEndpoints : YetaWFEndpoints {

        public const string Off = "Off";

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            endpoints.MapPost(GetEndpoint(package, typeof(AlertDisplayModuleEndpoints), Off), (HttpContext context) => {
                Manager.SessionSettings.SiteSettings.SetValue<bool>("YetaWF_Basics_AlertDone", true);
                Manager.SessionSettings.SiteSettings.Save();
                return Results.Ok();
            });
        }
    }
}
