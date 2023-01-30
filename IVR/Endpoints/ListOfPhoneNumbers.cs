/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Softelvdm.Modules.IVR.Components;
using System.Linq;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace Softelvdm.Modules.IVR.Endpoints {

    public class ListOfPhoneNumbersEndpoints : YetaWFEndpoints {

        internal const string AddPhoneNumber = "AddPhoneNumber";

        private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(ListOfPhoneNumbersEndpoints), name, defaultValue, parms); }

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(ListOfPhoneNumbersEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken();

            group.MapPost(GridSupport.DisplaySortFilter, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                return await GridSupport.GetGridPartialAsync<ListOfPhoneNumbersDisplayComponent.Entry>(context, null, ListOfPhoneNumbersDisplayComponent.GetGridModel(false, false, false), gridPvData);
            });

            group.MapPost(GridSupport.EditSortFilter, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                return await GridSupport.GetGridPartialAsync<ListOfPhoneNumbersEditComponent.Entry>(context, null, ListOfPhoneNumbersEditComponent.GetGridModel(false), gridPvData);
            });

            group.MapPost(AddPhoneNumber, async (HttpContext context, [FromBody] GridSupport.GridAdditionPartialViewData<ListOfPhoneNumbersEditComponent.Entry> pvData, string fieldPrefix, string newPhoneNumber, bool sms) => {
                // Validation
                string? phoneNumber = PhoneNumberNationalValidationAttribute.GetE164(newPhoneNumber);
                if (string.IsNullOrWhiteSpace(phoneNumber))
                    throw new Error(__ResStr("invPhone", "Phone number {0} is not a valid phone number", newPhoneNumber));
                if ((from l in pvData.GridData where l.PhoneNumber == phoneNumber select l).FirstOrDefault() != null)
                    throw new Error(__ResStr("dupPhone", "Phone number {0} has already been added", newPhoneNumber));
                // add new grid record
                return await GridSupport.GetGridRecordAsync(context, pvData, new GridRecordData() {
                    GridDef = ListOfPhoneNumbersEditComponent.GetGridModel(false),
                    Data = new ListOfPhoneNumbersEditComponent.Entry(phoneNumber, sms),
                    FieldPrefix = fieldPrefix,
                });
            })
                .ExcludeDemoMode();
        }
    }
}
