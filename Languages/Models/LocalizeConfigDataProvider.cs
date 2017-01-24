using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Languages.DataProvider {

    public class LocalizeConfigData {

        public enum TranslationServiceEnum {
            [EnumDescription("none", "None")]
            None = 0,
            [EnumDescription("Google Translate")]
            GoogleTranslate = 1,
            [EnumDescription("Microsoft Translator")]
            MicrosoftTranslator = 2,
        }

        public const int MaxGoogleTranslateAPIKey = 100;
        public const int MaxGoogleTranslateAppName = 100;
        public const int MaxMSClientId = 100;
        public const int MaxMSClientSecret = 100;

        [Data_PrimaryKey]
        public int Id { get; set; }

        public TranslationServiceEnum TranslationService { get; set; }

        [StringLength(MaxGoogleTranslateAPIKey)]
        public string GoogleTranslateAPIKey { get; set; }
        [StringLength(MaxGoogleTranslateAppName)]
        public string GoogleTranslateAppName { get; set; }

        [StringLength(MaxMSClientId)]
        public string MSClientId { get; set; }
        [StringLength(MaxMSClientSecret)]
        public string MSClientSecret { get; set; }

        public LocalizeConfigData() { }
    }

    public class LocalizeConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        private static object _lockObject = new object();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public LocalizeConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(DataProvider); }
        public LocalizeConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(DataProvider); }

        private IDataProvider<int, LocalizeConfigData> DataProvider {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName)) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            _dataProvider = new YetaWF.DataProvider.FileDataProvider<int, LocalizeConfigData>(
                                Path.Combine(YetaWFManager.DataFolder, AreaName + "_Config", SiteIdentity.ToString()),
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLSimpleObjectDataProvider<int, LocalizeConfigData>(AreaName + "_Config", SQLDbo, SQLConn,
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<int, LocalizeConfigData> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public static LocalizeConfigData GetConfig() {
            using (LocalizeConfigDataProvider configDP = new LocalizeConfigDataProvider()) {
                return configDP.GetItem();
            }
        }
        public LocalizeConfigData GetItem() {
            LocalizeConfigData config = DataProvider.Get(KEY);
            if (config == null) {
                lock (_lockObject) {
                    config = DataProvider.Get(KEY);
                    if (config == null) {
                        config = new LocalizeConfigData();
                        AddConfig(config);
                    }
                }
            }
            return config;
        }
        private void AddConfig(LocalizeConfigData data) {
            data.Id = KEY;
            if (!DataProvider.Add(data))
                throw new InternalError("Unexpected error adding settings");
        }
        public void UpdateConfig(LocalizeConfigData data) {
            data.Id = KEY;
            UpdateStatusEnum status = DataProvider.Update(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
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
        public void AddSiteData() {
            DataProvider.AddSiteData();
        }
        public void RemoveSiteData() {
            DataProvider.RemoveSiteData();
        }
        public bool UninstallModel(List<string> errorList) {
            return DataProvider.UninstallModel(errorList);
        }
        public bool ExportChunk(int chunk, SerializableList<SerializableFile> fileList, out object obj) {
            return DataProvider.ExportChunk(chunk, fileList, out obj);
        }
        public void ImportChunk(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            DataProvider.ImportChunk(chunk, fileList, obj);
        }
    }
}
