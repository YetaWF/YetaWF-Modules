/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.PageEdit.DataProvider;
using YetaWF.Core.Serializers;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Identity;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.PageEdit.Controllers {

    public class ControlPanelConfigModuleController : ControllerImpl<YetaWF.Modules.PageEdit.Modules.ControlPanelConfigModule> {

        public ControlPanelConfigModuleController() { }

        [Trim]
        public class Model {

            [Caption("W3C Validation"), Description("The Url used to validate the current page using a W3C Validation service - Use {0} where the Url is inserted - If no Url is defined, the Control Panel will not display a W3C Validation link")]
            [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Remote)]
            [StringLength(Globals.MaxUrl), Trim]
            public string W3CUrl { get; set; }

            [Category("General"), Caption("User List"), Description("List of user accounts that can be used to quickly log into the site")]
            [UIHint("YetaWF_Identity_ResourceUsers")]
            [Data_Binary]
            public SerializableList<User> Users { get; set; }

            public ControlPanelConfigData GetData(ControlPanelConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(ControlPanelConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [AllowGet]
        public async Task<ActionResult> ControlPanelConfig() {
            using (ControlPanelConfigDataProvider dataProvider = new ControlPanelConfigDataProvider()) {
                Model model = new Model { };
                ControlPanelConfigData data = await dataProvider.GetItemAsync();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The Control Panel settings could not be found"));
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> ControlPanelConfig_Partial(Model model) {
            using (ControlPanelConfigDataProvider dataProvider = new ControlPanelConfigDataProvider()) {
                ControlPanelConfigData origConfig = await dataProvider.GetItemAsync();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                ControlPanelConfigData config = await dataProvider.GetItemAsync();
                config = model.GetData(config); // merge new data into config
                model.SetData(config); // and all the data back into model for final display

                await dataProvider.UpdateConfigAsync(config);

                ObjectSupport.ModelDisposition modelDisp = ObjectSupport.EvaluateModelChanges(origConfig, config);
                switch (modelDisp) {
                    default:
                    case ObjectSupport.ModelDisposition.None:
                        return FormProcessed(model, this.__ResStr("okSaved", "Control Panel settings saved"));
                    case ObjectSupport.ModelDisposition.PageReload:
                        return FormProcessed(model, this.__ResStr("okSaved", "Control Panel settings saved"), OnClose: OnCloseEnum.ReloadPage, OnPopupClose: OnPopupCloseEnum.ReloadParentPage, ForceRedirect: true);
                    case ObjectSupport.ModelDisposition.SiteRestart:
                        return FormProcessed(model, this.__ResStr("okSavedRestart", "Control Panel settings saved - These settings won't take effect until the site is restarted"));
                }
            }
        }
    }
}