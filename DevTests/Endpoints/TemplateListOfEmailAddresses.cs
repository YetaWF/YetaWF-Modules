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
using YetaWF.Modules.DevTests.Components;

namespace YetaWF.Modules.DevTests.Endpoints {

    public class TemplateListOfEmailAddressesEndpoints : YetaWFEndpoints {

        private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(TemplateListOfEmailAddressesEndpoints), name, defaultValue, parms); }

        internal const string AddEmailAddress = "AddEmailAddress";

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            endpoints.MapPost(GetEndpoint(package, typeof(TemplateListOfEmailAddressesEndpoints), AddEmailAddress), async (HttpContext context, [FromBody] GridSupport.GridAdditionPartialViewData<ListOfEmailAddressesEditComponent.Entry> pvData, string fieldPrefix, string newEmailAddress) => {
                // Validation
                EmailValidationAttribute attr = new EmailValidationAttribute();
                if (!attr.IsValid(newEmailAddress))
                    throw new Error(attr.ErrorMessage ?? "???");
                if ((from l in pvData.GridData where l.EmailAddress.ToLower() == newEmailAddress.ToLower() select l).FirstOrDefault() != null)
                    throw new Error(__ResStr("dupEmail", "Email address {0} has already been added", newEmailAddress));
                // add new grid record
                return await GridSupport.GetGridRecordAsync(context, pvData, new GridRecordData() {
                    GridDef = ListOfEmailAddressesEditComponent.GetGridModel(false),
                    Data = new ListOfEmailAddressesEditComponent.Entry {
                        EmailAddress = newEmailAddress,
                    },
                    FieldPrefix = fieldPrefix,
                });
            })
                .RequireAuthorization()
                .AntiForgeryToken()
                .ExcludeDemoMode();
        }
    }
}
