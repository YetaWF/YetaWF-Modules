/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SitePropertiesService#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Packages;
using YetaWF.Core.Site;
using YetaWF.Core.Support;

namespace YetaWF.Modules.SitePropertiesService.Models {

    public class SiteDefinitionDataProvider : DataProviderImpl, IInitializeApplicationStartup {

        // STARTUP
        // STARTUP
        // STARTUP

        /// <summary>
        /// Called when any node of a (single- or multi-instance) site is starting up.
        /// </summary>
        public async Task InitializeApplicationStartupAsync() {
            // The SiteDefinitionDataProvider has two permanent disposable objects
            SiteDefinition.LoadSiteDefinitionAsync = LoadSiteDefinitionAsync;

            await LoadSitesCacheAsync();
        }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        static SiteDefinitionDataProvider() {
            SiteCache = new Dictionary<string, SiteDefinition>();
        }

        public SiteDefinitionDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<string, SiteDefinition> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<string, SiteDefinition>? CreateDataProvider() {
            Package package = YetaWF.Modules.SitePropertiesService.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, "YetaWF_SiteProperties", Cacheable: false, Parms: new { NoLanguages = true });
        }

        // API
        // API
        // API

        static private Dictionary<string, SiteDefinition> SiteCache { get; set; }

        private async Task LoadSitesCacheAsync() {
            DataProviderGetRecords<SiteDefinition> sites = await GetItemsAsync(0, 0, null, null);
            foreach (SiteDefinition site in sites.Data) {
                site.OriginalSiteDomain = site.SiteDomain;
                SiteCache.Add(site.SiteDomain.ToLower(), site);
            }
        }

        /// <summary>
        /// Load the site definition for the current site
        /// </summary>
        /// <returns></returns>
        public Task<SiteDefinition?> LoadSiteDefinitionAsync(string? siteDomain) {
            SiteDefinition? site;
            if (siteDomain == null)
                siteDomain = YetaWFManager.DefaultSiteName;
            if (!SiteCache.TryGetValue(siteDomain.ToLower(), out site))
                return Task.FromResult<SiteDefinition?>(null);
            return Task.FromResult<SiteDefinition?>(site);
        }
        public SiteDefinition? _defaultSite = null;

        /// <summary>
        /// Retrieve sites
        /// </summary>
        internal async Task<DataProviderGetRecords<SiteDefinition>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) {
            return await DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
    }
}