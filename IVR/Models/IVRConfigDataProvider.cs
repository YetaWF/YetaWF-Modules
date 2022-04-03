/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/IVR#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Audit;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Security;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace Softelvdm.Modules.IVR.DataProvider {

    public class IVRConfig {

        public const int MaxVoice = 60;

        [Data_PrimaryKey]
        public int Id { get; set; }

        public bool TestMode { get { return ! WebConfigHelper.GetValue<bool>(Softelvdm.Modules.TwilioProcessor.AreaRegistration.CurrentPackage.AreaName, "Live"); } }
        public string? LiveAccountSid { get { return WebConfigHelper.GetValue<string>(Softelvdm.Modules.TwilioProcessor.AreaRegistration.CurrentPackage.AreaName, "LiveAccountSid"); } }
        public string? LiveAuthToken { get { return WebConfigHelper.GetValue<string>(Softelvdm.Modules.TwilioProcessor.AreaRegistration.CurrentPackage.AreaName, "LiveAuthToken"); } }
        public string? TestAccountSid { get { return WebConfigHelper.GetValue<string>(Softelvdm.Modules.TwilioProcessor.AreaRegistration.CurrentPackage.AreaName, "TestAccountSid"); } }
        public string? TestAuthToken { get { return WebConfigHelper.GetValue<string>(Softelvdm.Modules.TwilioProcessor.AreaRegistration.CurrentPackage.AreaName, "TestAuthToken"); } }

        [StringLength(Globals.MaxUrl)]
        public string? LiveVerificationProcessCallUrl { get; set; }

        [StringLength(Globals.MaxUrl)]
        public string? TestVerificationProcessCallUrl { get; set; }

        [Data_Binary]
        public SerializableList<ExtensionPhoneNumber> NotificationNumbers { get; set; }

        [StringLength(Globals.MaxUrl)]
        public string? DisplayVoiceMailUrl { get; set; }

        [StringLength(MaxVoice)]
        public string? Voice { get; set; }
        [StringLength(MaxVoice)]
        public string? VoiceInternal { get; set; }

        [StringLength(Globals.MaxPublicKey)]
        public string? PublicKey { get; set; }
        [StringLength(Globals.MaxPrivateKey)]
        public string? PrivateKey { get; set; }

        [Data_Binary]
        public WeeklyHours OpeningHours { get; set; }

        public int MaxErrors { get; set; }

        public IVRConfig() {
            MaxErrors = 3;
            OpeningHours = WeeklyHours.WorkWeek;
            NotificationNumbers = new SerializableList<ExtensionPhoneNumber>();
        }
    }

    public class IVRConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public IVRConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public IVRConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, IVRConfig> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, IVRConfig>? CreateDataProvider() {
            Package package = Softelvdm.Modules.IVR.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Config", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public static async Task<IVRConfig> GetConfigAsync() {
            using (IVRConfigDataProvider configDP = new IVRConfigDataProvider()) {
                IVRConfig config = await configDP.GetItemAsync();
                if (config == null)
                    throw new InternalError("No IVR settings defined");
                return config;
            }
        }
        public static async Task<IVRConfig> GetConfigCondAsync() {
            using (IVRConfigDataProvider configDP = new IVRConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<IVRConfig> GetItemAsync() {
            IVRConfig? config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                config = new IVRConfig();

                string publicKey, privateKey;
                RSACrypto.MakeNewKeys(out publicKey, out privateKey);
                config.PrivateKey = privateKey;
                config.PublicKey = publicKey;

                await AddConfigAsync(config);
            }
            return config;
        }
        private async Task AddConfigAsync(IVRConfig data) {
            data.Id = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding settings");
            await Auditing.AddAuditAsync($"{nameof(IVRConfigDataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
                "Add Softelvdm_IVR Config",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
        public async Task UpdateConfigAsync(IVRConfig data) {
            IVRConfig? origConfig = Auditing.Active ? await GetItemAsync() : null;
            data.Id = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
            await Auditing.AddAuditAsync($"{nameof(IVRConfigDataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
                "Update Softelvdm_IVR Config",
                DataBefore: origConfig,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
    }
}
