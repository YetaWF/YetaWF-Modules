/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/TwilioProcessor#License */

using Softelvdm.Modules.TwilioProcessor.Controllers;
using Softelvdm.Modules.TwilioProcessor.Models.Attributes;
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

namespace Softelvdm.Modules.TwilioProcessor.DataProvider {

    public class TwilioData {

        public const int MaxAccountSid = 60;
        public const int MaxAuthToken = 60;

        [Data_PrimaryKey]
        public int Id { get; set; }

        public bool TestMode { get { return !LiveMode; } }

        public bool LiveMode { get { return WebConfigHelper.GetValue<bool>(AreaRegistration.CurrentPackage.AreaName, "Live"); } }
        public string LiveAccountSid { get { return WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "LiveAccountSid"); } }
        public string LiveAuthToken { get { return WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "LiveAuthToken"); } }
        public string TestAccountSid { get { return WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "TestAccountSid"); } }
        public string TestAuthToken { get { return WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, "TestAuthToken"); } }

        public bool SMSEnabled { get; set; }

        [StringLength(Globals.MaxPhoneNumber)]
        public string LiveSMSNumber { get; set; }

        [StringLength(Globals.MaxPhoneNumber)]
        public string TestSMSNumber { get; set; }

        public bool DeliveryReceipts { get; set; }
        public bool UseHttps { get; set; }

        public bool IsSMSConfigured() {
            return SMSEnabled && IsConfigured() && (TestMode ? !string.IsNullOrWhiteSpace(TestSMSNumber) : !string.IsNullOrWhiteSpace(LiveSMSNumber));
        }
        public bool IsConfigured() {
            return (TestMode ? !string.IsNullOrWhiteSpace(TestAccountSid) : !string.IsNullOrWhiteSpace(LiveAccountSid)) &&
                (TestMode ? !string.IsNullOrWhiteSpace(TestAuthToken) : !string.IsNullOrWhiteSpace(LiveAuthToken));
        }

        public string SMSNumberE164 {
            get {
                return PhoneNumberUSAttribute.GetE164(SMSNumber);
            }
        }
        public string SMSNumber {
            get {
                return TestMode ? TestSMSNumber : LiveSMSNumber;
            }
        }

        public TwilioData() {
            SMSEnabled = false;
        }
    }

    public class TwilioConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public TwilioConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public TwilioConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, TwilioData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, TwilioData> CreateDataProvider() {
            Package package = Softelvdm.Modules.TwilioProcessor.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Config", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public static async Task<TwilioData> GetConfigAsync() {
            using (TwilioConfigDataProvider configDP = new TwilioConfigDataProvider()) {
                TwilioData config = await configDP.GetItemAsync();
                if (config == null)
                    throw new InternalError("No Twilio settings defined");
                return config;
            }
        }
        public static async Task<TwilioData> GetConfigCondAsync() {
            using (TwilioConfigDataProvider configDP = new TwilioConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<TwilioData> GetItemAsync() {
            TwilioData config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                config = new TwilioData();
                await AddConfigAsync(config);
            }
            return config;
        }
        private async Task AddConfigAsync(TwilioData data) {
            data.Id = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding settings");
            await Auditing.AddAuditAsync($"{nameof(TwilioConfigDataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
                "Add Softelvdm_TwilioProcessor Config",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
        public async Task UpdateConfigAsync(TwilioData data) {
            TwilioData origConfig = Auditing.Active ? await GetItemAsync() : null;
            data.Id = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
            await Auditing.AddAuditAsync($"{nameof(TwilioConfigDataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
                "Update Softelvdm_TwilioProcessor Config",
                DataBefore: origConfig,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
    }
}
