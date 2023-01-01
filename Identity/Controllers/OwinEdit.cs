/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using System.Threading.Tasks;
using YetaWF.Core.Audit;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class OwinEditModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.OwinEditModule> {

        public OwinEditModuleController() { }

        [Trim]
        public class EditModel {

            public const int MaxKey = 200;

            public EditModel() {
                MicrosoftUrl = "https://portal.azure.com/#blade/Microsoft_AAD_RegisteredApps/ApplicationsListBlade"; // was "https://account.live.com/developers/applications/";
                GoogleUrl = "https://console.developers.google.com/apis/credentials"; // general "https://developers.google.com/identity/sign-in/web";
                FacebookUrl = "https://developers.facebook.com/apps";
                TwitterUrl = "https://developer.twitter.com/apps";
            }

            [Category("Built-In Login"), Caption("Password Renewal"), Description("The time after which a user must change the password - Set to 0 to allow passwords to remain valid indefinitely")]
            [UIHint("TimeSpanDH"), Required]
            public TimeSpan PasswordRenewal { get; set; }

            [Category("Built-In Login"), Caption("Password Settings:"), Description("")]
            [UIHint("String"), ReadOnly]
            public string Nothing { get; set; }

            [Category("Built-In Login"), Caption("Minimum Length"), Description("Defines the minimum length of a password")]
            [UIHint("IntValue2"), Required, Range(4, 24)]
            public int RequiredLength { get; set; }

            [Category("Built-In Login"), Caption("Requires Digit"), Description("Defines whether a password requires at least one digit")]
            [UIHint("Boolean")]
            public bool RequireDigit { get; set; }

            [Category("Built-In Login"), Caption("Requires Non-Alphanumeric"), Description("Defines whether a password requires at least one non alphanumeric (special) character")]
            [UIHint("Boolean")]
            public bool RequireNonAlphanumeric { get; set; }

            [Category("Built-In Login"), Caption("Requires Uppercase"), Description("Defines whether a password requires at least one uppercase character")]
            [UIHint("Boolean")]
            public bool RequireUppercase { get; set; }

            [Category("Built-In Login"), Caption("Requires Lowercase"), Description("Defines whether a password requires at least one lowercase character")]
            [UIHint("Boolean")]
            public bool RequireLowercase { get; set; }

            [Category("External Login"), Caption("Use Facebook"), Description("Defines whether Facebook logins are allowed")]
            [UIHint("Boolean")]
            public bool UseFacebook { get; set; }
            [Category("External Login"), Caption("Facebook"), Description("Provides a link to Facebook to set up authentication for your sites")]
            [UIHint("Url"), ReadOnly]
            public string FacebookUrl { get; set; }
            [Category("External Login"), Caption("Facebook App ID"), Description("The App ID for authentication using Facebook (defined in Facebook)")]
            [UIHint("Text80"), StringLength(MaxKey), Trim]
            [RequiredIf(nameof(UseFacebook), true)]
            [ExcludeDemoMode]
            public string FacebookPublic { get; set; }
            [Category("External Login"), Caption("Facebook App Secret"), Description("The App Secret for authentication using Facebook (defined in Facebook)")]
            [UIHint("Text80"), RequiredIfSuppliedAttribute("FacebookPublic"), StringLength(MaxKey), Trim]
            [RequiredIf(nameof(UseFacebook), true)]
            [ExcludeDemoMode]
            public string FacebookPrivate { get; set; }

            [Category("External Login"), Caption("Use Google"), Description("Defines whether Google logins are allowed")]
            [UIHint("Boolean")]
            public bool UseGoogle { get; set; }
            [Category("External Login"), Caption("Google"), Description("Provides a link to Google to set up authentication for your sites")]
            [UIHint("Url"), ReadOnly]
            public string GoogleUrl { get; set; }
            [Category("External Login"), Caption("Google Client ID"), Description("The Client ID for authentication using Google (defined using Google)")]
            [UIHint("Text80"), StringLength(MaxKey), Trim]
            [RequiredIf(nameof(UseGoogle), true)]
            [ExcludeDemoMode]
            public string GooglePublic { get; set; }
            [Category("External Login"), Caption("Google Client Secret"), Description("The Client Secret for authentication using Google (defined using Google)")]
            [UIHint("Text80"), RequiredIfSuppliedAttribute("GooglePublic"), StringLength(MaxKey), Trim]
            [RequiredIf(nameof(UseGoogle), true)]
            [ExcludeDemoMode]
            public string GooglePrivate { get; set; }

            [Category("External Login"), Caption("Use Microsoft"), Description("Defines whether Microsoft logins are allowed")]
            [UIHint("Boolean")]
            public bool UseMicrosoft { get; set; }
            [Category("External Login"), Caption("Microsoft"), Description("Provides a link to Microsoft to set up authentication for your sites")]
            [UIHint("Url"), ReadOnly]
            public string MicrosoftUrl { get; set; }
            [Category("External Login"), Caption("Microsoft Application ID"), Description("The Application (client) ID for authentication using Microsoft")]
            [UIHint("Text80"), StringLength(MaxKey), Trim]
            [RequiredIf(nameof(UseMicrosoft), true)]
            [ExcludeDemoMode]
            public string MicrosoftPublic { get; set; }
            [Category("External Login"), Caption("Microsoft Client Secret"), Description("The Client Secret for authentication using Microsoft")]
            [UIHint("Text80"), RequiredIfSuppliedAttribute("MicrosoftPublic"), StringLength(MaxKey), Trim]
            [RequiredIf(nameof(UseMicrosoft), true)]
            [ExcludeDemoMode]
            public string MicrosoftPrivate { get; set; }

            [Category("External Login"), Caption("Use Twitter"), Description("Defines whether Twitter logins are allowed")]
            [UIHint("Boolean")]
            public bool UseTwitter { get; set; }
            [Category("External Login"), Caption("Twitter"), Description("Provides a link to Twitter to set up authentication for your sites")]
            [UIHint("Url"), ReadOnly]
            public string TwitterUrl { get; set; }
            [Category("External Login"), Caption("Twitter API Key"), Description("The API Key for authentication using Twitter (defined using Twitter)")]
            [UIHint("Text80"), StringLength(MaxKey), Trim]
            [RequiredIf(nameof(UseTwitter), true)]
            [ExcludeDemoMode]
            public string TwitterPublic { get; set; }
            [Category("External Login"), Caption("Twitter API Secret Key"), Description("The API Secret Key for authentication using Twitter (defined using Twitter)")]
            [UIHint("Text80"), RequiredIfSuppliedAttribute("TwitterPublic"), StringLength(MaxKey), Trim]
            [RequiredIf(nameof(UseTwitter), true)]
            [ExcludeDemoMode]
            public string TwitterPrivate { get; set; }

            [Category("Cookies"), Caption("Sliding Expiration"), Description("Cookies are reissued with a new expiration time anytime a request is processed whose cookie is more than halfway through the expiration window")]
            [UIHint("Boolean")]
            public bool SlidingExpiration { get; set; }

            [Category("Cookies"), Caption("Expiration"), Description("The time a cookie will remain valid from the point it is created")]
            [UIHint("TimeSpanDHM"), Required]
            public TimeSpan ExpireTimeSpan { get; set; }

            [Category("Cookies"), Caption("Security Interval"), Description("The time after which a logged on user is re-validated (in case the password changed) to insure the authenticated user is still valid")]
            [UIHint("TimeSpanDHM"), Required]
            public TimeSpan SecurityStampValidationInterval { get; set; }
        }

        [AllowGet]
        public ActionResult OwinEdit() {
            EditModel model = new EditModel { };

            model.RequireDigit = OwinConfigHelper.GetValue<bool>(Module.AreaName, "Password:RequireDigit", false);
            model.RequiredLength = OwinConfigHelper.GetValue<int>(Module.AreaName, "Password:RequiredLength", 6);
            model.RequireNonAlphanumeric = OwinConfigHelper.GetValue<bool>(Module.AreaName, "Password:RequireNonAlphanumeric", false);
            model.RequireUppercase = OwinConfigHelper.GetValue<bool>(Module.AreaName, "Password:RequireUppercase", false);
            model.RequireLowercase = OwinConfigHelper.GetValue<bool>(Module.AreaName, "Password:RequireLowercase", false);

            model.SlidingExpiration = OwinConfigHelper.GetValue<bool>(Module.AreaName, "OWin:SlidingExpiration", true);
            long ticks = OwinConfigHelper.GetValue<long>(Module.AreaName, "OWin:ExpireTimeSpan", new TimeSpan(10, 0, 0, 0).Ticks); // 10 days
            model.ExpireTimeSpan = new TimeSpan(ticks);
            ticks = OwinConfigHelper.GetValue<long>(Module.AreaName, "OWin:SecurityStampValidationInterval", new TimeSpan(0, 30, 0).Ticks); // 30 minutes
            model.SecurityStampValidationInterval = new TimeSpan(ticks);
            ticks = OwinConfigHelper.GetValue<long>(Module.AreaName, "PasswordRenewal", new TimeSpan(0, 0, 0).Ticks); // 0  = indefinitely
            model.PasswordRenewal = new TimeSpan(ticks);

            model.UseMicrosoft = OwinConfigHelper.GetValue<bool>(Module.AreaName, "MicrosoftAccount:Enabled");
            model.MicrosoftPublic = OwinConfigHelper.GetValue<string>(Module.AreaName, "MicrosoftAccount:Public");
            model.MicrosoftPrivate = OwinConfigHelper.GetValue<string>(Module.AreaName, "MicrosoftAccount:Private");
            model.UseGoogle = OwinConfigHelper.GetValue<bool>(Module.AreaName, "GoogleAccount:Enabled");
            model.GooglePublic = OwinConfigHelper.GetValue<string>(Module.AreaName, "GoogleAccount:Public");
            model.GooglePrivate = OwinConfigHelper.GetValue<string>(Module.AreaName, "GoogleAccount:Private");
            model.UseFacebook = OwinConfigHelper.GetValue<bool>(Module.AreaName, "FacebookAccount:Enabled");
            model.FacebookPublic = OwinConfigHelper.GetValue<string>(Module.AreaName, "FacebookAccount:Public");
            model.FacebookPrivate = OwinConfigHelper.GetValue<string>(Module.AreaName, "FacebookAccount:Private");
            model.UseTwitter = OwinConfigHelper.GetValue<bool>(Module.AreaName, "TwitterAccount:Enabled");
            model.TwitterPublic = OwinConfigHelper.GetValue<string>(Module.AreaName, "TwitterAccount:Public");
            model.TwitterPrivate = OwinConfigHelper.GetValue<string>(Module.AreaName, "TwitterAccount:Private");
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> OwinEdit_Partial(EditModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            if (model.ExpireTimeSpan < new TimeSpan(0, 10, 0)) {
                ModelState.AddModelError(nameof(model.ExpireTimeSpan), this.__ResStr("timeSpan", "The minimum expiration timespan should be at least 10 minutes"));
                return PartialView(model);
            }
            if (model.SecurityStampValidationInterval < new TimeSpan(0, 1, 0)) {
                ModelState.AddModelError(nameof(model.SecurityStampValidationInterval), this.__ResStr("securityInterval", "The minimum validation interval should be at least 1 minute"));
                return PartialView(model);
            }

            OwinConfigHelper.SetValue<bool>(Module.AreaName, "Password:RequireDigit", model.RequireDigit);
            OwinConfigHelper.SetValue<int>(Module.AreaName, "Password:RequiredLength", model.RequiredLength);
            OwinConfigHelper.SetValue<bool>(Module.AreaName, "Password:RequireNonAlphanumeric", model.RequireNonAlphanumeric);
            OwinConfigHelper.SetValue<bool>(Module.AreaName, "Password:RequireUppercase", model.RequireUppercase);
            OwinConfigHelper.SetValue<bool>(Module.AreaName, "Password:RequireLowercase", model.RequireLowercase);

            OwinConfigHelper.SetValue<bool>(Module.AreaName, "OWin:SlidingExpiration", model.SlidingExpiration);
            OwinConfigHelper.SetValue<long>(Module.AreaName, "OWin:ExpireTimeSpan", model.ExpireTimeSpan.Ticks);
            OwinConfigHelper.SetValue<long>(Module.AreaName, "OWin:SecurityStampValidationInterval", model.SecurityStampValidationInterval.Ticks);
            OwinConfigHelper.SetValue<long>(Module.AreaName, "PasswordRenewal", model.PasswordRenewal.Ticks);

            OwinConfigHelper.SetValue<bool>(Module.AreaName, "MicrosoftAccount:Enabled", model.UseMicrosoft);
            OwinConfigHelper.SetValue<string>(Module.AreaName, "MicrosoftAccount:Public", model.MicrosoftPublic);
            OwinConfigHelper.SetValue<string>(Module.AreaName, "MicrosoftAccount:Private", model.MicrosoftPrivate);
            OwinConfigHelper.SetValue<bool>(Module.AreaName, "GoogleAccount:Enabled", model.UseGoogle);
            OwinConfigHelper.SetValue<string>(Module.AreaName, "GoogleAccount:Public", model.GooglePublic);
            OwinConfigHelper.SetValue<string>(Module.AreaName, "GoogleAccount:Private", model.GooglePrivate);
            OwinConfigHelper.SetValue<bool>(Module.AreaName, "FacebookAccount:Enabled", model.UseFacebook);
            OwinConfigHelper.SetValue<string>(Module.AreaName, "FacebookAccount:Public", model.FacebookPublic);
            OwinConfigHelper.SetValue<string>(Module.AreaName, "FacebookAccount:Private", model.FacebookPrivate);
            OwinConfigHelper.SetValue<bool>(Module.AreaName, "TwitterAccount:Enabled", model.UseTwitter);
            OwinConfigHelper.SetValue<string>(Module.AreaName, "TwitterAccount:Public", model.TwitterPublic);
            OwinConfigHelper.SetValue<string>(Module.AreaName, "TwitterAccount:Private", model.TwitterPrivate);
            await OwinConfigHelper.SaveAsync();

            await Auditing.AddAuditAsync($"{nameof(OwinEditModuleController)}.{nameof(OwinEdit_Partial)}", "Login", Guid.Empty,
                $"{nameof(OwinEdit_Partial)}", RequiresRestart: true
            );

            return FormProcessed(model, this.__ResStr("okSaved", "Login provider settings successfully saved - These settings won't take effect until the site (including all instances) is restarted."), NextPage: Manager.CurrentSite.HomePageUrl);
        }
    }
}
