/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Messenger#License */

using System.Collections.Generic;
using System.IO;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;

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

        private static object _lockObject = new object();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public ConfigDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public ConfigDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<int, ConfigData> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<int, ConfigData> CreateDataProvider() {
            Package package = YetaWF.Modules.Messenger.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package.AreaName,
                () => { // File
                    return new FileDataProvider<int, ConfigData>(
                        Path.Combine(YetaWFManager.DataFolder, AreaName + "_Config", SiteIdentity.ToString()),
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                (dbo, conn) => {  // SQL
                    return new SQLSimpleObjectDataProvider<int, ConfigData>(AreaName + "_Config", dbo, conn,
                        CurrentSiteIdentity: SiteIdentity,
                        Cacheable: true);
                },
                () => { // External
                    return MakeExternalDataProvider(new { AreaName = AreaName + "_Config", CurrentSiteIdentity = SiteIdentity, Cacheable = true });
                }
            );
        }

        // API
        // API
        // API

        public static ConfigData GetConfig() {
            using (ConfigDataProvider configDP = new ConfigDataProvider()) {
                return configDP.GetItem();
            }
        }
        public ConfigData GetItem() {
            ConfigData config = DataProvider.Get(KEY);
            if (config == null) {
                lock (_lockObject) {
                    config = DataProvider.Get(KEY);
                    if (config == null) {
                        config = new ConfigData();
                        AddConfig(config);
                    }
                }
            }
            return config;
        }
        private void AddConfig(ConfigData data) {
            data.Id = KEY;
            if (!DataProvider.Add(data))
                throw new InternalError("Unexpected error adding settings");
        }
        public void UpdateConfig(ConfigData data) {
            data.Id = KEY;
            UpdateStatusEnum status = DataProvider.Update(data.Id, data.Id, data);
            if (status != UpdateStatusEnum.OK)
                throw new InternalError("Unexpected error saving configuration {0}", status);
        }
    }
}
