/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;
#if MVC6
using Microsoft.AspNetCore.Identity;
#else
using Microsoft.AspNet.Identity;
#endif

namespace YetaWF.Modules.Identity.Models {
    public static class Managers {

        private static object _lockObject = new object();

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        // Gets the site specific UserManager instance (permanent)
        public static UserManager<UserDefinition> GetUserManager() {
            return (UserManager<UserDefinition>)YetaWFManager.ServiceProvider.GetService(typeof(UserManager<UserDefinition>));
        }
    }
}
