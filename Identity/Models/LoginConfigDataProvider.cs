/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Modules;
using YetaWF.Core.Identity;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Core.Audit;
using System;
using YetaWF.Core.Localize;
using System.Text;
#if MVC6
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
#else
using Microsoft.Owin.Security;
using System.Web;
#endif

namespace YetaWF.Modules.Identity.DataProvider {

    public class LoginConfigData {

        [Data_PrimaryKey]
        public int Id { get; set; }

        public bool AllowUserRegistration { get; set; }
        public RegistrationTypeEnum RegistrationType { get; set; }
        public bool SavePlainTextPassword { get; set; }
        [Data_NewValue]
        public TimeSpan ResetTimeSpan { get; set; }
        public bool Captcha { get; set; }
        public bool CaptchaForgotPassword { get; set; }
        public bool VerifyNewUsers { get; set; }
        public bool ApproveNewUsers { get; set; }
        public bool NotifyAdminNewUsers { get; set; }
        public bool BccVerification { get; set; }
        [Data_NewValue]
        public int MaxLoginFailures { get; set; }
        [Data_NewValue]
        public bool BccForgottenPassword { get; set; }
        public bool PersistentLogin { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string LoginUrl { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string RegisterUrl { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string TwoStepAuthUrl { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string ForgotPasswordUrl { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string VerificationPendingUrl { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string ApprovalPendingUrl { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string RejectedUrl { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string SuspendedUrl { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string LoggedOffUrl { get; set; }
        [Data_Binary]
        public SerializableList<Role> TwoStepAuth { get; set; }

        [Data_DontSave]
        public bool UseFacebook {
            get { return OwinConfigHelper.GetValue<bool>(AreaRegistration.CurrentPackage.AreaName, "FacebookAccount:Enabled") && DefinedFacebook; }
#if MVC6
            [Obsolete] // needed load existing saved settings
#endif
            set { }
        }
        [Data_DontSave]
        public bool UseGoogle {
            get { return OwinConfigHelper.GetValue<bool>(AreaRegistration.CurrentPackage.AreaName, "GoogleAccount:Enabled") && DefinedGoogle; }
#if MVC6
            [Obsolete]
#endif
            set { }
        }
        [Data_DontSave]
        public bool UseMicrosoft {
            get { return OwinConfigHelper.GetValue<bool>(AreaRegistration.CurrentPackage.AreaName, "MicrosoftAccount:Enabled") && DefinedMicrosoft; }
#if MVC6
            [Obsolete]
#endif
            set { }
        }
        [Data_DontSave]
        public bool UseTwitter {
            get { return OwinConfigHelper.GetValue<bool>(AreaRegistration.CurrentPackage.AreaName, "TwitterAccount:Enabled") && DefinedTwitter; }
#if MVC6
            [Obsolete]
#endif
            set { }
        }

        public bool DefinedFacebook {
            get {
                return !string.IsNullOrWhiteSpace(OwinConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "FacebookAccount:Public")) &&
                        !string.IsNullOrWhiteSpace(OwinConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "FacebookAccount:Private"));
            }
        }
        public bool DefinedGoogle {
            get {
                return !string.IsNullOrWhiteSpace(OwinConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "GoogleAccount:Public")) &&
                        !string.IsNullOrWhiteSpace(OwinConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "GoogleAccount:Private"));
            }
        }
        public bool DefinedMicrosoft {
            get {
                return !string.IsNullOrWhiteSpace(OwinConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "MicrosoftAccount:Public")) &&
                        !string.IsNullOrWhiteSpace(OwinConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "MicrosoftAccount:Private"));
            }
        }
        public bool DefinedTwitter {
            get {
                return !string.IsNullOrWhiteSpace(OwinConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "TwitterAccount:Public")) &&
                        !string.IsNullOrWhiteSpace(OwinConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "TwitterAccount:Private"));
            }
        }

        public LoginConfigData() {
            ResetTimeSpan = new TimeSpan(1, 0, 0, 0);
        }
    }

