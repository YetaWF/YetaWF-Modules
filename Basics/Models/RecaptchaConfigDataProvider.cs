/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using System.Threading.Tasks;
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

        private static AsyncLock _lockObject = new AsyncLock();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public RecaptchaConfigDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProviderAsync<int, RecaptchaConfig> DataProvider { get { return GetDataProvider(); } }

        private IDataProviderAsync<int, RecaptchaConfig> CreateDataProvider() {
            Package package = YetaWF.Modules.Basics.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName, Cacheable: true);
        }

        // API
        // API
        // API

        public static async Task<RecaptchaConfig> GetConfigAsync() {
            using (RecaptchaConfigDataProvider configDP = new RecaptchaConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<RecaptchaConfig> GetItemAsync() {
            RecaptchaConfig config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                using (await _lockObject.LockAsync()) {
                    config = await DataProvider.GetAsync(KEY);
                    if (config == null) {
                        config = new RecaptchaConfig();
                        await AddConfigAsync(config);
                    }
                }
            }
            return config;
        }
        public async Task AddConfigAsync(RecaptchaConfig data) {
            data.Key = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding recaptcha settings");
        }
        public async Task UpdateConfigAsync(RecaptchaConfig data) {
            data.Key = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Key, data.Key, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Can't save captcha configuration {0}", status);
        }

        internal static async Task<RecaptchaConfig> LoadRecaptchaConfig() {
            using (RecaptchaConfigDataProvider recaptchaDP = new RecaptchaConfigDataProvider()) {
                return await recaptchaDP.GetItemAsync();
            }
        }

        internal static async Task SaveRecaptchaConfig(RecaptchaConfig config) {
            using (RecaptchaConfigDataProvider recaptchaDP = new RecaptchaConfigDataProvider()) {
                await recaptchaDP.UpdateConfigAsync(config);
            }
        }
    }
}
