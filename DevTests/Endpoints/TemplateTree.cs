/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Packages;
using YetaWF.Modules.DevTests.Modules;

namespace YetaWF.Modules.DevTests.Endpoints {

    public class TemplateTreeModuleEndpoints : YetaWFEndpoints {

        internal const string GetRecords = "GetRecords";

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(TemplateTreeModuleEndpoints)))
                .AntiForgeryToken();

            group.MapPost(TreeSupport.GetRecords, async (HttpContext context, [FromBody] TreeSupport.TreeAdditionPartialViewData<TemplateTreeModule.EntryElement> tvData) => {
                return await TreeSupport.GetTreePartialAsync<TemplateTreeModule.EntryElement>(context, null, tvData, TemplateTreeModule.GetTreeModel(), TemplateTreeModule.GetDynamicSubEntries());
            });
        }
    }
}
