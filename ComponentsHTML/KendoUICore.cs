/* Copyright © 2019 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// This static class implements adding Kendo UI files to a page.
    /// </summary>
    public static class KendoUICore {

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        /// <summary>
        /// Adds a named Kendo UI Core file.
        /// </summary>
        /// <param name="file">The name of the Kendo UI file to add.</param>
        public static async Task AddFileAsync(string file) {
            if (Manager.IsPostRequest) return;// can't add this while processing a post request
            if (Manager.CurrentSite.CanUseCDNComponents) return;// already included
            if (_kendoAddon == null)
                _kendoAddon = VersionManager.FindAddOnNamedVersion(AreaRegistration.CurrentPackage.AreaName, "telerik.com.Kendo_UI_Core");
            string productJsUrl = _kendoAddon.GetAddOnJsUrl();
            string url = productJsUrl + file;
            await Manager.ScriptManager.AddAsync(url);
        }
        private static VersionManager.AddOnProduct _kendoAddon = null;
    }
}