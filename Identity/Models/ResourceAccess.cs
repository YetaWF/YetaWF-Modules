/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Site;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.DataProvider {

    public class ResourceAccessDataProvider : IInitializeApplicationStartup, IResource {
        protected YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        // STARTUP
        // STARTUP
        // STARTUP

        public Task InitializeApplicationStartupAsync(bool firstNode) {
            Resource.ResourceAccess = (IResource)this;
            return Task.CompletedTask;
        }

        // IRESOURCE
        // IRESOURCE
        // IRESOURCE

        public bool IsBackDoorWideOpen()
        {
            if (backDoor == null) {
                backDoor = WebConfigHelper.GetValue<bool>(AreaRegistration.CurrentPackage.AreaName, "BACKDOOR-IS-WIDE-OPEN");
            }
            return (bool) backDoor;
        }
        public void ShutTheBackDoor() {
            WebConfigHelper.SetValue<string>(AreaRegistration.CurrentPackage.AreaName, "BACKDOOR-IS-WIDE-OPEN", "0");
            WebConfigHelper.Save();
            backDoor = false;
        }
        private bool? backDoor = null;

        public async Task<bool> IsResourceAuthorizedAsync(string resourceName) {
            // we need to check if this resource is protected

            if (string.IsNullOrEmpty(resourceName))
                throw new InternalError("Missing resource name");

            if (IsBackDoorWideOpen())
                return true;
            if (Manager.IsDemo)
                return true;

            // check if this is the superuser
            if (Manager.HasSuperUserRole)
                return true;

            using (AuthorizationDataProvider authDP = new AuthorizationDataProvider()) {

                Authorization auth = await authDP.GetItemAsync(resourceName);
                if (auth == null) {
                    Logging.AddLog("Resource {0} doesn't exist", resourceName);
#if DEBUG
                    throw new InternalError("Resource {0} doesn't exist", resourceName);
#else
                return false;// not authorized, there is no such resource
#endif
                }
                RoleComparer roleComp = new RoleComparer();
                using (RoleDefinitionDataProvider roleDP = new RoleDefinitionDataProvider()) {
                    if (!Manager.HaveUser) {
                        // check if anonymous user allowed
                        if (auth.AllowedRoles.Contains(new Role { RoleId = roleDP.GetAnonymousRoleId() }, roleComp))
                            return true;
                        return false;
                    }
                    // authenticated user
                    // check if any authenticated user allowed
                    if (auth.AllowedRoles.Contains(new Role { RoleId = roleDP.GetUserRoleId() }, roleComp))
                        return true;
                }

                string userName = Manager.UserName;
                UserDefinition user = (UserDefinition)Manager.UserObject;// get the saved user
                if (user == null)
                    throw new InternalError("UserObject missing for authenticated user");

                // check if this user is allowed
                if (auth.AllowedUsers.Contains(new User { UserId = user.UserId }, new UserComparer()))
                    return true;

                // check if this user is in a permitted role
                foreach (Role loginRole in user.RolesList) {
                    if (auth.AllowedRoles.Contains(new Role { RoleId = loginRole.RoleId }, roleComp))
                        return true;
                }
            }            // simply not authorized
            return false;
        }

        public async Task ResolveUserAsync() {

            if (!Manager.HaveCurrentRequest)
                throw new InternalError("No httpRequest");

            // check whether we have a logged on user
#if MVC6
            if (SiteDefinition.INITIAL_INSTALL || !Manager.CurrentContext.User.Identity.IsAuthenticated)
#else
            if (SiteDefinition.INITIAL_INSTALL || !Manager.CurrentRequest.IsAuthenticated)
#endif
            {
                return;// no user logged in
            }
            // get user info and save in Manager
            string userName = Manager.CurrentContext.User.Identity.Name;
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                if (!await userDP.IsInstalledAsync()) {
                    Logging.AddErrorLog("UserDefinitionDataProvider not installed");
                    return;
                }
                UserDefinition user = await userDP.GetItemAsync(userName);
                if (user == null) {
                    Logging.AddErrorLog("Authenticated user {0} doesn't exist", userName);
#if DEBUG
                    //throw new InternalError("Authenticated user doesn't exist");
                    return;
#else
                    return;
#endif
                }
                // Check whether user needs to set up two-step authentication
                // External login providers don't require local two-step authentication (should be offered by external login provider)
                // If any of the user's roles require two-step authentication and the user has not enabled two-step authentication providers,
                // set marker so we can redirect the user
                if (Manager.Need2FAState == null) {
                    Manager.Need2FAState = false;
                    using (UserLoginInfoDataProvider logInfoDP = new UserLoginInfoDataProvider()) {
                        if (!await logInfoDP.IsExternalUserAsync(user.UserId)) {
                            // not an external login, so check if we need two-step auth
                            LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
                            if (config.TwoStepAuth != null && user.RolesList != null) {
                                foreach (Role role in config.TwoStepAuth) {
                                    if (role.RoleId == Resource.ResourceAccess.GetUserRoleId() || user.RolesList.Contains(new Role { RoleId = role.RoleId }, new RoleComparer())) {
                                        if ((await user.GetEnabledAndAvailableTwoStepAuthenticationsAsync()).Count == 0)
                                            Manager.Need2FAState = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                // user good to go
                Manager.UserName = user.UserName;
                Manager.UserEmail = user.Email;
                Manager.UserId = user.UserId;
                Manager.UserObject = user;
                await UserSettings.UserSettingsAccess.ResolveUserAsync();
                Manager.UserRoles = (from l in user.RolesList select l.RoleId).ToList();

                int superuserRole = Resource.ResourceAccess.GetSuperuserRoleId();
                if (user.RolesList.Contains(new Role { RoleId = superuserRole }, new RoleComparer()))
                    Manager.SetSuperUserRole(true);
            }
        }

        public List<RoleInfo> GetDefaultRoleList(bool Exclude2FA = false) {
            using(RoleDefinitionDataProvider roleDP = new RoleDefinitionDataProvider()) {
                List<RoleDefinition> allRoles = roleDP.GetAllRoles();
                List<RoleInfo> roles;
                if (Exclude2FA) {
                    roles = (from r in allRoles
                             where r.RoleId != roleDP.GetUser2FARoleId()
                             select new RoleInfo { RoleId = r.RoleId, Name = r.Name, Description = r.Description }).ToList();
                } else {
                    roles = (from r in allRoles select new RoleInfo { RoleId = r.RoleId, Name = r.Name, Description = r.Description }).ToList();
                }
                return roles;
            }
        }

        public List<User> GetDefaultUserList() {
            return new SerializableList<User>();
        }

        public int GetSuperuserRoleId() {
            return RoleDefinitionDataProvider.SuperUserId;
        }

        public int GetUserRoleId() {
            using (RoleDefinitionDataProvider roleDP = new RoleDefinitionDataProvider()) {
                return roleDP.GetUserRoleId();
            }
        }
        public int GetUser2FARoleId() {
            using (RoleDefinitionDataProvider roleDP = new RoleDefinitionDataProvider()) {
                return roleDP.GetUser2FARoleId();
            }
        }

        public int GetEditorRoleId() {
            using (RoleDefinitionDataProvider roleDP = new RoleDefinitionDataProvider()) {
                return roleDP.GetEditorRoleId();
            }
        }

        public int GetAdministratorRoleId() {
            using (RoleDefinitionDataProvider roleDP = new RoleDefinitionDataProvider()) {
                return roleDP.GetAdministratorRoleId();
            }
        }

        public int GetAnonymousRoleId() {
            using (RoleDefinitionDataProvider roleDP = new RoleDefinitionDataProvider()) {
                return roleDP.GetAnonymousRoleId();
            }
        }
        public int GetRoleId(string name) {
            using (RoleDefinitionDataProvider roleDP = new RoleDefinitionDataProvider()) {
                return roleDP.GetRoleId(name);
            }
        }

        public async Task<int> GetUserIdAsync(string userName) {
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemAsync(userName);
                if (user == null) return 0;
                return user.UserId;
            }
        }
        public async Task<string> GetUserNameAsync(int userId) {
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemByUserIdAsync(userId);
                if (user == null) return null;
                return user.UserName;
            }
        }
        public async Task<string> GetUserEmailAsync(int userId) {
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemByUserIdAsync(userId);
                if (user == null) return null;
                return user.Email;
            }
        }
        public int GetSuperuserId() {
            return SuperuserDefinitionDataProvider.SuperUserId;
        }

        public async Task AddRoleAsync(string roleName, string description) {
            using (RoleDefinitionDataProvider roleDP = new RoleDefinitionDataProvider()) {
                await roleDP.AddItemAsync(new RoleDefinition { Name = roleName, Description = description });
            }
        }
        public async Task RemoveRoleAsync(string roleName) {
            using (RoleDefinitionDataProvider roleDP = new RoleDefinitionDataProvider()) {
                await roleDP.RemoveItemAsync(roleName);
            }
        }

        public async Task AddRoleToUserAsync(int userId, string roleName) {
            int roleId;
            // get the role id for roleName
            using (RoleDefinitionDataProvider roleDP = new RoleDefinitionDataProvider()) {
                RoleDefinition role = await roleDP.GetItemAsync(roleName);
                if (role == null) throw new InternalError("Unexpected error in AddRoleToUser - expected role {0} not found", roleName);
                roleId = role.RoleId;
            }
            // add the role to the user
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemByUserIdAsync(userId);
                if (user == null) throw new InternalError("Unexpected error in AddRoleToUser - no user found");
                Role role = new Role { RoleId = roleId };
                if (!user.RolesList.Contains(role, new RoleComparer())) {
                    user.RolesList.Add(role);
                    UpdateStatusEnum status = await userDP.UpdateItemAsync(user);
                    if (status != UpdateStatusEnum.OK)
                        throw new InternalError("Unexpected status {0} updating user account in AddRoleToUser", status);
                }
            }
        }

        public async Task RemoveRoleFromUserAsync(int userId, string roleName) {
            int roleId;
            // get the role id for roleName
            using (RoleDefinitionDataProvider roleDP = new RoleDefinitionDataProvider()) {
                RoleDefinition role = await roleDP.GetItemAsync(roleName);
                if (role == null) throw new InternalError("Unexpected error in AddRoleToUser - expected role {0} not found", roleName);
                roleId = role.RoleId;
            }
            // remove the role from the user
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemByUserIdAsync(userId);
                if (user != null) {
                    Role role = (from Role r in user.RolesList where r.RoleId == roleId select r).FirstOrDefault();
                    if (role != null) {
                        user.RolesList.Remove(role);
                        UpdateStatusEnum status = await userDP.UpdateItemAsync(user);
                        if (status != UpdateStatusEnum.OK)
                            throw new InternalError("Unexpected status {0} updating user account in RemoveRoleFromUser", status);
                    }
                }
            }
        }

        public ModuleAction GetSelectTwoStepAction(int userId, string userName, string userEmail) {
            SelectTwoStepAuthModule mod = new SelectTwoStepAuthModule();
            return mod.GetAction_SelectTwoStepAuth(null, userId, userName, userEmail);
        }
        public async Task<ModuleAction> GetForceTwoStepActionSetupAsync(string url) {
            SelectTwoStepSetupModule mod = new SelectTwoStepSetupModule();
            return await mod.GetAction_ForceTwoStepSetupAsync(url);
        }
        public void ShowNeed2FA() {
            Manager.AddOnManager.AddExplicitlyInvokedModules(
                new SerializableList<ModuleDefinition.ReferencedModule> {
                    new ModuleDefinition.ReferencedModule { ModuleGuid = ModuleDefinition.GetPermanentGuid(typeof(Need2FADisplayModule)) }
                }
            );
        }

        public async Task AddEnabledTwoStepAuthenticationAsync(int userId, string auth) {
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemByUserIdAsync(userId);
                if (user == null) throw new InternalError($"Unexpected error in {nameof(AddEnabledTwoStepAuthenticationAsync)} - no user found");
                TwoStepDefinition authDef = new DataProvider.TwoStepDefinition { Name = auth };
                if (!user.EnabledTwoStepAuthentications.Contains(authDef, new TwoStepDefinitionComparer())) {
                    user.EnabledTwoStepAuthentications.Add(authDef);
                    UpdateStatusEnum status = await userDP.UpdateItemAsync(user);
                    if (status != UpdateStatusEnum.OK)
                        throw new InternalError($"Unexpected status {status} updating user account in {nameof(AddEnabledTwoStepAuthenticationAsync)}");
                    Manager.Need2FAState = null;//reevaluate now that user has enabled a two-step authentication
                }
            }
        }
        public async Task<bool> HasEnabledTwoStepAuthenticationAsync(int userId, string auth) {
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemByUserIdAsync(userId);
                if (user == null) return false;
                TwoStepDefinition authDef = new DataProvider.TwoStepDefinition { Name = auth };
                return user.EnabledTwoStepAuthentications.Contains(authDef, new TwoStepDefinitionComparer());
            }
        }
        public async Task RemoveEnabledTwoStepAuthenticationAsync(int userId, string auth) {
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemByUserIdAsync(userId);
                if (user == null) throw new InternalError("Unexpected error in RemoveEnabledTwoStepAuthentication - no user found");
                TwoStepDefinition authDef = user.EnabledTwoStepAuthentications.Find(m => m.Name == auth);
                if (authDef != null) {
                    user.EnabledTwoStepAuthentications.Remove(authDef);
                    UpdateStatusEnum status = await userDP.UpdateItemAsync(user);
                    if (status != UpdateStatusEnum.OK)
                        throw new InternalError("Unexpected status {0} updating user account in RemoveEnabledTwoStepAuthentication", status);
                    Manager.Need2FAState = null;//reevaluate now that user has removed a two-step authentication
                }
            }
        }
        public async Task AddTwoStepLoginFailureAsync() {
            int userId = Manager.SessionSettings.SiteSettings.GetValue<int>(LoginTwoStepController.IDENTITY_TWOSTEP_USERID);
            if (userId == 0)
                throw new InternalError("No user id available in AddTwoStepLoginFailure");
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemByUserIdAsync(userId);
                if (user == null) throw new InternalError("Unexpected error in AddTwoStepLoginFailure - no user found");
                LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
                user.LoginFailures = user.LoginFailures + 1;
                if (config.MaxLoginFailures != 0 && user.LoginFailures >= config.MaxLoginFailures) {
                    if (user.UserStatus != UserStatusEnum.Suspended)
                        user.UserStatus = UserStatusEnum.Suspended;
                }
                UpdateStatusEnum status = await userDP.UpdateItemAsync(user);
                if (status != UpdateStatusEnum.OK)
                    throw new InternalError("Unexpected status {0} updating user account in AddTwoStepLoginFailure", status);
            }
        }
        public async Task<bool> GetTwoStepLoginFailuresExceededAsync() {
            int userId = Manager.SessionSettings.SiteSettings.GetValue<int>(LoginTwoStepController.IDENTITY_TWOSTEP_USERID);
            if (userId == 0)
                throw new InternalError("No user id available in GetTwoStepLoginFailures");
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                UserDefinition user = await userDP.GetItemByUserIdAsync(userId);
                if (user == null) throw new InternalError("Unexpected error in GetTwoStepLoginFailures - no user found");
                LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
                return config.MaxLoginFailures != 0 && user.LoginFailures >= config.MaxLoginFailures;
            }
        }

        public async Task LogoffAsync() {
            await LoginModuleController.UserLogoffAsync();
        }
        public async Task LoginAsAsync(int userId) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = await dataProvider.GetItemByUserIdAsync(userId);
                if (user == null)
                    throw new Error(this.__ResStr("noUser", "User with id {0} doesn't exist", userId));
                if (user.UserStatus != UserStatusEnum.Approved)
                    throw new Error(this.__ResStr("notApproved", "User account for user {0} has not been approved - can't log in", user.UserName));
                await LoginModuleController.UserLoginAsync(user);
            }
        }
    }
}
