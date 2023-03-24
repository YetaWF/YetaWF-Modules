/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Identity;
using YetaWF.Core.IO;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Pages;
using YetaWF.Core.Serializers;
using YetaWF.Core.Support;
using YetaWF.DataProvider;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Endpoints;
using YetaWF.Modules.Identity.Models;

namespace YetaWF.Modules.Identity.Modules;

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

    [Category("General"), Caption("Add Url"), Description("The Url to add a new user - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string AddUrl { get; set; }
    [Category("General"), Caption("Display Url"), Description("The Url to display a user - if omitted, a default page is generated")]
    [UIHint("Url"), AdditionalMetadata("UrlType", UrlTypeEnum.Local), UrlValidation(UrlValidationAttribute.SchemaEnum.Any, UrlTypeEnum.Local), StringLength(Globals.MaxUrl), Trim]
    public string DisplayUrl { get; set; }
    [Category("General"), Caption("Edit Url"), Description("The Url to edit a user - if omitted, a default page is generated")]
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

    public override async Task<List<ModuleAction>> GetModuleMenuListAsync(ModuleAction.RenderModeEnum renderMode, ModuleAction.ActionLocationEnum location) {
        List<ModuleAction> menuList = await base.GetModuleMenuListAsync(renderMode, location);
        UsersAddModule mod = new UsersAddModule();
        menuList.New(mod.GetAction_Add(AddUrl), location);
        return menuList;
    }

    public ModuleAction GetAction_Users(string url) {
        return new ModuleAction() {
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
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(UsersBrowseModuleEndpoints), UsersBrowseModuleEndpoints.Remove),
            QueryArgs = new { UserName = userName },
            Image = "#Remove",
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
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(UsersBrowseModuleEndpoints), nameof(UsersBrowseModuleEndpoints.SendVerificationEmail)),
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
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(UsersBrowseModuleEndpoints), nameof(UsersBrowseModuleEndpoints.SendApprovedEmail)),
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
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(UsersBrowseModuleEndpoints), nameof(UsersBrowseModuleEndpoints.SendRejectedEmail)),
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
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(UsersBrowseModuleEndpoints), nameof(UsersBrowseModuleEndpoints.SendSuspendedEmail)),
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
        return new ModuleAction() {
            Url = Utility.UrlFor(typeof(UsersBrowseModuleEndpoints), nameof(UsersBrowseModuleEndpoints.RehashAllPasswords)),
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
            IPasswordHasher<UserDefinition> passwordHasher = (IPasswordHasher<UserDefinition>)Manager.ServiceProvider.GetService(typeof(IPasswordHasher<UserDefinition>));
            string hashedNewPassword = passwordHasher.HashPassword(user, newPassword);
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

    public class BrowseItem {

        [Caption("Actions"), Description("The available actions")]
        [UIHint("ModuleActionsGrid"), ReadOnly]
        public List<ModuleAction> Commands { get; set; }

        public async Task<List<ModuleAction>> __GetCommandsAsync() {
            List<ModuleAction> actions = new List<ModuleAction>();

            UsersDisplayModule dispMod = new UsersDisplayModule();
            actions.New(dispMod.GetAction_Display(Module.DisplayUrl, UserName), ModuleAction.ActionLocationEnum.GridLinks);

            UsersEditModule editMod = new UsersEditModule();
            actions.New(editMod.GetAction_Edit(Module.EditUrl, UserName), ModuleAction.ActionLocationEnum.GridLinks);

            if (UserStatus == UserStatusEnum.NeedValidation) {
                actions.New(await Module.GetAction_SendVerificationEmailAsync(UserName), ModuleAction.ActionLocationEnum.GridLinks);
            } else if (UserStatus == UserStatusEnum.Approved) {
                actions.New(await Module.GetAction_SendApprovedEmailAsync(UserName), ModuleAction.ActionLocationEnum.GridLinks);
            } else if (UserStatus == UserStatusEnum.Rejected) {
                actions.New(await Module.GetAction_SendRejectedEmailAsync(UserName), ModuleAction.ActionLocationEnum.GridLinks);
            } else if (UserStatus == UserStatusEnum.Suspended) {
                actions.New(await Module.GetAction_SendSuspendedEmailAsync(UserName), ModuleAction.ActionLocationEnum.GridLinks);
            }
            actions.New(Module.GetAction_RemoveLink(UserName), ModuleAction.ActionLocationEnum.GridLinks);

            return actions;
        }

        [Caption("Name"), Description("The user's name")]
        [UIHint("String"), ReadOnly]
        public string UserName { get; set; }

        [Caption("Email Address"), Description("The user's email address")]
        [UIHint("YetaWF_Identity_Email"), ReadOnly]
        public string Email { get; set; }

        [Caption("User Id"), Description("The user's internal id")]
        [UIHint("IntValue"), ReadOnly]
        public int UserId { get; set; }

        [Caption("Status"), Description("The user's current account status")]
        [UIHint("Enum"), ReadOnly]
        public UserStatusEnum UserStatus { get; set; }

        [Caption("New Password"), Description("Defines whether the user must change the password")]
        [UIHint("Boolean"), ReadOnly]
        public bool NeedsNewPassword { get; set; }

        [Caption("Verification Code"), Description("The verification code to verify the user")]
        [UIHint("String"), ReadOnly]
        public string VerificationCode { get; set; }

        [Caption("Registration IP"), Description("The IP address from which the user registered on this site")]
        [UIHint("IPAddress"), StringLength(Globals.MaxIP), ReadOnly]
        public string RegistrationIP { get; set; }

        [Caption("Created"), Description("The date/time the user account was created")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime Created { get; set; }

        [Caption("Last Login"), Description("The last time the user logged into his/her account")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime? LastLoginDate { get; set; }

        [Caption("Last Login IP"), Description("The IP address from which the user last logged on this site")]
        [UIHint("IPAddress"), StringLength(Globals.MaxIP), ReadOnly]
        public string LastLoginIP { get; set; }

        [Caption("Last Password Change"), Description("The last time the user changed his/her password")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime? LastPasswordChangedDate { get; set; }
        [Caption("Password Change IP"), Description("The IP address from which the user last changed the password on this site")]
        [UIHint("IPAddress"), StringLength(Globals.MaxIP), ReadOnly]
        public string PasswordChangeIP { get; set; }

        [Caption("Last Activity"), Description("The last time the user did anything on this site")]
        [UIHint("DateTime"), ReadOnly]
        public DateTime? LastActivityDate { get; set; }
        [Caption("Last Activity IP"), Description("The IP address from which the user did anything on this site")]
        [UIHint("IPAddress"), StringLength(Globals.MaxIP), ReadOnly]
        public string LastActivityIP { get; set; }

        private UsersBrowseModule Module { get; set; }

        public BrowseItem(UsersBrowseModule module, UserDefinition data) {
            Module = module;
            ObjectSupport.CopyData(data, this);
        }
    }

    public class BrowseModel {
        [UIHint("Grid"), ReadOnly]
        public GridDefinition GridDef { get; set; }
    }
    public GridDefinition GetGridModel() {
        return new GridDefinition {
            ModuleGuid = ModuleGuid,
            SettingsModuleGuid = PermanentGuid,
            RecordType = typeof(BrowseItem),
            AjaxUrl = Utility.UrlFor<UsersBrowseModuleEndpoints>(GridSupport.BrowseGridData),
            DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                    DataProviderGetRecords<UserDefinition> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters, IncludeSuperuser: false);
                    return new DataSourceResult {
                        Data = (from s in browseItems.Data select new BrowseItem(this, s)).ToList<object>(),
                        Total = browseItems.Total
                    };
                }
            },
        };
    }

    public async Task<ActionInfo> RenderModuleAsync() {
        BrowseModel model = new BrowseModel {
            GridDef = GetGridModel()
        };
        return await RenderAsync(model);
    }
}
