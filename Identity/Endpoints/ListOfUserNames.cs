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
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Addons;
using YetaWF.Modules.Identity.Components;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Endpoints {

    public class ListOfUserNamesEndpoints : YetaWFEndpoints {

        internal const string AddUserName = "AddUserName";

        private static string __ResStr(string name, string defaultValue, params object[] parms) { return ResourceAccess.GetResourceString(typeof(ListOfUserNamesEndpoints), name, defaultValue, parms); }

        public static void RegisterEndpoints(IEndpointRouteBuilder endpoints, Package package, string areaName) {

            RouteGroupBuilder group = endpoints.MapGroup(GetPackageApiRoute(package, typeof(ListOfUserNamesEndpoints)))
                .RequireAuthorization()
                .AntiForgeryToken();

            group.MapPost(GridSupport.BrowseGridData, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                ModuleDefinition module = await GetModuleAsync(gridPvData.__ModuleGuid);
                if (!module.IsAuthorized()) return Results.Unauthorized();
                return await GridSupport.GetGridPartialAsync(context, module, ListOfUserNamesEditComponent.GetGridAllUsersModel(), gridPvData);
            })
                .ResourceAuthorize(Info.Resource_AllowListOfUserNamesAjax);

            group.MapPost(GridSupport.DisplaySortFilter, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                return await GridSupport.GetGridPartialAsync<ListOfUserNamesDisplayComponent.Entry>(context, null,ListOfUserNamesDisplayComponent.GetGridModel(false, false, false), gridPvData);
            });

            group.MapPost(GridSupport.EditSortFilter, async (HttpContext context, [FromBody] GridSupport.GridPartialViewData gridPvData) => {
                return await GridSupport.GetGridPartialAsync<ListOfUserNamesEditComponent.Entry>(context, null, ListOfUserNamesEditComponent.GetGridModel(false), gridPvData);
            });

            group.MapPost(AddUserName, async (HttpContext context, [FromBody] GridSupport.GridAdditionPartialViewData<ListOfUserNamesEditComponent.Entry> pvData, string fieldPrefix, string newUser) => {
                using (UserDefinitionDataProvider userDP = new DataProvider.UserDefinitionDataProvider()) {
                    UserDefinition user = await userDP.GetItemAsync(newUser);
                    if (user == null)
                        throw new Error(__ResStr("noUser", "User {0} not found", newUser));
                    if ((from l in pvData.GridData where l.UserId == user.UserId select l).FirstOrDefault() != null)
                        throw new Error(__ResStr("dupUser", "User {0} has already been added", newUser));
                    // add new grid record
                    return await GridSupport.GetGridRecordAsync(context, pvData, new GridRecordData() {
                        GridDef = ListOfUserNamesEditComponent.GetGridModel(false),
                        Data = new ListOfUserNamesEditComponent.Entry {
                            UserName = newUser,
                            UserId = user.UserId,
                        },
                        FieldPrefix = fieldPrefix,
                    });
                }
            });
        }
    }
}
