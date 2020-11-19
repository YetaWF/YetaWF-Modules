/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Dashboard#License */

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Dashboard.DataProvider;

namespace YetaWF.Modules.Dashboard.Controllers {

    public class AuditConfigModuleController : ControllerImpl<YetaWF.Modules.Dashboard.Modules.AuditConfigModule> {

        public AuditConfigModuleController() { }

        [Trim]
        public class Model {

            [Caption("Retention (Days)"), Description("Defines the number of days audit information is saved - Audit information that is older than the specified number of days is deleted")]
            [UIHint("IntValue"), Range(1, 99999999), Required]
            public int Days { get; set; }

            public AuditConfigData GetData(AuditConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(AuditConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [AllowGet]
        public async Task<ActionResult> AuditConfig() {
            using (AuditConfigDataProvider dataProvider = new AuditConfigDataProvider()) {
                Model model = new Model { };
                AuditConfigData data = await dataProvider.GetItemAsync();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The audit settings could not be found"));
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AuditConfig_Partial(Model model) {
            using (AuditConfigDataProvider dataProvider = new AuditConfigDataProvider()) {
                AuditConfigData data = await dataProvider.GetItemAsync();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                await dataProvider.UpdateConfigAsync(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Audit settings saved"), NextPage: Manager.ReturnToUrl);
            }
        }
    }
}
