/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Panels.Components;
using YetaWF.Modules.Panels.DataProvider;

namespace YetaWF.Modules.Panels.Endpoints {

    public class PageBarModuleEndpoints : YetaWFEndpoints {

        private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(PageBarModuleEndpoints), name, defaultValue, parms); }

        internal const string AddPage = "AddPage";
        internal const string SaveExpandCollapse = "SaveExpandCollapse";

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(PageBarModuleEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken();

            group.MapPost(AddPage, async (HttpContext context, [FromBody] GridSupport.GridAdditionPartialViewData<ListOfLocalPagesEditComponent.Entry> pvData, string fieldPrefix, string newUrl) => {
                // Validation
                UrlValidationAttribute attr = new UrlValidationAttribute(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local);
                if (!attr.IsValid(newUrl))
                    throw new Error(attr.ErrorMessage!);
                if ((from l in pvData.GridData where l.Url.ToLower() == newUrl.ToLower() select l).FirstOrDefault() != null)
                    throw new Error(__ResStr("dupUrl", "Page {0} has already been added", newUrl));
                // add new grid record
                return await GridSupport.GetGridRecordAsync(context, pvData, new GridRecordData() {
                    GridDef = ListOfLocalPagesEditComponent.GetGridModel(false),
                    Data = new ListOfLocalPagesEditComponent.Entry {
                        Url = newUrl,
                    },
                    FieldPrefix = fieldPrefix,
                });
            })
                .RequireAuthorization()
                .AntiForgeryToken()
                .ExcludeDemoMode();


            RouteGroupBuilder groupSimple = endpoints.MapGroup(GetPackageApiRoute(package, typeof(PageBarModuleEndpoints)));

            groupSimple.MapPost(SaveExpandCollapse, (HttpContext context, bool expanded) => {
                PageBarDataProvider.SaveExpanded(expanded);
                return Results.Json("", null, "application/json");
            });

        }
    }
}
