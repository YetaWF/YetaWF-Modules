/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

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
using YetaWF.DataProvider;

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

        public AlertConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public AlertConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, AlertConfig> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, AlertConfig> CreateDataProvider() {
            Package package = YetaWF.Modules.Basics.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_AlertConfig",
                () => { // File
                    return new FileDataProvider<int, AlertConfig>(
                        Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()),
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                (dbo, conn) => {  // SQL
                    return new SQLSimpleObjectDataProvider<int, AlertConfig>(Dataset, dbo, conn,
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                () => { // External
                    return MakeExternalDataProvider(new { Package = Package, Dataset = Dataset, CurrentSiteIdentity = SiteIdentity, Cacheable = true });
                }
            );
        }

        // API
        // API
        // API

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
    }
}
