/* Copyright © 2021 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Models;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ComponentsHTML.Views {

    /// <summary>
    /// Implements a standard grid data portion view.
    /// </summary>
    public class GridPartialDataView : YetaWFView, IYetaWFView<ModuleDefinition, GridPartialData> {

        internal const string ViewName = "GridPartialDataView";

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
        /// Renders the a grid partial view which is just the grid data portion.
        /// </summary>
        /// <param name="module">The module on behalf of which the grid data portion is rendered.</param>
        /// <param name="model">The model being rendered by the grid data portion.</param>
        /// <returns>The HTML representing the grid data portion.</returns>
        public async Task<string> RenderViewAsync(ModuleDefinition module, GridPartialData model) {

            // save settings
            await GridLoadSave.SaveSettingsAsync(model);

            // handle async properties
            await HandlePropertiesAsync(model.Data.Data);

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append(await HtmlHelper.ForDisplayContainerAsync(model, "GridPartialData"));

            return hb.ToString();
        }
        internal static async Task HandlePropertiesAsync(List<object> data) {
            foreach (object item in data)
                await HandlePropertiesAsync(item);
        }
        internal static Task HandlePropertiesAsync(object data) {
            return ObjectSupport.HandlePropertyAsync<List<ModuleAction>>("Commands", "__GetCommandsAsync", data);
        }
    }
}
