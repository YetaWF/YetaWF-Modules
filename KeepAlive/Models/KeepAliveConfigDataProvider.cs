/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/KeepAlive#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.KeepAlive.DataProvider {

    public class KeepAliveConfigData {

        [Data_PrimaryKey]
        public int Id { get; set; }

        public int Interval { get; set; }
        [StringLength(Globals.MaxUrl)]
        public string Url { get; set; }

        public KeepAliveConfigData() {
            Interval = 30;
            Url = null;
        }
    }

    public class KeepAliveConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public KeepAliveConfigDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, KeepAliveConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, KeepAliveConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.KeepAlive.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Config", Cacheable: true);
        }

        // API
        // API
        // API

        public static async Task<KeepAliveConfigData> GetConfigAsync() {
            using (KeepAliveConfigDataProvider configDP = new KeepAliveConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<KeepAliveConfigData> GetItemAsync() {
            KeepAliveConfigData config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                config = new KeepAliveConfigData();
                await AddConfigAsync(config);
            }
            return config;
        }
        private async Task AddConfigAsync(KeepAliveConfigData data) {
            data.Id = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding settings");
            await Auditing.AddAuditAsync($"{nameof(KeepAliveConfigDataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
                "Add KeepAlive Config",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
        public async Task UpdateConfigAsync(KeepAliveConfigData data) {
            KeepAliveConfigData origConfig = Auditing.Active ? await GetItemAsync() : null;
            data.Id = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
            await Auditing.AddAuditAsync($"{nameof(KeepAliveConfigDataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
                "Update KeepAlive Config",
                DataBefore: origConfig,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
    }
}
