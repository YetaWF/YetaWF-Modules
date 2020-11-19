/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// This static class implements adding Kendo UI and Kendo UI files to a page.
    /// </summary>
    public static class KendoUICore {

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        /// <summary>
        /// Called by components that use Kendo UI to add the basic Kendo UI JavaScript/CSS to the page.
        /// </summary>
        public static async Task UseAsync() {

            if (Manager.IsPostRequest) return;// can't add this while processing a post request

            CoreRendering.ComponentsData cData = CoreRendering.GetComponentsData();
            if (!cData.KendoUIUsed) {
                cData.KendoUIUsed = true;

                // Find Kendo UI theme
                SkinAccess skinAccess = new SkinAccess();
                string skin = Manager.CurrentPage.KendoUISkin;
                if (string.IsNullOrWhiteSpace(skin))
                    skin = Manager.CurrentSite.KendoUISkin;
                string kendoUITheme = await skinAccess.FindKendoUISkinAsync(skin);
                await Manager.AddOnManager.AddAddOnNamedAsync(AreaRegistration.CurrentPackage.AreaName, "telerik.com.Kendo_UI_Core", kendoUITheme);

                Manager.ScriptManager.AddVolatileOption(AreaRegistration.CurrentPackage.AreaName, "kendoUI", true, Replace: true);
            }
        }

        /// <summary>
        /// Adds a named Kendo UI Core file.
        /// </summary>
        /// <param name="file">The name of the Kendo UI file to add.</param>
        public static async Task AddFileAsync(string file) {

            if (Manager.IsPostRequest) return;// can't add this while processing a post request

            await UseAsync();

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