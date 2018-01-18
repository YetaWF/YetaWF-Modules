/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Menus#License */

using System;
using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Menus;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

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
            return MakeDataProvider(package.AreaName + "_MenuInfo",
                () => { // File
                    return new FileDataProvider<Guid, MenuInfo>(
                        Path.Combine(YetaWFManager.DataFolder, AreaName, SiteIdentity.ToString()),
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                (dbo, conn) => {  // SQL
                    return new SQLSimpleObjectDataProvider<Guid, MenuInfo>(AreaName, dbo, conn,
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                () => { // External
                    return MakeExternalDataProvider(new { AreaName = AreaName, CurrentSiteIdentity = SiteIdentity, Cacheable = true });
                }
            );
        }

        // API
        // API
        // API

        public void DoAction(Guid moduleGuid, Action action) {
            StringLocks.DoAction(LockKey(moduleGuid), () => {
                action();
            });
        }
        private string LockKey(Guid moduleGuid) {
            return string.Format("{0}_{1}", this.AreaName, moduleGuid);
        }
        public MenuInfo GetItem(Guid moduleGuid) {
            return DataProvider.Get(moduleGuid);
        }
        public void ReplaceItem(MenuInfo data) {
            DoAction(data.ModuleGuid, () => {
                RemoveItem(data.ModuleGuid);
                AddItem(data);
            });
        }
        protected bool AddItem(MenuInfo data) {
            return DataProvider.Add(data);
        }
        protected UpdateStatusEnum UpdateItem(MenuInfo data) {
            return DataProvider.Update(data.ModuleGuid, data.ModuleGuid, data);
        }
        public bool RemoveItem(Guid moduleGuid) {
            return DataProvider.Remove(moduleGuid);
        }
    }
}
