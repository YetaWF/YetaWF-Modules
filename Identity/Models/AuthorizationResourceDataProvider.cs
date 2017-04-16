/* Copyright © 2017 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using YetaWF.Core.DataProvider;
using YetaWF.Core.Identity;
using YetaWF.Core.Localize;
using YetaWF.Core.Log;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.Identity.DataProvider {

    public class AuthorizationResourceDataProvider : IInitializeApplicationStartup {

        // STARTUP
        // STARTUP
        // STARTUP

        private static Dictionary<string, ResourceAttribute> AuthorizationResources { get; set; }

        /// <summary>
        /// Visit all known assemblies and collect authorization resources
        /// </summary>
        public void InitializeApplicationStartup() {

            AuthorizationResources = new Dictionary<string, ResourceAttribute>();

            Logging.AddLog("Locating Authorization Resources");
            List<Package> packages = Package.GetAvailablePackages();
            foreach (Package package in packages) {
                foreach (ResourceAttribute attr in package.Resources)
                    AuthorizationResources.Add(attr.Name, attr);
            }
            Logging.AddLog("Completed locating Authorization Resources");
        }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public AuthorizationResourceDataProvider() { }

        // LOAD/SAVE
        // LOAD/SAVE
        // LOAD/SAVE

        public ResourceAttribute GetItem(string resourceName) {
            ResourceAttribute resAttr;
            if (!AuthorizationResources.TryGetValue(resourceName, out resAttr))
                throw new Error(this.__ResStr("noRes", "Authorization resource {0} not found", resourceName));
            return resAttr;
        }
        public List<ResourceAttribute> GetItems() {
            int total;
            List<ResourceAttribute> list = (from d in AuthorizationResources.Values select d).ToList();//copy
            total = list.Count();
            return list;
        }
    }
}
