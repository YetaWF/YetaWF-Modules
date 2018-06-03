/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Audit;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Basics.DataProvider {

    public class RecaptchaConfigDataProviderInit : IInitializeApplicationStartup {

        // STARTUP
        // STARTUP
        // STARTUP

        public Task InitializeApplicationStartupAsync() {
            RecaptchaConfig.LoadRecaptchaConfigAsync = RecaptchaConfigDataProvider.LoadRecaptchaConfigAsync;
            RecaptchaV2Config.SaveRecaptchaConfigAsync = RecaptchaConfigDataProvider.SaveRecaptchaConfigAsync;
            return Task.CompletedTask;
        }

    }

    public class RecaptchaConfigDataProvider : DataProviderImpl, IInstallableModel {

        public static readonly int KEY = 1;

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

        public static async Task<RecaptchaConfig> GetConfigAsync() {
            using (RecaptchaConfigDataProvider configDP = new RecaptchaConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<RecaptchaConfig> GetItemAsync() {
            RecaptchaConfig config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                config = new RecaptchaConfig();
                await AddConfigAsync(config);
            }
            return config;
        }
        public async Task AddConfigAsync(RecaptchaConfig data) {
            data.Key = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding recaptcha settings");
            await Auditing.AddAuditAsync($"{nameof(RecaptchaConfigDataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
                "Add Recaptcha Config",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
        public async Task UpdateConfigAsync(RecaptchaConfig data) {
            RecaptchaConfig origConfig = Auditing.Active ? await GetItemAsync() : null;
            data.Key = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Key, data.Key, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Can't save captcha configuration {0}", status);
            await Auditing.AddAuditAsync($"{nameof(RecaptchaConfigDataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
                "Update Recaptcha Config",
                DataBefore: origConfig,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }

        internal static async Task<RecaptchaConfig> LoadRecaptchaConfigAsync() {
            using (RecaptchaConfigDataProvider recaptchaDP = new RecaptchaConfigDataProvider()) {
                return await recaptchaDP.GetItemAsync();
            }
        }

        internal static async Task SaveRecaptchaConfigAsync(RecaptchaConfig config) {
            using (RecaptchaConfigDataProvider recaptchaDP = new RecaptchaConfigDataProvider()) {
                await recaptchaDP.UpdateConfigAsync(config);
            }
        }
    }
}
