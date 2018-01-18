/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Support.TwoStepAuthorization;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class LoginTwoStepController : YetaWFController {

        public const string IDENTITY_TWOSTEP_USERID = "YetaWF_Identity-Login-TwoStep";
        public const string IDENTITY_TWOSTEP_NEXTURL = "YetaWF_Identity-Login-NextUrl";
        public const string IDENTITY_TWOSTEP_CLOSEONLOGIN = "YetaWF_Identity-Login-CloseOnLogin";

        /// <summary>
        /// Log in a user after two-step authentication
        /// </summary>
        [AllowGet]
        public ActionResult Login() {

            // verify that the user already entered the name/password correctly
            int userId = Manager.SessionSettings.SiteSettings.GetValue<int>(IDENTITY_TWOSTEP_USERID, 0);
            bool closeOnLogin = Manager.SessionSettings.SiteSettings.GetValue<bool>(IDENTITY_TWOSTEP_CLOSEONLOGIN, false);
            string returnUrl = Manager.SessionSettings.SiteSettings.GetValue<string>(IDENTITY_TWOSTEP_NEXTURL);
            Manager.SessionSettings.SiteSettings.ClearValue(LoginTwoStepController.IDENTITY_TWOSTEP_USERID);
            Manager.SessionSettings.SiteSettings.ClearValue(LoginTwoStepController.IDENTITY_TWOSTEP_NEXTURL);
            Manager.SessionSettings.SiteSettings.ClearValue(LoginTwoStepController.IDENTITY_TWOSTEP_CLOSEONLOGIN);
            Manager.SessionSettings.SiteSettings.Save();
            if (userId == 0) return Redirect(Manager.CurrentSite.HomePageUrl);

            // verify that the TwoStepAuthorization provider just verified this user
            TwoStepAuth twoStep = new TwoStepAuth();
            if (!twoStep.VerifyTwoStepAutheticationDone(userId))
                return Redirect(Manager.CurrentSite.HomePageUrl);
            twoStep.ClearTwoStepAuthetication(userId);

            Resource.ResourceAccess.LoginAs(userId);

            if (closeOnLogin)
                return View("YetaWF_Identity_TwoStepDone");
            else {
                returnUrl = QueryHelper.AddRando(returnUrl); // to defeat client-side caching
                return Redirect(returnUrl);
            }
        }
    }
}