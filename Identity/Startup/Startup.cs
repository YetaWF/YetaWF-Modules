/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

#if MVC6

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using YetaWF.Core.Identity;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;

namespace YetaWF.Modules.Identity
{

    public class Startup : IIdentity {

        const string AREA = "YetaWF_Identity";

        public void Setup(IServiceCollection services) {

            services.AddIdentity<UserDefinition, RoleDefinition>()
                .AddUserStore<UserStore>()
                .AddRoleStore<RoleStore>();

            services.Configure<IdentityOptions>(options => {

                // Password settings
                options.Password.RequireDigit = WebConfigHelper.GetValue<bool>(AREA, "Password:RequireDigit");
                options.Password.RequiredLength = WebConfigHelper.GetValue<int>(AREA, "Password:RequiredLength");
                options.Password.RequireNonAlphanumeric = WebConfigHelper.GetValue<bool>(AREA, "Password:RequireNonAlphanumeric");
                options.Password.RequireUppercase = WebConfigHelper.GetValue<bool>(AREA, "Password:RequireUppercase");
                options.Password.RequireLowercase = WebConfigHelper.GetValue<bool>(AREA, "Password:RequireLowercase");

                long secIntvl = WebConfigHelper.GetValue<long>(AREA, "OWin:SecurityStampValidationInterval", new TimeSpan(0, 30, 0).Ticks); // 30 minutes
                options.SecurityStampValidationInterval = new TimeSpan(secIntvl);

                // We handle lockouts
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(0);
                options.Lockout.MaxFailedAccessAttempts = 0;

                // Cookie settings
                string scheme = options.Cookies.ApplicationCookie.AuthenticationScheme = WebConfigHelper.GetValue<string>(AREA, "OWin:AuthenticationType");
                if (scheme == "ApplicationCookie") {
                    long ticks = WebConfigHelper.GetValue<long>(AREA, "OWin:ExpireTimeSpan", new TimeSpan(10, 0, 0, 0).Ticks);
                    options.Cookies.ApplicationCookie.ExpireTimeSpan = new TimeSpan(ticks);
                    options.Cookies.ApplicationCookie.SlidingExpiration = WebConfigHelper.GetValue<bool>(AREA, "OWin:SlidingExpiration", true);
                    options.Cookies.ApplicationCookie.CookieSecure = Microsoft.AspNetCore.Http.CookieSecurePolicy.None;
                    options.Cookies.ApplicationCookie.CookieName = string.Format(".YetaWF.Cookies.{0}", YetaWFManager.DefaultSiteName);
                    options.Cookies.ApplicationCookie.AutomaticAuthenticate = true;
                }
                options.Cookies.ExternalCookie.CookieName = string.Format(".YetaWF.Cookies.Ext.{0}", YetaWFManager.DefaultSiteName);

                // User settings
                // the default is acceptable: options.User.AllowedUserNameCharacters
                // We handle email/name ourselves
                options.User.RequireUniqueEmail = false;
            });
        }

        public void SetupLoginProviders(IApplicationBuilder app) {

            string pub = WebConfigHelper.GetValue<string>(AREA, "FacebookAccount:Public");
            string priv = WebConfigHelper.GetValue<string>(AREA, "FacebookAccount:Private");
            if (!string.IsNullOrWhiteSpace(pub) && !string.IsNullOrWhiteSpace(priv)) {
                app.UseFacebookAuthentication(new FacebookOptions() {
                    AppId = pub,
                    AppSecret = priv,
                });
            }
            pub = WebConfigHelper.GetValue<string>(AREA, "GoogleAccount:Public");
            priv = WebConfigHelper.GetValue<string>(AREA, "GoogleAccount:Private");
            if (!string.IsNullOrWhiteSpace(pub) && !string.IsNullOrWhiteSpace(priv)) {
                app.UseGoogleAuthentication(new GoogleOptions() {
                    ClientId = pub,
                    ClientSecret = priv,
                });
            }
            pub = WebConfigHelper.GetValue<string>(AREA, "TwitterAccount:Public");
            priv = WebConfigHelper.GetValue<string>(AREA, "TwitterAccount:Private");
            if (!string.IsNullOrWhiteSpace(pub) && !string.IsNullOrWhiteSpace(priv)) {
                app.UseTwitterAuthentication(new TwitterOptions() {
                    ConsumerKey = pub,
                    ConsumerSecret = priv
                });
            }
            if (!string.IsNullOrWhiteSpace(pub) && !string.IsNullOrWhiteSpace(priv)) {
                pub = WebConfigHelper.GetValue<string>(AREA, "MicrosoftAccount:Public");
                priv = WebConfigHelper.GetValue<string>(AREA, "MicrosoftAccount:Private");
                app.UseMicrosoftAccountAuthentication(new MicrosoftAccountOptions() {
                    ClientId = pub,
                    ClientSecret = priv
                });
            }
        }
    }
}
#else
#endif