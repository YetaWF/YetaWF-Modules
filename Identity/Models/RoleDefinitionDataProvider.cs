/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Audit;
using YetaWF.Core.DataProvider;
using YetaWF.Core.DataProvider.Attributes;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
#if MVC6
#else
using Microsoft.AspNet.Identity;
#endif

namespace YetaWF.Modules.Identity.DataProvider {

#if MVC6
    public class RoleDefinition
#else
    public class RoleDefinition : IRole
#endif
    {

        public const int MaxName = 100;
        public const int MaxDescription = 200;

        [Data_PrimaryKey, StringLength(MaxName)]
        public string Name { get; set; }
        [Data_Identity]
        public int RoleId { get; set; } // our internal role id

        // .net id
        [DontSave]
        public string Id { get { return RoleId.ToString(); } set { RoleId = Convert.ToInt32(value); } }

        [StringLength(MaxDescription)]
        public string Description { get; set; }
        [StringLength(Globals.MaxUrl)]
        [Data_NewValue]
        public string PostLoginUrl { get; set; }

        public RoleDefinition() { }
    }

    /// <summary>
    /// RoleDefinitionDataProvider
    /// Roles are separated by site
    /// File,SQL - A small set of roles is expected, so they're preloaded - If a large # > 100 is expected, this must be rewritten
    /// </summary>
    public class RoleDefinitionDataProvider : DataProviderImpl, IInstallableModel {

        private static object _lockObject = new object();

        private static int UserDemoIdentity = DataProviderImpl.IDENTITY_SEED - 5;
        private static int User2FAIdentity = DataProviderImpl.IDENTITY_SEED - 4;
        public static int SuperUserId = DataProviderImpl.IDENTITY_SEED - 3;
        private static int UserIdentity = DataProviderImpl.IDENTITY_SEED - 2;
        private static int AnonymousIdentity = DataProviderImpl.IDENTITY_SEED - 1;

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public RoleDefinitionDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public RoleDefinitionDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<string, RoleDefinition> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<string, RoleDefinition> CreateDataProvider() {
            Package package = YetaWF.Modules.Identity.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Roles", SiteIdentity: SiteIdentity, Cacheable: true);
        }

        // API
        // API
        // API

