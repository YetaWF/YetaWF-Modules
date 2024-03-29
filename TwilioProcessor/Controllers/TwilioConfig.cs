/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TwilioProcessor#License */

using Microsoft.AspNetCore.Mvc;
using Softelvdm.Modules.TwilioProcessor.DataProvider;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;

namespace Softelvdm.Modules.TwilioProcessor.Controllers {

    public class TwilioConfigModuleController : ControllerImpl<Softelvdm.Modules.TwilioProcessor.Modules.TwilioConfigModule> {

        public TwilioConfigModuleController() { }

        [Trim]
        [HeaderAttribute("In order to use Twilio services you need a Twilio account. It is possible to test the service with a free account.")]
        public class Model {

            [Category("Accounts"), Caption("Test Mode"), Description("Defines whether test mode or live mode is used - During development use test mode - This can only be set using AppSettings.json")]
            [UIHint("Boolean"), ReadOnly]
            public bool TestMode { get; set; }

            [Category("SMS"), Caption("Enabled"), Description("Defines whether the Twilio SMS phone number is enabled")]
            [UIHint("Boolean")]
            public bool SMSEnabled { get; set; }

            [Category("Accounts"), Caption("Live Account Sid"), Description("The Live Account Sid is obtained from your Twilio account, defined in appsettings.json (applies to all sites in this YetaWF instance) and is used when Test Mode is disabled")]
            [UIHint("String"), ReadOnly]
            [HelpLink("https://www.twilio.com/")]
            [ExcludeDemoMode]
            public string? LiveAccountSid { get; set; }
            [Category("Accounts"), Caption("Live Auth Token"), Description("The Live Auth Token is obtained from your Twilio account, defined in appsettings.json (applies to all sites in this YetaWF instance) and is used when Test Mode is disabled")]
            [UIHint("String"), ReadOnly]
            [ExcludeDemoMode]
            public string? LiveAuthToken { get; set; }

            [Category("Accounts"), Caption("Test Account Sid"), Description("The Test Account Sid is obtained from your Twilio account, defined in appsettings.json (applies to all sites in this YetaWF instance) and is used when Test Mode is enabled")]
            [UIHint("String"), ReadOnly]
            [HelpLink("https://www.twilio.com/")]
            [ExcludeDemoMode]
            public string? TestAccountSid { get; set; }
            [Category("Accounts"), Caption("Test Auth Token"), Description("The Test Auth Token is obtained from your Twilio account, defined in appsettings.json (applies to all sites in this YetaWF instance) and is used when Test Mode is enabled")]
            [UIHint("String"), ReadOnly]
            [ExcludeDemoMode]
            public string? TestAuthToken { get; set; }

            [Category("SMS"), Caption("Live SMS Number"), Description("The default live phone number for SMS associated with your Twilio account - Only purchased Twilio provided phone numbers can be used (see Twilio for details)")]
            [UIHint("Text20"), StringLength(Globals.MaxPhoneNumber), PhoneNumberNationalValidation, Trim]
            [ExcludeDemoMode]
            public string? LiveSMSNumber { get; set; }

            [Category("SMS"), Caption("Test SMS Number"), Description("The default test phone number for SMS associated with your Twilio account - Only Twilio provided test phone numbers can be used (see Twilio for details)")]
            [UIHint("Text20"), StringLength(Globals.MaxPhoneNumber), PhoneNumberNationalValidation, Trim]
            [ExcludeDemoMode]
            public string? TestSMSNumber { get; set; }

            [Category("SMS"), Caption("Max. SMS Per Second"), Description("Defines the maximum number of SMS that are supported by the phone number - Set to 0 to disable pacing - Twilio phone numbers have restrictions as to how many SMS can be sent per second")]
            [UIHint("IntValue4")]
            public int MaxSMSPerSecond { get; set; }

            [Category("SMS"), Caption("Delivery Receipts"), Description("Defines whether delivery receipts from Twilio are enabled - Delivery status is logged to the YetaWF log")]
            [UIHint("Boolean")]
            public bool DeliveryReceipts { get; set; }
            [Category("SMS"), Caption("Receipts Security"), Description("Delivery receipts are sent from Twilio using a Url - This property defines whether https:// or http:// is used")]
            [UIHint("Boolean")]
            public bool UseHttps { get; set; }

            [Category("Accounts"), Caption("Visit Twilio"), Description("A link to Twilio to set up your account")]
            [UIHint("Url"), ReadOnly]
            public string TwilioUrl { get; set; }

            public TwilioData GetData(TwilioData data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void UpdateData(TwilioData data) {
                LiveAccountSid = data.LiveAccountSid;
                LiveAuthToken = data.LiveAuthToken;
                TestAccountSid = data.TestAccountSid;
                TestAuthToken = data.TestAuthToken;
            }
            public void SetData(TwilioData data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() {
                TwilioUrl = "https://www.twilio.com";
            }
        }

        [AllowGet]
        public async Task<ActionResult> TwilioConfig() {
            using (TwilioConfigDataProvider dataProvider = new TwilioConfigDataProvider()) {
                Model model = new Model { };
                TwilioData data = await dataProvider.GetItemAsync();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The TwilioProcessor settings could not be found"));
                model.SetData(data);
                model.UpdateData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> TwilioConfig_Partial(Model model) {
            using (TwilioConfigDataProvider configDP = new TwilioConfigDataProvider()) {
                TwilioData config = await configDP.GetItemAsync();// get the original item
                model.UpdateData(config);
                if (!ModelState.IsValid)
                    return PartialView(model);
                config = model.GetData(config); // merge new data into original
                model.SetData(config); // and all the data back into model for final display
                await configDP.UpdateConfigAsync(config);

                return FormProcessed(model, this.__ResStr("okSaved", "TwilioProcessor settings saved"), NextPage: Manager.ReturnToUrl);
            }
        }
    }
}