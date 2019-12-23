/* Copyright �2020 Softel vdm, Inc.. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace Softelvdm.Modules.IVR.DataProvider {

    public class HolidayEntry {

        public const int MaxDescription = 100;

        [Data_Identity]
        public int Id { get; set; }

        [Data_PrimaryKey]
        public DateTime HolidayDate { get; set; }

        [StringLength(MaxDescription)]
        public string Description { get; set; }

        public HolidayEntry() { }
    }

    public class HolidayEntryDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public HolidayEntryDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public HolidayEntryDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProviderIdentity<DateTime, object, HolidayEntry> DataProvider { get { return GetDataProvider(); } }

        private IDataProviderIdentity<DateTime, object, HolidayEntry> CreateDataProvider() {
            Package package = Softelvdm.Modules.IVR.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Holidays", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public Task<HolidayEntry> GetItemAsync(DateTime holidayDate) {
            return DataProvider.GetAsync(holidayDate, null);
        }
        public Task<HolidayEntry> GetItemByIdentityAsync(int id) {
            return DataProvider.GetByIdentityAsync(id);
        }
        public async Task<bool> AddItemAsync(HolidayEntry data) {
            if (!await DataProvider.AddAsync(data))
                return false;
            await Auditing.AddAuditAsync($"{nameof(HolidayEntryDataProvider)}.{nameof(AddItemAsync)}", Dataset, Guid.Empty,
                $"Add Holiday Entry {data.Id}",
                DataBefore: null,
                DataAfter: data
            );
            return true;
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(HolidayEntry data) {

            HolidayEntry origData = Auditing.Active ? await GetItemByIdentityAsync(data.Id) : null;

            UpdateStatusEnum status = await DataProvider.UpdateByIdentityAsync(data.Id, data);
            if (status != UpdateStatusEnum.OK)
                return status;

            await Auditing.AddAuditAsync($"{nameof(HolidayEntryDataProvider)}.{nameof(UpdateItemAsync)}", Dataset, Guid.Empty,
                $"Update Holiday Entry {data.Id}",
                DataBefore: origData,
                DataAfter: data
            );
            return UpdateStatusEnum.OK;
        }
        public async Task<bool> RemoveItemByIdentityAsync(int id) {

            HolidayEntry origData = Auditing.Active ? await GetItemByIdentityAsync(id) : null;

            if (!await DataProvider.RemoveByIdentityAsync(id))
                return false;

            await Auditing.AddAuditAsync($"{nameof(HolidayEntryDataProvider)}.{nameof(RemoveItemByIdentityAsync)}", Dataset, Guid.Empty,
                $"Remove Holiday Entry {id}",
                DataBefore: origData,
                DataAfter: null
            );
            return true;
        }
        public Task<DataProviderGetRecords<HolidayEntry>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            return DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
    }
}
