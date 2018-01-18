/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TawkTo#License */

using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.TawkTo.DataProvider {

    public class ConfigData {

        public const int MaxAccount = 100;
        public const int MaxAPIKey = 100;
        public const int MaxCss = 100;

        [Data_PrimaryKey]
        public int Id { get; set; }

        [StringLength(MaxAccount)]
        public string Account { get; set; }
        [StringLength(MaxAPIKey)]
        public string APIKey { get; set; }

        [StringLength(MaxCss)]
        public string ExcludedPagesCss { get; set; }
        [StringLength(MaxCss)]
        public string IncludedPagesCss { get; set; }

        public bool IsConfigured {
            get {
                return !string.IsNullOrWhiteSpace(Account) && !string.IsNullOrWhiteSpace(APIKey);
            }
        }

        public ConfigData() { }
    }

    public class ConfigDataDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        private static object _lockObject = new object();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public ConfigDataDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public ConfigDataDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, ConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, ConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.TawkTo.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package.AreaName,
                () => { // File
                    return new FileDataProvider<int, ConfigData>(
                        Path.Combine(YetaWFManager.DataFolder, AreaName + "_Config", SiteIdentity.ToString()),
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                (dbo, conn) => {  // SQL
                    return new SQLSimpleObjectDataProvider<int, ConfigData>(AreaName + "_Config", dbo, conn,
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                () => { // External
                    return MakeExternalDataProvider(new { AreaName = AreaName + "_Config", CurrentSiteIdentity = SiteIdentity, Cacheable = true });
                }
            );
        }

        // API
        // API
        // API

        public static ConfigData GetConfig() {
            using (ConfigDataDataProvider configDP = new ConfigDataDataProvider()) {
                return configDP.GetItem();
            }
        }
        public ConfigData GetItem() {
            ConfigData config = DataProvider.Get(KEY);
            if (config == null) {
                lock (_lockObject) {
                    config = DataProvider.Get(KEY);
                    if (config == null) {
                        config = new ConfigData();
                        AddConfig(config);
                    }
                }
            }
            return config;
        }
        private void AddConfig(ConfigData data) {
            data.Id = KEY;
            if (!DataProvider.Add(data))
                throw new InternalError("Unexpected error adding settings");
        }
        public void UpdateConfig(ConfigData data) {
            data.Id = KEY;
            UpdateStatusEnum status = DataProvider.Update(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
        }
    }
}
