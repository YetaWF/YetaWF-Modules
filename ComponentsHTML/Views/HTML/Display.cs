/* Copyright © 2022 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Threading.Tasks;
using YetaWF.Core.Components;
using YetaWF.Core.Localize;
using YetaWF.Core.Modules;
using YetaWF.Core.Packages;
using YetaWF.Core.Support;
using YetaWF.Modules.ComponentsHTML.Components;

namespace YetaWF.Modules.ComponentsHTML.Views {

    /// <summary>
    /// Implements a standard Display view.
    /// </summary>
    /// <remarks>The model is rendered using the PropertyList display component, wrapped in a form with a Close button.</remarks>
    public class DisplayView : YetaWFView, IYetaWFView2<ModuleDefinition, object> {

        internal const string ViewName = ModuleDefinition.StandardViews.Display;

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
        public async Task<string> RenderViewAsync(ModuleDefinition module, object model) {

            HtmlBuilder hb = new HtmlBuilder();

            string? actionName = (string?)HtmlHelper.RouteData.Values["action"];

            hb.Append($@"
{await RenderBeginFormAsync(ActionName: actionName)}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(new FormButton[] {
        new FormButton() { ButtonType = ButtonTypeEnum.Cancel, Text= Manager.IsInPopup ? this.__ResStr("btnClose", "Close") : this.__ResStr("btnReturn", "Return") },
    })}
{await RenderEndFormAsync()}");
            return hb.ToString();
        }

        /// <summary>
        /// Renders the view's partial view.
        /// A partial view is the portion of the view between &lt;form&gt; and &lt;/form&gt; tags.
        /// </summary>
        /// <param name="module">The module on behalf of which the partial view is rendered.</param>
        /// <param name="model">The model being rendered by the partial view.</param>
        /// <returns>The HTML representing the partial view.</returns>
        public async Task<string> RenderPartialViewAsync(ModuleDefinition module, object model) {

            HtmlBuilder hb = new HtmlBuilder();
            hb.Append(await HtmlHelper.ForDisplayContainerAsync(model, "PropertyList"));
            return hb.ToString();

        }
    }
}
