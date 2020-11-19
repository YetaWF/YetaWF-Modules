/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Scheduler#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Scheduler.DataProvider {

    public class SchedulerConfigData {

        [Data_PrimaryKey]
        public int Id { get; set; }

        public int Days { get; set; }

        public SchedulerConfigData() {
            Days = 14;
        }
    }

    public class SchedulerConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public SchedulerConfigDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, SchedulerConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, SchedulerConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.Scheduler.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Config", Cacheable: true);
        }

        // API
        // API
        // API

        public static async Task<SchedulerConfigData> GetConfigAsync() {
            using (SchedulerConfigDataProvider configDP = new SchedulerConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<SchedulerConfigData> GetItemAsync() {
            SchedulerConfigData config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                config = new SchedulerConfigData();
                await AddConfigAsync(config);
            }
            return config;
        }
        private async Task AddConfigAsync(SchedulerConfigData data) {
            data.Id = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding settings");
            await Auditing.AddAuditAsync($"{nameof(SchedulerConfigDataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
                "Add Scheduler Config",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
        public async Task UpdateConfigAsync(SchedulerConfigData data) {
            SchedulerConfigData origConfig = Auditing.Active ? await GetItemAsync() : null;
            data.Id = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
            await Auditing.AddAuditAsync($"{nameof(SchedulerConfigDataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
                "Update Scheduler Config",
                DataBefore: origConfig,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
    }
}
