using System.Threading.Tasks;
using YetaWF.Core.Support;
using YetaWF.Modules.Caching.Controllers;
using YetaWF.Modules.Caching.DataProvider;

namespace YetaWF.Modules.Caching.Startup {

    public class Application : IInitializeApplicationStartup {

        public const string Distributed = "Distributed";

        public Task InitializeApplicationStartupAsync(bool firstNode) {

            // permanently created dataproviders (never disposed)
            bool distributed = WebConfigHelper.GetValue<bool>(AreaRegistration.CurrentPackage.AreaName, Distributed);

            if (distributed) {
                // distributed caching uses local and shared cache
                YetaWF.Core.IO.Caching.LocalCacheProvider = new LocalCacheObjectDataProvider();
                YetaWF.Core.IO.Caching.SharedCacheProvider = new SharedCacheObjectDataProvider();
                YetaWF.Core.IO.Caching.StaticCacheProvider = new StaticSharedCacheObjectDataProvider();
                YetaWF.Core.IO.Caching.MultiInstance = true;
            } else {
                // non-distributed caching uses local cache only
                YetaWF.Core.IO.Caching.LocalCacheProvider = new LocalCacheObjectDataProvider();
                YetaWF.Core.IO.Caching.SharedCacheProvider = new LocalCacheObjectDataProvider();
                YetaWF.Core.IO.Caching.StaticCacheProvider = new StaticSingleCacheObjectDataProvider();
                YetaWF.Core.IO.Caching.MultiInstance = false;
            }
            return Task.CompletedTask;
        }
    }
}
