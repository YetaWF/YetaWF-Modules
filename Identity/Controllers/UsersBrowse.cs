/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using YetaWF.Core;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Localize;
using YetaWF.Core.Menus;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Core.Views.Shared;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Modules;
using YetaWF.Modules.Identity.Support;

namespace YetaWF.Modules.Identity.Controllers {

    public class UsersBrowseModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.UsersBrowseModule> {

        public class BrowseItem {

            [Caption("Actions"), Description("The available actions")]
            [UIHint("ActionIcons"), ReadOnly]
            public MenuList Commands {
                get {
                    MenuList actions = new MenuList() { RenderMode = ModuleAction.RenderModeEnum.IconsOnly };

                    UsersDisplayModule dispMod = new UsersDisplayModule();
                    actions.New(dispMod.GetAction_Display(Module.DisplayUrl, UserName), ModuleAction.ActionLocationEnum.GridLinks);

                    UsersEditModule editMod = new UsersEditModule();
                    actions.New(editMod.GetAction_Edit(Module.EditUrl, UserName), ModuleAction.ActionLocationEnum.GridLinks);

                    if (UserStatus == UserStatusEnum.NeedValidation) {
                        actions.New(Module.GetAction_SendVerificationEmail(UserName), ModuleAction.ActionLocationEnum.GridLinks);
                    } else if (UserStatus == UserStatusEnum.Approved) {
                        actions.New(Module.GetAction_SendApprovedEmail(UserName), ModuleAction.ActionLocationEnum.GridLinks);
                    } else if (UserStatus == UserStatusEnum.Rejected) {
                        actions.New(Module.GetAction_SendRejectedEmail(UserName), ModuleAction.ActionLocationEnum.GridLinks);
                    } else if (UserStatus == UserStatusEnum.Suspended) {
                        actions.New(Module.GetAction_SendSuspendedEmail(UserName), ModuleAction.ActionLocationEnum.GridLinks);
                    }
                    actions.New(Module.GetAction_RemoveLink(UserName), ModuleAction.ActionLocationEnum.GridLinks);

                    return actions;
                }
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
            [UIHint("Grid")]
            public GridDefinition GridDef { get; set; }
        }

        [HttpGet]
        public ActionResult UsersBrowse() {
            BrowseModel model = new BrowseModel { };
            model.GridDef = new GridDefinition {
                AjaxUrl = GetActionUrl("UsersBrowse_GridData"),
                ModuleGuid = Module.ModuleGuid,
                RecordType = typeof(BrowseItem),
                SettingsModuleGuid = Module.PermanentGuid,
            };
            return View(model);
        }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        public ActionResult UsersBrowse_GridData(int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters, Guid settingsModuleGuid) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                int total;
                List<UserDefinition> browseItems = dataProvider.GetItems(skip, take, sort, filters, out total);
                GridHelper.SaveSettings(skip, take, sort, filters, settingsModuleGuid);
                return GridPartialView(new DataSourceResult {
                    Data = (from s in browseItems select new BrowseItem(Module, s)).ToList<object>(),
                    Total = total
                });
            }
        }

        [HttpPost]
        [Permission("RemoveUsers")]
        [ExcludeDemoMode]
        public ActionResult Remove(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = GetUser(userName, dataProvider);
                dataProvider.RemoveItem(userName);
                return Reload(null, Reload: ReloadEnum.ModuleParts);
            }
        }

        [HttpPost]
        [Permission("SendEmails")]
        [ExcludeDemoMode]
        public ActionResult SendVerificationEmail(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = GetUser(userName, dataProvider);
                Emails emails = new Emails();
                emails.SendVerification(user);
                return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("verificationSent", "Verification email sent to user {0}.", user.Email));
            }
        }

        [HttpPost]
        [Permission("SendEmails")]
        [ExcludeDemoMode]
        public ActionResult SendApprovedEmail(string userName) {
            using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                UserDefinition user = GetUser(userName, dataProvider);
                Emails emails = new Emails();
                emails.SendApproval(user);
                return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("approvalSent", "Approval email sent to user {0}.", user.Email));
            }
        }

        [HttpPost]
        [ExcludeDemoMode]
        public ActionResult RehashAllPasswords() {
            using (UserDefinitionDataProvider userDP = new UserDefinitionDataProvider()) {
                userDP.RehashAllPasswords();
                return Reload(null, Reload: ReloadEnum.ModuleParts, PopupText: this.__ResStr("rehashDone", "All user passwords have been rehashed"));
            }
        }

        private UserDefinition GetUser(string userName, UserDefinitionDataProvider dataProvider) {
            if (string.IsNullOrWhiteSpace(userName))
                throw new Error(this.__ResStr("noItem", "No user name specified"));
            UserDefinition user = dataProvider.GetItem(userName);
            if (user == null)
                throw new Error(this.__ResStr("notFoundUser", "User {0} not found.", userName));
            return user;
        }
    }
}