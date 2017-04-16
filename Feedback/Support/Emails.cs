/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using System;
using YetaWF.Core.Localize;
using YetaWF.Core.SendEmail;
using YetaWF.Core.Support;
using YetaWF.Modules.Feedback.Controllers;

namespace YetaWF.Modules.Feedback.Support {

    public class Emails {

        public Emails(YetaWFManager manager) { Manager = manager; }

        protected static YetaWFManager Manager { get; private set; }

        public void SendFeedback(string toEmail, string fromEmail, string subject, string message, string ccEmail = null) {

            if (string.IsNullOrWhiteSpace(toEmail) && string.IsNullOrWhiteSpace(ccEmail))
                return;

            if (string.IsNullOrWhiteSpace(fromEmail))
                fromEmail = this.__ResStr("anonymous", "(Anonymous User)");

            SendEmail sendEmail = new SendEmail();
            object parms = new {
                FromEmail = fromEmail,
                Subject = subject,
                Message = message,
                IPAddress = Manager.UserHostAddress,
                DateTime = Formatting.FormatDateTime(DateTime.UtcNow),
            };

            subject = this.__ResStr("feedbackSubject2", "Feedback \"{0}\" from {1} - {2}", subject, fromEmail, Manager.CurrentSite.SiteDomain);
            if (string.IsNullOrWhiteSpace(toEmail)) {
                sendEmail.PrepareEmailMessage(ccEmail, subject, sendEmail.GetEmailFile(AreaRegistration.CurrentPackage, "Feedback Email.txt"), parameters: parms);
            } else {
                sendEmail.PrepareEmailMessage(toEmail, subject, sendEmail.GetEmailFile(AreaRegistration.CurrentPackage, "Feedback Email.txt"), parameters: parms);
                if (!string.IsNullOrWhiteSpace(ccEmail))
                    sendEmail.AddBcc(ccEmail);
            }
            sendEmail.Send(true);
        }
    }
}