/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace Softelvdm.Modules.IVR.DataProvider {

    public class BlockedNumberEntry {

        public const int MaxDescription = 10000;

        [Data_Identity]
        public int Id { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        [Data_PrimaryKey]
        [StringLength(Globals.MaxPhoneNumber)]
        public string Number { get; set; } = null!;

        [StringLength(MaxDescription)]
        public string? Description { get; set; }

        public BlockedNumberEntry() { }
    }

    public class BlockedNumberDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public BlockedNumberDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public BlockedNumberDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProviderIdentity<string, object, BlockedNumberEntry> DataProvider { get { return GetDataProvider(); } }

        private IDataProviderIdentity<string, object, BlockedNumberEntry>? CreateDataProvider() {
            Package package = Softelvdm.Modules.IVR.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_BlockedNumbers", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public Task<BlockedNumberEntry?> GetItemAsync(string number) {
            string? n = PhoneNumberValidationAttribute.GetE164(number);
            if (n == null) return Task.FromResult<BlockedNumberEntry?>(null);
            return DataProvider.GetAsync(n, null);
        }
        public Task<BlockedNumberEntry?> GetItemByIdentityAsync(int id) {
            return DataProvider.GetByIdentityAsync(id);
        }
        public async Task<bool> AddItemAsync(BlockedNumberEntry data) {

            data.Created = DateTime.UtcNow;
            data.Number = PhoneNumberValidationAttribute.GetE164(data.Number) ?? throw new InternalError($"Phone number {data.Number} is invalid");

            if (!await DataProvider.AddAsync(data))
                return false;
            await Auditing.AddAuditAsync($"{nameof(BlockedNumberDataProvider)}.{nameof(AddItemAsync)}", Dataset, Guid.Empty,
                $"Add Blocked Number {data.Id}",
                DataBefore: null,
                DataAfter: data
            );
            return true;
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(BlockedNumberEntry data) {

            BlockedNumberEntry? origData = Auditing.Active ? await GetItemByIdentityAsync(data.Id) : null;

            data.Updated = DateTime.UtcNow;
            data.Number = PhoneNumberValidationAttribute.GetE164(data.Number) ?? throw new InternalError($"Phone number {data.Number} is invalid");
            UpdateStatusEnum status = await DataProvider.UpdateByIdentityAsync(data.Id, data);
            if (status != UpdateStatusEnum.OK)
                return status;

            await Auditing.AddAuditAsync($"{nameof(BlockedNumberDataProvider)}.{nameof(UpdateItemAsync)}", Dataset, Guid.Empty,
                $"Update Blocked Number {data.Id}",
                DataBefore: origData,
                DataAfter: data
            );
            return UpdateStatusEnum.OK;
        }
        public async Task<bool> RemoveItemByIdentityAsync(int id) {

            BlockedNumberEntry? origData = Auditing.Active ? await GetItemByIdentityAsync(id) : null;

            if (!await DataProvider.RemoveByIdentityAsync(id))
                return false;

            await Auditing.AddAuditAsync($"{nameof(BlockedNumberDataProvider)}.{nameof(RemoveItemByIdentityAsync)}", Dataset, Guid.Empty,
                $"Remove Blocked Number {id}",
                DataBefore: origData,
                DataAfter: null
            );
            return true;
        }
        public Task<DataProviderGetRecords<BlockedNumberEntry>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo>? sort, List<DataProviderFilterInfo>? filters) {
            return DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
    }
}
