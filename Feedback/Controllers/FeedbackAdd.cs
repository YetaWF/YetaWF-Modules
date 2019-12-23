/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Feedback.DataProvider;
using YetaWF.Modules.Feedback.Support;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Feedback.Controllers {
    public class FeedbackAddModuleController : ControllerImpl<YetaWF.Modules.Feedback.Modules.FeedbackAddModule> {

        public FeedbackAddModuleController() { }

        [Trim]
        public class AddModel {

            public int Key { get; set; }

            [Caption("Subject"), Description("Please enter the subject for your message")]
            [UIHint("Text80"), StringLength(FeedbackData.MaxSubject), Required, Trim]
            public string Subject { get; set; }

            [Caption("Your Name"), Description("Please enter your name - Your name will not be publicly visible")]
            [UIHint("Text40"), SuppressIf(nameof(RequireName), false), StringLength(FeedbackData.MaxName), Required, Trim]
            public string Name { get; set; }

            [Caption("Your Email Address"), Description("Please enter your email address - Your email address will not be publicly visible")]
            [UIHint("Email"), SuppressIf(nameof(RequireEmail), false), StringLength(Globals.MaxEmail), EmailValidation, Required, Trim]
            public string Email { get; set; }

            [Caption("Message"), Description("Please enter the message")]
            [UIHint("TextAreaSourceOnly"), StringLength(FeedbackData.MaxMessage), Required]
            public string Message { get; set; }

            [Caption("Captcha"), Description("Please verify that you're a human and not a spam bot")]
            [UIHint("RecaptchaV2"), RecaptchaV2("Please verify that you're a human and not a spam bot"), SuppressIf("ShowCaptcha", false)]
            public RecaptchaV2Data Captcha { get; set; }

            [UIHint("Hidden")]
            public bool RequireName { get; set; }
            [UIHint("Hidden")]
            public bool RequireEmail { get; set; }
            [UIHint("Hidden")]
            public bool ShowCaptcha { get; set; }

            public AddModel() {
                Captcha = new RecaptchaV2Data() { };
            }

            public FeedbackData GetData() {
                FeedbackData data = new FeedbackData();
                ObjectSupport.CopyData(this, data);
                return data;
            }
        }

        [AllowGet]
        public async Task<ActionResult> FeedbackAdd() {
            AddModel model = new AddModel {
                Captcha = new RecaptchaV2Data(),
                Subject = Module.DefaultSubject,
                Message = Module.DefaultMessage,
            };
            FeedbackConfigData config = await FeedbackConfigDataProvider.GetConfigAsync();
            model.RequireName = config.RequireName;
            model.RequireEmail = config.RequireEmail;
            model.ShowCaptcha = config.Captcha;
            return View(model);
        }


        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> FeedbackAdd_Partial(AddModel model) {

            FeedbackConfigData config = await FeedbackConfigDataProvider.GetConfigAsync();
            model.RequireName = config.RequireName;
            model.RequireEmail = config.RequireEmail;
            model.ShowCaptcha = config.Captcha;

            if (!ModelState.IsValid)
                return PartialView(model);

            using (FeedbackDataProvider dataProvider = new FeedbackDataProvider()) {
                if (!await dataProvider.AddItemAsync(model.GetData()))
                    throw new InternalError("Feedback couldn't be sent");

                Emails emails = new Emails(Manager);
                await emails.SendFeedbackAsync(config.Email, model.Email, model.Subject, model.Message, model.Name, config.BccEmails ? Manager.CurrentSite.AdminEmail : null);

                return FormProcessed(model, this.__ResStr("okSaved", "Your message has been sent!"), NextPage: Manager.ReturnToUrl);
            }
        }
    }
}
