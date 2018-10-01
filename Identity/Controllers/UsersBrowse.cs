/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Modules;
using YetaWF.Modules.Identity.Support;
using YetaWF.Core.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class UsersBrowseModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.UsersBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands { get; set; }

            public async Task<MenuList> __GetCommandsAsync() {
                MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

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
        private GridDefinition GetGridModel() {
            return new GridDefinition {
                ModuleGuid = Module.ModuleGuid,
                SettingsModuleGuid = Module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = GetActionUrl(nameof(UsersBrowse_GridData)),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                        DataProviderGetRecords<UserDefinition> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem(Module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult UsersBrowse() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel()
            };
            return View(model);
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> UsersBrowse_GridData(string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync(GetGridModel(), fieldPrefix, skip, take, sorts, filters);
        }

        [AllowPost]
        [Permission("RemoveUsers")]
        [ExcludeDemoMode]
        public async Task<ActionResult> Remove(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = await GetUserAsync(userName, dataProvider);
                await dataProvider.RemoveItemAsync(userName);
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }

        [AllowPost]
        [Permission("SendEmails")]
        [ExcludeDemoMode]
        public async Task<ActionResult> SendVerificationEmail(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = await GetUserAsync(userName, dataProvider);
                Emails emails = new Emails();
                await emails.SendVerificationAsync(user);
                return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("verificationSent", "Verification email sent to user {0}.", user.Email));
            }
        }

        [AllowPost]
        [Permission("SendEmails")]
        [ExcludeDemoMode]
        public async Task<ActionResult> SendApprovedEmail(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = await GetUserAsync(userName, dataProvider);
                Emails emails = new Emails();
                await emails.SendApprovalAsync(user);
                return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("approvalSent", "Approval email sent to user {0}.", user.Email));
            }
        }

        [AllowPost]
        [Permission("SendEmails")]
        [ExcludeDemoMode]
        public async Task<ActionResult> SendRejectedEmail(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = await GetUserAsync(userName, dataProvider);
                Emails emails = new Emails();
                await emails.SendRejectedAsync(user);
                return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("rejectionSent", "Rejection email sent to user {0}.", user.Email));
            }
        }

        [AllowPost]
        [Permission("SendEmails")]
        [ExcludeDemoMode]
        public async Task<ActionResult> SendSuspendedEmail(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = await GetUserAsync(userName, dataProvider);
                Emails emails = new Emails();
                await emails.SendSuspendedAsync(user);
                return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("suspendedSent", "Suspension email sent to user {0}.", user.Email));
            }
        }

        [AllowPost]
        [ExcludeDemoMode]
        public async Task<ActionResult> RehashAllPasswords() {
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                await userDP.RehashAllPasswordsAsync();
                return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("rehashDone", "All user passwords have been rehashed"));
            }
        }

        private async Task<UserDefinition> GetUserAsync(string userName, UserDefinitionDataProvider dataProvider) {
            if (string.IsNullOrWhiteSpace(userName))
                throw new Error(this.__ResStr("noItem", "No user name specified"));
            UserDefinition user = await dataProvider.GetItemAsync(userName);
            if (user == null)
                throw new Error(this.__ResStr("notFoundUser", "User {0} not found.", userName));
            return user;
        }
    }
}