/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;
using YetaWF.Modules.ComponentsHTML.Controllers;

namespace YetaWF.Modules.ComponentsHTML.Views {

    /// <summary>
    /// Implements a standard tree data portion view.
    /// </summary>
    public class TreePartialDataView : YetaWFView, IYetaWFView<ModuleDefinition, TreePartialData> {

        internal const string ViewName = "TreePartialDataView";

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
        /// Renders the a tree partial view which is just the tree data portion.
        /// </summary>
        /// <param name="module">The module on behalf of which the tree data portion is rendered.</param>
        /// <param name="model">The model being rendered by the tree data portion.</param>
        /// <returns>The HTML representing the tree data portion.</returns>
        public async Task<string> RenderViewAsync(ModuleDefinition module, TreePartialData model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append(await HtmlHelper.ForDisplayContainerAsync(model, "TreePartialData"));

            return hb.ToString();
        }
    }
}
