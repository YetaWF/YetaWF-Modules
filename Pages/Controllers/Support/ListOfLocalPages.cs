/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Pages.Addons;
using YetaWF.Modules.Pages.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Pages.Controllers {

    public class ListOfLocalPagesController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ResourceAuthorize(Info.Resource_AllowListOfLocalPagesAjax)]
        public async Task<ActionResult> ListOfLocalPagesBrowse_GridData(string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters /*, Guid settingsModuleGuid - not available in templates */) {
            return await GridPartialViewAsync(ListOfLocalPagesEditComponent.GetGridAllUsersModel(), fieldPrefix, skip, take, sorts, filters);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ListOfLocalPagesDisplay_SortFilter(string data, string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync<ListOfLocalPagesDisplayComponent.Entry>(ListOfLocalPagesDisplayComponent.GetGridModel(false), data, fieldPrefix, skip, take, sorts, filters);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ListOfLocalPagesEdit_SortFilter(string data, string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync<ListOfLocalPagesEditComponent.Entry>(ListOfLocalPagesEditComponent.GetGridModel(false), data, fieldPrefix, skip, take, sorts, filters);
        }
    }
}
