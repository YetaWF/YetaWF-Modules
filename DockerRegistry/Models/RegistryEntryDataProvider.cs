/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/DockerRegistry#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.DockerRegistry.DataProvider {

    public class RegistryEntry {

        public const int MaxRegistryName = 100;
        public const int MaxUserName = 80;
        public const int MaxPassword = 40;

        [Data_Identity, Data_PrimaryKey]
        public int Id { get; set; }

        public int UserId { get; set; }

        [StringLength(MaxRegistryName)]
        public string RegistryName { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string RegistryURL { get; set; }
        [StringLength(MaxUserName)]
        public string UserName { get; set; }
        [StringLength(MaxPassword)]
        public string Password { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public RegistryEntry() { }

        public static implicit operator RegistryEntry(DataProviderGetRecords<RegistryEntry> v) {
            throw new NotImplementedException();
        }
    }


    public class RegistryEntryDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public RegistryEntryDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public RegistryEntryDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, RegistryEntry> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, RegistryEntry> CreateDataProvider() {
            Package package = YetaWF.Modules.DockerRegistry.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_RegistryEntry", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public async Task<RegistryEntry> GetItemAsync(int id) {
            Manager.NeedUser();
            RegistryEntry reg = await DataProvider.GetAsync(id);
            if (reg == null)
                return null;
            if (reg.UserId != Manager.UserId)
                return null;
            return reg;
        }
        public Task<bool> AddItemAsync(RegistryEntry data) {
            Manager.NeedUser();
            data.UserId = Manager.UserId;
            data.Created = DateTime.UtcNow;
            return DataProvider.AddAsync(data);
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(RegistryEntry data) {
            if (!await VerifyOwnerAsync(data.Id))
                return UpdateStatusEnum.RecordDeleted;
            data.Updated = DateTime.UtcNow;
            return await DataProvider.UpdateAsync(data.Id, data.Id, data);
        }
        private async Task<bool> VerifyOwnerAsync(int id) {
            Manager.NeedUser();
            RegistryEntry reg = await GetItemAsync(id);
            if (reg == null)
                return false;
            if (reg.UserId != Manager.UserId)
                return false;
            return true;
        }
        public async Task<bool> RemoveItemAsync(int id) {
            if (!await VerifyOwnerAsync(id))
                return false;
            return await DataProvider.RemoveAsync(id);
        }
        public Task<DataProviderGetRecords<RegistryEntry>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            Manager.NeedUser();
            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = nameof(RegistryEntry.UserId), Operator = "==", Value = Manager.UserId });
            return DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
        public Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            return DataProvider.RemoveRecordsAsync(filters);
        }
    }
}
