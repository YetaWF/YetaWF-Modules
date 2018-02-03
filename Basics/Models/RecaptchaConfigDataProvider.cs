/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.Basics.DataProvider {

    public class RecaptchaConfigDataProviderInit : IInitializeApplicationStartup {

        // STARTUP
        // STARTUP
        // STARTUP

        public void InitializeApplicationStartup() {
            RecaptchaConfig.LoadRecaptchaConfig = RecaptchaConfigDataProvider.LoadRecaptchaConfig;
            RecaptchaConfig.SaveRecaptchaConfig = RecaptchaConfigDataProvider.SaveRecaptchaConfig;
        }

    }

    public class RecaptchaConfigDataProvider : DataProviderImpl, IInstallableModel {

        public static readonly int KEY = 1;

        private static object _lockObject = new object();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public RecaptchaConfigDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, RecaptchaConfig> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, RecaptchaConfig> CreateDataProvider() {
            Package package = YetaWF.Modules.Basics.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName, Cacheable: true);
        }

        // API
        // API
        // API

        public static RecaptchaConfig GetConfig() {
            using (RecaptchaConfigDataProvider configDP = new RecaptchaConfigDataProvider()) {
                return configDP.GetItem();
            }
        }
        public RecaptchaConfig GetItem() {
            RecaptchaConfig config = DataProvider.Get(KEY);
            if (config == null) {
                lock (_lockObject) {
                    config = DataProvider.Get(KEY);
                    if (config == null) {
                        config = new RecaptchaConfig();
                        AddConfig(config);
                    }
                }
            }
            return config;
        }
        public void AddConfig(RecaptchaConfig data) {
            data.Key = KEY;
            if (!DataProvider.Add(data))
                throw new InternalError("Unexpected error adding recaptcha settings");
        }
        public void UpdateConfig(RecaptchaConfig data) {
            data.Key = KEY;
            UpdateStatusEnum status = DataProvider.Update(data.Key, data.Key, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Can't save captcha configuration {0}", status);
        }

        internal static RecaptchaConfig LoadRecaptchaConfig() {
            using (RecaptchaConfigDataProvider recaptchaDP = new RecaptchaConfigDataProvider()) {
                return recaptchaDP.GetItem();
            }
        }

        internal static void SaveRecaptchaConfig(RecaptchaConfig config) {
            using (RecaptchaConfigDataProvider recaptchaDP = new RecaptchaConfigDataProvider()) {
                recaptchaDP.UpdateConfig(config);
            }
        }
    }
}
