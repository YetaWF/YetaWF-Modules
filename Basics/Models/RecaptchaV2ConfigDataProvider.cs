/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Audit;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Basics.DataProvider {

    public class RecaptchaV2ConfigDataProviderInit : IInitializeApplicationStartup {

        // STARTUP
        // STARTUP
        // STARTUP

        public Task InitializeApplicationStartupAsync() {
            RecaptchaV2Config.LoadRecaptchaV2ConfigAsync = RecaptchaV2ConfigDataProvider.LoadRecaptchaV2Config;
            RecaptchaV2Config.SaveRecaptchaV2ConfigAsync = RecaptchaV2ConfigDataProvider.SaveRecaptchaV2Config;
            return Task.CompletedTask;
        }
    }

    public class RecaptchaV2ConfigDataProvider : DataProviderImpl, IInstallableModel {

        public static readonly int KEY = 1;

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public RecaptchaV2ConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public RecaptchaV2ConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, RecaptchaV2Config> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, RecaptchaV2Config>? CreateDataProvider() {
            Package package = YetaWF.Modules.Basics.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_RecaptchaV2", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public static async Task<RecaptchaV2Config> GetConfigAsync() {
            using (RecaptchaV2ConfigDataProvider configDP = new RecaptchaV2ConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<RecaptchaV2Config> GetItemAsync() {
            RecaptchaV2Config? config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                config = new RecaptchaV2Config();
                await AddConfigAsync(config);
            }
            return config;
        }
        public async Task AddConfigAsync(RecaptchaV2Config data) {
            data.Key = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding RecaptchaV2 Settings");
            await Auditing.AddAuditAsync($"{nameof(RecaptchaV2ConfigDataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
                "Add RecaptchaV2 Config",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
        public async Task UpdateConfigAsync(RecaptchaV2Config data) {
            RecaptchaV2Config? origConfig = Auditing.Active ? await GetItemAsync() : null;
            data.Key = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Key, data.Key, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Can't save captcha configuration {0}", status);
            await Auditing.AddAuditAsync($"{nameof(RecaptchaV2ConfigDataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
                "Update RecaptchaV2 Config",
                DataBefore: origConfig,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }

        internal static async Task<RecaptchaV2Config> LoadRecaptchaV2Config() {
            using (RecaptchaV2ConfigDataProvider dp = new RecaptchaV2ConfigDataProvider()) {
                return await dp.GetItemAsync();
            }
        }

        internal static async Task SaveRecaptchaV2Config(RecaptchaV2Config config) {
            using (RecaptchaV2ConfigDataProvider dp = new RecaptchaV2ConfigDataProvider()) {
                await dp.UpdateConfigAsync(config);
            }
        }
    }
}
