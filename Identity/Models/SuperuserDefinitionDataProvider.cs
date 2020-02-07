/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Packages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Controllers;

namespace YetaWF.Modules.Identity.DataProvider {

    /// <summary>
    /// SuperuserDefinitionDataProvider
    /// The superuser is common to all sites - only ONE is supported (how many security holes do you really need?)
    /// </summary>
    public class SuperuserDefinitionDataProvider : DataProviderImpl, IInstallableModel, IInitializeApplicationStartup {

        public static readonly int SuperUserId = 1;

        private static readonly string SUPERUSERNAME = "SuperUserName";
        private static readonly string SUPERUSERPASSWORDRANDOM = "SuperUserPasswordRandom";

        public Task InitializeApplicationStartupAsync() {
            if (SuperUserPasswordRandom)
                Logging.AddWarningLog($"Emergency login \"{_superPswd.ToString()}\" (informational message)");
            return Task.CompletedTask;
        }

        static SuperuserDefinitionDataProvider() { }

        internal static string SuperUserName {
            get {
                if (SuperuserAvailable)
                    return _super;
                return SUPERUSERNAME;
            }
        }
        private static string _super = null;

        public static bool SuperuserAvailable {
            get {
                if (_super == null)
                    _super = WebConfigHelper.GetValue<string>(AreaRegistration.CurrentPackage.AreaName, SUPERUSERNAME, null);
                return !string.IsNullOrWhiteSpace(_super);
            }
        }

        internal static bool SuperUserPasswordRandom {
            get {
                if (_superPswdRandom == null) {
                    _superPswdRandom = WebConfigHelper.GetValue<bool>(AreaRegistration.CurrentPackage.AreaName, SUPERUSERPASSWORDRANDOM, false);
                }
                return (bool)_superPswdRandom;
            }
        }
        private static bool? _superPswdRandom = null;

        internal static string SuperUserPassword {
            get {
                if (SuperUserPasswordRandom) {
                    return _superPswd.ToString();
                }
                return null;
            }
        }
        private static Guid _superPswd = Guid.NewGuid();

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public SuperuserDefinitionDataProvider() : base(0) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<string, UserDefinition> DataProvider { get { return GetDataProvider(); } }

        protected IDataProvider<string, UserDefinition> CreateDataProvider() {
            Package package = YetaWF.Modules.Identity.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Superusers", Cacheable: true, Parms: new { IdentitySeed = SuperUserId, NoLanguages = true });
        }

        // API
        // API
        // API

        public async Task<UserDefinition> GetSuperuserAsync() {
            if (!SuperuserAvailable)
                return null;
            List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "UserId", Operator = "==", Value = SuperuserDefinitionDataProvider.SuperUserId });
            return await GetItemAsync(filters);
        }
        public async Task<UserDefinition> GetItemAsync(List<DataProviderFilterInfo> filters) {
            UserDefinition user = await DataProvider.GetOneRecordAsync(filters);
            if (user == null) return null;
            user.UserName = SuperUserName;
            user.RolesList.Add(new Role { RoleId = Resource.ResourceAccess.GetSuperuserRoleId() });
            return user;
        }
        public async Task<bool> AddItemAsync(UserDefinition user) {
            if (user.UserId != SuperuserDefinitionDataProvider.SuperUserId || string.Compare(user.UserName, SuperUserName, true) != 0)
                throw new Error(this.__ResStr("cantAddSuper", "Wrong user id or user name - Can't add as superuser"));
            user.RolesList = new SerializableList<Role> { new Role { RoleId = Resource.ResourceAccess.GetSuperuserRoleId() } };
            bool result = await DataProvider.AddAsync(user);
            await Auditing.AddAuditAsync($"{nameof(SuperuserDefinitionDataProvider)}.{nameof(AddItemAsync)}", user.UserName, Guid.Empty,
                "Add Superuser",
                DataBefore: null,
                DataAfter: user
            );
            return result;
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(UserDefinition user) {
            if (user.UserId != SuperuserDefinitionDataProvider.SuperUserId || string.Compare(user.UserName, SuperUserName, true) != 0)
                throw new Error(this.__ResStr("cantUpdateSuper", "Wrong user id or user name - Can't update as superuser"));
            user.RolesList = new SerializableList<Role> { new Role { RoleId = Resource.ResourceAccess.GetSuperuserRoleId() } };
            return await UpdateItemAsync(user.UserName, user);
        }
        private async Task<UpdateStatusEnum> UpdateItemAsync(string originalName, UserDefinition data) {
            if (string.Compare(originalName, SuperUserName, true) == 0) {
                if (data.UserName != originalName)
                    throw new Error(this.__ResStr("cantRenameSuper", "The user \"{0}\" can't be renamed.", data.UserName));
                // we allow status change even for a superuser (mainly to support login failures with automatic suspension)
                //if (data.UserStatus != UserStatusEnum.Approved)
                //    throw new Error(this.__ResStr("cantChangeStatusSuper", "The user \"{0}\" must remain an approved user. That's the only one that can bail you out when the entire site is broken.", data.UserName));
            }
            if (data.UserId != SuperuserDefinitionDataProvider.SuperUserId || string.Compare(data.UserName, SuperUserName, true) != 0)
                throw new Error(this.__ResStr("cantUpdateSuper", "Wrong user id or user name - Can't update as superuser"));
            UpdateStatusEnum result;
            UserDefinition origSuperuser;// need to get current superuser because user may have changed the name through Appsettings.json

            Package package = YetaWF.Modules.Identity.Controllers.AreaRegistration.CurrentPackage;
            using (ILockObject lockObject = await YetaWF.Core.IO.Caching.LockProvider.LockResourceAsync($"{package.AreaName}.{nameof(SuperuserDefinitionDataProvider)}_{originalName}")) {
                List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "UserId", Operator = "==", Value = SuperuserDefinitionDataProvider.SuperUserId });
                origSuperuser = await DataProvider.GetOneRecordAsync(filters);
                data.RolesList = new SerializableList<Role> { new Role { RoleId = Resource.ResourceAccess.GetSuperuserRoleId() } };
                result = await DataProvider.UpdateAsync(origSuperuser.UserName, data.UserName, data);
            }
            await Auditing.AddAuditAsync($"{nameof(SuperuserDefinitionDataProvider)}.{nameof(UpdateItemAsync)}", data.UserName, Guid.Empty,
                "Update Superuser",
                DataBefore: origSuperuser,
                DataAfter: data
            );
            return result;
        }
        public bool RemoveItem(string userName) {
            throw new Error(this.__ResStr("cantRemoveSuper", "The user with role \"{0}\" can't be removed. Who else is going to bail you out once you mess up your website?", Globals.Role_Superuser));
        }

        public async Task AddSuperuser() {
            await DataProvider.AddAsync(await GetSuperuserUserAsync());
        }
        private async Task<UserDefinition> GetSuperuserUserAsync() {
            using (RoleDefinitionDataProvider roleProvider = new RoleDefinitionDataProvider(SiteIdentity)) {
                RoleDefinition role = await roleProvider.GetItemAsync(Globals.Role_Superuser);
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

        public new async Task<bool> InstallModelAsync(List<string> errorList) {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Installing new models is not possible when distributed caching is enabled");
            if (!await DataProvider.InstallModelAsync(errorList))
                return false;
            // add the one and only superuser
            await DataProvider.AddAsync(await GetSuperuserUserAsync());
            return true;
        }
        public new Task AddSiteDataAsync() { return Task.CompletedTask; }
        public new Task RemoveSiteDataAsync() { return Task.CompletedTask; }
    }
}
