/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using YetaWF.Core.Support;

[assembly: OwinStartupAttribute(typeof(YetaWF.Modules.Identity.Startup))]
namespace YetaWF.Modules.Identity {
    public class Startup
    {
        const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void Configuration(IAppBuilder app) {

            // This may get initialized before our Core Components so we can't use the Package type
            // Package package = AreaRegistration.CurrentPackage;
            // string area = "YetaWF_Identity";
            string area = "YetaWF_Identity";

            string authType = WebConfigHelper.GetValue<string>(area, "OWin:AuthenticationType"); // "ApplicationCookie"
            if (!string.IsNullOrWhiteSpace(authType)) {
                System.TimeSpan expireTimeSpan = WebConfigHelper.GetValue<System.TimeSpan>(area, "OWin:ExpireTimeSpan"); // new System.TimeSpan(365, 0, 0, 0, 0)
                bool slidingExpiration = WebConfigHelper.GetValue<bool>(area, "OWin:SlidingExpiration"); // true

                // Enable the application to use a cookie to store information for the signed in user
                // the cookie name is YetaWF instance specific (this allows domains with different subdomains to be used in different IIS sites)
                app.UseCookieAuthentication(new CookieAuthenticationOptions {
                    AuthenticationType = authType,
                    ExpireTimeSpan = expireTimeSpan,
                    ReturnUrlParameter = "ReturnUrl",
                    SlidingExpiration = slidingExpiration,
                    CookieSecure = CookieSecureOption.Never,
                    CookieName = string.Format(".YetaWF.Cookies.{0}", YetaWFManager.DefaultSiteName),
                });
                // Use a cookie to temporarily store information about a user logging in with a third party login provider
                app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

                string publicKey, privateKey;
                // MICROSOFT
                // site sets up an account at https://account.live.com/developers/applications/create
                // a user can remove a site's permission here https://account.live.com/consent/Manage
                // redirect url is http://..yourdomain.com../signin-microsoft
                publicKey = WebConfigHelper.GetValue<string>(area, "MicrosoftAccount:Public");
                privateKey = WebConfigHelper.GetValue<string>(area, "MicrosoftAccount:Private");
                if (!string.IsNullOrWhiteSpace(publicKey) && !string.IsNullOrWhiteSpace(privateKey))
                    app.UseMicrosoftAccountAuthentication(publicKey, privateKey);

                // FACEBOOK
                // site sets up an account at https://developers.facebook.com/apps
                // redirect url is http://..yourdomain.com../signin-facebook   (this does not appear to be used by Facebook)
                publicKey = WebConfigHelper.GetValue<string>(area, "Facebook:Public");
                privateKey = WebConfigHelper.GetValue<string>(area, "Facebook:Private");
                if (!string.IsNullOrWhiteSpace(publicKey) && !string.IsNullOrWhiteSpace(privateKey))
                    app.UseFacebookAuthentication(publicKey, privateKey);

                // GOOGLE
                // site sets up an account at https://console.developers.google.com/
                // a user can remove a site's permission here https://security.google.com/settings/security/permissions
                // redirect url is http://..yourdomain.com../signin-google
                publicKey = WebConfigHelper.GetValue<string>(area, "Google:Public");
                privateKey = WebConfigHelper.GetValue<string>(area, "Google:Private");
                if (!string.IsNullOrWhiteSpace(publicKey) && !string.IsNullOrWhiteSpace(privateKey))
                    app.UseGoogleAuthentication(publicKey, privateKey);

            }
        }
    }
}