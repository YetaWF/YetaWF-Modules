/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Pages.Addons;
using YetaWF.Modules.Pages.Components;

namespace YetaWF.Modules.Pages.Endpoints {

    public class TemplateListOfLocalPagesEndpoints : YetaWFEndpoints {

        private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(TemplateListOfLocalPagesEndpoints), name, defaultValue, parms); }

        internal const string AddPage = "AddPage";

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            endpoints.MapPost(GetEndpoint(package, typeof(TemplateListOfLocalPagesEndpoints), AddPage), async (HttpContext context, [FromBody] PartialView.PartialViewData pvData, string data, string fieldPrefix, string newUrl) => {
                // Validation
                UrlValidationAttribute attr = new UrlValidationAttribute(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local);
                if (!attr.IsValid(newUrl))
                    throw new Error(attr.ErrorMessage!);
                List<ListOfLocalPagesEditComponent.Entry> list = Utility.JsonDeserialize<List<ListOfLocalPagesEditComponent.Entry>>(data);
                if ((from l in list where l.Url.ToLower() == newUrl.ToLower() select l).FirstOrDefault() != null)
                    throw new Error(__ResStr("dupUrl", "Page {0} has already been added", newUrl));
                // add new grid record
                ListOfLocalPagesEditComponent.Entry entry = new ListOfLocalPagesEditComponent.Entry {
                    Url = newUrl,
                };
                return await GridSupport.GetGridRecordAsync(context, pvData, await ListOfLocalPagesEditComponent.GridRecordAsync(fieldPrefix, entry));
            })
            .RequireAuthorization()
            .AntiForgeryToken()
            .ExcludeDemoMode()
            .ResourceAuthorize(Info.Resource_AllowListOfLocalPagesAjax);
        }
    }
}
