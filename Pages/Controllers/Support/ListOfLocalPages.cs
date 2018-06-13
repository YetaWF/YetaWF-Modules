/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Pages.Addons;
using YetaWF.Modules.Pages.DataProvider;
using YetaWF.Core.Pages;
using YetaWF.Core.Components;
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
        public async Task<ActionResult> ListOfLocalPagesBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters /*, Guid settingsModuleGuid - not available in templates */) {
            using (PageDefinitionDataProvider pagesDP = new PageDefinitionDataProvider()) {
                DataProviderGetRecords<PageDefinition> browseItems = await pagesDP.GetItemsAsync(skip, take, sort, filters);
                //Grid.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return await GridPartialViewAsync(new DataSourceResult {
                    Data = (from s in browseItems.Data select new ListOfLocalPagesEditComponent.GridAllEntry(s)).ToList<object>(),
                    Total = browseItems.Total
                });
            }
        }
    }
}
