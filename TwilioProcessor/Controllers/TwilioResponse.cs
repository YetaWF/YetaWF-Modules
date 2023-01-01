/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TwilioProcessor#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Log;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace Softelvdm.Modules.TwilioProcessor.Controllers {

    public class TwilioResponseController : YetaWFController {

        public TwilioResponseController() { }

        /// <summary>
        /// Processes a Twilio delivery receipt indicating the status of an SMS message.
        /// </summary>
        /// <returns>An empty action result.</returns>
        /// <remarks>
        /// For details about all parameters see the Twilio documentation at https://www.twilio.com/docs/api/rest/sending-messages.
        /// </remarks>
        [AllowGet]
        public new ActionResult Response(string ValidateToNumber, string To, string From, string MessageSid, string MessageStatus, string ErrorCode) {
            if (ValidateToNumber != To) {
                Logging.AddErrorLog("Response from Twilio: Invalid (to number doesn't match) - {0}", Manager.CurrentRequestUrl);
            } else {
                if (ErrorCode != "0")
                    Logging.AddErrorLog("Response from Twilio: {0} {1} {2} {3} {4} {5}", From, To, ErrorCode, MessageStatus, MessageSid, Manager.CurrentRequestUrl);
                else
                    Logging.AddLog("Response from Twilio: {0} {1} {2} {3} {4} {5}", From, To, ErrorCode, MessageStatus, MessageSid, Manager.CurrentRequestUrl);
            }
            return new EmptyResult();
        }
    }
}