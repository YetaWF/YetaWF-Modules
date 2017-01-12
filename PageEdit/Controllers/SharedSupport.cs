/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/PageEdit#License */

using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Support;
using YetaWF.Modules.PageEdit.Views.Shared;

namespace YetaWF.Modules.PageEdit.Controllers {

    public class SharedSupportController : ControllerImpl<YetaWF.Core.Modules.ModuleDefinition> {

        public SharedSupportController() { }

        [HttpPost]
        [ConditionalAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult AddUserToPage(string prefix, int newRecNumber, string newValue) {

            if (string.IsNullOrWhiteSpace(newValue))
                throw new Error(this.__ResStr("noParm", "No user name specified."));

            int userId = Resource.ResourceAccess.GetUserId(newValue);
            if (userId == 0)
                throw new Error(this.__ResStr("noUser", "User {0} doesn't exist", newValue));

            UsersHelper.GridAllowedUser userEntry = new UsersHelper.GridAllowedUser(userId);
            GridDefinition.GridEntryDefinition gridEntryDef = new GridDefinition.GridEntryDefinition(prefix, newRecNumber, userEntry);
            return GridPartialView(gridEntryDef);
        }
    }
}