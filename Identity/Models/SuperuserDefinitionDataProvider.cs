/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Controllers;

namespace YetaWF.Modules.Identity.DataProvider {

    /// <summary>
    /// SuperuserDefinitionDataProvider
    /// The superuser is common to all sites - only ONE is supported (how many security holes do you really need?)
    /// </summary>
    public class SuperuserDefinitionDataProvider : DataProviderImpl, IInstallableModel {

        public static readonly int SuperUserId = 1;

        private static readonly string SUPERUSERNAME = "SuperUserName";

        static SuperuserDefinitionDataProvider() { }

        internal static string SuperUserName {
            get {
                if (string.IsNullOrWhiteSpace(_super))
                    _super = WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, SUPERUSERNAME);
                if (!string.IsNullOrWhiteSpace(_super))
                    return _super;
                return SUPERUSERNAME;
            }
        }
        private static string _super;

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        private static object _lockObject = new object();

        public SuperuserDefinitionDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<string, UserDefinition> DataProvider { get { return GetDataProvider(); } }

        protected IDataProvider<string, UserDefinition> CreateDataProvider() {
            Package package = YetaWF.Modules.Identity.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider2(package, package.AreaName + "_Superusers", Cacheable: true, Parms: new { IdentitySeed = SuperUserId, NoLanguages = true });
        }

        // API
        // API
        // API

        public UserDefinition GetSuperuser() {
            List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "UserId", Operator = "==", Value = SuperuserDefinitionDataProvider.SuperUserId });
            return GetItem(filters);
        }
        public UserDefinition GetItem(List<DataProviderFilterInfo> filters) {
            UserDefinition user = DataProvider.GetOneRecord(filters);
            if (user == null) return null;
            user.UserName = SuperUserName;
            user.RolesList.Add(new Role { RoleId = Resource.ResourceAccess.GetSuperuserRoleId() });
            return user;
        }
        public bool AddItem(UserDefinition user) {
            if (user.UserId != SuperuserDefinitionDataProvider.SuperUserId || string.Compare(user.UserName, SuperUserName, true) != 0)
                throw new Error(this.__ResStr("cantAddSuper", "Wrong user id or user name - Can't add as superuser"));
            user.RolesList = new SerializableList<Role> { new Role { RoleId = Resource.ResourceAccess.GetSuperuserRoleId() } };
            return DataProvider.Add(user);
        }
        public UpdateStatusEnum UpdateItem(UserDefinition user) {
            if (user.UserId != SuperuserDefinitionDataProvider.SuperUserId || string.Compare(user.UserName, SuperUserName, true) != 0)
                throw new Error(this.__ResStr("cantUpdateSuper", "Wrong user id or user name - Can't update as superuser"));
            user.RolesList = new SerializableList<Role> { new Role { RoleId = Resource.ResourceAccess.GetSuperuserRoleId() } };
            return UpdateItem(user.UserName, user);
        }
        public UpdateStatusEnum UpdateItem(string originalName, UserDefinition data) {
            if (string.Compare(originalName, SuperUserName, true) == 0) {
                if (data.UserName != originalName)
                    throw new Error(this.__ResStr("cantRenameSuper", "The user \"{0}\" can't be renamed. It is defined in the site's Appsettings.json", data.UserName));
                // we allow status change even for a superuser (mainly to support login failures with automatic suspension)
                //if (data.UserStatus != UserStatusEnum.Approved)
                //    throw new Error(this.__ResStr("cantChangeStatusSuper", "The user \"{0}\" must remain an approved user. That's the only one that can bail you out when the entire site is broken.", data.UserName));
            }
            if (data.UserId != SuperuserDefinitionDataProvider.SuperUserId || string.Compare(data.UserName, SuperUserName, true) != 0)
                throw new Error(this.__ResStr("cantUpdateSuper", "Wrong user id or user name - Can't update as superuser"));
            lock (_lockObject) {
                UserDefinition superUser;// need to get current superuser because user may have changed the name through Appsettings.json
                List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "UserId", Operator = "==", Value = SuperuserDefinitionDataProvider.SuperUserId });
                superUser = DataProvider.GetOneRecord(filters);
                superUser.RolesList = new SerializableList<Role> { new Role { RoleId = Resource.ResourceAccess.GetSuperuserRoleId() } };
                return DataProvider.Update(superUser.UserName, data.UserName, data);
            }
        }
        public bool RemoveItem(string userName) {
            throw new Error(this.__ResStr("cantRemoveSuper", "The user with role \"{0}\" can't be removed. Who else is going to bail you out once you mess up your website?", Globals.Role_Superuser));
        }

        public void AddSuperuser() {
            DataProvider.Add(GetSuperuserUser());
        }
        private UserDefinition GetSuperuserUser() {
            using (RoleDefinitionDataProvider roleProvider = new RoleDefinitionDataProvider(SiteIdentity)) {
                RoleDefinition role = roleProvider.GetItem(Globals.Role_Superuser);
                return new UserDefinition() {
                    UserName = SuperUserName,
                    RolesList = new SerializableList<Role>() { new Role() { RoleId = role.RoleId } },
                    UserStatus = UserStatusEnum.Approved,
                    Comment = this.__ResStr("super", "The superuser for all sites"),
                    Email = SuperUserName + "@" + Manager.CurrentSite.SiteDomain,
                    RegistrationIP = "127.0.0.1",
                };
            }
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public new bool InstallModel(List<string> errorList) {
            if (!DataProvider.InstallModel(errorList))
                return false;
            // add the one and only superuser
            DataProvider.Add(GetSuperuserUser());
            return true;
        }
        public new void AddSiteData() { }
        public new void RemoveSiteData() { }
    }
}
