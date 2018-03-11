/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Languages.DataProvider {

    public class LocalizeConfigData {

        public enum TranslationServiceEnum {
            [EnumDescription("None", "None")]
            None = 0,
            [EnumDescription("Google Translate")]
            GoogleTranslate = 1,
            [EnumDescription("Microsoft Translator")]
            MicrosoftTranslator = 2,
        }

        public const int MaxGoogleTranslateAPIKey = 100;
        public const int MaxGoogleTranslateAppName = 100;
        public const int MaxMSClientKey = 100;

        [Data_PrimaryKey]
        public int Id { get; set; }

        public TranslationServiceEnum TranslationService { get; set; }

        [StringLength(MaxGoogleTranslateAPIKey)]
        public string GoogleTranslateAPIKey { get; set; }
        [StringLength(MaxGoogleTranslateAppName)]
        public string GoogleTranslateAppName { get; set; }

        [StringLength(MaxMSClientKey)]
        public string MSClientKey { get; set; }

        public LocalizeConfigData() { }
    }

    public class LocalizeConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        private static AsyncLock _lockObject = new AsyncLock();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public LocalizeConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public LocalizeConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, LocalizeConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, LocalizeConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.Languages.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Config", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public static async Task<LocalizeConfigData> GetConfigAsync() {
            using (LocalizeConfigDataProvider configDP = new LocalizeConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<LocalizeConfigData> GetItemAsync() {
            LocalizeConfigData config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                using (await _lockObject.LockAsync()) {
                    config = await DataProvider.GetAsync(KEY);
                    if (config == null) {
                        config = new LocalizeConfigData();
                        await AddConfigAsync(config);
                    }
                }
            }
            return config;
        }
        private async Task AddConfigAsync(LocalizeConfigData data) {
            data.Id = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding settings");
        }
        public async Task UpdateConfigAsync(LocalizeConfigData data) {
            data.Id = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
        }
    }
}
