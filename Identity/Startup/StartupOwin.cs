/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

#if MVC6
#else

using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security;
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
                long ticks = WebConfigHelper.GetValue<long>(area, "OWin:ExpireTimeSpan");
                System.TimeSpan expireTimeSpan = new System.TimeSpan(ticks);
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
                // site sets up an account at https://apps.dev.microsoft.com/
                // a user can remove a site's permission here https://account.live.com/consent/Manage
                // redirect url is http://..yourdomain.com../signin-microsoft
                publicKey = WebConfigHelper.GetValue<string>(area, "MicrosoftAccount:Public");
                privateKey = WebConfigHelper.GetValue<string>(area, "MicrosoftAccount:Private");
                if (!string.IsNullOrWhiteSpace(publicKey) && !string.IsNullOrWhiteSpace(privateKey))
                    app.UseMicrosoftAccountAuthentication(publicKey, privateKey);

                // FACEBOOK
                // site sets up an account at https://developers.facebook.com/apps
                // redirect url is http://..yourdomain.com../signin-facebook   (this does not appear to be used by Facebook)
                publicKey = WebConfigHelper.GetValue<string>(area, "FacebookAccount:Public");
                privateKey = WebConfigHelper.GetValue<string>(area, "FacebookAccount:Private");
                if (!string.IsNullOrWhiteSpace(publicKey) && !string.IsNullOrWhiteSpace(privateKey))
                    app.UseFacebookAuthentication(publicKey, privateKey);

                // GOOGLE
                // site sets up an account at https://console.developers.google.com/
                // a user can remove a site's permission here https://security.google.com/settings/security/permissions
                // redirect url is http://..yourdomain.com../signin-google
                publicKey = WebConfigHelper.GetValue<string>(area, "GoogleAccount:Public");
                privateKey = WebConfigHelper.GetValue<string>(area, "GoogleAccount:Private");
                if (!string.IsNullOrWhiteSpace(publicKey) && !string.IsNullOrWhiteSpace(privateKey))
                    app.UseGoogleAuthentication(publicKey, privateKey);

                // TWITTER
                // site sets up an account at https://apps.twitter.com/
                publicKey = WebConfigHelper.GetValue<string>(area, "TwitterAccount:Public");
                privateKey = WebConfigHelper.GetValue<string>(area, "TwitterAccount:Private");
                if (!string.IsNullOrWhiteSpace(publicKey) && !string.IsNullOrWhiteSpace(privateKey)) {
                    // http://stackoverflow.com/questions/25011890/owin-twitter-login-the-remote-certificate-is-invalid-according-to-the-validati
                    // see http://stackoverflow.com/questions/21651211/asp-net-mvc-5-owin-twitter-auth-throwing-401-exception
                    app.UseTwitterAuthentication(new Microsoft.Owin.Security.Twitter.TwitterAuthenticationOptions {
                        ConsumerKey = publicKey,
                        ConsumerSecret = privateKey,
                        BackchannelCertificateValidator = new CertificateSubjectKeyIdentifierValidator(new[] {
                            "A5EF0B11CEC04103A34A659048B21CE0572D7D47", // VeriSign Class 3 Secure Server CA - G2
                            "0D445C165344C1827E1D20AB25F40163D8BE79A5", // VeriSign Class 3 Secure Server CA - G3
                            "7FD365A7C2DDECBBF03009F34339FA02AF333133", // VeriSign Class 3 Public Primary Certification Authority - G5
                            "39A55D933676616E73A761DFA16A7E59CDE66FAD", // Symantec Class 3 Secure Server CA - G4
                            "5168FF90AF0207753CCCD9656462A212B859723B", //DigiCert SHA2 High Assurance Server C‎A
                            "B13EC36903F8BF4701D498261A0802EF63642BC3" //DigiCert High Assurance EV Root CA
                        }),
                    });
                }
            }
        }
    }
}
#endif
