/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/SiteProperties#License */

using System;
using System.Collections.Generic;
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

        public void InitializeApplicationStartup() {
            // The SiteDefinitionDataProvider has two permanent disposable objects
            SiteDefinition.LoadSiteDefinition = LoadSiteDefinition;
            SiteDefinition.SaveSiteDefinition = SaveSiteDefinition;
            SiteDefinition.RemoveSiteDefinition = RemoveSiteDefinition;
            SiteDefinition.GetSites = GetItems;
            SiteDefinition.LoadStaticSiteDefinition = LoadStaticSiteDefinition;

            LoadStaticSitesCache();
        }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public const string LOCKEDURL = "/Maintenance/Offline For Maintenance.html";

        static SiteDefinitionDataProvider() {
            SiteCache = new Dictionary<string, SiteDefinition>();
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
        static private Dictionary<string, string> StaticSiteCache { get; set; }

        private void LoadStaticSitesCache() {
            StaticSiteCache = new Dictionary<string, string>();
            if (SiteDefinition.INITIAL_INSTALL) return;
            int total;
            List<SiteDefinition> sites = GetItems(0, 0, null, null, out total);
            foreach (SiteDefinition site in sites)
                if (!string.IsNullOrWhiteSpace(site.StaticDomain))
                    StaticSiteCache.Add(site.StaticDomain.ToLower(), site.SiteDomain);
        }

        private static void AddCache(SiteDefinition site) {
            RemoveCache(site);
            try {
                SiteCache.Add(site.SiteDomain.ToLower(), site);
            } catch (Exception) { }// could fail if added by two threads. Simply ignore, first just won, which is OK.
        }
        private static void RemoveCache(SiteDefinition siteDefinition) {
            try {
                SiteCache.Remove(siteDefinition.SiteDomain.ToLower());
            } catch (Exception) { }// could fail if it wasn't added.
        }

        /// <summary>
        /// Load the site definition for the current site
        /// </summary>
        /// <returns></returns>
        public SiteDefinition LoadSiteDefinition(string siteDomain) {
            if (DataProvider.IsInstalled()) {
                SiteDefinition site;
                if (siteDomain == null || string.Compare(siteDomain, "Localhost", true) == 0)
                    siteDomain = YetaWFManager.DefaultSiteName;
                if (SiteCache.TryGetValue(siteDomain.ToLower(), out site))
                    return site;
                site = DataProvider.Get(siteDomain);
                if (site == null)
                    return null;
                site.OriginalSiteDomain = site.SiteDomain;
                site.OriginalUseCDN = site.UseCDN;
                site.OriginalCDNUrl = site.CDNUrl;
                site.OriginalCDNUrlSecure = site.CDNUrlSecure;
                site.OriginalStaticDomain = site.StaticDomain;
                AddCache(site);
                AddLockedStatus(site);
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
        public SiteDefinition LoadStaticSiteDefinition(string staticDomain) {
            string domain;
            if (StaticSiteCache.TryGetValue(staticDomain.ToLower(), out domain)) {
                SiteDefinition site = LoadSiteDefinition(domain);
                return site;
            }
            return null;
        }

        /// <summary>
        /// Save the site definition for the current site
        /// </summary>
        internal bool SaveSiteDefinition(SiteDefinition site) {
            bool restartRequired = false;
            RemoveCache(site);// next load will add it again
            AddLockedStatus(site);
            CleanData(site);
            SaveImages(ModuleDefinition.GetPermanentGuid(typeof(SitePropertiesModule)), site);
            if (site.OriginalSiteDomain != null) {
                UpdateStatusEnum status = DataProvider.Update(site.OriginalSiteDomain, site.SiteDomain, site);
                if (status != UpdateStatusEnum.OK)
                    throw new Error(this.__ResStr("updFail", "Can't update site definition - it may have already been removed", site.OriginalSiteDomain));
            } else {
                if (!DataProvider.Add(site))
                    throw new Error(this.__ResStr("siteExists", "Can't add new site \"{0}\" - site already exists", site.SiteDomain));
                site.OriginalSiteDomain = site.SiteDomain;
            }
            // update appsettings.json
            if (string.Compare(YetaWFManager.DefaultSiteName, site.OriginalSiteDomain, true) == 0 && site.SiteDomain != site.OriginalSiteDomain) {
                WebConfigHelper.SetValue<string>(YetaWF.Core.Controllers.AreaRegistration.CurrentPackage.AreaName, "DEFAULTSITE", site.SiteDomain);
                WebConfigHelper.Save();
                restartRequired = true;
            }
            // restart required for uihint changes because uihints are cached or CDN changes
            if (!restartRequired) {
                if (site.OriginalUseCDN != site.UseCDN || site.OriginalCDNUrl != site.CDNUrl || site.OriginalCDNUrlSecure != site.CDNUrlSecure ||
                        site.OriginalStaticDomain != site.StaticDomain)
                    restartRequired = true;
            }
            return restartRequired;
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

        internal void RemoveSiteDefinition() {
            SiteDefinition site = Manager.CurrentSite;
            if (site.IsDefaultSite)
                throw new Error(this.__ResStr("cantDeleteDefault", "The default site of a YetaWF instance cannot be removed"));

            LocalizationSupport localizationSupport = new LocalizationSupport();
            localizationSupport.SetUseLocalizationResources(false);// turn off use of localization resources - things are about to be removed

            // turn off logging - things are about to be removed
            YetaWF.Core.Log.Logging.TerminateLogging();

            // remove all saved data
            RemoveCache(site);
            Package.RemoveSiteData(Manager.SiteFolder);
            DataProvider.Remove(site.SiteDomain);// remove domain
            Manager.RestartSite();// everything is now invalid anyway
        }

        /// <summary>
        /// Retrieve sites
        /// </summary>
        internal List<SiteDefinition> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            return DataProvider.GetRecords(skip, take, sort, filters, out total);
        }
        /// <summary>
        /// Retrieve sites
        /// </summary>
        internal SiteDefinition.SitesInfo GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            SiteDefinition.SitesInfo info = new SiteDefinition.SitesInfo();
            int total;
            info.Sites = GetItems(skip, take, sort, filters, out total);
            info.Total = total;
            return info;
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public new bool InstallModel(List<string> errorList) {
            if (!DataProvider.InstallModel(errorList))
                return false;
            try {
                SiteDefinition siteDef = new SiteDefinition();
                if (SiteDefinition.INITIAL_INSTALL) {
                    siteDef.SiteDomain = YetaWFManager.DefaultSiteName;
                    if (!DataProvider.Add(siteDef))
                        throw new InternalError("Couldn't add default site");
                    Manager.CurrentSite = siteDef;
                    siteDef.OriginalSiteDomain = siteDef.SiteDomain;
                }
                return true;
            } catch (Exception exc) {
                errorList.Add(string.Format("{0}: {1}", typeof(SiteDefinition).FullName, exc.Message));
                return false;
            }
        }
    }
}