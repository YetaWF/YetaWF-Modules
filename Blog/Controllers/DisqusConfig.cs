/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Support;

namespace YetaWF.Modules.Blog.Controllers {

    public class DisqusConfigModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.DisqusConfigModule> {

        public DisqusConfigModuleController() { }

        [Trim]
        public class Model {

            [Caption("Shortname"), Description("The Shortname you assigned to your site (at Disqus)")]
            [UIHint("Text40"), StringLength(DisqusConfigData.MaxShortName), ShortNameValidation, Required, Trim]
            [HelpLink("https://yetawf.disqus.com/admin/settings/general/")]
            public string ShortName { get; set; }

            public DisqusConfigData GetData(DisqusConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(DisqusConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [HttpGet]
        public ActionResult DisqusConfig() {
            using (DisqusConfigDataProvider dataProvider = new DisqusConfigDataProvider()) {
                Model model = new Model { };
                DisqusConfigData data = dataProvider.GetItem();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The Disqus settings could not be found"));
                model.SetData(data);
                return View(model);
            }
        }

        [HttpPost]
        [ExcludeDemoMode]
        [ValidateAntiForgeryToken]
        public ActionResult DisqusConfig_Partial(Model model) {
            using (DisqusConfigDataProvider dataProvider = new DisqusConfigDataProvider()) {
                DisqusConfigData data = dataProvider.GetItem();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                dataProvider.UpdateConfig(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Disqus settings saved"), NextPage: Manager.ReturnToUrl);
            }
        }
    }
}