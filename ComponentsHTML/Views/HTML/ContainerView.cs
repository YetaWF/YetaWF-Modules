/* Copyright © 2023 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ComponentsHTML.Views {

    /// <summary>
    /// Implements a view that renders a container.
    /// </summary>
    public class ContainerView : YetaWFView, IYetaWFView<ModuleDefinition, ContainerDataContainer.ContainerData> {

        /// <summary>
        /// Defines the name of the view.
        /// </summary>
        public const string ViewName = "ContainerView";

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
        /// Renders the a partial view which is just the component.
        /// </summary>
        /// <param name="module">The module on behalf of which the tree data portion is rendered.</param>
        /// <param name="model">The model being rendered by the tree data portion.</param>
        /// <returns>The HTML representing the tree data portion.</returns>
        public async Task<string> RenderViewAsync(ModuleDefinition module, ContainerDataContainer.ContainerData model) {

            HtmlBuilder hb = new HtmlBuilder();

            hb.Append(await HtmlHelper.ForDisplayContainerAsync(model, ContainerDataContainer.TemplateName));

            return hb.ToString();
        }
    }
}
