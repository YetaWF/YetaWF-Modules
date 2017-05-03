/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Basics.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Basics.Controllers {

    public class AlertConfigModuleController : ControllerImpl<YetaWF.Modules.Basics.Modules.AlertConfigModule> {

        public AlertConfigModuleController() { }

        [Trim]
        public class Model {

            [Caption("Enabled"), Description("Defines whether message display is enabled")]
            [UIHint("Boolean")]
            public bool Enabled { get; set; }

            [Caption("Message Handling"), Description("Defines how long the message is displayed")]
            [UIHint("Enum")]
            public AlertConfig.MessageHandlingEnum MessageHandling { get; set; }

            [Caption("Message"), Description("Defines the message to display")]
            [UIHint("TextArea"), AdditionalMetadata("EmHeight", 5), StringLength(DataProvider.AlertConfig.MaxMessage), Trim]
            [RequiredIf("Enabled", true)]
            public string Message { get; set; }

            public AlertConfig GetData(AlertConfig data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(AlertConfig data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [HttpGet]
        public ActionResult AlertConfig() {
            using (AlertConfigDataProvider dataProvider = new AlertConfigDataProvider()) {
                Model model = new Model { };
                AlertConfig data = dataProvider.GetItem();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The Alert settings could not be found"));
                model.SetData(data);
                return View(model);
            }
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult AlertConfig_Partial(Model model) {
            using (AlertConfigDataProvider dataProvider = new AlertConfigDataProvider()) {
                AlertConfig data = dataProvider.GetItem();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                dataProvider.UpdateConfig(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Alert settings saved"), NextPage: Manager.ReturnToUrl);
            }
        }
    }
}