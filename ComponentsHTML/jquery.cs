/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System;
using System.Threading.Tasks;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Components {

    /// <summary>
    /// This static class implements adding jQuery and its files to a page.
    /// </summary>
    /// <remarks>Very few components still use jQuery, notably FileUpload1. If used, they call UseAsync to include jQuery support files.</remarks>
    public static class jquery {

        private static YetaWFManager Manager { get { return YetaWFManager.Manager; } }

        /// <summary>
        /// Called by components that use jquery to add the basic jquery to the page.
        /// </summary>
        [Obsolete("jquery should not be used any longer")]
        public static async Task UseAsync() {

            if (Manager.IsPostRequest) return;// can't add this while processing a post request

            CoreRendering.ComponentsData cData = CoreRendering.GetComponentsData();
            if (!cData.jqueryUsed) {
                cData.jqueryUsed = true;

                await Manager.AddOnManager.AddAddOnNamedAsync(AreaRegistration.CurrentPackage.AreaName, "jquery");
                Manager.ScriptManager.AddVolatileOption(AreaRegistration.CurrentPackage.AreaName, "jquery", true, Replace: true);
            }
        }
    }
}