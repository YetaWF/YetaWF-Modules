/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Localize;
using YetaWF.Core.SendEmail;
using YetaWF.Core.Support;
using YetaWF.Modules.Feedback.Controllers;

namespace YetaWF.Modules.Feedback.Support {

    public class Emails {

        public Emails(YetaWFManager manager) { Manager = manager; }

        protected static YetaWFManager Manager { get; private set; }

        public async Task SendFeedbackAsync(string toEmail, string fromEmail, string subject, string message, string name, string ccEmail = null) {

            if (string.IsNullOrWhiteSpace(toEmail) && string.IsNullOrWhiteSpace(ccEmail))
                return;

            if (string.IsNullOrWhiteSpace(fromEmail))
                fromEmail = this.__ResStr("anonymous", "(Anonymous User)");

            SendEmail sendEmail = new SendEmail();
            object parms = new {
                FromEmail = fromEmail,
                Subject = subject,
                Message = message,
                Name = name,
                IPAddress = Manager.UserHostAddress,
                DateTime = Formatting.FormatDateTime(DateTime.UtcNow),
            };

            subject = this.__ResStr("feedbackSubject2", "Feedback \"{0}\" from {1} - {2}", subject, fromEmail, Manager.CurrentSite.SiteDomain);
            if (string.IsNullOrWhiteSpace(toEmail)) {
                await sendEmail.PrepareEmailMessageAsync(ccEmail, subject, await sendEmail.GetEmailFileAsync(AreaRegistration.CurrentPackage, "Feedback Email.txt"), Parameters: parms);
            } else {
                await sendEmail.PrepareEmailMessageAsync(toEmail, subject, await sendEmail.GetEmailFileAsync(AreaRegistration.CurrentPackage, "Feedback Email.txt"), Parameters: parms);
                if (!string.IsNullOrWhiteSpace(ccEmail))
                    sendEmail.AddBcc(ccEmail);
            }
            await sendEmail.SendAsync(true);
        }
    }
}