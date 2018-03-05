/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
#else
using Microsoft.AspNet.Identity;
#endif

namespace YetaWF.Modules.Identity.Models {

    /// <summary>
    /// There is one UserStore per site, stored using PermanentManager
    /// </summary>

#if MVC6
    public class YetaWFSecurityStampValidator : SecurityStampValidator<UserDefinition> {
        public YetaWFSecurityStampValidator(IOptions<SecurityStampValidatorOptions> options, SignInManager<UserDefinition> signInManager, ISystemClock clock) : base(options, signInManager, clock) { }
    }
#endif

    public class UserStore :
            IUserStore<UserDefinition>,
            IUserLoginStore<UserDefinition>,
            IUserPasswordStore<UserDefinition>,
#if MVC6
            IUserSecurityStampStore<UserDefinition>,
            IUserEmailStore<UserDefinition>,
            ISecurityStampValidator,
#else
#endif
            IUserRoleStore<UserDefinition>
    {

        static object _lockObject = new object();

#if MVC6
        public UserStore(){
            CurrentSiteIdentity = YetaWFManager.Manager.CurrentSite.Identity;
        }
#else
        public UserStore(int siteIdentity, float f) { /* THIS float IS TO REMIND ME SO I DON'T ACCIDENTALLY ALLOCATE A NEW ONE */
            CurrentSiteIdentity = siteIdentity;
        }
#endif
        public int CurrentSiteIdentity { get; private set; }

        public void Dispose() { Dispose(true); }
        protected virtual void Dispose(bool disposing) { }
        //~UserStore() { Dispose(false); }

        // IUserStore
        // IUserStore
        // IUserStore

#if MVC6
        //$$$ FIX ALL ASYNC
        public Task<IdentityResult> CreateAsync(UserDefinition user, CancellationToken cancellationToken) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                bool status = dataProvider.AddItem(user);
                if (!status)
                    throw new Error(this.__ResStr("userExists", "User {0} already exists.", user.UserName));
                return Task.FromResult<IdentityResult>(IdentityResult.Success);
            }
        }
#else
        public async Task CreateAsync(UserDefinition user) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                bool status = await dataProvider.AddItemAsync(user);
                if (!status)
                    throw new Error(this.__ResStr("userExists", "User {0} already exists.", user.UserName));
            }
        }
#endif
#if MVC6
        public Task<IdentityResult> DeleteAsync(UserDefinition user, CancellationToken cancellationToken) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                bool status = dataProvider.RemoveItem(user.UserName);
                if (!status)
                    throw new Error(this.__ResStr("userNotFound", "User {0} not found.", user.UserName));
                return Task.FromResult<IdentityResult>(IdentityResult.Success);
            }
        }
#else
        public async Task DeleteAsync(UserDefinition user) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                bool status = await dataProvider.RemoveItemAsync(user.UserName);
                if (!status)
                    throw new Error(this.__ResStr("userNotFound", "User {0} not found.", user.UserName));
            }
        }
#endif
#if MVC6
        public Task<IdentityResult> UpdateAsync(UserDefinition user, CancellationToken cancellationToken) {
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
                return Task.FromResult<IdentityResult>(IdentityResult.Success);
            }
        }
#else
        public async Task UpdateAsync(UserDefinition user) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                UpdateStatusEnum status = await dataProvider.UpdateItemAsync(user);
                switch (status) {
                    default:
                    case UpdateStatusEnum.NewKeyExists:
                        throw new InternalError("Unexpected update status");
                    case UpdateStatusEnum.RecordDeleted:
                        throw new Error(this.__ResStr("userDeleted", "User {0} not found.", user.UserName));
                    case UpdateStatusEnum.OK:
                        break;
                }
            }
        }
#endif
#if MVC6
        public Task<UserDefinition> FindByIdAsync(string userId, CancellationToken cancellationToken)
#else
        public async Task<UserDefinition> FindByIdAsync(string userId)
#endif
        {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                UserDefinition user = null;
                try {
                    user = await dataProvider.GetItemByUserIdAsync(Convert.ToInt32(userId));
                } catch (Exception) { }
                return user;
            }
        }
