/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Addons;
using System.Collections.Generic;
using System.Linq;
using YetaWF.Modules.Identity.Components;
using Microsoft.AspNetCore.Mvc;

namespace YetaWF.Modules.Identity.Controllers {

    public class ResourceUsersController : ControllerImpl<YetaWF.Core.Modules.ModuleDefinition> {

        public ResourceUsersController() { }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ResourceUsersDisplay_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<ResourceUsersDisplayComponent.Entry>(ResourceUsersDisplayComponent.GetGridModel(false), gridPVData);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        public async Task<ActionResult> ResourceUsersEdit_SortFilter(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync<ResourceUsersEditComponent.Entry>(ResourceUsersEditComponent.GetGridModel(false), gridPVData);
        }
        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddUserToResource(string data, string fieldPrefix, string newUser) {
            // validate
            if (string.IsNullOrWhiteSpace(newUser))
                throw new Error(this.__ResStr("noParm", "No user name specified"));
            int userId = await Resource.ResourceAccess.GetUserIdAsync(newUser);
            if (userId == 0)
                throw new Error(this.__ResStr("noUser", "User {0} doesn't exist.", newUser));
            string userName = await Resource.ResourceAccess.GetUserNameAsync(userId);
            // check duplicate
            List<ResourceUsersEditComponent.Entry> list = Utility.JsonDeserialize<List<ResourceUsersEditComponent.Entry>>(data);
            if ((from l in list where l.UserId == userId select l).FirstOrDefault() != null)
                throw new Error(this.__ResStr("dupUser", "User {0} has already been added", newUser));
            // render
            ResourceUsersEditComponent.Entry entry = new ResourceUsersEditComponent.Entry(userId, userName);
            return await GridRecordViewAsync(await ResourceUsersEditComponent.GridRecordAsync(fieldPrefix, entry));
        }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ResourceAuthorize(Info.Resource_AllowListOfUserNamesAjax)]
        public async Task<ActionResult> ResourceUsersBrowse_GridData(GridPartialViewData gridPVData) {
            return await GridPartialViewAsync(ResourceUsersEditComponent.GetGridAllUsersModel(), gridPVData);
        }
    }
}
