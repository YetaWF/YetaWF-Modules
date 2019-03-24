/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using Softelvdm.Modules.TwilioProcessor.Models.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace Softelvdm.Modules.IVR.DataProvider {

    public class ExtensionPhoneNumber {
        [StringLength(Globals.MaxPhoneNumber)]
        public string PhoneNumber { get; set; }
        [Data_NewValue]
        public bool SendSMS { get; set; }
    }

    public class ExtensionEntry {

        public const int MaxExtension = 10;
        public const int MaxDescription = 80;

        [Data_Identity]
        public int Id { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        [Data_PrimaryKey]
        [StringLength(MaxExtension)]
        public string Extension { get; set; }

        [StringLength(MaxDescription)]
        public string Description { get; set; }

        public SerializableList<ExtensionPhoneNumber> PhoneNumbers { get; set; }
        public SerializableList<User> Users { get; set; }

        public ExtensionEntry() {
            PhoneNumbers = new SerializableList<ExtensionPhoneNumber>();
            Users = new SerializableList<User>();
        }
    }

    public class ExtensionEntryDataProvider : DataProviderImpl, IInstallableModel {

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public ExtensionEntryDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public ExtensionEntryDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProviderIdentity<string, object, ExtensionEntry> DataProvider { get { return GetDataProvider(); } }

        private IDataProviderIdentity<string, object, ExtensionEntry> CreateDataProvider() {
            Package package = Softelvdm.Modules.IVR.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Extensions", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public Task<ExtensionEntry> GetItemAsync(string extension) {
            return DataProvider.GetAsync(extension, null);
        }
        public Task<ExtensionEntry> GetItemByIdentityAsync(int id) {
            return DataProvider.GetByIdentityAsync(id);
        }
        public async Task<bool> AddItemAsync(ExtensionEntry data) {
            data.Created = DateTime.UtcNow;
            foreach (ExtensionPhoneNumber ext in data.PhoneNumbers)
                ext.PhoneNumber = PhoneNumberUSAttribute.GetE164(ext.PhoneNumber);// standardize
            if (!await DataProvider.AddAsync(data))
                return false;
            await Auditing.AddAuditAsync($"{nameof(ExtensionEntryDataProvider)}.{nameof(AddItemAsync)}", Dataset, Guid.Empty,
                $"Add Extension Entry {data.Id}",
                DataBefore: null,
                DataAfter: data
            );
            return true;
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(ExtensionEntry data) {

            ExtensionEntry origData = Auditing.Active ? await GetItemByIdentityAsync(data.Id) : null;

            data.Updated = DateTime.UtcNow;
            foreach (ExtensionPhoneNumber ext in data.PhoneNumbers)
                ext.PhoneNumber = PhoneNumberUSAttribute.GetE164(ext.PhoneNumber);// standardize
            UpdateStatusEnum status = await DataProvider.UpdateByIdentityAsync(data.Id, data);
            if (status != UpdateStatusEnum.OK)
                return status;

            await Auditing.AddAuditAsync($"{nameof(ExtensionEntryDataProvider)}.{nameof(UpdateItemAsync)}", Dataset, Guid.Empty,
                $"Update Extension Entry {data.Id}",
                DataBefore: origData,
                DataAfter: data
            );
            return UpdateStatusEnum.OK;
        }
        public async Task<bool> RemoveItemByIdentityAsync(int id) {

            ExtensionEntry origData = Auditing.Active ? await GetItemByIdentityAsync(id) : null;

            if (!await DataProvider.RemoveByIdentityAsync(id))
                return false;

            await Auditing.AddAuditAsync($"{nameof(ExtensionEntryDataProvider)}.{nameof(RemoveItemByIdentityAsync)}", Dataset, Guid.Empty,
                $"Remove Extension Entry {id}",
                DataBefore: origData,
                DataAfter: null
            );
            return true;
        }

        public async Task<List<string>> GetExtensionsForUserAsync(int userId) {
            // not exactly optimal, but whatevz
            List<string> list = new List<string>();
            DataProviderGetRecords<ExtensionEntry> data = await GetItemsAsync(0, 0, null, null);
            foreach (ExtensionEntry ext in data.Data) {
                foreach (User user in ext.Users) {
                    if (user.UserId == userId)
                        list.Add(ext.Extension);
                }
            }
            return list;
        }

        public Task<DataProviderGetRecords<ExtensionEntry>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            return DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
    }
}
