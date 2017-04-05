/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class OwinEditModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.OwinEditModule> {

        public OwinEditModuleController() { }

        [Trim]
        [Header("Login provider settings apply to all sites within this YetaWF instance.")]
        public class EditModel {

            public const int MaxKey = 200;

            public EditModel() {
                MicrosoftUrl = "https://account.live.com/developers/applications/";
                GoogleUrl = "https://console.developers.google.com/";
                FacebookUrl = "https://developers.facebook.com/apps";
                TwitterUrl = "https://dev.twitter.com/apps";
            }

            [Caption("Sliding Expiration"), Description("Cookies are reissued with a new expiration time anytime a request is processed whose cookie is more than halfway through the expiration window")]
            [UIHint("Boolean")]
            public bool SlidingExpiration { get; set; }

            [Caption("Expiration"), Description("The time a cookie will remain valid from the point it is created")]
            [UIHint("TimeSpan"), Required]
            public TimeSpan ExpireTimeSpan { get; set; }

            [Caption("Security Interval"), Description("The time after which a logged on user is re-validated (in case the password changed) to insure the authenticated user is still valid")]
            [UIHint("TimeSpan"), Required]
            public TimeSpan SecurityStampValidationInterval { get; set; }

            [Caption("Facebook"), Description("Provides a link to Facebook to set up authentication for your sites")]
            [UIHint("Url"), ReadOnly]
            public string FacebookUrl { get; set; }
            [Caption("Facebook App ID"), Description("The App ID for authentication using Facebook (defined in Facebook)")]
            [UIHint("Text80"), StringLength(MaxKey), Trim]
            [ExcludeDemoMode]
            public string FacebookPublic { get; set; }
            [Caption("Facebook App Secret"), Description("The App Secret for authentication using Facebook (defined in Facebook)")]
            [UIHint("Text80"), RequiredIfSupplied("FacebookPublic"), StringLength(MaxKey), Trim]
            [ExcludeDemoMode]
            public string FacebookPrivate { get; set; }

            [Caption("Google"), Description("Provides a link to Google to set up authentication for your sites")]
            [UIHint("Url"), ReadOnly]
            public string GoogleUrl { get; set; }
            [Caption("Google Client ID"), Description("The Client ID for authentication using Google (defined using Google)")]
            [UIHint("Text80"), StringLength(MaxKey), Trim]
            [ExcludeDemoMode]
            public string GooglePublic { get; set; }
            [Caption("Google Client Secret"), Description("The Client Secret for authentication using Google (defined using Google)")]
            [UIHint("Text80"), RequiredIfSupplied("GooglePublic"), StringLength(MaxKey), Trim]
            [ExcludeDemoMode]
            public string GooglePrivate { get; set; }

            [Caption("Microsoft"), Description("Provides a link to Microsoft to set up authentication for your sites")]
            [UIHint("Url"), ReadOnly]
            public string MicrosoftUrl { get; set; }
            [Caption("Microsoft Public Key"), Description("The public key for authentication using Microsoft")]
            [UIHint("Text80"), StringLength(MaxKey), Trim]
            [ExcludeDemoMode]
            public string MicrosoftPublic { get; set; }
            [Caption("Microsoft Private Key"), Description("The private key for authentication using Microsoft")]
            [UIHint("Text80"), RequiredIfSupplied("MicrosoftPublic"), StringLength(MaxKey), Trim]
            [ExcludeDemoMode]
            public string MicrosoftPrivate { get; set; }

            [Caption("Twitter"), Description("Provides a link to Twitter to set up authentication for your sites")]
            [UIHint("Url"), ReadOnly]
            public string TwitterUrl { get; set; }
            [Caption("Twitter Consumer Key"), Description("The Consumer Key (API Key) for authentication using Twitter (defined using Twitter)")]
            [UIHint("Text80"), StringLength(MaxKey), Trim]
            [ExcludeDemoMode]
            public string TwitterPublic { get; set; }
            [Caption("Twitter Consumer Secret"), Description("The Consumer Secret (API Secret) for authentication using Twitter (defined using Twitter)")]
            [UIHint("Text80"), RequiredIfSupplied("TwitterPublic"), StringLength(MaxKey), Trim]
            [ExcludeDemoMode]
            public string TwitterPrivate { get; set; }
        }

        [HttpGet]
        public ActionResult OwinEdit() {
            EditModel model = new EditModel { };

            model.SlidingExpiration = WebConfigHelper.GetValue<bool>(Module.Area, "OWin:SlidingExpiration");
            long ticks = WebConfigHelper.GetValue<long>(Module.Area, "OWin:ExpireTimeSpan", new TimeSpan(10, 0, 0, 0).Ticks); // 10 days
            model.ExpireTimeSpan = new TimeSpan(ticks);
            ticks = WebConfigHelper.GetValue<long>(Module.Area, "OWin:SecurityStampValidationInterval", new TimeSpan(0, 30, 0).Ticks); // 30 minutes
            model.SecurityStampValidationInterval = new TimeSpan(ticks);

            model.MicrosoftPublic = WebConfigHelper.GetValue<string>(Module.Area, "MicrosoftAccount:Public");
            model.MicrosoftPrivate = WebConfigHelper.GetValue<string>(Module.Area, "MicrosoftAccount:Private");
            model.GooglePublic = WebConfigHelper.GetValue<string>(Module.Area, "GoogleAccount:Public");
            model.GooglePrivate = WebConfigHelper.GetValue<string>(Module.Area, "GoogleAccount:Private");
            model.FacebookPublic = WebConfigHelper.GetValue<string>(Module.Area, "FacebookAccount:Public");
            model.FacebookPrivate = WebConfigHelper.GetValue<string>(Module.Area, "FacebookAccount:Private");
            model.TwitterPublic = WebConfigHelper.GetValue<string>(Module.Area, "TwitterAccount:Public");
            model.TwitterPrivate = WebConfigHelper.GetValue<string>(Module.Area, "TwitterAccount:Private");
            return View(model);
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult OwinEdit_Partial(EditModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            if (model.ExpireTimeSpan < new TimeSpan(0, 10, 0)) {
                ModelState.AddModelError("ExpireTimeSpan", this.__ResStr("timeSpan", "The minimum expiration timespan should be at least 10 minutes"));
                return PartialView(model);
            }
            if (model.SecurityStampValidationInterval < new TimeSpan(0, 1, 0)) {
                ModelState.AddModelError("SecurityStampValidationInterval", this.__ResStr("securityInterval", "The minimum validation interval should be at least 1 minute"));
                return PartialView(model);
            }

            WebConfigHelper.SetValue<bool>(Module.Area, "OWin:SlidingExpiration", model.SlidingExpiration);
            WebConfigHelper.SetValue<long>(Module.Area, "OWin:ExpireTimeSpan", model.ExpireTimeSpan.Ticks);
            WebConfigHelper.SetValue<long>(Module.Area, "OWin:SecurityStampValidationInterval", model.SecurityStampValidationInterval.Ticks);

            WebConfigHelper.SetValue<string>(Module.Area, "MicrosoftAccount:Public", model.MicrosoftPublic);
            WebConfigHelper.SetValue<string>(Module.Area, "MicrosoftAccount:Private", model.MicrosoftPrivate);
            WebConfigHelper.SetValue<string>(Module.Area, "GoogleAccount:Public", model.GooglePublic);
            WebConfigHelper.SetValue<string>(Module.Area, "GoogleAccount:Private", model.GooglePrivate);
            WebConfigHelper.SetValue<string>(Module.Area, "FacebookAccount:Public", model.FacebookPublic);
            WebConfigHelper.SetValue<string>(Module.Area, "FacebookAccount:Private", model.FacebookPrivate);
            WebConfigHelper.SetValue<string>(Module.Area, "TwitterAccount:Public", model.TwitterPublic);
            WebConfigHelper.SetValue<string>(Module.Area, "TwitterAccount:Private", model.TwitterPrivate);
            WebConfigHelper.Save();

            Manager.RestartSite();

            return FormProcessed(model, this.__ResStr("okSaved", "Web.config/appsettings.json has been updated - Web application is now restarting."), NextPage: Manager.CurrentSite.HomePageUrl);
        }
    }
}