/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ComponentsHTML.Views {

    /// <summary>
    /// Implements a standard grid record view (one record).
    /// </summary>
    public class GridRecordView : YetaWFView, IYetaWFView<ModuleDefinition, GridRecordData> {

        internal const string ViewName = "GridRecord";

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
        public async Task<string> RenderViewAsync(ModuleDefinition module, GridRecordData model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append(await HtmlHelper.ForDisplayContainerAsync(model, "GridRecord"));

            return hb.ToString();
        }
    }
}
