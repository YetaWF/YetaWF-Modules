/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Pages#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Pages.DataProvider {

    public class PageDefinitionDataProviderStartup : IInitializeApplicationStartup {

        public Task InitializeApplicationStartupAsync() {
            PageDefinition.LoadPageDefinitionAsync = LoadPageDefinitionAsync;
            PageDefinition.LoadPageDefinitionByUrlAsync = LoadPageDefinitionAsync;
            PageDefinition.CreatePageDefinitionAsync = CreatePageDefinitionAsync;
            PageDefinition.SavePageDefinitionAsync = SavePageDefinitionAsync;
            PageDefinition.RemovePageDefinitionAsync = RemovePageDefinitionAsync;
            PageDefinition.GetDesignedPagesAsync = GetDesignedPagesAsync;
            PageDefinition.GetDesignedGuidsAsync = GetDesignedGuidsAsync;
            PageDefinition.GetDesignedUrlsAsync = GetDesignedUrlsAsync;
            PageDefinition.GetPagesFromModuleAsync = GetPagesFromModuleAsync;
            return Task.CompletedTask;
        }

        private async Task<List<PageDefinition.DesignedPage>> GetDesignedPagesAsync() {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                using (ILockObject lockObject = await pageDP.LockDesignedPagesAsync()) {
                    List<PageDefinition.DesignedPage> pages = (from p in await pageDP.GetDesignedPagesWithoutLockAsync() select new PageDefinition.DesignedPage { Url = p.Value.Url, PageGuid = p.Value.PageGuid }).ToList();
                    await lockObject.UnlockAsync();
                    return pages;
                }
            }
        }

        private async Task<List<string>> GetDesignedUrlsAsync() {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                using (ILockObject lockObject = await pageDP.LockDesignedPagesAsync()) {
                    List<string> pages = (from p in await pageDP.GetDesignedPagesWithoutLockAsync() select p.Value.Url).ToList();
                    await lockObject.UnlockAsync();
                    return pages;
                }
            }
        }
        private async Task<List<Guid>> GetDesignedGuidsAsync() {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                using (ILockObject lockObject = await pageDP.LockDesignedPagesAsync()) {
                    List<Guid> pages = (from p in await pageDP.GetDesignedPagesWithoutLockAsync() select p.Value.PageGuid).ToList();
                    await lockObject.UnlockAsync();
                    return pages;
                }
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
        private async Task<PageDefinition?> LoadPageDefinitionAsync(Guid key) {
            using (PageDefinitionDataProvider pageDP = new PageDefinitionDataProvider()) {
                return await pageDP.LoadPageDefinitionAsync(key);
            }
        }
        private async Task<PageDefinition?> LoadPageDefinitionAsync(string url) {
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
        Task<DesignedPagesDictionaryByUrl> GetDesignedPagesAsync();
        Task<List<PageDefinition>> GetPagesFromModuleAsync(Guid moduleGuid);
    }

    public class PageDefinitionDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public PageDefinitionDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public PageDefinitionDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<Guid, PageDefinition> DataProvider { get { return GetDataProvider(); } }
        private IPageDefinitionIOMode DataProviderIOMode { get { return GetDataProvider(); } }

        private IDataProvider<Guid, PageDefinition>? CreateDataProvider() {
            Package package = YetaWF.Modules.Pages.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName, SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // CACHE
        // CACHE
        // CACHE

        private string CacheKey(Guid guid) {
            return string.Format("__Page_{0}_{1}", YetaWFManager.Manager.CurrentSite.Identity, guid);
        }
        private class GetCachedPageInfo {
            public PageDefinition? Page { get; set; }
            public bool Success { get; set; }
        }
        private async Task<GetCachedPageInfo> GetCachedPageAsync(Guid guid) {
            GetCachedPageInfo pageInfo = new GetCachedPageInfo();
            GetObjectInfo<PageDefinition> objInfo;
            using (ICacheDataProvider sharedCacheDP = YetaWF.Core.IO.Caching.GetSharedCacheProvider()) {
                objInfo = await sharedCacheDP.GetAsync<PageDefinition>(CacheKey(guid));
            }
            if (!objInfo.Success)
                return pageInfo;

            if (objInfo.Data != null) {
                // make a type correct copy of the data, we don't want caller to modify the cached page
                Type objType = objInfo.Data.GetType();
                PageDefinition newObj = (PageDefinition)Activator.CreateInstance(objType)!;
                ObjectSupport.CopyData(objInfo.Data, newObj);
                pageInfo.Page = newObj;
            }
            pageInfo.Success = true;
            return pageInfo;
        }
        private async Task SetCachedPageAsync(PageDefinition page) {
            using (ICacheDataProvider sharedCacheDP = YetaWF.Core.IO.Caching.GetSharedCacheProvider()) {
                await sharedCacheDP.AddAsync(CacheKey(page.PageGuid), page);
            }
        }
        private async Task SetEmptyCachedPageAsync(Guid guid) {
            using (ICacheDataProvider sharedCacheDP = YetaWF.Core.IO.Caching.GetSharedCacheProvider()) {
                await sharedCacheDP.AddAsync<PageDefinition>(CacheKey(guid), null);
            }
        }
        private async Task RemoveCachedPageAsync(Guid guid) {
            using (ICacheDataProvider sharedCacheDP = YetaWF.Core.IO.Caching.GetSharedCacheProvider()) {
                await sharedCacheDP.RemoveAsync<PageDefinition>(CacheKey(guid));
            }
        }

        // API PAGE
        // API PAGE
        // API PAGE

        public async Task<PageDefinition> CreatePageDefinitionAsync(string url) {
            PageDefinition page = new PageDefinition();
            page.AllowedRoles.Add(
                new PageDefinition.AllowedRole { RoleId = Resource.ResourceAccess.GetAdministratorRoleId(), View = PageDefinition.AllowedEnum.Yes, Edit = PageDefinition.AllowedEnum.Yes, Remove = PageDefinition.AllowedEnum.Yes, }
            );
            page.Url = url.Trim();
            page.Temporary = false;
            return await CreatePageDefinitionAsync(page);
        }
        private async Task<PageDefinition> CreatePageDefinitionAsync(PageDefinition page) {
            using (ILockObject lockObject = await LockDesignedPagesAsync()) {
                PageDefinition.DesignedPage desPage = new PageDefinition.DesignedPage() { PageGuid = page.PageGuid, Url = page.Url, };
                DesignedPagesDictionaryByUrl designedPagesByUrl = await GetDesignedPagesWithoutLockAsync();
                if (designedPagesByUrl.ContainsKey(desPage.Url.ToLower()))
                    throw new Error(this.__ResStr("pageUrlErr", "A page with Url {0} already exists.", desPage.Url));
                if (!await DataProvider.AddAsync(page))
                    throw new Error(this.__ResStr("pageGuidErr", "A page with Guid {0} already exists.", desPage.PageGuid));
                await SetCachedPageAsync(page);
                await AddDesignedPageAsync(designedPagesByUrl, page.Url.ToLower(), desPage);
                await lockObject.UnlockAsync();
            }
            await Auditing.AddAuditAsync($"{nameof(PageDefinitionDataProvider)}.{nameof(CreatePageDefinitionAsync)}", page.Url, page.PageGuid,
                "Create Page",
                DataBefore: null,
                DataAfter: page,
                ExpensiveMultiInstance: true
            );
            return page;
        }

        public async Task<PageDefinition?> LoadPageDefinitionAsync(Guid key) {
            GetCachedPageInfo pageInfo = await GetCachedPageAsync(key);
            PageDefinition? page;
            if (pageInfo.Success) {
                page = pageInfo.Page;
                if (page != null)
                    page.Temporary = false;
            } else {
                page = await DataProvider.GetAsync(key);
                if (page != null) {
                    page.Temporary = false;
                    await SetCachedPageAsync(page);
                } else
                    await SetEmptyCachedPageAsync(key);
            }
            return page;
        }
        public async Task<PageDefinition?> LoadPageDefinitionAsync(string url) {
            Guid guid = GetGuidFromUrl(url);
            if (guid == Guid.Empty)
                guid = await FindPageAsync(url);
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

            PageDefinition? oldPage = null;

            using (ILockObject lockObject = await LockDesignedPagesAsync()) {

                page.Updated = DateTime.UtcNow;
                CleanupUsersAndRoles(page);
                await SaveImagesAsync(page.PageGuid, page);

                oldPage = await LoadPageDefinitionAsync(page.PageGuid);
                if (oldPage == null)
                    throw new Error(this.__ResStr("pageDeleted", "Page '{0}' has been deleted and can no longer be updated."), page.Url);

                Guid newPage = await FindPageAsync(page.Url);
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
                await SetCachedPageAsync(page);
                await Manager.StaticPageManager.RemovePageAsync(page.Url);

                if (newPage == Guid.Empty) {
                    DesignedPagesDictionaryByUrl designedPagesByUrl = await GetDesignedPagesWithoutLockAsync();
                    designedPagesByUrl.Remove(oldPage.Url.ToLower());

                    PageDefinition.DesignedPage desPage = new PageDefinition.DesignedPage() { PageGuid = page.PageGuid, Url = page.Url, };
                    await AddDesignedPageAsync(designedPagesByUrl, page.Url.ToLower(), desPage);
                }
                await lockObject.UnlockAsync();
            }
            await Auditing.AddAuditAsync($"{nameof(PageDefinitionDataProvider)}.{nameof(SavePageDefinitionAsync)}", oldPage.Url, oldPage.PageGuid,
                "Save Page",
                DataBefore: oldPage,
                DataAfter: page,
                ExpensiveMultiInstance: true
            );
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
            PageDefinition? page;
            using (ILockObject lockObject = await LockDesignedPagesAsync()) {
                page = await LoadPageDefinitionAsync(pageGuid);
                if (page == null)
                    return false;
                await Manager.StaticPageManager.RemovePageAsync(page.Url);
                await DataProvider.RemoveAsync(pageGuid);
                await RemoveCachedPageAsync(pageGuid);
                DesignedPagesDictionaryByUrl designedPagesUrl = await GetDesignedPagesWithoutLockAsync();
                await RemoveDesignedPageAsync(designedPagesUrl, page.Url);
                await lockObject.UnlockAsync();
            }
            await Auditing.AddAuditAsync($"{nameof(PageDefinitionDataProvider)}.{nameof(RemovePageDefinitionAsync)}", page.Url, page.PageGuid,
                "Remove Page",
                DataBefore: page,
                DataAfter: null,
                ExpensiveMultiInstance: true
            );
            return true;
        }

        public async Task<DataProviderGetRecords<PageDefinition>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) {
            DataProviderGetRecords<PageDefinition> list = await DataProvider.GetRecordsAsync(skip, take, sort, filters);
            foreach (PageDefinition page in list.Data)
                page.Temporary = false;
            return list;
        }

        // DESIGNED PAGES
        // DESIGNED PAGES
        // DESIGNED PAGES

        // Designed pages are site specific and are stored as a static site-specific object

        private string DESIGNEDPAGESKEY = $"__DesignedPages__{YetaWFManager.Manager.CurrentSite.Identity}";

        internal async Task<DesignedPagesDictionaryByUrl> GetDesignedPagesWithoutLockAsync() {
            if (!await DataProvider.IsInstalledAsync())
                return new DesignedPagesDictionaryByUrl();// don't save this, it's not permanent
            using (ICacheDataProvider staticCacheDP = YetaWF.Core.IO.Caching.GetStaticCacheProvider()) {
                DesignedPagesDictionaryByUrl data;
                GetObjectInfo<DesignedPagesDictionaryByUrl> info = await staticCacheDP.GetAsync<DesignedPagesDictionaryByUrl>(DESIGNEDPAGESKEY);
                if (!info.Success) {
                    data = await DataProviderIOMode.GetDesignedPagesAsync();
                    await staticCacheDP.AddAsync(DESIGNEDPAGESKEY, data);
                } else
                    data = info.RequiredData;
                return data;
            }
        }
        // Requires an active static lock
        private async Task AddDesignedPageAsync(DesignedPagesDictionaryByUrl designedPagesByUrl, string url, PageDefinition.DesignedPage designedPage) {
            designedPagesByUrl.Add(url, designedPage);
            using (ICacheDataProvider staticCacheDP = YetaWF.Core.IO.Caching.GetStaticCacheProvider()) {
                await staticCacheDP.AddAsync(DESIGNEDPAGESKEY, designedPagesByUrl);
            }
        }
        // Requires an active static lock
        private async Task RemoveDesignedPageAsync(DesignedPagesDictionaryByUrl designedPagesByUrl, string url) {
            designedPagesByUrl.Remove(url.ToLower());
            using (ICacheDataProvider staticCacheDP = YetaWF.Core.IO.Caching.GetStaticCacheProvider()) {
                await staticCacheDP.AddAsync(DESIGNEDPAGESKEY, designedPagesByUrl);
            }
        }
        internal async Task<ILockObject> LockDesignedPagesAsync() {
            return await YetaWF.Core.IO.Caching.LockProvider.LockResourceAsync(DESIGNEDPAGESKEY);
        }
        /// <summary>
        /// Given a url returns a designed page guid.
        /// Searching page by guid is slow - use Url instead
        /// </summary>
        protected async Task<Guid> FindPageAsync(string url) {
            PageDefinition.DesignedPage? designedPage;
            DesignedPagesDictionaryByUrl designedPagesByUrl = await GetDesignedPagesWithoutLockAsync();
            if (designedPagesByUrl.TryGetValue(url.ToLower(), out designedPage))
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
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Installing new models is not possible when distributed caching is enabled");
            using (ICacheDataProvider staticCacheDP = YetaWF.Core.IO.Caching.GetStaticCacheProvider()) {
                await staticCacheDP.RemoveAsync<DesignedPagesDictionaryByUrl>(DESIGNEDPAGESKEY);
            }
            return await DataProvider.InstallModelAsync(errorList);
        }
        public new async Task<bool> UninstallModelAsync(List<string> errorList) {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Uninstalling models is not possible when distributed caching is enabled");
            bool status = await DataProvider.UninstallModelAsync(errorList);
            using (ICacheDataProvider staticCacheDP = YetaWF.Core.IO.Caching.GetStaticCacheProvider()) {
                await staticCacheDP.RemoveAsync<DesignedPagesDictionaryByUrl>(DESIGNEDPAGESKEY);
            }
            return status;
        }
    }
}
