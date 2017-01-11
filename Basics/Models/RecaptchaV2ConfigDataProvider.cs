/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Basics#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.Basics.DataProvider {

    public class RecaptchaV2ConfigDataProviderInit : IInitializeApplicationStartup {

        // STARTUP
        // STARTUP
        // STARTUP

        public void InitializeApplicationStartup() {
            RecaptchaV2Config.LoadRecaptchaV2Config = RecaptchaV2ConfigDataProvider.LoadRecaptchaV2Config;
            RecaptchaV2Config.SaveRecaptchaV2Config = RecaptchaV2ConfigDataProvider.SaveRecaptchaV2Config;
        }
    }

    public class RecaptchaV2ConfigDataProvider : DataProviderImpl, IInstallableModel {

        public static readonly int KEY = 1;

        private static object _lockObject = new object();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public RecaptchaV2ConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(DataProvider); }
        public RecaptchaV2ConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(DataProvider); }

        private IDataProvider<int, RecaptchaV2Config> DataProvider {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName + "_RecaptchaV2")) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            _dataProvider = new YetaWF.DataProvider.FileDataProvider<int, RecaptchaV2Config>(
                                Path.Combine(YetaWFManager.DataFolder, AreaName),
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLSimpleObjectDataProvider<int, RecaptchaV2Config>(AreaName, SQLDbo, SQLConn,
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<int, RecaptchaV2Config> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public static RecaptchaV2Config GetConfig() {
            using (RecaptchaV2ConfigDataProvider configDP = new RecaptchaV2ConfigDataProvider()) {
                return configDP.GetItem();
            }
        }
        public RecaptchaV2Config GetItem() {
            RecaptchaV2Config config = DataProvider.Get(KEY);
            if (config == null) {
                lock (_lockObject) {
                    config = DataProvider.Get(KEY);
                    if (config == null) {
                        config = new RecaptchaV2Config();
                        AddConfig(config);
                    }
                }
            }
            return config;
        }
        public void AddConfig(RecaptchaV2Config data) {
            data.Key = KEY;
            if (!DataProvider.Add(data))
                throw new InternalError("Unexpected error adding RecaptchaV2 Settings");
        }
        public void UpdateConfig(RecaptchaV2Config data) {
            data.Key = KEY;
            UpdateStatusEnum status = DataProvider.Update(data.Key, data.Key, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Can't save captcha configuration {0}", status);
        }

        internal static RecaptchaV2Config LoadRecaptchaV2Config() {
            using (RecaptchaV2ConfigDataProvider dp = new RecaptchaV2ConfigDataProvider()) {
                return dp.GetItem();
            }
        }

        internal static void SaveRecaptchaV2Config(RecaptchaV2Config config) {
            using (RecaptchaV2ConfigDataProvider dp = new RecaptchaV2ConfigDataProvider()) {
                dp.UpdateConfig(config);
            }
        }


        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public bool IsInstalled() {
            return DataProvider.IsInstalled();
        }
        public bool InstallModel(List<string> errorList) {
            return DataProvider.InstallModel(errorList);
        }
        public bool UninstallModel(List<string> errorList) {
            return DataProvider.UninstallModel(errorList);
        }
        public void AddSiteData() {
            DataProvider.AddSiteData();
        }
        public void RemoveSiteData() {
            DataProvider.RemoveSiteData();
        }
        public bool ExportChunk(int chunk, SerializableList<SerializableFile> fileList, out object obj) {
            return DataProvider.ExportChunk(chunk, fileList, out obj);
        }
        public void ImportChunk(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            DataProvider.ImportChunk(chunk, fileList, obj);
        }
    }
}
