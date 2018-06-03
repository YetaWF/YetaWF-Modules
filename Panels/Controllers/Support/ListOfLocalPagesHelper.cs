/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Panels#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Pages.DataProvider;
using YetaWF.Core.Pages;
using YetaWF.Modules.Panels.Addons;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Panels.Controllers.Shared {

    public class ListOfLocalPagesHelperController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ResourceAuthorize(Info.Resource_AllowListOfLocalPagesAjax)]
        public async Task<ActionResult> ListOfLocalPagesBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters /*, Guid settingsModuleGuid - not available in templates */) {
            using (PageDefinitionDataProvider pagesDP = new PageDefinitionDataProvider()) {
                DataProviderGetRecords<PageDefinition> browseItems = await pagesDP.GetItemsAsync(skip, take, sort, filters);
                //Grid.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return await GridPartialViewAsync(new DataSourceResult {
                    Data = (from s in browseItems.Data select new Views.Shared.ListOfLocalPagesHelper.GridAllEntry(s)).ToList<object>(),
                    Total = browseItems.Total
                });
            }
        }
    }
}
