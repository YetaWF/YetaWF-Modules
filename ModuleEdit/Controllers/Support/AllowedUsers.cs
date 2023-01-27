/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;
using YetaWF.Modules.ModuleEdit.Components;
using System.Linq;
using System;
using YetaWF.Core.Models;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.ModuleEdit.Controllers {

    public class AllowedUsersController : ControllerImpl<YetaWF.Core.Modules.ModuleDefinition> {

        public AllowedUsersController() { }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddUserToModule(string data, string fieldPrefix, string newUser, Guid editGuid) {
            // validate
            if (string.IsNullOrWhiteSpace(newUser))
                throw new Error(this.__ResStr("noParm", "No user name specified"));
            int userId = await Resource.ResourceAccess.GetUserIdAsync(newUser);
            if (userId == 0)
                throw new Error(this.__ResStr("noUser", "User {0} doesn't exist.", newUser));
            string userName = await Resource.ResourceAccess.GetUserNameAsync(userId);

            // check duplicate
            List<ModuleDefinition.GridAllowedUser> list = Utility.JsonDeserialize<List<ModuleDefinition.GridAllowedUser>>(data);
            if ((from l in list where l.UserId == userId select l).FirstOrDefault() != null)
                throw new Error(this.__ResStr("dupUser", "User {0} has already been added", newUser));

            // render
            ModuleDefinition.GridAllowedUser entry = new ModuleDefinition.GridAllowedUser {
                DisplayUserId = userId,
                UserId = userId,
                View = ModuleDefinition.AllowedEnum.Yes,
                DisplayUserName = userName
            };
            return await GridRecordViewAsync(await AllowedUsersEditComponent.GridRecordAsync(fieldPrefix, entry, editGuid));
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ResourceAuthorize(YetaWF.Modules.Identity.Addons.Info.Resource_AllowListOfUserNamesAjax)]
        public async Task<ActionResult> AllowedUsersBrowse_GridData(GridPartialViewData gridPvData) {
            return await GridPartialViewAsync(AllowedUsersEditComponent.GetGridAllUsersModel(), gridPvData);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> AllowedUsersEdit_SortFilter(GridPartialViewData gridPvData, Guid editGuid) {
            GridDefinition gridModel = AllowedUsersEditComponent.GetGridModel(false);
            ModuleDefinition? module = await ModuleDefinition.LoadAsync(editGuid);
            gridModel.ResourceRedirect = module;
            return await GridPartialViewAsync<ModuleDefinition.GridAllowedUser>(gridModel, gridPvData);
        }
    }
}