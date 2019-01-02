/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Identity.Addons;
using YetaWF.Modules.Identity.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class ListOfUserNamesController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ListOfUserNamesDisplay_SortFilter(string data, string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync<ListOfUserNamesDisplayComponent.Entry>(ListOfUserNamesDisplayComponent.GetGridModel(false), data, fieldPrefix, skip, take, sorts, filters);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ListOfUserNamesEdit_SortFilter(string data, string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync<ListOfUserNamesEditComponent.Entry>(ListOfUserNamesEditComponent.GetGridModel(false), data, fieldPrefix, skip, take, sorts, filters);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ResourceAuthorize(Info.Resource_AllowListOfUserNamesAjax)]
        public async Task<ActionResult> ListOfUserNamesBrowse_GridData(string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync(ListOfUserNamesEditComponent.GetGridAllUsersModel(), fieldPrefix, skip, take, sorts, filters);
        }
    }
}
