/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Basics#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Basics.DataProvider {

    public class AlertConfig {

        public const int MaxMessage = 2000;

        public enum MessageHandlingEnum {
            [EnumDescription("Display Once", "Display the message only once for the session")]
            DisplayOnce = 0,
            [EnumDescription("Display Until Dismissed", "Display the message until dismissed by the user")]
            DisplayUntilOff = 1,
        }

        [Data_PrimaryKey]
        public int Id { get; set; }

        public bool Enabled { get; set; }

        [StringLength(MaxMessage)]
        public MultiString CompleteMessage { get; set; }

        [Data_DontSave]
        public string Message {
            get {
                return CompleteMessage[MultiString.ActiveLanguage];
            }
            set {
                CompleteMessage[MultiString.ActiveLanguage] = value;
            }
        }

        public MessageHandlingEnum MessageHandling { get; set; }

        public AlertConfig() {
            CompleteMessage = new MultiString();
            Enabled = false;
            Message = null;
            MessageHandling = MessageHandlingEnum.DisplayUntilOff;
        }
    }

    public class AlertConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        private static object _lockObject = new object();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public AlertConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(DataProvider); }
        public AlertConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(DataProvider); }

        private IDataProvider<int, AlertConfig> DataProvider {
            get {
                if (_dataProvider == null) {
                    Package package = Package.GetPackageFromAssembly(GetType().Assembly);
                    switch (GetIOMode(package.AreaName)) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            _dataProvider = new YetaWF.DataProvider.FileDataProvider<int, AlertConfig>(
                                Path.Combine(YetaWFManager.DataFolder, AreaName + "_AlertConfig", SiteIdentity.ToString()),
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLSimpleObjectDataProvider<int, AlertConfig>(AreaName + "_AlertConfig", SQLDbo, SQLConn,
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<int, AlertConfig> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public static AlertConfig GetConfig() {
            using (AlertConfigDataProvider configDP = new AlertConfigDataProvider()) {
                return configDP.GetItem();
            }
        }
        public AlertConfig GetItem() {
            AlertConfig config = DataProvider.Get(KEY);
            if (config == null) {
                lock (_lockObject) {
                    config = DataProvider.Get(KEY);
                    if (config == null) {
                        config = new AlertConfig();
                        AddConfig(config);
                    }
                }
            }
            return config;
        }
        private void AddConfig(AlertConfig data) {
            data.Id = KEY;
            if (!DataProvider.Add(data))
                throw new InternalError("Unexpected error adding settings");
        }
        public void UpdateConfig(AlertConfig data) {
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
