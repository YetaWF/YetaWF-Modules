/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

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

#if MVC6
            return (UserManager<UserDefinition>)YetaWFManager.ServiceProvider.GetService(typeof(UserManager<UserDefinition>));
#else

            // See if we already have it
            UserManager<UserDefinition> userManager;
            if (PermanentManager.TryGetObject<UserManager<UserDefinition>>(out userManager))
                return userManager;

            lock (_lockObject) { // lock used to insure we only get one user store (during startup)

                if (PermanentManager.TryGetObject<UserManager<UserDefinition>>(out userManager))
                    return userManager;

                // create a new instance
                UserStore userStore = new UserStore(Manager.CurrentSite.Identity, 1.0f);
                PermanentManager.AddObject<UserStore>(userStore);
                userManager = new UserManager<UserDefinition>(userStore);
                userManager.UserValidator = new UserValidator<UserDefinition>(userManager) { AllowOnlyAlphanumericUserNames = false };

                PermanentManager.AddObject<UserManager<UserDefinition>>(userManager);
                return userManager;
            }
#endif
        }
    }
}