        public async Task<RoleDefinition> GetItemAsync(string key) {
            if (key == Globals.Role_Superuser)
                return MakeSuperuserRole();
            return await DataProvider.GetAsync(key);
        }
        public RoleDefinition GetRoleById(string roleId) {
            if (roleId == RoleDefinitionDataProvider.SuperUserId.ToString())
                return MakeSuperuserRole();
            List<RoleDefinition> roles = GetAllRoles();
            return (from RoleDefinition r in roles where r.Id == roleId select r).FirstOrDefault();
        }
        public RoleDefinition GetRoleById(int roleId) {
            if (roleId == RoleDefinitionDataProvider.SuperUserId)
                return MakeSuperuserRole();
            List<RoleDefinition> roles = GetAllRoles();
            return (from RoleDefinition r in roles where r.RoleId == roleId select r).FirstOrDefault();
        }
        public async Task<bool> AddItemAsync(RoleDefinition data) {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Adding new models is not possible when distributed caching is enabled");
            if (data.RoleId == RoleDefinitionDataProvider.SuperUserId || data.Name == Globals.Role_Superuser)
                throw new Error(this.__ResStr("cantAddSuper", "Can't add built-in superuser role"));
            if (!await DataProvider.AddAsync(data))
                return false;
            await GetAllUserRolesAsync(true);
            await Auditing.AddAuditAsync($"{nameof(RoleDefinitionDataProvider)}.{nameof(AddItemAsync)}", data.Name, Guid.Empty,
                "Add Role",
                DataBefore: null,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
            return true;
        }
        public Task<UpdateStatusEnum> UpdateItemAsync(RoleDefinition data) {
            return UpdateItemAsync(data.Name, data);
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(string originalRole, RoleDefinition data) {
            if (data.RoleId == RoleDefinitionDataProvider.SuperUserId || data.Name == Globals.Role_Superuser)
                throw new Error(this.__ResStr("cantUpdateSuper", "Can't update built-in superuser role"));
            if (originalRole != data.Name && IsPredefinedRole(originalRole))
                throw new Error(this.__ResStr("cantUpdateUser", "The {0} role can't be updated", originalRole));
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Changing roles is not possible when distributed caching is enabled");
            RoleDefinition origRole = Auditing.Active ? await GetItemAsync(originalRole) : null;
            UpdateStatusEnum status = await DataProvider.UpdateAsync(originalRole, data.Name, data);
            if (status == UpdateStatusEnum.OK)
                await GetAllUserRolesAsync(true);
            await Auditing.AddAuditAsync($"{nameof(RoleDefinitionDataProvider)}.{nameof(UpdateItemAsync)}", originalRole, Guid.Empty,
                "Update Role",
                DataBefore: origRole,
                DataAfter: data,
                ExpensiveMultiInstance: true
            );
            return status;
        }
        public async Task<bool> RemoveItemAsync(string role) {
            if (role == Globals.Role_Superuser)
                throw new Error(this.__ResStr("cantRemoveSuper", "Can't remove built-in superuser role"));
            if (IsPredefinedRole(role))
                throw new Error(this.__ResStr("cantRemoveUser", "The {0} role can't be removed", role));
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Removing roles is not possible when distributed caching is enabled");
            RoleDefinition origRole = Auditing.Active ? await GetItemAsync(role) : null;
            if (!await DataProvider.RemoveAsync(role))
                return false;
            await GetAllUserRolesAsync(true);
            await Auditing.AddAuditAsync($"{nameof(RoleDefinitionDataProvider)}.{nameof(RemoveItemAsync)}", role, Guid.Empty,
                "Remove Role",
                DataBefore: origRole,
                DataAfter: null,
                ExpensiveMultiInstance: true
            );
            return true;
        }
        public async Task<DataProviderGetRecords<RoleDefinition>> GetItemsAsync() {
            DataProviderGetRecords<RoleDefinition> list = await DataProvider.GetRecordsAsync(0, 0, null, null);
            list.Data.Insert(0, MakeSuperuserRole());
            return list;
        }
        public async Task<DataProviderGetRecords<RoleDefinition>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            DataProviderGetRecords<RoleDefinition> list = await DataProvider.GetRecordsAsync(skip, take, sort, filters);
            list.Data.Insert(0, MakeSuperuserRole());
            return list;
        }
        public async Task<int> RemoveItemsAsync(List<DataProviderFilterInfo> filters) {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Removing roles is not possible when distributed caching is enabled");
            int count = await DataProvider.RemoveRecordsAsync(filters);
            await GetAllUserRolesAsync(true);
            return count;
        }
        public int GetAdministratorRoleId() { return GetRoleId(Globals.Role_Administrator); }
        public int GetEditorRoleId() { return GetRoleId(Globals.Role_Editor); }
        public int GetUserRoleId() { return UserIdentity; }
        public int GetUser2FARoleId() { return User2FAIdentity; }
        public int GetUserDemoRoleId() { return UserDemoIdentity; }
        public int GetAnonymousRoleId() { return AnonymousIdentity; }
        public int GetRoleId(string roleName) {
            if (roleName == Globals.Role_Superuser)
                return RoleDefinitionDataProvider.SuperUserId;
            List<RoleDefinition> roles = GetAllRoles();
            RoleDefinition role = (from RoleDefinition r in roles where r.Name == roleName select r).FirstOrDefault();
            if (role == null) throw new InternalError($"Required role {roleName} not found");
            return role.RoleId;
        }
        public bool IsPredefinedRole(string role) {
            return (role == Globals.Role_Superuser || role == Globals.Role_User || role == Globals.Role_UserDemo || role == Globals.Role_User2FA || role == Globals.Role_Editor || role == Globals.Role_Anonymous || role == Globals.Role_Administrator);
        }

        /// <summary>
        /// Returns a list of all roles, including User and Anonymous.
        /// </summary>
        /// <param name="force">Force reload ignorin g cached data if true. Otherwise the cached information is returned.</param>
        /// <returns></returns>
        /// <remarks>
        /// This method is cached and deliberately does not use async/await to simplify usage.
        /// </remarks>
        public List<RoleDefinition> GetAllRoles() {
            List<RoleDefinition> roles = YetaWFManager.Syncify(async () => {
                return await GetAllUserRolesAsync();
            });
            roles = roles.ToList(); // copy
            roles.Add(MakeUserRole());
            roles.Add(MakeUserDemoRole());
            roles.Add(MakeUser2FARole());
            roles.Add(MakeAnonymousRole());
            return roles;
        }

        /// <summary>
        /// Retrieves list of all roles except User and Anonymous.
        /// </summary>
        public async Task<List<RoleDefinition>> GetAllUserRolesAsync(bool force = false) {

            using (ICacheDataProvider cacheDP = YetaWF.Core.IO.Caching.GetStaticSmallObjectCacheProvider()) {

                string ROLESKEY = $"__RoleDefinitions_{YetaWFManager.Manager.CurrentSite.Identity}";

                if (!force) {
                    GetObjectInfo<List<RoleDefinition>> cache = await cacheDP.GetAsync<List<RoleDefinition>>(ROLESKEY);
                    if (cache.Success)
                        return cache.RequiredData;
                }

                if (!await DataProvider.IsInstalledAsync())
                    return new List<RoleDefinition>() { MakeSuperuserRole() };

                // Load the roles
                DataProviderGetRecords<RoleDefinition> list = await GetItemsAsync();
                List<RoleDefinition> roles = list.Data;

                await cacheDP.AddAsync(ROLESKEY, roles);
                return roles;
            }
        }
        private RoleDefinition MakeSuperuserRole() {
            return new RoleDefinition() {
                RoleId = RoleDefinitionDataProvider.SuperUserId,
                Name = Globals.Role_Superuser,
                Description = this.__ResStr("superuserRole", "The {0} role can do EVERYTHING on all sites - no restrictions", Globals.Role_Superuser)
            };
        }
        private RoleDefinition MakeUserRole() {
            return new RoleDefinition() { RoleId = UserIdentity, Name = Globals.Role_User, Description = this.__ResStr("userRole", "The {0} role describes every authenticated user (i.e., not an anonymous user)", Globals.Role_User) };
        }
        private RoleDefinition MakeUserDemoRole() {
            return new RoleDefinition() { RoleId = UserDemoIdentity, Name = Globals.Role_UserDemo, Description = this.__ResStr("userDemoRole", "The {0} role describes an authenticated user that is limited to demo functionality", Globals.Role_UserDemo) };
        }
        private RoleDefinition MakeUser2FARole() {
            return new RoleDefinition() { RoleId = User2FAIdentity, Name = Globals.Role_User2FA, Description = this.__ResStr("user2faRole", "The {0} role describes every authenticated user that must set up two-step authentication (i.e., not an anonymous user)", Globals.Role_User2FA) };
        }
        private RoleDefinition MakeAnonymousRole() {
            return new RoleDefinition() { RoleId = AnonymousIdentity, Name = Globals.Role_Anonymous, Description = this.__ResStr("anonymousRole", "The {0} role describes every user that is not logged in (i.e., not an authenticated user)", Globals.Role_Anonymous) };
        }
        public async Task AddAdministratorRoleAsync() {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Adding roles is not possible when distributed caching is enabled");
            await DataProvider.AddAsync(
                new RoleDefinition() {
                    Name = Globals.Role_Administrator,
                    Description = this.__ResStr("adminRole", "An administrator can do EVERYTHING on ONE site (the site where the user has the {0} role)", Globals.Role_Administrator)
                }
            );
            await GetAllUserRolesAsync(true);// reload
        }
        public async Task AddEditorRoleAsync() {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Adding roles is not possible when distributed caching is enabled");
            await DataProvider.AddAsync(
                new RoleDefinition() {
                    Name = Globals.Role_Editor,
                    Description = this.__ResStr("editorRole", "An editor is a user who can perform editing actions on the site")
                }
            );
            await GetAllUserRolesAsync(true);// reload
        }

        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL
        // IINSTALLABLEMODEL

        public new async Task<bool> InstallModelAsync(List<string> errorList) {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Installing new models is not possible when distributed caching is enabled");
            if (!await DataProvider.InstallModelAsync(errorList))
                return false;
            await AddSiteDataAsync();
            return true;
        }
        public new async Task AddSiteDataAsync() {
            if (YetaWF.Core.Support.Startup.MultiInstance) throw new InternalError("Adding site data is not possible when distributed caching is enabled");
            await AddAdministratorRoleAsync();
            await AddEditorRoleAsync();
        }
    }
}
