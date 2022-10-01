/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/Identity#License */

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        /// Visit all known assemblies and collect authorization resources.
        /// </summary>
        public Task InitializeApplicationStartupAsync() {

            AuthorizationResources = new Dictionary<string, ResourceAttribute>();

            Logging.AddLog("Locating Authorization Resources");
            List<Package> packages = Package.GetAvailablePackages();
            foreach (Package package in packages) {
                foreach (ResourceAttribute attr in package.Resources)
                    AuthorizationResources.Add(attr.Name, attr);
            }
            Logging.AddLog("Completed locating Authorization Resources");
            return Task.CompletedTask;
        }

        // IMPLEMENTATION
        // IMPLEMENTATION
        // IMPLEMENTATION

        public AuthorizationResourceDataProvider() { }

        // API
        // API
        // API

        public ResourceAttribute GetItem(string resourceName) {
            ResourceAttribute resAttr;
            if (!AuthorizationResources.TryGetValue(resourceName, out resAttr))
                throw new Error(this.__ResStr("noRes", "Authorization resource {0} not found", resourceName));
            return resAttr;
        }
        public DataProviderGetRecords<ResourceAttribute> GetItems() {
            DataProviderGetRecords<ResourceAttribute> recs = new DataProviderGetRecords<ResourceAttribute>();
            recs.Data = (from d in AuthorizationResources.Values select d).ToList();//copy
            recs.Total = recs.Data.Count();
            return recs;
        }
    }
}
