/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

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

    public class CallLogEntry {

        public const int MaxCity = 100;
        public const int MaxState = 30;
        public const int MaxZip = 20;
        public const int MaxCountry = 100;

        [Data_Identity, Data_PrimaryKey]
        public int Id { get; set; }

        public DateTime Created { get; set; }

        [StringLength(Globals.MaxPhoneNumber)]
        public string Caller { get; set; }
        [StringLength(MaxCity)]
        public string CallerCity { get; set; }
        [StringLength(MaxState)]
        public string CallerState { get; set; }
        [StringLength(MaxZip)]
        public string CallerZip { get; set; }
        [StringLength(MaxCountry)]
        public string CallerCountry { get; set; }

        [Data_Index, StringLength(Globals.MaxPhoneNumber)]
        public string To { get; set; }

        public CallLogEntry() { }
    }

    public class CallLogDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public CallLogDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public CallLogDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProviderIdentity<int, object, CallLogEntry> DataProvider { get { return GetDataProvider(); } }

        private IDataProviderIdentity<int, object, CallLogEntry> CreateDataProvider() {
            Package package = Softelvdm.Modules.IVR.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_CallLog", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public Task<CallLogEntry> GetItemByIdentityAsync(int id) {
            return DataProvider.GetByIdentityAsync(id);
        }
        public async Task<bool> AddItemAsync(CallLogEntry data) {
            data.Created = DateTime.UtcNow;
            if (!await DataProvider.AddAsync(data))
                return false;
            await Auditing.AddAuditAsync($"{nameof(CallLogDataProvider)}.{nameof(AddItemAsync)}", Dataset, Guid.Empty,
                $"Add Call Log Entry {data.Id}",
                DataBefore: null,
                DataAfter: data
            );
            return true;
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(CallLogEntry data) {

            CallLogEntry origData = Auditing.Active ? await GetItemByIdentityAsync(data.Id) : null;

            //data.Updated = DateTime.UtcNow;
            UpdateStatusEnum status = await DataProvider.UpdateByIdentityAsync(data.Id, data);
            if (status != UpdateStatusEnum.OK)
                return status;

            await Auditing.AddAuditAsync($"{nameof(CallLogDataProvider)}.{nameof(UpdateItemAsync)}", Dataset, Guid.Empty,
                $"Update Call Log Entry {data.Id}",
                DataBefore: origData,
                DataAfter: data
            );
            return UpdateStatusEnum.OK;
        }
        public async Task<bool> RemoveItemByIdentityAsync(int id) {

            CallLogEntry origData = Auditing.Active ? await GetItemByIdentityAsync(id) : null;

            if (!await DataProvider.RemoveByIdentityAsync(id))
                return false;

            await Auditing.AddAuditAsync($"{nameof(CallLogDataProvider)}.{nameof(RemoveItemByIdentityAsync)}", Dataset, Guid.Empty,
                $"Remove Call Log Entry {id}",
                DataBefore: origData,
                DataAfter: null
            );
            return true;
        }
        public Task<DataProviderGetRecords<CallLogEntry>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            return DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
    }
}
