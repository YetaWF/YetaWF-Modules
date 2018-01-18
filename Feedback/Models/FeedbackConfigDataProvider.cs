/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

namespace YetaWF.Modules.Feedback.DataProvider {

    public class FeedbackConfigData {

        public const int MaxProp1 = 100;
        public const int MaxProp2 = 100;

        [Data_PrimaryKey]
        public int Id { get; set; }

        public bool Captcha { get; set; }
        public bool RequireEmail { get; set; }
        public bool BccEmails { get; set; }
        [StringLength(Globals.MaxEmail)]
        public string Email { get; set; }

        public FeedbackConfigData() {
            RequireEmail = true;
            Captcha = true;
            BccEmails = false;
        }
    }

    public class FeedbackConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        private static object _lockObject = new object();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public FeedbackConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public FeedbackConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, FeedbackConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, FeedbackConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.Feedback.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Config",
                () => { // File
                    return new FileDataProvider<int, FeedbackConfigData>(
                        Path.Combine(YetaWFManager.DataFolder, Dataset, SiteIdentity.ToString()),
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                (dbo, conn) => {  // SQL
                    return new SQLSimpleObjectDataProvider<int, FeedbackConfigData>(Dataset, dbo, conn,
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

        public static FeedbackConfigData GetConfig() {
            using (FeedbackConfigDataProvider configDP = new FeedbackConfigDataProvider()) {
                return configDP.GetItem();
            }
        }
        public FeedbackConfigData GetItem() {
            FeedbackConfigData config = DataProvider.Get(KEY);
            if (config == null) {
                lock (_lockObject) {
                    config = DataProvider.Get(KEY);
                    if (config == null) {
                        config = new FeedbackConfigData();
                        AddConfig(config);
                    }
                }
            }
            return config;
        }
        private void AddConfig(FeedbackConfigData data) {
            data.Id = KEY;
            if (!DataProvider.Add(data))
                throw new InternalError("Unexpected error adding settings");
        }
        public void UpdateConfig(FeedbackConfigData data) {
            data.Id = KEY;
            UpdateStatusEnum status = DataProvider.Update(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
        }
    }
}
