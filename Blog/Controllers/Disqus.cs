/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Blog#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Controllers.Support;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Views.Shared;

// Documentation:
// https://disqus.com/api/docs/
// https://help.disqus.com/customer/en/portal/articles/236206-integrating-single-sign-on

namespace YetaWF.Modules.Blog.Controllers {

    public class DisqusModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.DisqusModule> {

        public DisqusModuleController() { }

        public class DisplayModel {
            public string ShortName { get; set; }
            public bool UseSSO { get; set; }
            public string AuthPayload { get; set; }
            public string PublicKey { get; set; }
            public string LoginUrl { get; set; }
            public string LogoffUrl { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        [HttpGet]
        public ActionResult Disqus() {
            using (DisqusConfigDataProvider dataProvider = new DisqusConfigDataProvider()) {
                DisqusConfigData config = dataProvider.GetItem();
                if (config == null)
                    throw new Error(this.__ResStr("notFound", "The Disqus settings could not be found"));
                if (string.IsNullOrWhiteSpace(config.ShortName))
                    throw new Error(this.__ResStr("notShortName", "The Disqus settings must be updated to define the site's Shortname"));
                DisplayModel model = new DisplayModel {
                    ShortName = config.ShortName,
                };
                if (config.UseSSO &&
                        !string.IsNullOrWhiteSpace(config.PrivateKey) && !string.IsNullOrWhiteSpace(config.PublicKey) &&
                        !string.IsNullOrWhiteSpace(config.LoginUrl)) {
                    model.UseSSO = true;
                    if (Manager.HaveUser) {
                        model.PublicKey = config.PublicKey;
                        string avatarUrl = "";
                        if (config.AvatarType == DisqusConfigData.AvatarTypeEnum.Gravatar)
                            avatarUrl = "https:" + GravatarHelper.GravatarUrl(Manager.UserEmail, config.GravatarSize, config.GravatarRating, config.GravatarDefault);
                        SSO sso = new Support.SSO(config.PrivateKey);
                        model.AuthPayload = sso.GetPayload(Manager.UserId.ToString(), Manager.UserName, Manager.UserEmail, avatarUrl);
                    } else {
                        model.LoginUrl = Manager.CurrentSite.MakeUrl(config.LoginUrl);
                        model.Width = config.Width;
                        model.Height = config.Height;
                    }
                    string logoffUrl = WebConfigHelper.GetValue<string>("MvcApplication", "LogoffUrl", null);
                    if (string.IsNullOrWhiteSpace(logoffUrl))
                        throw new InternalError("MvcApplication LogoffUrl not defined in web.cofig - This is required to log off the current user");
                    model.LogoffUrl = Manager.CurrentSite.MakeUrl(logoffUrl + Manager.CurrentPage.EvaluatedCanonicalUrl);
                }
                return View(model);
            }
        }
    }
}
