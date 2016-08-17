/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Identity.Controllers {

    public class OwinEditModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.OwinEditModule> {

        public OwinEditModuleController() { }

        [Trim]
        public class EditModel {

            public const int MaxKey = 200;

            public EditModel() {
                MicrosoftUrl = "https://account.live.com/developers/applications/create";
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

            [Caption("Microsoft"), Description("Provides a link to Microsoft to set up authentication for your sites")]
            [UIHint("Url"), ReadOnly]
            public string MicrosoftUrl { get; set; }
            [Caption("Microsoft Public Key"), Description("The public key for authentication using Microsoft")]
            [UIHint("Text80"), StringLength(MaxKey), Trim]
            public string MicrosoftPublic { get; set; }
            [Caption("Microsoft Private Key"), Description("The private key for authentication using Microsoft")]
            [UIHint("Text80"), RequiredIfSupplied("MicrosoftPublic"), StringLength(MaxKey), Trim]
            public string MicrosoftPrivate { get; set; }

            [Caption("Google"), Description("Provides a link to Google to set up authentication for your sites")]
            [UIHint("Url"), ReadOnly]
            public string GoogleUrl { get; set; }
            [Caption("Google Public Key"), Description("The public key for authentication using Google")]
            [UIHint("Text80"), StringLength(MaxKey), Trim]
            public string GooglePublic { get; set; }
            [Caption("Google Private Key"), Description("The private key for authentication using Google")]
            [UIHint("Text80"), RequiredIfSupplied("GooglePublic"), StringLength(MaxKey), Trim]
            public string GooglePrivate { get; set; }

            [Caption("Facebook"), Description("Provides a link to Facebook to set up authentication for your sites")]
            [UIHint("Url"), ReadOnly]
            public string FacebookUrl { get; set; }
            [Caption("Facebook Public Key"), Description("The public key for authentication using Facebook")]
            [UIHint("Text80"), StringLength(MaxKey), Trim]
            public string FacebookPublic { get; set; }
            [Caption("Facebook Private Key"), Description("The private key for authentication using Facebook")]
            [UIHint("Text80"), RequiredIfSupplied("FacebookPublic"), StringLength(MaxKey), Trim]
            public string FacebookPrivate { get; set; }

            [Caption("Twitter"), Description("Provides a link to Twitter to set up authentication for your sites")]
            [UIHint("Url"), ReadOnly]
            public string TwitterUrl { get; set; }
            [Caption("Twitter Public Key"), Description("The public key for authentication using Twitter")]
            [UIHint("Text80"), StringLength(MaxKey), Trim]
            public string TwitterPublic { get; set; }
            [Caption("Twitter Private Key"), Description("The private key for authentication using Twitter")]
            [UIHint("Text80"), RequiredIfSupplied("TwitterPublic"), StringLength(MaxKey), Trim]
            public string TwitterPrivate { get; set; }
        }

        [HttpGet]
        public ActionResult OwinEdit() {
            EditModel model = new EditModel { };

            model.SlidingExpiration = WebConfigHelper.GetValue<bool>(Module.AreaName, "OWin:SlidingExpiration");
            model.ExpireTimeSpan = WebConfigHelper.GetValue<TimeSpan>(Module.AreaName, "OWin:ExpireTimeSpan");

            model.MicrosoftPublic = WebConfigHelper.GetValue<string>(Module.AreaName, "MicrosoftAccount:Public");
            model.MicrosoftPrivate = WebConfigHelper.GetValue<string>(Module.AreaName, "MicrosoftAccount:Private");
            model.GooglePublic = WebConfigHelper.GetValue<string>(Module.AreaName, "GoogleAccount:Public");
            model.GooglePrivate = WebConfigHelper.GetValue<string>(Module.AreaName, "GoogleAccount:Private");
            model.FacebookPublic = WebConfigHelper.GetValue<string>(Module.AreaName, "FacebookAccount:Public");
            model.FacebookPrivate = WebConfigHelper.GetValue<string>(Module.AreaName, "FacebookAccount:Private");
            model.TwitterPublic = WebConfigHelper.GetValue<string>(Module.AreaName, "TwitterAccount:Public");
            model.TwitterPrivate = WebConfigHelper.GetValue<string>(Module.AreaName, "TwitterAccount:Private");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult OwinEdit_Partial(EditModel model) {
            if (!ModelState.IsValid)
                return PartialView(model);
            if (model.ExpireTimeSpan < new TimeSpan(0, 10, 0)) {
                ModelState.AddModelError("ExpireTimeSpan", this.__ResStr("timeSpan", "The minimum expiration timespan should be at least 10 minutes"));
                return PartialView(model);
            }

            WebConfigHelper.SetValue<bool>(Module.AreaName, "OWin:SlidingExpiration", model.SlidingExpiration);
            WebConfigHelper.SetValue<TimeSpan>(Module.AreaName, "OWin:ExpireTimeSpan", model.ExpireTimeSpan);

            WebConfigHelper.SetValue<string>(Module.AreaName, "MicrosoftAccount:Public", model.MicrosoftPublic);
            WebConfigHelper.SetValue<string>(Module.AreaName, "MicrosoftAccount:Private", model.MicrosoftPrivate);
            WebConfigHelper.SetValue<string>(Module.AreaName, "GoogleAccount:Public", model.GooglePublic);
            WebConfigHelper.SetValue<string>(Module.AreaName, "GoogleAccount:Private", model.GooglePrivate);
            WebConfigHelper.SetValue<string>(Module.AreaName, "FacebookAccount:Public", model.FacebookPublic);
            WebConfigHelper.SetValue<string>(Module.AreaName, "FacebookAccount:Private", model.FacebookPrivate);
            WebConfigHelper.SetValue<string>(Module.AreaName, "TwitterAccount:Public", model.TwitterPublic);
            WebConfigHelper.SetValue<string>(Module.AreaName, "TwitterAccount:Private", model.TwitterPrivate);

            return FormProcessed(model, this.__ResStr("okSaved", "Web.config has been updated - Web application is now restarting."), NextPage: Manager.CurrentSite.HomePageUrl);
        }
    }
}