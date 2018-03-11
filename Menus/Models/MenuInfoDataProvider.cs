/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Menus.DataProvider {
    public class MenuInfo {

        [Data_PrimaryKey]
        public Guid ModuleGuid { get; set; }

        [Data_Binary, CopyAttribute]
        public MenuList Menu { get; set; }

        public MenuInfo() { }
    }

    public class MenuInfoDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public MenuInfoDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public MenuInfoDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<Guid, MenuInfo> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<Guid, MenuInfo> CreateDataProvider() {
            Package package = YetaWF.Modules.Menus.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_MenuInfo", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public async Task DoActionAsync(Guid moduleGuid, Func<Task> action) {
            await StringLocks.DoActionAsync(LockKey(moduleGuid), async () => {
                await action();
            });
        }
        private string LockKey(Guid moduleGuid) {
            return string.Format("{0}_{1}", this.Dataset, moduleGuid);
        }
        public async Task<MenuInfo> GetItemAsync(Guid moduleGuid) {
            return await DataProvider.GetAsync(moduleGuid);
        }
        public async Task ReplaceItemAsync(MenuInfo data) {
            await DoActionAsync(data.ModuleGuid, async () => {
                await RemoveItemAsync(data.ModuleGuid);
                await AddItemAsync(data);
            });
        }
        protected async Task<bool> AddItemAsync(MenuInfo data) {
            return await DataProvider.AddAsync(data);
        }
        protected async Task<UpdateStatusEnum> UpdateItemAsync(MenuInfo data) {
            return await DataProvider.UpdateAsync(data.ModuleGuid, data.ModuleGuid, data);
        }
        public async Task<bool> RemoveItemAsync(Guid moduleGuid) {
            return await DataProvider.RemoveAsync(moduleGuid);
        }
    }
}
