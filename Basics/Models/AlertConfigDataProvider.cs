/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Basics#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
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
            Message = string.Empty;
            MessageHandling = MessageHandlingEnum.DisplayUntilOff;
        }
    }

    public class AlertConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public AlertConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public AlertConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, AlertConfig> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, AlertConfig>? CreateDataProvider() {
            Package package = YetaWF.Modules.Basics.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_AlertConfig", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public static async Task<AlertConfig> GetConfigAsync() {
            using (AlertConfigDataProvider configDP = new AlertConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<AlertConfig> GetItemAsync() {
            AlertConfig? config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                config = new AlertConfig();
                await AddConfigAsync(config);
            }
            return config;
        }
        private async Task AddConfigAsync(AlertConfig data) {
            data.Id = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding settings");
            await Auditing.AddAuditAsync($"{nameof(AlertConfigDataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
                "Add Alert Config",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
        public async Task UpdateConfigAsync(AlertConfig data) {
            AlertConfig? origConfig = Auditing.Active ? await GetItemAsync() : null;
            data.Id = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
            await Auditing.AddAuditAsync($"{nameof(AlertConfigDataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
                "Update Alert Config",
                DataBefore: origConfig,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
    }
}
