/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using YetaWF.Core.Identity;
using YetaWF.Core.Support;
using YetaWF.Core.Support.Services;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;


namespace YetaWF.Modules.Identity {

    public class Startup : IIdentity {

        const string AREA = "YetaWF_Identity";

        public void Setup(IServiceCollection services) {

            string login = WebConfigHelper.GetValue<string>(AREA, "LoginProviderSettings");
            if (!string.IsNullOrWhiteSpace(login))
                OwinConfigHelper.InitAsync(Path.Combine(YetaWFManager.RootFolderWebProject, login)).Wait();// wait ok, startup only, load login provider settings

            services.AddIdentity<UserDefinition, RoleDefinition>()
                .AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>();

            services.Configure<IdentityOptions>(options => {

                // Password settings
                options.Password.RequireDigit = OwinConfigHelper.GetValue<bool>(AREA, "Password:RequireDigit", false);
                options.Password.RequiredLength = OwinConfigHelper.GetValue<int>(AREA, "Password:RequiredLength", 6);
                options.Password.RequireNonAlphanumeric = OwinConfigHelper.GetValue<bool>(AREA, "Password:RequireNonAlphanumeric", false);
                options.Password.RequireUppercase = OwinConfigHelper.GetValue<bool>(AREA, "Password:RequireUppercase", false);
                options.Password.RequireLowercase = OwinConfigHelper.GetValue<bool>(AREA, "Password:RequireLowercase", false);

                // long secIntvl = OwinConfigHelper.GetValue<long>(AREA, "OWin:SecurityStampValidationInterval", new TimeSpan(0, 30, 0).Ticks); // 30 minutes

                // We handle lockouts
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(0);
                options.Lockout.MaxFailedAccessAttempts = 0;

                // User settings
                // the default is acceptable: options.User.AllowedUserNameCharacters
                // We handle email/name ourselves
                options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(c => {
                long ticks = OwinConfigHelper.GetValue<long>(AREA, "OWin:ExpireTimeSpan", new TimeSpan(10, 0, 0, 0).Ticks);
                c.Cookie.Name = string.Format(".YetaWF.Cookies.{0}", YetaWFManager.DefaultSiteName);
                c.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
                c.Cookie.SameSite = OwinConfigHelper.GetValue<Microsoft.AspNetCore.Http.SameSiteMode>(AREA, "OWin:SameSiteMode", Microsoft.AspNetCore.Http.SameSiteMode.Strict);
                c.ExpireTimeSpan = new TimeSpan(ticks);
                c.SlidingExpiration = OwinConfigHelper.GetValue<bool>(AREA, "OWin:SlidingExpiration", true);
            });
            services.ConfigureExternalCookie(c => {
                long ticks = OwinConfigHelper.GetValue<long>(AREA, "OWin:ExpireTimeSpan", new TimeSpan(10, 0, 0, 0).Ticks);
                c.Cookie.Name = string.Format(".YetaWF.Cookies.Ext.{0}", YetaWFManager.DefaultSiteName);
                c.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
                c.Cookie.SameSite = OwinConfigHelper.GetValue<Microsoft.AspNetCore.Http.SameSiteMode>(AREA, "OWin:SameSiteMode", Microsoft.AspNetCore.Http.SameSiteMode.Strict);
                c.ExpireTimeSpan = new TimeSpan(ticks);
                c.SlidingExpiration = OwinConfigHelper.GetValue<bool>(AREA, "OWin:SlidingExpiration", true);
            });
        }

        public void SetupLoginProviders(IServiceCollection services) {
            // https://github.com/aspnet/Security/issues/1310

            AuthenticationBuilder authBuilder = services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);
            {
                string pub = OwinConfigHelper.GetValue<string>(AREA, "FacebookAccount:Public");
                string priv = OwinConfigHelper.GetValue<string>(AREA, "FacebookAccount:Private");
                if (!string.IsNullOrWhiteSpace(pub) && !string.IsNullOrWhiteSpace(priv)) {
                    authBuilder.AddFacebook(o => {
                        o.AppId = pub;
                        o.AppSecret = priv;
                    });
                }
            }
            {
                string pub = OwinConfigHelper.GetValue<string>(AREA, "GoogleAccount:Public");
                string priv = OwinConfigHelper.GetValue<string>(AREA, "GoogleAccount:Private");
                if (!string.IsNullOrWhiteSpace(pub) && !string.IsNullOrWhiteSpace(priv)) {
                    authBuilder.AddGoogle(o => {
                        o.ClientId = pub;
                        o.ClientSecret = priv;
                    });
                }
            }
            {
                string pub = OwinConfigHelper.GetValue<string>(AREA, "TwitterAccount:Public");
                string priv = OwinConfigHelper.GetValue<string>(AREA, "TwitterAccount:Private");
                if (!string.IsNullOrWhiteSpace(pub) && !string.IsNullOrWhiteSpace(priv)) {
                    authBuilder.AddTwitter(o => {
                        o.ConsumerKey = pub;
                        o.ConsumerSecret = priv;
                    });
                }
            }
            {
                string pub = OwinConfigHelper.GetValue<string>(AREA, "MicrosoftAccount:Public");
                string priv = OwinConfigHelper.GetValue<string>(AREA, "MicrosoftAccount:Private");
                if (!string.IsNullOrWhiteSpace(pub) && !string.IsNullOrWhiteSpace(priv)) {
                    authBuilder.AddMicrosoftAccount(o => {
                        o.ClientId = pub;
                        o.ClientSecret = priv;
                    });
                }
            }

            authBuilder.AddDynamicAuthentication();
        }
    }
}
