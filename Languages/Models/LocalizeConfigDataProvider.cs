/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Audit;
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
            [EnumDescription("Microsoft Translator - Azure")]
            MicrosoftTranslator = 2,
        }

        public const int MaxGoogleTranslateAPIKey = 100;
        public const int MaxGoogleTranslateAppName = 100;
        public const int MaxMSClientKey = 100;
        public const int MaxMSRegion = 100;

        [Data_PrimaryKey]
        public int Id { get; set; }

        public TranslationServiceEnum TranslationService { get; set; }

        [StringLength(MaxGoogleTranslateAPIKey)]
        public string? GoogleTranslateAPIKey { get; set; }
        [StringLength(MaxGoogleTranslateAppName)]
        public string? GoogleTranslateAppName { get; set; }

        [StringLength(Globals.MaxUrl)]
        [Data_NewValue]
        public string? MSTextTranslationUrl { get; set; }
        [StringLength(MaxMSRegion)]
        [Data_NewValue]
        public string? MSTextTranslationRegion { get; set; }
        [StringLength(MaxMSClientKey)]
        public string? MSClientKey { get; set; }
        [Data_NewValue]
        public int MSRequestLimit { get; set; }

        public LocalizeConfigData() {
            MSRequestLimit = 2;
        }
    }

    public class LocalizeConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public LocalizeConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public LocalizeConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, LocalizeConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, LocalizeConfigData>? CreateDataProvider() {
            Package package = YetaWF.Modules.Languages.AreaRegistration.CurrentPackage;
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
            LocalizeConfigData? config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                config = new LocalizeConfigData();
                await AddConfigAsync(config);
            }
            return config;
        }
        private async Task AddConfigAsync(LocalizeConfigData data) {
            data.Id = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding settings");
            await Auditing.AddAuditAsync($"{nameof(LocalizeConfigDataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
                "Add Localize Config",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
        public async Task UpdateConfigAsync(LocalizeConfigData data) {
            LocalizeConfigData? origConfig = Auditing.Active ? await GetItemAsync() : null;
            data.Id = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
            await Auditing.AddAuditAsync($"{nameof(LocalizeConfigDataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
                "Update Localize Config",
                DataBefore: origConfig,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
    }
}
