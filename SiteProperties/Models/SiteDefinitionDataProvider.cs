/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SiteProperties#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Security;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
using YetaWF.Modules.SiteProperties.Modules;

namespace YetaWF.Modules.SiteProperties.Models {

    public class SiteDefinitionDataProvider : DataProviderImpl, IInstallableModel, IInitializeApplicationStartup {

        // STARTUP
        // STARTUP
        // STARTUP

        public async Task InitializeApplicationStartupAsync() {
            // The SiteDefinitionDataProvider has two permanent disposable objects
            SiteDefinition.LoadSiteDefinitionAsync = LoadSiteDefinitionAsync;
            SiteDefinition.SaveSiteDefinitionAsync = SaveSiteDefinitionAsync;
            SiteDefinition.RemoveSiteDefinitionAsync = RemoveSiteDefinitionAsync;
            SiteDefinition.GetSitesAsync = GetItemsAsync;
            SiteDefinition.LoadStaticSiteDefinitionAsync = LoadStaticSiteDefinitionAsync;
            SiteDefinition.LoadTestSiteDefinitionAsync = LoadTestSiteDefinitionAsync;

            await LoadSitesCacheAsync();
        }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public const string LOCKEDURL = "/Maintenance/Offline For Maintenance.html";

        private static AsyncLock lockObject = new AsyncLock();

        static SiteDefinitionDataProvider() {
            SiteCache = new Dictionary<string, SiteDefinition>();
            TestSiteCache = new Dictionary<string, SiteDefinition>();
        }

        public SiteDefinitionDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<string, SiteDefinition> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<string, SiteDefinition> CreateDataProvider() {
            Package package = YetaWF.Modules.SiteProperties.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName, Cacheable: true, Parms: new { IdentitySeed = SiteDefinition.SiteIdentitySeed, NoLanguages = true });
        }

        // API
        // API
        // API

        static private Dictionary<string, SiteDefinition> SiteCache { get; set; }
        static private Dictionary<string, SiteDefinition> StaticSiteCache { get; set; }
        static private Dictionary<string, SiteDefinition> TestSiteCache { get; set; }

        private async Task LoadSitesCacheAsync() {
            StaticSiteCache = new Dictionary<string, SiteDefinition>();
            if (SiteDefinition.INITIAL_INSTALL) return;
            DataProviderGetRecords<SiteDefinition> sites = await GetItemsAsync(0, 0, null, null);
            foreach (SiteDefinition site in sites.Data) {
                site.OriginalSiteDomain = site.SiteDomain;
                if (!string.IsNullOrWhiteSpace(site.StaticDomain) && !StaticSiteCache.ContainsKey(site.StaticDomain.ToLower()))
                    StaticSiteCache.Add(site.StaticDomain.ToLower(), site);
                SiteCache.Add(site.SiteDomain.ToLower(), site);
                if (!string.IsNullOrWhiteSpace(site.SiteTestDomain) && !TestSiteCache.ContainsKey(site.SiteTestDomain.ToLower()))
                    TestSiteCache.Add(site.SiteTestDomain.ToLower(), site);
            }
        }

        /// <summary>
        /// Load the site definition for the current site
        /// </summary>
        /// <returns></returns>
        public async Task<SiteDefinition> LoadSiteDefinitionAsync(string siteDomain) {
            if (await DataProvider.IsInstalledAsync()) {
                SiteDefinition site;
                if (siteDomain == null || string.Compare(siteDomain, "Localhost", true) == 0)
                    siteDomain = YetaWFManager.DefaultSiteName;
                if (!SiteCache.TryGetValue(siteDomain.ToLower(), out site))
                    return null;
                return site;
            }
            return null;
        }
        public SiteDefinition _defaultSite = null;

        /// <summary>
        /// Load a site definition for a static site domain.
        /// </summary>
        /// <param name="staticDomain">The domain name.</param>
        /// <returns></returns>
        public Task<SiteDefinition> LoadStaticSiteDefinitionAsync(string staticDomain) {
            SiteDefinition site;
            if (StaticSiteCache.TryGetValue(staticDomain.ToLower(), out site))
                return Task.FromResult(site);
            return Task.FromResult<SiteDefinition>(null);
        }

        /// <summary>
        /// Load a site definition for a test site domain.
        /// </summary>
        /// <param name="testDomain">The domain name.</param>
        /// <returns></returns>
        public Task<SiteDefinition> LoadTestSiteDefinitionAsync(string testDomain) {
            SiteDefinition site;
            if (TestSiteCache.TryGetValue(testDomain.ToLower(), out site))
                return Task.FromResult(site);
            return Task.FromResult<SiteDefinition>(null);
        }

