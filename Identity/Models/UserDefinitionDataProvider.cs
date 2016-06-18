/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.Models;

namespace YetaWF.Modules.Identity.DataProvider {

    public enum UserStatusEnum {
        [EnumDescription("Needs Validation", "A new user account that has not yet been validated or approved")]
        NeedValidation = 0,
        [EnumDescription("Needs Approval", "A user account whose email address has been validated but still needs approval")]
        NeedApproval = 1,
        [EnumDescription("Approved User", "A user account that has been approved")]
        Approved = 2,
        [EnumDescription("Rejected User", "A user account that has been rejected")]
        Rejected = 20,
        [EnumDescription("Suspended User", "A user account that has been suspended")]
        Suspended = 21,
    }

    public class UserDefinition : IUser {

        public const int MaxVerificationCode = 100;
        public const int MaxComment = 1000;

        [Data_PrimaryKey, StringLength(Globals.MaxUser)]
        public string UserName { get; set; }
        [Data_Identity]
        public int UserId { get; set; } // our internal user id

        // asp.net id
        [DontSave]
        public string Id { get { return UserId.ToString(); } set { UserId = Convert.ToInt32(value); } }

        [StringLength(Globals.MaxPswd)]
        public string PasswordHash { get; set; }
        [StringLength(Globals.MaxPswd)]
        public string PasswordPlainText { get; set; }

        [Data_Index, StringLength(Globals.MaxEmail)]
        public string Email { get; set; }

        [StringLength(MaxComment)]
        public string Comment { get; set; }

        public UserStatusEnum UserStatus { get; set; }

        [StringLength(MaxVerificationCode)]
        public string VerificationCode { get; set; }
        [StringLength(Globals.MaxIP)]
        public string RegistrationIP { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastLoginDate { get; set; }
        [StringLength(Globals.MaxIP)]
        public string LastLoginIP { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        [StringLength(Globals.MaxIP)]
        public string PasswordChangeIP { get; set; }
        public DateTime? LastActivityDate { get; set; }
        [StringLength(Globals.MaxIP)]
        public string LastActivityIP { get; set; }

        public SerializableList<LoginInfo> LoginInfoList { get; set; } // external login providers
        public SerializableList<Role> RolesList { get; set; } // role ids for this user

        public UserDefinition() {
            RolesList = new SerializableList<Role>();
            LoginInfoList = new SerializableList<LoginInfo>();
            Created = DateTime.UtcNow;
            UserStatus = UserStatusEnum.NeedValidation;
            VerificationCode = Guid.NewGuid().ToString();
        }
    }

    public class LoginInfo {

        public const int MaxLoginProvider = 100;
        public const int MaxProviderKey = 100;

        public LoginInfo() { }
        [StringLength(MaxLoginProvider)]
        public string LoginProvider { get; set; }
        [StringLength(MaxProviderKey)]
        public string ProviderKey { get; set; }
    }
    public class LoginInfoComparer : IEqualityComparer<LoginInfo> {
        public bool Equals(LoginInfo x, LoginInfo y) {
            return x.LoginProvider == y.LoginProvider && x.ProviderKey == y.ProviderKey;
        }
        public int GetHashCode(LoginInfo x) {
            return x.ProviderKey.GetHashCode() + x.LoginProvider.GetHashCode();
        }
    }

    /// <summary>
    /// UserDefinitionDataProvider
    /// Users are separated by site
    /// File - A small set of users is expected - all users are preloaded so less than 20 is recommended
    /// SQL - No limit
    /// </summary>
    public class UserDefinitionDataProvider : DataProviderImpl, IInstallableModel {

        static UserDefinitionDataProvider() { }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        private static object _lockObject = new object();

        public UserDefinitionDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(DataProvider); }
        public UserDefinitionDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(DataProvider); }

