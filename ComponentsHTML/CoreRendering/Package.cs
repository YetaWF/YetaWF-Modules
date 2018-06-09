using YetaWF.Core.Packages;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML {

    public partial class CoreRendering {

        public Package GetImplementingPackage() {
            return AreaRegistration.CurrentPackage;
        }
    }
}
