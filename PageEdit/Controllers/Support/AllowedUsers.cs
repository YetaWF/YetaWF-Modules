/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.PageEdit.Components;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.PageEdit.Controllers {

    public class AllowedUsersController : ControllerImpl<YetaWF.Modules.PageEdit.Modules.PageEditModule> {

        public AllowedUsersController() { }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddUserToPage(string data, string fieldPrefix, string newUser) {
            // validate
            if (string.IsNullOrWhiteSpace(newUser))
                throw new Error(this.__ResStr("noParm", "No user name specified."));
            int userId = await Resource.ResourceAccess.GetUserIdAsync(newUser);
            if (userId == 0)
                throw new Error(this.__ResStr("noUser", "User {0} doesn't exist", newUser));

            // check duplicate
            List<AllowedUsersEditComponent.GridAllowedUser> list = YetaWFManager.JsonDeserialize<List<AllowedUsersEditComponent.GridAllowedUser>>(data);
            if ((from l in list where l.UserId == userId select l).FirstOrDefault() != null)
                throw new Error(this.__ResStr("dupUser", "User {0} has already been added", newUser));

            // render
            string userName = await Resource.ResourceAccess.GetUserNameAsync(userId);
            AllowedUsersEditComponent.GridAllowedUser userEntry = new AllowedUsersEditComponent.GridAllowedUser(userId, userName);
            return await GridRecordViewAsync(await AllowedUsersEditComponent.GridRecordAsync(fieldPrefix, userEntry));
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ResourceAuthorize(YetaWF.Modules.Identity.Addons.Info.Resource_AllowListOfUserNamesAjax)]
        public async Task<ActionResult> AllowedUsersBrowse_GridData(string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync(AllowedUsersEditComponent.GetGridAllUsersModel(), fieldPrefix, skip, take, sorts, filters);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> AllowedUsersEdit_SortFilter(string data, string fieldPrefix, int skip, int take, List<DataProviderSortInfo> sorts, List<DataProviderFilterInfo> filters) {
            return await GridPartialViewAsync<AllowedUsersEditComponent.GridAllowedUser>(AllowedUsersEditComponent.GetGridModel(false), data, fieldPrefix, skip, take, sorts, filters);
        }

    }
}