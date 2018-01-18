/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.PageEdit.DataProvider {

    public class ControlPanelConfigData {

        [Data_PrimaryKey]
        public int Id { get; set; }

        [StringLength(Globals.MaxUrl)]
        public string W3CUrl { get; set; }

        [Data_Binary]
        public SerializableList<User> Users { get; set; }

        public ControlPanelConfigData() {
            W3CUrl = "https://validator.w3.org/nu/?doc={0}";
            Users = new SerializableList<User>();
        }
    }

    public class ControlPanelConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        private static object _lockObject = new object();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public ControlPanelConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public ControlPanelConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, ControlPanelConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, ControlPanelConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.PageEdit.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package.AreaName,
                () => { // File
                    return new FileDataProvider<int, ControlPanelConfigData>(
                        Path.Combine(YetaWFManager.DataFolder, AreaName + "_ControlPanel_Config", SiteIdentity.ToString()),
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                (dbo, conn) => {  // SQL
                    return new SQLSimpleObjectDataProvider<int, ControlPanelConfigData>(AreaName + "_ControlPanel_Config", dbo, conn,
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                () => { // External
                    return MakeExternalDataProvider(new { AreaName = AreaName + "_ControlPanel_Config", CurrentSiteIdentity = SiteIdentity, Cacheable = true });
                }
            );
        }

        // API
        // API
        // API

        public static ControlPanelConfigData GetConfig() {
            using (ControlPanelConfigDataProvider configDP = new ControlPanelConfigDataProvider()) {
                return configDP.GetItem();
            }
        }
        public ControlPanelConfigData GetItem() {
            ControlPanelConfigData config = DataProvider.Get(KEY);
            if (config == null) {
                lock (_lockObject) {
                    config = DataProvider.Get(KEY);
                    if (config == null) {
                        config = new ControlPanelConfigData();
                        AddConfig(config);
                    }
                }
            }
            return config;
        }
        private void AddConfig(ControlPanelConfigData data) {
            data.Id = KEY;
            if (!DataProvider.Add(data))
                throw new InternalError("Unexpected error adding settings");
        }
        public void UpdateConfig(ControlPanelConfigData data) {
            data.Id = KEY;
            UpdateStatusEnum status = DataProvider.Update(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
        }
    }
}