        protected IDataProvider<string, UserDefinition> DataProvider {
            get {
                if (_dataProvider == null) {
                    switch (GetIOMode(AreaRegistration.CurrentPackage.AreaName + "_Users")) {
                        default:
                        case WebConfigHelper.IOModeEnum.File:
                            _dataProvider = new YetaWF.DataProvider.FileDataProvider<string, UserDefinition>(
                                Path.Combine(YetaWFManager.DataFolder, AreaName, SiteIdentity.ToString()),
                                CurrentSiteIdentity: SiteIdentity,
                                Cacheable: true);
                            break;
                        case WebConfigHelper.IOModeEnum.Sql:
                            _dataProvider = new YetaWF.DataProvider.SQLSimpleObjectDataProvider<string, UserDefinition>(AreaName, SQLDbo, SQLConn,
                                CurrentSiteIdentity: SiteIdentity,
                                NoLanguages: true,
                                Cacheable: true);
                            break;
                    }
                }
                return _dataProvider;
            }
        }
        private IDataProvider<string, UserDefinition> _dataProvider { get; set; }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public UserDefinition GetItem(int userId) {
            List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "UserId", Operator = "==", Value = userId });
            return GetItem(filters);
        }
        public UserDefinition GetItem(string userName) {
            if (userName == SuperuserDefinitionDataProvider.SuperUserName) {
                using (SuperuserDefinitionDataProvider superDP = new SuperuserDefinitionDataProvider()) {
                    return superDP.GetSuperuser();
                }
            }
            return DataProvider.Get(userName);
        }
        public UserDefinition GetItem(List<DataProviderFilterInfo> filters) {
            UserDefinition user = DataProvider.GetOneRecord(filters);
            if (user != null)
                return user;
            using (SuperuserDefinitionDataProvider superDP = new SuperuserDefinitionDataProvider()) {
                return superDP.GetItem(filters);
            }
        }
        public UserDefinition GetItemByUserId(int id) { return GetItem(id); }
        public UserDefinition GetItemByEmail(string email) {
            List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "Email", Operator = "==", Value = email });
            return GetItem(filters);
        }

        public bool AddItem(UserDefinition data) {
            CleanupRoles(data);
            if (data.UserId == SuperuserDefinitionDataProvider.SuperUserId || data.UserName == SuperuserDefinitionDataProvider.SuperUserName) {
                using (SuperuserDefinitionDataProvider superDP = new SuperuserDefinitionDataProvider()) {
                    return superDP.AddItem(data);
                }
            }
            return DataProvider.Add(data);
        }
        public UpdateStatusEnum UpdateItem(UserDefinition data) {
            CleanupRoles(data);
            if (data.UserId == SuperuserDefinitionDataProvider.SuperUserId || data.UserName == SuperuserDefinitionDataProvider.SuperUserName) {
                using (SuperuserDefinitionDataProvider superDP = new SuperuserDefinitionDataProvider()) {
                    return superDP.UpdateItem(data);
                }
            }
            return UpdateItem(data.UserName, data);
        }
        public UpdateStatusEnum UpdateItem(string originalName, UserDefinition data) {
            CleanupRoles(data);
            if (data.UserName == SuperuserDefinitionDataProvider.SuperUserName && originalName != SuperuserDefinitionDataProvider.SuperUserName)
                return UpdateStatusEnum.NewKeyExists;
            if (originalName == SuperuserDefinitionDataProvider.SuperUserName) {
                if (data.UserName != originalName)
                    throw new Error(this.__ResStr("cantRenameSuper", "The user \"{0}\" can't be renamed. It is defined in the site's web.config", originalName));
                using (SuperuserDefinitionDataProvider superDP = new SuperuserDefinitionDataProvider()) {
                    return superDP.UpdateItem(data);
                }
            }
            return DataProvider.Update(originalName, data.UserName, data);
        }
        public bool RemoveItem(string userName) {
            if (userName == SuperuserDefinitionDataProvider.SuperUserName) {
                using (SuperuserDefinitionDataProvider superDP = new SuperuserDefinitionDataProvider()) {
                    return superDP.RemoveItem(userName);
                }
            }
            return DataProvider.Remove(userName);
        }
        public List<UserDefinition> GetItems(List<DataProviderFilterInfo> filters) {
            int total;
            return GetItems(0, 0, null, filters, out total);
        }
        public List<UserDefinition> GetItems(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, out int total) {
            List<UserDefinition> users = DataProvider.GetRecords(skip, take, sort, filters, out total);
            using (SuperuserDefinitionDataProvider superDP = new SuperuserDefinitionDataProvider()) {
                UserDefinition superuser = superDP.GetItem(filters);
                if (superuser != null) {
                    users.Insert(0, superuser);
                    ++total;
                }
            }
            return users;
        }
        public int RemoveItems(List<DataProviderFilterInfo> filters) {
            return DataProvider.RemoveRecords(filters);
        }
        public void RehashAllPasswords() {
            LoginConfigData config = LoginConfigDataProvider.GetConfig();
            if (!config.SavePlainTextPassword)
                throw new InternalError("RehashAllPasswords is only available if plain text passwords are saved");
            UserManager<UserDefinition> userManager = Managers.GetUserManager();
            const int TAKE = 10;
            for (int skip = 0 ; ; skip += TAKE) {
                int total;
                List<UserDefinition> list = GetItems(skip, TAKE, null, null, out total);
                if (list.Count == 0)
                    break;
                foreach (UserDefinition user in list) {
                    user.PasswordHash = userManager.PasswordHasher.HashPassword(user.PasswordPlainText);
                    UpdateStatusEnum status = UpdateItem(user);
                    if (status != UpdateStatusEnum.OK)
                        throw new InternalError("Update failed - status {0} user id {1}", status, user.Id);
                }
            }
        }

        private void CleanupRoles(UserDefinition data) {
            if (data.RolesList == null) data.RolesList = new SerializableList<Role>();
            // remove User and Superuser from allowed roles as he/she's in there by default
            using (RoleDefinitionDataProvider roleDP = new RoleDefinitionDataProvider(SiteIdentity)) {
                int userRole = roleDP.GetUserRoleId();
                int superuserRole = SuperuserDefinitionDataProvider.SuperUserId;
                data.RolesList = new SerializableList<Role>((from r in data.RolesList where r.RoleId != userRole && r.RoleId != superuserRole select r).ToList());
            }
        }
        public void AddAdministrator(string name) {
            DataProvider.Add(GetAdministrator(name));
        }
        public void AddEditor(string name) {
            DataProvider.Add(GetEditor(name));
        }
        public void AddUser(string name) {
            DataProvider.Add(GetUser(name));
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
        public bool UninstallModel(List<string> errorList) {
            return DataProvider.UninstallModel(errorList);
        }
        public void AddSiteData() {
            DataProvider.AddSiteData();
        }
        public void RemoveSiteData() {
            DataProvider.RemoveSiteData();
        }
        public bool ExportChunk(int chunk, SerializableList<SerializableFile> fileList, out object obj) {
            return DataProvider.ExportChunk(chunk, fileList, out obj);
        }
        public void ImportChunk(int chunk, SerializableList<SerializableFile> fileList, object obj) {
            DataProvider.ImportChunk(chunk, fileList, obj);
        }
        private UserDefinition GetUser(string name) {
            using (RoleDefinitionDataProvider roleProvider = new RoleDefinitionDataProvider(SiteIdentity)) {
                int roleId = roleProvider.GetUserRoleId();
                return new UserDefinition() {
                    UserName = name,
                    RolesList = new SerializableList<Role>(),
                    UserStatus = UserStatusEnum.Approved,
                    Comment = this.__ResStr("user", "A sample user"),
                    Email = name + "@" + Manager.CurrentSite.SiteDomain,
                    RegistrationIP = "127.0.0.1",
                };
            }
        }
        private UserDefinition GetEditor(string name) {
            using (RoleDefinitionDataProvider roleProvider = new RoleDefinitionDataProvider(SiteIdentity)) {
                return new UserDefinition() {
                    UserName = name,
                    RolesList = new SerializableList<Role>() {
                        new Role() { RoleId = roleProvider.GetEditorRoleId() },
                    },
                    UserStatus = UserStatusEnum.Approved,
                    Comment = this.__ResStr("editor", "A sample site editor"),
                    Email = name + "@" + Manager.CurrentSite.SiteDomain,
                    RegistrationIP = "127.0.0.1",
                };
            }
        }
        private UserDefinition GetAdministrator(string name) {
            using (RoleDefinitionDataProvider roleProvider = new RoleDefinitionDataProvider(SiteIdentity)) {
                return new UserDefinition() {
                    UserName = name,
                    RolesList = new SerializableList<Role>() {
                        new Role() { RoleId = roleProvider.GetAdministratorRoleId() },
                    },
                    UserStatus = UserStatusEnum.Approved,
                    Comment = this.__ResStr("admin", "A sample administrator"),
                    Email = name + "@" + Manager.CurrentSite.SiteDomain,
                    RegistrationIP = "127.0.0.1",
                };
            }
        }
    }
}
