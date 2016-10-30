/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.Owin.Security;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.DataProvider {

    public class LoginConfigData {

        [Data_PrimaryKey]
        public int Id { get; set; }

        public bool AllowUserRegistration { get; set; }
        public RegistrationTypeEnum RegistrationType { get; set; }
        public bool SavePlainTextPassword { get; set; }
        public bool Captcha { get; set; }
        public bool CaptchaForgotPassword { get; set; }
        public bool VerifyNewUsers { get; set; }
        public bool ApproveNewUsers { get; set; }
        public bool NotifyAdminNewUsers { get; set; }
        public bool BccVerification { get; set; }
        [Data_NewValue("(0)")]
        public int MaxLoginFailures { get; set; }
        [Data_NewValue("(0)")]
        public bool BccForgottenPassword { get; set; }
        public bool PersistentLogin { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string RegisterUrl { get; set; }
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

        [Data_NewValue("(0)")]
        public bool UseFacebook { get; set; }
        [Data_NewValue("(0)")]
        public bool UseGoogle { get; set; }
        [Data_NewValue("(0)")]
        public bool UseMicrosoft { get; set; }
        [Data_NewValue("(0)")]
        public bool UseTwitter { get; set; }

        public bool DefinedFacebook {
            get {
                return !string.IsNullOrWhiteSpace(WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "FacebookAccount:Public")) &&
                        !string.IsNullOrWhiteSpace(WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "FacebookAccount:Private"));
            }
        }
        public bool DefinedGoogle {
            get {
                return !string.IsNullOrWhiteSpace(WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "GoogleAccount:Public")) &&
                        !string.IsNullOrWhiteSpace(WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "GoogleAccount:Private"));
            }
        }
        public bool DefinedMicrosoft {
            get {
                return !string.IsNullOrWhiteSpace(WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "MicrosoftAccount:Public")) &&
                        !string.IsNullOrWhiteSpace(WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "MicrosoftAccount:Private"));
            }
        }
        public bool DefinedTwitter {
            get {
                return !string.IsNullOrWhiteSpace(WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "TwitterAccount:Public")) &&
                        !string.IsNullOrWhiteSpace(WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "TwitterAccount:Private"));
            }
        }

        public LoginConfigData() { }
    }

    public class LoginConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        private static object _lockObject = new object();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public LoginConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(DataProvider); }
        public LoginConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(DataProvider); }

        private IDataProvider<int, LoginConfigData> DataProvider {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName)) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            _dataProvider = new YetaWF.DataProvider.FileDataProvider<int, LoginConfigData>(
                                Path.Combine(YetaWFManager.DataFolder, AreaName, SiteIdentity.ToString()),
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLSimpleObjectDataProvider<int, LoginConfigData>(AreaName, SQLDbo, SQLConn,
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<int, LoginConfigData> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public static LoginConfigData GetConfig() {
            using (LoginConfigDataProvider configDP = new LoginConfigDataProvider()) {
                return configDP.GetItem();
            }
        }
        public LoginConfigData GetItem() {
            LoginConfigData config = DataProvider.Get(KEY);
            if (config == null) {
                lock (_lockObject) {
                    config = DataProvider.Get(KEY);
                    if (config == null) {
                        config = new LoginConfigData() {
                            Id = KEY,
                            AllowUserRegistration = true,
                            RegistrationType = RegistrationTypeEnum.EmailOnly,
                            SavePlainTextPassword = true,
                            Captcha = false,
                            VerifyNewUsers = false,
                            ApproveNewUsers = false,
                            NotifyAdminNewUsers = false,
                            BccVerification = false,
                            BccForgottenPassword = false,
                            PersistentLogin = true,
                        };
                        AddConfig(config);
                    }
                }
            }
            return config;
        }
        private void AddConfig(LoginConfigData data) {
            data.Id = KEY;
            if (!DataProvider.Add(data))
                throw new InternalError("Unexpected error adding settings");
        }
        public void UpdateConfig(LoginConfigData data) {
            data.Id = KEY;
            UpdateStatusEnum status = DataProvider.Update(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
        }

        public List<AuthenticationDescription> GetActiveExternalLoginProviders() {
            LoginConfigData configData = GetConfig();
            List <AuthenticationDescription> list = new List<AuthenticationDescription>();
            List<AuthenticationDescription> loginProviders = Manager.CurrentContext.GetOwinContext().Authentication.GetExternalAuthenticationTypes().ToList();
            foreach (AuthenticationDescription provider in loginProviders) {
                string name = provider.AuthenticationType;
                if (name == "Facebook" && configData.UseFacebook && configData.DefinedFacebook)
                    list.Add(provider);
                else if (name == "Google" && configData.UseGoogle && configData.DefinedGoogle)
                    list.Add(provider);
                else if (name == "Microsoft" && configData.UseMicrosoft && configData.DefinedMicrosoft)
                    list.Add(provider);
                else if (name == "Twitter" && configData.UseTwitter && configData.DefinedTwitter)
                    list.Add(provider);
            }
            return list;
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public bool IsInstalled() {
            return DataProvider.IsInstalled();
        }
        public bool InstallModel(List<string> errorList) {
            return DataProvider.InstallModel(errorList);
        }
        public bool UninstallModel(List<string> errorList) {
            return DataProvider.UninstallModel(errorList);
        }
        public void AddSiteData() {
            DataProvider.AddSiteData();
        }
        public void RemoveSiteData() {
            DataProvider.RemoveSiteData();
        }
        public bool ExportChunk(int chunk, SerializableList<SerializableFile> fileList, out object obj) {
            return DataProvider.ExportChunk(chunk, fileList, out obj);
        }
        public void ImportChunk(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            DataProvider.ImportChunk(chunk, fileList, obj);
        }
    }
}
