/* Copyright � 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/AddThis#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.AddThis.DataProvider;

namespace YetaWF.Modules.AddThis.Controllers {

    public class SharingSidebarConfigModuleController : ControllerImpl<YetaWF.Modules.AddThis.Modules.SharingSidebarConfigModule> {

        public SharingSidebarConfigModuleController() { }

        [Trim]
        public class Model {

            [Category("General"), Caption("Javascript Code"), Description("The code for the Sharing Sidebar, obtained from your AddThis Dashboard"), HelpLink("https://www.addthis.com/dashboard")]
            [UIHint("TextArea"), AdditionalMetadata("SourceOnly", true)]
            [StringLength(ConfigData.MaxCode)]
            [AllowHtml]
            public string Code { get; set; }

            public ConfigData GetData(ConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(ConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [HttpGet]
        public ActionResult SharingSidebarConfig() {
            using (ConfigDataDataProvider dataProvider = new ConfigDataDataProvider()) {
                Model model = new Model { };
                ConfigData data = dataProvider.GetItem();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The configuration settings could not be found"));
                model.SetData(data);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SharingSidebarConfig_Partial(Model model) {
            using (ConfigDataDataProvider dataProvider = new ConfigDataDataProvider()) {
                ConfigData data = dataProvider.GetItem();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                dataProvider.UpdateConfig(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Configuration Settings saved"), NextPage: Manager.ReturnToUrl);
            }
        }
    }
}