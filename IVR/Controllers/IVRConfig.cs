/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Microsoft.AspNetCore.Mvc;
using Softelvdm.Modules.IVR.DataProvider;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Security;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace Softelvdm.Modules.IVR.Controllers {

    public class IVRConfigModuleController : ControllerImpl<Softelvdm.Modules.IVR.Modules.IVRConfigModule> {

        public IVRConfigModuleController() { }

        [Trim]
        public class Model {

            [Category("Accounts"), Caption("Test Mode"), Description("Defines whether test mode or live mode is used - During development use test mode - This can only be set using AppSettings.json")]
            [UIHint("Boolean"), ReadOnly]
            public bool TestMode { get; set; }

            [Category("Accounts"), Caption("Live Account Sid"), Description("The Live Account Sid is obtained from your Twilio account, defined in appsettings.json (applies to all sites in this YetaWF instance) and is used when Test Mode is disabled")]
            [UIHint("String"), ReadOnly]
            [HelpLink("https://www.twilio.com/")]
            [ExcludeDemoMode]
            public string LiveAccountSid { get; set; }
            [Category("Accounts"), Caption("Live Auth Token"), Description("The Live Auth Token is obtained from your Twilio account, defined in appsettings.json (applies to all sites in this YetaWF instance) and is used when Test Mode is disabled")]
            [UIHint("String"), ReadOnly]
            [ExcludeDemoMode]
            public string LiveAuthToken { get; set; }

            [Category("Accounts"), Caption("Test Account Sid"), Description("The Test Account Sid is obtained from your Twilio account, defined in appsettings.json (applies to all sites in this YetaWF instance) and is used when Test Mode is enabled")]
            [UIHint("String"), ReadOnly]
            [HelpLink("https://www.twilio.com/")]
            [ExcludeDemoMode]
            public string TestAccountSid { get; set; }
            [Category("Accounts"), Caption("Test Auth Token"), Description("The Test Auth Token is obtained from your Twilio account, defined in appsettings.json (applies to all sites in this YetaWF instance) and is used when Test Mode is enabled")]
            [UIHint("String"), ReadOnly]
            [ExcludeDemoMode]
            public string TestAuthToken { get; set; }

            [Category("Notifications"), Caption("SMS Numbers"), Description("Defines the phone numbers that receive a text message whenever a call is received")]
            [TextAbove("Phone numbers added here receive a text message when a call is received (before the caller can select an extension or leave a voice mail). This is may become annoying fast if you receive many calls on the main number. " +
                "Make sure to enable the SMS checkbox for each phone number where you want to receive text messages.")]
            [UIHint("Softelvdm_IVR_ListOfPhoneNumbers"), Required]
            public SerializableList<ExtensionPhoneNumber> NotificationNumbers { get; set; }

            [Category("IVR"), Caption("Voice"), Description("Defines the Voice variable, which can be used to override the default voice - This voice is used for all messages to external users - If not specified, the default voice is used")]
            [UIHint("Text80"), StringLength(DataProvider.IVRConfig.MaxVoice), Trim]
            [HelpLink("https://www.twilio.com/console/voice/twiml/text-to-speech")]
            public string Voice { get; set; }
            [Category("IVR"), Caption("Voice (Internal)"), Description("Defines the Voice variable, which can be used to override the default voice - This voice is used for all messages to internal users - If not specified, the default voice is used")]
            [UIHint("Text80"), StringLength(DataProvider.IVRConfig.MaxVoice), Trim]
            [HelpLink("https://www.twilio.com/console/voice/twiml/text-to-speech")]
            public string VoiceInternal { get; set; }

            [Category("IVR"), Caption("Max. Errors"), Description("The maximum number of invalid entries by the caller before hanging up - Set to 0 to ignore")]
            [UIHint("IntValue2"), Range(0, 99)]
            public int MaxErrors { get; set; }

            [Category("IVR"), Caption("Live Call Process Url"), Description("Defines the Request Url used to verify a Twilio Call request in Live mode - This is typically used for load balancers and cases where the request is redirected, which changes the original Url - Instead of inspecting the various non-standard headers, specify the expected Url here - If not specified the default is http[s]://...yoursite.com.../Softelvdm_IVR/Call/Process")]
            [UIHint("Text80"), StringLength(Globals.MaxUrl), Trim]
            public string LiveVerificationProcessCallUrl { get; set; }

            [Category("IVR"), Caption("Test Call Process Url"), Description("Defines the Request Url used to verify a Twilio Fax Sent request in Test mode - This is typically used for load balancers and cases where the request is redirected, which changes the original Url - Instead of inspecting the various non-standard headers, specify the expected Url here - If not specified the default is http[s]://...yoursite.com.../Softelvdm_IVR/Call/Process")]
            [UIHint("Text80"), StringLength(Globals.MaxUrl), Trim]
            public string TestVerificationProcessCallUrl { get; set; }

            [Category("Voice Mail"), Caption("Display Url"), Description("Defines the optional page used to display a voice mail - If no page is defined, a Url is generated")]
            [UIHint("Url"), StringLength(Globals.MaxUrl), AdditionalMetadata("UrlType", UrlTypeEnum.Local | UrlTypeEnum.Remote), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Remote|UrlTypeEnum.Local)]
            public string DisplayVoiceMailUrl { get; set; }

            [TextAbove("Enter the hours your company is open.")]
            [Category("Hours"), Caption(""), Description("")]
            [UIHint("WeeklyHours")]
            public WeeklyHours OpeningHours { get; set; }

            [Category("Security"), Caption("Generate Keys"), Description("Select to generate new keys, which replace the current Private and Public keys")]
            [UIHint("Boolean")]
            public bool GenerateKeys { get; set; }

            [Category("Security"), Caption("Public Key"), Description("The public key used to encrypt the token sent from the IVR to Twilio with POST requests, to be validated on return")]
            [UIHint("TextAreaSourceOnly"), StringLength(Globals.MaxPublicKey)]
            [ExcludeDemoMode]
            public string PublicKey { get; set; }

            [Category("Security"), Caption("Private Key"), Description("The private key used to decrypt the token sent from the IVR - DO NOT SHARE THIS KEY!")]
            [UIHint("TextAreaSourceOnly"), StringLength(Globals.MaxPrivateKey)]
            [ExcludeDemoMode]
            public string PrivateKey { get; set; }

            public IVRConfig GetData(IVRConfig data) {
                ObjectSupport.CopyData(this, data);
                return data;
            }
            public void UpdateData(IVRConfig data) {
                TestMode = data.TestMode;
                LiveAccountSid = data.LiveAccountSid;
                LiveAuthToken = data.LiveAuthToken;
                TestAccountSid = data.TestAccountSid;
                TestAuthToken = data.TestAuthToken;
            }
            public void SetData(IVRConfig data) {
                ObjectSupport.CopyData(data, this);
            }
            public Model() { }
        }

        [AllowGet]
        public async Task<ActionResult> IVRConfig() {
            using (IVRConfigDataProvider dataProvider = new IVRConfigDataProvider()) {
                Model model = new Model { };
                IVRConfig data = await dataProvider.GetItemAsync();
                if (data == null)
                    throw new Error(this.__ResStr("notFound", "The IVR settings could not be found"));
                model.SetData(data);
                model.UpdateData(data);
                return View(model);
            }
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> IVRConfig_Partial(Model model) {

            if (model.GenerateKeys) {
                string publicKey, privateKey;
                RSACrypto.MakeNewKeys(out publicKey, out privateKey);
                model.PrivateKey = privateKey;
                model.PublicKey = publicKey;
                model.GenerateKeys = false;
            }

            using (IVRConfigDataProvider configDP = new IVRConfigDataProvider()) {
                IVRConfig config = await configDP.GetItemAsync();// get the original item
                model.UpdateData(config);
                if (!ModelState.IsValid)
                    return PartialView(model);
                config = model.GetData(config); // merge new data into original
                model.SetData(config); // and all the data back into model for final display
                await configDP.UpdateConfigAsync(config);

                using (ScriptDataProvider scriptDP = new ScriptDataProvider()) {
                    scriptDP.ClearScript();
                }
                return FormProcessed(model, this.__ResStr("okSaved", "IVR settings saved"), NextPage: Manager.ReturnToUrl);
            }
        }
    }
}