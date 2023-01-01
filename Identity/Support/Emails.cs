/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.SendEmail;
using YetaWF.Core.Support;
using YetaWF.Core.Support.UrlHistory;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Support {

    public class Emails {

        protected YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        public const string EmailsFolder = "Emails";

        private string SendingEmailAddress = null;

        public Emails() { }

        public string GetSendingEmailAddress() {
            return SendingEmailAddress;
        }

        public async Task SendForgottenEmailAsync(UserDefinition user, string ccEmail = null) {
            SendEmail sendEmail = new SendEmail();
            object parms = new {
                User = user,
                LoginUrl = Manager.CurrentSite.MakeUrl(Manager.CurrentSite.LoginUrl),
            };
            string subject = this.__ResStr("forgotSubject", "Forgotten password for {0}", Manager.CurrentSite.SiteDomain);
            await sendEmail.PrepareEmailMessageAsync(user.Email, subject, await sendEmail.GetEmailFileAsync(Package.GetCurrentPackage(this), "Forgot Password.txt"), Parameters: parms);
            if (!string.IsNullOrWhiteSpace(ccEmail))
                sendEmail.AddBcc(ccEmail);
            await sendEmail.SendAsync(true);
            SendingEmailAddress = await sendEmail.GetSendingEmailAddressAsync();
        }
        public async Task SendPasswordResetEmailAsync(UserDefinition user, string ccEmail = null) {
            ResetPasswordModule resetMod = (ResetPasswordModule)await ModuleDefinition.CreateUniqueModuleAsync(typeof(ResetPasswordModule));
            ModuleAction reset = resetMod.GetAction_ResetPassword(null, user.UserId, user.ResetKey.ToString());

            SendEmail sendEmail = new SendEmail();
            object parms = new {
                User = user,
                ResetUrl = reset.GetCompleteUrl(),
                ResetKey = user.ResetKey.ToString(),
                ValidUntil = Formatting.FormatDateTime(user.ResetValidUntil),
            };
            string subject = this.__ResStr("resetSubject", "Password Reset for {0}", Manager.CurrentSite.SiteDomain);
            await sendEmail.PrepareEmailMessageAsync(user.Email, subject, await sendEmail.GetEmailFileAsync(Package.GetCurrentPackage(this), "Password Reset.txt"), Parameters: parms);
            if (!string.IsNullOrWhiteSpace(ccEmail))
                sendEmail.AddBcc(ccEmail);
            await sendEmail.SendAsync(true);
            SendingEmailAddress = await sendEmail.GetSendingEmailAddressAsync();
        }
        public async Task SendVerificationAsync(UserDefinition user, string ccEmail = null) {

            string retUrl = Manager.ReturnToUrl;
            string urlOnly;
            QueryHelper qh = QueryHelper.FromUrl(Manager.CurrentSite.LoginUrl, out urlOnly);
            qh.Add("Name", user.UserName, Replace: true);
            qh.Add("V", user.VerificationCode, Replace: true);
            string urlNoOrigin = qh.ToUrl(urlOnly);
            qh.Add("CloseOnLogin", "true", Replace: true);
            qh.Add(Globals.Link_OriginList, Utility.JsonSerialize(new List<Origin> { new Origin { Url = retUrl } }), Replace: true);
            string url = qh.ToUrl(urlOnly);

            SendEmail sendEmail = new SendEmail();
            object parms = new {
                User = user,
                Url = Manager.CurrentSite.MakeUrl(url),
                UrlNoOrigin = Manager.CurrentSite.MakeUrl(urlNoOrigin),
                Code = user.VerificationCode,
            };
            string subject = this.__ResStr("verificationSubject", "Verification required for site {0}", Manager.CurrentSite.SiteDomain);
            await sendEmail.PrepareEmailMessageAsync(user.Email, subject, await sendEmail.GetEmailFileAsync(Package.GetCurrentPackage(this), "Account Verification.txt"), Parameters: parms);
            if (!string.IsNullOrWhiteSpace(ccEmail))
                sendEmail.AddBcc(ccEmail);
            await sendEmail.SendAsync(true);
            SendingEmailAddress = await sendEmail.GetSendingEmailAddressAsync();
        }

        public async Task SendApprovalAsync(UserDefinition user, string ccEmail = null) {
            SendEmail sendEmail = new SendEmail();
            object parms = new {
                User = user,
                LoginUrl = Manager.CurrentSite.MakeUrl(Manager.CurrentSite.LoginUrl),
            };
            string subject = this.__ResStr("approvalSubject", "Approved for site {0}!", Manager.CurrentSite.SiteDomain);
            await sendEmail.PrepareEmailMessageAsync(user.Email, subject, await sendEmail.GetEmailFileAsync(Package.GetCurrentPackage(this), "Account Approved.txt"), Parameters: parms);
            if (!string.IsNullOrWhiteSpace(ccEmail))
                sendEmail.AddBcc(ccEmail);
            await sendEmail.SendAsync(true);
            SendingEmailAddress = await sendEmail.GetSendingEmailAddressAsync();
        }

        public async Task SendApprovalNeededAsync(UserDefinition user) {
            // get the registration module for some defaults
            RegisterModule regMod = (RegisterModule)await ModuleDefinition.CreateUniqueModuleAsync(typeof(RegisterModule));
            ModuleAction approve = await regMod.GetAction_ApproveAsync(user.UserName);
            ModuleAction reject = await regMod.GetAction_RejectAsync(user.UserName);

            SendEmail sendEmail = new SendEmail();
            object parms = new {
                User = user,
                ApprovalUrl = approve.GetCompleteUrl(),
                RejectUrl = reject.GetCompleteUrl(),
            };
            string subject = this.__ResStr("approvalNeededSubject", "Approval required for user {0} - site {1}", user.UserName, Manager.CurrentSite.SiteDomain);
            await sendEmail.PrepareEmailMessageAsync(null, subject, await sendEmail.GetEmailFileAsync(Package.GetCurrentPackage(this), "Account Approval.txt"), Parameters: parms);
            await sendEmail.SendAsync(false);
            SendingEmailAddress = await sendEmail.GetSendingEmailAddressAsync();
        }

        public async Task SendRejectedAsync(UserDefinition user, string ccEmail = null) {
            SendEmail sendEmail = new SendEmail();
            object parms = new {
                User = user,
            };
            string subject = this.__ResStr("rejectedSubject", "Your account for {0}", Manager.CurrentSite.SiteDomain);
            await sendEmail.PrepareEmailMessageAsync(user.Email, subject, await sendEmail.GetEmailFileAsync(Package.GetCurrentPackage(this), "Account Rejected.txt"), Parameters: parms);
            if (!string.IsNullOrWhiteSpace(ccEmail))
                sendEmail.AddBcc(ccEmail);
            await sendEmail.SendAsync(true);
            SendingEmailAddress = await sendEmail.GetSendingEmailAddressAsync();
        }

        public async Task SendSuspendedAsync(UserDefinition user, string ccEmail = null) {
            SendEmail sendEmail = new SendEmail();
            object parms = new {
                User = user,
            };
            string subject = this.__ResStr("suspendedSubject", "Your account for {0}", Manager.CurrentSite.SiteDomain);
            await sendEmail.PrepareEmailMessageAsync(user.Email, subject, await sendEmail.GetEmailFileAsync(Package.GetCurrentPackage(this), "Account Suspended.txt"), Parameters: parms);
            if (!string.IsNullOrWhiteSpace(ccEmail))
                sendEmail.AddBcc(ccEmail);
            await sendEmail.SendAsync(true);
            SendingEmailAddress = await sendEmail.GetSendingEmailAddressAsync();
        }

        public async Task SendNewUserCreatedAsync(UserDefinition user) {
            // get the registration module for some defaults
            RegisterModule regMod = (RegisterModule)await ModuleDefinition.CreateUniqueModuleAsync(typeof(RegisterModule));
            ModuleAction reject = await regMod.GetAction_RejectAsync(user.UserName);

            SendEmail sendEmail = new SendEmail();
            object parms = new {
                User = user,
                RejectUrl = reject.GetCompleteUrl(),
            };
            string subject = this.__ResStr("notifyNewUserSubject", "New account for user {0} - site  {1}", user.UserName, Manager.CurrentSite.SiteDomain);
            await sendEmail.PrepareEmailMessageAsync(null, subject, await sendEmail.GetEmailFileAsync(Package.GetCurrentPackage(this), "New Account Created.txt"), Parameters: parms);
            await sendEmail.SendAsync(false);
            SendingEmailAddress = await sendEmail.GetSendingEmailAddressAsync();
        }
    }
}
