/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Pages.Addons;
using YetaWF.Modules.Pages.DataProvider;
using YetaWF.Core.Pages;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Pages.Controllers.Shared {

    public class ListOfLocalPagesHelperController : YetaWFController {

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ResourceAuthorize(Info.Resource_AllowListOfLocalPagesAjax)]
        public ActionResult ListOfLocalPagesBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters /*, Guid settingsModuleGuid - not available in templates */) {
            using (PageDefinitionDataProvider pagesDP = new PageDefinitionDataProvider()) {
                int total;
                List<PageDefinition> browseItems = pagesDP.GetItems(skip, take, sort, filters, out total);
                //GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return GridPartialView(new DataSourceResult {
                    Data = (from s in browseItems select new Views.Shared.ListOfLocalPagesHelper.GridAllEntry(s)).ToList<object>(),
                    Total = total
                });
            }
        }
    }
}
