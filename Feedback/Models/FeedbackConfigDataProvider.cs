/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Feedback#License */

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

namespace YetaWF.Modules.Feedback.DataProvider {

    public class FeedbackConfigData {

        public const int MaxProp1 = 100;
        public const int MaxProp2 = 100;

        [Data_PrimaryKey]
        public int Id { get; set; }

        public bool Captcha { get; set; }
        [Data_NewValue]
        public bool RequireName { get; set; }
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

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public FeedbackConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public FeedbackConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, FeedbackConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, FeedbackConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.Feedback.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Config", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public static async Task<FeedbackConfigData> GetConfigAsync() {
            using (FeedbackConfigDataProvider configDP = new FeedbackConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<FeedbackConfigData> GetItemAsync() {
            FeedbackConfigData config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                config = new FeedbackConfigData();
                await AddConfigAsync(config);
            }
            return config;
        }
        private async Task AddConfigAsync(FeedbackConfigData data) {
            data.Id = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding settings");
            await Auditing.AddAuditAsync($"{nameof(FeedbackConfigDataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
                "Add Feedback Config",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
        public async Task UpdateConfigAsync(FeedbackConfigData data) {
            FeedbackConfigData origConfig = Auditing.Active ? await GetItemAsync() : null;
            data.Id = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
            await Auditing.AddAuditAsync($"{nameof(FeedbackConfigDataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
                "Update Feedback Config",
                DataBefore: origConfig,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
    }
}