#if MVC6
        public Task<string> GetUserIdAsync(UserDefinition user, CancellationToken cancellationToken) {
            return Task.FromResult<string>(user.UserId.ToString());
        }
        public Task<string> GetUserNameAsync(UserDefinition user, CancellationToken cancellationToken) {
            return Task.FromResult<string>(user.UserName);
        }
        public Task SetUserNameAsync(UserDefinition user, string userName, CancellationToken cancellationToken) {
            user.UserName = userName;
            return Task.FromResult(0);
        }
        public Task<string> GetNormalizedUserNameAsync(UserDefinition user, CancellationToken cancellationToken) {
            return Task.FromResult<string>(user.UserName);
        }
        public Task SetNormalizedUserNameAsync(UserDefinition user, string normalizedName, CancellationToken cancellationToken) {
            if (string.Compare(user.UserName, normalizedName, true) != 0)
                user.UserName = normalizedName;
            return Task.FromResult(0);
        }
#else
#endif
#if MVC6
        public Task<UserDefinition> FindByNameAsync(string userName, CancellationToken cancellationToken)
#else
        public async Task<UserDefinition> FindByNameAsync(string userName)
#endif
        {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                UserDefinition user = await dataProvider.GetItemAsync(userName);
                return user;
            }
        }

        // IUserLoginStore
        // IUserLoginStore
        // IUserLoginStore
#if MVC6
        public Task AddLoginAsync(UserDefinition user, UserLoginInfo login, CancellationToken cancellationToken)
#else
        public async Task AddLoginAsync(UserDefinition user, UserLoginInfo login)
#endif
        {
            using (UserLoginInfoDataProvider logInfoDP = new DataProvider.UserLoginInfoDataProvider()) {
                await logInfoDP.AddItemAsync(user.UserId, login.LoginProvider, login.ProviderKey);
            }

            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                UpdateStatusEnum status = await dataProvider.UpdateItemAsync(user);
                switch (status) {
                    case UpdateStatusEnum.RecordDeleted:
                        throw new Error(this.__ResStr("delUser", "Can't update user {0}, because the user has been deleted.", user.UserName));
                    default:
                    case UpdateStatusEnum.NewKeyExists:
                        throw new InternalError("Unexpected update status");
                    case UpdateStatusEnum.OK:
                        break;
                }
            }
        }
#if MVC6
        public Task<UserDefinition> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) {
            using (UserLoginInfoDataProvider logInfoDP = new DataProvider.UserLoginInfoDataProvider(CurrentSiteIdentity)) {
                UserDefinition user = logInfoDP.GetItem(loginProvider, providerKey);
                return Task.FromResult(user);
            }
        }
#else
        public async Task<UserDefinition> FindAsync(UserLoginInfo login) {
            using (UserLoginInfoDataProvider logInfoDP = new DataProvider.UserLoginInfoDataProvider(CurrentSiteIdentity)) {
                UserDefinition user = await logInfoDP.GetItemAsync(login.LoginProvider, login.ProviderKey);
                return user;
            }
        }
#endif
#if MVC6
        public Task<IList<UserLoginInfo>> GetLoginsAsync(UserDefinition user, CancellationToken cancellationToken)
#else
        public Task<IList<UserLoginInfo>> GetLoginsAsync(UserDefinition user)
#endif
        {
            throw new NotImplementedException();
        }
#if MVC6
        public Task RemoveLoginAsync(UserDefinition user, string loginProvider, string providerKey, CancellationToken cancellationToken) {
            using (UserLoginInfoDataProvider logInfoDP = new DataProvider.UserLoginInfoDataProvider(CurrentSiteIdentity)) {
                logInfoDP.RemoveItem(loginProvider, providerKey);
                return Task.FromResult(0);
            }
        }
#else
        public async Task RemoveLoginAsync(UserDefinition user, UserLoginInfo login) {
            using (UserLoginInfoDataProvider logInfoDP = new DataProvider.UserLoginInfoDataProvider(CurrentSiteIdentity)) {
                await logInfoDP.RemoveItemAsync(login.LoginProvider, login.ProviderKey);
            }
        }
#endif

        // IUserPasswordStore
        // IUserPasswordStore
        // IUserPasswordStore
