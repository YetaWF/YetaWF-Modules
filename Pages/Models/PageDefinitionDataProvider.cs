/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Pages.DataProvider {

    public class PageDefinitionDataProviderStartup : IInitializeApplicationStartup {

        public void InitializeApplicationStartup() {
            PageDefinition.LoadPageDefinitionAsync = LoadPageDefinitionAsync;
            PageDefinition.LoadPageDefinitionByUrlAsync = LoadPageDefinitionAsync;
            PageDefinition.CreatePageDefinitionAsync = CreatePageDefinitionAsync;
            PageDefinition.SavePageDefinitionAsync = SavePageDefinitionAsync;
            PageDefinition.RemovePageDefinitionAsync = RemovePageDefinitionAsync;
            PageDefinition.GetDesignedPages = GetDesignedPages;
            PageDefinition.GetDesignedGuids = GetDesignedGuids;
            PageDefinition.GetDesignedUrls = GetDesignedUrls;
            PageDefinition.GetPagesFromModuleAsync = GetPagesFromModuleAsync;
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
        private async Task<bool> RemovePageDefinitionAsync(Guid pageGuid) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                return await pageDP.RemovePageDefinitionAsync(pageGuid);
            }
        }
        private async Task SavePageDefinitionAsync(PageDefinition page) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                await pageDP.SavePageDefinitionAsync(page);
            }
        }
        private async Task<PageDefinition> CreatePageDefinitionAsync(string url) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                return await pageDP.CreatePageDefinitionAsync(url);
            }
        }
        private async Task<PageDefinition> LoadPageDefinitionAsync(Guid key) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                return await pageDP.LoadPageDefinitionAsync(key);
            }
        }
        private async Task<PageDefinition> LoadPageDefinitionAsync(string url) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                return await pageDP.LoadPageDefinitionAsync(url);
            }
        }
        private async Task<List<PageDefinition>> GetPagesFromModuleAsync(Guid moduleGuid) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                return await pageDP.GetPagesFromModuleAsync(moduleGuid);
            }
        }
    }

    // For performance reasons, all page urls are preloaded to make lookup faster - this could be changed for SQL

    public class DesignedPagesDictionaryByUrl : SerializableDictionary<string, PageDefinition.DesignedPage> { }

    interface IPageDefinitionIOMode {
        DesignedPagesDictionaryByUrl GetDesignedPages();
        Task<List<PageDefinition>> GetPagesFromModuleAsync(Guid moduleGuid);
    }
    
    public class PageDefinitionDataProvider : DataProviderImpl, IInstallableModel {

        private static AsyncLock _lockObject = new AsyncLock();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public PageDefinitionDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public PageDefinitionDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<Guid, PageDefinition> DataProvider { get { return GetDataProvider(); } }
        private IPageDefinitionIOMode DataProviderIOMode { get { return GetDataProvider(); } }

        private IDataProvider<Guid, PageDefinition> CreateDataProvider() {
            Package package = YetaWF.Modules.Pages.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName, SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API PAGE
        // API PAGE
        // API PAGE

        public async Task<PageDefinition> CreatePageDefinitionAsync(string url) {
            using (await _lockObject.LockAsync()) {
                PageDefinition page = new PageDefinition();
                page.AllowedRoles.Add(
                    new PageDefinition.AllowedRole { RoleId = Resource.ResourceAccess.GetAdministratorRoleId(), View = PageDefinition.AllowedEnum.Yes, Edit = PageDefinition.AllowedEnum.Yes, Remove = PageDefinition.AllowedEnum.Yes, }
                );
                page.Url = url.Trim();
                page.Temporary = false;
                return await CreatePageDefinitionAsync(page);
            }
        }
        private async Task<PageDefinition> CreatePageDefinitionAsync(PageDefinition page) {
            using (await _lockObject.LockAsync()) {
                PageDefinition.DesignedPage desPage = new PageDefinition.DesignedPage() { PageGuid = page.PageGuid, Url = page.Url, };
                if (DesignedPagesByUrl.ContainsKey(desPage.Url.ToLower()))
                    throw new Error(this.__ResStr("pageUrlErr", "A page with Url {0} already exists.", desPage.Url));
                if (!await DataProvider.AddAsync(page))
                    throw new Error(this.__ResStr("pageGuidErr", "A page with Guid {0} already exists.", desPage.PageGuid));
                DesignedPagesByUrl.Add(page.Url.ToLower(), desPage);
                return page;
            }
        }
        public async Task<PageDefinition> LoadPageDefinitionAsync(Guid key) {
            PageDefinition page = await DataProvider.GetAsync(key);
            if (page == null)
                return null;
            page.Temporary = false;
            return page;
        }
        public async Task<PageDefinition> LoadPageDefinitionAsync(string url) {
            Guid guid = GetGuidFromUrl(url);
            if (guid == Guid.Empty)
                guid = FindPage(url);
            if (guid == Guid.Empty)
                return null;
            return await LoadPageDefinitionAsync(guid);
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
        public async Task SavePageDefinitionAsync(PageDefinition page) {
            if (page.Temporary)
                throw new InternalError("Page {0} is a temporary page and can't be saved", page.PageGuid);

            page.Updated = DateTime.UtcNow;
            CleanupUsersAndRoles(page);
            SaveImages(page.PageGuid, page);

            using (await _lockObject.LockAsync()) {

                PageDefinition oldPage = await LoadPageDefinitionAsync(page.PageGuid);
                if (oldPage == null)
                    throw new Error(this.__ResStr("pageDeleted", "Page '{0}' has been deleted and can no longer be updated."), page.Url);

                Guid newPage = FindPage(page.Url);
                if (newPage != Guid.Empty && newPage != page.PageGuid)
                    throw new Error(this.__ResStr("pageRename", "A page with Url '{0}' already exists."), page.Url);

                UpdateStatusEnum status = await DataProvider.UpdateAsync(page.PageGuid, page.PageGuid, page);
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

                    PageDefinition.DesignedPage desPage = new PageDefinition.DesignedPage() { PageGuid = page.PageGuid, Url = page.Url, };
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
        public async Task<bool> RemovePageDefinitionAsync(Guid pageGuid) {
            using (await _lockObject.LockAsync()) {
                PageDefinition page = await LoadPageDefinitionAsync(pageGuid);
                if (page == null)
                    return false;
                Manager.StaticPageManager.RemovePage(page.Url);
                await DataProvider.RemoveAsync(pageGuid);
                DesignedPagesByUrl.Remove(page.Url.ToLower());
                return true;
            }
        }
        public async Task<DataProviderGetRecords<PageDefinition>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            DataProviderGetRecords<PageDefinition> list = await DataProvider.GetRecordsAsync(skip, take, sort, filters);
            foreach (PageDefinition page in list.Data)
                page.Temporary = false;
            return list;
        }

        // DESIGNED PAGES
        // DESIGNED PAGES
        // DESIGNED PAGES

        // Designed pages are site specific and DesignedPagesByUrl is a permanent site-specific object

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
                return GetDesignedPages();
            }
        }
        private DesignedPagesDictionaryByUrl GetDesignedPages() {
            DesignedPagesDictionaryByUrl byUrl;
            if (PermanentManager.TryGetObject<DesignedPagesDictionaryByUrl>(out byUrl))
                return byUrl;

            using (_lockObject.Lock()) {
                // See if we already have it as a permanent object
                if (PermanentManager.TryGetObject<DesignedPagesDictionaryByUrl>(out byUrl))
                    return byUrl;
                // Load the designed pages
                if (!DataProvider.IsInstalledAsync().Result)
                    return new DesignedPagesDictionaryByUrl();// don't save this, it's not permanent
                byUrl = DataProviderIOMode.GetDesignedPages();
                PermanentManager.AddObject<DesignedPagesDictionaryByUrl>(byUrl);
            }
            return byUrl;
        }

        /// <summary>
        /// Given a url returns a designed page guid.
        /// Searching page by guid is slow - use Url instead
        /// </summary>
        protected Guid FindPage(string url) {
            PageDefinition.DesignedPage designedPage;
            if (DesignedPagesByUrl.TryGetValue(url.ToLower(), out designedPage))
                return designedPage.PageGuid;
            return Guid.Empty;
        }

        // MODULE ON WHICH PAGE(S) QUERY
        // MODULE ON WHICH PAGE(S) QUERY
        // MODULE ON WHICH PAGE(S) QUERY

        public async Task<List<PageDefinition>> GetPagesFromModuleAsync(Guid moduleGuid) {
            return await DataProviderIOMode.GetPagesFromModuleAsync(moduleGuid);
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public new async Task<bool> InstallModelAsync(List<string> errorList) {
            PermanentManager.RemoveObject<DesignedPagesDictionaryByUrl>();
            return await DataProvider.InstallModelAsync(errorList);
        }
        public new async Task<bool> UninstallModelAsync(List<string> errorList) {
            bool status = await DataProvider.UninstallModelAsync(errorList);
            PermanentManager.RemoveObject<DesignedPagesDictionaryByUrl>();
            return status;
        }
    }
}
