/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Search.DataProvider {

    public class SearchConfigData {

        [Data_PrimaryKey]
        public int Id { get; set; }

        [Range(2,10)]
        public int SmallestMixedToken { get; set; }
        [Range(2, 10)]
        public int SmallestUpperCaseToken { get; set; }

        public SearchConfigData() {
            SmallestMixedToken = 3;
            SmallestUpperCaseToken = 2;
        }
    }

    public class SearchConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public SearchConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public SearchConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, SearchConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, SearchConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.Search.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Config", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public static async Task<SearchConfigData> GetConfigAsync() {
            using (SearchConfigDataProvider configDP = new SearchConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<SearchConfigData> GetItemAsync() {
            SearchConfigData config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                config = new SearchConfigData();
                await AddConfigAsync(config);
            }
            return config;
        }
        private async Task AddConfigAsync(SearchConfigData data) {
            data.Id = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding settings");
            await Auditing.AddAuditAsync($"{nameof(SearchConfigDataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
                "Add Search Config",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
        public async Task UpdateConfigAsync(SearchConfigData data) {
            SearchConfigData origConfig = Auditing.Active ? await GetItemAsync() : null;
            data.Id = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving settings {0}", status);
            await Auditing.AddAuditAsync($"{nameof(SearchConfigDataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
                "Update Search Config",
                DataBefore: origConfig,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }


        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public new async Task<bool> IsInstalledAsync() {
            if (DataProvider == null) return false;
            return await base.IsInstalledAsync();
        }
        public new async Task<bool> InstallModelAsync(List<string> errorList) {
            if (DataProvider == null) return true;
            return await base.InstallModelAsync(errorList);
        }
        public new async Task<bool> UninstallModelAsync(List<string> errorList) {
            if (DataProvider == null) return true;
            return await base.UninstallModelAsync(errorList);
        }
        public new async Task AddSiteDataAsync() {
            if (DataProvider == null) return;
            await base.AddSiteDataAsync();
        }
        public new async Task RemoveSiteDataAsync() {
            if (DataProvider == null) return;
            await base.RemoveSiteDataAsync();
        }
        public new async Task<DataProviderExportChunk> ExportChunkAsync(int chunk, SerializableList<SerializableFile> fileList) {
            if (DataProvider == null) {
                return new DataProviderExportChunk {
                    More = false
                };
            }
            return await base.ExportChunkAsync(chunk, fileList);
        }
        public new async Task ImportChunkAsync(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            if (DataProvider == null) return;
            await base.ImportChunkAsync(chunk, fileList, obj);
        }
    }
}
