/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using YetaWF.Core.Endpoints;
using YetaWF.Modules.Pages.Components;

namespace YetaWF.Modules.Pages.Endpoints {

    public class ListOfLocalPagesEndpoint {

        //[AllowPost]
        //[ConditionalAntiForgeryToken]
        //[ResourceAuthorize(Info.Resource_AllowListOfLocalPagesAjax)]
        public static async Task<IResult> BrowseGridData(HttpContext context, GridSupport.GridPartialViewData gridPVData /* settingsModuleGuid - not available in templates */) {
            return await GridSupport.GetGridPartialAsync(context, ListOfLocalPagesEditComponent.GetGridAllUsersModel(), gridPVData);
        }

        //[AllowPost]
        //[ConditionalAntiForgeryToken]
        //public async Task<ActionResult> ListOfLocalPagesDisplay_SortFilter(GridPartialViewData gridPVData) {
        //    return await GridPartialViewAsync<ListOfLocalPagesDisplayComponent.Entry>(ListOfLocalPagesDisplayComponent.GetGridModel(false), gridPVData);
        //}
        //[AllowPost]
        //[ConditionalAntiForgeryToken]
        //public async Task<ActionResult> ListOfLocalPagesEdit_SortFilter(GridPartialViewData gridPVData) {
        //    return await GridPartialViewAsync<ListOfLocalPagesEditComponent.Entry>(ListOfLocalPagesEditComponent.GetGridModel(false), gridPVData);
        //}
    }
}
