/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    /// <summary>
    /// Implements the YetaWF.Core.Addons.IAddOnSupport interface, which is called when the framework detects the use of the Text component type.
    /// The AddSupportAsync method is called so Text component specific configuration options and localizations can be added to the page.
    /// </summary>
    public class TextAreaSourceOnlyBoth : IAddOnSupport {

        /// <summary>
        /// Called by the framework so the component can add component specific client-side configuration options and localizations to the page.
        /// </summary>
        /// <param name="manager">The YetaWF.Core.Support.Manager instance of current HTTP request.</param>
        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;

            string area = AreaRegistration.CurrentPackage.AreaName;
            scripts.AddLocalization(area, "CopyToClip", this.__ResStr("copyToClip", "Copied to clipboard"));

            return Task.CompletedTask;
        }
    }
}
