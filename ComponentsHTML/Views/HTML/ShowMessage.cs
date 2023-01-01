/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ComponentsHTML.Views {

    /// <summary>
    /// Implements a standard ShowMessage view.
    /// </summary>
    /// <remarks>The model (a message) is rendered as an alert div.</remarks>
    public class ShowMessageView : YetaWFView, IYetaWFView<ModuleDefinition, object> {

    internal const string ViewName = "ShowMessage";

        /// <summary>
        /// Returns the package implementing the view.
        /// </summary>
        /// <returns>Returns the package implementing the view.</returns>
        public override Package GetPackage() { return AreaRegistration.CurrentPackage; }
        /// <summary>
        /// Returns the name of the view.
        /// </summary>
        /// <returns>Returns the name of the view.</returns>
        public override string GetViewName() { return ViewName; }

        /// <summary>
        /// Renders the view.
        /// </summary>
        /// <param name="module">The module on behalf of which the view is rendered.</param>
        /// <param name="model">The model being rendered by the view.</param>
        /// <returns>The HTML representing the view.</returns>
        public Task<string> RenderViewAsync(ModuleDefinition module, object? model) {

            HtmlBuilder hb = new HtmlBuilder();

            string message = model != null ? model.ToString()! : string.Empty;

            if (!string.IsNullOrWhiteSpace(message)) {
                 hb.Append($@"
<div class='{Utility.HE(Globals.CssDivAlert)}'>
    {Utility.HE(message)}
</div>");
            }
            return Task.FromResult(hb.ToString());
        }
    }
}
