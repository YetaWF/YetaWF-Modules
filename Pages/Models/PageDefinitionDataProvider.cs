/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Support.StaticPages;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Pages.DataProvider
{
    public class PageDefinitionDataProviderStartup : IInitializeApplicationStartup
    {
        public void InitializeApplicationStartup() {
            PageDefinition.LoadPageDefinition = LoadPageDefinition;
            PageDefinition.LoadPageDefinitionByUrl = LoadPageDefinition;
            PageDefinition.CreatePageDefinition = CreatePageDefinition;
            PageDefinition.SavePageDefinition = SavePageDefinition;
            PageDefinition.RemovePageDefinition = RemovePageDefinition;
            PageDefinition.GetDesignedPages = GetDesignedPages;
            PageDefinition.GetDesignedGuids = GetDesignedGuids;
            PageDefinition.GetDesignedUrls = GetDesignedUrls;
            PageDefinition.GetPagesFromModule = GetPagesFromModule;
        }

        private List<PageDefinition.DesignedPage> GetDesignedPages() {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                return pageDP.DesignedPages;
            }
        }

        private List<string> GetDesignedUrls() {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                return pageDP.DesignedUrls;
            }
        }
        private List<Guid> GetDesignedGuids() {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                return pageDP.DesignedGuids;
            }
        }
        private bool RemovePageDefinition(Guid pageGuid) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                return pageDP.RemovePageDefinition(pageGuid);
            }
        }
        private void SavePageDefinition(PageDefinition page) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                pageDP.SavePageDefinition(page);
            }
        }
        private PageDefinition CreatePageDefinition(string url) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                return pageDP.CreatePageDefinition(url);
            }
        }
        private PageDefinition LoadPageDefinition(Guid key) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                return pageDP.LoadPageDefinition(key);
            }
        }
        private PageDefinition LoadPageDefinition(string url) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                return pageDP.LoadPageDefinition(url);
            }
        }
        private List<PageDefinition> GetPagesFromModule(Guid moduleGuid) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                return pageDP.GetPagesFromModule(moduleGuid);
            }
        }
    }

    // For performance reasons, all page urls are preloaded to make lookup faster - this could be changed for SQL

    public class PageDefinitionDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        private static object _lockObject = new object();

        public PageDefinitionDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(DataProvider); }
        public PageDefinitionDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(DataProvider); }

        private IDataProvider<Guid, PageDefinition> DataProvider {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName)) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            _dataProvider = new FileDataProvider<Guid, PageDefinition>(
                                Path.Combine(YetaWFManager.DataFolder, AreaName, SiteIdentity.ToString()),
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new SQLSimpleObjectDataProvider<Guid, PageDefinition>(AreaName, SQLDbo, SQLConn,
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<Guid, PageDefinition> _dataProvider { get; set; }

        // LOAD/SAVE PAGE
        // LOAD/SAVE PAGE
        // LOAD/SAVE PAGE

        public PageDefinition CreatePageDefinition(string url) {
            lock (_lockObject) {
                PageDefinition page = new PageDefinition();
                page.AllowedRoles.Add(
                    new PageDefinition.AllowedRole { RoleId = Resource.ResourceAccess.GetAdministratorRoleId(), View = PageDefinition.AllowedEnum.Yes, Edit = PageDefinition.AllowedEnum.Yes, Remove = PageDefinition.AllowedEnum.Yes, }
                );
                page.Url = url.Trim();
                page.Temporary = false;
                return CreatePageDefinition(page);
            }
        }
        private PageDefinition CreatePageDefinition(PageDefinition page) {
            lock (_lockObject) {
                DesignedPage desPage = new DesignedPage() { PageGuid = page.PageGuid, Url = page.Url, };
                if (DesignedPagesByUrl.ContainsKey(desPage.Url.ToLower()))
                    throw new Error(this.__ResStr("pageUrlErr", "A page with Url {0} already exists.", desPage.Url));
                if (!DataProvider.Add(page))
                    throw new Error(this.__ResStr("pageGuidErr", "A page with Guid {0} already exists.", desPage.PageGuid));
                DesignedPagesByUrl.Add(page.Url.ToLower(), desPage);
                return page;
            }
        }
        public PageDefinition LoadPageDefinition(Guid key) {
            PageDefinition page = DataProvider.Get(key);
            if (page == null)
                return null;
            page.Temporary = false;
            return page;
        }
        public PageDefinition LoadPageDefinition(string url) {
            Guid guid = GetGuidFromUrl(url);
            if (guid == Guid.Empty)
                guid = FindPage(url);
            if (guid == Guid.Empty)
                return null;
            return LoadPageDefinition(guid);
        }
        private Guid GetGuidFromUrl(string url) {
            Guid pageGuid = Guid.Empty;
            url = url.Trim().ToLower();
            if (url.StartsWith(Globals.PageUrl.ToLower())) {
                url = url.Substring(Globals.PageUrl.Length);
                if (!Guid.TryParse(url, out pageGuid))
                    return Guid.Empty;
            }
            return pageGuid;
        }
        public void SavePageDefinition(PageDefinition page) {
            if (page.Temporary)
                throw new InternalError("Page {0} is a temporary page and can't be saved", page.PageGuid);

            page.Updated = DateTime.UtcNow;
            CleanupUsersAndRoles(page);
            SaveImages(page.PageGuid, page);

            lock (_lockObject) {

                PageDefinition oldPage = LoadPageDefinition(page.PageGuid);
                if (oldPage == null)
                    throw new Error(this.__ResStr("pageDeleted", "Page '{0}' has been deleted and can no longer be updated."), page.Url);

                Guid newPage = FindPage(page.Url);
                if (newPage != Guid.Empty && newPage != page.PageGuid)
                    throw new Error(this.__ResStr("pageRename", "A page with Url '{0}' already exists."), page.Url);

                UpdateStatusEnum status = DataProvider.Update(page.PageGuid, page.PageGuid, page);
                switch (status) {
                    case UpdateStatusEnum.OK:
                        break;
                    case UpdateStatusEnum.NewKeyExists:
                        throw new InternalError("Unexpected UpdateStatusEnum.NewKeyExists in SavePageDefinition");
                    case UpdateStatusEnum.RecordDeleted:
                        throw new InternalError("Unexpected UpdateStatusEnum.RecordDeleted in SavePageDefinition");
                }

                Manager.StaticPageManager.RemovePage(page.Url);

                if (newPage == Guid.Empty) {
                    DesignedPagesByUrl.Remove(oldPage.Url.ToLower());

                    DesignedPage desPage = new DesignedPage() { PageGuid = page.PageGuid, Url = page.Url, };
                    DesignedPagesByUrl.Add(page.Url.ToLower(), desPage);
                }
            }
        }
        private void CleanupUsersAndRoles(PageDefinition page) {
            // remove superuser from allowed roles as he/she's in there by default
            if (page.AllowedRoles == null) page.AllowedRoles = new SerializableList<PageDefinition.AllowedRole>();
            int superuserRole = Resource.ResourceAccess.GetSuperuserRoleId();
            page.AllowedRoles = new SerializableList<PageDefinition.AllowedRole>((from r in page.AllowedRoles where r.RoleId != superuserRole && !r.IsEmpty() select r).ToList());
            // remove superuser from allowed users as he/she's allowed by default
            if (page.AllowedUsers == null) page.AllowedUsers = new SerializableList<PageDefinition.AllowedUser>();
            int superuserId =  Resource.ResourceAccess.GetSuperuserId();
            page.AllowedUsers = new SerializableList<PageDefinition.AllowedUser>((from u in page.AllowedUsers where u.UserId != superuserId && !u.IsEmpty() select u).ToList());
        }
        public bool RemovePageDefinition(Guid pageGuid) {
            lock (_lockObject) {
                PageDefinition page = LoadPageDefinition(pageGuid);
                if (page == null)
                    return false;
                Manager.StaticPageManager.RemovePage(page.Url);
                DataProvider.Remove(pageGuid);
                DesignedPagesByUrl.Remove(page.Url.ToLower());
                return true;
            }
        }

        public List<PageDefinition> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            return DataProvider.GetRecords(skip, take, sort, filters, out total);
        }

        // DESIGNED PAGES
        // DESIGNED PAGES
        // DESIGNED PAGES

        // Designed pages are site specific and DesignedPagesByUrl is a permanent site-specific object

        public class DesignedPage {
            public string Url { get; set; } // absolute Url (w/o http: or domain) e.g., /Home or /Test/Page123
            [Data_PrimaryKey]
            public Guid PageGuid { get; set; }
        }
        protected class DesignedPagesDictionaryByUrl : SerializableDictionary<string, DesignedPage> { }

        public List<PageDefinition.DesignedPage> DesignedPages {
            get {
                return (from p in DesignedPagesByUrl select new PageDefinition.DesignedPage { Url = p.Value.Url, PageGuid = p.Value.PageGuid }).ToList();
            }
        }
        public List<Guid> DesignedGuids {
            get {
                return (from p in DesignedPagesByUrl select p.Value.PageGuid).ToList();
            }
        }
        public List<string> DesignedUrls {
            get {
                return (from p in DesignedPagesByUrl select p.Value.Url).ToList();
            }
        }

        protected DesignedPagesDictionaryByUrl DesignedPagesByUrl {
            get {
                DesignedPagesDictionaryByUrl byUrl;
                GetDesignedPages(out byUrl);
                return byUrl;
            }
        }
        protected void GetDesignedPages(out DesignedPagesDictionaryByUrl designedPagesByUrl) {
            DesignedPagesDictionaryByUrl byUrl;

            if (PermanentManager.TryGetObject<DesignedPagesDictionaryByUrl>(out byUrl)) {
                designedPagesByUrl = byUrl;
                return;
            }

            lock (_lockObject) { // lock this so we only do this once
                // See if we already have it as a permanent object
                if (PermanentManager.TryGetObject<DesignedPagesDictionaryByUrl>(out byUrl)) {
                    designedPagesByUrl = byUrl;
                    return;
                }
                // Load the designed pages
                if (!DataProvider.IsInstalled()) {
                    designedPagesByUrl = new DesignedPagesDictionaryByUrl();// don't save this, it's not permanent
                    return;
                }
                switch (IOMode) {
                    default:
                        throw new InternalError("IOMode undetermined");
                    case WebConfigHelper.IOModeEnum.File:
                        GetDesignedPages_File(out byUrl);
                        break;
                    case WebConfigHelper.IOModeEnum.Sql:
                        GetDesignedPages_Sql(out byUrl);
                        break;
                }
                PermanentManager.AddObject<DesignedPagesDictionaryByUrl>(byUrl);
            }
            designedPagesByUrl = byUrl;
        }

        private void GetDesignedPages_Sql(out DesignedPagesDictionaryByUrl designedPagesByUrl) {
            using (SQLSimpleObjectDataProvider<Guid, DesignedPage> dp = new SQLSimpleObjectDataProvider<Guid, DesignedPage>(AreaName, SQLDbo, SQLConn, CurrentSiteIdentity: SiteIdentity)) {
                IDataProvider<Guid, DesignedPage> dataProvider = dp;
                int total;
                List<DesignedPage> pages = dataProvider.GetRecords(0, 0, null, null, out total);

                DesignedPagesDictionaryByUrl byUrl = new DesignedPagesDictionaryByUrl();
                foreach (DesignedPage page in pages) {
                    byUrl.Add(page.Url.ToLower(), page);
                }
                designedPagesByUrl = byUrl;
            }
        }

        private void GetDesignedPages_File(out DesignedPagesDictionaryByUrl designedPagesByUrl) {
            DesignedPagesDictionaryByUrl byUrl = new DesignedPagesDictionaryByUrl();
            List<Guid> pageGuids = DataProvider.GetKeyList();
            foreach (var pageGuid in pageGuids) {
                PageDefinition page = DataProvider.Get(pageGuid);
                if (page == null)
                    throw new InternalError("No PageDefinition for guid {0}", pageGuid);
                DesignedPage desPage = new DesignedPage() { Url = page.Url, PageGuid = page.PageGuid };
                byUrl.Add(page.Url.ToLower(), desPage);
            }
            designedPagesByUrl = byUrl;
        }

        /// <summary>
        /// Given a url returns a designed page guid.
        /// Searching page page id is slow - use Url instead
        /// </summary>
        protected Guid FindPage(string url) {
            DesignedPage designedPage;
            if (DesignedPagesByUrl.TryGetValue(url.ToLower(), out designedPage))
                return designedPage.PageGuid;
            return Guid.Empty;
        }

        // MODULE ON WHICH PAGE(S) QUERY
        // MODULE ON WHICH PAGE(S) QUERY
        // MODULE ON WHICH PAGE(S) QUERY

        public List<PageDefinition> GetPagesFromModule(Guid moduleGuid) {
            DataProvider.IsInstalled();// to initialize
            switch (IOMode) {
                default:
                    throw new InternalError("IOMode undetermined");
                case WebConfigHelper.IOModeEnum.File:
                    return PagesFromModule_File(moduleGuid);
                case WebConfigHelper.IOModeEnum.Sql:
                    return PagesFromModule_Sql(moduleGuid);
            }
        }

        private List<PageDefinition> PagesFromModule_File(Guid moduleGuid) {
            int total;
            List<PageDefinition> pages = GetItems(0, 0, null, null, out total);
            List<PageDefinition> pagesWithModule = new List<PageDefinition>();
            foreach (PageDefinition page in pages) {
                PageDefinition p = (from m in page.ModuleDefinitions where m.ModuleGuid == moduleGuid select page).FirstOrDefault();
                if (p != null)
                    pagesWithModule.Add(p);
            }
            return pagesWithModule.OrderBy(p => p.Url).ToList();
        }

        private List<PageDefinition> PagesFromModule_Sql(Guid moduleGuid) {
            int total;
            List<DataProviderSortInfo> sorts = DataProviderSortInfo.Join(null, new DataProviderSortInfo { Field = "Url", Order = DataProviderSortInfo.SortDirection.Ascending });
            List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "ModuleGuid", Operator = "==", Value = moduleGuid });
            using (PageDefinitionDataProvider pageDefDP = new PageDefinitionDataProvider()) {
                using (PageDefinitionForModulesProvider pageDefForModDP = new PageDefinitionForModulesProvider()) {
                    List<JoinData> joins = new List<JoinData> {
                        new JoinData {MainDP = pageDefDP, JoinDP = pageDefForModDP, MainColumn = "Identity", JoinColumn = "__Key", UseSite = false },
                    };
                    return DataProvider.GetRecords(0, 0, sorts, filters, out total, Joins: joins);
                }
            }
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public bool IsInstalled() {
            return DataProvider.IsInstalled();
        }
        public bool InstallModel(List<string> errorList) {
            PermanentManager.RemoveObject<DesignedPagesDictionaryByUrl>();
            return DataProvider.InstallModel(errorList);
        }
        public bool UninstallModel(List<string> errorList) {
            bool status = DataProvider.UninstallModel(errorList);
            PermanentManager.RemoveObject<DesignedPagesDictionaryByUrl>();
            return status;
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
