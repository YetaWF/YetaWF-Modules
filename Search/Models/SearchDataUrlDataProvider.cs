/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Search#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Search.DataProvider {

    public class SearchDataUrl {

        public const int MaxTitle = 100;
        public const int MaxSummary = 500;
        public const int MaxCustomData = 300;

        [Data_Identity]
        public int SearchDataUrlId { get; set; }

        [Data_PrimaryKey, StringLength(Globals.MaxUrl)]
        public string PageUrl { get; set; } = null!;
        [Data_NewValue]
        public PageDefinition.PageSecurityType PageSecurity { get; set; }

        [StringLength(MaxTitle)]
        public string? PageTitle { get; set; }
        [StringLength(MaxSummary)]
        public string? PageSummary { get; set; }

        [StringLength(MaxCustomData)]
        [Data_NewValue]
        public string? CustomData { get; set; }

        public DateTime DatePageCreated { get; set; }
        public DateTime? DatePageUpdated { get; set; }

        public SearchDataUrl() { }
    }

    public class SearchDataUrlDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public SearchDataUrlDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public SearchDataUrlDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProviderIdentity<string, object, SearchDataUrl> DataProvider { get { return GetDataProvider(); } }

        private IDataProviderIdentity<string, object, SearchDataUrl>? CreateDataProvider() {
            if (SearchDataProvider.IsUsable) {
                Package package = YetaWF.Modules.Search.AreaRegistration.CurrentPackage;
                return MakeDataProvider(package, package.AreaName + "_Urls", SiteIdentity: SiteIdentity, Cacheable: true);
            } else {
                return null;
            }
        }

        // API
        // API
        // API

        public async Task<SearchDataUrl?> GetItemAsync(int id) {
            if (!SearchDataProvider.IsUsable) return null;
            return await DataProvider.GetByIdentityAsync(id);
        }
        internal async Task<SearchDataUrl?> GetItemByUrlAsync(string pageUrl) {
            if (!SearchDataProvider.IsUsable) return null;
            List<DataProviderFilterInfo>? filters = null;
            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(SearchDataUrl.PageUrl), Operator = "==", Value = pageUrl });
            SearchDataUrl? searchUrl = await DataProvider.GetOneRecordAsync(filters);
            return searchUrl;
        }
        public async Task<bool> AddItemAsync(SearchDataUrl data) {
            if (!SearchDataProvider.IsUsable) return false;
            return await DataProvider.AddAsync(data);
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(SearchDataUrl data) {
            if (!SearchDataProvider.IsUsable) return UpdateStatusEnum.RecordDeleted;
            return await DataProvider.UpdateByIdentityAsync(data.SearchDataUrlId, data);
        }
        public async Task<int> RemoveItemsAsync(List<DataProviderFilterInfo>? filters) {
            if (!SearchDataProvider.IsUsable) return 0;
            return await DataProvider.RemoveRecordsAsync(filters);
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public new async Task<bool> IsInstalledAsync() {
            if (DataProvider == null) return false;
            return await DataProvider.IsInstalledAsync();
        }
        public new async Task<bool> InstallModelAsync(List<string> errorList) {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Installing new models is not possible when distributed caching is enabled");
            if (!SearchDataProvider.IsUsable) return true;
            return await DataProvider.InstallModelAsync(errorList);
        }
        public new async Task<bool> UninstallModelAsync(List<string> errorList) {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Uninstalling models is not possible when distributed caching is enabled");
            if (!SearchDataProvider.IsUsable) return true;
            return await DataProvider.UninstallModelAsync(errorList);
        }
        public new async Task AddSiteDataAsync() {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Adding site data is not possible when distributed caching is enabled");
            if (!SearchDataProvider.IsUsable) return;
            await DataProvider.AddSiteDataAsync();
        }
        public new async Task RemoveSiteDataAsync() {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Removing site data is not possible when distributed caching is enabled");
            if (!SearchDataProvider.IsUsable) return;
            await DataProvider.RemoveSiteDataAsync();
        }
        public new Task<DataProviderExportChunk> ExportChunkAsync(int chunk, SerializableList<SerializableFile> fileList) {
            // we don't export search data
            return Task.FromResult(new DataProviderExportChunk {
                More = false,
            });
            //if (!SearchDataProvider.IsUsable) return true;
            //return DataProvider.ExportChunk(chunk, fileList, out obj);
        }
        public new Task ImportChunkAsync(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            // we don't export search data
            return Task.CompletedTask;
            //if (!SearchDataProvider.IsUsable) return;
            //DataProvider.ImportChunk(chunk, fileList, obj);
        }
    }
}
