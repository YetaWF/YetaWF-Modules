/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Localize;
using YetaWF.Core.Support;
using YetaWF.Modules.Blog.Components;
using YetaWF.Modules.Blog.Controllers.Support;
using YetaWF.Modules.Blog.DataProvider;

// Documentation:
// https://disqus.com/api/docs/
// https://help.disqus.com/customer/en/portal/articles/236206-integrating-single-sign-on

namespace YetaWF.Modules.Blog.Controllers {

    public class DisqusModuleController : ControllerImpl<YetaWF.Modules.Blog.Modules.DisqusModule> {

        public DisqusModuleController() { }

        public class DisplayModel {
            public string ShortName { get; set; } = null!;
            public bool UseSSO { get; set; }
            public string AuthPayload { get; set; } = null!;
            public string PublicKey { get; set; } = null!;
            public string LoginUrl { get; set; } = null!;
            public string LogoffUrl { get; set; } = null!;
            public int Width { get; set; }
            public int Height { get; set; }
        }

        [AllowGet]
        public async Task<ActionResult> Disqus() {
            using (DisqusConfigDataProvider dataProvider = new DisqusConfigDataProvider()) {
                DisqusConfigData config = await dataProvider.GetItemAsync();
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
                            avatarUrl = "https:" + GravatarComponentBase.GravatarUrl(Manager.UserEmail!, config.GravatarSize, config.GravatarRating, config.GravatarDefault);
                        SSO sso = new Support.SSO(config.PrivateKey);
                        model.AuthPayload = sso.GetPayload(Manager.UserId.ToString(), Manager.UserName!, Manager.UserEmail!, avatarUrl);
                    } else {
                        model.LoginUrl = Manager.CurrentSite.MakeUrl(config.LoginUrl);
                        model.Width = config.Width;
                        model.Height = config.Height;
                    }
                    string? logoffUrl = WebConfigHelper.GetValue<string>("MvcApplication", "LogoffUrl", null, Package:false);
                    if (string.IsNullOrWhiteSpace(logoffUrl))
                        throw new InternalError("MvcApplication LogoffUrl not defined in web.cofig/appsettings.json - This is required to log off the current user");
                    model.LogoffUrl = Manager.CurrentSite.MakeUrl(logoffUrl + Manager.CurrentPage.EvaluatedCanonicalUrl);
                }
                return View(model);
            }
        }
    }
}
