/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

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
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace YetaWF.Modules.Identity.Models {

    /// <summary>
    /// There is one UserStore per site, stored using PermanentManager
    /// </summary>

    public class YetaWFSecurityStampValidator : SecurityStampValidator<UserDefinition> {
        public YetaWFSecurityStampValidator(IOptions<SecurityStampValidatorOptions> options, SignInManager<UserDefinition> signInManager, ISystemClock clock, ILoggerFactory logger) : base(options, signInManager, clock, logger) { }
    }

    public class UserStore :
            IUserStore<UserDefinition>,
            IUserLoginStore<UserDefinition>,
            IUserPasswordStore<UserDefinition>,
            IUserSecurityStampStore<UserDefinition>,
            IUserEmailStore<UserDefinition>,
            ISecurityStampValidator,
            IUserRoleStore<UserDefinition> {
        public UserStore() { }
        public int CurrentSiteIdentity { get { return YetaWFManager.Manager.CurrentSite.Identity; } }

        public void Dispose() { Dispose(true); }
        protected virtual void Dispose(bool disposing) { }
        //~UserStore() { Dispose(false); }

        // IUserStore
        // IUserStore
        // IUserStore

        public async Task<IdentityResult> CreateAsync(UserDefinition user, CancellationToken cancellationToken) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                bool status = await dataProvider.AddItemAsync(user);
                if (!status)
                    throw new Error(this.__ResStr("userExists", "User {0} already exists.", user.UserName));
                return IdentityResult.Success;
            }
        }
        public async Task<IdentityResult> DeleteAsync(UserDefinition user, CancellationToken cancellationToken) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                bool status = await dataProvider.RemoveItemAsync(user.UserName);
                if (!status)
                    throw new Error(this.__ResStr("userNotFound", "User {0} not found.", user.UserName));
                return IdentityResult.Success;
            }
        }
        public async Task<IdentityResult> UpdateAsync(UserDefinition user, CancellationToken cancellationToken) {
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
                return IdentityResult.Success;
            }
        }
        public async Task<UserDefinition> FindByIdAsync(string userId, CancellationToken cancellationToken) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                UserDefinition user = null;
                try {
                    user = await dataProvider.GetItemByUserIdAsync(Convert.ToInt32(userId));
                } catch (Exception) { }
                return user;
            }
        }
        public Task<string> GetUserIdAsync(UserDefinition user, CancellationToken cancellationToken) {
            return Task.FromResult<string>(user.UserId.ToString());
        }
        public Task<string> GetUserNameAsync(UserDefinition user, CancellationToken cancellationToken) {
            return Task.FromResult<string>(user.UserName);
        }
        public Task SetUserNameAsync(UserDefinition user, string userName, CancellationToken cancellationToken) {
            user.UserName = userName;
            return Task.CompletedTask;
        }
        public Task<string> GetNormalizedUserNameAsync(UserDefinition user, CancellationToken cancellationToken) {
            return Task.FromResult<string>(user.UserName);
        }
        public Task SetNormalizedUserNameAsync(UserDefinition user, string normalizedName, CancellationToken cancellationToken) {
            return Task.CompletedTask;
        }
        public async Task<UserDefinition> FindByNameAsync(string userName, CancellationToken cancellationToken) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider(this.CurrentSiteIdentity)) {
                UserDefinition user = await dataProvider.GetItemAsync(userName);
                return user;
            }
        }

        // IUserLoginStore
        // IUserLoginStore
        // IUserLoginStore
        public async Task AddLoginAsync(UserDefinition user, UserLoginInfo login, CancellationToken cancellationToken) {
            using (UserLoginInfoDataProvider logInfoDP = new DataProvider.UserLoginInfoDataProvider(CurrentSiteIdentity)) {
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
        public async Task<UserDefinition> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken) {
            using (UserLoginInfoDataProvider logInfoDP = new DataProvider.UserLoginInfoDataProvider(CurrentSiteIdentity)) {
                UserDefinition user = await logInfoDP.GetItemAsync(loginProvider, providerKey);
                return user;
            }
        }
        public Task<IList<UserLoginInfo>> GetLoginsAsync(UserDefinition user, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
        public async Task RemoveLoginAsync(UserDefinition user, string loginProvider, string providerKey, CancellationToken cancellationToken) {
            using (UserLoginInfoDataProvider logInfoDP = new DataProvider.UserLoginInfoDataProvider(CurrentSiteIdentity)) {
                await logInfoDP.RemoveItemAsync(loginProvider, providerKey);
            }
        }

        // IUserPasswordStore
        // IUserPasswordStore
        // IUserPasswordStore
        public Task<string> GetPasswordHashAsync(UserDefinition user, CancellationToken cancellationToken) {
            return Task.FromResult(user.PasswordHash);
        }
        public Task<bool> HasPasswordAsync(UserDefinition user, CancellationToken cancellationToken) {
            return Task.FromResult(user.PasswordHash != null);
        }
        public Task SetPasswordHashAsync(UserDefinition user, string passwordHash, CancellationToken cancellationToken) {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        // IUserRoleStore
        // IUserRoleStore
        // IUserRoleStore

        public Task AddToRoleAsync(UserDefinition user, string role, CancellationToken cancellationToken) {
            int roleId = Convert.ToInt32(role);
            user.RolesList.Add(new Role() { RoleId = roleId });
            return Task.CompletedTask;
        }
        public Task<IList<string>> GetRolesAsync(UserDefinition user, CancellationToken cancellationToken) {
            return Task.FromResult((IList<string>)(from Role role in user.RolesList select role.RoleId.ToString()).ToList<string>());
        }
        public Task<IList<UserDefinition>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }
        public Task<bool> IsInRoleAsync(UserDefinition user, string roleString, CancellationToken cancellationToken) {
            int roleId = Convert.ToInt32(roleString);
            Role role = (from Role r in user.RolesList where r.RoleId == roleId select r).FirstOrDefault();
            return Task.FromResult<bool>(role != null);
        }
        public Task RemoveFromRoleAsync(UserDefinition user, string roleString, CancellationToken cancellationToken) {
            int roleId = Convert.ToInt32(roleString);
            Role role = (from Role r in user.RolesList where r.RoleId == roleId select r).FirstOrDefault();
            if (role != null)
                user.RolesList.Remove(role);
            return Task.CompletedTask;
        }

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
            return Task.CompletedTask;
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
            return Task.FromResult<string>(user.SecurityStamp ?? "not previously initialized");
        }
    }
}
