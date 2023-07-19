using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using $companynamespace$.Modules.$projectnamespace$;

namespace $companynamespace$.Modules.$projectnamespace$.Pages {

    /// <summary>
    /// Implements the Plain page.
    /// </summary>
    public class PlainPage : PlainPageBase {

        /// <summary>
        /// Returns the package implementing the page.
        /// </summary>
        /// <returns>Returns the package implementing the page.</returns>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
    }
}
