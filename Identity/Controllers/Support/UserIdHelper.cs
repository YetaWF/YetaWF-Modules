/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Identity.Addons;
using YetaWF.Modules.Identity.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers.Shared {

    public class UserIdHelperController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ResourceAuthorize(Info.Resource_AllowUserIdAjax)]
        public ActionResult UsersBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters /*, Guid settingsModuleGuid - not available in templates */) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                int total;
                List<UserDefinition> browseItems = dataProvider.GetItems(skip, take, sort, filters, out total);
                //GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return GridPartialView(new DataSourceResult {
                    Data = (from s in browseItems select new Views.Shared.UserIdHelper.GridAllEntry(s)).ToList<object>(),
                    Total = total
                });
            }
        }
    }
}
