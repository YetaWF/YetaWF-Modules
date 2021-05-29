/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Packages;
using YetaWF.Core.Pages;
using YetaWF.Core.Skins;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    /// <summary>
    /// Implements the YetaWF.Core.Addons.IAddOnSupport interface, which is called when the framework detects the use of the Number component type.
    /// The AddSupportAsync method is called so Number component specific configuration options and localizations can be added to the page.
    /// </summary>
    public class NumberEdit : IAddOnSupport {

        /// <summary>
        /// Called by the framework so the component can add component specific client-side configuration options and localizations to the page.
        /// </summary>
        /// <param name="manager">The YetaWF.Core.Support.Manager instance of current HTTP request.</param>
        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            Package package = AreaRegistration.CurrentPackage;
            string areaName = package.AreaName;

            scripts.AddConfigOption(areaName, "SVG_fas_caret_up", SkinSVGs.Get(package, "fas-caret-up"));
            scripts.AddConfigOption(areaName, "SVG_fas_caret_down", SkinSVGs.Get(package, "fas-caret-down"));
            scripts.AddConfigOption(areaName, "SVG_fas_exclamation_triangle", SkinSVGs.Get(package, "fas-exclamation-triangle"));

            return Task.CompletedTask;
        }
    }
}
