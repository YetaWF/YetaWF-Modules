/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/CurrencyConverter#License */

using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.CurrencyConverter.DataProvider {
    public class ConfigData {

        public const int MaxAppID = 40;

        [Data_PrimaryKey]
        public int Id { get; set; }

        [StringLength(MaxAppID)]
        public string AppID { get; set; }
        public bool UseHttps { get; set; }

        public ConfigData() { }
    }

    public class ConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        private static AsyncLock _lockObject = new AsyncLock();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public ConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public ConfigDataProvider(int siteIdentity) : base(0) { }

        private IDataProviderAsync<int, ConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProviderAsync<int, ConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.CurrencyConverter.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName, Cacheable: true);
        }

        // API
        // API
        // API

        public static async Task<ConfigData> GetConfigAsync() {
            using (ConfigDataProvider configDP = new ConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<ConfigData> GetItemAsync() {
            ConfigData config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                using (await _lockObject.LockAsync()) {
                    config = await DataProvider.GetAsync(KEY);
                    if (config == null) {
                        config = new ConfigData() {
                            Id = KEY,
                            AppID = "",
                            UseHttps = false,
                        };
                        await AddConfigAsync(config);
                    }
                }
            }
            return config;
        }
        private async Task AddConfigAsync(ConfigData data) {
            data.Id = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding currency converter settings");
        }
        public async Task UpdateConfigAsync(ConfigData data) {
            data.Id = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving currency converter configuration {0}", status);
        }
    }
}
