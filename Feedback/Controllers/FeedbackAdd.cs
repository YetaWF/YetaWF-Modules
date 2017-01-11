/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Feedback#License */

using System.Web.Mvc;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Feedback.DataProvider;
using YetaWF.Modules.Feedback.Support;

namespace YetaWF.Modules.Feedback.Controllers {
    public class FeedbackAddModuleController : ControllerImpl<YetaWF.Modules.Feedback.Modules.FeedbackAddModule> {

        public FeedbackAddModuleController() { }

        [Trim]
        public class AddModel {

            public int Key { get; set; }

            [Caption("Subject"), Description("Please enter the subject for your message")]
            [UIHint("Text80"), StringLength(FeedbackData.MaxSubject), Required, Trim]
            public string Subject { get; set; }

            [Caption("Email Address"), Description("Please enter your email address - your email address will not be publicly visible")]
            [UIHint("Email"), SuppressIfEqual("RequireEmail", false), StringLength(Globals.MaxEmail), EmailValidation, Required, Trim]
            public string Email { get; set; }

            [Caption("Message"), Description("Please enter the message")]
            [UIHint("TextArea"), AdditionalMetadata("SourceOnly", true), StringLength(FeedbackData.MaxMessage), Required, AllowHtml]
            public string Message { get; set; }

            [Caption("Captcha"), Description("Please verify that you're a human and not a spam bot")]
            [UIHint("RecaptchaV2"), RecaptchaV2("Please verify that you're a human and not a spam bot"), SuppressIfEqual("ShowCaptcha", false)]
            public RecaptchaV2Data Captcha { get; set; }

            [UIHint("Hidden")]
            public bool RequireEmail { get; set; }
            [UIHint("Hidden")]
            public bool ShowCaptcha { get; set; }

            public AddModel() {
                FeedbackConfigData config = FeedbackConfigDataProvider.GetConfig();
                Captcha = new RecaptchaV2Data() { };
                RequireEmail = config.RequireEmail;
                ShowCaptcha = config.Captcha;
            }

            public FeedbackData GetData() {
                FeedbackData data = new FeedbackData();
                ObjectSupport.CopyData(this, data);
                return data;
            }
        }

        [HttpGet]
        public ActionResult FeedbackAdd() {
            AddModel model = new AddModel {
                Captcha = new RecaptchaV2Data(),
                Subject = Module.DefaultSubject,
                Message = Module.DefaultMessage,
            };
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult FeedbackAdd_Partial(AddModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            using (FeedbackDataDataProvider dataProvider = new FeedbackDataDataProvider()) {
                if (!dataProvider.AddItem(model.GetData()))
                    throw new InternalError("Feedback couldn't be sent");

                FeedbackConfigData config = FeedbackConfigDataProvider.GetConfig();
                Emails emails = new Emails(Manager);
                emails.SendFeedback(config.Email, model.Email, model.Subject, model.Message, config.BccEmails ? Manager.CurrentSite.AdminEmail : null);

                return FormProcessed(model, this.__ResStr("okSaved", "Feedback sent!"), NextPage: Manager.ReturnToUrl);
            }
        }
    }
}
