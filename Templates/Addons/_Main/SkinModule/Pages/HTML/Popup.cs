using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Core.Skins;
using $companynamespace$.Modules.$projectnamespace$;

namespace $companynamespace$.Modules.$projectnamespace$.Pages {

    /// <summary>
    /// Implements the Popup page.
    /// </summary>
    public class PopupPage : PopupPageBase {
        /// <summary>
        /// Returns the package implementing the page.
        /// </summary>
        /// <returns>Returns the package implementing the page.</returns>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the name of the page.
        /// </summary>
        /// <returns>Returns the name of the page.</returns>
        public override string GetPageName() { return SkinAccess.POPUP_VIEW_DEFAULT; }
    }
    /// <summary>
    /// Implements the PopupSmall page.
    /// </summary>
    public class PopupSmallPage : PopupPageBase {
        /// <summary>
        /// Returns the package implementing the page.
        /// </summary>
        /// <returns>Returns the package implementing the page.</returns>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the name of the page.
        /// </summary>
        /// <returns>Returns the name of the page.</returns>
        public override string GetPageName() { return SkinAccess.POPUP_VIEW_SMALL; }
    }
    /// <summary>
    /// Implements the PopupMedium page.
    /// </summary>
    public class PopupMediumPage : PopupPageBase {
        /// <summary>
        /// Returns the package implementing the page.
        /// </summary>
        /// <returns>Returns the package implementing the page.</returns>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the name of the page.
        /// </summary>
        /// <returns>Returns the name of the page.</returns>
        public override string GetPageName() { return SkinAccess.POPUP_VIEW_MEDIUM; }
    }
}
