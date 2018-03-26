/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Messenger.DataProvider {

    public enum MessageAcceptanceEnum {
        [EnumDescription("Always", "Messaging is permitted between users/roles without requesting messaging permission")]
        Always = 0,
        [EnumDescription("Accept First", "Messaging is permitted between users/roles as long as the recipient has accepted to receive messages from the sender")]
        AcceptFirst = 1,
    }

    public class ConfigData {

        [Data_PrimaryKey]
        public int Id { get; set; }

        public bool SameRoleToSameRole { get; set; }
        [Data_Binary]
        public SerializableList<Role> AllowedSameRoles { get; set; }

        public MessageAcceptanceEnum MessageAcceptance { get; set; }

        [Data_Binary]
        public SerializableList<Role> AllowedRolesToAllUsers { get; set; }

        [Data_Binary]
        public SerializableList<Role> AllUsersToAllowedRoles { get; set; }

        public ConfigData() {
            SameRoleToSameRole = false;
            AllowedSameRoles = new SerializableList<Role>();
            MessageAcceptance = MessageAcceptanceEnum.Always;
            AllowedRolesToAllUsers = new SerializableList<Role>();
            AllUsersToAllowedRoles = new SerializableList<Role>();
        }
    }

    public class ConfigDataProvider : DataProviderImpl, IInstallableModel {

        private const int KEY = 1;

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public ConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public ConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, ConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, ConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.Messenger.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Config", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public static async Task<ConfigData> GetConfigAsync() {
            using (ConfigDataProvider configDP = new ConfigDataProvider()) {
                return await configDP.GetItemAsync();
            }
        }
        public async Task<ConfigData> GetItemAsync() {
            ConfigData config = await DataProvider.GetAsync(KEY);
            if (config == null) {
                config = new ConfigData();
                await AddConfigAsync(config);
            }
            return config;
        }
        private async Task AddConfigAsync(ConfigData data) {
            data.Id = KEY;
            if (!await DataProvider.AddAsync(data))
                throw new InternalError("Unexpected error adding settings");
            await Auditing.AddAuditAsync($"{nameof(ConfigDataProvider)}.{nameof(AddConfigAsync)}", "Config", Guid.Empty,
                "Add Messenger Config",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
        public async Task UpdateConfigAsync(ConfigData data) {
            ConfigData origConfig = Auditing.Active ? await GetItemAsync() : null;
            data.Id = KEY;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
            await Auditing.AddAuditAsync($"{nameof(ConfigDataProvider)}.{nameof(UpdateConfigAsync)}", "Config", Guid.Empty,
                "Update Messenger Config",
                DataBefore: origConfig,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
        }
    }
}
