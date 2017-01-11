/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Basics#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;

namespace YetaWF.Modules.Basics.DataProvider {

    public class RecaptchaConfigDataProviderInit : IInitializeApplicationStartup {

        // STARTUP
        // STARTUP
        // STARTUP

        public void InitializeApplicationStartup() {
            RecaptchaConfig.LoadRecaptchaConfig = RecaptchaConfigDataProvider.LoadRecaptchaConfig;
            RecaptchaConfig.SaveRecaptchaConfig = RecaptchaConfigDataProvider.SaveRecaptchaConfig;
        }

    }

    public class RecaptchaConfigDataProvider : DataProviderImpl, IInstallableModel {

        public static readonly int KEY = 1;

        private static object _lockObject = new object();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public RecaptchaConfigDataProvider() : base(0) { SetDataProvider(DataProvider); }

        private IDataProvider<int, RecaptchaConfig> DataProvider {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName)) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            _dataProvider = new YetaWF.DataProvider.FileDataProvider<int, RecaptchaConfig>(
                                Path.Combine(YetaWFManager.DataFolder, AreaName),
                                Cacheable: true);
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLSimpleObjectDataProvider<int, RecaptchaConfig>(AreaName, SQLDbo, SQLConn,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<int, RecaptchaConfig> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public static RecaptchaConfig GetConfig() {
            using (RecaptchaConfigDataProvider configDP = new RecaptchaConfigDataProvider()) {
                return configDP.GetItem();
            }
        }
        public RecaptchaConfig GetItem() {
            RecaptchaConfig config = DataProvider.Get(KEY);
            if (config == null) {
                lock (_lockObject) {
                    config = DataProvider.Get(KEY);
                    if (config == null) {
                        config = new RecaptchaConfig();
                        AddConfig(config);
                    }
                }
            }
            return config;
        }
        public void AddConfig(RecaptchaConfig data) {
            data.Key = KEY;
            if (!DataProvider.Add(data))
                throw new InternalError("Unexpected error adding recaptcha settings");
        }
        public void UpdateConfig(RecaptchaConfig data) {
            data.Key = KEY;
            UpdateStatusEnum status = DataProvider.Update(data.Key, data.Key, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Can't save captcha configuration {0}", status);
        }

        internal static RecaptchaConfig LoadRecaptchaConfig() {
            using (RecaptchaConfigDataProvider recaptchaDP = new RecaptchaConfigDataProvider()) {
                return recaptchaDP.GetItem();
            }
        }

        internal static void SaveRecaptchaConfig(RecaptchaConfig config) {
            using (RecaptchaConfigDataProvider recaptchaDP = new RecaptchaConfigDataProvider()) {
                recaptchaDP.UpdateConfig(config);
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
