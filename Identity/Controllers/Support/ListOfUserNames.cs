/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Models.Attributes;

using YetaWF.Modules.Identity.Addons;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Components;
using YetaWF.Core.Views.Shared;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class ListOfUserNamesController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ResourceAuthorize(Info.Resource_AllowListOfUserNamesAjax)]
        public async Task<ActionResult> ListOfUserNamesBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters /*, Guid settingsModuleGuid - not available in templates */) {
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                DataProviderGetRecords<UserDefinition> browseItems = await userDP.GetItemsAsync(skip, take, sort, filters);
                //Grid.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return await GridPartialViewAsync(new DataSourceResult {
                    Data = (from s in browseItems.Data select new ListOfUserNamesEditComponent.GridAllEntry(s)).ToList<object>(),
                    Total = browseItems.Total
                });
            }
        }
    }
}
