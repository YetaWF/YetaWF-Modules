/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Models {

    /// <summary>
    /// There is one UserStore per site, stored using PermanentManager
    /// </summary>

    public class UserStore :
            IUserStore<UserDefinition>,
            IUserLoginStore<UserDefinition>,
            IUserPasswordStore<UserDefinition>,
            IUserRoleStore<UserDefinition>
    {

        static object _lockObject = new object();

        public UserStore(int siteIdentity, float f) { /* THIS float IS TO REMIND ME SO I DON'T ACCIDENTALLY ALLOCATE A NEW ONE */
            CurrentSiteIdentity = siteIdentity;
        }
        public int CurrentSiteIdentity { get; private set; }

        public void Dispose() { Dispose(true); }
        protected virtual void Dispose(bool disposing) { }
        //~UserStore() { Dispose(false); }

        // IUserStore
        // IUserStore
        // IUserStore

        public System.Threading.Tasks.Task CreateAsync(UserDefinition user) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                bool status = dataProvider.AddItem(user);
                if (!status)
                    throw new Error(this.__ResStr("userExists", "User {0} already exists.", user.UserName));
                return System.Threading.Tasks.Task.FromResult(0);
            }
        }

        public System.Threading.Tasks.Task DeleteAsync(UserDefinition user) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                bool status = dataProvider.RemoveItem(user.UserName);
                if (!status)
                    throw new Error(this.__ResStr("userNotFound", "User {0} not found.", user.UserName));
                return System.Threading.Tasks.Task.FromResult(0);
            }
        }

        public System.Threading.Tasks.Task<UserDefinition> FindByIdAsync(string userId) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                UserDefinition user = dataProvider.GetItemByUserId(Convert.ToInt32(userId));
                return System.Threading.Tasks.Task.FromResult(user);
            }
        }

        public System.Threading.Tasks.Task<UserDefinition> FindByNameAsync(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                UserDefinition user = dataProvider.GetItem(userName);
                return System.Threading.Tasks.Task.FromResult(user);
            }
        }

        public System.Threading.Tasks.Task UpdateAsync(UserDefinition user) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                UpdateStatusEnum status = dataProvider.UpdateItem(user);
                switch (status) {
                    default:
                    case UpdateStatusEnum.NewKeyExists:
                        throw new InternalError("Unexpected update status");
                    case UpdateStatusEnum.RecordDeleted:
                        throw new Error(this.__ResStr("userDeleted", "User {0} not found.", user.UserName));
                    case UpdateStatusEnum.OK:
                        break;
                }
                return System.Threading.Tasks.Task.FromResult(0);
            }
        }

        // IUserLoginStore
        // IUserLoginStore
        // IUserLoginStore

        public System.Threading.Tasks.Task AddLoginAsync(UserDefinition user, UserLoginInfo login) {

            using (UserLoginInfoDataProvider logInfoDP = new DataProvider.UserLoginInfoDataProvider()) {
                logInfoDP.AddItem(user.UserId, login.LoginProvider, login.ProviderKey);
            }

            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                UpdateStatusEnum status = dataProvider.UpdateItem(user);
                switch (status) {
                    case UpdateStatusEnum.RecordDeleted:
                        throw new Error(this.__ResStr("delUser", "Can't update user {0}, because the user has been deleted.", user.UserName));
                    default:
                    case UpdateStatusEnum.NewKeyExists:
                        throw new InternalError("Unexpected update status");
                    case UpdateStatusEnum.OK:
                        break;
                }
                return System.Threading.Tasks.Task.FromResult(0);
            }
        }

        public System.Threading.Tasks.Task<UserDefinition> FindAsync(UserLoginInfo login) {
            using (UserLoginInfoDataProvider logInfoDP = new DataProvider.UserLoginInfoDataProvider(CurrentSiteIdentity)) {
                UserDefinition user = logInfoDP.GetItem(login.LoginProvider, login.ProviderKey);
                return System.Threading.Tasks.Task.FromResult(user);
            }
        }

        public System.Threading.Tasks.Task<IList<UserLoginInfo>> GetLoginsAsync(UserDefinition user) {
            throw new NotImplementedException();
        }

        public System.Threading.Tasks.Task RemoveLoginAsync(UserDefinition user, UserLoginInfo login) {
            using (UserLoginInfoDataProvider logInfoDP = new DataProvider.UserLoginInfoDataProvider(CurrentSiteIdentity)) {
                logInfoDP.RemoveItem(login.LoginProvider, login.ProviderKey);
                return System.Threading.Tasks.Task.FromResult(0);
            }
        }

        // IUserPasswordStore
        // IUserPasswordStore
        // IUserPasswordStore

        public System.Threading.Tasks.Task<string> GetPasswordHashAsync(UserDefinition user) {
            return System.Threading.Tasks.Task.FromResult(user.PasswordHash);
        }

        public System.Threading.Tasks.Task<bool> HasPasswordAsync(UserDefinition user) {
            return System.Threading.Tasks.Task.FromResult(user.PasswordHash != null);
        }

        public System.Threading.Tasks.Task SetPasswordHashAsync(UserDefinition user, string passwordHash) {
            user.PasswordHash = passwordHash;
            return System.Threading.Tasks.Task.FromResult(0);
        }

        // IUserRoleStore
        // IUserRoleStore
        // IUserRoleStore

        public System.Threading.Tasks.Task AddToRoleAsync(UserDefinition user, string role) {
            int roleId = Convert.ToInt32(role);
            user.RolesList.Add(new Role() { RoleId = roleId });
            return System.Threading.Tasks.Task.FromResult(0);
        }

        public System.Threading.Tasks.Task<IList<string>> GetRolesAsync(UserDefinition user) {
            return System.Threading.Tasks.Task.FromResult((IList<string>) (from Role role in user.RolesList select role.RoleId.ToString()).ToList<string>());
        }

        public System.Threading.Tasks.Task<bool> IsInRoleAsync(UserDefinition user, string roleString) {
            int roleId = Convert.ToInt32(roleString);
            Role role = (from Role r in user.RolesList where r.RoleId == roleId select r).FirstOrDefault();
            return System.Threading.Tasks.Task.FromResult<bool>(role != null);
        }

        public System.Threading.Tasks.Task RemoveFromRoleAsync(UserDefinition user, string roleString) {
            int roleId = Convert.ToInt32(roleString);
            Role role = (from Role r in user.RolesList where r.RoleId == roleId select r).FirstOrDefault();
            if (role != null)
                user.RolesList.Remove(role);
            return System.Threading.Tasks.Task.FromResult(0);
        }
    }
}