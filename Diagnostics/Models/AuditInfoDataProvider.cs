using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Extensions;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Diagnostics.DataProvider {

    public class AuditInfo {

        public const int MaxAction = 60;
        public const int MaxIdentifyString = 80;

        [Data_Identity, Data_PrimaryKey]
        public int Id { get; set; }
        [Data_Index]
        public DateTime Created { get; set; }
        public int SiteIdentity { get; set; }
        [Data_NewValue("(0)")]
        public int UserId { get; set; }

        [Data_Index]
        [StringLength(MaxIdentifyString)]
        public string IdentifyString { get; set; }
        [Data_Index]
        public Guid IdentifyGuid { get; set; }

        [Data_Index]
        [StringLength(MaxAction)]
        public string Action { get; set; }
        [StringLength(0)]
        public string Changes { get; set; }
        [StringLength(0)]
        public string Description { get; set; }
        public bool RequiresRestart { get; set; }
        public bool ExpensiveMultiInstance { get; set; }

        [Data_Binary]
        public byte[] DataBefore { get; set; }
        [Data_Binary]
        public byte[] DataAfter { get; set; }

        public AuditInfo() { }
    }

    public class AuditInfoDataProvider : DataProviderImpl, IInstallableModel, IAudit, IInitializeApplicationStartup {

        // Startup
        // Startup
        // Startup

        public Task InitializeApplicationStartupAsync() {
            YetaWF.Core.Audit.Auditing.AuditProvider = this;
            return Task.CompletedTask;
        }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public AuditInfoDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProviderIdentity<int, object, AuditInfo> DataProvider { get { return GetDataProvider(); } }

        private IDataProviderIdentity<int, object, AuditInfo> CreateDataProvider() {
            Package package = YetaWF.Modules.Diagnostics.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_AuditInfo", Parms: new { NoLanguages = true });
        }

        // API
        // API
        // API

        public Task<AuditInfo> GetItemAsync(int id) {
            return DataProvider.GetByIdentityAsync(id);
        }
        public Task<bool> AddItemAsync(AuditInfo data) {
            return DataProvider.AddAsync(data);
        }
        public Task<UpdateStatusEnum> UpdateItemAsync(AuditInfo data) {
            return DataProvider.UpdateByIdentityAsync(data.Id, data);
        }
        public Task<bool> RemoveItemAsync(int id) {
            return DataProvider.RemoveByIdentityAsync(id);
        }
        public Task<DataProviderGetRecords<AuditInfo>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            return DataProvider.GetRecordsAsync(skip, take, sort, filters);
        }
        public Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            return DataProvider.RemoveRecordsAsync(filters);
        }

        // IAudit
        // IAudit
        // IAudit

        public async Task AddAsync(Core.Audit.AuditInfo info) {
            AuditInfo auditInfo = new AuditInfo {
                Changes = info.Changes,
                Created = DateTime.UtcNow,
                Action = info.Action.Truncate(AuditInfo.MaxAction),
                DataAfter = info.DataAfter,
                DataBefore = info.DataBefore,
                Description = info.Description,
                ExpensiveMultiInstance = info.ExpensiveMultiInstance,
                IdentifyGuid = info.IdentifyGuid,
                IdentifyString = info.IdentifyString != null ? info.IdentifyString.Truncate(AuditInfo.MaxIdentifyString) : "",
                RequiresRestart = info.RequiresRestart,
                SiteIdentity = info.SiteIdentity,
                UserId = info.UserId,
            };
            if (!await AddItemAsync(auditInfo))
                throw new InternalError("Couldn't add audit information");
        }
        public async Task<bool> HasItemsAsync() {
            List<DataProviderFilterInfo> filters = null;
            filters = DataProviderFilterInfo.Join(filters, new DataProviderFilterInfo { Field = "Created", Operator = ">=", Value = YetaWF.Core.Support.Startup.MultiInstanceStartTime });
            DataProviderGetRecords<AuditInfo> info = await DataProvider.GetRecordsAsync(0, 1, null, filters);
            return info.Total > 0;
        }
        //$$$ call to reset during multinstance startup
        public Task RemoveAllAsync() {
            return DataProvider.RemoveRecordsAsync(null);
        }
    }
}
