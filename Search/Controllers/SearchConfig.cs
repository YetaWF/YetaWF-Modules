/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Search.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Search.Controllers {

    public class SearchConfigModuleController : ControllerImpl<YetaWF.Modules.Search.Modules.SearchConfigModule> {

        public SearchConfigModuleController() { }

        [Trim]
        public class Model {

            [Caption("Smallest Keyword (Mixed Case)"), Description("The smallest mixed case keyword")]
            [UIHint("IntValue2"), Range(2, 10), Required]
            public int SmallestMixedToken { get; set; }

            [Caption("Smallest Keyword (Uppercase)"), Description("The smallest all uppercase keyword")]
            [UIHint("IntValue2"), Range(2, 10), Required]
            public int SmallestUpperCaseToken { get; set; }

            [Caption("Results Url"), Description("The Url where the search results are displayed")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Local)]
            [StringLength(Globals.MaxUrl), Required, Trim]
            public string ResultsUrl { get; set; }

            [Caption("Max. Results"), Description("The maximum number of search results to display")]
            [UIHint("IntValue4"), Range(10, 1000)]
            public int MaxResults { get; set; }

            [Caption("Show Url"), Description("Defines whether the Url is shown in search results")]
            [UIHint("Boolean")]
            public bool ShowUrl { get; set; }

            [Caption("Show Summary"), Description("Defines whether a page summary is shown in search results")]
            [UIHint("Boolean")]
            public bool ShowSummary { get; set; }

            public SearchConfigData GetData(SearchConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(SearchConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [HttpGet]
        public ActionResult SearchConfig() {
            using (SearchConfigDataProvider dataProvider = new SearchConfigDataProvider()) {
                Model model = new Model { };
                SearchConfigData data = dataProvider.GetItem();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The search configuration was not found."));
                model.SetData(data);
                return View(model);
            }
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult SearchConfig_Partial(Model model) {
            using (SearchConfigDataProvider dataProvider = new SearchConfigDataProvider()) {
                SearchConfigData data = dataProvider.GetItem();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                dataProvider.UpdateConfig(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Search configuration saved"));
            }
        }
    }
}