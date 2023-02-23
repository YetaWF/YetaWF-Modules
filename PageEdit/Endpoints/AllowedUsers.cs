/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Endpoints.Filters;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.PageEdit.Components;

namespace YetaWF.Modules.PageEdit.Endpoints;

public class AllowedUsersEndpoints : YetaWFEndpoints {

    internal const string AddUserToPage = "AddUserToPage";

    private static string __ResStr(string name, string defaultValue, params object?[] parms) { return ResourceAccess.GetResourceString(typeof(AllowedUsersEndpoints), name, defaultValue, parms); }

    public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

        RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(AllowedUsersEndpoints)))
            .RequireAuthorization()
            .AntiForgeryToken();

        group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
            ModuleDefinition module = await GetModuleAsync(gridPvData.__ModuleGuid);
            if (!module.IsAuthorized()) return Results.Unauthorized();
            return await GridSupport.GetGridPartialAsync(context, module, AllowedUsersEditComponent.GetGridAllUsersModel(), gridPvData);
        })
            .ResourceAuthorize(YetaWF.Modules.Identity.Addons.Info.Resource_AllowListOfUserNamesAjax);

        group.MapPost(GridSupport.EditSortFilter, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
            return await GridSupport.GetGridPartialAsync<AllowedUsersEditComponent.GridAllowedUser>(context, null, AllowedUsersEditComponent.GetGridModel(false), gridPvData);
        });

        group.MapPost(AddUserToPage, async (HttpContext context, [FromBody] GridSupport.GridAdditionPartialViewData<AllowedUsersEditComponent.GridAllowedUser> pvData, string fieldPrefix, string newUser) => {
            // validate
            if (string.IsNullOrWhiteSpace(newUser))
                throw new Error(__ResStr("noParm", "No user name specified."));
            int userId = await Resource.ResourceAccess.GetUserIdAsync(newUser);
            if (userId == 0)
                throw new Error(__ResStr("noUser", "User {0} doesn't exist", newUser));

            // check duplicate
            if ((from l in pvData.GridData where l.UserId == userId select l).FirstOrDefault() != null)
                throw new Error(__ResStr("dupUser", "User {0} has already been added", newUser));

            // add new grid record
            string userName = await Resource.ResourceAccess.GetUserNameAsync(userId);
            AllowedUsersEditComponent.GridAllowedUser userEntry = new AllowedUsersEditComponent.GridAllowedUser(userId, userName);
            return await GridSupport.GetGridRecordAsync(context, pvData, new GridRecordData() {
                GridDef = AllowedUsersEditComponent.GetGridModel(false),
                Data = userEntry,
                FieldPrefix = fieldPrefix,
            });
        });
    }
}
