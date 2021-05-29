/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Feedback.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Feedback.Controllers {

    public class FeedbackConfigModuleController : ControllerImpl<YetaWF.Modules.Feedback.Modules.FeedbackConfigModule> {

        public FeedbackConfigModuleController() { }

        [Trim]
        public class Model {

            [Caption("Use Captcha"), Description("The user has to pass a \"human\" test")]
            [UIHint("Boolean")]
            public bool Captcha { get; set; }

            [Caption("Name Required"), Description("Defines whether the user must enter his/her name to be able to send feedback")]
            [UIHint("Boolean")]
            public bool RequireName { get; set; }

            [Caption("Email Address Required"), Description("Defines whether the user must enter his/her email address to be able to send feedback")]
            [UIHint("Boolean")]
            public bool RequireEmail { get; set; }

            [Caption("Email Address"), Description("The email address where all feedback messages are sent")]
            [UIHint("Email"), StringLength(Globals.MaxEmail), EmailValidation, Trim]
            public string? Email { get; set; }

            [Caption("Copy Feedback Emails"), Description("Defines whether the site administrator receives a copy of all feedback messages")]
            [UIHint("Boolean")]
            public bool BccEmails { get; set; }

            public FeedbackConfigData GetData(FeedbackConfigData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void SetData(FeedbackConfigData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [AllowGet]
        public async Task<ActionResult> FeedbackConfig() {
            using (FeedbackConfigDataProvider dataProvider = new FeedbackConfigDataProvider()) {
                Model model = new Model { };
                FeedbackConfigData data = await dataProvider.GetItemAsync();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The feedback settings were not found."));
                model.SetData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> FeedbackConfig_Partial(Model model) {
            using (FeedbackConfigDataProvider dataProvider = new FeedbackConfigDataProvider()) {
                FeedbackConfigData data = await dataProvider.GetItemAsync();// get the original item
                if (!ModelState.IsValid)
                    return PartialView(model);
                data = model.GetData(data); // merge new data into original
                model.SetData(data); // and all the data back into model for final display
                await dataProvider.UpdateConfigAsync(data);
                return FormProcessed(model, this.__ResStr("okSaved", "Feedback settings saved"), NextPage: Manager.ReturnToUrl);
            }
        }
    }
}