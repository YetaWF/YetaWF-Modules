﻿/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Backups#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Backups.DataProvider {

    public class ConfigData {

        [Data_PrimaryKey]
        public int Id { get; set; }

        public int Days { get; set; }
        public bool DataOnly { get; set; }

        public ConfigData() {
            Days = 7;
            DataOnly = true;
        }
    }

    public class ConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public ConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public ConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, ConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, ConfigData>? CreateDataProvider() {
            Package package = YetaWF.Modules.Backups.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Config", SiteIdentity: SiteIdentity, Cacheable: true);
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
            ConfigData? config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                config = new ConfigData();
                await AddConfigAsync(config);
            }
            return config;
        }
        private async Task AddConfigAsync(ConfigData data) {
            data.Id = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding settings");
            await Auditing.AddAuditAsync($"{nameof(ConfigDataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
                "Add Backup Config",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
        public async Task UpdateConfigAsync(ConfigData data) {
            ConfigData? origConfig = Auditing.Active ? await GetItemAsync() : null;
            data.Id = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
            await Auditing.AddAuditAsync($"{nameof(ConfigDataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
                "Update Backup Config",
                DataBefore: origConfig,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
    }
}
