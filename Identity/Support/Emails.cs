/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.SendEmail;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Support {

    public class Emails {

        protected YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        public const string EmailsFolder = "Emails";

        public Emails() { }

        public void SendForgottenEmail(UserDefinition user, string ccEmail = null) {
            SendEmail sendEmail = new SendEmail();
            object parms = new {
                User = user,
                LoginUrl = Manager.CurrentSite.MakeUrl(Manager.CurrentSite.LoginUrl),
            };
            string subject = this.__ResStr("forgotSubject", "Forgotten password for {0}", Manager.CurrentSite.SiteDomain);
            sendEmail.PrepareEmailMessage(user.Email, subject, sendEmail.GetEmailFile(Package.GetCurrentPackage(this), "Forgot Password.txt"), parameters: parms);
            if (!string.IsNullOrWhiteSpace(ccEmail))
                sendEmail.AddBcc(ccEmail);
            sendEmail.Send(true);
        }
        public void SendVerification(UserDefinition user, string ccEmail = null) {
            SendEmail sendEmail = new SendEmail();
            object parms = new {
                User = user,
                LoginUrl = Manager.CurrentSite.MakeUrl(Manager.CurrentSite.LoginUrl),
            };
            string subject = this.__ResStr("verificationSubject", "Verification required for site {0}", Manager.CurrentSite.SiteDomain);
            sendEmail.PrepareEmailMessage(user.Email, subject, sendEmail.GetEmailFile(Package.GetCurrentPackage(this), "Account Verification.txt"), parameters: parms);
            if (!string.IsNullOrWhiteSpace(ccEmail))
                sendEmail.AddBcc(ccEmail);
            sendEmail.Send(true);
        }

        public void SendApproval(UserDefinition user, string ccEmail = null) {
            SendEmail sendEmail = new SendEmail();
            object parms = new {
                User = user,
                LoginUrl = Manager.CurrentSite.MakeUrl(Manager.CurrentSite.LoginUrl),
            };
            string subject = this.__ResStr("approvalSubject", "Approved for site {0}!", Manager.CurrentSite.SiteDomain);
            sendEmail.PrepareEmailMessage(user.Email, subject, sendEmail.GetEmailFile(Package.GetCurrentPackage(this), "Account Approved.txt"), parameters: parms);
            if (!string.IsNullOrWhiteSpace(ccEmail))
                sendEmail.AddBcc(ccEmail);
            sendEmail.Send(true);
        }

        public void SendApprovalNeeded(UserDefinition user) {
            // get the registration module for some defaults
            RegisterModule regMod = (RegisterModule) ModuleDefinition.CreateUniqueModule(typeof(RegisterModule));
            ModuleAction approve = regMod.GetAction_Approve(user.UserName);
            ModuleAction reject = regMod.GetAction_Reject(user.UserName);

            SendEmail sendEmail = new SendEmail();
            object parms = new {
                User = user,
                ApprovalUrl = approve.GetCompleteUrl(),
                RejectUrl = reject.GetCompleteUrl(),
            };
            string subject = this.__ResStr("approvalNeededSubject", "Approval required for user {0} - site {1}", user.UserName, Manager.CurrentSite.SiteDomain);
            sendEmail.PrepareEmailMessage(null, subject, sendEmail.GetEmailFile(Package.GetCurrentPackage(this), "Account Approval.txt"), parameters: parms);
            sendEmail.Send(true);
        }

        public void SendRejected(UserDefinition user, string ccEmail = null) {
            SendEmail sendEmail = new SendEmail();
            object parms = new {
                User = user,
            };
            string subject = this.__ResStr("rejectedSubject", "Your account for {0}", Manager.CurrentSite.SiteDomain);
            sendEmail.PrepareEmailMessage(user.Email, subject, sendEmail.GetEmailFile(Package.GetCurrentPackage(this), "Account Rejected.txt"), parameters: parms);
            if (!string.IsNullOrWhiteSpace(ccEmail))
                sendEmail.AddBcc(ccEmail);
            sendEmail.Send(true);
        }

        public void SendSuspended(UserDefinition user, string ccEmail = null) {
            SendEmail sendEmail = new SendEmail();
            object parms = new {
                User = user,
            };
            string subject = this.__ResStr("suspendedSubject", "Your account for {0}", Manager.CurrentSite.SiteDomain);
            sendEmail.PrepareEmailMessage(user.Email, subject, sendEmail.GetEmailFile(Package.GetCurrentPackage(this), "Account Suspended.txt"), parameters: parms);
            if (!string.IsNullOrWhiteSpace(ccEmail))
                sendEmail.AddBcc(ccEmail);
            sendEmail.Send(true);
        }

        public void SendNewUserCreated(UserDefinition user) {
            // get the registration module for some defaults
            RegisterModule regMod = (RegisterModule) ModuleDefinition.CreateUniqueModule(typeof(RegisterModule));
            ModuleAction reject = regMod.GetAction_Reject(user.UserName);

            SendEmail sendEmail = new SendEmail();
            object parms = new {
                User = user,
                RejectUrl = reject.GetCompleteUrl(),
            };
            string subject = this.__ResStr("notifyNewUserSubject", "New account for user {0} - site  {1}", user.UserName, Manager.CurrentSite.SiteDomain);
            sendEmail.PrepareEmailMessage(null, subject, sendEmail.GetEmailFile(Package.GetCurrentPackage(this), "New Account Created.txt"), parameters: parms);
            sendEmail.Send(true);
        }
    }
}