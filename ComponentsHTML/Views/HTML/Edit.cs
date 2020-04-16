/* Copyright © 2020 Softel vdm, Inc. - https://yetawf.com/Documentation/YetaWF/ComponentsHTML#License */

using System.Collections.Generic;
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
    /// Implements a standard EditApply view.
    /// </summary>
    /// <remarks>The model is rendered using the PropertyList edit component, wrapped in a form with Apply/Submit/Cancel form buttons.</remarks>
    public class EditApplyView : EditView {

        internal new const string ViewName = ModuleDefinition.StandardViews.EditApply;

        /// <summary>
        /// Returns the name of the view.
        /// </summary>
        /// <returns>Returns the name of the view.</returns>
        public override string GetViewName() { return ViewName; }
    }

    /// <summary>
    /// Implements a standard Edit view.
    /// </summary>
    /// <remarks>The model is rendered using the PropertyList edit component, wrapped in a form with Submit/Cancel form buttons.</remarks>
    public class EditView : YetaWFView, IYetaWFView2<ModuleDefinition, object> {

        internal const string ViewName = ModuleDefinition.StandardViews.Edit;

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

            string apply = null, applyTT = null; bool applyShown;
            if (ObjectSupport.TryGetPropertyValue<bool>(model, "__applyShown", out applyShown, GetViewName() == EditApplyView.ViewName) && applyShown) {
                ObjectSupport.TryGetPropertyValue<string>(model, "__applyTT", out applyTT);
                ObjectSupport.TryGetPropertyValue<string>(model, "__apply", out apply);
            }
            string submit = null, submitTT = null; bool submitShown;  ButtonTypeEnum submitType = ButtonTypeEnum.Submit; string submitName = null;
            if (ObjectSupport.TryGetPropertyValue<bool>(model, "__submitShown", out submitShown, true) && submitShown) {
                ObjectSupport.TryGetPropertyValue<string>(model, "__submitTT", out submitTT);
                ObjectSupport.TryGetPropertyValue<string>(model, "__submit", out submit);
                ObjectSupport.TryGetPropertyValue<ButtonTypeEnum>(model, "__submitType", out submitType, ButtonTypeEnum.Submit);
                ObjectSupport.TryGetPropertyValue<string>(model, "__submitName", out submitName);
            }
            string cancel = null, cancelTT = null; bool cancelShown; ButtonTypeEnum cancelType = ButtonTypeEnum.Cancel;
            if (ObjectSupport.TryGetPropertyValue<bool>(model, "__cancelShown", out cancelShown, true) && cancelShown) {
                ObjectSupport.TryGetPropertyValue<string>(model, "__cancelTT", out cancelTT);
                ObjectSupport.TryGetPropertyValue<string>(model, "__cancel", out cancel);
                ObjectSupport.TryGetPropertyValue<ButtonTypeEnum>(model, "__cancelType", out cancelType, ButtonTypeEnum.Cancel);
            }

            string actionName = (string)HtmlHelper.RouteData.Values["action"];

            List<FormButton> buttons = new List<FormButton>();
            if (applyShown)
                buttons.Add(new FormButton() { ButtonType = ButtonTypeEnum.Apply, Text = apply, Title = applyTT,  });
            if (submitShown)
                buttons.Add(new FormButton() { ButtonType = submitType, Text = submit, Title = submitTT, Name = submitName });
            if (cancelShown)
                buttons.Add(new FormButton() { ButtonType = cancelType, Text = cancel, Title = cancelTT });

            hb.Append($@"
{await RenderBeginFormAsync(ActionName: actionName)}
    {await PartialForm(async () => await RenderPartialViewAsync(module, model))}
    {await FormButtonsAsync(buttons)}
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
            hb.Append(await HtmlHelper.ForEditContainerAsync(model, "PropertyList"));
            return hb.ToString();

        }
    }
}
