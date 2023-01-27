/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using YetaWF.Core.Addons;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Packages;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ComponentsHTML.Endpoints {

    /// <summary>
    /// Endpoints for the Skin template.
    /// </summary>
    public class SkinEndpoints : YetaWFEndpoints {

        internal const string GetSkins = "GetSkins";

        internal class Lists {
            public string PagesHTML { get; set; } = null!;
            public string PopupsHTML { get; set; } = null!;
        }

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageRoute(package, typeof(SkinEndpoints)))
                .RequireAuthorization()
                .ResourceAuthorize(CoreInfo.Resource_ModuleLists);

            // Returns data to replace a dropdownlist's data with new modules given a package name.
            group.MapPost(GetSkins, (HttpContext context, string skinCollection) => {
                Lists lists = new Lists {
                    PagesHTML = SkinNamePageEditComponent.RenderReplacementSkinsForCollection(skinCollection),
                    PopupsHTML = SkinNamePopupEditComponent.RenderReplacementSkinsForCollection(skinCollection),
                };
                return Results.Json(lists);
            });
        }
    }
}