        /// <summary>
        /// Save the site definition for the current site
        /// </summary>
        internal async Task SaveSiteDefinitionAsync(SiteDefinition site) {

            SiteDefinition origSite = YetaWF.Core.Audit.Auditing.Active ? await LoadSiteDefinitionAsync(site.OriginalSiteDomain) : null;

            using (await lockObject.LockAsync()) { // protect SiteCache locally

                SiteCache.Remove(site.SiteDomain.ToLower());
                if (!string.IsNullOrWhiteSpace(site.StaticDomain))
                    StaticSiteCache.Remove(site.StaticDomain.ToLower());
                if (!string.IsNullOrWhiteSpace(site.SiteTestDomain))
                    TestSiteCache.Remove(site.SiteTestDomain.ToLower());
                SiteCache.Add(site.SiteDomain.ToLower(), site);
                if (!string.IsNullOrWhiteSpace(site.StaticDomain))
                    StaticSiteCache.Add(site.StaticDomain.ToLower(), site);
                if (!string.IsNullOrWhiteSpace(site.SiteTestDomain))
                    TestSiteCache.Add(site.SiteTestDomain.ToLower(), site);

                AddLockedStatus(site);
                CleanData(site);
                await SaveImagesAsync(ModuleDefinition.GetPermanentGuid(typeof(SitePropertiesModule)), site);
                if (site.OriginalSiteDomain != null) {
                    UpdateStatusEnum status = await DataProvider.UpdateAsync(site.OriginalSiteDomain, site.SiteDomain, site);
                    if (status != UpdateStatusEnum.OK)
                        throw new Error(this.__ResStr("updFail", "Can't update site definition - it may have already been removed", site.OriginalSiteDomain));
                } else {
                    if (!await DataProvider.AddAsync(site))
                        throw new Error(this.__ResStr("siteExists", "Can't add new site \"{0}\" - site already exists", site.SiteDomain));
                    site.OriginalSiteDomain = site.SiteDomain;
                }
                // update appsettings.json
                if (string.Compare(YetaWFManager.DefaultSiteName, site.OriginalSiteDomain, true) == 0 && site.SiteDomain != site.OriginalSiteDomain) {
                    WebConfigHelper.SetValue<string>(YetaWF.Core.Controllers.AreaRegistration.CurrentPackage.AreaName, "DEFAULTSITE", site.SiteDomain);
                    await WebConfigHelper.SaveAsync();
                    await Auditing.AddAuditAsync($"{nameof(SiteDefinitionDataProvider)}.{nameof(SaveSiteDefinitionAsync)}", site.OriginalSiteDomain, Guid.Empty,
                        $"DEFAULTSITE", RequiresRestart: true
                    );
                }
            }
            await Auditing.AddAuditAsync($"{nameof(SiteDefinitionDataProvider)}.{nameof(SaveSiteDefinitionAsync)}", site.OriginalSiteDomain, Guid.Empty,
                "Save Site Settings",
                DataBefore: origSite,
                DataAfter: site,
                ExpensiveMultiInstance: true
            );
        }

        private void AddLockedStatus(SiteDefinition siteDef) {
            string lockedForIP = WebConfigHelper.GetValue<string>("YetaWF_Core", "LOCKED-FOR-IP");
            if (!string.IsNullOrWhiteSpace(lockedForIP)) {
                // web config
                siteDef.LockedExternal = true;// locked by web config
                siteDef.LockedExternalForIP = lockedForIP;
            } else if (siteDef.Locked) {
                // user lock
                siteDef.LockedExternal = false;
                if (string.IsNullOrWhiteSpace(siteDef.LockedForIP) && HaveManager)
                    siteDef.LockedForIP = Manager.UserHostAddress;
            } else {
                // no lock
                siteDef.LockedExternal = false;
            }
            if (string.IsNullOrWhiteSpace(siteDef.LockedUrl))
                siteDef.LockedUrl = SiteDefinitionDataProvider.LOCKEDURL;
        }

        private void CleanData(SiteDefinition siteDef) {
            if (string.IsNullOrWhiteSpace(siteDef.PublicKey) || string.IsNullOrWhiteSpace(siteDef.PrivateKey)) {
                string publicKey, privateKey;
                RSACrypto.MakeNewKeys(out publicKey, out privateKey);
                siteDef.PublicKey = publicKey;
                siteDef.PrivateKey = privateKey;
            }
        }

        internal async Task RemoveSiteDefinitionAsync() {
            SiteDefinition site = Manager.CurrentSite;

            if (site.IsDefaultSite)
                throw new Error(this.__ResStr("cantDeleteDefault", "The default site of a YetaWF instance cannot be removed"));

            using (await lockObject.LockAsync()) { // protect SiteCache locally

                // remove all saved data
                SiteCache.Remove(site.SiteDomain.ToLower());
                if (!string.IsNullOrWhiteSpace(site.StaticDomain))
                    StaticSiteCache.Remove(site.StaticDomain.ToLower());
                if (!string.IsNullOrWhiteSpace(site.SiteTestDomain))
                    TestSiteCache.Remove(site.SiteTestDomain.ToLower());

                await Package.RemoveSiteDataAsync(Manager.SiteFolder);
                await DataProvider.RemoveAsync(site.SiteDomain);// remove domain
            }
            await Auditing.AddAuditAsync($"{nameof(SiteDefinitionDataProvider)}.{nameof(SaveSiteDefinitionAsync)}", site.SiteDomain, Guid.Empty,
                "Remove Site",
                DataBefore: site,
                DataAfter: null,
                ExpensiveMultiInstance: true
            );
        }

        /// <summary>
        /// Retrieve sites
        /// </summary>
        internal async Task<DataProviderGetRecords<SiteDefinition>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            return await DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public new async Task<bool> InstallModelAsync(List<string> errorList) {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Installing new models is not possible when distributed caching is enabled");
            if (!await DataProvider.InstallModelAsync(errorList))
                return false;
            try {
                SiteDefinition siteDef = new SiteDefinition();
                if (SiteDefinition.INITIAL_INSTALL) {
                    siteDef.SiteDomain = YetaWFManager.DefaultSiteName;
                    if (!await DataProvider.AddAsync(siteDef))
                        throw new InternalError("Couldn't add default site");
                    Manager.CurrentSite = siteDef;
                    siteDef.OriginalSiteDomain = siteDef.SiteDomain;
                }
                return true;
            } catch (Exception exc) {
                errorList.Add(string.Format("{0}: {1}", typeof(SiteDefinition).FullName, ErrorHandling.FormatExceptionMessage(exc)));
                return false;
            }
        }
    }
}