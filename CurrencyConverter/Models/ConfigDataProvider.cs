/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/CurrencyConverter#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

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

        private static object _lockObject = new object();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public ConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public ConfigDataProvider(int siteIdentity) : base(0) { }

        private IDataProvider<int, ConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, ConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.CurrencyConverter.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package.AreaName,
                () => { // File
                    return new FileDataProvider<int, ConfigData>(
                        Path.Combine(YetaWFManager.DataFolder, AreaName),
                        Cacheable: true);
                },
                (dbo, conn) => {  // SQL
                    return new SQLSimpleObjectDataProvider<int, ConfigData>(AreaName, dbo, conn,
                        Cacheable: true);
                },
                () => { // External
                    return MakeExternalDataProvider(new { AreaName = AreaName, Cacheable = true });
                }
            );
        }

        // API
        // API
        // API

        public static ConfigData GetConfig() {
            using (ConfigDataProvider configDP = new ConfigDataProvider()) {
                return configDP.GetItem();
            }
        }
        public ConfigData GetItem() {
            ConfigData config = DataProvider.Get(KEY);
            if (config == null) {
                lock (_lockObject) {
                    config = DataProvider.Get(KEY);
                    if (config == null) {
                        config = new ConfigData() {
                            Id = KEY,
                            AppID = "",
                            UseHttps = false,
                        };
                        AddConfig(config);
                    }
                }
            }
            return config;
        }
        private void AddConfig(ConfigData data) {
            data.Id = KEY;
            if (!DataProvider.Add(data))
                throw new InternalError("Unexpected error adding currency converter settings");
        }
        public void UpdateConfig(ConfigData data) {
            data.Id = KEY;
            UpdateStatusEnum status = DataProvider.Update(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving currency converter configuration {0}", status);
        }
    }
}
