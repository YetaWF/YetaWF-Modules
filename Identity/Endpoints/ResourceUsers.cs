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
using YetaWF.Modules.Identity.Addons;
using YetaWF.Modules.Identity.Components;

namespace YetaWF.Modules.Identity.Endpoints {

    public class ResourceUsersEndpoints : YetaWFEndpoints {

        internal const string AddUserToResource = "AddUserToResource";

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ResourceUsersEndpoints), name, defaultValue, parms); }

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(ResourceUsersEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken();

            group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                ModuleDefinition module = await GetModuleAsync(gridPvData.__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                return await GridSupport.GetGridPartialAsync(context, module, ResourceUsersEditComponent.GetGridAllUsersModel(), gridPvData);
            })
                .ResourceAuthorize(Info.Resource_AllowListOfUserNamesAjax);

            group.MapPost(GridSupport.DisplaySortFilter, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                return await GridSupport.GetGridPartialAsync<ResourceUsersDisplayComponent.Entry>(context, null,ResourceUsersDisplayComponent.GetGridModel(false), gridPvData);
            });

            group.MapPost(GridSupport.EditSortFilter, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                return await GridSupport.GetGridPartialAsync<ResourceUsersEditComponent.Entry>(context, null, ResourceUsersEditComponent.GetGridModel(false), gridPvData);
            });

            group.MapPost(AddUserToResource, async (HttpContext context, [FromBody] GridSupport.GridAdditionPartialViewData<ResourceUsersEditComponent.Entry> pvData, string fieldPrefix, string newUser) => {
                // validate
                if (string.IsNullOrWhiteSpace(newUser))
                    throw new Error(__ResStr("noParm", "No user name specified"));
                int userId = await Resource.ResourceAccess.GetUserIdAsync(newUser);
                if (userId == 0)
                    throw new Error(__ResStr("noUser", "User {0} doesn't exist.", newUser));
                string userName = await Resource.ResourceAccess.GetUserNameAsync(userId);
                // check duplicate
                if ((from l in pvData.GridData where l.UserId == userId select l).FirstOrDefault() != null)
                    throw new Error(__ResStr("dupUser", "User {0} has already been added", newUser));
                // add new grid record
                return await GridSupport.GetGridRecordAsync(context, pvData, new GridRecordData() {
                    GridDef = ResourceUsersEditComponent.GetGridModel(false),
                    Data = new ResourceUsersEditComponent.Entry(userId, userName),
                    FieldPrefix = fieldPrefix,
                });
            });
        }
    }
}
