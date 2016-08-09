/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Basics#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Basics.DataProvider;

namespace YetaWF.Modules.Basics.Controllers {

    public class RecaptchaConfigModuleController : ControllerImpl<YetaWF.Modules.Basics.Modules.RecaptchaConfigModule> {

        public RecaptchaConfigModuleController() { }

        [Trim]
        public class EditModel {

            public EditModel() {
                GoogleUrl = "https://www.google.com/recaptcha";
            }

            [Caption("Public Key"), Description("The public key used to communicate with the Google/Recaptcha site")]
            [UIHint("Text80"), StringLength(YetaWF.Core.Views.Shared.RecaptchaConfig.MaxPublicKey), Trim]
            [ExcludeDemoMode]
            public string PublicKey { get; set; }

            [Caption("Private Key"), Description("The private key used to communicate with the Google/Recaptcha site")]
            [UIHint("Text80"), StringLength(YetaWF.Core.Views.Shared.RecaptchaConfig.MaxPrivateKey), Trim]
            [ExcludeDemoMode]
            public string PrivateKey { get; set; }

            [Caption("Theme"), Description("Theme used for the recaptcha control throughout the site")]
            [UIHint("Enum")]
            public RecaptchaConfig.ThemeEnum Theme { get; set; }

            [Caption("Info"), Description("Visit Google to obtain keys for Recaptcha use on your site")]
            [UIHint("Url"), ReadOnly]
            public string GoogleUrl { get; set; }

            public RecaptchaConfig GetData() {
                RecaptchaConfig data = new RecaptchaConfig();
                ObjectSupport.CopyData(this, data);
                return data;
            }

            public void SetData(RecaptchaConfig data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        [HttpGet]
        public ActionResult RecaptchaConfig() {
            using (RecaptchaConfigDataProvider dataProvider = new RecaptchaConfigDataProvider()) {
                EditModel model = new EditModel { };
                RecaptchaConfig data = dataProvider.GetItem();
                model.SetData(data);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult RecaptchaConfig_Partial(EditModel model) {
            using (RecaptchaConfigDataProvider dataProvider = new RecaptchaConfigDataProvider()) {
                RecaptchaConfig data = dataProvider.GetItem();// get the original item

                if (!ModelState.IsValid)
                    return PartialView(model);

                ObjectSupport.CopyData(model, data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display

                dataProvider.UpdateConfig(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Captcha configuration saved"));
            }
        }
    }
}