/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Languages#License */

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

        private static object _lockObject = new object();

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

        public static LocalizeConfigData GetConfig() {
            using (LocalizeConfigDataProvider configDP = new LocalizeConfigDataProvider()) {
                return configDP.GetItem();
            }
        }
        public LocalizeConfigData GetItem() {
            LocalizeConfigData config = DataProvider.Get(KEY);
            if (config == null) {
                lock (_lockObject) {
                    config = DataProvider.Get(KEY);
                    if (config == null) {
                        config = new LocalizeConfigData();
                        AddConfig(config);
                    }
                }
            }
            return config;
        }
        private void AddConfig(LocalizeConfigData data) {
            data.Id = KEY;
            if (!DataProvider.Add(data))
                throw new InternalError("Unexpected error adding settings");
        }
        public void UpdateConfig(LocalizeConfigData data) {
            data.Id = KEY;
            UpdateStatusEnum status = DataProvider.Update(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
        }
    }
}
