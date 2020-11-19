/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// This static class implements adding jQuery UI to a page.
    /// </summary>
    public static class JqueryUICore {

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        /// <summary>
        /// Called by components that use jQuery UI to add the basic jQuery UI CSS to the page.
        /// JavaScript files are never loaded and jQuery UI is not used. Only the CSS and the themes are available.
        /// </summary>
        public static async Task UseAsync() {

            if (Manager.IsPostRequest) return;// can't add this while processing a post request

            CoreRendering.ComponentsData cData = CoreRendering.GetComponentsData();
            if (!cData.JqueryUIUsed) {
                cData.JqueryUIUsed = true;

                // Find the jquery theme+
                SkinAccess skinAccess = new SkinAccess();
                string skin = Manager.CurrentPage.jQueryUISkin;
                if (string.IsNullOrWhiteSpace(skin))
                    skin = Manager.CurrentSite.jQueryUISkin;
                string jqueryUIFolder = await skinAccess.FindJQueryUISkinAsync(skin);
                await Manager.AddOnManager.AddAddOnNamedAsync(AreaRegistration.CurrentPackage.AreaName, "jqueryui-themes", jqueryUIFolder);
            }
        }
    }
}