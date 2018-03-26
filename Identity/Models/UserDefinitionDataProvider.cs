/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using YetaWF.Core.Support.TwoStepAuthorization;
using YetaWF.Modules.Identity.Models;
using YetaWF.Core.Audit;
#if MVC6
using Microsoft.AspNetCore.Identity;
#else
using Microsoft.AspNet.Identity;
#endif

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

#if MVC6
    public class UserDefinition
#else
    public class UserDefinition : IUser
#endif
    {

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

        [Data_NewValue("(0)")]
        public int LoginFailures { get; set; }

        [StringLength(60)] // max length is really a guid, leave some extra
        public string SecurityStamp { get; set; }

        public SerializableList<Role> RolesList { get; set; } // role ids for this user
        public SerializableList<TwoStepDefinition> EnabledTwoStepAuthentications { get; set; }

        public async Task<List<string>> GetEnabledAndAvailableTwoStepAuthenticationsAsync() {
            TwoStepAuth twoStep = new TwoStepAuth();
            List<ITwoStepAuth> list = await twoStep.GetTwoStepAuthProcessorsAsync();
            List<string> procs = (from p in list select p.Name).ToList();
            List<string> enabledTwoStepAuths = (from e in EnabledTwoStepAuthentications select e.Name).ToList();
            procs = procs.Intersect(enabledTwoStepAuths).ToList();
            return procs;
        }

        public UserDefinition() {
            RolesList = new SerializableList<Role>();
            Created = DateTime.UtcNow;
            UserStatus = UserStatusEnum.NeedValidation;
            VerificationCode = Guid.NewGuid().ToString();
            EnabledTwoStepAuthentications = new SerializableList<TwoStepDefinition>();
        }
    }

    public class TwoStepDefinition {
        public const int MaxName = 80;
        public TwoStepDefinition() { }
        [StringLength(MaxName)]
        public string Name { get; set; }
    }
    public class TwoStepDefinitionComparer : IEqualityComparer<TwoStepDefinition> {
        public bool Equals(TwoStepDefinition x, TwoStepDefinition y) {
            return x.Name == y.Name && x.Name == y.Name;
        }
        public int GetHashCode(TwoStepDefinition x) {
            return x.Name.GetHashCode() + x.Name.GetHashCode();
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

        public UserDefinitionDataProvider() : base(YetaWFManager.Manager.CurrentSite.Identity) { SetDataProvider(CreateDataProvider()); }
        public UserDefinitionDataProvider(int siteIdentity) : base(siteIdentity) { SetDataProvider(CreateDataProvider()); }

        private IDataProvider<string, UserDefinition> DataProvider { get { return GetDataProvider(); } }

        private IDataProvider<string, UserDefinition> CreateDataProvider() {
            Package package = YetaWF.Modules.Identity.Controllers.AreaRegistration.CurrentPackage;
            return MakeDataProvider(package, package.AreaName + "_Users", SiteIdentity: SiteIdentity, Cacheable: true, Parms: new { NoLanguages = true});
        }

        // API
        // API
        // API

        public async Task<UserDefinition> GetItemAsync(int userId) {
            List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "UserId", Operator = "==", Value = userId });
            return await GetItemAsync(filters);
        }
        public async Task<UserDefinition> GetItemAsync(string userName) {
            if (string.Compare(userName, SuperuserDefinitionDataProvider.SuperUserName, true) == 0) {
                using (SuperuserDefinitionDataProvider superDP = new SuperuserDefinitionDataProvider()) {
                    return await superDP.GetSuperuserAsync();
                }
            }
            return await DataProvider.GetAsync(userName);
        }
        public async Task<UserDefinition> GetItemAsync(List<DataProviderFilterInfo> filters) {
            UserDefinition user = await DataProvider.GetOneRecordAsync(filters);
            if (user != null)
                return user;
            using (SuperuserDefinitionDataProvider superDP = new SuperuserDefinitionDataProvider()) {
                return await superDP.GetItemAsync(filters);
            }
        }
        public async Task<UserDefinition> GetItemByUserIdAsync(int id) { return await GetItemAsync(id); }
        public async Task<UserDefinition> GetItemByEmailAsync(string email) {
            List<DataProviderFilterInfo> filters = DataProviderFilterInfo.Join(null, new DataProviderFilterInfo { Field = "Email", Operator = "==", Value = email });
            return await GetItemAsync(filters);
        }

        public async Task<bool> AddItemAsync(UserDefinition data) {
            CleanupRoles(data);
            if (data.UserId == SuperuserDefinitionDataProvider.SuperUserId || string.Compare(data.UserName, SuperuserDefinitionDataProvider.SuperUserName, true) == 0) {
                using (SuperuserDefinitionDataProvider superDP = new SuperuserDefinitionDataProvider()) {
                    return await superDP.AddItemAsync(data);
                }
            }
            bool result = await DataProvider.AddAsync(data);
            await Auditing.AddAuditAsync($"{nameof(UserDefinitionDataProvider)}.{nameof(AddItemAsync)}", data.UserName, Guid.Empty,
                "Add User",
                DataBefore: null,
                DataAfter: data
            );
            return result;
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(UserDefinition data) {
            CleanupRoles(data);
            if (data.UserId == SuperuserDefinitionDataProvider.SuperUserId || string.Compare(data.UserName, SuperuserDefinitionDataProvider.SuperUserName, true) == 0) {
                using (SuperuserDefinitionDataProvider superDP = new SuperuserDefinitionDataProvider()) {
                    return await superDP.UpdateItemAsync(data);
                }
            }
            return await UpdateItemAsync(data.UserName, data);
        }
        public async Task<UpdateStatusEnum> UpdateItemAsync(string originalName, UserDefinition data) {
            CleanupRoles(data);
            if (string.Compare(data.UserName, SuperuserDefinitionDataProvider.SuperUserName, true) == 0 &&
                    string.Compare(originalName, SuperuserDefinitionDataProvider.SuperUserName, true) != 0)
                return UpdateStatusEnum.NewKeyExists;
            if (string.Compare(originalName, SuperuserDefinitionDataProvider.SuperUserName, true) == 0) {
                if (data.UserName != originalName)
                    throw new Error(this.__ResStr("cantRenameSuper", "The user \"{0}\" can't be renamed. It is defined in the site's Appsettings.json", originalName));
                using (SuperuserDefinitionDataProvider superDP = new SuperuserDefinitionDataProvider()) {
                    return await superDP.UpdateItemAsync(data);
                }
            }
            UserDefinition origUser = Auditing.Active ? await GetItemAsync(originalName) : null;
            UpdateStatusEnum result = await DataProvider.UpdateAsync(originalName, data.UserName, data);
            await Auditing.AddAuditAsync($"{nameof(UserDefinitionDataProvider)}.{nameof(UpdateItemAsync)}", originalName, Guid.Empty,
                "Update User",
                DataBefore: origUser,
                DataAfter: data
            );
            return result;
        }
        public async Task<bool> RemoveItemAsync(string userName) {
            UserDefinition user = await GetItemAsync(userName);
            if (user == null)
                return false;
            using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider(SiteIdentity)) {
                await logInfoDP.RemoveItemAsync(user.UserId);
            }
            if (string.Compare(userName, SuperuserDefinitionDataProvider.SuperUserName, true) == 0) {
                using (SuperuserDefinitionDataProvider superDP = new SuperuserDefinitionDataProvider()) {
                    if (!superDP.RemoveItem(userName))
                        return false;
                }
            } else {
                if (!await DataProvider.RemoveAsync(userName))
                    return false;
            }
            // remove any data stored for this user from packages (if it fails, whatevz)
            await User.RemoveDependentPackagesAsync(user.UserId);
            await Auditing.AddAuditAsync($"{nameof(UserDefinitionDataProvider)}.{nameof(RemoveItemAsync)}", userName, Guid.Empty,
                "Remove User",
                DataBefore: user,
                DataAfter: null
            );
            return true;
        }
        public async Task<DataProviderGetRecords<UserDefinition>> GetItemsAsync(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) {
            UserDefinition superuser = null;
            int origSkip = skip, origTake = take;
            List<UserDefinition> users;

            using (SuperuserDefinitionDataProvider superDP = new SuperuserDefinitionDataProvider()) {
                superuser = await superDP.GetItemAsync(filters);
            }

            DataProviderGetRecords<UserDefinition> recs = new DataProviderGetRecords<UserDefinition>();

            recs.Total = 0;
            if (superuser != null) {
                recs.Total = 1;
                if (skip > 0) {
                    superuser = null;
                    --skip;
                } else {
                    if (take > 0)
                        take--;
                }
            }

            int userTotal = 0;
            if (take == 0 && origTake > 0) {
                // we just need the total
                DataProviderGetRecords<UserDefinition> trecs = await DataProvider.GetRecordsAsync(0, 1, sort, filters);
                userTotal = trecs.Total;
                recs.Data = new List<UserDefinition>();
            } else {
                DataProviderGetRecords<UserDefinition> trecs = await DataProvider.GetRecordsAsync(skip, take, sort, filters);
                userTotal = trecs.Total;
                users = trecs.Data;
            }
            if (superuser != null)
                recs.Data.Insert(0, superuser);
            recs.Total += userTotal;
            return recs;
        }
        public async Task RehashAllPasswordsAsync() {
            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
            if (!config.SavePlainTextPassword)
                throw new InternalError("Rehashing all passwords is only available if plain text passwords are saved");
            UserManager<UserDefinition> userManager = Managers.GetUserManager();
            const int TAKE = 10;
            for (int skip = 0; ; skip += TAKE) {
                DataProviderGetRecords<UserDefinition> list = await GetItemsAsync(skip, TAKE, null, null);
                if (list.Data.Count == 0)
                    break;
                foreach (UserDefinition user in list.Data) {
                    if (!string.IsNullOrWhiteSpace(user.PasswordPlainText)) {
#if MVC6
                        IPasswordHasher<UserDefinition> passwordHasher = (IPasswordHasher<UserDefinition>) YetaWFManager.ServiceProvider.GetService(typeof(IPasswordHasher<UserDefinition>));
                        user.PasswordHash = passwordHasher.HashPassword(user, user.PasswordPlainText);
#else
                        user.PasswordHash = userManager.PasswordHasher.HashPassword(user.PasswordPlainText);
#endif
                        UpdateStatusEnum status = await UpdateItemAsync(user);
                        if (status != UpdateStatusEnum.OK)
                            throw new InternalError("Update failed - status {0} user id {1}", status, user.Id);
                    }
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
        public async Task AddAdministratorAsync(string name) {
            await DataProvider.AddAsync(GetAdministrator(name));
        }
        public async Task AddEditorAsync(string name) {
            await DataProvider.AddAsync(GetEditor(name));
        }
        public async Task AddUserAsync(string name) {
            await DataProvider.AddAsync(GetUser(name));
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
