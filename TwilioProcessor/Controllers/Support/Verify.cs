/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TwilioProcessor#License */

using System.Collections.Generic;
using System.Text;
using YetaWF.Core.Support;
using YetaWF.Core.Log;
#if MVC6
using Microsoft.AspNetCore.Http;
#else
using System.Web;
#endif
#if !DEBUG
using System;
using System.Linq;
using System.Collections.Specialized;
using Twilio.Security;
#endif

namespace Softelvdm.Modules.TwilioProcessor.Controllers.Support {

    public static class Verify {

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        public static bool VerifyTwilio(string authToken, string verificationRequestUrl) {
#if !DEBUG
            string requestUrl;
            if (string.IsNullOrWhiteSpace(verificationRequestUrl))
                requestUrl = Manager.CurrentRequestUrl;
            else {
                // add querystring
                QueryHelper qh = QueryHelper.FromUrl(Manager.CurrentRequestUrl);
                requestUrl = qh.ToUrl(verificationRequestUrl);
            }

            HttpRequest request = Manager.CurrentRequest;
            string signature = request.Headers["X-Twilio-Signature"];
            RequestValidator validator = new RequestValidator(authToken);

            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (string key in request.Form.Keys) {
                dict.Add(key, request.Form[key]);
            }

            if (!validator.Validate(requestUrl, dict, signature)) {
                // load balancer, etc. may mess up url, dump headers so we can see what to use
                DumpHeaders(requestUrl, dict, signature);
                return false;
            } else {
#if DEBUG
                DumpHeaders(requestUrl, dict, signature, Error: false);
#endif
            }
#endif
            return true;
        }

        public static void DumpHeaders(string usedRequestUrl, Dictionary<string,string> dict, string signature, bool Error = true) {
            StringBuilder sb = new StringBuilder();
            sb.Append($"UsedRequestUrl={usedRequestUrl}\r\n");
            sb.Append($"Signature={signature}\r\n");
            string requestUrl = Manager.CurrentRequestUrl;
            sb.Append($"RequestUrl={requestUrl}\r\n");
            HttpRequest request = Manager.CurrentRequest;
            foreach (string key in request.Headers.Keys) {
                sb.Append($"{key}={request.Headers[key]}\r\n");
            }
            foreach (string key in dict.Keys) {
                sb.Append($"dict[{key}]={dict[key]}\r\n");
            }
            if (Error)
                Logging.AddErrorLog(sb.ToString());
            else
                Logging.AddLog(sb.ToString());

        }
    }
}
