/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.DataProvider;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.Controllers;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Models;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Identity;
#else
using Microsoft.AspNet.Identity;
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Modules {

    public class UsersBrowseModuleDataProvider : ModuleDefinitionDataProvider<Guid, UsersBrowseModule>, IInstallableModel { }

    [ModuleGuid("{040eb38f-069a-4bf0-894d-bcb3ff8816e7}")]
    [UniqueModule(UniqueModuleStyle.NonUnique)]
    [ModuleCategory("Configuration")]
    public class UsersBrowseModule : ModuleDefinition {

        public UsersBrowseModule() : base() {
            Title = this.__ResStr("modTitle", "Users");
            Name = this.__ResStr("modName", "Users");
            Description = this.__ResStr("modSummary", "Used to display and manage users. This can be accessed using Admin > Panel > Identity, Users tab (standard YetaWF site).");
            DefaultViewName = StandardViews.Browse;
            UsePartialFormCss = false;
        }

        public override IModuleDefinitionIO GetDataProvider() { return new UsersBrowseModuleDataProvider(); }

        [Category("General"), Caption("Add URL"), Description("The URL to add a new user - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string AddUrl { get; set; }
        [Category("General"), Caption("Display URL"), Description("The URL to display a user - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string DisplayUrl { get; set; }
        [Category("General"), Caption("Edit URL"), Description("The URL to edit a user - if omitted, a default page is generated")]
        [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
        public string EditUrl { get; set; }

        public override SerializableList<AllowedRole> DefaultAllowedRoles { get { return AdministratorLevel_DefaultAllowedRoles; } }
        public override List<RoleDefinition> ExtraRoles {
            get {
                return new List<RoleDefinition>() {
                    new RoleDefinition("RemoveUsers",
                        this.__ResStr("roleRemItemsC", "Remove Users"), this.__ResStr("roleRemItems", "The role has permission to remove individual users"),
                        this.__ResStr("userRemItemsC", "Remove Users"), this.__ResStr("userRemItems", "The user has permission to remove individual users")),
                    new RoleDefinition("SendEmails",
                        this.__ResStr("roleSendEmailsC", "Send Emails"), this.__ResStr("roleSendEmails", "The role has permission to send emails to users about their account"),
                        this.__ResStr("userSendEmailsC", "Send Emails"), this.__ResStr("userSendEmails", "The user has permission to send emails to users about their account")),
                };
            }
        }

        public override async Task<MenuList> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
            MenuList menuList = await base.GetModuleMenuListAsync(renderMode, location);
            UsersAddModule mod = new UsersAddModule();
            menuList.New(mod.GetAction_Add(AddUrl), location);
            return menuList;
        }

        public ModuleAction GetAction_Users(string url) {
            return new ModuleAction(this) {
                Url = string.IsNullOrWhiteSpace(url) ? ModulePermanentUrl : url,
                Image = "#Browse",
                LinkText = this.__ResStr("usersLink", "Users"),
                MenuText = this.__ResStr("usersText", "Users"),
                Tooltip = this.__ResStr("usersTooltip", "Display and manage users"),
                Legend = this.__ResStr("usersLegend", "Displays and manages users"),
                Style = ModuleAction.ActionStyleEnum.Normal,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                Category = ModuleAction.ActionCategoryEnum.Read,
                Mode = ModuleAction.ActionModeEnum.Any,
            };
        }

        public ModuleAction GetAction_RemoveLink(string userName) {
            if (!IsAuthorized("RemoveUsers")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(UsersBrowseModuleController), "Remove"),
                QueryArgs = new { UserName = userName },
                Image = "#Remove",
                NeedsModuleContext = true,
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("removeLink", "Remove User"),
                MenuText = this.__ResStr("removeMenu", "Remove User"),
                Tooltip = this.__ResStr("removeTT", "Removes the user"),
                Legend = this.__ResStr("removeLegend", "Removes the user"),
                Category = ModuleAction.ActionCategoryEnum.Delete,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("removeConfirm", "Are you sure you want to remove user \"{0}\"?", userName),
            };
        }

        public async Task<ModuleAction> GetAction_SendVerificationEmailAsync(string userName) {
            if (!IsAuthorized("SendEmails")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(UsersBrowseModuleController), nameof(UsersBrowseModuleController.SendVerificationEmail)),
                NeedsModuleContext = true,
                QueryArgs = new { UserName = userName },
                Image = await CustomIconAsync("VerificationEmail.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("sendVerificationLink", "Send Verification"),
                MenuText = this.__ResStr("sendVerificationMenu", "Send Verification"),
                Tooltip = this.__ResStr("sendVerificationTT", "Sends a verification email to the user"),
                Legend = this.__ResStr("sendVerificationLegend", "Sends a verification email to the user"),
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("sendVerificationConfirm", "Are you sure you want to send a verification email to user \"{0}\"?", userName),
            };
        }

        public async Task<ModuleAction> GetAction_SendApprovedEmailAsync(string userName) {
            if (!IsAuthorized("SendEmails")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(UsersBrowseModuleController), nameof(UsersBrowseModuleController.SendApprovedEmail)),
                NeedsModuleContext = true,
                QueryArgs = new { UserName = userName },
                Image = await CustomIconAsync("ApprovedEmail.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("sendApprovedLink", "Send Approved"),
                MenuText = this.__ResStr("sendApprovedMenu", "Send Approved"),
                Tooltip = this.__ResStr("sendApprovedTT", "Send an email to the user that his/her account has been approved"),
                Legend = this.__ResStr("sendApprovedLegend", "Sends an email to the user that his/her account has been approved"),
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("sendApprovedConfirm", "Are you sure you want to send an email to user \"{0}\" that the account has been approved?", userName),
            };
        }
        public async Task<ModuleAction> GetAction_SendRejectedEmailAsync(string userName) {
            if (!IsAuthorized("SendEmails")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(UsersBrowseModuleController), nameof(UsersBrowseModuleController.SendRejectedEmail)),
                NeedsModuleContext = true,
                QueryArgs = new { UserName = userName },
                Image = await CustomIconAsync("RejectedEmail.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("sendRejectedLink", "Send Rejected"),
                MenuText = this.__ResStr("sendRejectedMenu", "Send Rejected"),
                Tooltip = this.__ResStr("sendRejectedTT", "Send an email to the user that his/her account has been rejected"),
                Legend = this.__ResStr("sendRejectedLegend", "Sends an email to the user that his/her account has been rejected"),
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("sendRejectedConfirm", "Are you sure you want to send an email to user \"{0}\" that the account has been rejected?", userName),
            };
        }
        public async Task<ModuleAction> GetAction_SendSuspendedEmailAsync(string userName) {
            if (!IsAuthorized("SendEmails")) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(UsersBrowseModuleController), nameof(UsersBrowseModuleController.SendSuspendedEmail)),
                NeedsModuleContext = true,
                QueryArgs = new { UserName = userName },
                Image = await CustomIconAsync("SuspendedEmail.png"),
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("sendSuspendedLink", "Send Suspended"),
                MenuText = this.__ResStr("sendSuspendedMenu", "Send Suspended"),
                Tooltip = this.__ResStr("sendSuspendedTT", "Send an email to the user that his/her account has been suspended"),
                Legend = this.__ResStr("sendSuspendedLegend", "Sends an email to the user that his/her account has been suspended"),
                Category = ModuleAction.ActionCategoryEnum.Update,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.NoAuto,
                ConfirmationText = this.__ResStr("sendSuspendedConfirm", "Are you sure you want to send an email to user \"{0}\" that the account has been suspended?", userName),
            };
        }
        public ModuleAction GetAction_RehashAllPasswords() {
            if (!Manager.HasSuperUserRole) return null;
            return new ModuleAction(this) {
                Url = Utility.UrlFor(typeof(UsersBrowseModuleController), nameof(UsersBrowseModuleController.RehashAllPasswords)),
                NeedsModuleContext = true,
                Style = ModuleAction.ActionStyleEnum.Post,
                LinkText = this.__ResStr("rehashLink", "Rehash All Passwords"),
                MenuText = this.__ResStr("rehashMenu", "Rehashes All Passwords"),
                Tooltip = this.__ResStr("rehashTT", "Recalculate the password hash based on each user's plain text password - This will take a while - ONLY DO THIS AFTER YOU IMPORTED USER INFORMATION"),
                Legend = this.__ResStr("rehashLegend", "Recalculates the password hash based on each user's plain text password - this will take a while - ONLY DO THIS AFTER YOU IMPORTED USER INFORMATION"),
                Category = ModuleAction.ActionCategoryEnum.Significant,
                Mode = ModuleAction.ActionModeEnum.Any,
                Location = ModuleAction.ActionLocationEnum.ModuleLinks,
                ConfirmationText = this.__ResStr("rehashConfirm", "Are you sure you want to Recalculate the password hash based on each user's plain text password?"),
            };
        }

        // Properties used to save initial user settings from InitPages.txt
        public string SuperUserPassword {
            set {
                YetaWFManager.Syncify(() => ChangePasswordAsync(SuperuserDefinitionDataProvider.SuperUserName, value));// super rare, so sync is OK
            }
        }

        /// <summary>
        /// Add administrator - used from site template to add a site admin
        /// </summary>
        public void AddAdministrator(string name, string pswd) {
            YetaWFManager.Syncify(async () => { // super rare, so sync is OK
                using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                    await dataProvider.AddAdministratorAsync(name);
                }
                await ChangePasswordAsync(name, pswd);
            });
        }
        /// <summary>
        /// Add an editor - used from site template to add a site editor
        /// </summary>
        public void AddEditor(string name, string pswd) {
            YetaWFManager.Syncify(async () => { // super rare, so sync is OK
                using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                    await dataProvider.AddEditorAsync(name);
                }
                await ChangePasswordAsync(name, pswd);
            });
        }

        /// <summary>
        /// Add a user - used from site template to add a site user
        /// </summary>
        public void AddUser(string name, string pswd) {
            YetaWFManager.Syncify(async () => { // super rare, so sync is OK
                using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                    await dataProvider.AddUserAsync(name);
                }
                await ChangePasswordAsync(name, pswd);
            });
        }

        // Change a user password
        private async Task ChangePasswordAsync(string userName, string newPassword) {

            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = await dataProvider.GetItemAsync(userName);
                if (user == null)
                    throw new Error(this.__ResStr("notFound", "User {0} not found", userName));

                UserManager<UserDefinition> userManager = Managers.GetUserManager();
                string hashedNewPassword;
#if MVC6
                IPasswordHasher<UserDefinition> passwordHasher = (IPasswordHasher<UserDefinition>) YetaWFManager.ServiceProvider.GetService(typeof(IPasswordHasher<UserDefinition>));
                hashedNewPassword = passwordHasher.HashPassword(user, newPassword);
#else
                hashedNewPassword = userManager.PasswordHasher.HashPassword(newPassword);
#endif
                //ModuleDefinition.GetPermanentGuid(typeof(RegisterModule))
                LoginConfigData config = await LoginConfigDataProvider.GetConfigAsync();
                user.PasswordPlainText = config.SavePlainTextPassword ? newPassword : null;
                user.PasswordHash = hashedNewPassword;
                UpdateStatusEnum status = await dataProvider.UpdateItemAsync(user);
                switch (status) {
                    default:
                    case UpdateStatusEnum.NewKeyExists:
                        throw new InternalError("Unexpected status {0}", status);
                    case UpdateStatusEnum.RecordDeleted:
                        throw new Error(this.__ResStr("changeUserNotFound", "The user account for user {0} no longer exists.", userName));
                    case UpdateStatusEnum.OK:
                        break;
                }
            }
        }
    }
}
