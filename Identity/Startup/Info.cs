/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.Controllers;

namespace YetaWF.Modules.Identity.Addons {

    public class Info : IAddOnSupport {

        public const string Resource_AllowUserLogon = "YetaWF_Identity-AllowUserLogon";

        public static readonly int MAX_USERS_IN_RESOURCE = 10;// maximum allowed users in a resource definition

        public void AddSupport(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            //scripts.AddConfigOption(areaName, "MaxUsersInResource", MAX_USERS_IN_RESOURCE);
        }
    }
}