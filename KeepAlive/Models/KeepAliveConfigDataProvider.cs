/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/KeepAlive#License */

using YetaWF.Core;
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

        private static object _lockObject = new object();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public KeepAliveConfigDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, KeepAliveConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, KeepAliveConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.KeepAlive.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider2(package, package.AreaName + "_Config", Cacheable: true);
        }

        // API
        // API
        // API

        public static KeepAliveConfigData GetConfig() {
            using (KeepAliveConfigDataProvider configDP = new KeepAliveConfigDataProvider()) {
                return configDP.GetItem();
            }
        }
        public KeepAliveConfigData GetItem() {
            KeepAliveConfigData config = DataProvider.Get(KEY);
            if (config == null) {
                lock (_lockObject) {
                    config = DataProvider.Get(KEY);
                    if (config == null) {
                        config = new KeepAliveConfigData();
                        AddConfig(config);
                    }
                }
            }
            return config;
        }
        private void AddConfig(KeepAliveConfigData data) {
            data.Id = KEY;
            if (!DataProvider.Add(data))
                throw new InternalError("Unexpected error adding settings");
        }
        public void UpdateConfig(KeepAliveConfigData data) {
            data.Id = KEY;
            UpdateStatusEnum status = DataProvider.Update(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
        }
    }
}
