/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Blog#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Blog.Components;
using YetaWF.Modules.Blog.DataProvider;
using YetaWF.Modules.Blog.Support;

namespace YetaWF.Modules.Blog.Modules;

public class DisqusModuleDataProvider : ModuleDefinitionDataProvider<Guid, DisqusModule>, IInstallableModel { }

[ModuleGuid("{3ba64dfb-9292-4f9b-937e-0c8fe110bf45}")]
[UniqueModule(UniqueModuleStyle.NonUnique)]
[ModuleCategory("Comments")]
public class DisqusModule : ModuleDefinition2 {

    public DisqusModule() {
        Title = this.__ResStr("modTitle", "Comments");
        Name = this.__ResStr("modName", "Disqus Comments");
        Description = this.__ResStr("modSummary", "Maintains all comments for a blog entry on the current page using the Disqus service. This module is typically used on the same page as the Entry Display Module.");
    }

    public override IModuleDefinitionIO GetDataProvider() { return new DisqusModuleDataProvider(); }

    public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }

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

    public async Task<ActionInfo> RenderModuleAsync() {
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
                string? logoffUrl = WebConfigHelper.GetValue<string>("MvcApplication", "LogoffUrl", null, Package: false);
                if (string.IsNullOrWhiteSpace(logoffUrl))
                    throw new InternalError("MvcApplication LogoffUrl not defined in web.cofig/appsettings.json - This is required to log off the current user");
                model.LogoffUrl = Manager.CurrentSite.MakeUrl(logoffUrl + Manager.CurrentPage.EvaluatedCanonicalUrl);
            }
            return await RenderAsync(model);
        }
    }
}
