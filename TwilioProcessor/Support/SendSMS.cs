/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TwilioProcessor#License */

using Softelvdm.Modules.TwilioProcessor.Controllers;
using Softelvdm.Modules.TwilioProcessor.DataProvider;
using Softelvdm.Modules.TwilioProcessor.Models.Attributes;
using System;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using YetaWF.Core.Log;
using YetaWF.Core.Support;
using YetaWF.Core.Support.SendSMS;

namespace Softelvdm.Modules.TwilioProcessor.Support {

    public class TwilioSendSMS : ISendSMS, IInitializeApplicationStartup {

        /// <summary>
        /// Register this SMS processor during application startup.
        /// </summary>
        public Task InitializeApplicationStartupAsync() {
            YetaWF.Core.Support.SendSMS.SendSMS.Register(this);
            return Task.CompletedTask;
        }
        /// <summary>
        /// The name of the SMS provider.
        /// </summary>
        public string Name { get { return "TwilioProcessor"; } }
        /// <summary>
        /// Returns whether the SMS provider is available.
        /// </summary>
        /// <returns>True if it is available, false otherwise.</returns>
        public async Task<bool> IsAvailableAsync() {
            TwilioData twilioConfig = await TwilioConfigDataProvider.GetConfigCondAsync();
            return twilioConfig != null && twilioConfig.IsSMSConfigured();
        }
        /// <summary>
        /// Returns whether the SMS provider is in test mode.
        /// </summary>
        /// <returns>True if it is in test mode, false otherwise.</returns>
        public async Task<bool> IsTestModeAsync() {
            if (!await IsAvailableAsync()) throw new InternalError("SMS processing not available");
            TwilioData config = await TwilioConfigDataProvider.GetConfigAsync();
            return config.TestMode;
        }

        public async Task SendSMSAsync(string toNumber, string text, string FromNumber) {
            TwilioData config = await TwilioConfigDataProvider.GetConfigAsync();
            string accountSid = config.TestMode ? config.TestAccountSid : config.LiveAccountSid;
            string authToken = config.TestMode ? config.TestAuthToken : config.LiveAuthToken;
            string fromNumber = config.TestMode ? config.TestSMSNumber : config.LiveSMSNumber;
            if (!string.IsNullOrWhiteSpace(FromNumber))
                fromNumber = FromNumber;
            fromNumber = PhoneNumberUSAttribute.GetE164(fromNumber);
            if (fromNumber == null)
                throw new InternalError($"Invalid fromNumber {fromNumber} - {nameof(SendSMSAsync)}");
            toNumber = PhoneNumberUSAttribute.GetE164(toNumber);
            if (toNumber == null)
                throw new InternalError($"Invalid toNumber {toNumber} - {nameof(SendSMSAsync)}");
            string callbackUrl = null;
            if (config.DeliveryReceipts) {
                callbackUrl = YetaWFManager.Manager.CurrentSite.MakeUrl(
                    YetaWFManager.UrlFor(typeof(TwilioResponseController), "Response", new { ValidateToNumber = toNumber }),
                    PagePageSecurity: config.UseHttps ? YetaWF.Core.Pages.PageDefinition.PageSecurityType.httpsOnly : YetaWF.Core.Pages.PageDefinition.PageSecurityType.httpOnly
                );
            }

            TwilioClient.Init(accountSid, authToken);
            MessageResource m = MessageResource.Create(
                from: fromNumber, // From number, must be an SMS-enabled Twilio number
                to: toNumber,
                body: text,
                statusCallback: callbackUrl != null ? new Uri(callbackUrl) : null
            );
            if (m.ErrorCode != null)
                Logging.AddErrorLog("TwilioProcessor SendSMS failed: {0} {1} - {2} - {3}", fromNumber, toNumber, m.ErrorCode, m.ErrorMessage);
            else
                Logging.AddLog("TwilioProcessor SendSMS queued: {0} {1}", fromNumber, toNumber);
        }
    }
}
