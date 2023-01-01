/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Addons;
using YetaWF.Core.Localize;
using YetaWF.Core.Pages;
using YetaWF.Core.Support;

namespace YetaWF.Modules.ComponentsHTML.Addons.Templates {

    /// <summary>
    /// Implements the YetaWF.Core.Addons.IAddOnSupport interface, which is called when the framework detects the use of the FileUpload1 component type.
    /// The AddSupportAsync method is called so FileUpload1 component specific configuration options and localizations can be added to the page.
    /// </summary>
    public class FileUpload1Edit : IAddOnSupport {

        /// <inheritdoc/>
        public Task AddSupportAsync(YetaWFManager manager) {

            ScriptManager scripts = manager.ScriptManager;
            string area = AreaRegistration.CurrentPackage.AreaName;

            scripts.AddLocalization(area, "Only1FileSupported", this.__ResStr("oneFile", "Please drag one single file at a time - Multiple files are not supported"));

            return Task.CompletedTask;
        }
    }
}
