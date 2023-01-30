/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Packages;
using YetaWF.Modules.DevTests.Controllers;

namespace YetaWF.Modules.DevTests.Endpoints {

    public class TemplateTreeModuleEndpoints : YetaWFEndpoints {

        internal const string GetRecords = "GetRecords";

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(TemplateTreeModuleEndpoints)))
                .AntiForgeryToken();

            group.MapPost(TreeSupport.GetRecords, async (HttpContext context, [FromBody] TreeSupport.TreeAdditionPartialViewData<TemplateTreeModuleController.EntryElement> tvData) => {
                return await TreeSupport.GetTreePartialAsync<TemplateTreeModuleController.EntryElement>(context, null, tvData, TemplateTreeModuleController.GetTreeModel(), TemplateTreeModuleController.GetDynamicSubEntries());
            });
        }
    }
}
