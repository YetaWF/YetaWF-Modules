/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Endpoints;
using YetaWF.Core.Identity;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
using YetaWF.Modules.Identity.Endpoints;
using YetaWF.Modules.Identity.Modules;

namespace YetaWF.Modules.Identity.Controllers {

    public class UsersBrowseModuleController : ControllerImpl<YetaWF.Modules.Identity.Modules.UsersBrowseModule> {

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
        internal static GridDefinition GetGridModel(ModuleDefinition module) {
            return new GridDefinition {
                ModuleGuid = module.ModuleGuid,
                SettingsModuleGuid = module.PermanentGuid,
                RecordType = typeof(BrowseItem),
                AjaxUrl = Utility.UrlFor<UsersBrowseModuleEndpoints>(GridSupport.BrowseGridData),
                DirectDataAsync = async (int skip, int take, List<DataProviderSortInfo> sort, List<DataProviderFilterInfo> filters) => {
                    using (UserDefinitionDataProvider dataProvider = new UserDefinitionDataProvider()) {
                        DataProviderGetRecords<UserDefinition> browseItems = await dataProvider.GetItemsAsync(skip, take, sort, filters, IncludeSuperuser: false);
                        return new DataSourceResult {
                            Data = (from s in browseItems.Data select new BrowseItem((UsersBrowseModule)module, s)).ToList<object>(),
                            Total = browseItems.Total
                        };
                    }
                },
            };
        }

        [AllowGet]
        public ActionResult UsersBrowse() {
            BrowseModel model = new BrowseModel {
                GridDef = GetGridModel(Module)
            };
            return View(model);
        }
    }
}
