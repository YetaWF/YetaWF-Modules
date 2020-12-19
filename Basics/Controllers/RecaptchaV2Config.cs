/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Modules.Basics.DataProvider;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Basics.Controllers {

    public class RecaptchaV2ConfigModuleController : ControllerImpl<YetaWF.Modules.Basics.Modules.RecaptchaV2ConfigModule> {

        public RecaptchaV2ConfigModuleController() { }

        [Trim]
        public class EditModel {

            public EditModel() {
                GoogleUrl = "https://www.google.com/recaptcha";
            }

            [Caption("Public Key"), Description("The public key used to communicate with the Google/Recaptcha site")]
            [UIHint("Text80"), StringLength(YetaWF.Core.Components.RecaptchaV2Config.MaxPublicKey), Trim]
            [ExcludeDemoMode]
            public string PublicKey { get; set; }

            [Caption("Private Key"), Description("The private key used to communicate with the Google/Recaptcha site")]
            [UIHint("Text80"), StringLength(YetaWF.Core.Components.RecaptchaV2Config.MaxPrivateKey), RequiredIfSuppliedAttribute(nameof(PublicKey)), Trim]
            [ExcludeDemoMode]
            public string PrivateKey { get; set; }

            [Caption("Theme"), Description("The theme used for the recaptcha control throughout the site")]
            [UIHint("Enum")]
            public RecaptchaV2Config.ThemeEnum Theme { get; set; }

            [Caption("Size"), Description("The recaptcha control size used throughout the site")]
            [UIHint("Enum")]
            public RecaptchaV2Config.SizeEnum Size { get; set; }

            [Caption("Info"), Description("Visit Google to obtain keys for Recaptcha use on your site")]
            [UIHint("Url"), ReadOnly]
            public string GoogleUrl { get; set; }

            public RecaptchaV2Config GetData() {
                RecaptchaV2Config data = new RecaptchaV2Config();
                ObjectSupport.CopyData(this, data);
                return data;
            }

            public void SetData(RecaptchaV2Config data) {
                ObjectSupport.CopyData(data, this);
            }
        }

        [AllowGet]
        public async Task<ActionResult> RecaptchaV2Config() {
            using (RecaptchaV2ConfigDataProvider dataProvider = new RecaptchaV2ConfigDataProvider()) {
                EditModel model = new EditModel { };
                RecaptchaV2Config data = await dataProvider.GetItemAsync();
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> RecaptchaV2Config_Partial(EditModel model) {
            using (RecaptchaV2ConfigDataProvider dataProvider = new RecaptchaV2ConfigDataProvider()) {
                RecaptchaV2Config data = await dataProvider.GetItemAsync();// get the original item

                if (!ModelState.IsValid)
                    return PartialView(model);

                ObjectSupport.CopyData(model, data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display

                await dataProvider.UpdateConfigAsync(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Captcha configuration saved"));
            }
        }
    }
}