#if MVC6
        public Task<string> GetPasswordHashAsync(UserDefinition user, CancellationToken cancellationToken)
#else
        public Task<string> GetPasswordHashAsync(UserDefinition user)
#endif
        {
            return Task.FromResult(user.PasswordHash);
        }
#if MVC6
        public Task<bool> HasPasswordAsync(UserDefinition user, CancellationToken cancellationToken)
#else
        public Task<bool> HasPasswordAsync(UserDefinition user)
#endif
        {
            return Task.FromResult(user.PasswordHash != null);
        }
#if MVC6
        public Task SetPasswordHashAsync(UserDefinition user, string passwordHash, CancellationToken cancellationToken)
#else
        public Task SetPasswordHashAsync(UserDefinition user, string passwordHash)
#endif
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        // IUserRoleStore
        // IUserRoleStore
        // IUserRoleStore

#if MVC6
        public Task AddToRoleAsync(UserDefinition user, string role, CancellationToken cancellationToken)
#else
        public Task AddToRoleAsync(UserDefinition user, string role)
#endif
        {
            int roleId = Convert.ToInt32(role);
            user.RolesList.Add(new Role() { RoleId = roleId });
            return Task.CompletedTask;
        }
#if MVC6
        public Task<IList<string>> GetRolesAsync(UserDefinition user, CancellationToken cancellationToken)
#else
        public Task<IList<string>> GetRolesAsync(UserDefinition user)
#endif
        {
            return Task.FromResult((IList<string>) (from Role role in user.RolesList select role.RoleId.ToString()).ToList<string>());
        }
#if MVC6
        public Task<IList<UserDefinition>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
#else
#endif
#if MVC6
        public Task<bool> IsInRoleAsync(UserDefinition user, string roleString, CancellationToken cancellationToken)
#else
        public Task<bool> IsInRoleAsync(UserDefinition user, string roleString)
#endif
        {
            int roleId = Convert.ToInt32(roleString);
            Role role = (from Role r in user.RolesList where r.RoleId == roleId select r).FirstOrDefault();
            return Task.FromResult<bool>(role != null);
        }
#if MVC6
        public Task RemoveFromRoleAsync(UserDefinition user, string roleString, CancellationToken cancellationToken)
#else
        public Task RemoveFromRoleAsync(UserDefinition user, string roleString)
#endif
        {
            int roleId = Convert.ToInt32(roleString);
            Role role = (from Role r in user.RolesList where r.RoleId == roleId select r).FirstOrDefault();
            if (role != null)
                user.RolesList.Remove(role);
            return Task.CompletedTask;
        }

#if MVC6
        // IUserEmailStore
        // IUserEmailStore
        // IUserEmailStore

        // This only implements get/set of email address - all other methods are not used

        public Task SetEmailAsync(UserDefinition user, string email, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<string> GetEmailAsync(UserDefinition user, CancellationToken cancellationToken) {
            return Task.FromResult<string>(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(UserDefinition user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetEmailConfirmedAsync(UserDefinition user, bool confirmed, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<UserDefinition> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedEmailAsync(UserDefinition user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        public Task SetNormalizedEmailAsync(UserDefinition user, string normalizedEmail, CancellationToken cancellationToken) {
            if (string.Compare(user.Email, normalizedEmail, true) != 0)
                user.Email = normalizedEmail;
            return Task.FromResult(0);
        }

        // ISecurityStampValidator
        // ISecurityStampValidator
        // ISecurityStampValidator

        public Task ValidateAsync(CookieValidatePrincipalContext context) {
            return SecurityStampValidator.ValidatePrincipalAsync(context);
        }

        // IUserSecurityStampStore
        // IUserSecurityStampStore
        // IUserSecurityStampStore

        public Task SetSecurityStampAsync(UserDefinition user, string stamp, CancellationToken cancellationToken) {
            user.SecurityStamp = stamp;
            return Task.CompletedTask;
        }

        public Task<string> GetSecurityStampAsync(UserDefinition user, CancellationToken cancellationToken) {
            // we may not have a security stamp if the user definition was created on mvc5 or before we started using security stamps
            return Task.FromResult<string>(user.SecurityStamp??"not previously initialized");
        }
#else
#endif
    }
}