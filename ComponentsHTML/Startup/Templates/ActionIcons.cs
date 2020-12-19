/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    /// <summary>
    /// Implements the YetaWF.Core.Addons.IAddOnSupport interface, which is called when the framework detects the use of the ActionIcons component type.
    /// The AddSupportAsync method is called so ActionIcons component specific configuration options and localizations can be added to the page.
    /// </summary>
    public class ActionIcons : IAddOnSupport {

        internal const string CssActionIcons = "yActionIcons"; // action icons

        /// <summary>
        /// Called by the framework so the component can add component specific client-side configuration options and localizations to the page.
        /// </summary>
        /// <param name="manager">The YetaWF.Core.Support.Manager instance of current HTTP request.</param>
        public Task AddSupportAsync(YetaWFManager manager) {
            return Task.CompletedTask;
        }
    }
}
