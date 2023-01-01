/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Identity.Addons {

    public class Info : IAddOnSupport {

        public const string Resource_AllowUserLogon = "YetaWF_Identity-AllowUserLogon";
        public const string Resource_AllowUserIdAjax = "YetaWF_Identity-AllowUserIdAjax";
        public const string Resource_AllowListOfUserNamesAjax = "YetaWF_Identity-AllowListOfUserNamesAjax";

        public static readonly int MAX_USERS_IN_RESOURCE = 10;// maximum allowed users in a resource definition

        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string areaName = AreaRegistration.CurrentPackage.AreaName;

            //scripts.AddConfigOption(areaName, "MaxUsersInResource", MAX_USERS_IN_RESOURCE);

            return Task.CompletedTask;
        }
    }
}
