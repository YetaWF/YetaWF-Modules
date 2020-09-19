/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
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

            public SearchConfigData GetData(SearchConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(SearchConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [AllowGet]
        public async Task<ActionResult> SearchConfig() {
            using (SearchConfigDataProvider dataProvider = new SearchConfigDataProvider()) {
                Model model = new Model { };
                SearchConfigData data = await dataProvider.GetItemAsync();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The search configuration was not found."));
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> SearchConfig_Partial(Model model) {
            using (SearchConfigDataProvider dataProvider = new SearchConfigDataProvider()) {
                SearchConfigData data = await dataProvider.GetItemAsync();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                await dataProvider.UpdateConfigAsync(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Search configuration saved"));
            }
        }
    }
}