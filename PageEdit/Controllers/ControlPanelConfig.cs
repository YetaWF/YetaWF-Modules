/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Web.Mvc;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.PageEdit.DataProvider;

namespace YetaWF.Modules.PageEdit.Controllers {

    public class ControlPanelConfigModuleController : ControllerImpl<YetaWF.Modules.PageEdit.Modules.ControlPanelConfigModule> {

        public ControlPanelConfigModuleController() { }

        [Trim]
        public class Model {

            [Caption("W3C Validation"), Description("The Url used to validate the current page using a W3C Validation service - Use {0} where the Url is inserted - If no Url is defined, the Control Panel will not display a W3C Validation link")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlHelperEx.UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlHelperEx.UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            public string W3CUrl { get; set; }

            public ControlPanelConfigData GetData(ControlPanelConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(ControlPanelConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [HttpGet]
        public ActionResult ControlPanelConfig() {
            using (ControlPanelConfigDataProvider dataProvider = new ControlPanelConfigDataProvider()) {
                Model model = new Model { };
                ControlPanelConfigData data = dataProvider.GetItem();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The Control Panel settings could not be found"));
                model.SetData(data);
                return View(model);
            }
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult ControlPanelConfig_Partial(Model model) {
            using (ControlPanelConfigDataProvider dataProvider = new ControlPanelConfigDataProvider()) {
                ControlPanelConfigData data = dataProvider.GetItem();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                dataProvider.UpdateConfig(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Control Panel settings saved"), NextPage: Manager.ReturnToUrl);
            }
        }
    }
}