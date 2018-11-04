/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DevTests#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support.SendSMS;
#if MVC6
using Microsoft.AspNetCore.Mvc;
using YetaWF.Core.Support;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.DevTests.Controllers {

    public class SMSTestModuleController : ControllerImpl<YetaWF.Modules.DevTests.Modules.SMSTestModule> {

        public SMSTestModuleController() { }

        [Trim]
        [Header("SMS requires an SMS provider. These are not included with YetaWF and are available separately from Softel vdm, Inc. Check the YetaWF web site store for additional information. Please note that SMS providers don't issue an error when the SMS message is sent - Inspect the log file to find errors in SMS processing.")]
        public class Model {

            [Caption("Phone Number"), Description("The phone number which will receive the SMS message")]
            [TextBelow("The phone number is not validated - It is possible to specify an email address instead to test the email fallback if no SMS provider is available.")]
            [UIHint("Text20"), StringLength(30), Required, Trim]
            public string PhoneNumber { get; set; }

            [Caption("Text"), Description("The text message to send")]
            [UIHint("TextAreaSourceOnly"), StringLength(SendSMS.MaxMessageLength), Required]
            public string Text { get; set; }

            public Model() { }
        }

        [AllowGet]
        public ActionResult SMSTest() {
            Model model = new Model { };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> SMSTest_Partial(Model model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            SendSMS sendSMS = new SendSMS();
            await sendSMS.SendMessageAsync(model.PhoneNumber, model.Text);
            return FormProcessed(model, this.__ResStr("ok", "SMS sent"));
        }
    }
}
