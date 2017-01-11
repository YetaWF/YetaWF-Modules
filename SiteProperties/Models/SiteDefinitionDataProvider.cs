/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/SiteProperties#License */

using System;
using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Security;
using YetaWF.Core.Serializers;
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
        }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public const string LOCKEDURL = "/Maintenance/Offline For Maintenance.html";

        static SiteDefinitionDataProvider() {
            SiteCache = new Dictionary<string, SiteDefinition>();
        }
        public SiteDefinitionDataProvider() : base(0) { SetDataProvider(DataProvider); }

        // SQL, File

        private IDataProvider<String, SiteDefinition> DataProvider
        {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName)) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            _dataProvider = new YetaWF.DataProvider.FileDataProvider<String, SiteDefinition>(
                                Path.Combine(YetaWFManager.DataFolder, AreaName),
                                IdentitySeed: SiteDefinition.SiteIdentitySeed);
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLSimpleObjectDataProvider<String, SiteDefinition>(AreaName, SQLDbo, SQLConn,
                                NoLanguages: true,
                                IdentitySeed: SiteDefinition.SiteIdentitySeed,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<String, SiteDefinition> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        static private Dictionary<string, SiteDefinition> SiteCache { get; set; }

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
                site.OriginalCDNSiteFiles = site.CDNSiteFiles;
                site.OriginalCDNVault = site.CDNVault;
                site.OriginalCDNContent = site.CDNContent;
                site.OriginalCDNScripts = site.CDNScripts;
                site.OriginalCDNAddons = site.CDNAddons;
                site.OriginalCDNAddonsCustom = site.CDNAddonsCustom;
                site.OriginalCDNAddonsBundles = site.CDNAddonsBundles;
                site.OriginalCDNFileImage = site.CDNFileImage;
                AddCache(site);
                AddLockedStatus(site);
                return site;
            }
            return null;
        }
        public SiteDefinition _defaultSite = null;

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
            // restart required for uihint changes because uihints are cached or CDN changes
            if (site.OriginalUseCDN != site.UseCDN || site.OriginalCDNUrl != site.CDNUrl || site.OriginalCDNUrlSecure != site.CDNUrlSecure ||
                    site.OriginalCDNSiteFiles != site.CDNSiteFiles || site.OriginalCDNVault != site.CDNVault || site.OriginalCDNContent != site.CDNContent ||
                    site.OriginalCDNScripts != site.CDNScripts || site.OriginalCDNAddons != site.CDNAddons || site.OriginalCDNAddonsCustom != site.CDNAddonsCustom ||  site.OriginalCDNAddonsBundles != site.CDNAddonsBundles ||
                    site.OriginalCDNFileImage != site.CDNFileImage)
                restartRequired = true;
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

        public bool IsInstalled() {
            return DataProvider.IsInstalled();
        }
        public bool InstallModel(List<string> errorList) {

            if (!DataProvider.InstallModel(errorList))
                return false;
            try {
                SiteDefinition siteDef = new SiteDefinition();
                if (SiteDefinition.INITIAL_INSTALL) {
                    if (!Manager.IsLocalHost)
                        siteDef.SiteDomain = Manager.HostUsed;
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
        public bool UninstallModel(List<string> errorList) {
            return DataProvider.UninstallModel(errorList);
        }
        public void AddSiteData() {
            DataProvider.AddSiteData();
        }
        public void RemoveSiteData() {
            DataProvider.RemoveSiteData();
        }
        public bool ExportChunk(int chunk, SerializableList<SerializableFile> fileList, out object obj) {
            return DataProvider.ExportChunk(chunk, fileList, out obj);
        }
        public void ImportChunk(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            DataProvider.ImportChunk(chunk, fileList, obj);
        }
    }
}