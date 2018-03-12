/* Copyright © 2018 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Views.Shared;
#if MVC6
using Microsoft.AspNetCore.Mvc;
#else
using System.Web.Mvc;
#endif

namespace YetaWF.Modules.Identity.Controllers {

    public class SharedSupportController : ControllerImpl<YetaWF.Core.Modules.ModuleDefinition> {

        public SharedSupportController() { }

        [AllowPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public async Task<ActionResult> AddUserToResource(string prefix, int newRecNumber, string newValue) {

            if (string.IsNullOrWhiteSpace(newValue))
                throw new Error(this.__ResStr("noParm", "No user name specified"));

            int userId = await Resource.ResourceAccess.GetUserIdAsync(newValue);
            if (userId == 0)
                throw new Error(this.__ResStr("noUser", "User {0} doesn't exist.", newValue));

            string userName = await Resource.ResourceAccess.GetUserNameAsync(userId);
            UsersHelper.GridAllowedUser userEntry = new UsersHelper.GridAllowedUser(userId, userName);
            GridDefinition.GridEntryDefinition gridEntryDef = new GridDefinition.GridEntryDefinition(prefix, newRecNumber, userEntry);
            return await GridPartialViewAsync(gridEntryDef);
        }
    }
}