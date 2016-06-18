/* Copyright © 2016 Softel vdm, Inc. - http://yetawf.com/Documentation/YetaWF/Identity#License */

using Microsoft.AspNet.Identity;
using YetaWF.Core.Support;
using YetaWF.Modules.Identity.DataProvider;

namespace YetaWF.Modules.Identity.Models {
    public static class Managers {

        private static object _lockObject = new object();

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        // Gets the site specific UserManager instance (permanent)
        public static UserManager<UserDefinition> GetUserManager() {
            lock (_lockObject) {
                // See if we already have it
                UserManager<UserDefinition> userManager;
                if (PermanentManager.TryGetObject<UserManager<UserDefinition>>(out userManager))
                    return userManager;

                // create a new instance
                UserStore userStore = new UserStore(Manager.CurrentSite.Identity, 1.0f);
                PermanentManager.AddObject<UserStore>(userStore);

                userManager = new UserManager<UserDefinition>(userStore);
                PermanentManager.AddObject<UserManager<UserDefinition>>(userManager);
                return userManager;
            }
        }

        // Gets the site specific UserStore instance (permanent)
        public static UserStore GetUserStore() {
            return PermanentManager.GetObject<UserStore>();
        }

        //// Gets the site specific RoleManager instance (permanent)
        //public static RoleManager<RoleDefinition> GetRoleManager() {
        //    lock (_lockObject) {
        //        // See if we already have it
        //        RoleManager<RoleDefinition> roleManager;
        //        if (PermanentManager.TryGetObject<RoleManager<RoleDefinition>>(out roleManager))
        //            return roleManager;

        //        // create a new instance
        //        RoleStore roleStore = new RoleStore(1.0f);
        //        PermanentManager.AddObject<RoleStore>(roleStore);

        //        // get a role manager
        //        roleManager = new RoleManager<RoleDefinition>(roleStore);
        //        PermanentManager.AddObject<RoleManager<RoleDefinition>>(roleManager);
        //        return roleManager;
        //    }
        //}
        //// Gets the site specific RoleStore instance (permanent)
        //public static RoleStore GetRoleStore() {
        //    return PermanentManager.GetObject<RoleStore>();
        //}
    }
}