/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System;
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

namespace YetaWF.Modules.PageEdit.DataProvider {

    public class ControlPanelConfigData {

        [Data_PrimaryKey]
        public int Id { get; set; }

        [StringLength(Globals.MaxUrl)]
        [RequiresPageReload]
        public string W3CUrl { get; set; }

        [Data_Binary]
        [RequiresPageReload]
        public SerializableList<User> Users { get; set; }

        public ControlPanelConfigData() {
            W3CUrl = "https://validator.w3.org/nu/?doc={0}";
            Users = new SerializableList<User>();
        }
    }

    public class ControlPanelConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public ControlPanelConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public ControlPanelConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, ControlPanelConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, ControlPanelConfigData>? CreateDataProvider() {
            Package package = YetaWF.Modules.PageEdit.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_ControlPanel_Config", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public static async Task<ControlPanelConfigData> GetConfigAsync() {
            using (ControlPanelConfigDataProvider configDP = new ControlPanelConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<ControlPanelConfigData> GetItemAsync() {
            ControlPanelConfigData? config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                config = new ControlPanelConfigData();
                await AddConfigAsync(config);
            }
            return config;
        }
        private async Task AddConfigAsync(ControlPanelConfigData data) {
            data.Id = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding settings");
            await Auditing.AddAuditAsync($"{nameof(ControlPanelConfigDataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
                "Add ControlPanel Config",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
        public async Task UpdateConfigAsync(ControlPanelConfigData data) {
            ControlPanelConfigData? origConfig = Auditing.Active ? await GetItemAsync() : null;
            data.Id = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
            await Auditing.AddAuditAsync($"{nameof(ControlPanelConfigDataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
                "Update ControlPanel Config",
                DataBefore: origConfig,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
    }
}
