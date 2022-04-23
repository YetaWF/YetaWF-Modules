/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Visitors#License */

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Visitors.DataProvider;

namespace YetaWF.Modules.Visitors.Controllers {

    public class VisitorsConfigModuleController : ControllerImpl<YetaWF.Modules.Visitors.Modules.VisitorsConfigModule> {

        public VisitorsConfigModuleController() { }

        [Trim]
        public class Model {

            [Caption("Retention (Days)"), Description("Defines the number of days visitors information is saved - Visitors information that is older than the specified number of days is deleted")]
            [UIHint("IntValue"), Range(1, 99999999), Required]
            public int Days { get; set; }

            public VisitorsConfigData GetData(VisitorsConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(VisitorsConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [AllowGet]
        public async Task<ActionResult> VisitorsConfig() {
            using (VisitorsConfigDataProvider visitorDP = new VisitorsConfigDataProvider()) {
                if (!await visitorDP.IsInstalledAsync())
                    throw new Error(this.__ResStr("noInfo", "Visitor information is not available - See https://yetawf.com/Documentation/YetaWF/Visitors"));
                Model model = new Model { };
                VisitorsConfigData data = await visitorDP.GetItemAsync();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The visitors settings could not be found"));
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> VisitorsConfig_Partial(Model model) {
            using (VisitorsConfigDataProvider dataProvider = new VisitorsConfigDataProvider()) {
                VisitorsConfigData data = await dataProvider.GetItemAsync();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                await dataProvider.UpdateConfigAsync(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Visitors settings saved"), NextPage: Manager.ReturnToUrl);
            }
        }
    }
}