/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.Basics.DataProvider {

    public class RecaptchaV2ConfigDataProviderInit : IInitializeApplicationStartup {

        // STARTUP
        // STARTUP
        // STARTUP

        public void InitializeApplicationStartup() {
            RecaptchaV2Config.LoadRecaptchaV2Config = RecaptchaV2ConfigDataProvider.LoadRecaptchaV2Config;
            RecaptchaV2Config.SaveRecaptchaV2Config = RecaptchaV2ConfigDataProvider.SaveRecaptchaV2Config;
        }
    }

    public class RecaptchaV2ConfigDataProvider : DataProviderImpl, IInstallableModel {

        public static readonly int KEY = 1;

        private static object _lockObject = new object();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public RecaptchaV2ConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public RecaptchaV2ConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, RecaptchaV2Config> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, RecaptchaV2Config> CreateDataProvider() {
            Package package = YetaWF.Modules.Basics.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_RecaptchaV2", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public static RecaptchaV2Config GetConfig() {
            using (RecaptchaV2ConfigDataProvider configDP = new RecaptchaV2ConfigDataProvider()) {
                return configDP.GetItem();
            }
        }
        public RecaptchaV2Config GetItem() {
            RecaptchaV2Config config = DataProvider.Get(KEY);
            if (config == null) {
                lock (_lockObject) {
                    config = DataProvider.Get(KEY);
                    if (config == null) {
                        config = new RecaptchaV2Config();
                        AddConfig(config);
                    }
                }
            }
            return config;
        }
        public void AddConfig(RecaptchaV2Config data) {
            data.Key = KEY;
            if (!DataProvider.Add(data))
                throw new InternalError("Unexpected error adding RecaptchaV2 Settings");
        }
        public void UpdateConfig(RecaptchaV2Config data) {
            data.Key = KEY;
            UpdateStatusEnum status = DataProvider.Update(data.Key, data.Key, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Can't save captcha configuration {0}", status);
        }

        internal static RecaptchaV2Config LoadRecaptchaV2Config() {
            using (RecaptchaV2ConfigDataProvider dp = new RecaptchaV2ConfigDataProvider()) {
                return dp.GetItem();
            }
        }

        internal static void SaveRecaptchaV2Config(RecaptchaV2Config config) {
            using (RecaptchaV2ConfigDataProvider dp = new RecaptchaV2ConfigDataProvider()) {
                dp.UpdateConfig(config);
            }
        }
    }
}
