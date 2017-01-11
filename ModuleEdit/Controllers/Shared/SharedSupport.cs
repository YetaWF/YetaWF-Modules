/* Copyright © 2017 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/ModuleEdit#License */

using System;
using System.Web.Mvc;
using YetaWF.Core.Controllers;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Models;
using YetaWF.Core.Models.Attributes;
using YetaWF.Core.Modules;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ModuleEdit.Controllers {

    public class SharedSupportController : ControllerImpl<YetaWF.Core.Modules.ModuleDefinition> {

        public SharedSupportController() { }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ExcludeDemoMode]
        public ActionResult AddUserToModule(string prefix, int newRecNumber, string newValue, Guid editGuid) {

            if (string.IsNullOrWhiteSpace(newValue))
                throw new Error(this.__ResStr("noParm", "No user name specified"));
            if (editGuid == Guid.Empty)
                throw new InternalError("No module being edited");

            int userId = Resource.ResourceAccess.GetUserId(newValue);
            if (userId == 0)
                throw new Error(this.__ResStr("noUser", "User {0} doesn't exist", newValue));

            // get the type of the grid record from the module being edited by checking its
            // AllowedUsers property [AdditionalMetadata("GridEntry", typeof(GridAllowedUserEntry))]
            ModuleDefinition editMod = ModuleDefinition.Load(editGuid);
            PropertyData propData = ObjectSupport.GetPropertyData(editMod.GetType(), "AllowedUsers");
            Type gridEntryType = propData.GetAdditionalAttributeValue<Type>("GridEntry");

            // add new grid record
            ModuleDefinition.GridAllowedUser userEntry = (ModuleDefinition.GridAllowedUser) Activator.CreateInstance(gridEntryType);
            userEntry.SetUser(userId);
            GridDefinition.GridEntryDefinition gridEntryDef = new GridDefinition.GridEntryDefinition(prefix, newRecNumber, userEntry);
            return GridPartialView(gridEntryDef);
        }
    }
}