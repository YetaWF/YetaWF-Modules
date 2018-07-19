/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using YetaWF.Core.Audit;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
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

        public async Task<MenuInfo> GetItemAsync(Guid moduleGuid) {
            return await DataProvider.GetAsync(moduleGuid);
        }
        public async Task ReplaceItemAsync(MenuInfo data) {
            using (ILockObject lockObject = await ModuleDefinition.LockModuleAsync(data.ModuleGuid)) {
                await RemoveItemAsync(data.ModuleGuid);
                await AddItemAsync(data);
                await lockObject.UnlockAsync();
            }
        }
        protected async Task<bool> AddItemAsync(MenuInfo data) {
            bool result = await DataProvider.AddAsync(data);
            await Auditing.AddAuditAsync($"{nameof(MenuInfoDataProvider)}.{nameof(AddItemAsync)}", "Menu", data.ModuleGuid,
                "Add Menu",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
            return result;
        }
        protected async Task<UpdateStatusEnum> UpdateItemAsync(MenuInfo data) {
            MenuInfo origMenu = Auditing.Active ? await GetItemAsync(data.ModuleGuid) : null;
            UpdateStatusEnum result = await DataProvider.UpdateAsync(data.ModuleGuid, data.ModuleGuid, data);
            await Auditing.AddAuditAsync($"{nameof(MenuInfoDataProvider)}.{nameof(UpdateItemAsync)}", "Menu", data.ModuleGuid,
                "Update Menu",
                DataBefore: origMenu,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
            return result;
        }
        public async Task<bool> RemoveItemAsync(Guid moduleGuid) {
            MenuInfo origMenu = Auditing.Active ? await GetItemAsync(moduleGuid) : null;
            bool result = await DataProvider.RemoveAsync(moduleGuid);
            await Auditing.AddAuditAsync($"{nameof(MenuInfoDataProvider)}.{nameof(RemoveItemAsync)}", "Menu", moduleGuid,
                "Remove Menu",
                DataBefore: origMenu,
                DataAfter: null,
                ExpensiveMultiInstance: true
            );
            return result;
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public new async Task LocalizeModelAsync(string language, Func<string, bool> isHtml, Func<List<string>, Task<List<string>>> translateStringsAsync, Func<string, Task<string>> translateComplexStringAsync) {

            const int RECORDS = 20;

            for (int offset = 0 ; ; ) {
                DataProviderGetRecords<MenuInfo> data = await DataProvider.GetRecordsAsync(offset, RECORDS, null, null);
                if (data.Data.Count == 0)
                    break;
                bool changed = false;
                foreach (MenuInfo menu in data.Data) {
                    changed = await TranslateMenuListAsync(menu.Menu, language, (s) => false, translateStringsAsync, translateComplexStringAsync);
                    if (changed) {
                        UpdateStatusEnum status = await UpdateItemAsync(menu);
                        if (status != UpdateStatusEnum.OK)
                            throw new InternalError($"Update failed for type {typeof(MenuInfo).FullName} ({status})");
                    }
                }
                offset += data.Data.Count;
                if (offset >= data.Total)
                    break;
            }
        }

        private async Task<bool> TranslateMenuListAsync(SerializableList<ModuleAction> menu, string language, Func<string, bool> isHtml, Func<List<string>, Task<List<string>>> translateStringsAsync, Func<string, Task<string>> translateComplexStringAsync) {
            bool changed = false;
            foreach (ModuleAction action in menu) {
                changed = await ObjectSupport.TranslateObject(action, language, isHtml, translateStringsAsync, translateComplexStringAsync) || changed;
                if (action.SubMenu != null && action.SubMenu.Count > 0)
                    changed = await TranslateMenuListAsync(action.SubMenu, language, isHtml, translateStringsAsync, translateComplexStringAsync) || changed;
            }
            return changed;
        }
    }
}