    public class LoginConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public LoginConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public LoginConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, LoginConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, LoginConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.Identity.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName, SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public string PasswordRules {
            get {
                string areaName = AreaRegistration.CurrentPackage.AreaName;

                bool requireDigit = OwinConfigHelper.GetValue<bool>(areaName, "Password:RequireDigit", false);
                int requiredLength = OwinConfigHelper.GetValue<int>(areaName, "Password:RequiredLength", 6);
                bool requireNonAlphanumeric = OwinConfigHelper.GetValue<bool>(areaName, "Password:RequireNonAlphanumeric", false);
                bool requireUppercase = OwinConfigHelper.GetValue<bool>(areaName, "Password:RequireUppercase", false);
                bool requireLowercase = OwinConfigHelper.GetValue<bool>(areaName, "Password:RequireLowercase", false);

                StringBuilder sb = new StringBuilder();
                string delim = this.__ResStr("passwordDelim", ", ");
                if (requiredLength > 0) {
                    if (sb.Length > 0) sb.Append(delim);
                    sb.Append(this.__ResStr("passwordLen", "be at least {0} characters long", requiredLength));
                }
                if (requireDigit) {
                    if (sb.Length > 0) sb.Append(delim);
                    sb.Append(this.__ResStr("passwordNum", "contain at least one number 0-9"));
                }
                if (requireNonAlphanumeric) {
                    if (sb.Length > 0) sb.Append(delim);
                    sb.Append(this.__ResStr("passwordNonAlpha", "contain at least one special character (non alphanumeric)"));
                }
                if (requireUppercase) {
                    if (sb.Length > 0) sb.Append(delim);
                    sb.Append(this.__ResStr("passwordUpper", "contain at least one uppercase character A-Z"));
                }
                if (requireLowercase) {
                    if (sb.Length > 0) sb.Append(delim);
                    sb.Append(this.__ResStr("passwordLower", "contain at least one lowercase character a-z"));
                }

                sb.Insert(0, this.__ResStr("passwordStart", "The password must "));
                sb.Append(this.__ResStr("passwordEnd", "."));

                return sb.ToString();
            }
        }

        public static async Task<LoginConfigData> GetConfigAsync() {
            using (LoginConfigDataProvider configDP = new LoginConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<LoginConfigData> GetItemAsync() {
            LoginConfigData config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                config = new LoginConfigData() {
                    Id = KEY,
                    AllowUserRegistration = true,
                    RegistrationType = RegistrationTypeEnum.EmailOnly,
                    SavePlainTextPassword = false,
                    Captcha = false,
                    VerifyNewUsers = false,
                    ApproveNewUsers = false,
                    NotifyAdminNewUsers = false,
                    BccVerification = false,
                    BccForgottenPassword = false,
                    PersistentLogin = true,
                };
                await AddConfigAsync(config);
            }
            return config;
        }
        private async Task AddConfigAsync(LoginConfigData data) {
            data.Id = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding settings");
            await Auditing.AddAuditAsync($"{nameof(LoginConfigDataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
                "Add Login Config",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
        public async Task UpdateConfigAsync(LoginConfigData data) {
            LoginConfigData origConfig = Auditing.Active ? await GetItemAsync() : null;
            data.Id = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
            await Auditing.AddAuditAsync($"{nameof(LoginConfigDataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
                "Update Login Config",
                DataBefore: origConfig,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }

        public class LoginProviderDescription {
            public string InternalName { get; set; }
            public string DisplayName { get; set; }
        }

        public async Task<List<LoginProviderDescription>> GetActiveExternalLoginProvidersAsync() {
            LoginConfigData configData = await GetConfigAsync();
            List<LoginProviderDescription> list = new List<LoginProviderDescription>();
#if MVC6
            SignInManager<UserDefinition> _signinManager = (SignInManager<UserDefinition>)YetaWFManager.ServiceProvider.GetService(typeof(SignInManager<UserDefinition>));

            List<AuthenticationScheme> loginProviders = (await _signinManager.GetExternalAuthenticationSchemesAsync()).ToList();
            foreach (AuthenticationScheme provider in loginProviders) {
                string name = provider.Name;
                if (name == "Facebook" && configData.UseFacebook && configData.DefinedFacebook)
                    list.Add(new LoginProviderDescription { InternalName = name, DisplayName = provider.DisplayName });
                else if (name == "Google" && configData.UseGoogle && configData.DefinedGoogle)
                    list.Add(new LoginProviderDescription { InternalName = name, DisplayName = provider.DisplayName });
                else if (name == "Microsoft" && configData.UseMicrosoft && configData.DefinedMicrosoft)
                    list.Add(new LoginProviderDescription { InternalName = name, DisplayName = provider.DisplayName });
                else if (name == "Twitter" && configData.UseTwitter && configData.DefinedTwitter)
                    list.Add(new LoginProviderDescription { InternalName = name, DisplayName = provider.DisplayName });
            }
#else
            List<AuthenticationDescription> loginProviders = Manager.CurrentContext.GetOwinContext().Authentication.GetExternalAuthenticationTypes().ToList();
            foreach (AuthenticationDescription provider in loginProviders) {
                string name = provider.AuthenticationType;
                if (name == "Facebook" && configData.UseFacebook && configData.DefinedFacebook)
                    list.Add(new LoginProviderDescription { InternalName = name, DisplayName = provider.Caption });
                else if (name == "Google" && configData.UseGoogle && configData.DefinedGoogle)
                    list.Add(new LoginProviderDescription { InternalName = name, DisplayName = provider.Caption });
                else if (name == "Microsoft" && configData.UseMicrosoft && configData.DefinedMicrosoft)
                    list.Add(new LoginProviderDescription { InternalName = name, DisplayName = provider.Caption });
                else if (name == "Twitter" && configData.UseTwitter && configData.DefinedTwitter)
                    list.Add(new LoginProviderDescription { InternalName = name, DisplayName = provider.Caption });
            }
#endif
            return list;
        }
    }
}
